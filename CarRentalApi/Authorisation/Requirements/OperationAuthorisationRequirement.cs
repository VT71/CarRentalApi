using Azure;
using Microsoft.AspNetCore.Authorization;

namespace CarRentalApi.Authorisation.Requirements;

public class OperationAuthorisationRequirement: IAuthorizationRequirement
{
    public string OperationName { get; }

    public OperationAuthorisationRequirement(string operationName)
    {
        OperationName = operationName;
    }
}
public static class Operations
{
    public static OperationAuthorisationRequirement Create { get; } = new("Create");
    public static OperationAuthorisationRequirement Read { get; } = new("Read");
    public static OperationAuthorisationRequirement Update { get; } = new("Update");
    public static OperationAuthorisationRequirement Delete { get; } = new("Delete");
}