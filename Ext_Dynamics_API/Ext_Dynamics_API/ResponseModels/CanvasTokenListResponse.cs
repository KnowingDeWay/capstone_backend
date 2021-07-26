using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.ResponseModels
{
    public class ListResponse<T>
    {
        public string ResponseMessage { get; set; }
        public List<T> ListContent { get; set; }
    }
}
