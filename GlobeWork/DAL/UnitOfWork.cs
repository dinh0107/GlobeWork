using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using GlobeWork.Models;
using WebGrease.Css.Ast.Selectors;

namespace GlobeWork.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly DataEntities _context = new DataEntities();
        private GenericRepository<Admin> _adminRepository;
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