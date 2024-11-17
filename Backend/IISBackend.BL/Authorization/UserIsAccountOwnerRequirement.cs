using Microsoft.AspNetCore.Authorization;

namespace IISBackend.API.Authorization;

public class UserIsAccountOwnerRequirement : IAuthorizationRequirement { }
