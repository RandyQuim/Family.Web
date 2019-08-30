using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Family.Web.Models;
using Family.Web.Services;

namespace Family.Web.Controllers
{
    public class ImagesController : Controller
    {
        /// <summary>
        /// Renders the 'Add' partial view
        /// </summary>
        /// <param name="page">The page source/name that renders the 'Add' view</param>
        /// <returns>The partial view for adding an image</returns>
        // GET: Images
        public ActionResult Add(string page)
        {
            return PartialView("_Add", new Image() { Page = page } );
        }

        /// <summary>
        /// Creates the file name and image path for an image and sends to service
        /// </summary>
        /// <param name="imageModel">The model of the image to add</param>
        /// <returns>The user page of images</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Image imageModel)
        {
            string fileName = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);
            string extension = Path.GetExtension(imageModel.ImageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            imageModel.ImagePath = "~/images/" + fileName;
            fileName = Path.Combine(Server.MapPath("~/images/"), fileName);
            ImageServices service = new ImageServices();
            service.SaveImage(imageModel, fileName);
            ModelState.Clear();
            return RedirectToAction("GetUserPage", "Users", new { imageModel.Page } );
        }

        /// <summary>
        /// Returns the view to display images with a list of images
        /// </summary>
        /// <param name="page">The page name/source</param>
        /// <returns>The partial view of images</returns>
        public ActionResult ImageView(string page)
        {
            ImageServices service = new ImageServices();
            UserServices userService = new UserServices();
            List<Image> model = service.GetUserImageData(userService.GetUserIdOfCurrentPage(page));
            service.SetPages(model, page);
            return PartialView("_Images", model);
        }

        /// <summary>
        /// Returns an image to display for deletion
        /// </summary>
        /// <param name="imageId">The id of a specific image to delete</param>
        /// <param name="page">The page name/source</param>
        /// <returns>The view to display and delete an image</returns>
        public ActionResult Delete(int imageId, string page)
        {
            ImageServices service = new ImageServices();
            Image image = service.Find(imageId); 
            if (image == null)
            {
                return HttpNotFound();
            }
            image.Page = page;
            return View(image);
        }

        /// <summary>
        /// Sends an image to services for deletion
        /// </summary>
        /// <param name="image">The image to delete</param>
        /// <returns>The source page of the delete</returns>
        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Image image)
        {
            ImageServices service = new ImageServices();
            if (!service.Delete(image.ImageId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return RedirectToAction("GetUserPage", "Users", new { page = image.Page });
        }

        /// <summary>
        /// Renders the edit page with image data to display
        /// </summary>
        /// <param name="imageId">The identifier for an image</param>
        /// <param name="page">The page name/source</param>
        /// <returns>The edit page</returns>
        public ActionResult Edit(int imageId, string page)
        {
            ImageServices service = new ImageServices();
            Image image = service.Find(imageId);
            if (image == null)
            {
                return HttpNotFound();
            }
            image.Page = page;
            return View(image);
        }

        /// <summary>
        /// Sends an image to services for editing/modifying
        /// </summary>
        /// <param name="image">The image to edit</param>
        /// <returns>The source page</returns>
        // POST: Physiologies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Image image)
        {
            if (ModelState.IsValid)
            {
                ImageServices service = new ImageServices();
                service.Edit(image);
                return RedirectToAction("GetUserPage", "Users", new { page = image.Page });
            }
            return View(image);
        }
    }
}