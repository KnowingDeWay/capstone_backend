using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Security;
using Microsoft.AspNetCore.Mvc;
using Scrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Controllers
{
    public class UserManagementController : ControllerBase
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly TokenManager _tokenManager;
        private readonly ScryptEncoder _scryptHasher; // Using the SCRYPT hashing algorithm 
        private readonly SystemConfig _config;

        public UserManagementController(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _tokenManager = new TokenManager(_dbCtx, _config);
            _scryptHasher = new ScryptEncoder();
        }


    }
}
