using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DayMaster.Models;
using Task = DayMaster.Models.Task;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Build.Framework;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace DayMaster.Controllers
{
    public class TasksController : Controller
    {
        private readonly DayMasterContext _context;

        public TasksController(DayMasterContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(string search)
        {
            string? username = HttpContext.Session.GetString("username");

            var task1 = await _context.Tasks.Where(a => a.username == username).ToListAsync();


            if (search != null)
            {
                var task2 = await _context.Tasks.Where(a => a.taskName.ToLower().Contains(search.ToLower()) && a.username == username).ToListAsync();
               
                var combinedTasks = (Tasks1: task1, Tasks2: task2);


                return View((combinedTasks.Tasks1.AsEnumerable(), combinedTasks.Tasks2.AsEnumerable()));
            }
            else
            {
                var task2 = task1;
                var combinedTasks = (Tasks1: task1, Tasks2: task2);

                return View((combinedTasks.Tasks1.AsEnumerable(), combinedTasks.Tasks2.AsEnumerable()));
            }
        }
                        

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.taskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("taskId,taskName,taskDescription,taskStatus,priority,date")] Task task)
        {
            if (ModelState.IsValid)
            {
                task.username = HttpContext.Session.GetString("username");
               
                _context.Add(task);
                await _context.SaveChangesAsync();
                TaskHistory tHistory = new TaskHistory();
                tHistory.taskId = task.taskId;
                tHistory.useId = task.username;
                tHistory.ActionTime = DateTime.Now;
                tHistory.ActionName = "Add Task";

                _context.Add(tHistory);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("taskId,taskName,taskDescription,taskStatus,priority,date")] Task task)
        {
            if (id != task.taskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    task.username = HttpContext.Session.GetString("username");
                    _context.Update(task);
                    await _context.SaveChangesAsync();

                    TaskHistory tHistory = new TaskHistory();
                    tHistory.taskId = task.taskId;
                    tHistory.useId = task.username;
                    tHistory.ActionTime = DateTime.Now;
                    tHistory.ActionName = "Edit";
                    _context.Add(tHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.taskId))
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

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.taskId == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'DayMasterContext.Task'  is null.");
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                TaskHistory tHistory = new TaskHistory();
                tHistory.taskId = task.taskId;
                tHistory.useId = task.username;
                tHistory.ActionTime = DateTime.Now;
                tHistory.ActionName = "Delete";
                _context.Add(tHistory);
                await _context.SaveChangesAsync();
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
          return (_context.Tasks?.Any(e => e.taskId == id)).GetValueOrDefault();
        }

        public ActionResult GenerateReport(string startDate, string endDate)
        {
            try
            {
                string? username = HttpContext.Session.GetString("username");
                var data = _context.Tasks.Where(m =>m.username==username && m.date <= DateTime.Parse(endDate) && m.date >= DateTime.Parse(startDate)).
                    GroupBy(m => m.taskStatus)
                    .Select(g => new
                    {
                        TaskStatus = g.Key,    // The task status value
                        Count = g.Count()  // The count of tasks with the same status
                    }).ToList();

                if (data != null & data.Count > 0)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        IXLWorksheet ws =wb.Worksheets.Add(ToConvertDataTable(data.ToList()),"Report");

                        var dataIncomplete = _context.Tasks.Where(m => m.username == username && m.date <= DateTime.Parse(endDate) &&
                                                m.date >= DateTime.Parse(startDate)
                                                && m.taskStatus== "Incomplete").ToList();

                        DataTable dataTableIncomplete = ToConvertDataTable(dataIncomplete);

                        int lastRowIndex = ws.LastRowUsed().RowNumber();

                        ws.Cell(lastRowIndex + 2, 1).InsertData(dataTableIncomplete.AsEnumerable());


                        using (MemoryStream ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            string fileName = $"Report.xlsx";
                            return File(ms.ToArray(), "application/vnd.openxmlformats-officeddocuments.spreadsheetml.sheet", fileName);
                        }
                    }
                }
                else
                {
                    TempData["Error"] = "Data not found!";
                }
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("index");

        }
        public DataTable ToConvertDataTable<T>(List<T> items)
        {

            DataTable dt = new DataTable();
            PropertyInfo[] propInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

          
                var value = new object[propInfo.Length];
                for (int i = 0; i < propInfo.Length; i++)
                {
                    value[i] = propInfo[i].Name.ToUpper();
                }
                dt.Rows.Add(value);

            DataRow emptyRow = dt.NewRow();
            dt.Rows.Add(emptyRow); 


            foreach (T item in items)
            {
                var values = new object[propInfo.Length];
                for (int i = 0; i < propInfo.Length; i++)
                {
                    values[i] = propInfo[i].GetValue(item, null);
                }
                dt.Rows.Add(values);
            }
            return dt;
        }
        public IActionResult Logout()
        {
           
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index", "Users");
        }

    }
}
