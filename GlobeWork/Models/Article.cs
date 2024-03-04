using GlobeWork.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlobeWork.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Display(Name = "Tiêu đề", Description = "Tiêu đề dài tối đa 150 ký tự"),
         Required(ErrorMessage = "Hãy nhập tiêu đề"), StringLength(150, ErrorMessage = "Tối đa 150 ký tự"),
         UIHint("TextBox")]
        public string Subject { get; set; }
        [Display(Name = "Trích dẫn ngắn"), Required(ErrorMessage = "Hãy nhập trích dẫn ngắn"),
         StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Nội dung"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Ảnh đại diện"), UIHint("ImageArticle")]
        public string Image { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Display(Name = "Ngày đăng")]
        public DateTime CreateDate { get; set; }
        public int View { get; set; }
        public int? EmployerId { get; set; }
        public int? AdminId { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Hiện trang chủ")]
        public bool IsAdmin { get; set; }
        [Display(Name = "Hiện Menu")]
        public bool Menu { get; set; }

        [Display(Name = "Hiện chân trang")]
        public bool Footer { get; set; }
        [StringLength(300)]
        public string Url { get; set; }
        public int? StudyAbroadCategoryId { get; set; }
        public DateTime? Hot { get; set; }

        [Display(Name = "Thẻ tiêu đề"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string TitleMeta { get; set; }

        [Display(Name = "Thẻ mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string DescriptionMeta { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        public virtual Employer Employer { get; set; }
        public virtual Admin Admin { get; set; }
        public virtual StudyAbroadCategory StudyAbroadCategory { get; set; }
        [Display(Name ="Loại bài viết")]
        public TypeArticle TypeArticle { get; set; }

        public Article()
        {
            CreateDate = DateTime.Now;
            Active = true;
            View = 1;
            Sort = 1;
        }
    }
    public enum TypeArticle
    {
        [Display(Name ="Tin tức")]
        Article,
        [Display(Name ="Trường")]
        School
    }
}