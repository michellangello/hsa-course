docker-compose down -v --remove-orphans
rm -rf ./data
docker-compose up --build -d