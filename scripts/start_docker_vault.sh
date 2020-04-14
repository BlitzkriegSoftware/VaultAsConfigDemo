#!/bin/bash
# See: https://hub.docker.com/_/vault
imagename=some-vault
port=8200
docker stop ${imagename}
docker rm ${imagename}
docker pull vault
set -x
docker run --cap-add=IPC_LOCK -d -p ${port}:${port} --name=${imagename} vault
# docker run --cap-add=IPC_LOCK -e 'VAULT_DEV_ROOT_TOKEN_ID=myroot' -e 'VAULT_DEV_LISTEN_ADDRESS=127.0.0.1:${port}' -p ${port}:${port} --name=${imagename} vault server
vault status
set +x