using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.Controllers;
[Authorize]
public class AuthorizeController : ControllerBase
{
    protected Guid? GetAuthId()
    {
        var userIdStr = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        if (userIdStr == null) return null;
        return Guid.Parse(userIdStr);
    }
    protected string? GetAuthName()
    {
        return User.Identity?.Name;
    }
}