using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlobeWork.Models
{
    public class JobType
    {
        public int Id { get; set; }
        [Display(Name = "Tên loại công việc"), Required(ErrorMessage = "Vui lòng nhập tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string Name { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [Display(Name = "Ngày tạo"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreateDate { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<JobPost> Posts { get; set; }
        public JobType()
        {
            CreateDate = DateTime.Now;
        }
    }
}