#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MQVersionGatherConsumerService/MQVersionGatherConsumerService.csproj", "MQVersionGatherConsumerService/"]
COPY ["AliveIPGather/AliveIPGather.csproj", "AliveIPGather/"]
COPY ["Utilities/Utilities.csproj", "Utilities/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["VersionRetrieverService/VersionGather.csproj", "VersionRetrieverService/"]
RUN dotnet restore "./MQVersionGatherConsumerService/./MQVersionGatherConsumerService.csproj"
COPY . .
WORKDIR "/src/MQVersionGatherConsumerService"
RUN dotnet build "./MQVersionGatherConsumerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MQVersionGatherConsumerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MQVersionGatherConsumerService.dll"]