using Benkyou.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Benkyou.Infrastructure.Attributes;

public class RoleAttribute : TypeFilterAttribute
{
    public RoleAttribute(string roleName) : base(typeof(RoleRequirementFilter))
    {
        Arguments = new object[] { roleName };
    }
}