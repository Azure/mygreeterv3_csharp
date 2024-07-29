# Resource Information

## Resource and Resource Type

| Name         | ResourceType |
|:-------------|:-------------|
| [4e5c148e-05ed-528d-a941-4b75b0514a93](https://portal.azure.com/#@microsoft.onmicrosoft.com/resource/subscriptions/8ecadfc9-d1a3-4ea4-b844-0d9f87e4d7c8/providers/Microsoft.Authorization/roleAssignments/4e5c148e-05ed-528d-a941-4b75b0514a93) |  Microsoft.Authorization/roleAssignments |
| [servicehub-mygreeterv3-managedIdentity](https://portal.azure.com/#@microsoft.onmicrosoft.com/resource/subscriptions/8ecadfc9-d1a3-4ea4-b844-0d9f87e4d7c8/resourceGroups/servicehub-clagx80475-rg/providers/Microsoft.ManagedIdentity/userAssignedIdentities/servicehub-mygreeterv3-managedIdentity) |  Microsoft.ManagedIdentity/userAssignedIdentities |
| [servicehub-mygreeterv3-fedIdentity](https://portal.azure.com/#@microsoft.onmicrosoft.com/resource/subscriptions/8ecadfc9-d1a3-4ea4-b844-0d9f87e4d7c8/resourceGroups/servicehub-clagx80475-rg/providers/Microsoft.ManagedIdentity/userAssignedIdentities/servicehub-mygreeterv3-managedIdentity/federatedIdentityCredentials/servicehub-mygreeterv3-fedIdentity) |  servicehub-mygreeterv3-managedIdentity/federatedIdentityCredentials |


## Deployments and Dependencies

| Name                  | Depends On  |
|:----------------------|:------------|
| servicehub-mygreeterv3-managed-identityDeploy | <ul><li>servicehub-clagx80475-clusterDeploy</li><li>servicehub-clagx80475-rg</li></ul>  |
| servicehub-mygreeterv3azuresdkraeastusDeploy | <ul><li>servicehub-mygreeterv3-managed-identityDeploy</li><li>servicehub-clagx80475-rg</li></ul>  |
