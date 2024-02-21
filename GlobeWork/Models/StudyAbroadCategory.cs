using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlobeWork.Models
{
    public class StudyAbroadCategory
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        [Display(Name = "Danh mục cha")]
        public int? ParentId { get; set; }
        [Display(Name = "Hiển thị menu")]
        public bool ShowMenu { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Ảnh đại diện"), StringLength(500)]
        public string Image { get; set; }
        [Display(Name = "Banner"), StringLength(500)]
        public string Banner { get; set; }
        [Display(Name = "Quốc gia"), Required(ErrorMessage = "Hãy nhập chọn Quốc gia")]
        public int CountryId { get; set; }
        public virtual ICollection<StudyAbroad> StudyAbroads { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
        public virtual Country Country { get; set; }
        [Display(Name = "Thẻ tiêu đề"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string TitleMeta { get; set; }
        [Display(Name = "Thẻ mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string DescriptionMeta { get; set; }
        public StudyAbroadCategory()
        {
            Active = true;
        }
    }
}