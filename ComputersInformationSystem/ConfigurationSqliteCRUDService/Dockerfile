#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5200

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["ConfigurationSqliteCRUDService/ConfigurationSqliteCRUDService.csproj", "ConfigurationSqliteCRUDService/"]
COPY ["Utilities/Utilities.csproj", "Utilities/"]
COPY ["Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "./ConfigurationSqliteCRUDService/./ConfigurationSqliteCRUDService.csproj"
COPY . .
WORKDIR "/src/ConfigurationSqliteCRUDService"
RUN dotnet build "./ConfigurationSqliteCRUDService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./ConfigurationSqliteCRUDService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigurationSqliteCRUDService.dll"]