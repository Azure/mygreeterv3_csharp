using Grpc.Core;
using System.Threading.Tasks;
using ServiceHub.MyGreeter;
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
    public override async Task<Empty> DeleteResourceGroup(DeleteResourceGroupRequest request, ServerCallContext context)
    {
        var logger = Log.Logger.WithCtx(context);

        if (_resourceGroups == null)
        {
            logger.Error("ResourceGroupClient is nil in DeleteResourceGroup(), azuresdk feature is likely disabled");
            throw new RpcException(new Status(StatusCode.Unavailable, "ResourceGroupClient is nil, azuresdk feature is likely disabled"));
        }

        try
        {
            var resourceGroup = await _resourceGroups.GetAsync(request.Name);
            await resourceGroup.Value.DeleteAsync(WaitUntil.Completed);

            logger.Information("Deleted resource group: {ResourceName}", request.Name);
        }
        catch (RequestFailedException ex)
        {
            logger.Error(ex, "DeleteAsync() error: {ErrorMessage}", ex.Message);
            throw Server.HandleError(ex, "DeleteAsync");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An unexpected error occurred: {ErrorMessage}", ex.Message);
            throw Server.HandleError(ex, "DeleteResourceGroup");
        }

        return new Empty();
    }
}

