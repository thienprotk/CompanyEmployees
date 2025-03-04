﻿using Contracts;
using Entities;

namespace Repository;

public class CompanyRepository(RepositoryContext repositoryContext) : RepositoryBase<Company>(repositoryContext), ICompanyRepository
{
    public IEnumerable<Company> GetAllCompanies(bool trackChanges)
    {
        return FindAll(trackChanges).OrderBy(c => c.Name).ToList();
    }

    public Company? GetCompany(Guid companyId, bool trackChanges)
    {
        return FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefault();
    }

    public void CreateCompany(Company company)
    {
        Create(company);
    }

    public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
    {
        return FindByCondition(x => ids.Contains(x.Id), trackChanges).ToList();
    }
}
