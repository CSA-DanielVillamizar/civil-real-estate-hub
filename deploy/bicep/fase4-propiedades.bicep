// =============================================================================
// Plataforma Civil e Inmobiliaria — Catálogo de Propiedades: multimedia
//
// Template independiente y complementario a main.bicep (mismo criterio que
// fase2-mensajeria.bicep): aprovisiona SOLO el contenedor Blob nuevo para
// fotos/planos/renders de propiedades, en la MISMA Storage Account que ya
// existe desde Fase 2 — no crea una cuenta nueva. Deliberadamente sin el
// parámetro sqlAdministratorLoginPassword, para poder desplegarse sin esa
// credencial.
//
// El contenedor se crea con acceso público de lectura a nivel de blob
// (publicAccess: 'Blob'): son fotos de un catálogo inmobiliario público, se
// sirven directo en <img> sin SAS token — nunca contienen datos sensibles.
//
// Despliegue:
//   az deployment group create \
//     --resource-group <rg> \
//     --template-file deploy/bicep/fase4-propiedades.bicep
// =============================================================================

@description('Prefijo corto usado para nombrar los recursos (debe coincidir con el usado en main.bicep).')
param namePrefix string = 'plataformacivil'

var uniqueSuffix = uniqueString(resourceGroup().id)
var appServiceName = '${namePrefix}-api-${uniqueSuffix}'
var storageAccountName = toLower('st${uniqueSuffix}')
var containerName = 'propiedades-multimedia'

var storageBlobDataContributorRoleId = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')

resource appService 'Microsoft.Web/sites@2023-12-01' existing = {
  name: appServiceName
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' existing = {
  parent: storageAccount
  name: 'default'
}

resource propiedadesContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: containerName
  properties: {
    publicAccess: 'Blob'
  }
}

resource appServiceBlobAccess 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, appService.id, storageBlobDataContributorRoleId)
  scope: storageAccount
  properties: {
    roleDefinitionId: storageBlobDataContributorRoleId
    principalId: appService.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

output blobServiceUri string = storageAccount.properties.primaryEndpoints.blob
output containerName string = containerName
