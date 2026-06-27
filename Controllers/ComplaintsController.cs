using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_cvrde.Models;

namespace project_cvrde.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComplaintsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===========================
        // INDEX + SEARCH + FILTER
        // ===========================
        public IActionResult Index(string search, string status)
        {
            var complaints = _context.Complaints.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                complaints = complaints.Where(c =>
                    c.EmployeeID.ToString().Contains(search) ||
                    c.DepartmentID.ToString().Contains(search) ||
                    (c.ComplaintDescription != null &&
                     c.ComplaintDescription.Contains(search)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                complaints = complaints.Where(c => c.Status == status);
            }

            ViewBag.Search = search;
            ViewBag.Status = status;

            return View(complaints.OrderByDescending(c => c.ComplaintID).ToList());
        }

        // ===========================
        // CREATE
        // ===========================

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Complaint complaint)
        {
            if (ModelState.IsValid)
            {
                _context.Complaints.Add(complaint);
                _context.SaveChanges();

                TempData["Success"] = "Complaint Added Successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(complaint);
        }

        // ===========================
        // DETAILS
        // ===========================

        [HttpGet]
        public IActionResult Details(int id)
        {
            var complaint = _context.Complaints.FirstOrDefault(c => c.ComplaintID == id);

            if (complaint == null)
                return NotFound();

            return View(complaint);
        }

        // ===========================
        // EDIT
        // ===========================

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var complaint = _context.Complaints.Find(id);

            if (complaint == null)
                return NotFound();

            return View(complaint);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Complaint complaint)
        {
            if (id != complaint.ComplaintID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(complaint);
                    _context.SaveChanges();

                    TempData["Success"] = "Complaint Updated Successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Complaints.Any(c => c.ComplaintID == complaint.ComplaintID))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(complaint);
        }

        // ===========================
        // DELETE
        // ===========================

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var complaint = _context.Complaints.Find(id);

            if (complaint == null)
                return NotFound();

            return View(complaint);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Complaint complaint)
        {
            var existingComplaint = _context.Complaints.Find(complaint.ComplaintID);

            if (existingComplaint != null)
            {
                _context.Complaints.Remove(existingComplaint);
                _context.SaveChanges();

                TempData["Success"] = "Complaint Deleted Successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // DASHBOARD
        // ===========================

        public IActionResult Dashboard()
        {
            ViewBag.Total = _context.Complaints.Count();

            ViewBag.Open = _context.Complaints
                .Count(c => c.Status == "Open");

            ViewBag.InProgress = _context.Complaints
                .Count(c => c.Status == "In Progress");

            ViewBag.Resolved = _context.Complaints
                .Count(c => c.Status == "Resolved");

            ViewBag.High = _context.Complaints
                .Count(c => c.Priority == "High");

            ViewBag.Medium = _context.Complaints
                .Count(c => c.Priority == "Medium");

            ViewBag.Low = _context.Complaints
                .Count(c => c.Priority == "Low");

            return View();
        }
    }
}