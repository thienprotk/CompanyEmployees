using AutoMapper;
using Contracts;
using Entities;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : IEmployeeService
{
    public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges)
    {
        var company = await repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeesFromDb = await repository.Employee.GetEmployeesAsync(companyId, trackChanges);
        var employeesDto = mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

        return employeesDto;
    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
    {
        var company = await repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeDb = await repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
        if (employeeDb is null)
            throw new EmployeeNotFoundException(id);

        var employee = mapper.Map<EmployeeDto>(employeeDb);
        return employee;
    }

    public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreationDto, bool trackChanges)
    {
        var company = await repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = mapper.Map<Employee>(employeeForCreationDto);
        repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        await repository.SaveAsync();

        var employeeToReturn = mapper.Map<EmployeeDto>(employeeEntity);
        return employeeToReturn;
    }

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
    {
        var company = await repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeForCompany = await repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
        if (employeeForCompany is null)
            throw new EmployeeNotFoundException(id);

        repository.Employee.DeleteEmployee(employeeForCompany);
        await repository.SaveAsync();
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdateDto, bool compTrackChanges, bool empTrackChanges)
    {
        var company = await repository.Company.GetCompanyAsync(companyId, compTrackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = await repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges);
        if (employeeEntity is null)
            throw new EmployeeNotFoundException(id);

        mapper.Map(employeeForUpdateDto, employeeEntity);
        await repository.SaveAsync();
    }

    public async Task<(EmployeeForUpdateDto employeeToPatchDto, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
    {
        var company = await repository.Company.GetCompanyAsync(companyId, compTrackChanges);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = await repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges);
        if (employeeEntity is null)
            throw new EmployeeNotFoundException(companyId);

        var employeeToPatch = mapper.Map<EmployeeForUpdateDto>(employeeEntity);
        return (employeeToPatch, employeeEntity);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatchDto, Employee employeeEntity)
    {
        mapper.Map(employeeToPatchDto, employeeEntity);
        await repository.SaveAsync();
    }
}