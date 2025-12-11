FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "Gateway/AntiPlagiarism.Gateway/AntiPlagiarism.Gateway.csproj"
RUN dotnet publish "Gateway/AntiPlagiarism.Gateway/AntiPlagiarism.Gateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Приложение слушает 8080 внутри контейнера
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "AntiPlagiarism.Gateway.dll"]
