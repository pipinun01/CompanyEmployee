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
    }
}
