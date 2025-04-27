all: build run

.PHONY: build
build:
	podman build -f Dockerfile -t crossmint-challenge:latest .

.PHONY: run
run:
	podman run crossmint-challenge
