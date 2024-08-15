# Stage 1: Build the .NET application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

EXPOSE 50051

# Copy the csproj files and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy the source code and build for Client and Server
COPY . ./

# Publish the Client project
RUN dotnet publish -c Client -o /app/client

# Publish the Server project
RUN dotnet publish -c Server -o /app/server

# Stage 2: Create a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published Client and Server binaries
COPY --from=build /app/client ./client
COPY --from=build /app/server ./server
