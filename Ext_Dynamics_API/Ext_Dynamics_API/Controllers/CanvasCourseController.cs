using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Canvas.Models;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
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
    [Route("api/CanvasCourseController")]
    [ApiController]
    [Authorize]
    public class CanvasCourseController : ControllerBase
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly TokenManager _tokenManager;
        private readonly SystemConfig _config;
        private readonly CanvasTokenManager _canvasTokenManager;
        private readonly CanvasDataAccess _canvasDataAccess;

        public CanvasCourseController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _tokenManager = new TokenManager(_dbCtx, _config);
            _canvasTokenManager = new CanvasTokenManager(_dbCtx);
            _canvasDataAccess = new CanvasDataAccess(_config);
        }

        [HttpGet]
        [Route("GetInstructorCourses")]
        public IActionResult GetInstructorCourses()
        {
            var userToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);

            JwtSecurityToken decodedToken;
            var handler = new JwtSecurityTokenHandler();
            var objResponse = new ListResponse<Course>();
            try
            {
                decodedToken = handler.ReadJwtToken(userToken);
            }
            catch (ArgumentException)
            {
                objResponse.ResponseMessage = "User Token Is Not Valid";
                return new UnauthorizedObjectResult(objResponse);
            }

            var sysUserId = _tokenManager.GetUserIdFromToken(decodedToken);

            var canvasPat = _canvasTokenManager.GetActiveAccessToken(sysUserId);

            if(canvasPat == null)
            {
                objResponse.ResponseMessage = "No Canvas PAT Selected/Activated!";
                return new BadRequestObjectResult(objResponse);
            }

            try
            {
                objResponse.ListContent = _canvasDataAccess.GetInstructorCourses(canvasPat);
            }
            catch(Exception)
            {
                objResponse.ResponseMessage = "";
                return new NotFoundObjectResult(objResponse);
            }
            return new OkObjectResult(objResponse);
        }
    }
}
