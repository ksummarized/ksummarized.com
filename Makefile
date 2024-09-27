dev:
	docker compose --profile hot-reload up --build --watch

no-reload:
	docker compose --profile without-hot-reload up --build

db:
	docker compose up db -d

.PHONY: dev no-reload db
.DEFAULT_GOAL := dev
