using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace PriceWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableRateLimiting("UserApi")]
    public class UserController : ControllerBase
    {
        public UserController()
        {
            
        }
    }
}