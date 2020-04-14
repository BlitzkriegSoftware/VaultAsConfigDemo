#!/bin/bash
# See: https://hub.docker.com/_/vault
imagename=some-vault
docker stop ${imagename}
docker rm ${imagename}