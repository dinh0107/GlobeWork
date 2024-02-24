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
        public DbSet<Banner> Banners { get; set; }
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
        public DbSet<Country> Countries { get; set; }
        public DbSet<StudyAbroadCategory>  StudyAbroadCategories { get; set; }
        public DbSet<StudyAbroad>  StudyAbroads { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experiences> Experiences { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<UserViewLog> UserViewLogs { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Advise> Advises { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<Activity>  Activities { get; set; }

    }
}