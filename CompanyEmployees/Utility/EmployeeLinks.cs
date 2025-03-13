using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Utility;

public class EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDto> dataShaper) : IEmployeeLinks
{
    public Dictionary<string, MediaTypeHeaderValue> AcceptHeader { get; set; } =
        new Dictionary<string, MediaTypeHeaderValue>();

    public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeesDto, string? fields, Guid companyId, HttpContext httpContext)
    {
        var shapedEmployees = ShapeData(employeesDto, fields);

        if (ShouldGenerateLinks(httpContext))
            return ReturnLinkedEmployees(employeesDto, fields, companyId, httpContext, shapedEmployees);

        return ReturnShapedEmployees(shapedEmployees);
    }

    private List<Entity> ShapeData(IEnumerable<EmployeeDto> employeesDto, string? fields)
    {
        return dataShaper.ShapeData(employeesDto, fields).Select(e => e.Entity).ToList();
    }

    private bool ShouldGenerateLinks(HttpContext httpContext)
    {
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"]!;

        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private LinkResponse ReturnShapedEmployees(List<Entity> shapedEmployees)
    {
        return new LinkResponse { ShapedEntities = shapedEmployees };
    }

    private LinkResponse ReturnLinkedEmployees(IEnumerable<EmployeeDto> employeesDto, string? fields, Guid companyId, HttpContext httpContext, List<Entity> shapedEmployees)
    {
        var EmployeeDtoList = employeesDto.ToList();

        for (var index = 0; index < EmployeeDtoList.Count(); index++)
        {
            var employeeLinks = CreateLinksForEmployee(httpContext, companyId, EmployeeDtoList[index].Id, fields);
            shapedEmployees[index].Add("Links", employeeLinks);
        }

        var employeeCollection = new LinkCollectionWrapper<Entity>(shapedEmployees);
        var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);

        return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployees };
    }

    private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId, Guid id, string fields = "")
    {
        var links = new List<Link>
            {
                new(linkGenerator.GetUriByAction(httpContext, "GetEmployeeForCompany", values: new { companyId, id, fields })!,
                "self",
                "GET"),
                new(linkGenerator.GetUriByAction(httpContext, "DeleteEmployeeForCompany", values: new { companyId, id })!,
                "delete_employee",
                "DELETE"),
                new(linkGenerator.GetUriByAction(httpContext, "UpdateEmployeeForCompany", values: new { companyId, id })!,
                "update_employee",
                "PUT"),
                new(linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateEmployeeForCompany", values: new { companyId, id })!,
                "partially_update_employee",
                "PATCH")
            };
        return links;
    }

    private LinkCollectionWrapper<Entity> CreateLinksForEmployees(HttpContext httpContext,
        LinkCollectionWrapper<Entity> employeesWrapper)
    {
        employeesWrapper.Links.Add(new Link(linkGenerator.GetUriByAction(httpContext, "GetEmployeesForCompany", values: new { })!,
                "self",
                "GET"));

        return employeesWrapper;
    }
}