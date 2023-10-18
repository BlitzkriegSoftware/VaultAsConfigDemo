<#
	See: https://hub.docker.com/_/vault
#>

$Env:VAULT_TOKEN="myroot"
$Env:VAULT_ADDR='http://127.0.0.1:8200'

$imagename="some-vault"
$port=8200

docker stop ${imagename} 2>&1 | out-null
docker rm ${imagename} 2>&1 | out-null
docker pull hashicorp/vault

Write-Output "Vault UI: $($Env:VAULT_ADDR)/ui use token of $($Env:VAULT_TOKEN) to login"
docker run -d `
	--cap-add=IPC_LOCK `
	-p "${port}:${port}" `
	-e "VAULT_DEV_ROOT_TOKEN_ID=$($Env:VAULT_TOKEN)" `
	--name=${imagename} hashicorp/vault