using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Family.Web.Models
{
    public class PhysiologyDto
    {
        public int PhyId { get; set; }

        [Range(0, 10)]
        public int Feet { get; set; }

        [Range(0, 11)]
        public int Inches { get; set; }

        public decimal? Height { get; set; }

        [Range(0, 999.99)]
        [RegularExpression(@"^\d+\.\d{0,2}$", ErrorMessage = "Weight must be entered with one or two decimals")]
        public decimal? Weight { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [Display(Name = "Date/Time")]
        public System.DateTime Date { get; set; }

        [Display(Name = "Losing weight?")]
        public bool? IsLosing { get; set; }

        public int UserId { get; set; }
        public string AgeSnapshot { get; set; }
        public string Comment { get; set; }

        public string UserName { get; set; }
        public string Page { get; set; }
    }
}