using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Enums;
using Ext_Dynamics_API.Models;
using Ext_Dynamics_API.ResourceManagement;
using Ext_Dynamics_API.ResponseModels;
using Ext_Dynamics_API.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

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

        public UserCustomDataController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _tokenManager = new TokenManager(_dbCtx, _config);
        }

        [HttpGet]
        [Route("LoadCustomDataForUser/{scope}/{courseId}")]
        public IActionResult LoadCustomDataForUser([FromRoute] int scopeId)
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

            listResponse.ListContent = _dbCtx.UserCustomDataEntries.Where(x => x.ScopeId == scopeId).ToList();
            listResponse.ResponseMessage = "Success";

            return Ok(listResponse);
        }

        [HttpGet]
        [Route("GetCourseScopes/{courseId}")]
        public IActionResult GetCourseScopes([FromRoute] int courseId)
        {
            var userToken = _tokenManager.ReadAndValidateToken(Request.Headers[_config.authHeader]);
            var listResponse = new ListResponse<Scope>();
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

            listResponse.ListContent = _dbCtx.Scopes.Where(x => x.CourseId == courseId).ToList();
            listResponse.ResponseMessage = "Successful retreival of scopes";

            return Ok(listResponse);
        }

        [HttpPut]
        [Route("AddUserCustomDataEntry/{scopeId}")]
        public IActionResult AddUserCustomDataEntry([FromBody] UserCustomDataEntry dataEntry)
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

            var scope = _dbCtx.Scopes.Where(x => x.Id == dataEntry.ScopeId).FirstOrDefault();

            if(scope == null)
            {
                return new BadRequestObjectResult("The scope does not exist!");
            }

            _dbCtx.UserCustomDataEntries.Add(dataEntry);

            try
            {
                _dbCtx.SaveChanges();
            }
            catch(Exception)
            {
                _dbCtx.UserCustomDataEntries.Remove(dataEntry); // Rollback changes
                return new BadRequestObjectResult("Failed to save new data item");
            }

            return Ok($"Data Entry: {dataEntry.ItemName} was added to catergorical scope: {scope.Name}");
        }

        [HttpPut]
        [Route("EditCustomDataEntry/{entryId}/{newName}/{newValue}")]
        public IActionResult EditCustomDataEntry([FromRoute] int entryId, [FromRoute] string newName, [FromRoute] string newValue)
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

            var dataEntry = _dbCtx.UserCustomDataEntries.Where(x => x.Id == entryId).FirstOrDefault();

            if(dataEntry == null)
            {
                return new BadRequestObjectResult("Attempted to edit Invalid Data Entry!");
            }

            if(dataEntry.DataType == CustomDataType.Number)
            {
                try
                {
                    double.Parse(newValue);
                }
                catch(FormatException)
                {
                    return new BadRequestObjectResult("The new value is not a valid number!");
                }
                catch(Exception)
                {
                    return new BadRequestObjectResult("Invalid data for new value!");
                }
            }

            dataEntry.ItemName = newName;
            dataEntry.Content = newValue;

            try
            {
                _dbCtx.SaveChanges();
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Unable to delete Custom Data Item");
            }

            return Ok($"Successfully edited Custom Data Item: {newName}");
        }

        [HttpDelete]
        [Route("DeleteCustomDataEntry/{entryId}")]
        public IActionResult DeleteCustomDataEntry([FromRoute] int entryId)
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

            var dataEntry = _dbCtx.UserCustomDataEntries.Where(x => x.Id == entryId).FirstOrDefault();

            if (dataEntry == null)
            {
                return new BadRequestObjectResult("Attempted to delete Invalid Data Entry!");
            }

            try
            {
                _dbCtx.SaveChanges();
            }
            catch (Exception) 
            {
                return new BadRequestObjectResult("Unable to delete Custom Data Item");
            }

            return Ok($"Successfully edited Custom Data Item: {dataEntry.ItemName}");
        }

        [HttpPost]
        [Route("AddScope/{scopeName}/{courseId}")]
        public IActionResult AddScope([FromRoute] string scopeName, [FromRoute] int courseId)
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

            var newScope = new Scope
            {
                Name = scopeName,
                CourseId = courseId,
                UserId = sysUserId
            };

            _dbCtx.Scopes.Add(newScope);

            try
            {
                _dbCtx.SaveChanges();
            }
            catch (Exception)
            {
                _dbCtx.Scopes.Remove(newScope); // Rollback changes
                return new BadRequestObjectResult("Failed to save new data item");
            }

            return Ok($"Added new Scope: {newScope.Name}");
        }

        [HttpPut]
        [Route("EditScope/{scopeId}{newName}")]
        public IActionResult EditScope([FromRoute] int scopeId, [FromRoute] string newName)
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

            var scope = _dbCtx.Scopes.Where(x => x.Id == scopeId).FirstOrDefault();

            if(scope == null)
            {
                return new BadRequestObjectResult("Cannot edit non-existent scope!");
            }

            scope.Name = newName;
            scope.UserId = sysUserId;

            try
            {
                _dbCtx.SaveChanges();
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Failed to update scope");
            }

            return Ok($"Successfully edited scope: {newName}");
        }

        [HttpDelete]
        [Route("DeleteScope/{scope}")]
        public IActionResult DeleteScope([FromRoute] int scopeId)
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

            var scope = _dbCtx.Scopes.Where(x => x.Id == scopeId).FirstOrDefault();

            if (scope == null)
            {
                return new BadRequestObjectResult("Cannot delete non-existent scope!");
            }

            _dbCtx.Scopes.Remove(scope);

            try
            {
                _dbCtx.SaveChanges();
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Failed to delete scope");
            }

            return Ok($"Successfully deleted scope: {scope.Name}");
        }
    }
}
