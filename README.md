# Disaster Relief Application

A comprehensive web application for managing disaster relief operations, including incident reporting, donation tracking, volunteer management, and task coordination.

## 🚀 Features

### Core Functionality
- **Incident Reporting** - Report and track disaster incidents with image/PDF attachments
- **Donation Management** - Track physical, financial, and service donations
- **Volunteer Registration** - Register volunteers and manage their profiles
- **Task Management** - Create and assign tasks to volunteers
- **Admin Panel** - Comprehensive admin dashboard for approvals and management

### Key Features
- ✅ User authentication with ASP.NET Core Identity
- ✅ Role-based authorization (Admin/User roles)
- ✅ File upload support (images and PDFs)
- ✅ Real-time status tracking
- ✅ Responsive design with Bootstrap 5
- ✅ Professional admin interface with sidebar navigation
- ✅ South African Rand (R) currency support

## 🛠️ Technology Stack

- **Framework:** ASP.NET Core 8.0 (MVC)
- **Database:** SQL Server / Azure SQL Database
- **ORM:** Entity Framework Core 8.0
- **Authentication:** ASP.NET Core Identity
- **Frontend:** Bootstrap 5, Font Awesome 6
- **CI/CD:** Azure DevOps Pipelines

## 📋 Prerequisites

- .NET 8.0 SDK
- SQL Server 2019+ or SQL Server LocalDB
- Visual Studio 2022 or VS Code
- Azure DevOps account (for CI/CD)
- Azure subscription (for deployment)

## 🔧 Installation & Setup

### 1. Clone the Repository

```powershell
git clone https://dev.azure.com/YOUR-ORG/DisasterReliefApplication/_git/DisasterReliefApp
cd DisasterReliefApp
```

### 2. Configure Database Connection

Update `appsettings.json`:

```json
{
    "ConnectionStrings": {
       "DefaultConnection": "Server=tcp:giftofgivers-disasterrelief-prod.database.windows.net,1433;Initial Catalog=GiftOfGivers_DisasterRelief_DB_Dev1;Persist Security Info=False;User ID=ST10263265;Password=Poggyboled@100P;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
     }
}
```

### 3. Apply Database Migrations

```powershell
cd DisasterAlleviationApplication
dotnet ef database update
```

### 4. Run the Application

```powershell
dotnet run
```

Navigate to: `https://localhost:5001`

## 👤 Default Admin Credentials

```
Email:    admin@disasterrelief.com
Password: Admin@123
```

**⚠️ Change the password after first login!**

## 📁 Project Structure

```
DisasterAlleviationApplication/
├── Controllers/
│   ├── AdminController.cs          # Admin panel management
│   ├── DonationController.cs       # Donation operations
│   ├── HomeController.cs           # Home and dashboard
│   ├── IncidentController.cs       # Incident reporting
│   ├── TaskController.cs           # Task management
│   └── VolunteerController.cs      # Volunteer management
├── Models/
│   ├── ApplicationDbContext.cs     # EF Core DbContext
│   ├── ApplicationUser.cs          # Custom Identity user
│   ├── DisasterIncident.cs         # Incident model
│   ├── Donation.cs                 # Donation model
│   ├── DonationCategory.cs         # Donation categories
│   ├── Volunteer.cs                # Volunteer model
│   ├── VolunteerAssignment.cs      # Task assignments
│   └── VolunteerTask.cs            # Task model
├── Views/
│   ├── Admin/                      # Admin panel views
│   ├── Donation/                   # Donation views
│   ├── Home/                       # Home and dashboard
│   ├── Incident/                   # Incident views
│   ├── Task/                       # Task views
│   ├── Volunteer/                  # Volunteer views
│   └── Shared/
│       ├── _Layout.cshtml          # Main layout
│       └── _AdminLayout.cshtml     # Admin layout
├── Migrations/                     # EF Core migrations
├── Data/
│   └── DbInitializer.cs            # Database seeding
├── wwwroot/
│   └── uploads/                    # File uploads
├── appsettings.json                # Configuration
├── Program.cs                      # Application startup
└── azure-pipelines.yml             # CI/CD configuration
```

## 🔐 User Roles

### Admin Role
- Approve/reject volunteer applications
- Manage incidents (update status)
- Manage donations (update status)
- View system statistics
- Access admin dashboard

### User Role (Default)
- Report incidents
- Make donations
- Register as volunteer
- Apply for tasks
- View public information

## 🌊 Gitflow Branching Strategy

### Branches:
- **`main`** - Production code
- **`develop`** - Development integration
- **`feature/*`** - New features
- **`release/*`** - Release preparation
- **`hotfix/*`** - Production fixes
- **`bugfix/*`** - Development fixes

### Workflow:
1. Create feature branch from `develop`
2. Develop and test locally
3. Push and create Pull Request
4. Code review and approval
5. Merge to `develop`
6. Deploy to test environment
7. Create release branch
8. Merge to `main` for production

See `BRANCHING_STRATEGY.md` for detailed guidelines.

## 🚀 CI/CD Pipeline

### Build Pipeline
- **Trigger:** Push to `main`, `develop`, or `release/*`
- **Steps:**
  1. Restore dependencies
  2. Build solution
  3. Run tests
  4. Publish artifacts

### Deployment Pipeline
- **Test Environment:** Auto-deploy from `develop`
- **Production Environment:** Auto-deploy from `main` (with approval)

See `AZURE_DEVOPS_SETUP.md` for detailed setup instructions.

## 📊 Database Schema

### Main Tables:
- **AspNetUsers** - User accounts (Identity)
- **AspNetRoles** - User roles (Identity)
- **DisasterIncidents** - Disaster incident reports
- **Donations** - Donation records
- **DonationCategories** - Donation categories
- **Volunteers** - Volunteer profiles
- **VolunteerTasks** - Tasks for volunteers
- **VolunteerAssignments** - Task assignments

## 🧪 Testing

### Run Tests:
```powershell
dotnet test
```

### Test Coverage:
```powershell
dotnet test --collect:"XPlat Code Coverage"
```

## 📦 Deployment

### Deploy to Azure:

```powershell
# Publish application
dotnet publish -c Release -o ./publish

# Deploy to Azure Web App
az webapp deployment source config-zip --resource-group DisasterRelief-RG --name DisasterRelief-Prod --src ./publish.zip
```

## 🔧 Configuration

### Environment Variables:
- `ASPNETCORE_ENVIRONMENT` - Development/Staging/Production
- `ConnectionStrings__DefaultConnection` - Database connection

### App Settings:
- Connection strings
- Logging levels
- Authentication settings

## 📝 API Endpoints

### Public Routes:
- `GET /` - Home page
- `GET /Home/Dashboard` - User dashboard
- `GET /Admin/Login` - Admin login
- `GET /Admin/Register` - Admin registration

### Authenticated Routes:
- `GET /Incident/Create` - Report incident
- `POST /Incident/Create` - Submit incident
- `GET /Donation/Create` - Make donation
- `POST /Donation/Create` - Submit donation
- `GET /Volunteer/Create` - Register as volunteer
- `POST /Volunteer/Create` - Submit registration

### Admin Routes (Admin Role Required):
- `GET /Admin/Dashboard` - Admin dashboard
- `GET /Admin/Volunteers` - Manage volunteers
- `POST /Admin/ApproveVolunteer` - Approve volunteer
- `GET /Admin/Incidents` - Manage incidents
- `GET /Admin/Donations` - Manage donations

## 🐛 Troubleshooting

### Database Connection Issues:
```powershell
# Check SQL Server is running
# Update connection string in appsettings.json
# Run migrations
dotnet ef database update
```

### Migration Issues:
```powershell
# Drop database and recreate
dotnet ef database drop -f
dotnet ef database update
```

### Build Issues:
```powershell
# Clean and rebuild
dotnet clean
dotnet build
```

## 📚 Documentation

- **Branching Strategy:** See `BRANCHING_STRATEGY.md`
- **Azure DevOps Setup:** See `AZURE_DEVOPS_SETUP.md`
- **Admin Credentials:** See `ADMIN_CREDENTIALS.txt`

## 🤝 Contributing

1. Create feature branch from `develop`
2. Make changes and commit
3. Push to remote
4. Create Pull Request
5. Wait for code review
6. Address feedback
7. Merge after approval

## 📄 License

This project is for educational purposes.

## 👥 Team

- Development Team
- Project Manager
- QA Team

## 📞 Support

For issues or questions:
- Create work item in Azure Boards
- Contact team lead
- Review documentation

---

## Quick Start Commands

```powershell
# Clone repository
git clone <repository-url>

# Navigate to project
cd DisasterAlleviationApplication/DisasterAlleviationApplication

# Restore packages
dotnet restore

# Apply migrations
dotnet ef database update

# Run application
dotnet run

# Access application
# https://localhost:5001

# Login as admin
# Email: admin@disasterrelief.com
# Password: Admin@123
```

---

**Version:** 1.0.0  
**Last Updated:** October 2025
