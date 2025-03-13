using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Presentation.ModelBinders;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesController(IServiceManager service) : ControllerBase
{
    [HttpOptions]
    public IActionResult GetCompaniesOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");

        return Ok();
    }

    [HttpGet(Name = "GetCompanies")]
    public async Task<IActionResult> GetCompanies()
    {
        var companies = await service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
        return Ok(companies);
    }

    [HttpGet("{id:guid}", Name = "CompanyById")]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var company = await service.CompanyService.GetCompanyAsync(id, trackChanges: false);
        return Ok(company);
    }

    [HttpPost(Name = "CreateCompany")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto companyDto)
    {
        var createdCompany = await service.CompanyService.CreateCompanyAsync(companyDto);
        return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
    }

    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)

    {
        var companies = await service.CompanyService.GetByIdsAsync(ids, trackChanges: false);
        return Ok(companies);
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
    {
        var result = await service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
        return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        await service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
    {
        await service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
        return NoContent();
    }
}