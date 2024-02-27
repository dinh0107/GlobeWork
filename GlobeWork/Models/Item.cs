using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlobeWork.Models
{
    public class Item
    {
        public int Sort { get; set; }
        [Display(Name = "Tiêu đề"), Required(ErrorMessage = "Chưa nhập tiêu đề"), UIHint("TextBox")]
        public string Title { get; set; }
        [Display(Name = "Mô tả"), UIHint("TextBox")]
        public string Description { get; set; }
    }

    public class Parter
    {
        public int Id { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int Sort { get; set; }
        [Display(Name = "Tiêu đề"), Required(ErrorMessage = "Chưa nhập tiêu đề"), UIHint("TextBox")]
        public string Title { get; set; }
        [Display(Name = "Url"), UIHint("TextBox")]
        public string Url { get; set; }
    }
}