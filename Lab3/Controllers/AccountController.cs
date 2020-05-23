using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers
{

    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {

        /// <summary>
        /// https://localhost:5001/Account/Login
        /// Redirect to Google Authentication page if user is not authorized.
        /// </summary>
        public IActionResult Login(string returnUrl)
        {
            return new ChallengeResult(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(LoginCallback), new { returnUrl })
                });
        }

        /// <summary>
        /// Redirect back after Google Authorization.
        /// </summary>
        public async Task<IActionResult> LoginCallback(string returnUrl)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("External");

            if (!authenticateResult.Succeeded)
                return BadRequest();

            var claimsIdentity = new ClaimsIdentity("Application");

            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier));
            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.GivenName));
            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Surname));
            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Email));
            claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Name));

            await HttpContext.SignInAsync(
                "Application",
                new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect(returnUrl);
        }
    }
}