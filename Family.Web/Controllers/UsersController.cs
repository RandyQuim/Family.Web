using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Family.Web.Models;
using Family.Web.Services;
using Microsoft.Ajax.Utilities;

namespace Family.Web.Controllers
{
    public class UsersController : Controller
    {
        private Entities db = new Entities();

        // GET: Users
       public ActionResult Index()
        {
            UserServices service = new UserServices();
            List<UserDto> usersModel = service.GetUsers();
            return View("Index", usersModel);
        }

        //GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Name,Permission,Password,UserName")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Name,Permission,Password,UserName,BirthDate,Page")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Renders a partial view modal as a popup for user to login
        /// </summary>
        /// <returns>The partial viw for login</returns>
        public ActionResult LogIn()
        {
            return PartialView("_LogIn");
        }

        /// <summary>
        /// Posts user login fields for validation in services
        /// </summary>
        /// <param name="user">The user entered fields for login</param>
        /// <returns>The results of the login attempt</returns>
        [HttpPost]
        public ActionResult LogIn(UserDto user)
        {
            UserServices service = new UserServices();
            UserDto model = service.ValidateLogIn(user);
            if (model == null)
            {
                return Content("Invalid username or password");
            }
            
            return Json(model.Name); 
        }

        /// <summary>
        /// Logs the user out of the system
        /// </summary>
        /// <returns>The username of the logged out user</returns>
        public ActionResult Logout()
        {
            string userName = Principal.Identity.UserName;
            Principal.Identity = null;
            return Json(userName);  
        }

        /// <summary>
        /// Retrieves the page the user is based out of (central method used when tracking page properties and variables)
        /// </summary>
        /// <param name="page">The name/source of the page</param>
        /// <returns>The view of the page the user chose (family member name)</returns>
        public ActionResult GetUserPage(string page)
        {
            UserServices service = new UserServices();
            UserDto model = service.GetUser(service.GetUserIdOfCurrentPage(page));         
            if (model != null)
            {
                model.Age = service.GetTimespanSinceBirthdate(model.BirthDate);
                model.Time = new Time()
                {
                    Hours = Convert.ToInt32(Time.GetBetween(model.Age, "Day(s)", "Hour(s)")),
                    Minutes = Convert.ToInt32(Time.GetBetween(model.Age, "Hour(s)", "Minute(s)")),
                    Seconds = Convert.ToInt32(Time.GetBetween(model.Age, "Minute(s)", "Second(s)"))
                };
                return View(page, model);
            }

            model = new UserDto();
            return View(page, model);
        }
    }
}
