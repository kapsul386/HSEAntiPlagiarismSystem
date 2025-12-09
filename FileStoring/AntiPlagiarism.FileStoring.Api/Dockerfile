# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# В КОНТЕКСТЕ уже лежит AntiPlagiarism.FileStoring.Api.csproj и всё рядом
COPY . .

RUN dotnet restore "AntiPlagiarism.FileStoring.Api.csproj"
RUN dotnet publish "AntiPlagiarism.FileStoring.Api.csproj" -c Release -o /app/publish

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV FILESTORING_ROOT=/app/data/files

ENTRYPOINT ["dotnet", "AntiPlagiarism.FileStoring.Api.dll"]
