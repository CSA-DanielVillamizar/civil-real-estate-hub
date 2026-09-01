// =============================================================================
// Plataforma Civil e Inmobiliaria — Infraestructura FinOps (Fase 8)
//
// Objetivo: arquitectura 100% Serverless/PaaS orientada a costo mensual ~$0,
// usando las capas gratuitas de Azure. Sin VNets, sin Private Link.
//
// Recursos:
//   1. Azure Static Web App (SKU Free)      — frontend React
//   2. App Service Plan + App Service (F1)  — backend .NET 8, Managed Identity
//   3. Azure SQL Server + Database          — sin acceso público general,
//                                              regla de firewall solo para
//                                              servicios de Azure
//   4. Key Vault (Standard, RBAC)           — "Key Vault Secrets User" para
//                                              la Managed Identity del App Service
//   5. Storage Account (colas)              — Fase 2: LeadCaptadoEvent async,
//                                              sin claves de cuenta (RBAC)
//   6. Communication Services + Email       — Fase 2: correo de bienvenida,
//      (dominio administrado por Azure)       dominio administrado por Azure
//   7. Storage Blob container (multimedia)  — Catálogo de propiedades:
//                                              fotos/planos, acceso público
//                                              de lectura, misma cuenta de 5.
//
// Despliegue:
//   az deployment group create \
//     --resource-group <rg> \
//     --template-file deploy/bicep/main.bicep \
//     --parameters sqlAdministratorLoginPassword=<password-seguro>
// =============================================================================

@description('Prefijo corto usado para nombrar los recursos (minúsculas, sin espacios).')
param namePrefix string = 'plataformacivil'

@description('Región de despliegue. Free tier de Static Web Apps solo está disponible en un subconjunto de regiones — verificar disponibilidad vigente antes de desplegar.')
param location string = 'centralus'

@description('Login administrador del servidor SQL.')
param sqlAdministratorLogin string = 'plataformaadmin'

@secure()
@description('Password del administrador SQL — provéalo en el despliegue, nunca lo hardcodee ni lo commitee.')
param sqlAdministratorLoginPassword string

@description('Nombre de la base de datos.')
param sqlDatabaseName string = 'plataforma_civil_inmobiliaria'

@description('URL de un webhook entrante (Slack/Teams/endpoint propio) para alertar al equipo comercial. Se puede dejar vacío y configurarlo después como App Setting.')
param notificacionesWebhookUrl string = ''

var uniqueSuffix = uniqueString(resourceGroup().id)
var staticWebAppName = '${namePrefix}-web-${uniqueSuffix}'
var appServicePlanName = '${namePrefix}-plan-${uniqueSuffix}'
var appServiceName = '${namePrefix}-api-${uniqueSuffix}'
var sqlServerName = '${namePrefix}-sql-${uniqueSuffix}'
// Key Vault exige nombres de máximo 24 caracteres (el más estricto de todos
// los recursos de este template) — se prescinde de namePrefix aquí para que
// nunca dependa de qué tan largo sea el prefijo elegido.
var keyVaultName = 'kv-${uniqueSuffix}'
var sqlConnectionStringSecretName = 'SqlConnectionString'
// Storage Account: 3-24 caracteres, solo minúsculas/números, sin guiones.
var storageAccountName = toLower('st${uniqueSuffix}')
var queueName = 'lead-notifications'
var propiedadesContainerName = 'propiedades-multimedia'
var communicationServiceName = '${namePrefix}-acs-${uniqueSuffix}'
var emailServiceName = '${namePrefix}-email-${uniqueSuffix}'

// Roles built-in de Azure RBAC — GUIDs estables, verificados contra la
// documentación oficial (Key Vault Secrets User, Storage Queue Data
// Contributor) y directamente vía `az role definition list` (Communication
// and Email Service Owner, que no aparece expuesto como GUID en la
// documentación pública — solo por nombre).
var keyVaultSecretsUserRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')
var storageQueueDataContributorRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '974c5e8b-45b9-4653-ba55-5f855dd0fb88')
var communicationEmailServiceOwnerRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '09976791-48a7-449e-bb21-39d1a415f350')
var storageBlobDataContributorRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')

// -----------------------------------------------------------------------
// 1) Azure Static Web App (Free) — frontend React
// -----------------------------------------------------------------------
resource staticWebApp 'Microsoft.Web/staticSites@2023-12-01' = {
  name: staticWebAppName
  location: location
  sku: {
    name: 'Free'
    tier: 'Free'
  }
  properties: {
    // Sin integración de repositorio en el recurso: el contenido se publica
    // desde GitHub Actions (frontend-deploy.yml) usando el token de despliegue
    // del recurso, no desde este template.
    buildProperties: {
      appLocation: 'frontend'
      outputLocation: 'dist'
    }
  }
}

// -----------------------------------------------------------------------
// 2) App Service Plan (F1 Free, Linux) + App Service — backend .NET 8
// -----------------------------------------------------------------------
resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  kind: 'linux'
  sku: {
    name: 'F1'
    tier: 'Free'
  }
  properties: {
    reserved: true // requerido para planes Linux
  }
}

resource appService 'Microsoft.Web/sites@2023-12-01' = {
  name: appServiceName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      minTlsVersion: '1.2'
      // F1 (Free) no soporta "Always On" — el app se duerme por inactividad,
      // coherente con el objetivo de costo $0.
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'ConnectionStrings__PlataformaDb'
          value: '@Microsoft.KeyVault(SecretUri=${sqlConnectionStringSecret.properties.secretUri})'
        }
        {
          name: 'Messaging__StorageQueueUri'
          value: storageAccount.properties.primaryEndpoints.queue
        }
        {
          name: 'Messaging__QueueName'
          value: queueName
        }
        {
          name: 'Notifications__CommunicationServicesEndpoint'
          value: 'https://${communicationService.properties.hostName}'
        }
        {
          name: 'Notifications__EmailFromAddress'
          value: 'DoNotReply@${emailDomain.properties.mailFromSenderDomain}'
        }
        {
          name: 'Notifications__WebhookUrl'
          value: notificacionesWebhookUrl
        }
        {
          name: 'Properties__BlobServiceUri'
          value: storageAccount.properties.primaryEndpoints.blob
        }
        {
          name: 'Properties__ContainerName'
          value: propiedadesContainerName
        }
      ]
    }
  }
}

// -----------------------------------------------------------------------
// 3) Azure SQL Server + Database
//    Sin VNet/Private Link: acceso público del servidor deshabilitado a
//    cualquier IP salvo la regla especial "AllowAllWindowsAzureIps"
//    (0.0.0.0-0.0.0.0), que permite solo tráfico interno de servicios Azure
//    (como este App Service) — no abre el servidor a Internet en general.
// -----------------------------------------------------------------------
resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdministratorLogin
    administratorLoginPassword: sqlAdministratorLoginPassword
    publicNetworkAccess: 'Enabled' // necesario para que la regla de firewall aplique; sin esto no hay forma de conectar sin Private Link
    minimalTlsVersion: '1.2'
  }
}

resource sqlFirewallAllowAzureServices 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Serverless con auto-pause: minimiza el costo a casi $0 cuando no hay uso
// (solo se cobra el almacenamiho mientras está pausada, no el cómputo).
//
// NOTA FINOPS: Azure también ofrece un nivel "Free" de SQL Database (créditos
// mensuales de vCore-segundos + almacenamiento, pensado exactamente para
// escenarios de costo $0). No lo usé aquí porque su forma exacta en Bicep
// (useFreeLimit / freeLimitExhaustionBehavior) es una función relativamente
// nueva y prefiero no adivinar su esquema en un template que aprovisiona
// infraestructura real y facturable — Serverless + auto-pause es el patrón
// estable y ampliamente documentado. Vale la pena evaluar el nivel Free como
// siguiente optimización, confirmando su sintaxis vigente en la documentación
// de Azure antes de adoptarlo.
resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  sku: {
    name: 'GP_S_Gen5'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 1
  }
  properties: {
    autoPauseDelay: 60 // minutos de inactividad antes de pausar (mínimo permitido)
    minCapacity: json('0.5')
    zoneRedundant: false
  }
}

// -----------------------------------------------------------------------
// 4) Key Vault (Standard, RBAC) — guarda la cadena de conexión SQL
// -----------------------------------------------------------------------
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true // RBAC en vez de access policies clásicas, como pediste
    enabledForTemplateDeployment: true
  }
}

resource sqlConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: sqlConnectionStringSecretName
  properties: {
    value: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorLoginPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
  }
}

// Otorga a la Managed Identity del App Service el rol "Key Vault Secrets
// User" — puede leer secretos, no administrarlos (mínimo privilegio).
resource appServiceKeyVaultAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, appService.id, keyVaultSecretsUserRoleId)
  scope: keyVault
  properties: {
    roleDefinitionId: keyVaultSecretsUserRoleId
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// -----------------------------------------------------------------------
// 5) Storage Account (colas) — Fase 2: LeadCaptadoEvent asíncrono
//    Sin claves de cuenta: allowSharedKeyAccess=false fuerza autenticación
//    vía Microsoft Entra ID (Managed Identity), coherente con Zero Trust.
// -----------------------------------------------------------------------
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    minimumTlsVersion: 'TLS1_2'
    allowSharedKeyAccess: false
    allowBlobPublicAccess: false
  }
}

resource queueService 'Microsoft.Storage/storageAccounts/queueServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
}

resource leadNotificationsQueue 'Microsoft.Storage/storageAccounts/queueServices/queues@2023-01-01' = {
  parent: queueService
  name: queueName
}

// Otorga a la Managed Identity del App Service el rol "Storage Queue Data
// Contributor" — puede enviar/leer/eliminar mensajes, no administrar la
// cuenta (mínimo privilegio).
resource appServiceStorageQueueAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, appService.id, storageQueueDataContributorRoleId)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageQueueDataContributorRoleId
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// -----------------------------------------------------------------------
// 6) Communication Services + Email (dominio administrado por Azure) —
//    Fase 2: correo de bienvenida al lead.
//    Recursos globales (no aceptan `location` regional); el dominio
//    "AzureManaged" evita el proceso de verificación DNS de un dominio
//    propio, a costa de enviar desde una dirección @<guid>.azurecomm.net.
// -----------------------------------------------------------------------
resource emailService 'Microsoft.Communication/emailServices@2023-04-01' = {
  name: emailServiceName
  location: 'global'
  properties: {
    dataLocation: 'United States'
  }
}

resource emailDomain 'Microsoft.Communication/emailServices/domains@2023-04-01' = {
  parent: emailService
  name: 'AzureManagedDomain'
  location: 'global'
  properties: {
    domainManagement: 'AzureManaged'
  }
}

resource communicationService 'Microsoft.Communication/communicationServices@2023-04-01' = {
  name: communicationServiceName
  location: 'global'
  properties: {
    dataLocation: 'United States'
    linkedDomains: [
      emailDomain.id
    ]
  }
}

// Otorga a la Managed Identity del App Service el rol "Communication and
// Email Service Owner" — necesario para enviar correos vía EmailClient con
// DefaultAzureCredential (Zero Trust, sin connection string).
resource appServiceCommunicationAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(communicationService.id, appService.id, communicationEmailServiceOwnerRoleId)
  scope: communicationService
  properties: {
    roleDefinitionId: communicationEmailServiceOwnerRoleId
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// -----------------------------------------------------------------------
// 7) Storage Blob container — catálogo de propiedades (multimedia)
//    Misma Storage Account de la sección 5, un contenedor Blob nuevo con
//    acceso público de lectura a nivel de blob: son fotos de un catálogo
//    inmobiliario público, se sirven directo en <img> sin SAS token.
// -----------------------------------------------------------------------
resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' = {
  parent: storageAccount
  name: 'default'
}

resource propiedadesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: propiedadesContainerName
  properties: {
    publicAccess: 'Blob'
  }
}

// Otorga a la Managed Identity del App Service el rol "Storage Blob Data
// Contributor" — puede leer/escribir/eliminar blobs, no administrar la
// cuenta (mínimo privilegio).
resource appServiceBlobAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, appService.id, storageBlobDataContributorRoleId)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageBlobDataContributorRoleId
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// -----------------------------------------------------------------------
// Outputs
// -----------------------------------------------------------------------
output staticWebAppDefaultHostname string = staticWebApp.properties.defaultHostname
output appServiceDefaultHostname string = appService.properties.defaultHostName
output sqlServerFullyQualifiedDomainName string = sqlServer.properties.fullyQualifiedDomainName
output keyVaultName string = keyVault.name
output storageQueueEndpoint string = storageAccount.properties.primaryEndpoints.queue
output communicationServicesEndpoint string = 'https://${communicationService.properties.hostName}'
output emailFromAddress string = 'DoNotReply@${emailDomain.properties.mailFromSenderDomain}'
output propiedadesBlobServiceUri string = storageAccount.properties.primaryEndpoints.blob
output propiedadesContainerName string = propiedadesContainerName
