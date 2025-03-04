﻿using Contracts;
using Entities;

namespace Repository;

public class EmployeeRepository(RepositoryContext repositoryContext) : RepositoryBase<Employee>(repositoryContext), IEmployeeRepository
{
    public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges)
    {
        return FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges).OrderBy(e => e.Name).ToList();
    }

    public Employee? GetEmployee(Guid companyId, Guid id, bool trackChanges)
    {
        return FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges).SingleOrDefault();
    }

    public void CreateEmployeeForCompany(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }
}
