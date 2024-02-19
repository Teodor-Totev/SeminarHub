namespace SeminarHub.Controllers
{
    using System.Globalization;
    using System.Security.Claims;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using SeminarHub.Data;
    using SeminarHub.Data.Models;
    using SeminarHub.Models.Seminar;

    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext context;

        public SeminarController(SeminarHubDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            SeminarFM model = new SeminarFM()
            {
                Categories = await GetCategories()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarFM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Seminar s = new Seminar()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                DateAndTime = model.DateAndTime,
                Duration = model.Duration,
                CategoryId = model.CategoryId,
                OrganizerId = GetUserId(),
            };

            await context.Seminars.AddAsync(s);
            await context.SaveChangesAsync();

            return RedirectToAction("All");
        }

        public async Task<IActionResult> All()
        {
            var seminars = await context
                .Seminars
                .Select(s => new SeminarVM
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    DateAndTime = s.DateAndTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Lecturer = s.Lecturer,
                    Category = s.Category.Name,
                    Organizer = s.Organizer.UserName
                })
                .ToArrayAsync();

            return View(seminars);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var userId = GetUserId();
            var seminarId = id;

            var sp = new SeminarParticipant()
            {
                ParticipantId = userId,
                SeminarId = seminarId
            };

            if (await context.SeminarsParticipants.ContainsAsync(sp))
            {
                return RedirectToAction("All");
            }

            await context.SeminarsParticipants.AddAsync(sp);
            await context.SaveChangesAsync();

            return RedirectToAction("Joined");
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            IEnumerable<SeminarVM> model = await context.SeminarsParticipants
                .Where(sp => sp.ParticipantId == GetUserId())
                .Select(s => new SeminarVM()
                {
                    Id = s.Seminar.Id,
                    Topic = s.Seminar.Topic,
                    Lecturer = s.Seminar.Lecturer,
                    Category = s.Seminar.Category.Name,
                    DateAndTime = s.Seminar.DateAndTime.ToString("dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture),
                    Organizer = s.Seminar.Organizer.UserName
                })
                .ToArrayAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            string userId = GetUserId();

            var sp = await context.SeminarsParticipants
                .FirstOrDefaultAsync(x => x.ParticipantId == userId && x.SeminarId == id);

            if (sp == null)
            {
                return BadRequest();
            }

            context.SeminarsParticipants.Remove(sp);
            await context.SaveChangesAsync();

            return RedirectToAction("Joined");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var seminar = await context.Seminars
                .FindAsync(id);

            if (seminar == null)
            {
                return BadRequest();
            }

            var model = new SeminarFM()
            {
                Topic = seminar.Topic,
                Lecturer = seminar.Lecturer,
                Details = seminar.Details,
                DateAndTime = seminar.DateAndTime,
                Duration = seminar.Duration,
                CategoryId = seminar.CategoryId,
                Categories = await GetCategories()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SeminarFM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var seminar = await context.Seminars
               .FindAsync(id);

            if (seminar == null)
            {
                return BadRequest();
            }

            seminar.Lecturer = model.Lecturer;
            seminar.Topic = model.Topic;
            seminar.Details = model.Details;
            seminar.DateAndTime = model.DateAndTime;
            seminar.Duration = model.Duration;
            seminar.CategoryId = model.CategoryId;

            await context.SaveChangesAsync();
            return RedirectToAction("All");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var selectedSeminar = await context.Seminars
                .Include(s => s.Category)
                .Select(s => new SeminarDetailVM
                {
                    Id = s.Id,
                    Details = s.Details,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    DateAndTime = s.DateAndTime.ToString("dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture),
                    Duration = s.Duration,
                    Category = s.Category.Name,
                    Organizer = s.Organizer.UserName
                })
                .FirstAsync(e => e.Id == id);

            return View(selectedSeminar);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var targetSeminar = await context.Seminars
                .FindAsync(id);

            if (targetSeminar == null)
            {
                return BadRequest();
            }

            var model = new SeminarDeleteVM()
            {
                Id = targetSeminar.Id,
                Topic = targetSeminar.Topic,
                DateAndTime = targetSeminar.DateAndTime.ToString("dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seminarToDelete = await context.Seminars
                .FindAsync(id);

            if (seminarToDelete == null) return BadRequest();

            context.Seminars.Remove(seminarToDelete);
            await context.SaveChangesAsync();

            return RedirectToAction("All");
        }

        private async Task<IEnumerable<CategoryVM>> GetCategories()
        {
            return await context.Categories
                .Select(c => new CategoryVM { Id = c.Id, Name = c.Name })
                .ToArrayAsync();
        }

        private string GetUserId()
            => User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

    }
}
