# Variables
BACKEND_DOCKER_COMPOSE_PATH = ./docker-compose.yml
BACKEND_SOLUTION = Backend
BACKEND_API_PROJECT = ./$(BACKEND_SOLUTION)/IISBackend.API
BACKEND_DAL_PROJECT = ./$(BACKEND_SOLUTION)/IISBackend.DAL
TEST_SECRET_FILE = ./.config/local-secret.env
SECRET_FILE = ./.config/secret.env
BACKEND_DEPENDENCY_CONTAINERS = db
FRONTEND_DEPENDENCY_CONTAINERS = db api
BUILD_DIR = ./bin/Release/net8.0

# Default target: display help
all: compose

# Target to build the backend project
compose:
	@echo "Building the backend project..."
	docker compose -f "$(BACKEND_DOCKER_COMPOSE_PATH)" --env-file "$(TEST_SECRET_FILE)" up --build -d

deploy:
	@echo "Deploying the backend project..."
	docker compose -f "$(BACKEND_DOCKER_COMPOSE_PATH)" --env-file "$(SECRET_FILE)" up --build -d

backend_build:
	@echo "Building the backend project..."
	dotnet build $(BACKEND_SOLUTION)

backend_compose:
	@echo "Building the backend project..."
	docker compose -f "$(BACKEND_DOCKER_COMPOSE_PATH)" --env-file "$(TEST_SECRET_FILE)" up --build -d $(FRONTEND_DEPENDENCY_CONTAINERS)

# Target to run the backend project
backend_run:
	@echo "Running the backend project..."
	docker compose -f "$(BACKEND_DOCKER_COMPOSE_PATH)" --env-file "$(SECRET_FILE)" up $(BACKEND_DEPENDENCY_CONTAINERS) --build -d
	dotnet run --project $(BACKEND_API_PROJECT)

# Target for `make backend run`
backend: backend_run

migration: 
	@echo "Running the migration..."
	@if [ -z "$(NAME)" ]; then \
		echo "NAME is not set"; \
		exit 1; \
	else \
		echo "migrationName is set to $(NAME)"; \
	fi
	cd $(BACKEND_SOLUTION) && dotnet ef migrations add $(NAME) --project ../$(BACKEND_DAL_PROJECT) --startup-project ../$(BACKEND_API_PROJECT)

remove_migration:
	@echo "Removing the last migration..."
	cd $(BACKEND_SOLUTION) && dotnet ef migrations remove --project ../$(BACKEND_DAL_PROJECT) --startup-project ../$(BACKEND_API_PROJECT)

downgrade_db:
	@echo "Downgrading the database..."
		@echo "Running the migration..."
	@if [ -z "$(NAME)" ]; then \
		echo "NAME is not set"; \
		exit 1; \
	else \
		echo "migrationName is set to $(NAME)"; \
	fi
	cd $(BACKEND_SOLUTION) && dotnet ef database update $(NAME) --project ../$(BACKEND_DAL_PROJECT) --startup-project ../$(BACKEND_API_PROJECT)

# Target to clean the backend project
backend_clean:
	@echo "Cleaning the backend project..."
	dotnet clean $(BACKEND_SOLUTION)

# General help message
help:
	@echo "Available targets:"
	@echo "  backend         - Run the backend project and compose db container"
	@echo "  backend_build   - builds the backend api project"
	@echo "  backend_compose   - Compose the backend container"
	@echo "  backend_run     - Run the backend project and compose db container"
	@echo "  backend_clean   - Clean the backend solution"
	@echo "  migration       - Add the migration with argument NAME=migrationName"
	@echo "  remove_migration - Remove the last migration"
	@echo "  downgrade_db    - Downgrade the database to migration with argument NAME=migrationName"

# Phony targets
.PHONY: all backend backend_build backend_run backend_clean backend_publish backend_test help
