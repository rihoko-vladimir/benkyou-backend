using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sets.Api.Controllers;


[ApiController]
[Route("/api/v{version:apiVersion}/sets")]
[Authorize]
public class SetsController : ControllerBase
{
    
}