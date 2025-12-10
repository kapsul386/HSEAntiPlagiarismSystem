# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "Analysis/AntiPlagiarism.Analysis.Api/AntiPlagiarism.Analysis.Api.csproj"
RUN dotnet publish "Analysis/AntiPlagiarism.Analysis.Api/AntiPlagiarism.Analysis.Api.csproj" -c Release -o /app/publish

# ===== RUNTIME STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "AntiPlagiarism.Analysis.Api.dll"]
