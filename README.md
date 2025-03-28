# ksummarized.com

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-white.svg)](https://sonarcloud.io/summary/new_code?id=ksummarized_ksummarized.com_frontend)
[![Build and test - backend](https://github.com/ksummarized/ksummarized.com/actions/workflows/build-and-test-backend.yml/badge.svg)](https://github.com/ksummarized/ksummarized.com/actions/workflows/build-and-test-backend.yml)
[![Lint and test - frontend](https://github.com/ksummarized/ksummarized.com/actions/workflows/lint-and-test-frontend.yml/badge.svg)](https://github.com/ksummarized/ksummarized.com/actions/workflows/lint-and-test-frontend.yml)

## How to run locally

To run this application locally it is recommended to have the following installed:

- docker
- docker-compose
- powershell
- dotnet sdk
- entity framework tools

Firstly there is a need to configure environment variables:

1. Copy `.env.example` as `.env` and populate the environment variables.
1. Copy `appsettings.json` as `appsettings.Development.json` and populate the variables.

Next install dev-certs to use https in powershell

```powershell
dotnet dev-certs https -ep ".aspnet\https\aspnetapp.pfx"  -p devcertpasswd --trust
```

or in bash/zsh

```bash
dotnet dev-certs https -ep .aspnet/https/aspnetapp.pfx -p devcertpasswd --trust
```

Database migrations will be automatically applied when executing docker-compose via the `migrations` service.

Next You should go back to the main directory and run `docker compose --profile hot-reload up --build --watch`

You can now visit the site at: <http://localhost:8888/>

## Scripts

Directory `scripts` contains some helpful scripts which automate some parts of working with this directory.

## Development

The application is started using the following command (with hot-reload feature):

```bash
make
```

or without the hot-reload feature:

```bash
make no-reload
```

If you want to run the application manually you can use the following command:

```bash
docker compose --profile hot-reload up --build --watch
```

or without the hot-reload feature:

```bash
docker compose --profile without-hot-reload up --build --watch
```

The `--watch` parameter starts containers with the `hot-reload` feature. This enables the auto-reload functionality, meaning that the container will be automatically reloaded when the code for either the frontend or backend changes.

> The `hot-reload` feature for backend applications uses `dotnet watch`, which only detects changes to existing files. It will not restart the container if new files are added (dotnet watch [issue](https://github.com/dotnet/aspnetcore/issues/8321)).

### DB Migrations

When working with migrations remember to add parameter for project and startup project.
For example when generating a new migration while in backend directory:

```bash
dotnet ef migrations add -p ./src/infrastructure -s ./src/api "Example"
```
