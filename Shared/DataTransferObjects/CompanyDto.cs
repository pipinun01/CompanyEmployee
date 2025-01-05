namespace Shared.DataTransferObjects
{
    public record CompanyDto(Guid id, string Name, string fullAdress);

    public record CompanyForCreationDto(string Name, string Address, string Country, IEnumerable<EmployeeForCreationDto> Employees);
}
