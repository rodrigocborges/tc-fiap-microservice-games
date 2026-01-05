# Estágio 1: Build da Aplicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Otimização de cache: Copia csproj e restaura antes de copiar o resto
COPY *.sln .
COPY FIAPCloudGames.API/*.csproj ./FIAPCloudGames.API/
COPY FIAPCloudGames.Application/*.csproj ./FIAPCloudGames.Application/
COPY FIAPCloudGames.Domain/*.csproj ./FIAPCloudGames.Domain/
COPY FIAPCloudGames.Infrastructure/*.csproj ./FIAPCloudGames.Infrastructure/
COPY FIAPCloudGames.Tests/*.csproj ./FIAPCloudGames.Tests/
RUN dotnet restore "FIAPCloudGames.sln"

# Copia todo o código e faz o build
COPY . .
# Usei --no-restore pois já restauramos antes.
RUN dotnet publish "FIAPCloudGames.API/FIAPCloudGames.API.csproj" -c Release -o /app/publish --no-restore

# Estágio 2: Criação da Imagem Final (Otimizada com Alpine)
# A tag "alpine" usa uma distribuição Linux minúscula (aprox 5MB base)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
WORKDIR /app

# Instala dependências de globalização (necessário para SQL Client e formatação de moeda)
RUN apk add --no-cache icu-libs

# Configura o .NET para usar as libs de globalização instaladas
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Copia os artefatos do estágio de build
COPY --from=build /app/publish .

# Define a porta
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Define usuário não-root para segurança (boa prática em K8s)
USER app

ENTRYPOINT ["dotnet", "FIAPCloudGames.API.dll"]