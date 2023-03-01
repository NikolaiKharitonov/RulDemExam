using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RulDemka.Controller
{
    
    class Connect
    {
        public static Model.RulDemEntities context;

        public static Model.RulDemEntities GetContext()
        {
            if (context == null)
                context = new Model.RulDemEntities();
            return context;
        }
    }
}
