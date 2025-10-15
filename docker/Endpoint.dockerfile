FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY Core/InfinityMirror.Core.csproj Core/
COPY Endpoint/InfinityMirror.Endpoint.csproj Endpoint/

WORKDIR /source/Endpoint
RUN dotnet restore

# Software version number
#   - Should correspond to tag
#   - Including default value so if someone just runs "docker build", it will work
ARG SOLUTION_VERSION=docker

# copy everything else and build app
WORKDIR /source
COPY . .
WORKDIR /source/Endpoint
RUN dotnet publish --self-contained false -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "InfinityMirror.Endpoint.dll"]

# We are listening on 8080, fyi
EXPOSE 8080
