using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Family.Web.Models
{
    public class UserDto
    {
        public int UserId { get; set; }

        [Display(Name = "Full Name")]
        public string Name { get; set; }

        public string Permission { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Display(Name = "User Name")]
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        public DateTime? BirthDate { get; set; }
        public string Age { get; set; }
        public Time Time { get; set; }

        public Image Image { get; set; }
        public List<Image> Images { get; set; }
        public string Page { get; set; }
    }
}