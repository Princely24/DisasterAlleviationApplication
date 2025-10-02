# Push to GitHub Script
# This script will help you push your project to GitHub

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Push to GitHub" -ForegroundColor Cyan
Write-Host "Disaster Relief Application" -ForegroundColor Cyan
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
Write-Host "Step 1: Configure Git User (if not already done)" -ForegroundColor Yellow
Write-Host "----------------------------------------"

$currentUser = git config --global user.name
$currentEmail = git config --global user.email

if ($currentUser) {
    Write-Host "Current Git user: $currentUser <$currentEmail>" -ForegroundColor Cyan
    $changeUser = Read-Host "Do you want to change it? (y/n)"
    if ($changeUser -eq "y") {
        $userName = Read-Host "Enter your name"
        $userEmail = Read-Host "Enter your email"
        git config --global user.name "$userName"
        git config --global user.email "$userEmail"
    }
} else {
    $userName = Read-Host "Enter your name (e.g., John Doe)"
    $userEmail = Read-Host "Enter your email (e.g., john@example.com)"
    git config --global user.name "$userName"
    git config --global user.email "$userEmail"
}

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
Write-Host "Step 3: GitHub Repository URL" -ForegroundColor Yellow
Write-Host "----------------------------------------"
Write-Host ""
Write-Host "Before continuing, create a new repository on GitHub:" -ForegroundColor Cyan
Write-Host "1. Go to https://github.com/new" -ForegroundColor White
Write-Host "2. Repository name: DisasterAlleviationApplication" -ForegroundColor White
Write-Host "3. Description: Disaster Relief Management System" -ForegroundColor White
Write-Host "4. Visibility: Public or Private" -ForegroundColor White
Write-Host "5. DO NOT initialize with README, .gitignore, or license" -ForegroundColor Yellow
Write-Host "6. Click 'Create repository'" -ForegroundColor White
Write-Host ""

$githubUrl = Read-Host "Enter your GitHub repository URL (e.g., https://github.com/username/DisasterAlleviationApplication.git)"

# Remove existing origin if it exists
git remote remove origin 2>$null

# Add GitHub as remote
git remote add origin $githubUrl
Write-Host "✓ GitHub remote added" -ForegroundColor Green

Write-Host ""
Write-Host "Step 4: Add Files and Commit" -ForegroundColor Yellow
Write-Host "----------------------------------------"

# Check if there are any commits
$hasCommits = git log --oneline 2>$null
if (-not $hasCommits) {
    Write-Host "Creating initial commit..." -ForegroundColor Cyan
    git add .
    git commit -m "Initial commit: Disaster Relief Application with CI/CD configuration"
    Write-Host "✓ Initial commit created" -ForegroundColor Green
} else {
    Write-Host "✓ Repository already has commits" -ForegroundColor Green
}

Write-Host ""
Write-Host "Step 5: Push to GitHub" -ForegroundColor Yellow
Write-Host "----------------------------------------"

# Set main branch
git branch -M main

Write-Host "Pushing to GitHub..." -ForegroundColor Cyan
Write-Host ""
Write-Host "Note: You may be prompted for GitHub credentials" -ForegroundColor Yellow
Write-Host "If using 2FA, you'll need a Personal Access Token instead of password" -ForegroundColor Yellow
Write-Host ""

try {
    git push -u origin main
    Write-Host "✓ Successfully pushed to GitHub!" -ForegroundColor Green
} catch {
    Write-Host "✗ Push failed" -ForegroundColor Red
    Write-Host ""
    Write-Host "Common issues:" -ForegroundColor Yellow
    Write-Host "1. Authentication failed - Use Personal Access Token" -ForegroundColor White
    Write-Host "2. Repository not empty - Force push with: git push -u origin main --force" -ForegroundColor White
    Write-Host "3. Network issues - Check your internet connection" -ForegroundColor White
    exit 1
}

Write-Host ""
Write-Host "Step 6: Create Additional Branches (Optional)" -ForegroundColor Yellow
Write-Host "----------------------------------------"

$createBranches = Read-Host "Do you want to create 'develop' branch? (y/n)"
if ($createBranches -eq "y") {
    git checkout -b develop
    git push -u origin develop
    git checkout main
    Write-Host "✓ Develop branch created and pushed" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "✓ Successfully Pushed to GitHub!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Your repository is now available at:" -ForegroundColor Yellow
Write-Host $githubUrl.Replace(".git", "") -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Yellow
Write-Host "1. Visit your GitHub repository" -ForegroundColor White
Write-Host "2. Add a description and topics" -ForegroundColor White
Write-Host "3. Configure branch protection rules (Settings → Branches)" -ForegroundColor White
Write-Host "4. Add collaborators if needed (Settings → Collaborators)" -ForegroundColor White
Write-Host ""
