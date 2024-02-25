using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlobeWork.Models
{
    public class Career
    {
        public int Id { get; set; }
        [Display(Name = "Ngành nghề"), Required(ErrorMessage = "Vui lòng nhập tên ngành nghề"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Ảnh đại diện"), UIHint("ImageCareer")]
        public string Image { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự")]
        public string Url { get; set; }
        [Display(Name = "Giới thiệu chung"), StringLength(500), UIHint("TextArea")]
        public string Description { get; set; }
        [Display(Name = "Mô tả"), UIHint("EditorBox")]
        public string Body { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Danh mục cha")]
        public int? ParentId { get; set; }
        [Display(Name = "Hiện trang chủ")]
        public bool ShowHome { get; set; }
        [Display(Name = "Nổi bật")]
        public bool Hot { get; set; }
        [Display(Name = "Hiện chân trang")]
        public bool Footer { get; set; }
        [Display(Name = "Ngành du học Hot")]
        public bool StudyHot { get; set; }
        [Display(Name = "Hiện menu")]
        public bool Menu { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int Sort { get; set; }
        [Display(Name = "Ngày tạo"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreateDate { get; set; }
        public virtual ICollection<JobPost> Posts { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<StudyAbroad> StudyAbroads { get; set; }

        public Career()
        {
            CreateDate = DateTime.Now;
            Active = true;
        }
    }
}