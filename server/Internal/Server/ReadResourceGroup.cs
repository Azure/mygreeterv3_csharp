using Grpc.Core;
using System.Threading.Tasks;
using ServiceHub.MyGreeterCsharp;
using Serilog;
using AKSMiddleware;
using Google.Protobuf.WellKnownTypes;
using Azure.Identity;
using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Compute;

namespace Server;

public partial class GeneratedServer
{
    public override async Task<ReadResourceGroupResponse> ReadResourceGroup(ReadResourceGroupRequest request, ServerCallContext context)
    {
        if (_resourceGroups == null)
        {
            Log.Logger.WithCtx(context).Warning("ResourceGroupClient is nil in ReadResourceGroup(), azuresdk feature is likely disabled");
            throw new RpcException(new Status(StatusCode.Unimplemented, "ResourceGroupClient is nil in ReadResourceGroup(), azuresdk feature is likely disabled"));
        }

        try
        {
            var resourceGroup = await _resourceGroups.GetAsync(request.Name);

            if (resourceGroup == null)
            {
                Log.Logger.WithCtx(context).Warning("Resource group not found: {ResourceName}", request.Name);
                throw new RpcException(new Status(StatusCode.NotFound, $"Resource group '{request.Name}' not found"));
            }

            var readResourceGroup = new ResourceGroup
            {
                Id = resourceGroup.Value.Data.Id.ToString(),
                Name = resourceGroup.Value.Data.Name,
                Location = resourceGroup.Value.Data.Location
            };

            Log.Logger.WithCtx(context).Information("Read resource group: {ResourceName} in {Location}", readResourceGroup.Name, readResourceGroup.Location);

            return new ReadResourceGroupResponse { ResourceGroup = readResourceGroup };
        }
        catch (RequestFailedException ex)
        {
            Log.Logger.WithCtx(context).Error(ex, "GetAsync() error: {ErrorMessage}", ex.Message);
            throw Server.HandleError(ex, "GetAsync");
        }
        catch (Exception ex)
        {
            Log.Logger.WithCtx(context).Error(ex, "An unexpected error occurred: {ErrorMessage}", ex.Message);
            throw Server.HandleError(ex, "ReadResourceGroup");
        }
    }
}
