using Grpc.Core;
using Greet;

namespace Greet.Services;

public partial class GreeterService : MyGreeter.MyGreeterBase
{

    public GreeterService()
    {

    }
}