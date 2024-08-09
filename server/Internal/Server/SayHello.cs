using Grpc.Core;
using System.Threading.Tasks;
using ServiceHub.MyGreeter;
using Serilog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AKSMiddleware;

namespace Server;

public partial class GeneratedServer
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        string reqJson = JsonConvert.SerializeObject(request);
        Log.Logger.WithCtx(context).Information($"API handler logger output. req: {reqJson}");

        return Task.FromResult(new HelloReply
        {
            Message = "Echo back what you sent me (SayHello): " + request.Name + " " + request.Age.ToString() + " " + request.Email
        });
    }
}