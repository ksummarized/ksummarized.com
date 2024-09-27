dev:
	docker compose --profile hot-reload up --build --watch

no-reload:
	docker compose --profile without-hot-reload up --build

.PHONY: dev no-reload
.DEFAULT_GOAL := dev
