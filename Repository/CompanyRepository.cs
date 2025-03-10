﻿using Contracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class CompanyRepository(RepositoryContext repositoryContext) : RepositoryBase<Company>(repositoryContext), ICompanyRepository
{
    public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)
    {
        return await FindAll(trackChanges).OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Company?> GetCompanyAsync(Guid companyId, bool trackChanges)
    {
        return await FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefaultAsync();
    }

    public void CreateCompany(Company company)
    {
        Create(company);
    }

    public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        return await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync();
    }

    public void DeleteCompany(Company company)
    {
        Delete(company);
    }
}
