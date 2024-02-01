using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using GlobeWork.Models;

namespace GlobeWork.DAL
{
    public class DataEntities : DbContext
    {
        public DataEntities() : base("name=DataEntities") { }
        public DbSet<ConfigSite> ConfigSites { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Career> Careers { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<JobType> JobTypes { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<ApplyJob> ApplyJobs { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<EmployerLog> EmployerLogs { get; set; }

    }
}