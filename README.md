# VaultAsConfigDemo
Using Hashicorp Vault as a Configuration Store

## Prerequisites

1. Install Docker: https://docs.docker.com/get-docker/
2. Install Vault Client: https://www.vaultproject.io/downloads
3. Start Docker
4. Run `./scripts/start_docker_vault.sh` to start Vault in Docker
5. Run the demo in Visual Studio or Code to see how it works
6. Stop Docker Vault `./scripts/start_docker_vault.sh`

## Setting up a structure for key value pairs or json blobs

1. Decide on a folder structure, for the demo we're using a subpath of:

   `/{system}/{component}/{environment}`

2. Decide on loose key/value pairs or a json datagram. For this demo we have examples of both.

3. Store the configuration using code or the **vault** cli

## Code


