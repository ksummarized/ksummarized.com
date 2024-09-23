#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
COPY . /src
WORKDIR /src
CMD ["dotnet", "watch", "--verbose", "--non-interactive", "--no-launch-profile", "--project", "src/api/"]

#Todo: clean it up for multiproject build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/api/api.csproj src/api/
RUN dotnet restore "src/api/api.csproj"
COPY . .
WORKDIR /src/src/api
RUN dotnet build "api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api.dll"]
