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

Next go to the `scripts` directory and run `apply_migrations.ps1`
Next You should go back to the main directory and run `docker compose up --build`
This can be done with the following snippet.

```powershell
cd scripts
.\apply_migrations.ps1
cd ..\
docker compose up --build
```

You can now visit the site at: <http://localhost:8888/>

## Scripts

Directory `scripts` contains some helpful scripts which automate some parts of working with this directory.
