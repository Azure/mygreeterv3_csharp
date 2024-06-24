﻿using System.Threading.Tasks;
using Grpc.Net.Client;
using Greet;

// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("http://localhost:50051");
var client = new MyGreeter.MyGreeterClient(channel);
var reply = await client.SayHelloAsync(
                  new HelloRequest { });
Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();