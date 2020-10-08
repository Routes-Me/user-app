FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
COPY . /app
WORKDIR /app
ENTRYPOINT ["dotnet", "BlazorUserApp.dll"]
