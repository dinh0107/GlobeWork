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
}