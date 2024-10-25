using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
namespace ConvicartWebApp
{
    public class SubscriptionHandler : AuthorizationHandler<SubscriptionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SubscriptionRequirement requirement)
        {
            // Check if user has the claim for the subscription type
            if (context.User.HasClaim(c => c.Type == "Subscription"))
            {
                var userSubscription = context.User.FindFirst(c => c.Type == "Subscription")?.Value;

                // Check if the user's subscription meets or exceeds the required level
                if (IsAuthorized(userSubscription, requirement.RequiredSubscription))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }

        private bool IsAuthorized(string? userSubscription, string requiredSubscription)
        {
            // Define hierarchy: Bronze < Silver < Gold
            var levels = new List<string> { "Bronze", "Silver", "Gold" };

            var userLevel = levels.IndexOf(userSubscription ?? "");
            var requiredLevel = levels.IndexOf(requiredSubscription);

            return userLevel >= requiredLevel;
        }
    }

}
