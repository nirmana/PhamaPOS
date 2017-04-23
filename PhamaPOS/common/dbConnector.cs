using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhamaPOS_Data;

namespace PhamaPOS.common
{
    public class dbConnector
    {
        public PhamaPOSEntities getConn()
        {
           return new PhamaPOSEntities();
        }
       
    }
}
