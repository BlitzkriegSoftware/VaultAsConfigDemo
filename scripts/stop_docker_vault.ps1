<#
	See: https://hub.docker.com/_/vault
#>
$imagename="some-vault"
docker stop ${imagename} 2>&1 | out-null
docker rm ${imagename} 2>&1 | out-null