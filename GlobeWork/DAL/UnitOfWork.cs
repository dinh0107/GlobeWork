using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using GlobeWork.Models;
using GlobeWork.Models;
using WebGrease.Css.Ast.Selectors;

namespace GlobeWork.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly DataEntities _context = new DataEntities();
        private GenericRepository<ConfigSite> _configSiteRepository;
        private GenericRepository<Admin> _adminRepository;
        private GenericRepository<Banner> _bannerRepository;
        private GenericRepository<User> _userRepository;
        private GenericRepository<Rank> _rankRepository;
        private GenericRepository<JobType> _jobTypeRepository;
        private GenericRepository<ApplyJob> _applyJobRepository;
        private GenericRepository<Candidate> _candidateRepository;
        private GenericRepository<Career> _careerRepository;
        private GenericRepository<City> _cityRepository;
        private GenericRepository<District> _districtRepository;
        private GenericRepository<Company> _companyRepository;
        private GenericRepository<JobPost> _jobPostRepository;
        private GenericRepository<Skill> _skillRepository;
        private GenericRepository<Employer> _employerRepository;
        private GenericRepository<EmployerLog> _employerLogRepository;
        private GenericRepository<Country> _countryLogRepository;
        private GenericRepository<StudyAbroadCategory> _studyAbroadCategoryRepository;
        private GenericRepository<StudyAbroad> _studyAbroadRepository;
        private GenericRepository<Follow> _followRepository;
        private GenericRepository<UserLog> _userLogRepository;
        private GenericRepository<Like> _likeRepository;
        private GenericRepository<Education> _educationRepository;
        private GenericRepository<Experiences> _experienceRepository;
        private GenericRepository<Project> _projectRepository;
        private GenericRepository<Certificate> _certificateRepository;
        private GenericRepository<UserViewLog> _userViewLogRepository;
        private GenericRepository<Article> _articleRepository;


        public GenericRepository<Article> ArticleRepository =>
      _articleRepository ?? (_articleRepository = new GenericRepository<Article>(_context));
        public GenericRepository<UserViewLog> UserViewLogRepository =>
       _userViewLogRepository ?? (_userViewLogRepository = new GenericRepository<UserViewLog>(_context));
        public GenericRepository<Project> ProjectRepository =>
        _projectRepository ?? (_projectRepository = new GenericRepository<Project>(_context));
        public GenericRepository<Certificate> CertificateRepository =>
        _certificateRepository ?? (_certificateRepository = new GenericRepository<Certificate>(_context));

        public GenericRepository<Education> EducationRepository =>
       _educationRepository ?? (_educationRepository = new GenericRepository<Education>(_context));
        public GenericRepository<Experiences> ExperienceRepository =>
        _experienceRepository ?? (_experienceRepository = new GenericRepository<Experiences>(_context));
        public GenericRepository<UserLog> UserLogRepository =>
        _userLogRepository ?? (_userLogRepository = new GenericRepository<UserLog>(_context));
        public GenericRepository<Banner> BannerRepository =>
        _bannerRepository ?? (_bannerRepository = new GenericRepository<Banner>(_context));
        public GenericRepository<Follow> FollowRepository =>
        _followRepository ?? (_followRepository = new GenericRepository<Follow>(_context));
        public GenericRepository<Like> LikeRepository =>
        _likeRepository ?? (_likeRepository = new GenericRepository<Like>(_context));
        public GenericRepository<StudyAbroadCategory> StudyAbroadCategoryRepository =>
         _studyAbroadCategoryRepository ?? (_studyAbroadCategoryRepository = new GenericRepository<StudyAbroadCategory>(_context));
        public GenericRepository<StudyAbroad> StudyAbroadRepository =>
         _studyAbroadRepository ?? (_studyAbroadRepository = new GenericRepository<StudyAbroad>(_context));
        public GenericRepository<ConfigSite> ConfigSiteRepository =>
          _configSiteRepository ?? (_configSiteRepository = new GenericRepository<ConfigSite>(_context));
        public GenericRepository<Country> CountryRepository =>
         _countryLogRepository ?? (_countryLogRepository = new GenericRepository<Country>(_context));
        public GenericRepository<Employer> EmployerRepository =>
           _employerRepository ?? (_employerRepository = new GenericRepository<Employer>(_context));
        public GenericRepository<Admin> AdminRepository =>
            _adminRepository ?? (_adminRepository = new GenericRepository<Admin>(_context));
        public GenericRepository<User> UserRepository =>
            _userRepository ?? (_userRepository = new GenericRepository<User>(_context));
        public GenericRepository<ApplyJob> ApplyJobRepository =>
            _applyJobRepository ?? (_applyJobRepository = new GenericRepository<ApplyJob>(_context));
        public GenericRepository<Rank> RankRepository =>
            _rankRepository ?? (_rankRepository = new GenericRepository<Rank>(_context));
        public GenericRepository<JobType> JobTypeRepository =>
            _jobTypeRepository ?? (_jobTypeRepository = new GenericRepository<JobType>(_context));
        public GenericRepository<City> CityRepository =>
            _cityRepository ?? (_cityRepository = new GenericRepository<City>(_context));
        public GenericRepository<District> DistrictRepository =>
           _districtRepository ?? (_districtRepository = new GenericRepository<District>(_context));
        public GenericRepository<Candidate> CandidateRepository =>
            _candidateRepository ?? (_candidateRepository = new GenericRepository<Candidate>(_context));
        public GenericRepository<Career> CareerRepository =>
            _careerRepository ?? (_careerRepository = new GenericRepository<Career>(_context));
        public GenericRepository<Company> CompanyRepository =>
            _companyRepository ?? (_companyRepository = new GenericRepository<Company>(_context));
        public GenericRepository<JobPost> JobPostRepository =>
            _jobPostRepository ?? (_jobPostRepository = new GenericRepository<JobPost>(_context));
        public GenericRepository<Skill> SkillRepository =>
            _skillRepository ?? (_skillRepository = new GenericRepository<Skill>(_context));
        public GenericRepository<EmployerLog> EmployerLogRepository =>
    _employerLogRepository ?? (_employerLogRepository = new GenericRepository<EmployerLog>(_context));
        public void Save()
        {
            _context.SaveChanges();
        }
        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}