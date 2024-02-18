#!/bin/bash
export VAULT_FORMAT=json
port=8200
export listen_addr="0.0.0.0:${port}"
export VAULT_ADDR="http://${listen_addr}"
export VAULT_TOKEN=myroot
export vault_app="myApp"
export vault_env="dev"
export STORE="secret/${vault_app}/${vault_env}"
vault kv put ${STORE} @myApp-dev-settings.json
echo "----"
vault kv get ${STORE}