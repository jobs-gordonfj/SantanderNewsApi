FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SantanderNewsApi.csproj", "./"]
RUN dotnet restore "SantanderNewsApi.csproj"

COPY . .
RUN dotnet publish "SantanderNewsApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "SantanderNewsApi.dll" ]
