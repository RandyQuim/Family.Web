using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web;
using Family.Web.Models;

namespace Family.Web.Services
{
    public class ImageServices
    {
        private Entities db = new Entities();

        /// <summary>
        /// Saves a System.Drawing.Image to the specified path and all data relating to an image to the database as a new record
        /// </summary>
        /// <param name="imageModel">The image model of data for the database</param>
        /// <param name="fileName">The specified path to save the image</param>
        public void SaveImage(Models.Image imageModel, string fileName)
        {
            System.Drawing.Image image = ImageRotation(imageModel);
            image.Save(fileName);
            UserServices service = new UserServices();
            imageModel.UserId = service.GetUser().UserId;
            db.Images.Add(imageModel);
            db.SaveChanges();
        }

        /// <summary>
        /// Returns a list of images pertaining to a specific user
        /// </summary>
        /// <param name="userId">The identifier for a user</param>
        /// <returns>A list of images</returns>
        public List<Models.Image> GetUserImageData(int userId)
        {
            return db.Images.Where(x => x.UserId == userId).ToList();
        }

        /// <summary>
        /// Sets the page property of all images
        /// </summary>
        /// <param name="images">A list of images</param>
        /// <param name="page">Identifies the page name relating to a specific user to render the proper view</param>
        public void SetPages(List<Models.Image> images, string page)
        {
            images.ForEach(x => { x.Page = page; });
        }

        /// <summary>
        /// Finds and returns an image entity 
        /// </summary>
        /// <param name="imageId">The identifier for a specific image model</param>
        /// <returns>The image model</returns>
        public Models.Image Find(int imageId)
        {
            return db.Images.Find(imageId);
        }

        /// <summary>
        /// Removes/deletes a row of image data 
        /// </summary>
        /// <param name="imageId">The identifier for a specific image model</param>
        /// <returns>The boolean value determining if the image was found or not</returns>
        public bool Delete(int imageId)
        {
            Models.Image image = db.Images.Find(imageId);
            if (image != null)
            {
                db.Images.Remove(image);
                db.SaveChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Modifies changes in an image record in the database
        /// </summary>
        /// <param name="image">The image model/entity</param>
        public void Edit(Models.Image image)
        {
            db.Entry(image).State = EntityState.Modified;
            db.SaveChanges();
        }

       /// <summary>
       /// Rotates the image when posted from mobile (mobile pictures were showing up sideways on computers)
       /// </summary>
       /// <param name="image">The image model containing the image file from HttpPostedFiledBase class</param>
       /// <returns>The properly rotated image as a System.Drawing.Image</returns>
        public System.Drawing.Image ImageRotation(Models.Image image)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(image.ImageFile.InputStream, true, true);

            if (originalImage.PropertyIdList.Contains(0x0112))
            {
                int rotationValue = originalImage.GetPropertyItem(0x0112).Value[0];
                switch (rotationValue)
                {
                    case 1: // landscape, do nothing
                        break;

                    case 8: // rotated 90 right
                            // de-rotate:
                        originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                        break;

                    case 3: // bottoms up
                        originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                        break;

                    case 6: // rotated 90 left
                        originalImage.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                        break;
                }
            }

            return originalImage;
        }
    }
}