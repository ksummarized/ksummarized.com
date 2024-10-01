dev:
	docker compose --profile hot-reload up --build --watch

no-reload:
	docker compose --profile without-hot-reload up --build

db:
	docker compose up db -d

db_down:
	docker compose down db

apply_migrations:
	@cd scripts && pwsh apply_migrations.ps1
	@cd ../

.PHONY: dev no-reload db db_down apply_migrations
.DEFAULT_GOAL := dev
