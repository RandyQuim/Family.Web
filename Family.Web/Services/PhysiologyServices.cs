using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using Family.Web.Models;

namespace Family.Web.Services
{
    public class PhysiologyServices
    {
        /// <summary>
        /// The database context
        /// </summary>
        private Entities db = new Entities();
        
        /// <summary>
        /// Constant for inches per centimeter
        /// </summary>
        private readonly decimal InchesPerCentimeter = 0.3937M;

        /// <summary>
        /// Retrieves the a list of physiology records relating to a specific user
        /// </summary>
        /// <param name="userId">The identifier for a user</param>
        /// <returns>A user's physiology history in list form</returns>
        public List<PhysiologyDto> GetUserPhysiologyHistory(int userId)
        {
            var physiologyList = (from u in this.db.Users
                                  join up in this.db.User_Phy
                                      on u.UserId equals up.UserId
                                  join p in this.db.Physiologies
                                      on up.PhyId equals p.PhyId
                                  where u.UserId == userId
                                  select new PhysiologyDto()
                                  {
                                      Height = p.Height,
                                      Weight = p.Weight,
                                      Date = p.Date,
                                      IsLosing = p.IsLosing,
                                      PhyId = p.PhyId,
                                      UserId = userId,
                                      Comment = p.Comment,
                                      AgeSnapshot = p.AgeSnapshot,
                                      Page = (from x in this.db.Users
                                              where x.UserId == userId
                                              select x.Page).FirstOrDefault()
                                  }).ToList();

            foreach (var element in physiologyList)
            {
                ConvertToFeetAndInches(element);
            }

            return physiologyList;
        }

        /// <summary>
        /// Retrieves all physiology records for all users for administrative purposes
        /// </summary>
        /// <returns>The list of all physiology records</returns>
        public List<PhysiologyDto> GetAllUserPhysiologyHistory()
        {
            var allPhys = (from x in this.db.Physiologies
                          select new PhysiologyDto
                          {
                              Height = x.Height,
                              Weight = x.Weight,
                              Date = x.Date,
                              IsLosing = x.IsLosing,
                              Comment = x.Comment,
                              PhyId = x.PhyId,
                              UserName = (from up in this.db.User_Phy
                                         join user in this.db.Users
                                         on up.UserId equals user.UserId
                                         where x.PhyId == up.PhyId
                                         select user.Name).FirstOrDefault()
                          }).ToList();
            return allPhys;
        }

        /// <summary>
        /// Converts centimeters (as stored in decimal form in the database) to feet and inches
        /// </summary>
        /// <param name="obj">The physiology object to modify</param>
        private void ConvertToFeetAndInches(PhysiologyDto obj)
        {
            int totalInches = Convert.ToInt32(obj.Height.GetValueOrDefault() * InchesPerCentimeter);
            obj.Feet = totalInches / 12;
            obj.Inches = totalInches % 12;
        }

        /// <summary>
        /// Finds a specific physiology record and adds and adjusts relating properties for display 
        /// </summary>
        /// <param name="phyId">The identifier for a physiology record</param>
        /// <returns>The specified physiology dto model</returns>
        public PhysiologyDto FindPhysiologyRecord(int phyId)
        {
            Physiology physiology = db.Physiologies.Find(phyId);
            PhysiologyDto model = new PhysiologyDto()
            {
                Height = physiology.Height,
                Weight = physiology.Weight,
                Date = physiology.Date,
                IsLosing = physiology.IsLosing,
                PhyId = phyId,
                Comment = physiology.Comment,
                AgeSnapshot = physiology.AgeSnapshot,
                UserId = (from u in this.db.User_Phy
                          where u.PhyId == phyId
                          select u.UserId).FirstOrDefault(),
            };

            if(model.Comment == null)
            {
                model.Comment = "";
            }

            ConvertToFeetAndInches(model);

            return model;
        }

        /// <summary>
        /// Deletes/Removes a specific physiology record
        /// </summary>
        /// <param name="phyId">The identifier to a physiology record</param>
        /// <returns>The boolean value determining if the record was found for deletion or not</returns>
        public bool Delete(int phyId)
        {
            Physiology physiology = db.Physiologies.Find(phyId);
            if (physiology != null)
            {
                var userPhy = (from x in this.db.User_Phy
                    where x.PhyId == phyId
                    select x).Single();

                if (userPhy != null)
                {
                    db.Physiologies.Remove(physiology);
                    db.User_Phy.Remove(userPhy);
                    db.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Modifies a physiology record in the database
        /// </summary>
        /// <param name="physiology">The physiology dto model to save</param>
        public void Edit(PhysiologyDto physiology)
        {
            var phy = PhysiologyToDomain(physiology);
            db.Entry(phy).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// Saves a newly created physiology record
        /// </summary>
        /// <param name="physiology">The physiology model</param>
        public void SaveCreate(PhysiologyDto physiology)
        {
            var phyDomain = PhysiologyToDomain(physiology);

            UserServices userService = new UserServices();
            UserDto dto = userService.GetUser();
            phyDomain.AgeSnapshot = userService.GetTimespanSinceBirthdate(dto.BirthDate);

            var domain = new User_Phy
            {
                UserId = physiology.UserId,
                Physiology = phyDomain
            };
            db.User_Phy.Add(domain);
            db.SaveChanges();
        }

        /// <summary>
        /// Returns the page name of a specific user to render the proper view
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserPage(int userId)
        {
            var page = (from x in this.db.Users
                        where x.UserId == userId
                        select x.Page).FirstOrDefault();
            return page;
        }

        /// <summary>
        /// Converts inches to centimeters for storing height (in decimal form)
        /// </summary>
        /// <param name="physiology">The physiology model</param>
        /// <returns>The converted value to centimeters</returns>
        public decimal ConvertToCentimeters(PhysiologyDto physiology)
        {
            return Math.Round(((physiology.Feet * 12 + physiology.Inches) / InchesPerCentimeter), 2);
        }

        /// <summary>
        /// Transfers physiology dto model data into the domain model
        /// </summary>
        /// <param name="physiology">The physiology dto model</param>
        /// <returns>The physiology domain model</returns>
        private Physiology PhysiologyToDomain(PhysiologyDto physiology)
        {
            var phy = new Physiology
            {
                Height = ConvertToCentimeters(physiology),
                Weight = physiology.Weight,
                IsLosing = physiology.IsLosing,
                Date = physiology.Date,
                PhyId = physiology.PhyId,
                Comment = physiology.Comment,
                AgeSnapshot = physiology.AgeSnapshot
            };

            return phy;
        }
    }
}