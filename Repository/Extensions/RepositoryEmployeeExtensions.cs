using Entities.Models;
using System.Linq.Dynamic.Core;
using Repository.Extensions.Utility;

namespace Repository.Extensions
{
    public static class RepositoryEmployeeExtensions
    {
        public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> employees, uint minAge, uint maxAge) => employees.Where(w => (w.Age >= minAge && w.Age <= maxAge));

        public static IQueryable<Employee> Search(this IQueryable<Employee> employees, string searchTerm)
        {
            if(string.IsNullOrEmpty(searchTerm)) return employees;
            var lowerCaseTerm = searchTerm.Trim().ToLower();

            return employees.Where(w => w.Name.ToLower().Contains(lowerCaseTerm));
        }

        public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, string orderByQueryString)
        {
            if (string.IsNullOrEmpty(orderByQueryString)) return employees.OrderBy(or=>or.Name);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Employee>(orderByQueryString);

            if(string.IsNullOrEmpty(orderQuery)) return employees.OrderBy(or => or.Name);
            return employees.OrderBy(orderQuery);
        }
    }
}
