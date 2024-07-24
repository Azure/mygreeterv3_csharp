# Resource Information

## Resource and Resource Type

| Name         | ResourceType |
|:-------------|:-------------|
| [7bc4a598-f505-5a3d-a694-6b02f8c30bbf](https://portal.azure.com/#@microsoft.onmicrosoft.com/resource/subscriptions/8ecadfc9-d1a3-4ea4-b844-0d9f87e4d7c8/providers/Microsoft.Authorization/roleAssignments/7bc4a598-f505-5a3d-a694-6b02f8c30bbf) |  Microsoft.Authorization/roleAssignments |
| [servicehub-mygreeterv3-managedIdentity](https://portal.azure.com/#@microsoft.onmicrosoft.com/resource/subscriptions/8ecadfc9-d1a3-4ea4-b844-0d9f87e4d7c8/resourceGroups/servicehub-tyhmv4nvyg-rg/providers/Microsoft.ManagedIdentity/userAssignedIdentities/servicehub-mygreeterv3-managedIdentity) |  Microsoft.ManagedIdentity/userAssignedIdentities |
| [servicehub-mygreeterv3-fedIdentity](https://portal.azure.com/#@microsoft.onmicrosoft.com/resource/subscriptions/8ecadfc9-d1a3-4ea4-b844-0d9f87e4d7c8/resourceGroups/servicehub-tyhmv4nvyg-rg/providers/Microsoft.ManagedIdentity/userAssignedIdentities/servicehub-mygreeterv3-managedIdentity/federatedIdentityCredentials/servicehub-mygreeterv3-fedIdentity) |  servicehub-mygreeterv3-managedIdentity/federatedIdentityCredentials |


## Deployments and Dependencies

| Name                  | Depends On  |
|:----------------------|:------------|
| servicehub-mygreeterv3-managed-identityDeploy | <ul><li>servicehub-tyhmv4nvyg-clusterDeploy</li><li>servicehub-tyhmv4nvyg-rg</li></ul>  |
| servicehub-mygreeterv3azuresdkraeastusDeploy | <ul><li>servicehub-mygreeterv3-managed-identityDeploy</li><li>servicehub-tyhmv4nvyg-rg</li></ul>  |
