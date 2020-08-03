using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace API.Controllers
{
    [ApiController]
    public class WarrantyController : ControllerBase
    {
        private readonly ILogger<WarrantyController> _logger;

        public WarrantyController(ILogger<WarrantyController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("warranty/info")]
        public IActionResult Info()
        {
            var info = new
            {
                AppliesTo = "All models",
                MonthOfLifeLessThan = 24,
                MileageLessThan = 100000
            };

            return new JsonResult(info);
        }

        [HttpGet]
        [Route("warranty/status")]
        public IActionResult Status()
        {
            return new JsonResult("In warranty");
        }
    }
}
