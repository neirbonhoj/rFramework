using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rPlayerManager_Client_
{
    public static class Main
    {
        //Key: Role Discord ID | Value: Rank (Default is 1)
        public static Dictionary<ulong, int> Permissions = new Dictionary<ulong, int>();
    }
}
