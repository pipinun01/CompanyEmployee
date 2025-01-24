using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Dynamic;

namespace Service
{
    internal sealed class EmployeeService: IEmployeeService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly IDataShaper<EmployeeDto> _dataShaper;

        public EmployeeService(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper, IDataShaper<EmployeeDto> dataShaper)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
            _dataShaper = dataShaper;
        }
        public async Task<(IEnumerable<ExpandoObject> employees, MetaData metada)> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            //var company = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyId, trackChanges);
            //if(company is null)
            //{
            //    throw new CompanyNotFoundException(companyId);
            //}
            if(!employeeParameters.ValidAgeRange)
                throw new MaxAgeRangeBadRequestException();
            await CheckIfCompanyExists(companyId, trackChanges);
            var employeesFromDb = await _repositoryManager.EmployeeRepository.GetEmployeesAsync(companyId, employeeParameters,  trackChanges);
            var employeeDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
            var shapeData = _dataShaper.ShapeData(employeeDto, employeeParameters.Fields);
            return (employees: shapeData, metada: employeesFromDb.MetaData);
        }
        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            //var comoany = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyId, trackChanges);
            //if(comoany is null)
            //    throw new CompanyNotFoundException(companyId);

            await CheckIfCompanyExists(companyId, trackChanges);

            //var employeeDb = await _repositoryManager.EmployeeRepository.GetEmployeeAsync(companyId, id, trackChanges);
            //if (employeeDb is null)
            //    throw new EmployeeNotFoundException(id);

            var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);
            var employeeDto = _mapper.Map<EmployeeDto>(employeeDb);
            return employeeDto;
        }
        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
        {
            //var company = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyId, false);
            //if (company is null)
            //    throw new CompanyNotFoundException(companyId);

            await CheckIfCompanyExists(companyId, trackChanges);
            var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
            _repositoryManager.EmployeeRepository.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repositoryManager.SaveAsync();

            var employeeReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return employeeReturn;
        }
        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            //var company = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyId, trackChanges);
            //if(company is null)
            //    throw new CompanyNotFoundException(companyId);

            await CheckIfCompanyExists(companyId, trackChanges);

            //var employeeForCompany = await _repositoryManager.EmployeeRepository.GetEmployeeAsync(companyId, id, trackChanges);
            //if (employeeForCompany is null)
            //    throw new EmployeeNotFoundException(id);

            var employeeForCompany = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);
            _repositoryManager.EmployeeRepository.DeleteEmployee(employeeForCompany);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateEmployeeForCompanyAsync(Guid companyId,  Guid id, EmployeeForUpdateDto employeeForUpdateDto, bool compTrackChanges, bool empTrackChanges)
        {
            //var company = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyId, compTrackChanges);
            //if( company is null)
            //    throw new CompanyNotFoundException(companyId);

            await CheckIfCompanyExists(companyId, compTrackChanges);

            //var employeeEntity = await _repositoryManager.EmployeeRepository.GetEmployeeAsync(companyId, id, empTrackChanges);
            //if(employeeEntity is null)
            //    throw new EmployeeNotFoundException(id);

            var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
            _mapper.Map(employeeForUpdateDto, employeeEntity);
            await _repositoryManager.SaveAsync();
        }
        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            //var company = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyId, compTrackChanges);
            //if(company is null)
            //    throw new CompanyNotFoundException(companyId);

            await CheckIfCompanyExists(companyId, compTrackChanges);

            //var employeeEntity = await _repositoryManager.EmployeeRepository.GetEmployeeAsync(companyId, id, empTrackChanges);
            //if( employeeEntity is null)
            //    throw new EmployeeNotFoundException(id);

            var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
            return (employeeToPatch, employeeEntity);
        }
        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repositoryManager.SaveAsync();
        }
        private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
        {
            var company = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyId, trackChanges);
            if (company is null)
                throw new CompanyNotFoundException(companyId);
        }
        private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
        {
            var employeeEntity = await _repositoryManager.EmployeeRepository.GetEmployeeAsync(companyId, id, trackChanges);
            if (employeeEntity is null)
                throw new EmployeeNotFoundException(id);
            return employeeEntity;
        }
    }
}
