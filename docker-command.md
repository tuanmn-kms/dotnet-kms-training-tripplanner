# Open terminal and go to project:
cd kmstraining.reactui

# Build image:
docker build --build-arg VITE_API_URL=http://localhost:7777/api -t kmstraining-reactui:dev .

# Run container:
docker run -d --name reactui-dev -p 3888:80 kmstraining-reactui:dev

# Verify:
docker ps

# Stop/remove when needed:
docker stop reactui-dev
docker rm reactui-dev


## React UI

### Auto update changed code
docker compose --profile development up --build reactui-dev