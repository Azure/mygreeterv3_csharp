using Grpc.Core;
using System.Threading.Tasks;
using Greet.Server;
using Serilog;
using MiddlewareListInterceptors;
using Google.Protobuf.WellKnownTypes;
using Azure.Identity;
using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Compute;

namespace Greet.Services;

public partial class GreeterService
{
    public override async Task<Empty> CreateResourceGroup(CreateResourceGroupRequest request, ServerCallContext context)
    {
        Log.Logger.WithCtx(context).Information("Received a create resource group request! RgName: {RgName}, RgRegion: {RgRegion}", request.Name, request.Region);

        if (_resourceGroups == null)
        {
            Log.Logger.WithCtx(context).Warning("ResourceGroupClient is nil in CreateResourceGroup(), azuresdk feature is likely disabled");
            return new Empty();
            // throw new RpcException(new Status(StatusCode.Unimplemented, "ResourceGroupClient is nil in CreateResourceGroup(), azuresdk feature is likely disabled"));
        }

        try
        {
            ResourceGroupData resourceGroupData = new ResourceGroupData(request.Region);
            ArmOperation<ResourceGroupResource> operation = await _resourceGroups.CreateOrUpdateAsync(WaitUntil.Completed, request.Name, resourceGroupData);
            ResourceGroupResource resourceGroup = operation.Value;

            Log.Logger.WithCtx(context).Information("Created resource group: {ResourceId}", resourceGroup.Id);
        }
        catch (RequestFailedException ex)
        {
            Log.Logger.WithCtx(context).Error(ex, "CreateOrUpdateAsync() error: {ErrorMessage}", ex.Message);
            throw new RpcException(new Status(StatusCode.Internal, "CreateOrUpdateAsync() failed"), ex.Message);
        }
        catch (Exception ex)
        {
            Log.Logger.WithCtx(context).Error(ex, "An unexpected error occurred: {ErrorMessage}", ex.Message);
            throw new RpcException(new Status(StatusCode.Unknown, "An unexpected error occurred"), ex.Message);
        }

        return new Empty();
    }
}