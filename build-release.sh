#!/bin/sh
# Clean up old build if present
rm -rf Draughts/publish

# Build the console part
cd Draughts.Command/
dotnet publish -c Release -o ../Draughts/publish

# Build the webapp part
cd ../Draughts/
dotnet publish -c Release -o ./publish

# Build and export docker image
sudo docker build -t draughts -f Dockerfile .
sudo docker save draughts | gzip > ./publish/docker-image-draughts.tar.gz
sudo chmod 644 ./publish/docker-image-draughts.tar.gz

# Clean up docker image
sudo docker image rm draughts --force
