﻿using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _context;
        private readonly Lazy<ICompanyRepository> _CompanyRepository;
        private readonly Lazy<IEmployeeRepository> _EmployeeRepository;

        public RepositoryManager(RepositoryContext context)
        {
            _context = context;
            _CompanyRepository = new Lazy<ICompanyRepository>(() => new CompanyRepository(_context));
            _EmployeeRepository = new Lazy<IEmployeeRepository>(() => new EmployeeRepository(_context));
        }
        public ICompanyRepository CompanyRepository => _CompanyRepository.Value;
        public IEmployeeRepository EmployeeRepository => _EmployeeRepository.Value;
        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
