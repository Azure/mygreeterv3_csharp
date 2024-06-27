using Grpc.Net.Client;
using Greet;

namespace Greet
{
    public static class Client
    {
        public static MyGreeter.MyGreeterClient NewClient(GrpcChannel channel)
        {
            return new MyGreeter.MyGreeterClient(channel);
        }
    }
}