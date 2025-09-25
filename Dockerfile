# Estágio 1: Build da Aplicação
# SDK do .NET 9 para compilar e publicar a aplicação.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copia os arquivos .csproj e restaura as dependências primeiro para aproveitar o cache do Docker.
COPY *.sln .
COPY FIAPCloudGames.API/*.csproj ./FIAPCloudGames.API/
COPY FIAPCloudGames.Application/*.csproj ./FIAPCloudGames.Application/
COPY FIAPCloudGames.Domain/*.csproj ./FIAPCloudGames.Domain/
COPY FIAPCloudGames.Infrastructure/*.csproj ./FIAPCloudGames.Infrastructure/
COPY FIAPCloudGames.Tests/*.csproj ./FIAPCloudGames.Tests/
RUN dotnet restore "FIAPCloudGames.sln"

# Copia todo o resto do código fonte e publica a API.
COPY . .
# O comando de publish compila o projeto e coloca os artefatos prontos para execução na pasta /app/publish.
RUN dotnet publish "FIAPCloudGames.API/FIAPCloudGames.API.csproj" -c Release -o /app/publish

# Estágio 2: Criação da Imagem Final
# Usamos a imagem ASP.NET runtime, que é muito menor que a SDK, pois só precisamos rodar a aplicação.
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Define a porta que a aplicação irá expor dentro do contêiner.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Integração com o APM Newrelic
# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y 'newrelic-dotnet-agent' \
&& rm -rf /var/lib/apt/lists/*

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so
#

# Define o comando para iniciar a aplicação quando o contêiner for executado.
ENTRYPOINT ["dotnet", "FIAPCloudGames.API.dll"]