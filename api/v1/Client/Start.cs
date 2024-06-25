using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;

using Greet;
using Grpc.Net.Client;

public class ClientOptions
{
    public string? RemoteAddr { get; set; }
    public string? HttpAddr { get; set; }
    public bool JsonLog { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public long IntervalMilliSec { get; set; }
}


public static class StartCommand
{
    public static Command Execute()
    {
        var remoteAddrOption = new Option<string>(
            "--remote-addr",
            description: "The remote server's addr for this client to connect to",
            getDefaultValue: () => "localhost:50051");

        var jsonLogOption = new Option<bool>(
            "--json-log",
            description: "The format of the log is json or user friendly key-value pairs",
            getDefaultValue: () => false);

        var nameOption = new Option<string>(
            "--name",
            description: "The name to send in Hello request",
            getDefaultValue: () => "MyName");

        var ageOption = new Option<int>(
            "--age",
            description: "The age to send in Hello request",
            getDefaultValue: () => 53);

        var emailOption = new Option<string>(
            "--email",
            description: "The email to send in Hello request",
            getDefaultValue: () => "test@test.com");

        var addressOption = new Option<string>(
            "--address",
            description: "The address to send in Hello request",
            getDefaultValue: () => "123 Main St, Seattle, WA 98101");

        var intervalMilliSecOption = new Option<long>(
            "--interval-milli-sec",
            description: "The interval between two requests. Negative numbers mean sending one request.",
            getDefaultValue: () => -1);

        var startCommand = new Command("hello", "Call SayHello")
        {
            remoteAddrOption,
            jsonLogOption,
            nameOption,
            ageOption,
            emailOption,
            addressOption,
            intervalMilliSecOption
        };

        startCommand.Handler = CommandHandler.Create<ClientOptions>(hello);

        return startCommand;
    }


    public static async Task hello(ClientOptions options)
    {

        // Create a gRPC channel
        using var channel = GrpcChannel.ForAddress($"http://{options.RemoteAddr}");
        var client = new MyGreeter.MyGreeterClient(channel);


        if (options.IntervalMilliSec < 0)
        {
            await SayHello(client, options.Name, options.Age, options.Email, options.Address);
        }
        else
        {
            while (true)
            {
                await SayHello(client, options.Name, options.Age, options.Email, options.Address);
                await Task.Delay((int)options.IntervalMilliSec);
            }
        }
    }

    private static async Task SayHello(MyGreeter.MyGreeterClient client, string name, int age, string email, string address)
    {

        string[] addressParts = address.Split(',');
        string street = addressParts[0].Trim();
        string city = addressParts[1].Trim();
        string[] stateAndZip = addressParts[2].Trim().Split(' ');
        string state = stateAndZip[0];
        string zipString = stateAndZip[1];
        int zipCode = Convert.ToInt32(zipString);

        // Create Address instance
        var addr = new Address
        {
            Street = street,
            City = city,
            State = state,
            Zipcode = zipCode
        };

        // Prepare HelloRequest
        var request = new HelloRequest
        {
            Name = name,
            Age = age,
            Email = email,
            Address = addr
        };

        try
        {
            var reply = await client.SayHelloAsync(request);
            Console.WriteLine("Greeting: " + reply.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
