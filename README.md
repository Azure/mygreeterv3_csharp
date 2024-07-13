# Project

This repository holds code for a sample MyGreeter service. 

# Usage

To run this service, it is necessary to generate a gRPC service from the ``api.proto`` file. To do this, navigate into the ``api/v1/`` directory and run the command ``dotnet build``. This will use the protoc compiler under the hood to compile the proto file, generating C# gRPC files in a directory called ``api/v1/obj/``. 

To run the client and server, nagivate into the ``/Server/`` directory in two separate terminals, then run the following commands:


```dotnet run --configuration Server start```

```dotnet run --configuration Client hello```

# Directory Guide

## api/v1/

This directory holds a .NET project that includes configuration for the service itself in ``Proto/api.proto``. 

The ``Client/Client.cs`` function returns a new client, registered with interceptors.

### Interceptor

The directory currently includes the interceptors for testing purposes; in deployment, the interceptors would be in a separate dotnet package that gets imported in the .csproj file. The interceptors are all part of the MiddlewareListInterceptors namespace, which is how they get imported in other files (such as ``Server/Internal/Server/Server.cs`` and ``api/v1/Client/Client.cs``).

``DefaultServerInterceptors`` and ``DefaultClientInterceptors`` each return a list of interceptors that the client and server register themselves. Currently, the server receives the RequestId, CtxLogger, and ServerLoggerInterceptor interceptors. The RequestId interceptor looks for a requestid in the current ServerCallContext; if it doesn't find one, it adds a new one to the context, as well as to the LogContext. CtxLogger adds method and request fields to the LogContext. ServerLoggerInterceptor is taken from the documentation for .NET gRPC Interceptors, but is the jumping off point for creating the API Autologger (includes timing, logging after the end of the call).

The client currently receives duplicate interceptors to test the functionality of registering a list of interceptors. ClientLoggerInterceptor is taken from the documentation for .NET gRPC Interceptors, and it contains functionality for logging the request, as well as adding metadata to the client context.


## Server/

This directory holds a .NET project that builds and runs a Server and a Client for the gRPC service. This project imports the api/v1 project in the ``GrpcGreeter.csproj`` file.

### cmd/

The ``cmd`` directory holds the entry point for the Client and Server services. The ``GrpcGreeter.csproj`` file holds configurations for the separate Client/Server, including which files get compiled and run for each command.

``cmd/Server`` holds a basic entry point for the Server to run the Serve function, which is found in ``Server/Internal/Server/Server.cs``. 

``cmd/Client`` holds the entry point for the Client. In ``Start.cs``, a new client is created using the ``NewClient`` function from ``api/v1/Client/Client.cs``. ``Start.cs`` also defines and calls the ``hello()`` function, effectively sending a HelloRequest to the Server.

### Internal/LogAttrs

``LogAttrs`` adds new properties to a logger, using Serilog Enrichment. These properties will be propagated throughout the entire application.

### Internal/Server

This directory holds the code to run the gRPC server.

``Server.cs`` creates logging configuration for the server, injects the logger and other properties into the Dependency Injection container, and builds & starts the service.


## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
