
# Create a new builder
$BUILDER = docker buildx create --use

# Build and push the image
docker buildx build --platform=linux/amd64,linux/arm64 --build-arg BUILD_CONFIGURATION=Release --push -t syukurdocker/e-commerce:dev_v1.0 .

# Remove the builder
docker buildx rm $BUILDER