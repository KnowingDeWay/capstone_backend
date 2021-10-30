using Ext_Dynamics_API.Canvas;
using Ext_Dynamics_API.Canvas.ResponseModels;
using Ext_Dynamics_API.Configuration.Models;
using Ext_Dynamics_API.DataAccess;
using Ext_Dynamics_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.ResourceManagement
{
    public class UserCustomDataManager : IDisposable
    {
        private readonly ExtensibleDbContext _dbCtx;
        private readonly CanvasDataAccess _canvasDataAccess;
        private readonly SystemConfig _config;

        public UserCustomDataManager(ExtensibleDbContext dbCtx)
        {
            _dbCtx = dbCtx;
            _config = SystemConfig.LoadConfig();
            _canvasDataAccess = new CanvasDataAccess(_config);
        }

        public bool ScopeExists(string scope, int userId, int courseId)
        {
            return _dbCtx.Scopes.Where(x => x.Name.Equals(scope) && x.UserId == userId).FirstOrDefault() != null;
        }

        public List<UserCustomDataEntry> GetUserCustomDataEntries(string scope, int courseId)
        {
            var custDataEntries = new List<UserCustomDataEntry>();
            custDataEntries = _dbCtx.UserCustomDataEntries.Where(x => x.Scope.Equals(scope) && x.CourseId == courseId).ToList();
            return custDataEntries;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            ((IDisposable)_canvasDataAccess).Dispose();
        }
    }
}
