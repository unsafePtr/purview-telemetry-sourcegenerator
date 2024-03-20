# Variables
ROOT_FOLDER = src/
SOLUTION_FILE = $(ROOT_FOLDER)Purview.Telemetry.SourceGenerator.sln
TEST_PROJECT = $(ROOT_FOLDER)Purview.Telemetry.SourceGenerator.sln
CONFIGURATION = Debug

PACK_VERSION = 0.0.6-prerelease
ARTIFACT_FOLDER = p:/sync-projects/.local-nuget/

# Targets
build:
	dotnet build $(SOLUTION_FILE) --configuration $(CONFIGURATION)

test:
	dotnet test $(TEST_PROJECT) --configuration $(CONFIGURATION)

pack:
	dotnet pack -c $(CONFIGURATION) -o $(ARTIFACT_FOLDER) $(ROOT_FOLDER)Purview.Telemetry.SourceGenerator/Purview.Telemetry.SourceGenerator.csproj --property:Version=$(PACK_VERSION)

format:
	dotnet format $(ROOT_FOLDER)

.PHONY: build test
