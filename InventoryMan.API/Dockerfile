FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia los archivos .csproj y restaura las dependencias
COPY ["InventoryMan.API/InventoryMan.API.csproj", "InventoryMan.API/"]
COPY ["InventoryMan.Application/InventoryMan.Application.csproj", "InventoryMan.Application/"]
COPY ["InventoryMan.Core/InventoryMan.Core.csproj", "InventoryMan.Core/"]
COPY ["InventoryMan.Infrastructure/InventoryMan.Infrastructure.csproj", "InventoryMan.Infrastructure/"]
RUN dotnet restore "InventoryMan.API/InventoryMan.API.csproj"

# Copia el resto del código y compila
COPY . .
WORKDIR "/src/InventoryMan.API"
RUN dotnet build "InventoryMan.API.csproj" -c Release -o /app/build

# Publica
FROM build AS publish
RUN dotnet publish "InventoryMan.API.csproj" -c Release -o /app/publish

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV PORT=8080
ENV ASPNETCORE_URLS=http://+:${PORT}

EXPOSE ${PORT}

ENTRYPOINT ["dotnet", "InventoryMan.API.dll"]




# Ejecutarlo en PowerShell
# Para ejecutar todos los comandos desde el archivo docker-compose.yml
# docker-compose down
# docker rmi inventoryman-api:latest
# docker-compose up --build
# Para compilar la imagen
# docker build -t inventoryman-api -f Dockerfile ..


# Para ejecutar el container
# docker run -dp 5000:80 --name api-server api-server

#* Para publicarlo en DigitalOcean
#* doctl auth switch --context drelliseo
#* doctl registry login
#* docker image tag inventoryman-api registry.digitalocean.com/dr-elliseo/inventoryman-api:20250103.01
#* docker image push registry.digitalocean.com/dr-elliseo/inventoryman-api:20250103.01

#* para correlo localmente
#* docker run --name api-server-DigOc -dp 5000:80 registry.digitalocean.com/dr-elliseo/api-server:20230923.01





