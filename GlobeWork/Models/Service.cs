using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlobeWork.Models
{
    public class Service
    {
        public int Id { get; set; }
        [Display(Name = "Tên gói"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Giá"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? Amount { get; set; }
        [Display(Name = "Số ngày"), UIHint("NumberBox")]
        public int IntDate {  get; set; }
        [Display(Name = "Trích dẫn ngắn"), Required(ErrorMessage = "Hãy nhập trích dẫn ngắn"),
        StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string Image { get; set; }

        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        public DateTime CreateDate { get; set; }
        [Display(Name = "Loại gói hiển thị")]
        public TypeService TypeService { get; set; }
        public Service()
        {
            CreateDate = DateTime.Now;
        }
    }
    public enum TypeService
    {
        [Display(Name ="Việc làm")]
        Job,
        [Display(Name ="Du học")]
        StudyAbroad,
        [Display(Name ="Công ty")]
        Company
    }
}