using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Microsoft.EntityFrameworkCore;
using Ext_Dynamics_API.Models;
using Scrypt;
using System;
using Xunit;
using Ext_Dynamics_API.Controllers;
using Ext_Dynamics_API.Enums;
using Ext_Dynamics_API.RequestModels;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Ext_Dynamics_API.ResponseModels;
using Moq;
using Microsoft.AspNetCore.Http;

namespace Ext_Dynamics_API_Tests
{
    public class AuthControllerTests : IDisposable
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly SystemConfig _config;
        private readonly ScryptEncoder _encoder;

        public AuthControllerTests()
        {
            _config = SystemConfig.LoadConfig();
            DbContextOptionsBuilder<ExtensibleDbContext> optionsBuilder = new();
            optionsBuilder.UseSqlServer(_config.testDbConnString);
            _dbCtx = new ExtensibleDbContext(optionsBuilder.Options);
            _encoder = new ScryptEncoder();
            SetupContext();
        }

        private void SetupContext()
        {
            var testUser = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Fraser")).FirstOrDefault();
            if(testUser == null)
            {
                _dbCtx.UserAccounts.Add(new ApplicationUserAccount()
                {
                    AppUserName = "Fraser",
                    UserPassword = _encoder.Encode("password"),
                    UserType = UserType.Instructor
                });
                _dbCtx.SaveChanges();
            }
        }

        [Fact]
        public void Login_User_Valid_Creds()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Fraser",
                Password = "password"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var correctOutcome = _dbCtx.UserTokenEntries.Where(x => x.EncodedToken.Equals(token)).FirstOrDefault() != null;

            Assert.True(correctOutcome);
        }

        [Fact]
        public void Login_User_Invalid_Creds()
        {
            var controller = new AuthController(_dbCtx);
            var creds = new LoginCredentials()
            {
                Username = "Fraser",
                Password = "pass"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var correctOutcome = string.IsNullOrWhiteSpace(token);

            Assert.True(correctOutcome);
        }

        [Fact]
        public void Logout_User_Valid_Creds()
        {
            var controller = new AuthController(_dbCtx);

            var creds = new LoginCredentials()
            {
                Username = "Fraser",
                Password = "password"
            };
            var result = controller.LoginUser(creds) as ObjectResult;

            var token = ((AuthResponse)result.Value).ResponseToken;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers[_config.authHeader] = $"Bearer {token}";

            var controllerContext = new ControllerContext() { HttpContext = httpContext };

            controller.ControllerContext = controllerContext;

            controller.LogoutUser();

            var correctOutcome = _dbCtx.UserTokenEntries.Where(x => x.EncodedToken.Equals(token)).FirstOrDefault() == null;

            Assert.True(correctOutcome);
        }

        public void Dispose()
        {
            var user = _dbCtx.UserAccounts.Where(x => x.AppUserName.Equals("Fraser")).FirstOrDefault();
            if(user != null)
            {
                var tokenEntries = _dbCtx.UserTokenEntries.Where(x => x.AppUserId == user.Id).ToList();
                _dbCtx.UserTokenEntries.RemoveRange(tokenEntries);
                _dbCtx.UserAccounts.Remove(user);
                _dbCtx.SaveChanges();
            }
        }
    }
}
