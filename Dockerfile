# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CareNest_Appointment.API/CareNest_Appointment.API.csproj", "CareNest_Appointment.API/"]
COPY ["CareNest_Appointment.Application/CareNest_Appointment.Application.csproj", "CareNest_Appointment.Application/"]
COPY ["CareNest_Appointment.Domain/CareNest_Appointment.Domain.csproj", "CareNest_Appointment.Domain/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["CareNest_Appointment.Infrastructure/CareNest_Appointment.Infrastructure.csproj", "CareNest_Appointment.Infrastructure/"]
RUN dotnet restore "./CareNest_Appointment.API/CareNest_Appointment.API.csproj"
COPY . .
WORKDIR "/src/CareNest_Appointment.API"
RUN dotnet build "./CareNest_Appointment.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CareNest_Appointment.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CareNest_Appointment.API.dll"]