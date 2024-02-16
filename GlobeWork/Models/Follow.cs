using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GlobeWork.Models
{
    public class Follow
    {
        public int Id { get; set; }
        [Display(Name ="Tài khoản")]
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public virtual User User { get; set; } 
        public virtual Company Company { get; set; } 
    }
    public class Like
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public int JobId { get; set; }
        public virtual User User { get; set; }
        public virtual JobPost JobPost { get; set; }
    }
}