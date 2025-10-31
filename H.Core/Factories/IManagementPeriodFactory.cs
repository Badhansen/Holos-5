using H.Core.Models.Animals;

namespace H.Core.Factories;

public interface IManagementPeriodFactory
{
    /// <summary>
    /// Creates a new ManagementPeriodDto with default values
    /// </summary>
    IManagementPeriodDto CreateManagementPeriodDto();
    
    /// <summary>
    /// Creates a new ManagementPeriodDto from a template ManagementPeriodDto
    /// </summary>
    IManagementPeriodDto CreateManagementPeriodDto(IManagementPeriodDto template);
    
    /// <summary>
    /// Creates a new ManagementPeriodDto from a domain ManagementPeriod
    /// </summary>
    IManagementPeriodDto CreateManagementPeriodDto(ManagementPeriod managementPeriod);
    
    /// <summary>
    /// Creates a new domain ManagementPeriod from a ManagementPeriodDto
    /// </summary>
    ManagementPeriod CreateManagementPeriod(IManagementPeriodDto dto);
}