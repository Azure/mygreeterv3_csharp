using Grpc.Core;
using System.Threading.Tasks;

namespace Greet.Services;

public partial class GreeterService
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Echo back what you sent me (SayHello): " + request.Name + " " + request.Age.ToString() + " " + request.Email
        });
    }
}