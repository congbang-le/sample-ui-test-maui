#!/bin/bash

# Script to load VM Docker images from tar files
# This script loads the exported Docker images

echo "=== Loading VM Docker Images ==="

# Function to load image from tar file
load_image() {
    local tar_file="$1"
    local image_name="$2"
    
    if [ -f "docker-images/$tar_file" ]; then
        echo "Loading $image_name from docker-images/$tar_file..."
        docker load < "docker-images/$tar_file"
        
        if [ $? -eq 0 ]; then
            echo "Successfully loaded $image_name"
        else
            echo "Failed to load $image_name"
        fi
    else
        echo "Warning: docker-images/$tar_file not found"
        echo "  Please ensure the tar file exists or run build-images.sh first"
    fi
}

# Load each VM application image
echo "Loading VM application images..."

load_image "vm-app.tar.gz" "vm-app:latest"
load_image "vm-vs-sync-app.tar.gz" "vm-vs-sync-app:latest" 
load_image "vm-tt-app.tar.gz" "vm-tt-app:latest"

echo ""
echo "=== Image Loading Complete ==="
echo ""
echo "Loaded images:"
docker images | grep -E "(vm-app|vm-vs-sync-app|vm-tt-app)"
