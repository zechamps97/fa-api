using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

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
                MonthOfLifeFrom = new DateTime(2020, 01, 01),
                MonthOfLifeTo = new DateTime(2022, 12, 31),
                MileageUpTo = 100000
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
