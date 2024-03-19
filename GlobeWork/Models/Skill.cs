using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlobeWork.Models
{
    public class Skill
    {
        public int Id { get; set; }
        [Display(Name = "Kỹ năng"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string SkillName { get; set; }
        [Display(Name = "Hiện trang chủ")]
        public bool ShowHome { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Display(Name = "Ngày tạo"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreateDate { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }
        public virtual ICollection<JobPost> Posts { get; set; }
        public virtual ICollection<User> Users { get; set; }
        [Display(Name = "Loại")]
        public TypeSkill TypeSkill { get; set; }
        public Skill()
        {
            CreateDate = DateTime.Now;
        }
    }
    public enum TypeSkill
    {
        [Display(Name ="Kỹ năng")]
        Skill,
        [Display(Name = "Từ khóa nổi bât")]
        Keyword
    }
}