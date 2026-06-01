# Open terminal and go to project:
cd kmstraining.reactui

# Build image:
docker build --build-arg VITE_API_URL=http://localhost:32779/api -t kmstraining-reactui:dev .

# Run container:
docker run -d --name reactui-dev -p 3888:80 kmstraining-reactui:dev

# Verify:
docker ps

# Stop/remove when needed:
docker stop reactui-dev
docker rm reactui-dev