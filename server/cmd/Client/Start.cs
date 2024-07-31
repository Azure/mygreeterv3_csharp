namespace Greet.Client {

    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.NamingConventionBinder;

    using Microsoft.Extensions.Logging;
    using Serilog;
    using Serilog.Formatting.Compact;
    using Serilog.Templates;

    using Greet;
    using Grpc.Net.Client;
    using Grpc.Core;

    using MiddlewareListInterceptors;
    using LogAttrs;

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
        public string? RgName { get; set; }
        public string? RgRegion { get; set; }
        public bool CallAllRgOps { get; set; }
    }


    public static class StartCommand
    {
        public static Command Execute()
        {
            var remoteAddrOption = new Option<string>(
                "--remote-addr",
                description: "The remote server's addr for this client to connect to",
                getDefaultValue: () => "localhost:50051");

            var httpAddrOption = new Option<string>(
                "--http-addr",
                description: "The remote HTTP gateway addr",
                getDefaultValue: () => "http://localhost:50061");

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

            var rgNameOption = new Option<string>(
                "--rg-name",
                description: "The name of the resource group",
                getDefaultValue: () => "MyGreeter-resource-group");

            var rgRegionOption = new Option<string>(
                "--rg-region",
                description: "The region of the resource group",
                getDefaultValue: () => "eastus");

            var callAllRgOpsOption = new Option<bool>(
                "--call-all-rg-ops",
                description: "Call all resource group operations",
                getDefaultValue: () => true);

            var startCommand = new Command("hello", "Call SayHello")
            {
                remoteAddrOption,
                httpAddrOption,
                jsonLogOption,
                nameOption,
                ageOption,
                emailOption,
                addressOption,
                intervalMilliSecOption,
                rgNameOption,
                rgRegionOption,
                callAllRgOpsOption
            };

            startCommand.Handler = CommandHandler.Create<ClientOptions>(hello);

            return startCommand;
        }

        public static async Task hello(ClientOptions options)
        {

            // Serilog configuration
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.With<LogAttrs.CustomAttributeEnricher>();

            if (options.JsonLog)
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Console(new ExpressionTemplate("{ {time: @t, level: @l, msg: @m, EX: @x, ..@p} }\n"));
            }
            else
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Console(outputTemplate: "{Timestamp} [{Level}] {Message} {CustomAttributes:lj}{Properties}{NewLine}{Exception}");
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            var client = Greet.Client.Client.NewClient(options.RemoteAddr, Log.Logger);

            if (options.IntervalMilliSec < 0)
            {
                await SayHello(client, options.Name, options.Age, options.Email, options.Address, options);
            }
            else
            {
                while (true)
                {
                    await SayHello(client, options.Name, options.Age, options.Email, options.Address, options);
                    await Task.Delay((int)options.IntervalMilliSec);
                }
            }
        }

        private static async Task SayHello(MyGreeter.MyGreeterClient client, string name, int age, string email, string address, ClientOptions options)
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
            var helloRequest = new HelloRequest
            {
                Name = name,
                Age = age,
                Email = email,
                Address = addr
            };

            try
            {
                var reply = await client.SayHelloAsync(helloRequest);
                Log.Information("Greeting: {Message}", reply.Message);
            }
            catch (Exception ex)
            {
                Log.Error("Error: {Message}", ex.Message);
            }

            try
            {
                var reply = await client.CreateResourceGroupAsync(new CreateResourceGroupRequest
                {
                    Name = options.RgName,
                    Region = options.RgRegion
                });

                Log.Information("Resource Group reponse received: {reply}", reply);
            }

            catch (Exception ex)
            {
                Log.Error("Error: {Message}", ex.Message);
            }


        }
    }
}