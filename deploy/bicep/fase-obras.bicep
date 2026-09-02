// =============================================================================
// Plataforma Civil e Inmobiliaria — Portal de avance de obra: evidencia
//
// Template independiente y complementario a main.bicep (mismo criterio que
// fase2-mensajeria.bicep y fase4-propiedades.bicep): aprovisiona SOLO el
// contenedor Blob nuevo para fotos de evidencia de hitos de obra, en la
// MISMA Storage Account que ya existe desde Fase 2 — no crea una cuenta
// nueva. Deliberadamente sin el parámetro sqlAdministratorLoginPassword,
// para poder desplegarse sin esa credencial.
//
// El App Service ya tiene el rol "Storage Blob Data Contributor" asignado a
// nivel de toda la Storage Account (ver fase4-propiedades.bicep) — cubre
// también este contenedor nuevo, no hace falta repetir la asignación.
//
// El contenedor se crea con acceso público de lectura a nivel de blob
// (publicAccess: 'Blob'), mismo criterio ya aceptado para
// propiedades-multimedia: se sirve directo en <img> sin SAS token. El
// nombre del blob es un GUID (tan inadivinable como el propio token de
// acceso al proyecto), así que no supone una fuga real.
//
// Despliegue:
//   az deployment group create \
//     --resource-group <rg> \
//     --template-file deploy/bicep/fase-obras.bicep
// =============================================================================

var uniqueSuffix = uniqueString(resourceGroup().id)
var storageAccountName = toLower('st${uniqueSuffix}')
var containerName = 'obras-evidencia'

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-01-01' existing = {
  parent: storageAccount
  name: 'default'
}

resource obrasContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  parent: blobService
  name: containerName
  properties: {
    publicAccess: 'Blob'
  }
}

output blobServiceUri string = storageAccount.properties.primaryEndpoints.blob
output containerName string = containerName
