using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeckOfCards.WebApi.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1")]
    [Obsolete("Future Home of Health Endpoint")]
    public class HealthController
    {
        public HealthController()
        {

        }

        [HttpGet]
        public IActionResult Get()
        {
            return new OkObjectResult(new { Value1 = "Health" });
        }
    }
}
