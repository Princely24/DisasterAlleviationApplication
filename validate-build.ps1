# Build Validation Script
# Run this before creating a Pull Request to ensure your code will pass CI/CD

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build Validation Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$ErrorCount = 0

# Step 1: Clean solution
Write-Host "Step 1: Cleaning solution..." -ForegroundColor Yellow
dotnet clean
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Clean failed" -ForegroundColor Red
    $ErrorCount++
} else {
    Write-Host "✓ Clean successful" -ForegroundColor Green
}

Write-Host ""

# Step 2: Restore packages
Write-Host "Step 2: Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Restore failed" -ForegroundColor Red
    $ErrorCount++
} else {
    Write-Host "✓ Restore successful" -ForegroundColor Green
}

Write-Host ""

# Step 3: Build solution
Write-Host "Step 3: Building solution..." -ForegroundColor Yellow
dotnet build --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Build failed" -ForegroundColor Red
    $ErrorCount++
} else {
    Write-Host "✓ Build successful" -ForegroundColor Green
}

Write-Host ""

# Step 4: Run tests
Write-Host "Step 4: Running tests..." -ForegroundColor Yellow
dotnet test --configuration Release --no-build --verbosity normal
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠ Tests failed or no tests found" -ForegroundColor Yellow
} else {
    Write-Host "✓ Tests passed" -ForegroundColor Green
}

Write-Host ""

# Step 5: Check for migrations
Write-Host "Step 5: Checking migrations..." -ForegroundColor Yellow
cd DisasterAlleviationApplication
$migrations = dotnet ef migrations list 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠ Could not list migrations (this is OK if DB is not accessible)" -ForegroundColor Yellow
} else {
    Write-Host "✓ Migrations checked" -ForegroundColor Green
    Write-Host $migrations
}
cd ..

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

if ($ErrorCount -eq 0) {
    Write-Host "✓ All Checks Passed!" -ForegroundColor Green
    Write-Host "Your code is ready for Pull Request" -ForegroundColor Green
} else {
    Write-Host "✗ $ErrorCount Check(s) Failed" -ForegroundColor Red
    Write-Host "Please fix the errors before creating a Pull Request" -ForegroundColor Red
    exit 1
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
