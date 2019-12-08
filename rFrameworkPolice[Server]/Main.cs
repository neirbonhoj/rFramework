using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static rFrameworkPolice_Server_.Functions;
using static rConfig.ConfigManager;
namespace rFrameworkPolice_Server_
{
    class Main : BaseScript
    {
        public Main()
        {
            InitConfig();
            //Cache the users table in the database
            string SQLQuery = "SELECT * FROM `users`";
            ExecuteSQLQuery(SQLQuery);

            //Start Discord bot
            StartDiscordBotProcess();
        }
    }
}
