using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Ext_Dynamics_API.Models
{
    public class EntityBase
    {
        public int Id { get; set; }

        [Timestamp]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public byte[] RowVersion { get; set; }
    }
}
