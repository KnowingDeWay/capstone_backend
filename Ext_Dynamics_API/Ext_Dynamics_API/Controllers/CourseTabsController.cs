using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Models.CustomTabModels;
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
    [Route("api/CourseTabsController")]
    [ApiController]
    [Authorize]
    public class CourseTabsController : ControllerBase
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly TokenManager _tokenManager;
        private readonly SystemConfig _config;
        private readonly CanvasTokenManager _canvasTokenManager;
        private readonly CanvasDataAccess _canvasDataAccess;

        public CourseTabsController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _tokenManager = new TokenManager(_dbCtx, _config);
            _canvasTokenManager = new CanvasTokenManager(_dbCtx);
            _canvasDataAccess = new CanvasDataAccess(_config);
        }

        [HttpGet]
        [Route("GetCourseDataTable/{courseId}")]
        public IActionResult GetCourseDataTable([FromRoute] int courseId)
        {
            var userToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);

            JwtSecurityToken decodedToken;
            var handler = new JwtSecurityTokenHandler();
            var objResponse = new ObjectResponse<CourseDataTable>();
            try
            {
                decodedToken = handler.ReadJwtToken(userToken);
            }
            catch (ArgumentException)
            {
                objResponse.Message = "User Token Is Not Valid";
                return new UnauthorizedObjectResult(objResponse);
            }

            var sysUserId = _tokenManager.GetUserIdFromToken(decodedToken);

            var canvasPat = _canvasTokenManager.GetActiveAccessToken(sysUserId);

            if (canvasPat == null)
            {
                objResponse.Message = "No Canvas PAT Selected/Activated!";
                return new BadRequestObjectResult(objResponse);
            }

            var dataTable = CourseDataTable.LoadDataTable(courseId, canvasPat, _dbCtx);
            objResponse.Value = dataTable;
            objResponse.Message = "Successful retrevial of data table";
            
            return Ok(objResponse);
        }
    }
}
