using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.DTO
{
    public class CompanyDbConfig
    {
        public int CompanyId { get; set; }

        public string ConnectionString { get; set; }

        public string TenantName { get; set; }
    }
}
