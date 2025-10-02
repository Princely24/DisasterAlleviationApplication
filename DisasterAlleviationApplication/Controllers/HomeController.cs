using System.Diagnostics;
using DisasterAlleviationApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DisasterAlleviationApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();
            
            // Get statistics for dashboard
            viewModel.TotalIncidents = await _context.DisasterIncidents.CountAsync();
            viewModel.TotalDonations = await _context.Donations.CountAsync();
            viewModel.TotalVolunteers = await _context.Volunteers.CountAsync();
            viewModel.ActiveTasks = await _context.VolunteerTasks.Where(t => t.Status == Models.TaskStatus.Open || t.Status == Models.TaskStatus.Assigned).CountAsync();
            
            // Get recent incidents
            viewModel.RecentIncidents = await _context.DisasterIncidents
                .Include(d => d.User)
                .OrderByDescending(d => d.ReportedDate)
                .Take(5)
                .ToListAsync();
            
            // Get recent donations
            viewModel.RecentDonations = await _context.Donations
                .Include(d => d.User)
                .Include(d => d.Category)
                .OrderByDescending(d => d.DonationDate)
                .Take(5)
                .ToListAsync();

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            
            var viewModel = new UserDashboardViewModel
            {
                User = user,
                MyIncidents = await _context.DisasterIncidents
                    .Where(d => d.UserId == userId)
                    .OrderByDescending(d => d.ReportedDate)
                    .Take(5)
                    .ToListAsync(),
                MyDonations = await _context.Donations
                    .Include(d => d.Category)
                    .Where(d => d.UserId == userId)
                    .OrderByDescending(d => d.DonationDate)
                    .Take(5)
                    .ToListAsync()
            };

            // Get volunteer profile if exists
            var volunteer = await _context.Volunteers
                .Include(v => v.Assignments)
                .ThenInclude(a => a.Task)
                .FirstOrDefaultAsync(v => v.UserId == userId);

            if (volunteer != null)
            {
                viewModel.VolunteerProfile = volunteer;
                viewModel.MyTasks = await _context.VolunteerAssignments
                    .Include(a => a.Task)
                    .Where(a => a.VolunteerId == volunteer.Id)
                    .OrderByDescending(a => a.AssignedDate)
                    .Take(5)
                    .ToListAsync();
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class DashboardViewModel
    {
        public int TotalIncidents { get; set; }
        public int TotalDonations { get; set; }
        public int TotalVolunteers { get; set; }
        public int ActiveTasks { get; set; }
        public List<DisasterIncident> RecentIncidents { get; set; } = new List<DisasterIncident>();
        public List<Donation> RecentDonations { get; set; } = new List<Donation>();
    }

    public class UserDashboardViewModel
    {
        public ApplicationUser? User { get; set; }
        public Volunteer? VolunteerProfile { get; set; }
        public List<DisasterIncident> MyIncidents { get; set; } = new List<DisasterIncident>();
        public List<Donation> MyDonations { get; set; } = new List<Donation>();
        public List<VolunteerAssignment> MyTasks { get; set; } = new List<VolunteerAssignment>();
    }
}
