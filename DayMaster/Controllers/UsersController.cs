using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DayMaster.Models;

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
    }
}
