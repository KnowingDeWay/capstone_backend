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

        public bool IsScopeUnique(string scope, int canvasUserId)
        {
            return _dbCtx.Scopes.Where(x => x.Name.Equals(scope) && x.CanvasUserId == canvasUserId).FirstOrDefault() != null;
        }

        public List<UserCustomDataEntry> InterpretDataAsEntries(CustomDataResponse dataResponse)
        {
            var custDataEntries = new List<UserCustomDataEntry>();
            foreach(var entry in dataResponse.Data)
            {
                var custEntry = new UserCustomDataEntry
                {

                };
            }
            return custDataEntries;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            ((IDisposable)_canvasDataAccess).Dispose();
        }
    }
}
