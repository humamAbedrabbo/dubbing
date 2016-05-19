using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dubbingModel;

namespace dubbingApp.Models
{
    public class staticCache
    {
        static DUBBDBEntities db = new DUBBDBEntities();

        private static List<dubbDomain> domainValuesList = null;

        public static void loadDomainValuesList()
        {
            domainValuesList = db.dubbDomains.ToList();
        }
        public static List<dubbDomain> getDomainValuesList()
        {
            return domainValuesList;
        }
    }
}