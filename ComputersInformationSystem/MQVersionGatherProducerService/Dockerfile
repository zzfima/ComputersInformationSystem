#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MQVersionGatherProducerService/MQVersionGatherProducerService.csproj", "MQVersionGatherProducerService/"]
COPY ["MQProducer/MQProducer.csproj", "MQProducer/"]
COPY ["Utilities/Utilities.csproj", "Utilities/"]
COPY ["Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "./MQVersionGatherProducerService/./MQVersionGatherProducerService.csproj"
COPY . .
WORKDIR "/src/MQVersionGatherProducerService"
RUN dotnet build "./MQVersionGatherProducerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MQVersionGatherProducerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MQVersionGatherProducerService.dll"]