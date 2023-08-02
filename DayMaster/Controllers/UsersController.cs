using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DayMaster.Models;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;

namespace DayMaster.Controllers
{
    public class UsersController : Controller
    {
        private readonly DayMasterContext _context;

        public UsersController(DayMasterContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
              return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string UserName, String Password)
        {
            if (ModelState.IsValid)
            {
                var UserExist = await _context.User
                .FirstOrDefaultAsync(m => m.userName == UserName && m.password == Password);
                if (UserExist == null)
                {
                    ViewBag.Message = "User not found with this userid and/or password";
                    return View();
                }
                else
                {
                    HttpContext.Session.SetString("username",UserName);
                    Notification notf = new Notification();
                    notf.userID = UserName;
                    notf.notificationDate = DateTime.Now;
                    notf.IsRead = true;
                    _context.Add(notf);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Tasks");
                }
            }
            else
                return View();
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,userName,password,firstName,lastName,phone,SecurityQuestion,SecurityAnswer")] User user)
        {
            if (ModelState.IsValid)
            {
                if (UserExists(user.userName))
                {
                    ViewBag.Message = "This User name already exit. Try another";
                    return View();
                }
                else {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    ViewBag.Message = "User created";
                    return View();
                }
            }
            return View(user);
        }

        private bool UserExists(string UName)
        {
          return (_context.User?.Any(e => e.userName == UName)).GetValueOrDefault();
        }

        //get
        public IActionResult forgotPwd()
        {
            return View(new ChangePasswordModel { AskForUsername = true });
        }

        public async Task<IActionResult> IsUsernameValidAsync(string username)
        {
            var UserExist =await _context.User
                .FirstOrDefaultAsync(m => m.userName == username);

            if(UserExist !=null)
            {
                return View("forgotPwd",new ChangePasswordModel { AskForUsername = false, IsUsernameValid = true,SecurityQuestion=UserExist.SecurityQuestion,Username=username });
            }
            else
            {
                ViewBag.Message = "Invalid user name, please try again...!";
                return View("forgotPwd", new ChangePasswordModel { AskForUsername = true });
            }
        }

        public async Task<IActionResult> IsSecurityAnswerValid(string username, string securityAnswer)
        {
            var valid = await _context.User
                .FirstOrDefaultAsync(m => m.userName == username && m.SecurityAnswer == securityAnswer);

            if(valid != null)
            {
                return View("forgotPwd", new ChangePasswordModel { AskForUsername = false, IsUsernameValid = false, IsSecurityAnswerValid=true,Username=username });
            }
            else
            {
                ViewBag.Message = "Invalid security Question answer, please try again...!";
                return View("forgotPwd", new ChangePasswordModel { AskForUsername = true });
            }
        }

        public async Task<IActionResult> UpdatePassword(string username, string newPassword)
        {
            var user = await _context.User.FirstOrDefaultAsync(m => m.userName == username);
            user.password=newPassword;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return View("forgotPwd", new ChangePasswordModel { AskForUsername = false, IsUsernameValid = false, PasswordChanged=true});

        }
    }
}
