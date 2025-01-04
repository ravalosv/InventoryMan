
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

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

# Genera un certificado auto-firmado
RUN apt-get update && \
    apt-get install -y openssl && \
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /app/server.key -out /app/server.crt \
    -subj "/CN=localhost" && \
    openssl pkcs12 -export -out /app/certificate.pfx \
    -inkey /app/server.key -in /app/server.crt \
    -passout pass:your_password_1234

ENV ASPNETCORE_URLS=https://+:8443;http://+:8080
ENV ASPNETCORE_HTTPS_PORT=8443
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/certificate.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=your_password_1234

EXPOSE 8080
EXPOSE 8443

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





