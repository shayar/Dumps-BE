using Dumps.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dumps.API.Controllers
{
    public class RolesController : BaseController
    {
        /// <summary>
        /// Check if the current user is an Admin.
        /// </summary>
        /// <returns>APIResponse indicating whether the user is an Admin.</returns>
        [HttpGet("is-admin")]
        [Authorize]
        [ProducesResponseType(typeof(bool), 200)]
        public IActionResult Index()
        {
            var isAdmin = User.IsInRole(SD.Role_Admin);
            return Ok(new { IsAdmin = isAdmin });
        }
    }
}
