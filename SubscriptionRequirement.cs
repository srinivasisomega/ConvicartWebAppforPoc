using Microsoft.AspNetCore.Authorization;
namespace ConvicartWebApp
{
    public class SubscriptionRequirement : IAuthorizationRequirement
    {
        public string RequiredSubscription { get; }

        public SubscriptionRequirement(string requiredSubscription)
        {
            RequiredSubscription = requiredSubscription;
        }
    }

}
