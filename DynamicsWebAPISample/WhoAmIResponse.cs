using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsWebAPISample
{
    public class WhoAmIResponse
    {
        public Guid BusinessUnitId { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
