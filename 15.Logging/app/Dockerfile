# ------------------------------------------------
# Базовий образ для рантайму
# ------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ------------------------------------------------
# Образ для збірки
# ------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Копіюємо файл .csproj
COPY MyApi.csproj ./
RUN dotnet restore MyApi.csproj

# 2. Копіюємо усі інші файли *.cs
COPY . . 
RUN dotnet build MyApi.csproj -c $BUILD_CONFIGURATION -o /app/build

# ------------------------------------------------
# Публікація
# ------------------------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish MyApi.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ------------------------------------------------
# Фінальний образ
# ------------------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyApi.dll"]
