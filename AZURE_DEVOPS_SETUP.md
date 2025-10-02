# Azure DevOps Setup Guide
## Disaster Relief Application - CI/CD Configuration

---

## Part 1: Azure Repos Setup (10 Marks)

### Step 1: Create Azure DevOps Project

1. **Go to Azure DevOps:** https://dev.azure.com
2. **Sign in** with your Microsoft account
3. **Create New Project:**
   - Click "+ New Project"
   - **Project Name:** `DisasterReliefApplication`
   - **Visibility:** Private
   - **Version Control:** Git
   - **Work Item Process:** Agile
   - Click "Create"

### Step 2: Initialize Git Repository

#### Option A: Push Existing Code to Azure Repos

```powershell
# Navigate to your project directory
cd C:\Users\Uesr\source\repos\DisasterAlleviationApplication

# Initialize git (if not already initialized)
git init

# Add all files
git add .

# Initial commit
git commit -m "Initial commit: Disaster Relief Application"

# Add Azure Repos as remote
git remote add origin https://dev.azure.com/YOUR-ORG/DisasterReliefApplication/_git/DisasterReliefApp

# Push to Azure Repos
git push -u origin main
```

#### Option B: Clone from Azure Repos

```powershell
# Get the clone URL from Azure DevOps (Repos → Files → Clone)
git clone https://dev.azure.com/YOUR-ORG/DisasterReliefApplication/_git/DisasterReliefApp

# Copy your project files into the cloned directory
# Then commit and push
```

### Step 3: Create Branch Structure (Gitflow)

```powershell
# Create and push develop branch
git checkout -b develop
git push origin develop

# Set develop as default branch in Azure DevOps:
# Repos → Branches → ... (next to develop) → Set as default branch
```

### Step 4: Configure Branch Policies

#### For `main` Branch:

1. **Go to:** Repos → Branches
2. **Click:** ... (next to `main`) → Branch policies
3. **Configure:**
   - ✅ **Require a minimum number of reviewers:** 1
   - ✅ **Check for linked work items:** Optional
   - ✅ **Check for comment resolution:** Required
   - ✅ **Limit merge types:** Squash merge only
   - ✅ **Build validation:** Add build pipeline (after creating it)

#### For `develop` Branch:

1. **Go to:** Repos → Branches
2. **Click:** ... (next to `develop`) → Branch policies
3. **Configure:**
   - ✅ **Require a minimum number of reviewers:** 1
   - ✅ **Build validation:** Add build pipeline

### Step 5: Create Feature Branches

```powershell
# Example: Create feature branch for admin panel
git checkout develop
git checkout -b feature/admin-panel

# Work on feature...
git add .
git commit -m "Add: Admin panel with volunteer approval"

# Push feature branch
git push origin feature/admin-panel
```

### Step 6: Create Pull Request

1. **Go to:** Repos → Pull Requests
2. **Click:** "New Pull Request"
3. **Configure:**
   - **Source:** `feature/admin-panel`
   - **Target:** `develop`
   - **Title:** "Add: Admin panel with volunteer approval"
   - **Description:** Describe changes, testing done
   - **Reviewers:** Add team members
   - **Work Items:** Link related work items
4. **Click:** "Create"

### Step 7: Code Review Process

**Reviewer Checklist:**
- [ ] Code follows project standards
- [ ] No security vulnerabilities
- [ ] Tests are included
- [ ] Documentation updated
- [ ] Build passes
- [ ] No merge conflicts

**Actions:**
- **Approve:** If all checks pass
- **Request Changes:** If issues found
- **Comment:** For questions or suggestions

### Step 8: Complete Pull Request

1. **After approval:** Click "Complete"
2. **Merge Type:** Squash commit
3. **Delete source branch:** ✅ Yes
4. **Complete work items:** ✅ Yes

---

## Part 2: Azure Pipelines Setup (10 Marks)

### Step 1: Create Build Pipeline

1. **Go to:** Pipelines → Pipelines
2. **Click:** "New Pipeline"
3. **Select:** "Azure Repos Git"
4. **Choose:** Your repository
5. **Configure:** "Existing Azure Pipelines YAML file"
6. **Path:** `/azure-pipelines.yml`
7. **Click:** "Continue" → "Run"

### Step 2: Configure Pipeline Triggers

The `azure-pipelines.yml` file includes:

```yaml
trigger:
  branches:
    include:
    - main
    - develop
    - release/*
```

**This means:**
- ✅ Builds trigger on push to `main`
- ✅ Builds trigger on push to `develop`
- ✅ Builds trigger on push to any `release/*` branch
- ✅ Pull requests trigger validation builds

### Step 3: Pipeline Stages Explained

#### Stage 1: Build
**Tasks:**
1. **Install .NET SDK** - Ensures .NET 8.0 is available
2. **Restore NuGet Packages** - Downloads dependencies
3. **Restore Dependencies** - Restores project dependencies
4. **Build Solution** - Compiles the application
5. **Run Unit Tests** - Executes all tests
6. **Publish Code Coverage** - Shows test coverage
7. **Publish Application** - Creates deployment package
8. **Publish Build Artifacts** - Stores build output

#### Stage 2: Deploy to Test
**Conditions:**
- Only runs if Build stage succeeds
- Only runs on `develop` branch

**Tasks:**
1. Download build artifacts
2. Deploy to Azure Web App (Test environment)

#### Stage 3: Deploy to Production
**Conditions:**
- Only runs if Deploy to Test succeeds
- Only runs on `main` branch

**Tasks:**
1. Download build artifacts
2. Deploy to Azure Web App (Production environment)

### Step 4: Create Azure Web Apps

#### Test Environment:
```powershell
# Create resource group
az group create --name DisasterRelief-RG --location eastus

# Create App Service Plan
az appservice plan create --name DisasterRelief-Plan --resource-group DisasterRelief-RG --sku B1

# Create Web App for Test
az webapp create --name DisasterRelief-Test --resource-group DisasterRelief-RG --plan DisasterRelief-Plan --runtime "DOTNET|8.0"
```

#### Production Environment:
```powershell
# Create Web App for Production
az webapp create --name DisasterRelief-Prod --resource-group DisasterRelief-RG --plan DisasterRelief-Plan --runtime "DOTNET|8.0"
```

### Step 5: Configure Service Connection

1. **Go to:** Project Settings → Service connections
2. **Click:** "New service connection"
3. **Select:** "Azure Resource Manager"
4. **Authentication:** Service principal (automatic)
5. **Scope:** Subscription
6. **Resource Group:** DisasterRelief-RG
7. **Service connection name:** `Azure-Service-Connection`
8. **Click:** "Save"

### Step 6: Update Pipeline Variables

1. **Go to:** Pipelines → Your pipeline → Edit
2. **Update these values in `azure-pipelines.yml`:**
   ```yaml
   azureSubscription: 'Azure-Service-Connection'  # Your service connection name
   appName: 'DisasterRelief-Test'                 # Your test app name
   ```

### Step 7: Configure Environments

1. **Go to:** Pipelines → Environments
2. **Create Test Environment:**
   - Name: `Test`
   - Description: "Test/Staging environment"
   - Approvals: None (auto-deploy)

3. **Create Production Environment:**
   - Name: `Production`
   - Description: "Production environment"
   - **Approvals:** Add approvers (require manual approval before production deploy)

### Step 8: Configure Database Connection Strings

#### In Azure Web App:

1. **Go to:** Azure Portal → Your Web App
2. **Navigate to:** Configuration → Application settings
3. **Add Connection String:**
   - **Name:** `DefaultConnection`
   - **Value:** Your Azure SQL connection string
   - **Type:** SQLAzure
4. **Click:** "Save"

### Step 9: Monitor Pipeline Runs

1. **Go to:** Pipelines → Pipelines
2. **Click:** Your pipeline name
3. **View:**
   - Recent runs
   - Success/failure status
   - Build duration
   - Test results
   - Deployment status

### Step 10: View Pipeline Analytics

1. **Go to:** Pipelines → Analytics
2. **View:**
   - Pass rate
   - Pipeline duration
   - Test pass rate
   - Failure trends

---

## Pipeline Features

### ✅ Automated Build
- Triggers on every push to `main`, `develop`, or `release/*`
- Compiles code
- Runs tests
- Creates deployment package

### ✅ Automated Testing
- Runs all unit tests
- Publishes test results
- Shows code coverage
- Fails build if tests fail

### ✅ Automated Deployment
- **Test Environment:** Auto-deploys from `develop` branch
- **Production Environment:** Auto-deploys from `main` branch (with approval)

### ✅ Build Artifacts
- Stores compiled application
- Available for download
- Used for deployment

### ✅ Notifications
- Email on build failure
- Slack/Teams integration available
- Pull request status updates

---

## Testing the Pipeline

### Test 1: Feature Branch Build

```powershell
# Create feature branch
git checkout -b feature/test-pipeline develop

# Make a small change
echo "# Test" >> README.md

# Commit and push
git add .
git commit -m "Add: Test pipeline configuration"
git push origin feature/test-pipeline

# Create Pull Request in Azure DevOps
# Watch the build pipeline run automatically
```

### Test 2: Deploy to Test

```powershell
# Merge feature to develop (via Pull Request)
# Pipeline will automatically:
# 1. Build the application
# 2. Run tests
# 3. Deploy to Test environment

# Verify deployment at: https://disasterrelief-test.azurewebsites.net
```

### Test 3: Deploy to Production

```powershell
# Create release branch
git checkout -b release/1.0.0 develop
git push origin release/1.0.0

# Create Pull Request: release/1.0.0 → main
# After approval and merge:
# Pipeline will deploy to production (with manual approval)
```

---

## Troubleshooting

### Build Fails:
1. Check build logs in Azure Pipelines
2. Verify .NET SDK version matches
3. Ensure all NuGet packages restore correctly
4. Check for compilation errors

### Deployment Fails:
1. Verify service connection is valid
2. Check Azure Web App exists
3. Verify connection strings are configured
4. Check deployment logs

### Tests Fail:
1. Run tests locally first
2. Check test project configuration
3. Verify test dependencies
4. Review test logs in pipeline

---

## Best Practices

### ✅ Do:
- Commit frequently with clear messages
- Create small, focused pull requests
- Write tests for new features
- Review code thoroughly
- Keep branches up to date
- Delete branches after merge

### ❌ Don't:
- Commit directly to `main` or `develop`
- Create large, monolithic pull requests
- Skip code reviews
- Ignore failing tests
- Leave stale branches
- Commit sensitive data (passwords, keys)

---

## Monitoring & Maintenance

### Daily:
- Review failed builds
- Check pending pull requests
- Monitor test results

### Weekly:
- Review pipeline performance
- Clean up old branches
- Update dependencies

### Monthly:
- Review branch policies
- Update pipeline configuration
- Audit security settings

---

## Resources

- **Azure DevOps Docs:** https://docs.microsoft.com/azure/devops/
- **Gitflow Workflow:** https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow
- **Azure Pipelines:** https://docs.microsoft.com/azure/devops/pipelines/
- **Branch Policies:** https://docs.microsoft.com/azure/devops/repos/git/branch-policies

---

## Support

For issues or questions:
1. Check Azure DevOps documentation
2. Review pipeline logs
3. Contact team lead
4. Create work item in Azure Boards
