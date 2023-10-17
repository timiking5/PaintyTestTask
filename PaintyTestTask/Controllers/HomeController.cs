using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Models;
using PaintyTestTask.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace PaintyTestTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            var requsterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var subs = _db.Subscriptions.Where(x => x.FromId == requsterId);
            var model = _db.Users.Select(x => new Tuple<ApplicationUser, bool>(x, !subs.Where(t => t.ToId == x.Id).Any())).ToList();
            if (User.Identity.IsAuthenticated)
            {

                model.Remove(model.Where(x => x.Item1.Id == requsterId).First());
            }
            return View(model);
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
}