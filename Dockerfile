FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY **/*.sln ./
COPY **/*.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Timezone, globalization if you need it
ENV ASPNETCORE_URLS=http://+:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish ./
EXPOSE 8080

ENTRYPOINT ["dotnet", "YourApi.dll"]
