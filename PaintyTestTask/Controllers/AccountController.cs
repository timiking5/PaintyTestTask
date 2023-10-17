using Azure.Identity;
using DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using System.Security.Claims;
using Utility;

namespace PaintyTestTask.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _webHostEnvironment = webHostEnvironment;
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index(string username)
        {
            // find requesting user's id
            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // find user with his followings
            var user = _db.Users.Where(x => x.UserName == username).FirstOrDefault();
            if (user is null)
            {
                return NotFound();
            }
            var followings = _db.Subscriptions.Where(x => x.FromId == user.Id);
            AccountVM vm = new()
            {
                User = user,
                FollowButton = true,
                Publications = new()  
            };
            if (followings.Where(x => x.ToId == requesterId).Any() || user.Id == requesterId)
            { // If user is following the request user, then we can populate the vm
                vm.Publications = _db.Publications.Where(x => x.UserId == user.Id).ToList();
                vm.AccessDenied = false;
            }
            var requester = _db.Users.Where(x => x.Id == requesterId).FirstOrDefault();
            if (_db.Subscriptions.Where(x => x.ToId == user.Id && x.FromId == requesterId).Any())
            {
                vm.FollowButton = false;
            }
            return View(vm);
        }
        public IActionResult Details(int publicationId)
        {
            var publ = _db.Publications.Where(x => x.Id == publicationId).FirstOrDefault();
            if (publ is null)
            {
                return NotFound();
            }
            // find requester's id
            var requsterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // find publication's owner
            
            var owner = _db.Users.Where(x => x.Id == publ.UserId).FirstOrDefault();
            if (owner.Id != requsterId && !_db.Subscriptions.Where(x => x.FromId == owner.Id && x.ToId == requsterId).Any())
            { // forbid access if owner is not subscribed to requester
                return Forbid();
            }
            return View(publ);
        }
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Publication publ = new();
            publ.UserId = userId;
            return View(publ);
        }
        [HttpPost]
        public IActionResult Create(Publication publ, IFormFile? file)
        {
            publ.PostedDate = DateTime.Now.Date;
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, SD.ImgStorageUrl);
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    publ.ImgUrl = "\\" + SD.ImgStorageUrl + "\\" + fileName;
                }
            }
            _db.Publications.Add(publ);
            _db.SaveChanges();
            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }
        public IActionResult ToggleFollow(string userId)
        {
            var requsterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followings = _db.Subscriptions.Where(x => x.ToId == userId && x.FromId == requsterId);
            if (followings.Any())
            {
                _db.Subscriptions.Remove(followings.First());
            }
            else
            {
                _db.Subscriptions.Add(new()
                {
                    FromId = requsterId,
                    ToId = userId
                });
            }
            _db.SaveChanges();
            var user = _db.Users.FirstOrDefault(x => x.Id == userId);
            return RedirectToAction("Index", new RouteValueDictionary(
                new { controller = "Account", action = "Index", username = user?.UserName }));
        }
    }
}
