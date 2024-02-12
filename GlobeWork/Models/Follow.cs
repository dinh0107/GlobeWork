using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobeWork.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public User User { get; set; } 
        public Company Company { get; set; } 
    }
    public class Like
    {
        public int Id { get; set; }
        public int UserID { get; set; }
        public int JobId { get; set; }
        public User User { get; set; }
        public JobPost JobPost { get; set; }
    }
}