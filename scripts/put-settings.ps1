<#
	Put Settings
#>
$Env:VAULT_TOKEN="myroot"
$Env:VAULT_ADDR='http://127.0.0.1:8200'

[int]$PORT=8200
[string]$LISTEN_ADDR="0.0.0.0:${PORT}"

[string]$VAULT_FORMAT="json"
[string]$VAULT_APP="myApp"
[string]$VAULT_ENV="dev"
[string]$STORE="secret/${VAULT_APP}/${VAULT_ENV}"

[string]$SETTINGS_FILE=".\myApp-dev-settings.json"

vault kv put "${STORE}" "@${SETTINGS_FILE}"
Write-Output "----"
vault kv get "${STORE}"