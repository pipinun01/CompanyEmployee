using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyRepository: RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext context): base(context) { }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)=> await FindAll(trackChanges).OrderBy(x => x.Name).ToListAsync();
        public async Task<Company> GetCompanyAsync(Guid companyGuid, bool trackChanges) => await FindByCondition(c => c.Id.Equals(companyGuid), trackChanges).SingleOrDefaultAsync();

        public void CreateCompany(Company company)=>Create(company);
        public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)=> await FindByCondition(x=> ids.Contains(x.Id), trackChanges).ToListAsync();
        public void DeleteCompany(Company company)=>Delete(company);
    }
}
