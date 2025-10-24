# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /

# 1) Copy sln + individual csproj files (keeps restore cache hot)
COPY TreblleApi.sln ./

COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Api/Api.csproj Api/

# 2) Restore the solution (resolves all project references)
RUN dotnet restore TreblleApi.sln

# 3) Copy the rest of the source
COPY . .

# 4) Publish ONLY the Api (self-contained=false, smaller runtime image)
RUN dotnet publish Api/Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
# (optional) set TZ or globalization if you need it
# ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish ./
EXPOSE 8080

ENTRYPOINT ["dotnet", "Api.dll"]

