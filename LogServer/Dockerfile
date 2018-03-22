FROM microsoft/aspnetcore:2.0.0
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
# FROM microsoft/aspnetcore:2.0
# WORKDIR /LogServer
# COPY --from=build-env /LogServer/out .
ENTRYPOINT ["dotnet", "out/LogServer.dll", "--server.urls", "http://0.0.0.0:5000"]