using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Models;
using Ext_Dynamics_API.ResponseModels;
using Ext_Dynamics_API.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Controllers
{
    [Route("api/UserCustomDataController")]
    [ApiController]
    [Authorize]
    public class UserCustomDataController : ControllerBase
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly TokenManager _tokenManager;
        private readonly SystemConfig _config;
        private readonly CanvasTokenManager _canvasTokenManager;
        private readonly CanvasDataAccess _canvasDataAccess;

        public UserCustomDataController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _tokenManager = new TokenManager(_dbCtx, _config);
            _canvasTokenManager = new CanvasTokenManager(_dbCtx);
            _canvasDataAccess = new CanvasDataAccess(_config);
        }

        [HttpGet]
        [Route("LoadCustomDataForUser/{canvasUserId}/{scope}")]
        public IActionResult LoadCustomDataForUser([FromRoute] int canvasUserId, [FromRoute] string scope)
        {
            var userToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);
            var listResponse = new ListResponse<UserCustomDataEntry>();
            JwtSecurityToken decodedToken;
            var handler = new JwtSecurityTokenHandler();

            try
            {
                decodedToken = handler.ReadJwtToken(userToken);
            }
            catch (ArgumentException)
            {
                listResponse.ResponseMessage = "User Token Is Not Valid";
                return new UnauthorizedObjectResult(listResponse);
            }

            var sysUserId = _tokenManager.GetUserIdFromToken(decodedToken);

            var canvasPat = _canvasTokenManager.GetActiveAccessToken(sysUserId);

            if (canvasPat == null)
            {
                listResponse.ResponseMessage = "No Canvas PAT Selected/Activated!";
                return new BadRequestObjectResult(listResponse);
            }

            return Ok(listResponse);
        }

        [HttpPut]
        [Route("StoreUserCustomData/{canvasUserId}/{scope}")]
        public IActionResult StoreUserCustomData([FromRoute] int canvasUserId, [FromRoute] string scope, 
            [FromBody] Dictionary<string, object> data)
        {
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteScope/{scope}")]
        public IActionResult DeleteScope([FromRoute] string scope)
        {
            return Ok();
        }
    }
}
