# Quick Start Guide - Azure DevOps Setup
## Complete Setup in 30 Minutes

---

## 📋 Prerequisites Checklist

Before starting, ensure you have:
- [ ] Azure DevOps account (free at https://dev.azure.com)
- [ ] Azure subscription (for deployment)
- [ ] Git installed on your machine
- [ ] .NET 8.0 SDK installed
- [ ] Visual Studio 2022 or VS Code

---

## Part 1: Azure Repos Setup (10 Marks)

### ⏱️ Time: 15 minutes

### Step 1: Create Azure DevOps Project (3 minutes)

1. Go to https://dev.azure.com
2. Click **"+ New Project"**
3. Fill in details:
   - **Name:** `DisasterReliefApplication`
   - **Visibility:** Private
   - **Version Control:** Git
   - **Work Item Process:** Agile
4. Click **"Create"**

✅ **Marks: 2/10**

---

### Step 2: Push Code to Azure Repos (5 minutes)

#### Option A: Using PowerShell Script (Recommended)

```powershell
# Run the setup script
.\setup-git.ps1

# Follow the prompts:
# 1. Enter your name
# 2. Enter your email
# 3. Enter Azure Repos URL from: Repos → Files → Clone
```

#### Option B: Manual Setup

```powershell
# Initialize Git
git init

# Add all files
git add .

# Initial commit
git commit -m "Initial commit: Disaster Relief Application"

# Add remote (get URL from Azure DevOps → Repos → Clone)
git remote add origin https://dev.azure.com/YOUR-ORG/DisasterReliefApplication/_git/DisasterReliefApp

# Push main branch
git branch -M main
git push -u origin main

# Create develop branch
git checkout -b develop
git push -u origin develop
```

✅ **Marks: 4/10**

---

### Step 3: Configure Gitflow Branching (4 minutes)

#### Set Default Branch:
1. Go to **Repos → Branches**
2. Click **"..."** next to `develop`
3. Select **"Set as default branch"**

#### Configure Branch Policies for `main`:
1. Go to **Repos → Branches**
2. Click **"..."** next to `main` → **"Branch policies"**
3. Enable:
   - ✅ **Require a minimum number of reviewers:** 1
   - ✅ **Check for linked work items:** Optional
   - ✅ **Check for comment resolution:** All comments must be resolved
   - ✅ **Limit merge types:** Squash merge only
4. Click **"Save changes"**

#### Configure Branch Policies for `develop`:
1. Click **"..."** next to `develop` → **"Branch policies"**
2. Enable:
   - ✅ **Require a minimum number of reviewers:** 1
   - ✅ **Check for comment resolution:** All comments must be resolved
3. Click **"Save changes"**

✅ **Marks: 7/10**

---

### Step 4: Create Feature Branch & Pull Request (3 minutes)

```powershell
# Create feature branch
git checkout develop
git checkout -b feature/test-pipeline

# Make a small change
echo "# Test Pipeline" >> README.md

# Commit and push
git add .
git commit -m "Add: Test pipeline configuration"
git push origin feature/test-pipeline
```

#### Create Pull Request:
1. Go to **Repos → Pull Requests**
2. Click **"New Pull Request"**
3. Configure:
   - **Source:** `feature/test-pipeline`
   - **Target:** `develop`
   - **Title:** "Add: Test pipeline configuration"
   - **Description:** Use the template
4. Click **"Create"**

✅ **Marks: 10/10** ✨

---

## Part 2: Azure Pipelines Setup (10 Marks)

### ⏱️ Time: 15 minutes

### Step 1: Create Build Pipeline (5 minutes)

1. Go to **Pipelines → Pipelines**
2. Click **"New Pipeline"**
3. Select **"Azure Repos Git"**
4. Choose your repository: `DisasterReliefApp`
5. Select **"Existing Azure Pipelines YAML file"**
6. **Path:** `/azure-pipelines.yml`
7. Click **"Continue"**
8. Review the YAML configuration
9. Click **"Run"**

✅ **Marks: 3/10**

---

### Step 2: Configure Pipeline Triggers (2 minutes)

The `azure-pipelines.yml` already includes triggers:

```yaml
trigger:
  branches:
    include:
    - main
    - develop
    - release/*
```

**Verify triggers work:**
1. Make a small change to any file
2. Commit and push to `develop`
3. Watch pipeline automatically start

✅ **Marks: 5/10**

---

### Step 3: Add Build Tasks (Already Configured)

The pipeline includes these tasks:

1. ✅ **Install .NET SDK** - Sets up build environment
2. ✅ **Restore NuGet Packages** - Downloads dependencies
3. ✅ **Restore Dependencies** - Restores project dependencies
4. ✅ **Build Solution** - Compiles the code
5. ✅ **Run Unit Tests** - Executes automated tests
6. ✅ **Publish Code Coverage** - Shows test coverage
7. ✅ **Publish Application** - Creates deployment package
8. ✅ **Publish Build Artifacts** - Stores build output

✅ **Marks: 7/10**

---

### Step 4: Create Test Environment (3 minutes)

#### Create Azure Web App for Test:

```powershell
# Login to Azure
az login

# Create resource group
az group create --name DisasterRelief-RG --location eastus

# Create App Service Plan
az appservice plan create --name DisasterRelief-Plan --resource-group DisasterRelief-RG --sku B1 --is-linux

# Create Web App for Test Environment
az webapp create --name DisasterRelief-Test --resource-group DisasterRelief-RG --plan DisasterRelief-Plan --runtime "DOTNETCORE:8.0"
```

#### Configure Service Connection:

1. Go to **Project Settings → Service connections**
2. Click **"New service connection"**
3. Select **"Azure Resource Manager"**
4. Choose **"Service principal (automatic)"**
5. **Subscription:** Select your Azure subscription
6. **Resource Group:** DisasterRelief-RG
7. **Service connection name:** `Azure-Service-Connection`
8. Click **"Save"**

✅ **Marks: 9/10**

---

### Step 5: Create Environments & Enable Deployment (1 minute)

1. Go to **Pipelines → Environments**
2. Click **"New environment"**
3. Create **Test** environment:
   - **Name:** `Test`
   - **Description:** "Test/Staging environment"
   - **Resource:** None
   - Click **"Create"**

4. Create **Production** environment:
   - **Name:** `Production`
   - **Description:** "Production environment"
   - **Approvals:** Add yourself as approver
   - Click **"Create"**

✅ **Marks: 10/10** ✨

---

## 🎯 Verification Checklist

### Azure Repos (10 Marks):
- [x] Azure DevOps project created
- [x] Git repository initialized
- [x] Code pushed to Azure Repos
- [x] `main` and `develop` branches created
- [x] Gitflow branching strategy documented
- [x] Branch policies configured
- [x] Pull request template created
- [x] Feature branch created
- [x] Pull request created and reviewed
- [x] Code review process demonstrated

### Azure Pipelines (10 Marks):
- [x] Build pipeline created
- [x] YAML configuration file (`azure-pipelines.yml`)
- [x] Automatic triggers configured
- [x] Build tasks included (compile, test, publish)
- [x] Test execution configured
- [x] Artifact publishing configured
- [x] Test environment deployment configured
- [x] Service connection created
- [x] Environments created
- [x] Deployment automation working

---

## 📸 Evidence for Submission

### Screenshots to Take:

1. **Azure Repos:**
   - Repository with `main` and `develop` branches
   - Branch policies configuration
   - Pull request with code review
   - Commit history showing Gitflow

2. **Azure Pipelines:**
   - Pipeline overview showing stages
   - Successful build run
   - Test results
   - Deployment to test environment
   - Build artifacts

3. **Azure Portal:**
   - Resource group with web apps
   - Test environment running
   - Application settings configured

---

## 🚀 Test the Complete Workflow

### End-to-End Test (5 minutes):

```powershell
# 1. Create feature branch
git checkout develop
git pull origin develop
git checkout -b feature/test-workflow

# 2. Make a change
echo "# Workflow Test" >> README.md

# 3. Commit and push
git add .
git commit -m "Add: Test workflow documentation"
git push origin feature/test-workflow

# 4. Create Pull Request in Azure DevOps
# - Watch build pipeline trigger automatically
# - Review build results
# - Approve and merge

# 5. Verify deployment
# - Check test environment updates
# - Verify application is running
```

---

## 📊 Expected Results

### After Setup:

1. **Repository Structure:**
   ```
   main (protected)
   └── develop (protected, default)
       └── feature/test-pipeline
   ```

2. **Pipeline Runs:**
   - Build stage: ✅ Success
   - Test stage: ✅ Success
   - Deploy to Test: ✅ Success

3. **Deployments:**
   - Test environment: Running
   - Production environment: Ready

---

## 🎓 Marking Criteria Met

### Azure Repos (10 Marks):
✅ Git repository set up in Azure Repos  
✅ Gitflow branching strategy implemented  
✅ Branches created (main, develop)  
✅ Branch policies configured  
✅ Pull requests enabled  
✅ Code review process established  

### Azure Pipelines (10 Marks):
✅ Build pipeline created  
✅ Automated triggers configured  
✅ Code compilation task included  
✅ Test execution task included  
✅ Deployment to test environment configured  
✅ Artifact publishing configured  

**Total: 20/20 Marks** 🎉

---

## 🆘 Troubleshooting

### Git Push Fails:
```powershell
# Check remote URL
git remote -v

# Update remote URL
git remote set-url origin <correct-url>

# Try push again
git push origin main
```

### Build Pipeline Fails:
1. Check build logs in Azure Pipelines
2. Verify .NET SDK version (8.0)
3. Ensure all files are committed
4. Run `validate-build.ps1` locally

### Deployment Fails:
1. Verify service connection is valid
2. Check Azure Web App exists
3. Verify resource group permissions
4. Check deployment logs

---

## 📞 Support Resources

- **Azure DevOps Docs:** https://docs.microsoft.com/azure/devops/
- **Git Documentation:** https://git-scm.com/doc
- **Azure Pipelines:** https://docs.microsoft.com/azure/devops/pipelines/
- **Gitflow Guide:** https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow

---

## ✅ Final Checklist

Before submission, verify:
- [ ] Code pushed to Azure Repos
- [ ] Both `main` and `develop` branches exist
- [ ] Branch policies configured
- [ ] Pull request created and reviewed
- [ ] Build pipeline running successfully
- [ ] Tests executing in pipeline
- [ ] Deployment to test environment working
- [ ] Screenshots taken for evidence
- [ ] Documentation complete

---

**Good luck with your assignment!** 🚀
