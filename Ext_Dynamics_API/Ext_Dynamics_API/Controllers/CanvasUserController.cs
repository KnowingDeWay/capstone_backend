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
using Ext_Dynamics_API.Canvas.Enums.Params;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Controllers
{
    [Route("api/CanvasUserController")]
    [ApiController]
    [Authorize]
    public class CanvasUserController : ControllerBase
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly TokenManager _tokenManager;
        private readonly SystemConfig _config;
        private readonly CanvasDataAccess _canvasDataAccess;
        private readonly CanvasTokenManager _canvasTokenManager;

        public CanvasUserController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _tokenManager = new TokenManager(_dbCtx, _config);
            _canvasDataAccess = new CanvasDataAccess(_config);
            _canvasTokenManager = new CanvasTokenManager(_dbCtx);
        }

        [HttpGet]
        [Route("GetCourseStudents/{courseId}")]
        public IActionResult GetCourseStudents([FromRoute] int courseId, [FromQuery] int pageNumber = -1)
        {
            var userToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);

            JwtSecurityToken decodedToken;
            var handler = new JwtSecurityTokenHandler();
            var listResponse = new ListResponse<User>();
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

            try
            {
                listResponse.ListContent = _canvasDataAccess.GetUsersInCourse(canvasPat, courseId, 
                    EnrollmentParamType.student, pageNumber);
                listResponse.ResponseMessage = "Successful retreival of Canvas Students";
                return Ok(listResponse);
            }
            catch(Exception)
            {
                listResponse.ResponseMessage = "Failed to retreive students from Canvas";
                return new BadRequestObjectResult(listResponse);
            }
        }
    }
}
