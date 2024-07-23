# (Prerequisite) To deploy the shared resources needed, go to the directory of the shared resources and run the Makefile.
SERVER_DIR := $(shell dirname -- $(shell pwd))

#Kube config must exist within server directory to be accessible in docker container when running helm commands
connect-cluster:
	az account set --subscription 8ecadfc9-d1a3-4ea4-b844-0d9f87e4d7c8
	az aks get-credentials --resource-group servicehub-tyhmv4nvyg-rg --name servicehub-tyhmv4nvyg-cluster --file $(SERVER_DIR)/.kube/config
	if grep -qF -- "export KUBECONFIG=$(SERVER_DIR)/.kube/config" ~/.bashrc; then \
		echo 'KUBECONFIG exists in ~/.bashrc'; \
	else \
		echo 'KUBECONFIG does not exist in ~/.bashrc'; \
		echo 'export KUBECONFIG=$(SERVER_DIR)/.kube/config' >> ~/.bashrc; \
	fi

build-image:
	set -e; \
	if [ -z "$(READ_PAT)" ]; then \
		echo "READ_PAT does not exist. Please rerun the command with this variable"; \
		exit 1; \
	fi; \
	docker build --build-arg PAT=$(READ_PAT) \
	-t servicehubtyhmv4nvygeastusacr.azurecr.io/mygreeterv3_csharp -f ../Dockerfile ./../; \
	docker save -o mygreeterv3_csharp-image.tar servicehubtyhmv4nvygeastusacr.azurecr.io/mygreeterv3_csharp; \

push-image:
	set -e; \
	az acr login --name servicehubtyhmv4nvygeastusacr.azurecr.io; \
	docker load -i mygreeterv3_csharp-image.tar; \
	docker push servicehubtyhmv4nvygeastusacr.azurecr.io/mygreeterv3_csharp; \
	rm mygreeterv3_csharp-image.tar;