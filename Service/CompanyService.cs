using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

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
        public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges)
        {
            var companies = _repositoryManager.CompanyRepository.GetAllCompanies(trackChanges);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return companiesDto;
        }
        public CompanyDto GetCompany(Guid companyGuid, bool trackChanges)
        {
            var company = _repositoryManager.CompanyRepository.GetCompany(companyGuid, trackChanges);

            //проверка на null
            if(company == null) 
                throw new CompanyNotFoundException(companyGuid);
            var companyDto = _mapper.Map<CompanyDto>(company);
            return companyDto;
        }
        public CompanyDto CreateCompany(CompanyForCreationDto company)
        {
            var companyEntity = _mapper.Map<Company>(company);
            _repositoryManager.CompanyRepository.CreateCompany(companyEntity);
            _repositoryManager.Save();
            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return companyToReturn;
        }

        public IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
        {
            if(ids is null)
            {
                throw new IdParametersBadRequestException();
            }
            var companyEntity = _repositoryManager.CompanyRepository.GetByIds(ids, trackChanges);
            if(ids.Count() != companyEntity.Count())
            {
                throw new CollectionByIdsBadRequestException();
            }
            var companyToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntity);
            return companyToReturn;
        }
        public (ICollection<CompanyDto> companies, string ids) CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyCollaction)
        {
            if (companyCollaction is null)
                throw new CompanyCollectionBadRequest();
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollaction);
            foreach(var company in companyEntities)
            {
                _repositoryManager.CompanyRepository.CreateCompany(company);
            }
            _repositoryManager.Save();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c=>c.id));
            return(companies: companyCollectionToReturn.ToList(),  ids);
        }
        public void DeleteCompany(Guid companyId, bool trackChanges)
        {
            var company = _repositoryManager.CompanyRepository.GetCompany(companyId, trackChanges);
            if(company == null) 
                throw new CompanyNotFoundException(companyId);
            _repositoryManager.CompanyRepository.DeleteCompany(company);
            _repositoryManager.Save();
        }
        public void UpdateCompany(Guid companyid, CompanyForUpdateDto companyForUpdate, bool trackChanges)
        {
            var companyEntity = _repositoryManager.CompanyRepository.GetCompany(companyid, trackChanges);
            if( companyEntity == null)
                throw new CompanyNotFoundException(companyid);
            _mapper.Map(companyForUpdate, companyEntity);
            _repositoryManager.Save();
        }
    }
}
