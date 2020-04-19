#!/bin/bash
# See: https://hub.docker.com/_/vault

imagename=some-vault
port=8200
export listen_addr="0.0.0.0:${port}"
export VAULT_ADDR="http://${listen_addr}"
export VAULT_TOKEN=myroot

docker stop ${imagename}
docker rm ${imagename}
docker pull vault

echo "Vault UI: ${VAULT_ADDR}/ui use token of ${VAULT_TOKEN} to login"

# export VLC="VAULT_LOCAL_CONFIG=\"{ "listener": [{ "tcp": { "address": "127.0.0.1:8200", "tls_disable": "1" } }], "default_lease_ttl": "168h", "max_lease_ttl": "720h" }\""

set -x

docker run --cap-add=IPC_LOCK -p ${port}:${port} -e "VAULT_DEV_ROOT_TOKEN_ID=${VAULT_TOKEN}" -e "VAULT_DEV_LISTEN_ADDRESS=${listen_addr}" --name=${imagename} vault

set +x