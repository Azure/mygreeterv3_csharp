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
    public override async Task<ListResourceGroupResponse> ListResourceGroups(Empty request, ServerCallContext context)
    {
        if (_resourceGroups == null)
        {
            Log.Logger.WithCtx(context).Warning("ResourceGroupClient is nil in ListResourceGroups(), azuresdk feature is likely disabled");
            throw new RpcException(new Status(StatusCode.Unimplemented, "ResourceGroupClient is nil in ListResourceGroups(), azuresdk feature is likely disabled"));
        }

        var resourceGroupList = new List<ResourceGroup>();

        try
        {
            await foreach (var resourceGroup in _resourceGroups.GetAllAsync())
            {
                var resourceGroupProto = new ResourceGroup
                {
                    Id = resourceGroup.Id.ToString(),
                    Name = resourceGroup.Data.Name,
                    Location = resourceGroup.Data.Location
                };
                resourceGroupList.Add(resourceGroupProto);
            }

            Log.Logger.WithCtx(context).Information("Resource groups found: {Count}", resourceGroupList.Count);
        }
        catch (Exception ex)
        {
            var grpcError = Server.HandleError(ex, "ListResourceGroups");
            Log.Logger.WithCtx(context).Error(grpcError, "Error occurred during resource group listing");
            throw grpcError;
        }

        return new ListResourceGroupResponse
        {
            RgList = { resourceGroupList }
        };
    }
}