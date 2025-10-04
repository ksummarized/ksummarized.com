#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk
ARG DOTNET_VERSION=9.0

FROM ${DOTNET_SDK}:${DOTNET_VERSION} AS base

# Install Node.js and npm
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs

WORKDIR /src
COPY . .
ENTRYPOINT ["dotnet", "run", "--project", "backend/AppHost"]