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

## Full stack with PostgreSQL

### Run production-like stack (.NET API + React UI + PostgreSQL)
docker compose -f docker-compose.postgres.full-stack.yml up --build

### Run development API (hot reload) + PostgreSQL
docker compose -f docker-compose.postgres.full-stack.yml --profile development up --build api-dev reactui

### Optional: use custom PostgreSQL password
$env:POSTGRES_PASSWORD="YourStrongPostgresPassword"
docker compose -f docker-compose.postgres.full-stack.yml up --build