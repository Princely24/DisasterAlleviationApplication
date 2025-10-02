# Git Branching Strategy - Gitflow

## Overview
This project uses the **Gitflow** branching strategy to manage code development, testing, and production releases.

## Branch Structure

### Main Branches (Permanent)

#### 1. `main` (Production)
- **Purpose:** Production-ready code
- **Protected:** Yes - requires pull request approval
- **Deployment:** Automatically deploys to production environment
- **Naming:** `main`
- **Merge from:** `release/*` branches only

#### 2. `develop` (Development)
- **Purpose:** Integration branch for features
- **Protected:** Yes - requires pull request approval
- **Deployment:** Automatically deploys to test environment
- **Naming:** `develop`
- **Merge from:** `feature/*`, `bugfix/*` branches

### Supporting Branches (Temporary)

#### 3. Feature Branches
- **Purpose:** Develop new features
- **Naming Convention:** `feature/<feature-name>`
- **Examples:**
  - `feature/volunteer-registration`
  - `feature/donation-tracking`
  - `feature/incident-reporting`
- **Created from:** `develop`
- **Merged into:** `develop`
- **Lifetime:** Deleted after merge

#### 4. Release Branches
- **Purpose:** Prepare for production release
- **Naming Convention:** `release/<version>`
- **Examples:**
  - `release/1.0.0`
  - `release/1.1.0`
- **Created from:** `develop`
- **Merged into:** `main` AND `develop`
- **Lifetime:** Deleted after merge

#### 5. Hotfix Branches
- **Purpose:** Emergency fixes for production
- **Naming Convention:** `hotfix/<issue-name>`
- **Examples:**
  - `hotfix/login-bug`
  - `hotfix/database-connection`
- **Created from:** `main`
- **Merged into:** `main` AND `develop`
- **Lifetime:** Deleted after merge

#### 6. Bugfix Branches
- **Purpose:** Fix bugs in development
- **Naming Convention:** `bugfix/<bug-name>`
- **Examples:**
  - `bugfix/validation-error`
  - `bugfix/ui-alignment`
- **Created from:** `develop`
- **Merged into:** `develop`
- **Lifetime:** Deleted after merge

## Workflow

### 1. Starting a New Feature
```bash
# Switch to develop branch
git checkout develop
git pull origin develop

# Create feature branch
git checkout -b feature/new-feature-name

# Work on feature, commit changes
git add .
git commit -m "Add: Description of changes"

# Push to remote
git push origin feature/new-feature-name

# Create Pull Request in Azure DevOps
# Target: develop
```

### 2. Preparing a Release
```bash
# Create release branch from develop
git checkout develop
git pull origin develop
git checkout -b release/1.0.0

# Make final adjustments (version numbers, etc.)
git commit -m "Prepare release 1.0.0"
git push origin release/1.0.0

# Create Pull Request to main
# After approval, also merge back to develop
```

### 3. Emergency Hotfix
```bash
# Create hotfix from main
git checkout main
git pull origin main
git checkout -b hotfix/critical-bug

# Fix the issue
git commit -m "Fix: Critical bug description"
git push origin hotfix/critical-bug

# Create Pull Requests to both main and develop
```

## Pull Request Requirements

### All Pull Requests Must:
1. **Have a descriptive title** following convention:
   - `Add: <feature>` - New feature
   - `Fix: <bug>` - Bug fix
   - `Update: <component>` - Updates
   - `Refactor: <component>` - Code refactoring

2. **Include description** with:
   - What changed
   - Why it changed
   - How to test

3. **Pass all checks:**
   - Build succeeds
   - Tests pass
   - Code review approved

4. **Be reviewed by at least 1 team member**

5. **Have no merge conflicts**

## Branch Protection Rules

### `main` Branch:
- ✅ Require pull request reviews (minimum 1)
- ✅ Require status checks to pass
- ✅ Require branches to be up to date
- ✅ No direct commits allowed
- ✅ Require linear history

### `develop` Branch:
- ✅ Require pull request reviews (minimum 1)
- ✅ Require status checks to pass
- ✅ No direct commits allowed

## Commit Message Convention

### Format:
```
<type>: <subject>

<body (optional)>

<footer (optional)>
```

### Types:
- **Add:** New feature
- **Fix:** Bug fix
- **Update:** Update existing feature
- **Refactor:** Code refactoring
- **Docs:** Documentation changes
- **Test:** Adding or updating tests
- **Style:** Code style changes (formatting)
- **Chore:** Maintenance tasks

### Examples:
```
Add: Volunteer registration with file upload

- Added file upload functionality for volunteer profiles
- Implemented image validation (max 5MB)
- Updated database schema with new columns

Closes #123
```

```
Fix: Donation form not saving to database

- Added navigation property initialization
- Removed ModelState validation errors for server-set fields
- Added error handling and logging

Fixes #456
```

## Environment Mapping

| Branch | Environment | URL | Auto-Deploy |
|--------|-------------|-----|-------------|
| `main` | Production | https://disasterrelief-prod.azurewebsites.net | ✅ Yes |
| `develop` | Test | https://disasterrelief-test.azurewebsites.net | ✅ Yes |
| `feature/*` | Local | localhost | ❌ No |
| `release/*` | Staging | https://disasterrelief-staging.azurewebsites.net | ⚠️ Manual |

## Team Collaboration Guidelines

### Code Reviews:
1. Review code within 24 hours
2. Provide constructive feedback
3. Approve only if:
   - Code follows standards
   - Tests are included
   - Documentation is updated
   - No security issues

### Pull Request Process:
1. Create feature branch
2. Develop and test locally
3. Push to remote
4. Create pull request
5. Wait for CI/CD checks
6. Request review
7. Address feedback
8. Merge after approval

### Merge Strategy:
- **Feature → Develop:** Squash and merge
- **Release → Main:** Merge commit
- **Hotfix → Main/Develop:** Merge commit

## Quick Reference Commands

```bash
# Clone repository
git clone https://dev.azure.com/your-org/DisasterRelief/_git/DisasterReliefApp

# Create feature branch
git checkout -b feature/my-feature develop

# Update your branch with latest develop
git checkout develop
git pull origin develop
git checkout feature/my-feature
git merge develop

# Push changes
git add .
git commit -m "Add: My feature description"
git push origin feature/my-feature

# Delete local branch after merge
git branch -d feature/my-feature

# Delete remote branch after merge
git push origin --delete feature/my-feature
```

## CI/CD Pipeline Triggers

- **Push to `develop`:** Build → Test → Deploy to Test Environment
- **Push to `main`:** Build → Test → Deploy to Production
- **Pull Request:** Build → Test (no deployment)
- **Manual:** Can trigger any stage manually

## Version Numbering

Follow Semantic Versioning (SemVer): `MAJOR.MINOR.PATCH`

- **MAJOR:** Breaking changes
- **MINOR:** New features (backward compatible)
- **PATCH:** Bug fixes

Examples:
- `1.0.0` - Initial release
- `1.1.0` - Added admin panel
- `1.1.1` - Fixed donation bug
- `2.0.0` - Major redesign (breaking changes)
