using AutoMapper;
using AutoMapper.QueryableExtensions;
using CryptoPlayground.Data;
using CryptoPlayground.Models;
using CryptoPlayground.Models.TeamViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoPlayground.Controllers
{
    public class TeamController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        

        public TeamController(
            IMapper mapper,
            ILogger<TeamController> logger,
            ApplicationDbContext context)
        {
            _mapper = mapper;
            _logger = logger;
            _context = context;
        }

        // GET: Team
        public async Task<IActionResult> Index()
        {
            return View(await _context.Teams.OrderBy(t => t.Name).ProjectTo<TeamViewModel>().ToListAsync());
        }

        // GET: Team/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.Include("TeamMembers").SingleOrDefaultAsync(t => t.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            ViewBag.TeamMembers = new SelectList(team.TeamMembers.OrderBy(tm => tm.UserName), "Id", "UserName");
            return View(_mapper.Map<TeamViewModel>(team));
        }

        // GET: Team/Create
        public IActionResult Create()
        {
            ViewBag.AvailableUsers = new SelectList(_context.Users.Where(u => u.TeamId == null).OrderBy(u => u.UserName), "Id", "UserName");
            return View();
        }

        // POST: Team/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var team = _mapper.Map<Team>(model);
                _context.Add(team);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Team '{0}' was created.", team.Id);

                var users = await _context.Users.Where(u => model.TeamMembers.Contains(u.Id)).ToListAsync();
                foreach (var user in users)
                {
                    user.TeamId = team.Id;
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("Users were added to the team '{1}'.", team.Id);

                return RedirectToAction(nameof(Index));
            }

            ViewBag.AvailableUsers = new SelectList(_context.Users.Where(u => u.TeamId == null).OrderBy(u => u.UserName), "Id", "UserName");
            return View(model);
        }

        // GET: Team/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.Include("TeamMembers").SingleOrDefaultAsync(t => t.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            ViewBag.AvailableUsers = new SelectList(_context.Users.Where(u => u.TeamId == null || u.TeamId == id.Value).OrderBy(u => u.UserName), "Id", "UserName", team.TeamMembers);
            return View(_mapper.Map<TeamViewModel>(team));
        }

        // POST: Team/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var team = await _context.Teams.Include("TeamMembers").SingleOrDefaultAsync(t => t.Id == model.Id);
                if (team == null)
                {
                    return NotFound();
                }

                team.Name = model.Name;
                foreach (var member in team.TeamMembers)
                {
                    member.TeamId = null;
                }

                var users = await _context.Users.Where(u => model.TeamMembers.Contains(u.Id)).ToListAsync();
                foreach (var user in users)
                {
                    user.TeamId = team.Id;
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Team '{0}' was updated.", model.Id);

                return RedirectToAction(nameof(Index));
            }

            ViewBag.AvailableUsers = new SelectList(_context.Users.Where(u => u.TeamId == null || u.TeamId == model.Id).OrderBy(u => u.UserName), "Id", "UserName", model.TeamMembers);
            return View(model);
        }

        // GET: Team/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .SingleOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<TeamViewModel>(team));
        }

        // POST: Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.Include("TeamMembers").SingleOrDefaultAsync(t => t.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            foreach (var member in team.TeamMembers)
            {
                member.TeamId = null;
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Team '{0}' was deleted.", id);

            return RedirectToAction(nameof(Index));
        }
    }
}
