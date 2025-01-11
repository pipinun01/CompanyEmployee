using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class EmployeeService: IEmployeeService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeeService(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
        }
        public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
        {
            var company = _repositoryManager.CompanyRepository.GetCompany(companyId, trackChanges);
            if(company is null)
            {
                throw new CompanyNotFoundException(companyId);
            }
            var employeesFromDb = _repositoryManager.EmployeeRepository.GetEmployees(companyId, trackChanges);
            var employeeDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            return employeeDto;
        }
        public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            var comoany = _repositoryManager.CompanyRepository.GetCompany(companyId, trackChanges);
            if(comoany is null)
                throw new CompanyNotFoundException(companyId);

            var employeeDb = _repositoryManager.EmployeeRepository.GetEmployee(companyId, id, trackChanges);
            if (employeeDb is null)
                throw new EmployeeNotFoundException(id);

            var employeeDto = _mapper.Map<EmployeeDto>(employeeDb);
            return employeeDto;
        }
        public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
        {
            var company = _repositoryManager.CompanyRepository.GetCompany(companyId, false);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
            var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
            _repositoryManager.EmployeeRepository.CreateEmployeeForCompany(companyId, employeeEntity);
            _repositoryManager.Save();

            var employeeReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return employeeReturn;
        }
        public void DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges)
        {
            var company = _repositoryManager.CompanyRepository.GetCompany(companyId, trackChanges);
            if(company is null)
                throw new CompanyNotFoundException(companyId);
            var employeeForCompany = _repositoryManager.EmployeeRepository.GetEmployee(companyId, id, trackChanges);
            if (employeeForCompany is null)
                throw new EmployeeNotFoundException(id);
            _repositoryManager.EmployeeRepository.DeleteEmployee(employeeForCompany);
            _repositoryManager.Save();
        }
        public void UpdateEmployeeForCompany(Guid companyId,  Guid id, EmployeeForUpdateDto employeeForUpdateDto, bool compTrackChanges, bool empTrackChanges)
        {
            var company = _repositoryManager.CompanyRepository.GetCompany(companyId, compTrackChanges);
            if( company is null)
                throw new CompanyNotFoundException(companyId);
            var employeeEntity = _repositoryManager.EmployeeRepository.GetEmployee(companyId, id, empTrackChanges);
            if(employeeEntity is null)
                throw new EmployeeNotFoundException(id);
            _mapper.Map(employeeForUpdateDto, employeeEntity);
            _repositoryManager.Save();
        }
        public (EmployeeForUpdateDto employeeToPatch, Employee employeeEntity) GetEmployeeForPatch(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            var company = _repositoryManager.CompanyRepository.GetCompany(companyId, compTrackChanges);
            if(company is null)
                throw new CompanyNotFoundException(companyId);
            var employeeEntity = _repositoryManager.EmployeeRepository.GetEmployee(companyId, id, empTrackChanges);
            if( employeeEntity is null)
                throw new EmployeeNotFoundException(id);
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
            return (employeeToPatch, employeeEntity);
        }
        public void SaveChangesForPatch(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            _repositoryManager.Save();
        }
    }
}
