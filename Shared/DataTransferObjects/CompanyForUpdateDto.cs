﻿namespace Shared.DataTransferObjects;

public record CompanyForUpdateDto : CompanyForManipulationDto
{
    public IEnumerable<EmployeeForUpdateDto>? Employees { get; init; }
}
