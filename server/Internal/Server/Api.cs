using Grpc.Core;
using Serilog;
using System;
using ServiceHub.MyGreeter;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Compute;
// using Azure.ResourceManager.Storage;

using AKSMiddleware;

namespace Server;

public partial class GeneratedServer : MyGreeter.MyGreeterBase
{
    private readonly ResourceGroupCollection? _resourceGroups;
    private readonly Serilog.ILogger _logger;

    public GeneratedServer(ServerOptions options, Serilog.ILogger logger)
    {
        _logger = logger;

        if (options.EnableAzureSDKCalls)
        {
            var clientOptions = ArmPolicy.GetDefaultArmClientOptions(_logger);

            TokenCredential credential;
            if (!string.IsNullOrEmpty(options.IdentityResourceId))
            {
                ResourceIdentifier ResourceId = new ResourceIdentifier(options.IdentityResourceId);
                credential = new ManagedIdentityCredential(ResourceId);
            }
            else
            {
                credential = new DefaultAzureCredential();
            }
            try
            {
                var armClient = new ArmClient(credential, options.SubscriptionId, clientOptions);
                SubscriptionResource subscription = armClient.GetSubscriptionResource(new ResourceIdentifier($"/subscriptions/{options.SubscriptionId}"));
                _resourceGroups = subscription.GetResourceGroups();

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}