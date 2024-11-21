#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet
ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk
ARG DOTNET_VERSION=8.0

FROM ${DOTNET_RUNTIME}:${DOTNET_VERSION} AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM ${DOTNET_SDK}:${DOTNET_VERSION} AS development
COPY . /src
WORKDIR /src
CMD ["dotnet", "watch", "--verbose", "--non-interactive", "--no-launch-profile", "--project", "src/api/"]

FROM ${DOTNET_SDK}:${DOTNET_VERSION} AS migrations
COPY . /src
WORKDIR /src
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT [ "dotnet", "ef", "database", "update", "-p", "src/infrastructure", "-s", "src/api" ]

#Todo: clean it up for multiproject build
FROM ${DOTNET_SDK}:${DOTNET_VERSION} AS build
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
