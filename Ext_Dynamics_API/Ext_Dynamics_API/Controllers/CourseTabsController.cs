using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Models.CustomTabModels;
using Ext_Dynamics_API.RequestModels;
using Ext_Dynamics_API.ResourceManagement;
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
        private readonly CourseDataTableManager _tableManager;

        public CourseTabsController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _tokenManager = new TokenManager(_dbCtx, _config);
            _canvasTokenManager = new CanvasTokenManager(_dbCtx);
            _canvasDataAccess = new CanvasDataAccess(_config);
            _tableManager = new CourseDataTableManager(_dbCtx);
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

        [HttpPut]
        [Route("UpdateCourseTable")]
        public IActionResult UpdateCourseTable([FromBody] dynamic editedTable)
        {
            var userToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);

            JwtSecurityToken decodedToken;
            var handler = new JwtSecurityTokenHandler();
            try
            {
                decodedToken = handler.ReadJwtToken(userToken);
            }
            catch (ArgumentException)
            {
                return new UnauthorizedObjectResult("User Token Is Not Valid");
            }

            var sysUserId = _tokenManager.GetUserIdFromToken(decodedToken);

            var canvasPat = _canvasTokenManager.GetActiveAccessToken(sysUserId);

            if (canvasPat == null)
            {
                return new BadRequestObjectResult("No Canvas PAT Selected/Activated!");
            }

            var table = CourseDataTable.LoadDataTableFromDynamicObject(editedTable);

            _tableManager.Students = table.Students;
            var result = _tableManager.EditTable(table.CourseId, canvasPat, table);
            if(result)
            {
                return Ok("Successfully saved changes to Gradebook");
            }
            else
            {
                return NotFound("Failied to Edit Canvas Table");
            }
        }

        [HttpPost]
        [Route("AddNewTableColumn/{courseId}")]
        public IActionResult AddNewTableColumn([FromRoute] int courseId, [FromBody] NewColumnRequest newColRequest)
        {
            var userToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);

            JwtSecurityToken decodedToken;
            var handler = new JwtSecurityTokenHandler();
            try
            {
                decodedToken = handler.ReadJwtToken(userToken);
            }
            catch (ArgumentException)
            {
                return new UnauthorizedObjectResult("User Token Is Not Valid");
            }

            var sysUserId = _tokenManager.GetUserIdFromToken(decodedToken);

            var canvasPat = _canvasTokenManager.GetActiveAccessToken(sysUserId);

            if (canvasPat == null)
            {
                return new BadRequestObjectResult("No Canvas PAT Selected/Activated!");
            }

            var table = CourseDataTable.LoadDataTable(courseId, canvasPat, _dbCtx);

            var insertionSuccess = _tableManager.AddNewColumn(canvasPat, newColRequest.NewColumn, courseId, sysUserId, 
                table, newColRequest.CsvFileContent);

            if(insertionSuccess)
            {
                return Ok("Successfully added new Column");
            }
            else
            {
                return NotFound("An error occured while adding a new column");
            }
        }
    }
}
