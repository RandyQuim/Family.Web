using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Family.Web.Models;
using Family.Web.Services;

namespace Family.Web.Controllers
{
    public class PhysiologiesController : Controller
    {
        /// <summary>
        /// The database context
        /// </summary>
        private Entities db = new Entities();

        /// <summary>
        /// The main page of physiologies for administrative purposes
        /// </summary>
        /// <returns>The physiologies admin page</returns>
        // GET: Physiologies
        public ActionResult Index()
        {
            PhysiologyServices service = new PhysiologyServices();
            List<PhysiologyDto> physiologies = service.GetAllUserPhysiologyHistory();
            return View(physiologies);
        }

        /// <summary>
        /// Lists the history of a user's physiology
        /// </summary>
        /// <param name="page">The page name/source</param>
        /// <returns>The partial view of physiology records</returns>
        public ActionResult ListPhysiologyHistory(string page)
        {
            PhysiologyServices phyService = new PhysiologyServices();
            UserServices userService = new UserServices();
            int userId = userService.GetUserIdOfCurrentPage(page);
            List<PhysiologyDto> phyModel = phyService.GetUserPhysiologyHistory(userId);
            ViewBag.UserId = userId;
            ViewBag.Page = page;
            return PartialView("_ListPhysiologyHistory", phyModel);
        }

        /// <summary>
        /// The details of a specific physiology record
        /// </summary>
        /// <param name="id">The identifier of a specific physiology record</param>
        /// <param name="page">The page name/source</param>
        /// <returns>The details view</returns>
        // GET: Physiologies/Details/5
        public ActionResult Details(int id, string page)
        {
            PhysiologyServices phyService = new PhysiologyServices();
            PhysiologyDto model = phyService.FindPhysiologyRecord(id);
            model.Page = page;
            return View(model);
        }

        /// <summary>
        /// Returns the view to create a physiology record
        /// </summary>
        /// <param name="userId">The identifier for the user to create a record for</param>
        /// <param name="page">The name/source of the page</param>
        /// <returns>The create view</returns>
        // GET: Physiologies/Create
        public ActionResult Create(int userId, string page)
        {
            return View(new PhysiologyDto(){ Date = DateTime.Today, UserId = userId, Page = page });
        }
                
        /// <summary>
        /// Creates a physiology record
        /// </summary>
        /// <param name="physiology">The model to create</param>
        /// <returns>The page source</returns>
        // POST: Physiologies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhysiologyDto physiology)
        {
            if (ModelState.IsValid)
            {
                PhysiologyServices service = new PhysiologyServices();
                service.SaveCreate(physiology);               
                return RedirectToAction("GetUserPage", "Users", new { page = physiology.Page });
            }

            return View(physiology);
        }

        /// <summary>
        /// Renders the edit view
        /// </summary>
        /// <param name="id">The id of the record to edit</param>
        /// <param name="page">The page name/source</param>
        /// <returns>The edit view</returns>
        // GET: Physiologies/Edit/5
        public ActionResult Edit(int id, string page)
        {
            PhysiologyServices service = new PhysiologyServices();
            PhysiologyDto physiology = service.FindPhysiologyRecord(id);
            if (physiology == null)
            {
                return HttpNotFound();
            }
            physiology.Page = page;
            return View(physiology);
        }

        /// <summary>
        /// Sends changes to services to modify
        /// </summary>
        /// <param name="physiology">The modified physiology record</param>
        /// <returns>The page source</returns>
        // POST: Physiologies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PhysiologyDto physiology)
        {
            if (ModelState.IsValid)
            {
                PhysiologyServices service = new PhysiologyServices();
                service.Edit(physiology);
                if (physiology.Page == null)
                {
                    return RedirectToAction("Index", "Physiologies");
                }
                return RedirectToAction("GetUserPage", "Users", new { page = physiology.Page });
            }
            return View(physiology);
        }

        /// <summary>
        /// Renders the delete view
        /// </summary>
        /// <param name="id">The id of the physiology record to display</param>
        /// <param name="page">The page name/source</param>
        /// <returns>The delete view</returns>
        // GET: Physiologies/Delete/5
        public ActionResult Delete(int id, string page)
        {
            PhysiologyServices service = new PhysiologyServices();
            PhysiologyDto phyModel = service.FindPhysiologyRecord(id);
           
            if (phyModel == null)
            {
                return HttpNotFound();
            }
            phyModel.Page = page;
            return View(phyModel);
        }

        /// <summary>
        /// Sends physiology record to services to delete
        /// </summary>
        /// <param name="physiology">The physiology record to delete</param>
        /// <returns>The page source</returns>
        // POST: Physiologies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(PhysiologyDto physiology)
        {
            PhysiologyServices service = new PhysiologyServices();
            if (!service.Delete(physiology.PhyId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (physiology.Page == null)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("GetUserPage", "Users", new { page = physiology.Page });
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
