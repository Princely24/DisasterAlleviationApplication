# Git Repository Setup Script for Azure DevOps
# Run this script to initialize your repository and push to Azure Repos

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Disaster Relief Application" -ForegroundColor Cyan
Write-Host "Git Repository Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if git is installed
try {
    $gitVersion = git --version
    Write-Host "✓ Git is installed: $gitVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Git is not installed!" -ForegroundColor Red
    Write-Host "Please install Git from: https://git-scm.com/download/win" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Step 1: Configure Git User" -ForegroundColor Yellow
Write-Host "----------------------------------------"

$userName = Read-Host "Enter your name (e.g., John Doe)"
$userEmail = Read-Host "Enter your email (e.g., john@example.com)"

git config --global user.name "$userName"
git config --global user.email "$userEmail"

Write-Host "✓ Git user configured" -ForegroundColor Green

Write-Host ""
Write-Host "Step 2: Initialize Repository" -ForegroundColor Yellow
Write-Host "----------------------------------------"

# Check if already initialized
if (Test-Path ".git") {
    Write-Host "✓ Git repository already initialized" -ForegroundColor Green
} else {
    git init
    Write-Host "✓ Git repository initialized" -ForegroundColor Green
}

Write-Host ""
Write-Host "Step 3: Add Remote Repository" -ForegroundColor Yellow
Write-Host "----------------------------------------"

$azureRepoUrl = Read-Host "Enter your Azure Repos URL (e.g., https://dev.azure.com/YOUR-ORG/DisasterReliefApplication/_git/DisasterReliefApp)"

# Remove existing origin if it exists
git remote remove origin 2>$null

git remote add origin $azureRepoUrl
Write-Host "✓ Remote repository added" -ForegroundColor Green

Write-Host ""
Write-Host "Step 4: Create Initial Commit" -ForegroundColor Yellow
Write-Host "----------------------------------------"

git add .
git commit -m "Initial commit: Disaster Relief Application with CI/CD configuration"
Write-Host "✓ Initial commit created" -ForegroundColor Green

Write-Host ""
Write-Host "Step 5: Create and Push Branches" -ForegroundColor Yellow
Write-Host "----------------------------------------"

# Push main branch
Write-Host "Pushing main branch..." -ForegroundColor Cyan
git branch -M main
git push -u origin main
Write-Host "✓ Main branch pushed" -ForegroundColor Green

# Create and push develop branch
Write-Host "Creating develop branch..." -ForegroundColor Cyan
git checkout -b develop
git push -u origin develop
Write-Host "✓ Develop branch created and pushed" -ForegroundColor Green

# Return to main
git checkout main

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "✓ Git Repository Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Go to Azure DevOps → Repos → Branches" -ForegroundColor White
Write-Host "2. Set 'develop' as the default branch" -ForegroundColor White
Write-Host "3. Configure branch policies (see AZURE_DEVOPS_SETUP.md)" -ForegroundColor White
Write-Host "4. Create your first feature branch:" -ForegroundColor White
Write-Host "   git checkout -b feature/my-feature develop" -ForegroundColor Cyan
Write-Host ""
Write-Host "Documentation:" -ForegroundColor Yellow
Write-Host "- Branching Strategy: BRANCHING_STRATEGY.md" -ForegroundColor White
Write-Host "- Azure DevOps Setup: AZURE_DEVOPS_SETUP.md" -ForegroundColor White
Write-Host "- Project README: README.md" -ForegroundColor White
Write-Host ""
