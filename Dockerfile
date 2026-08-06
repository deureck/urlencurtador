FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Copia todo o conteúdo da pasta atual
COPY . .
RUN ls -la
# 2. Restaura e publica apontando direto para o .csproj (ignorando o .sln)
RUN dotnet restore src/urlencurtador.csproj
RUN dotnet publish src/urlencurtador.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "urlencurtador.dll"]
