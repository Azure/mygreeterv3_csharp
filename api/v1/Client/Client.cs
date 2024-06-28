using Grpc.Net.Client;
using Greet;

namespace Greet
{
    public static class Client
    {
        public static MyGreeter.MyGreeterClient NewClient(ClientOptions options)
        {
            var channel = GrpcChannel.ForAddress($"http://{options.RemoteAddr}");
            return new MyGreeter.MyGreeterClient(channel);
        }
    }
}