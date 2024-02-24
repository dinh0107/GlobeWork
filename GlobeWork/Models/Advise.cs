using System;
using System.ComponentModel.DataAnnotations;

namespace GlobeWork.Models
{
    public class Advise
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? JobPostId { get; set; }
        public int? StudyAbroadId { get; set; }
        [Display(Name = "Lời nhắn"), DataType(DataType.MultilineText), StringLength(4000)]
        public string Body { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual User User { get; set; }
        public virtual JobPost JobPost { get; set; }
        public virtual StudyAbroad StudyAbroad { get; set; }
        public Advise() {
             CreateDate = DateTime.Now;
        }
    }
}