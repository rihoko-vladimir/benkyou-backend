FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Statistics.Api/Statistics.Api.csproj", "Statistics.Api/"]
RUN dotnet restore "Statistics.Api/Statistics.Api.csproj"
COPY . .
WORKDIR "/src/Statistics.Api"
RUN dotnet build "Statistics.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Statistics.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Statistics.Api.dll"]
