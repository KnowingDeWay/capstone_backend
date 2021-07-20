using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.RequestModels;
using Ext_Dynamics_API.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Controllers
{
    [Route("api/TokenAuthController")]
    [ApiController]
    public class TokenAuthController : ControllerBase
    {
        private readonly ExtensibleDbContext _dbCtx;
        public TokenAuthController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
        }

        [HttpPost]
        [Route("LoginUser")]
        public AuthResponse LoginUser([FromBody] LoginCredentials credentials)
        {
            return new AuthResponse();
        }

        [Authorize]
        [HttpPost]
        [Route("LogoutUser")]
        public AuthResponse LogoutUser()
        {
            return new AuthResponse();
        }
    }
}
