# Usar la imagen base oficial de .NET
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Usar la imagen de desarrollo para compilar el código
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PortalAcademico/PortalAcademico.csproj", "PortalAcademico/"]
RUN dotnet restore "PortalAcademico/PortalAcademico.csproj"
COPY . .
WORKDIR "/src/PortalAcademico"
RUN dotnet build "PortalAcademico.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PortalAcademico.csproj" -c Release -o /app/publish

# Establecer la imagen de producción
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PortalAcademico.dll"]
