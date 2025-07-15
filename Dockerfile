FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080


# Configuración de entorno predeterminada
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["NeonNovaApp/NeonNovaApp.csproj", "NeonNovaApp/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Intrastructure/Intrastructure.csproj", "Intrastructure/"]
RUN dotnet restore "NeonNovaApp/NeonNovaApp.csproj"
COPY . .
WORKDIR "/src/NeonNovaApp"
RUN dotnet build "NeonNovaApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NeonNovaApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Punto de entrada directo para la aplicación
ENTRYPOINT ["dotnet", "NeonNovaApp.dll"]