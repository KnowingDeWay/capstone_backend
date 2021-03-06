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
    [Route("api/CanvasAssignmentsController")]
    [ApiController]
    [Authorize]
    public class CanvasAssignmentsController : ControllerBase
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly TokenManager _tokenManager;
        private readonly SystemConfig _config;
        private readonly CanvasTokenManager _canvasTokenManager;
        private readonly CanvasDataAccess _canvasDataAccess;

        public CanvasAssignmentsController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _tokenManager = new TokenManager(_dbCtx, _config);
            _canvasTokenManager = new CanvasTokenManager(_dbCtx);
            _canvasDataAccess = new CanvasDataAccess(_config);
        }

        /// <summary>
        /// Gets all the assignments pertaining to the course
        /// </summary>
        /// <param name="courseId">The id of the course</param>
        /// <returns>
        /// ListResponse&lt;Assignment>: The list of assignments pertaining to the course and the result of this 
        /// operation
        /// </returns>
        [HttpGet]
        [Route("GetCourseAssignments/{courseId}")]
        public IActionResult GetCourseAssignments([FromRoute] int courseId)
        {
            var userToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);

            JwtSecurityToken decodedToken;
            var handler = new JwtSecurityTokenHandler();
            var objResponse = new ListResponse<Assignment>();
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

            if (canvasPat == null)
            {
                objResponse.ResponseMessage = "No Canvas PAT Selected/Activated!";
                return new BadRequestObjectResult(objResponse);
            }

            try
            {
                objResponse.ListContent = _canvasDataAccess.GetCourseAssignments(canvasPat, courseId);
            }
            catch(Exception)
            {
                objResponse.ResponseMessage = "Error occured during retrevial of canvas courses";
                return new NotFoundObjectResult(objResponse);
            }

            return new OkObjectResult(objResponse);
        }

    }
}
