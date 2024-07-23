using Grpc.Core;
using Greet;
using Greet.Server;

namespace Greet.Services;

public partial class GreeterService : MyGreeter.MyGreeterBase
{

    public GreeterService()
    {

    }
}