using GlobeWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace GlobeWork.ViewModel
{
    public class ListCareerViewModel
    {
        public IPagedList<Career> Careers { get; set; }
        public string Name { get; set; }
    }

    public class ListSkillViewModel
    {
        public IPagedList<Skill> Skills { get; set; }
        public string Name { get; set; }
    }
}