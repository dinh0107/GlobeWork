using GlobeWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobeWork.ViewModel
{
    public class EmployerViewModel
    {
        public Employer Employer { get; set; }
    }
    public class InsertEmployerViewModel
    {
        public JobPost JobPost { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<JobType> JobTypes { get; set; }
        public IEnumerable<Rank>  Ranks { get; set; }
        //public IEnumerable<Rank>  Ranks { get; set; }

    }
}