FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/PasswdService/PasswdService.csproj", "src/PasswdService/"]
COPY . .
WORKDIR "/src/src/PasswdService"

FROM build AS publish
RUN dotnet publish "PasswdService.csproj" --configuration Release --no-build --no-restore --output /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PasswdService.dll"]
