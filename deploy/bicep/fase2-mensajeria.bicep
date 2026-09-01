// =============================================================================
// Plataforma Civil e Inmobiliaria — Fase 2: Infraestructura de Mensajería
//
// Template independiente y complementario a main.bicep: aprovisiona SOLO los
// recursos nuevos de Fase 2 (Storage Account para colas, Communication
// Services + Email) y los enlaza a la Managed Identity del App Service ya
// existente. Deliberadamente NO declara el parámetro sqlAdministratorLoginPassword
// ni toca el servidor/base de datos SQL — permite desplegar esta parte sin
// exponer esa credencial.
//
// Los nombres de recursos usan la misma expresión uniqueString(resourceGroup().id)
// que main.bicep, así que generan exactamente los mismos nombres — este
// despliegue queda absorbido sin drift la próxima vez que se aplique
// main.bicep completo.
//
// Despliegue:
//   az deployment group create \
//     --resource-group <rg> \
//     --template-file deploy/bicep/fase2-mensajeria.bicep
// =============================================================================

@description('Prefijo corto usado para nombrar los recursos (debe coincidir con el usado en main.bicep).')
param namePrefix string = 'plataformacivil'

@description('Región de despliegue (recursos regionales; Communication Services/Email son globales).')
param location string = 'centralus'

var uniqueSuffix = uniqueString(resourceGroup().id)
var appServiceName = '${namePrefix}-api-${uniqueSuffix}'
var storageAccountName = toLower('st${uniqueSuffix}')
var queueName = 'lead-notifications'
var communicationServiceName = '${namePrefix}-acs-${uniqueSuffix}'
var emailServiceName = '${namePrefix}-email-${uniqueSuffix}'

var storageQueueDataContributorRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '974c5e8b-45b9-4653-ba55-5f855dd0fb88')
var communicationEmailServiceOwnerRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '09976791-48a7-449e-bb21-39d1a415f350')

// App Service ya existente (desplegado por main.bicep) — solo se referencia
// para leer su Managed Identity, no se modifica aquí.
resource appService 'Microsoft.Web/sites@2023-12-01' existing = {
  name: appServiceName
}

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

resource appServiceStorageQueueAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, appService.id, storageQueueDataContributorRoleId)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageQueueDataContributorRoleId
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

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

resource appServiceCommunicationAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(communicationService.id, appService.id, communicationEmailServiceOwnerRoleId)
  scope: communicationService
  properties: {
    roleDefinitionId: communicationEmailServiceOwnerRoleId
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

output storageQueueEndpoint string = storageAccount.properties.primaryEndpoints.queue
output queueName string = queueName
output communicationServicesEndpoint string = 'https://${communicationService.properties.hostName}'
output emailFromAddress string = 'DoNotReply@${emailDomain.properties.mailFromSenderDomain}'
