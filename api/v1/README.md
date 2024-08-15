## Overview

This nuget package contains the Api/V1 package. 

### `Proto/api.proto`

Defines a set of requests and responses for the MyGreeterCsharp service. The Api/V1 package includes the generated Grpc service from `api.proto`, including functions like `SayHello()`, `CreateResourceGroup()`, etc. 

### `Client/Client.cs`, 

`NewClient()` function returns a new client at the specified remote address. 

