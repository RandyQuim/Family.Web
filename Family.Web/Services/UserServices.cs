using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Family.Web.Controllers;
using Family.Web.Models;

namespace Family.Web.Services
{
    public class UserServices
    {
        /// <summary>
        /// The database context
        /// </summary>
        private Entities db = new Entities();

        /// <summary>
        /// Retrieves the user data of the currently logged in user
        /// </summary>
        /// <returns>The logged in user data</returns>
        public UserDto GetUser()
        {
            if (Principal.Identity == null || Principal.Identity.Name == null)
            {
                return new UserDto() { Name = "" };
            }

            var userInfo = (from x in db.Users
                where x.UserId == Principal.Identity.UserId
                select new UserDto
                {
                    Name = x.Name,
                    Permission = x.Permission,
                    Password = x.Password,
                    UserName = x.UserName,
                    UserId = x.UserId,               
                    BirthDate = x.BirthDate
                }).FirstOrDefault();

            return userInfo;
        }

        /// <summary>
        /// Overloaded method to return a specific user 
        /// </summary>
        /// <param name="userId">The identifier of a user</param>
        /// <returns>The specified user</returns>
        public UserDto GetUser(int userId)
        {
            var userInfo = (from x in db.Users
                            where x.UserId == userId
                            select new UserDto
                            {
                                Name = x.Name,
                                Permission = x.Permission,
                                Password = x.Password,
                                UserName = x.UserName,
                                UserId = x.UserId,
                                BirthDate = x.BirthDate
                            }).FirstOrDefault();

            return userInfo;
        }

        /// <summary>
        /// Retrives a list of all users
        /// </summary>
        /// <returns>The list of users</returns>
        public List<UserDto> GetUsers()
        {
            var usersInfo = from x in this.db.Users
                select new UserDto()
                {
                    Name = x.Name,
                    Permission = x.Permission,
                    Password = x.Password,
                    UserName = x.UserName,
                    UserId = x.UserId,
                    Page = x.Page,
                    BirthDate = x.BirthDate
                };
            return usersInfo.ToList();
        }

        /// <summary>
        /// Validates a user input username and password and returns the record if successful
        /// </summary>
        /// <param name="loginField">User entered login information</param>
        /// <returns>The logged in user</returns>
        public UserDto ValidateLogIn(UserDto loginField)
        {
            UserDto userInfo;
            try
            {
                userInfo = (from x in this.db.Users
                    where x.UserName == loginField.UserName && x.Password == loginField.Password
                    select new UserDto()
                    {
                        Name = x.Name,
                        Permission = x.Permission,
                        Password = x.Password,
                        UserName = x.UserName,
                        UserId = x.UserId
                    }).Single();

                Principal.Identity = userInfo;
            }
            catch (Exception)
            {
                return null;
            }

            return userInfo;
        }

        /// <summary>
        /// Retrieves the identifier for a user relating to a specific page name
        /// </summary>
        /// <param name="page">The name of the page to display proper view</param>
        /// <returns>The user id relating to a page</returns>
        public int GetUserIdOfCurrentPage(string page)
        {
            return (from user in this.db.Users
                         where user.Page == page
                         select user.UserId).FirstOrDefault();
        }

        /// <summary>
        /// Calculates the time span from the user's birthdate until present for display
        /// </summary>
        /// <param name="birthDateTime">The user's birthdate</param>
        /// <returns>The formatted string representing time span since birthdate</returns>
        public string GetTimespanSinceBirthdate(DateTime? birthDateTime)
        {
            DateTime now = DateTime.Now;
            int years = new DateTime(DateTime.Now.Subtract(birthDateTime.GetValueOrDefault()).Ticks).Year - 1;
            DateTime pastYearDate = birthDateTime.GetValueOrDefault().AddYears(years);
            int months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (pastYearDate.AddMonths(i) == now)
                {
                    months = i;
                    break;
                }
                if (pastYearDate.AddMonths(i) >= now)
                {
                    months = i - 1;
                    break;
                }
            }

            int days = now.Subtract(pastYearDate.AddMonths(months)).Days;
            int hours = now.Subtract(pastYearDate).Hours;
            int minutes = now.Subtract(pastYearDate).Minutes;
            int seconds = now.Subtract(pastYearDate).Seconds;
            return $"Age: {years} Year(s) {months} Month(s) {days} Day(s) {hours} Hour(s) {minutes} Minute(s) {seconds} Second(s)";
        }
    }
}