using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.ResponseModels
{
    [NotMapped]
    public class ObjectResponse
    {
        public string Message { get; set; }
        public object Value { get; set; }
    }
}
