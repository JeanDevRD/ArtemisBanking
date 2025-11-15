using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingWebApi.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {

    }
}
