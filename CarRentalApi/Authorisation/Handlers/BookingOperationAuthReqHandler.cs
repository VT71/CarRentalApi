using Microsoft.AspNetCore.Authorization;
using CarRentalApi.Authorisation.Requirements;
using CarRentalApi.Models;
using CarRentalApi.Authorisation;
using System.Security.Claims;

namespace CarRentalApi.Authorisation.Handlers;
public class BookingOperationAuthReqHandler : AuthorizationHandler<OperationAuthorisationRequirement, PaginatedList<Booking>>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
                                                   OperationAuthorisationRequirement requirement,
                                                   PaginatedList<Booking> resource)
    {
        if (requirement.OperationName == Operations.Read.OperationName)
        {
            if (context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.Employee))
            {
                context.Succeed(requirement);
            }
            if (context.User.IsInRole(Roles.Customer))
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId != null && resource.Items.All(b => b.UserId == userId))
                {
                    context.Succeed(requirement);
                }
            }
        }
        return Task.CompletedTask;
    }
}