using Grpc.Core;
using Serilog;
using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Compute;
// using Azure.ResourceManager.Storage;

namespace Greet.Services;

public partial class GreeterService : MyGreeter.MyGreeterBase
{
    private readonly ResourceGroupCollection? _resourceGroups;
    private readonly Serilog.ILogger _logger;

    public GreeterService(ServerOptions options, Serilog.ILogger logger)
    {
        _logger = logger;

        if (options.EnableAzureSDKCalls)
        {
            // TODO: need to create GetDefaultArmClientOptions function (middleware)
            var clientOptions = new ArmClientOptions();

            TokenCredential credential;
            if (!string.IsNullOrEmpty(options.IdentityResourceId))
            {
                // Use Managed Identity for authentication
                ResourceIdentifier ResourceId = new ResourceIdentifier(options.IdentityResourceId);
                credential = new ManagedIdentityCredential(ResourceId);
            }
            else
            {
                // Fallback to DefaultAzureCredential
                credential = new DefaultAzureCredential();
            }

            try
            {
                var armClient = new ArmClient(credential, options.SubscriptionId, clientOptions);

                // Get the subscription resource
                SubscriptionResource subscription = armClient.GetSubscriptionResource(new ResourceIdentifier($"/subscriptions/{options.SubscriptionId}"));

                // Get the ResourceGroupCollection from the subscription
                _resourceGroups = subscription.GetResourceGroups();

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                Environment.Exit(1);
            }
        }
    }
}