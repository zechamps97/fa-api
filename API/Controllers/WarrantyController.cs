﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WarrantyController : ControllerBase
    {
        private readonly ILogger<WarrantyController> _logger;

        public WarrantyController(ILogger<WarrantyController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Info()
        {
            return "It's in warranty, trust me!";
        }
    }
}