dev:
	docker compose --profile hot-reload up --build --watch

no-reload:
	docker compose --profile without-hot-reload up --build

db:
	docker compose up db -d

db_down:
	docker compose down db

.PHONY: dev no-reload db db_down
.DEFAULT_GOAL := dev
