using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class DefaultController : ControllerBase
    {
        /// <summary>
        /// https://localhost:5001/api/Test
        /// </summary>
        /// <returns> 200 - Status OK</returns>
        [Route("api/Test")]
        [Authorize]
        public IActionResult Get() => Ok($"Status OK");
    }
}