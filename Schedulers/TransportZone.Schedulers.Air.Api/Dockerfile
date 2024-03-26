FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Schedulers/TransportZone.Schedulers.Air.Api/TransportZone.Schedulers.Air.Api.csproj", "Schedulers/TransportZone.Schedulers.Air.Api/"]
RUN dotnet restore "Schedulers/TransportZone.Schedulers.Air.Api/TransportZone.Schedulers.Air.Api.csproj"
COPY . .
WORKDIR "/src/Schedulers/TransportZone.Schedulers.Air.Api"
RUN dotnet build "TransportZone.Schedulers.Air.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TransportZone.Schedulers.Air.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransportZone.Schedulers.Air.Api.dll"]
