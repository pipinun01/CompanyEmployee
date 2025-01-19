using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using Repository.Extensions;


namespace Repository
{
    public class EmployeeRepository: RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext context) : base(context) { }
        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employess = await FindByCondition(e=>e.CompanyId == companyId, trackChanges)
                .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
                .Search(employeeParameters.SearchItem)
                .OrderBy(or=>or.Name)
                .Skip((employeeParameters.pageNumber -1) * employeeParameters.pageSize)
                .Take(employeeParameters.pageSize)
                .ToListAsync();
            var count = await FindByCondition(e=>e.CompanyId.Equals(companyId), trackChanges).CountAsync();
            return new PagedList<Employee>(employess, count, employeeParameters.pageNumber, employeeParameters.pageSize);
        }
        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) => await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }
        public void DeleteEmployee(Employee employee)=>Delete(employee);
    }
}
