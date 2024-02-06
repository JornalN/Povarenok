using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Povarenok.AppData
{
    internal class ConnectDB
    {
        public static TradeEntities context;

        public static TradeEntities GetCont()
        {
            if(context==null) context = new TradeEntities();
            return context;
        }
    }
}
