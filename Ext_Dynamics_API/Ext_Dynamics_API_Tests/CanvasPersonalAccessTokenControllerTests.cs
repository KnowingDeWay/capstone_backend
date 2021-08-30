using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.Controllers;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Enums;
using Ext_Dynamics_API.Models;
using Ext_Dynamics_API.RequestModels;
using Ext_Dynamics_API.ResponseModels;
using Ext_Dynamics_API_Tests.HelperModules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Ext_Dynamics_API_Tests
{
    public class CanvasPersonalAccessTokenControllerTests : IDisposable
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly SystemConfig _config;
        private readonly ScryptEncoder _encoder;
        private readonly List<ApplicationUserAccount> _accounts;
        private readonly CanvasTokenHelper _canvasTokenHelper;

        public CanvasPersonalAccessTokenControllerTests()
        {
            _config = SystemConfig.LoadConfig();
            DbContextOptionsBuilder<ExtensibleDbContext> optionsBuilder = new();
            optionsBuilder.UseSqlServer(_config.testDbConnString);
            _dbCtx = new ExtensibleDbContext(optionsBuilder.Options);
            _encoder = new ScryptEncoder();
            _canvasTokenHelper = new CanvasTokenHelper();
            _accounts = new List<ApplicationUserAccount>();
            SetupContext();
        }

        private void SetupContext()
        {
            var userList = new List<ApplicationUserAccount>() 
            {
                new ApplicationUserAccount
                {
                    AppUserName = "Literal Snacc",
                    UserPassword = _encoder.Encode("snacx4life"),
                    UserType = UserType.Instructor
                },
                new ApplicationUserAccount
                {
                    AppUserName = "Queen Nolen",
                    UserPassword = _encoder.Encode("malay_ice_queen"),
                    UserType = UserType.Instructor
                },
                new ApplicationUserAccount
                {
                    AppUserName = "Cupcake Mistress",
                    UserPassword = _encoder.Encode("cupcakes24/7/365"),
                    UserType = UserType.Instructor
                },
                new ApplicationUserAccount
                {
                    AppUserName = "Alaya",
                    UserPassword = _encoder.Encode("mecha_gamer"),
                    UserType = UserType.Instructor
                }
            }; 
            _dbCtx.UserAccounts.AddRange(userList);
            _dbCtx.SaveChanges();

            _accounts.AddRange(userList);

            var firstUser = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Literal Snacc")).FirstOrDefault();

            var firstUserTokens = new List<CanvasPersonalAccessToken>()
            {
                new CanvasPersonalAccessToken
                {
                    TokenName = "FirstToken",
                    AccessToken = _canvasTokenHelper.CanvasTokenGenerator(),
                    AppUserId = firstUser.Id
                },
                new CanvasPersonalAccessToken
                {
                    TokenName = "SnaccToken",
                    AccessToken = _canvasTokenHelper.CanvasTokenGenerator(),
                    AppUserId = firstUser.Id
                },
                new CanvasPersonalAccessToken
                {
                    TokenName = "ThirdToken",
                    AccessToken = _canvasTokenHelper.CanvasTokenGenerator(),
                    AppUserId = firstUser.Id
                }
            };

            _dbCtx.PersonalAccessTokens.AddRange(firstUserTokens);

            var secondUser = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Queen Nolen")).FirstOrDefault();

            var secondUserTokens = new List<CanvasPersonalAccessToken>()
            {
                new CanvasPersonalAccessToken
                {
                    TokenName = "Nolen1",
                    AccessToken = _canvasTokenHelper.CanvasTokenGenerator(),
                    AppUserId = secondUser.Id
                },
                new CanvasPersonalAccessToken
                {
                    TokenName = "Nolen2",
                    AccessToken = _canvasTokenHelper.CanvasTokenGenerator(),
                    AppUserId = secondUser.Id
                },
                new CanvasPersonalAccessToken
                {
                    TokenName = "Nolen3",
                    AccessToken = _canvasTokenHelper.CanvasTokenGenerator(),
                    AppUserId = secondUser.Id
                }
            };

            _dbCtx.PersonalAccessTokens.AddRange(secondUserTokens);

            var thirdUser = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Cupcake Mistress")).FirstOrDefault();

            var thirdUserTokens = new List<CanvasPersonalAccessToken>()
            {
                new CanvasPersonalAccessToken
                {
                    TokenName = "Vanilla Cupcake",
                    AccessToken = _canvasTokenHelper.CanvasTokenGenerator(),
                    AppUserId = thirdUser.Id
                },
                new CanvasPersonalAccessToken
                {
                    TokenName = "Blueberry Cupcake",
                    AccessToken = _canvasTokenHelper.CanvasTokenGenerator(),
                    AppUserId = thirdUser.Id
                },
                new CanvasPersonalAccessToken
                {
                    TokenName = "Choc Chip Cupcake",
                    AccessToken = _canvasTokenHelper.CanvasTokenGenerator(),
                    AppUserId = thirdUser.Id
                }
            };

            _dbCtx.PersonalAccessTokens.AddRange(thirdUserTokens);

            _dbCtx.SaveChanges();
        }

        [Fact]
        public void Get_Access_Tokens_Valid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Literal Snacc",
                Password = "snacx4life"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            var response = canvasController.GetUserAccessTokens();

            var objResult = response as ObjectResult;

            var objValue = (ListResponse<CanvasPersonalAccessToken>)objResult.Value;

            var outcome = objValue.ListContent.Count == 3;

            Assert.True(outcome);
        }

        [Fact]
        public void Get_Access_Tokens_Invalid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Literal Snacc",
                Password = "snacx"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            var response = canvasController.GetUserAccessTokens();

            var objResult = response as ObjectResult;

            var objValue = (ListResponse<CanvasPersonalAccessToken>)objResult.Value;

            var outcome = objValue.ListContent == null;

            Assert.True(outcome);
        }

        [Fact]
        public void Get_Access_Token_Valid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Literal Snacc",
                Password = "snacx4life"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            var tokenToGet = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("FirstToken")).FirstOrDefault();

            var response = canvasController.GetUserAccessTokenById(tokenToGet.Id);

            var objResult = response as ObjectResult;

            var objValue = (ObjectResponse<CanvasPersonalAccessToken>)objResult.Value;

            var tokenRetreived = objValue.Value;

            var outcome = tokenRetreived.TokenName.Equals("FirstToken");

            Assert.True(outcome);
        }

        [Fact]
        public void Get_Access_Token_Invalid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Literal Snacc",
                Password = "snacx"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            var tokenToGet = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("FirstToken")).FirstOrDefault();

            var response = canvasController.GetUserAccessTokenById(tokenToGet.Id);

            var objResult = response as ObjectResult;

            var objValue = (ObjectResponse<CanvasPersonalAccessToken>)objResult.Value;

            var tokenRetreived = objValue.Value;

            var outcome = tokenRetreived == null;

            Assert.True(outcome);
        }

        [Fact]
        public void Get_Access_Token_Valid_But_Wrong_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Queen Nolen",
                Password = "malay_ice_queen"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            var tokenToGet = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("FirstToken")).FirstOrDefault();

            var response = canvasController.GetUserAccessTokenById(tokenToGet.Id);

            var objResult = response as ObjectResult;

            var objValue = (ObjectResponse<CanvasPersonalAccessToken>)objResult.Value;

            var tokenRetreived = objValue.Value;

            var outcome = tokenRetreived == null;

            Assert.True(outcome);
        }

        [Fact]
        public void Add_Access_Token_Valid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Cupcake Mistress",
                Password = "cupcakes24/7/365"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            canvasController.AddCanvasToken(new CanvasToken()
            {
                TokenName = "NewToken",
                ApiKey = _canvasTokenHelper.CanvasTokenGenerator()
            });

            var tokenAdded = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("NewToken")).FirstOrDefault();

            var outcome = tokenAdded != null;

            Assert.True(outcome);
        }

        [Fact]
        public void Add_Access_Token_Invalid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Cupcake Mistress",
                Password = "cupcakes"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            canvasController.AddCanvasToken(new CanvasToken()
            {
                TokenName = "NewToken2",
                ApiKey = _canvasTokenHelper.CanvasTokenGenerator()
            });

            var tokenAdded = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("NewToken2")).FirstOrDefault();

            var outcome = tokenAdded == null;

            Assert.True(outcome);
        }

        [Fact]
        public void Edit_Access_Token_Valid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Queen Nolen",
                Password = "malay_ice_queen"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            var tokenToUpdate = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("Nolen1")).FirstOrDefault();

            var updatedInfo = new CanvasPersonalAccessToken()
            {
                Id = tokenToUpdate.Id,
                TokenName = tokenToUpdate.TokenName,
                AccessToken = tokenToUpdate.AccessToken,
                AppUserId = tokenToUpdate.AppUserId,
                RowVersion = tokenToUpdate.RowVersion
            };

            updatedInfo.TokenName = "Nolen11";

            canvasController.EditAccessToken(updatedInfo);

            var updatedToken = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("Nolen11")).FirstOrDefault();

            var outcome = updatedToken != null;

            Assert.True(outcome);
        }

        [Fact]
        public void Edit_Access_Token_Wrong_But_Valid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Alaya",
                Password = "mecha_gamer"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            var tokenToUpdate = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("Nolen2")).FirstOrDefault();

            var updatedInfo = new CanvasPersonalAccessToken()
            {
                Id = tokenToUpdate.Id,
                TokenName = tokenToUpdate.TokenName,
                AccessToken = tokenToUpdate.AccessToken,
                AppUserId = tokenToUpdate.AppUserId,
                RowVersion = tokenToUpdate.RowVersion
            };

            updatedInfo.TokenName = "Nolen21";

            canvasController.EditAccessToken(updatedInfo);

            var updatedToken = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("Nolen21")).FirstOrDefault();

            var outcome = updatedToken == null;

            Assert.True(outcome);
        }

        [Fact]
        public void Edit_Access_Token_Invalid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Queen Nolen",
                Password = "ice_queen"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            var tokenToUpdate = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("Nolen1")).FirstOrDefault();

            var updatedInfo = new CanvasPersonalAccessToken()
            {
                Id = tokenToUpdate.Id,
                TokenName = tokenToUpdate.TokenName,
                AccessToken = tokenToUpdate.AccessToken,
                AppUserId = tokenToUpdate.AppUserId,
                RowVersion = tokenToUpdate.RowVersion
            };

            updatedInfo.TokenName = "Nolen31";

            canvasController.EditAccessToken(updatedInfo);

            var updatedToken = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("Nolen31")).FirstOrDefault();

            var outcome = updatedToken == null;

            Assert.True(outcome);
        }

        [Fact]
        public void Delete_Access_Token_Valid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Cupcake Mistress",
                Password = "cupcakes24/7/365"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            var patToDelete = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("Vanilla Cupcake")).FirstOrDefault();

            canvasController.DeleteAccessToken(patToDelete.Id);

            var outcome = _dbCtx.PersonalAccessTokens.Where(x => x.Id == patToDelete.Id).FirstOrDefault() == null;

            Assert.True(outcome);
        }

        [Fact]
        public void Delete_Access_Token_Wrong_User_But_Valid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Queen Nolen",
                Password = "ice_queen"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            var patToDelete = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("Blueberry Cupcake")).FirstOrDefault();

            canvasController.DeleteAccessToken(patToDelete.Id);

            var outcome = _dbCtx.PersonalAccessTokens.Where(x => x.Id == patToDelete.Id).FirstOrDefault() != null;

            Assert.True(outcome);
        }

        [Fact]
        public void Delete_Access_Token_Invalid_User()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Cupcake Mistress",
                Password = "cupcakes"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            var canvasController = new CanvasAccessTokenController(_dbCtx);

            canvasController.ControllerContext = controllerContext;

            var patToDelete = _dbCtx.PersonalAccessTokens.Where(x => x.TokenName.Equals("Choc Chip Cupcake")).FirstOrDefault();

            canvasController.DeleteAccessToken(patToDelete.Id);

            var outcome = _dbCtx.PersonalAccessTokens.Where(x => x.Id == patToDelete.Id).FirstOrDefault() != null;

            Assert.True(outcome);
        }

        public void Dispose()
        {
            foreach(var user in _accounts)
            {
                var currUser = _dbCtx.UserAccounts.Where(x => x.Id == user.Id).FirstOrDefault();
                if(currUser != null)
                {
                    _dbCtx.UserAccounts.Remove(currUser);
                }
            }
            _dbCtx.SaveChanges();
        }
    }
}
