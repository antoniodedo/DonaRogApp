#!/bin/bash
# DonaRogApp - Test Runner Script
# Bash script per eseguire i test automatici (Linux/Mac)

echo "========================================"
echo "   DonaRogApp - Test Suite Runner      "
echo "========================================"
echo ""

# Check if .NET SDK is installed
echo "Checking .NET SDK..."
DOTNET_VERSION=$(dotnet --version 2>/dev/null)
if [ $? -ne 0 ]; then
    echo "ERROR: .NET SDK not found. Please install .NET 9 SDK."
    exit 1
fi
echo "✓ .NET SDK version: $DOTNET_VERSION"
echo ""

# Menu
echo "Select test option:"
echo "1. Run ALL tests"
echo "2. Run Application tests only"
echo "3. Run Domain tests only"
echo "4. Run tests with CODE COVERAGE"
echo "5. Run specific test (Segmentation)"
echo "6. Run specific test (Donors)"
echo "7. Run specific test (Donations)"
echo "8. Run specific test (Template Selection - LRU)"
echo ""

read -p "Enter your choice (1-8): " choice

echo ""
echo "========================================"
echo "   Running Tests...                     "
echo "========================================"
echo ""

case $choice in
    1)
        echo "Running ALL tests..."
        dotnet test --logger "console;verbosity=normal"
        ;;
    2)
        echo "Running Application tests..."
        dotnet test test/DonaRogApp.Application.Tests/DonaRogApp.Application.Tests.csproj --logger "console;verbosity=normal"
        ;;
    3)
        echo "Running Domain tests..."
        dotnet test test/DonaRogApp.Domain.Tests/DonaRogApp.Domain.Tests.csproj --logger "console;verbosity=normal"
        ;;
    4)
        echo "Running tests with CODE COVERAGE..."
        dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/
        
        echo ""
        echo "Coverage report generated: ./TestResults/coverage.cobertura.xml"
        echo ""
        echo "To view HTML report, install reportgenerator:"
        echo "  dotnet tool install --global dotnet-reportgenerator-globaltool"
        echo "Then run:"
        echo "  reportgenerator -reports:./TestResults/coverage.cobertura.xml -targetdir:./TestResults/CoverageReport -reporttypes:Html"
        ;;
    5)
        echo "Running Segmentation tests..."
        dotnet test --filter "FullyQualifiedName~Segmentation" --logger "console;verbosity=detailed"
        ;;
    6)
        echo "Running Donor tests..."
        dotnet test --filter "FullyQualifiedName~Donor" --logger "console;verbosity=detailed"
        ;;
    7)
        echo "Running Donation tests..."
        dotnet test --filter "FullyQualifiedName~Donation" --logger "console;verbosity=detailed"
        ;;
    8)
        echo "Running Template Selection (LRU) tests..."
        dotnet test --filter "FullyQualifiedName~TemplateSelection" --logger "console;verbosity=detailed"
        ;;
    *)
        echo "Invalid choice. Running ALL tests by default..."
        dotnet test --logger "console;verbosity=normal"
        ;;
esac

echo ""
echo "========================================"

if [ $? -eq 0 ]; then
    echo "   ✓ Tests PASSED!                    "
else
    echo "   ✗ Tests FAILED!                    "
fi

echo "========================================"
echo ""
