using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlobeWork.Models
{
    public class Country
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Hãy nhập tên quốc gia"), Display(Name = "Tên quốc gia"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Name { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int Sort { get; set; }
        [Display(Name ="Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Nổi bật")]
        public bool Hot { get; set; }
        [Display(Name = "Hiện chân trang")]
        public bool Footer { get; set; }
        [Display(Name = "Học bổng")]
        public bool Scholarship { get; set; }
        [Display(Name = "Trường")]
        public bool School { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<JobPost> JobPosts { get; set; }
        public virtual ICollection<StudyAbroadCategory> StudyAbroadCategories { get; set; }
    }
}