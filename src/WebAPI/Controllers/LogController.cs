using System;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{

    [Route("[controller]")]
    public class LogController : Controller
    {
        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation($"Method '{nameof(Get)}' has been called");
            return $"Method '{nameof(Get)}' has been called";
        }

        [HttpPost]
        [Route("exception")]
        public string PostException()
        {
            var message = $"Method '{nameof(PostException)}' has been called";
            _logger.LogError(message);
            throw new Exception(message);
        }

    }

}
