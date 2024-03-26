FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Flights/TransportZone.Flights.Api/TransportZone.Flights.Api.csproj", "Flights/TransportZone.Flights.Api/"]
RUN dotnet restore "Flights/TransportZone.Flights.Api/TransportZone.Flights.Api.csproj"
COPY . .
WORKDIR "/src/Flights/TransportZone.Flights.Api"
RUN dotnet build "TransportZone.Flights.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TransportZone.Flights.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransportZone.Flights.Api.dll"]
