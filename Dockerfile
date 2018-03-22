FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /LogServer

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /LogServer
COPY --from=build-env /LogServer/out .
ENTRYPOINT ["dotnet", "LogServer.dll"]