# Quick Start Script for KMSTraining.ReactUI Docker

Write-Host "🚀 KMSTraining.ReactUI Docker Quick Start" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Function to display menu
function Show-Menu {
	Write-Host "Select an option:" -ForegroundColor Yellow
	Write-Host "1. Build and run Production UI (Nginx)"
	Write-Host "2. Build and run Development UI (Hot Reload)"
	Write-Host "3. Run with Docker Compose (UI only)"
	Write-Host "4. Run Full Stack (API + UI + Database)"
	Write-Host "5. Stop all containers"
	Write-Host "6. View logs"
	Write-Host "7. Clean up (remove containers and images)"
	Write-Host "8. Exit"
	Write-Host ""
}

# Function to build and run production
function Start-Production {
	Write-Host "🔨 Building production image..." -ForegroundColor Green
	docker build -t kmstraining-reactui:latest .

	Write-Host "🚀 Starting production container..." -ForegroundColor Green
	docker run -d `
		-p 3000:80 `
		--name kmstraining-reactui `
		kmstraining-reactui:latest

	Write-Host "✅ Production container started!" -ForegroundColor Green
	Write-Host "📱 Access UI at: http://localhost:3000" -ForegroundColor Cyan
}

# Function to build and run development
function Start-Development {
	Write-Host "🔨 Building development image..." -ForegroundColor Green
	docker build -t kmstraining-reactui:dev -f Dockerfile.dev .

	Write-Host "🚀 Starting development container..." -ForegroundColor Green
	docker run -d `
		-p 63452:63452 `
		-v ${PWD}:/app `
		-v /app/node_modules `
		--name kmstraining-reactui-dev `
		kmstraining-reactui:dev

	Write-Host "✅ Development container started!" -ForegroundColor Green
	Write-Host "📱 Access UI at: http://localhost:63452" -ForegroundColor Cyan
	Write-Host "🔄 Hot reload is enabled!" -ForegroundColor Cyan
}

# Function to run with docker-compose
function Start-Compose {
	Write-Host "🚀 Starting with Docker Compose..." -ForegroundColor Green
	docker-compose up -d

	Write-Host "✅ Container started!" -ForegroundColor Green
	Write-Host "📱 Access UI at: http://localhost:3000" -ForegroundColor Cyan
}

# Function to run full stack
function Start-FullStack {
	Write-Host "🚀 Starting Full Stack (API + UI + Database)..." -ForegroundColor Green
	docker-compose -f docker-compose.full-stack.yml up -d

	Write-Host "✅ Full stack started!" -ForegroundColor Green
	Write-Host "📱 React UI: http://localhost:3000" -ForegroundColor Cyan
	Write-Host "🔌 API: http://localhost:5001" -ForegroundColor Cyan
	Write-Host "💾 SQL Server: localhost:1433" -ForegroundColor Cyan
	Write-Host ""
	Write-Host "⏳ Waiting for services to be healthy..." -ForegroundColor Yellow
	Start-Sleep -Seconds 10
	docker-compose -f docker-compose.full-stack.yml ps
}

# Function to stop all
function Stop-All {
	Write-Host "🛑 Stopping all containers..." -ForegroundColor Red
	docker stop kmstraining-reactui 2>$null
	docker stop kmstraining-reactui-dev 2>$null
	docker-compose down 2>$null
	docker-compose -f docker-compose.full-stack.yml down 2>$null
	Write-Host "✅ All containers stopped!" -ForegroundColor Green
}

# Function to view logs
function Show-Logs {
	Write-Host "Select logs to view:" -ForegroundColor Yellow
	Write-Host "1. Production UI"
	Write-Host "2. Development UI"
	Write-Host "3. Docker Compose"
	Write-Host "4. Full Stack"
	$logChoice = Read-Host "Enter choice (1-4)"

	switch ($logChoice) {
		1 { docker logs -f kmstraining-reactui }
		2 { docker logs -f kmstraining-reactui-dev }
		3 { docker-compose logs -f }
		4 { docker-compose -f docker-compose.full-stack.yml logs -f }
	}
}

# Function to clean up
function Clean-Up {
	Write-Host "🧹 Cleaning up..." -ForegroundColor Red
	Write-Host "⚠️  This will remove all containers and images!" -ForegroundColor Yellow
	$confirm = Read-Host "Are you sure? (y/n)"

	if ($confirm -eq 'y') {
		Stop-All
		docker rm kmstraining-reactui 2>$null
		docker rm kmstraining-reactui-dev 2>$null
		docker-compose -f docker-compose.full-stack.yml down -v 2>$null
		docker rmi kmstraining-reactui:latest 2>$null
		docker rmi kmstraining-reactui:dev 2>$null
		Write-Host "✅ Cleanup complete!" -ForegroundColor Green
	}
}

# Main loop
do {
	Show-Menu
	$choice = Read-Host "Enter your choice (1-8)"
	Write-Host ""

	switch ($choice) {
		1 { Start-Production }
		2 { Start-Development }
		3 { Start-Compose }
		4 { Start-FullStack }
		5 { Stop-All }
		6 { Show-Logs }
		7 { Clean-Up }
		8 { 
			Write-Host "👋 Goodbye!" -ForegroundColor Cyan
			exit 
		}
		default { Write-Host "❌ Invalid choice!" -ForegroundColor Red }
	}

	Write-Host ""
	Write-Host "Press any key to continue..."
	$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
	Clear-Host
} while ($true)
