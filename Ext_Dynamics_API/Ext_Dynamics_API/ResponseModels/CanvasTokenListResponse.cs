using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.ResponseModels
{
    [NotMapped]
    public class ListResponse<T>
    {
        public string ResponseMessage { get; set; }
        public List<T> ListContent { get; set; }
    }
}
