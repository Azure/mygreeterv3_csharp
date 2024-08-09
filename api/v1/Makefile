all: service
MAIN_DIR := $(shell dirname -- $(shell dirname -- $(shell dirname -- $(shell pwd))))
USER_ID := $(shell id -u):$(shell id -g)
REGISTRY_NAME=servicehubregistry
IMG_NAME=service_hub_environment
IMG_DATE_TAG=20240329

# api.swagger.json file is automatically generated using npx buf generate command via the openapi plugin
# Set up goproxy variables in accordance to if goModuleNamePrefix is the user's repository or internal go.goms.io
service:
	docker run --mount src=$(MAIN_DIR),target=/app/binded-data,type=bind \
		$(REGISTRY_NAME).azurecr.io/$(IMG_NAME):$(IMG_DATE_TAG) \
		/bin/bash -c \
	"./maintain_service populateMethodFiles \
		--protoFilePath binded-data/mygreeterv3csharp/api/v1/Proto/api.proto \
		--svcDirName binded-data/mygreeterv3csharp --deleteGarbageFiles=false;"