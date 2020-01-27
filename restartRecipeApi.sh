echo "Pulling changes"
git pull
echo "Stopping recipe-api container"
docker stop recipe-api
echo "Remove recipe-api container"
docker rm recipe-api
echo "Build the new recipe-api docker image"
docker build -t recipe-api-image .
echo "Running the new recipe-api container"
docker run --name recipe-api --restart always -p 35001:80 -d recipe-api-image
echo ""

echo "Complete"
echo ""