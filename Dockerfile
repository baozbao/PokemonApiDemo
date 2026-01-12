# Stage 1 Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

## WORKDIR is calling mkdir -p {folder} && cd {folder}
WORKDIR /src

## restore proj, 把这些都COPY 进 src/里

COPY ["DemoApi.sln", "./"]
COPY ["DemoApi.Web/DemoApi.Web.csproj", "DemoApi.Web/"]
COPY ["DemoApi.Domain/DemoApi.Domain.csproj", "DemoApi.Domain/"]
COPY ["DemoApi.Infrastructure/DemoApi.Infrastructure.csproj", "DemoApi.Infrastructure/"]
COPY ["DemoApi.Service/DemoApi.Service.csproj", "DemoApi.Service/"]
COPY ["DemoApi.Tests.Unit/DemoApi.Tests.Unit.csproj", "DemoApi.Tests.Unit/"]
COPY ["DemoApi.Tests.Integration/DemoApi.Tests.Integration.csproj", "DemoApi.Tests.Integration/"]

RUN dotnet restore "DemoApi.sln"
## build proj
COPY . .
WORKDIR "/src/DemoApi.Web"
RUN dotnet build "DemoApi.Web.csproj" -c Release -o /app/build

# Stage 2 Publish
FROM build AS publish
RUN dotnet publish "DemoApi.Web.csproj" -c Release -o /app/publish

## ENV ASPNETCORE_HTTP_PORTS=5001 如果端口有冲突
## EXPOSE 5001    我们可以这样写 避免默认 net7 80 端口和 net8 8080 端口被占用

# Stage 3 Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "DemoApi.Web.dll"]
