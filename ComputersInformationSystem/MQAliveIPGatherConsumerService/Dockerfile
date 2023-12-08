#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS runtime
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MQAliveIPGatherConsumerService/MQAliveIPGatherConsumerService.csproj", "MQAliveIPGatherConsumerService/"]
COPY ["AliveIPGather/AliveIPGather.csproj", "AliveIPGather/"]
COPY ["Utilities/Utilities.csproj", "Utilities/"]
COPY ["Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "./MQAliveIPGatherConsumerService/./MQAliveIPGatherConsumerService.csproj"
COPY . .
WORKDIR "/src/MQAliveIPGatherConsumerService"
RUN dotnet publish "./MQAliveIPGatherConsumerService.csproj" -c Release -o out --no-restore

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["CacheService.exe"]
