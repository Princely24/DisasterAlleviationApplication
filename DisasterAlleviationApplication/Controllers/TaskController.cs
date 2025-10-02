using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DisasterAlleviationApplication.Models;

namespace DisasterAlleviationApplication.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Task
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.VolunteerTasks
                .Include(t => t.AssignedVolunteer)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
            return View(tasks);
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.VolunteerTasks
                .Include(t => t.AssignedVolunteer)
                .Include(t => t.Assignments)
                .ThenInclude(a => a.Volunteer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Category,Priority,Location,StartDate,EndDate,RequiredVolunteers,RequiredSkills,Equipment,EstimatedHours,Notes")] VolunteerTask task)
        {
            task.CreatedDate = DateTime.UtcNow;
            task.Status = Models.TaskStatus.Open;
            task.CurrentVolunteers = 0;
            
            // Remove validation errors for properties not in the form
            ModelState.Remove(nameof(VolunteerTask.CreatedDate));
            ModelState.Remove(nameof(VolunteerTask.Status));
            ModelState.Remove(nameof(VolunteerTask.CurrentVolunteers));
            ModelState.Remove(nameof(VolunteerTask.AssignedVolunteer));
            ModelState.Remove(nameof(VolunteerTask.Assignments));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(task);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Volunteer task created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error saving to database: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        ModelState.AddModelError("", $"Inner exception: {ex.InnerException.Message}");
                    }
                }
            }
            else
            {
                // Log validation errors to help debug
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }
            }
            return View(task);
        }

        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.VolunteerTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Category,Priority,Location,StartDate,EndDate,RequiredVolunteers,RequiredSkills,Equipment,EstimatedHours,Notes,Status")] VolunteerTask task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Task updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VolunteerTaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: Task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.VolunteerTasks
                .Include(t => t.AssignedVolunteer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.VolunteerTasks.FindAsync(id);
            if (task != null)
            {
                _context.VolunteerTasks.Remove(task);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Task deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Task/Apply/5
        public async Task<IActionResult> Apply(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.VolunteerTasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.UserId == userId);

            if (volunteer == null)
            {
                TempData["ErrorMessage"] = "You must register as a volunteer before applying for tasks.";
                return RedirectToAction(nameof(Create), "Volunteer");
            }

            if (volunteer.Status != VolunteerStatus.Approved && volunteer.Status != VolunteerStatus.Active)
            {
                TempData["ErrorMessage"] = "Your volunteer application must be approved before you can apply for tasks.";
                return RedirectToAction("MyProfile", "Volunteer");
            }

            // Check if already assigned to this task
            var existingAssignment = await _context.VolunteerAssignments
                .FirstOrDefaultAsync(a => a.VolunteerId == volunteer.Id && a.TaskId == task.Id);

            if (existingAssignment != null)
            {
                TempData["ErrorMessage"] = "You have already applied for this task.";
                return RedirectToAction(nameof(Details), new { id = task.Id });
            }

            // Check if task is still open
            if (task.Status != Models.TaskStatus.Open)
            {
                TempData["ErrorMessage"] = "This task is no longer accepting applications.";
                return RedirectToAction(nameof(Details), new { id = task.Id });
            }

            // Check if task has reached maximum volunteers
            if (task.CurrentVolunteers >= task.RequiredVolunteers)
            {
                TempData["ErrorMessage"] = "This task has reached its maximum number of volunteers.";
                return RedirectToAction(nameof(Details), new { id = task.Id });
            }

            var assignment = new VolunteerAssignment
            {
                VolunteerId = volunteer.Id,
                TaskId = task.Id,
                AssignedDate = DateTime.UtcNow,
                Status = AssignmentStatus.Assigned
            };

            _context.VolunteerAssignments.Add(assignment);
            task.CurrentVolunteers++;
            
            if (task.CurrentVolunteers >= task.RequiredVolunteers)
            {
                task.Status = Models.TaskStatus.Assigned;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "You have successfully applied for this task!";
            return RedirectToAction(nameof(Details), new { id = task.Id });
        }

        // GET: Task/MyTasks
        public async Task<IActionResult> MyTasks()
        {
            var userId = _userManager.GetUserId(User);
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.UserId == userId);

            if (volunteer == null)
            {
                TempData["ErrorMessage"] = "You must register as a volunteer to view your tasks.";
                return RedirectToAction(nameof(Create), "Volunteer");
            }

            var assignments = await _context.VolunteerAssignments
                .Include(a => a.Task)
                .Where(a => a.VolunteerId == volunteer.Id)
                .OrderByDescending(a => a.AssignedDate)
                .ToListAsync();

            return View(assignments);
        }

        private bool VolunteerTaskExists(int id)
        {
            return _context.VolunteerTasks.Any(e => e.Id == id);
        }
    }
}
