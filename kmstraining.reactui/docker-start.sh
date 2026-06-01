#!/bin/bash

# Quick Start Script for KMSTraining.ReactUI Docker
# For Linux/Mac users

echo "🚀 KMSTraining.ReactUI Docker Quick Start"
echo "========================================="
echo ""

# Colors
GREEN='\033[0;32m'
CYAN='\033[0;36m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Function to display menu
show_menu() {
	echo -e "${YELLOW}Select an option:${NC}"
	echo "1. Build and run Production UI (Nginx)"
	echo "2. Build and run Development UI (Hot Reload)"
	echo "3. Run with Docker Compose (UI only)"
	echo "4. Run Full Stack (API + UI + Database)"
	echo "5. Stop all containers"
	echo "6. View logs"
	echo "7. Clean up (remove containers and images)"
	echo "8. Exit"
	echo ""
}

# Function to build and run production
start_production() {
	echo -e "${GREEN}🔨 Building production image...${NC}"
	docker build -t kmstraining-reactui:latest .

	echo -e "${GREEN}🚀 Starting production container...${NC}"
	docker run -d \
		-p 3000:80 \
		--name kmstraining-reactui \
		kmstraining-reactui:latest

	echo -e "${GREEN}✅ Production container started!${NC}"
	echo -e "${CYAN}📱 Access UI at: http://localhost:3000${NC}"
}

# Function to build and run development
start_development() {
	echo -e "${GREEN}🔨 Building development image...${NC}"
	docker build -t kmstraining-reactui:dev -f Dockerfile.dev .

	echo -e "${GREEN}🚀 Starting development container...${NC}"
	docker run -d \
		-p 63452:63452 \
		-v $(pwd):/app \
		-v /app/node_modules \
		--name kmstraining-reactui-dev \
		kmstraining-reactui:dev

	echo -e "${GREEN}✅ Development container started!${NC}"
	echo -e "${CYAN}📱 Access UI at: http://localhost:63452${NC}"
	echo -e "${CYAN}🔄 Hot reload is enabled!${NC}"
}

# Function to run with docker-compose
start_compose() {
	echo -e "${GREEN}🚀 Starting with Docker Compose...${NC}"
	docker-compose up -d

	echo -e "${GREEN}✅ Container started!${NC}"
	echo -e "${CYAN}📱 Access UI at: http://localhost:3000${NC}"
}

# Function to run full stack
start_fullstack() {
	echo -e "${GREEN}🚀 Starting Full Stack (API + UI + Database)...${NC}"
	docker-compose -f docker-compose.full-stack.yml up -d

	echo -e "${GREEN}✅ Full stack started!${NC}"
	echo -e "${CYAN}📱 React UI: http://localhost:3000${NC}"
	echo -e "${CYAN}🔌 API: http://localhost:5001${NC}"
	echo -e "${CYAN}💾 SQL Server: localhost:1433${NC}"
	echo ""
	echo -e "${YELLOW}⏳ Waiting for services to be healthy...${NC}"
	sleep 10
	docker-compose -f docker-compose.full-stack.yml ps
}

# Function to stop all
stop_all() {
	echo -e "${RED}🛑 Stopping all containers...${NC}"
	docker stop kmstraining-reactui 2>/dev/null
	docker stop kmstraining-reactui-dev 2>/dev/null
	docker-compose down 2>/dev/null
	docker-compose -f docker-compose.full-stack.yml down 2>/dev/null
	echo -e "${GREEN}✅ All containers stopped!${NC}"
}

# Function to view logs
show_logs() {
	echo -e "${YELLOW}Select logs to view:${NC}"
	echo "1. Production UI"
	echo "2. Development UI"
	echo "3. Docker Compose"
	echo "4. Full Stack"
	read -p "Enter choice (1-4): " log_choice

	case $log_choice in
		1) docker logs -f kmstraining-reactui ;;
		2) docker logs -f kmstraining-reactui-dev ;;
		3) docker-compose logs -f ;;
		4) docker-compose -f docker-compose.full-stack.yml logs -f ;;
	esac
}

# Function to clean up
clean_up() {
	echo -e "${RED}🧹 Cleaning up...${NC}"
	echo -e "${YELLOW}⚠️  This will remove all containers and images!${NC}"
	read -p "Are you sure? (y/n): " confirm

	if [ "$confirm" = "y" ]; then
		stop_all
		docker rm kmstraining-reactui 2>/dev/null
		docker rm kmstraining-reactui-dev 2>/dev/null
		docker-compose -f docker-compose.full-stack.yml down -v 2>/dev/null
		docker rmi kmstraining-reactui:latest 2>/dev/null
		docker rmi kmstraining-reactui:dev 2>/dev/null
		echo -e "${GREEN}✅ Cleanup complete!${NC}"
	fi
}

# Main loop
while true; do
	show_menu
	read -p "Enter your choice (1-8): " choice
	echo ""

	case $choice in
		1) start_production ;;
		2) start_development ;;
		3) start_compose ;;
		4) start_fullstack ;;
		5) stop_all ;;
		6) show_logs ;;
		7) clean_up ;;
		8) 
			echo -e "${CYAN}👋 Goodbye!${NC}"
			exit 0
			;;
		*) echo -e "${RED}❌ Invalid choice!${NC}" ;;
	esac

	echo ""
	read -p "Press Enter to continue..."
	clear
done
