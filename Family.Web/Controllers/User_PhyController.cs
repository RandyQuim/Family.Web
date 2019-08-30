using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Family.Web.Models;

namespace Family.Web.Controllers
{
    public class User_PhyController : Controller
    {
        private Entities db = new Entities();

        // GET: User_Phy
        public ActionResult Index()
        {
            var user_Phy = db.User_Phy.Include(u => u.Physiology).Include(u => u.User);
            return View(user_Phy.ToList());
        }

        // GET: User_Phy/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Phy user_Phy = db.User_Phy.Find(id);
            if (user_Phy == null)
            {
                return HttpNotFound();
            }
            return View(user_Phy);
        }

        // GET: User_Phy/Create
        public ActionResult Create()
        {
            ViewBag.PhyId = new SelectList(db.Physiologies, "PhyId", "PhyId");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name");
            return View();
        }

        // POST: User_Phy/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,PhyId,UserPhyId")] User_Phy user_Phy)
        {
            if (ModelState.IsValid)
            {
                db.User_Phy.Add(user_Phy);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PhyId = new SelectList(db.Physiologies, "PhyId", "PhyId", user_Phy.PhyId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name", user_Phy.UserId);
            return View(user_Phy);
        }

        // GET: User_Phy/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Phy user_Phy = db.User_Phy.Find(id);
            if (user_Phy == null)
            {
                return HttpNotFound();
            }
            ViewBag.PhyId = new SelectList(db.Physiologies, "PhyId", "PhyId", user_Phy.PhyId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name", user_Phy.UserId);
            return View(user_Phy);
        }

        // POST: User_Phy/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,PhyId,UserPhyId")] User_Phy user_Phy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user_Phy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PhyId = new SelectList(db.Physiologies, "PhyId", "PhyId", user_Phy.PhyId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Name", user_Phy.UserId);
            return View(user_Phy);
        }

        // GET: User_Phy/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Phy user_Phy = db.User_Phy.Find(id);
            if (user_Phy == null)
            {
                return HttpNotFound();
            }
            return View(user_Phy);
        }

        // POST: User_Phy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User_Phy user_Phy = db.User_Phy.Find(id);
            db.User_Phy.Remove(user_Phy);
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
    }
}
