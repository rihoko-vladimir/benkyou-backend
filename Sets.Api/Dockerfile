FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Sets.Api/Sets.Api.csproj", "Sets.Api/"]
RUN dotnet restore "Sets.Api/Sets.Api.csproj"
COPY . .
WORKDIR "/src/Sets.Api"
RUN dotnet build "Sets.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sets.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sets.Api.dll"]
