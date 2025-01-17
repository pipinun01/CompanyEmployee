using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using System.ComponentModel.Design;

namespace Service
{
    internal sealed class CompanyService: ICompanyService
    {
        private readonly IRepositoryManager _repositoryManager;
        private ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompanyService(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges)
        {
            var companies = await _repositoryManager.CompanyRepository.GetAllCompaniesAsync(trackChanges);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return companiesDto;
        }
        public async Task<CompanyDto> GetCompanyAsync(Guid companyGuid, bool trackChanges)
        {
            //var company = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyGuid, trackChanges);

            ////проверка на null
            //if(company == null) 
            //    throw new CompanyNotFoundException(companyGuid);
            var company = await GetCompanyAndCheckIfItExists(companyGuid, trackChanges);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }
        public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company)
        {
            var companyEntity = _mapper.Map<Company>(company);
            _repositoryManager.CompanyRepository.CreateCompany(companyEntity);
            await _repositoryManager.SaveAsync();
            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return companyToReturn;
        }

        public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            if(ids is null)
            {
                throw new IdParametersBadRequestException();
            }
            var companyEntity = await _repositoryManager.CompanyRepository.GetByIdsAsync(ids, trackChanges);
            if(ids.Count() != companyEntity.Count())
            {
                throw new CollectionByIdsBadRequestException();
            }
            var companyToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntity);
            return companyToReturn;
        }
        public async Task<(ICollection<CompanyDto> companies, string ids)> CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollaction)
        {
            if (companyCollaction is null)
                throw new CompanyCollectionBadRequest();
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollaction);
            foreach(var company in companyEntities)
            {
                _repositoryManager.CompanyRepository.CreateCompany(company);
            }
            await _repositoryManager.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c=>c.id));
            return(companies: companyCollectionToReturn.ToList(),  ids);
        }
        public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
        {
            //var company = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyId, trackChanges);
            //if(company == null) 
            //    throw new CompanyNotFoundException(companyId);
            var company = await GetCompanyAndCheckIfItExists(companyId, trackChanges);
            _repositoryManager.CompanyRepository.DeleteCompany(company);
            await _repositoryManager.SaveAsync();
        }
        public async Task UpdateCompanyAsync(Guid companyid, CompanyForUpdateDto companyForUpdate, bool trackChanges)
        {
            //var companyEntity = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyid, trackChanges);
            //if( companyEntity == null)
            //    throw new CompanyNotFoundException(companyid);
            var companyEntity = await GetCompanyAndCheckIfItExists(companyid, trackChanges);
            _mapper.Map(companyForUpdate, companyEntity);
            await _repositoryManager.SaveAsync();
        }
        private async Task<Company> GetCompanyAndCheckIfItExists(Guid companyId, bool trackChanges)
        {
            var companyEntity = await _repositoryManager.CompanyRepository.GetCompanyAsync(companyId, trackChanges);
            if (companyEntity == null)
                throw new CompanyNotFoundException(companyId);
            return companyEntity;
        }
    }
}
