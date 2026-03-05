# DonaRogApp - Test Runner Script
# PowerShell script per eseguire i test automatici

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   DonaRogApp - Test Suite Runner      " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET SDK is installed
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: .NET SDK not found. Please install .NET 9 SDK." -ForegroundColor Red
    exit 1
}
Write-Host "✓ .NET SDK version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Menu
Write-Host "Select test option:" -ForegroundColor Cyan
Write-Host "1. Run ALL tests" -ForegroundColor White
Write-Host "2. Run Application tests only" -ForegroundColor White
Write-Host "3. Run Domain tests only" -ForegroundColor White
Write-Host "4. Run tests with CODE COVERAGE" -ForegroundColor White
Write-Host "5. Run specific test (Segmentation)" -ForegroundColor White
Write-Host "6. Run specific test (Donors)" -ForegroundColor White
Write-Host "7. Run specific test (Donations)" -ForegroundColor White
Write-Host "8. Run specific test (Template Selection - LRU)" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Enter your choice (1-8)"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Running Tests...                     " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

switch ($choice) {
    "1" {
        Write-Host "Running ALL tests..." -ForegroundColor Yellow
        dotnet test --logger "console;verbosity=normal"
    }
    "2" {
        Write-Host "Running Application tests..." -ForegroundColor Yellow
        dotnet test test/DonaRogApp.Application.Tests/DonaRogApp.Application.Tests.csproj --logger "console;verbosity=normal"
    }
    "3" {
        Write-Host "Running Domain tests..." -ForegroundColor Yellow
        dotnet test test/DonaRogApp.Domain.Tests/DonaRogApp.Domain.Tests.csproj --logger "console;verbosity=normal"
    }
    "4" {
        Write-Host "Running tests with CODE COVERAGE..." -ForegroundColor Yellow
        dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/
        
        Write-Host ""
        Write-Host "Coverage report generated: ./TestResults/coverage.cobertura.xml" -ForegroundColor Green
        Write-Host ""
        Write-Host "To view HTML report, install reportgenerator:" -ForegroundColor Cyan
        Write-Host "  dotnet tool install --global dotnet-reportgenerator-globaltool" -ForegroundColor White
        Write-Host "Then run:" -ForegroundColor Cyan
        Write-Host "  reportgenerator -reports:./TestResults/coverage.cobertura.xml -targetdir:./TestResults/CoverageReport -reporttypes:Html" -ForegroundColor White
    }
    "5" {
        Write-Host "Running Segmentation tests..." -ForegroundColor Yellow
        dotnet test --filter "FullyQualifiedName~Segmentation" --logger "console;verbosity=detailed"
    }
    "6" {
        Write-Host "Running Donor tests..." -ForegroundColor Yellow
        dotnet test --filter "FullyQualifiedName~Donor" --logger "console;verbosity=detailed"
    }
    "7" {
        Write-Host "Running Donation tests..." -ForegroundColor Yellow
        dotnet test --filter "FullyQualifiedName~Donation" --logger "console;verbosity=detailed"
    }
    "8" {
        Write-Host "Running Template Selection (LRU) tests..." -ForegroundColor Yellow
        dotnet test --filter "FullyQualifiedName~TemplateSelection" --logger "console;verbosity=detailed"
    }
    default {
        Write-Host "Invalid choice. Running ALL tests by default..." -ForegroundColor Yellow
        dotnet test --logger "console;verbosity=normal"
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✓ Tests PASSED!                    " -ForegroundColor Green
} else {
    Write-Host "   ✗ Tests FAILED!                    " -ForegroundColor Red
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Pause to see results
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
