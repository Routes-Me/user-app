
# FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
# EXPOSE 80

# FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
# WORKDIR /app

# # Copy csproj and restore as distinct layers
# COPY *.csproj ./
# RUN dotnet restore

# # Copy everything else and build
# COPY . ./
# RUN dotnet publish -c Release -o out

# # Build runtime image
# FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
# WORKDIR /app
# COPY --from=build-env /app/out .
# ENTRYPOINT ["dotnet", "RoutesApp.dll"]


FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o output

# FROM nginx:alpine
# WORKDIR  /user/share/nginx/html
# COPY --from=build-env /app/output/wwwroot/ .

FROM nginx:alpine
WORKDIR /var/www/web
COPY --from=build-env /app/output/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80