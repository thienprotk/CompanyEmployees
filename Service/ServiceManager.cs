﻿using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.Contracts;

namespace Service;

public sealed class ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper, IEmployeeLinks employeeLinks, UserManager<User> userManager, IConfiguration configuration) : IServiceManager
{
    private readonly Lazy<ICompanyService> _companyService = new(() => new CompanyService(repositoryManager, logger, mapper));
    private readonly Lazy<IEmployeeService> _employeeService = new(() => new EmployeeService(repositoryManager, logger, mapper, employeeLinks));
    private readonly Lazy<IAuthenticationService> _authenticationService = new(() => new AuthenticationService(userManager, logger, mapper, configuration));

    public ICompanyService CompanyService => _companyService.Value;
    public IEmployeeService EmployeeService => _employeeService.Value;
    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}
