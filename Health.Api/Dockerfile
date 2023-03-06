FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Health.Api/Health.Api.csproj", "Health.Api/"]
RUN dotnet restore "Health.Api/Health.Api.csproj"
COPY . .
WORKDIR "/src/Health.Api"
RUN dotnet build "Health.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Health.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Health.Api.dll"]
