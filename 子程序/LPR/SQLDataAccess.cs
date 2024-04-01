using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPR
{
    public class SQLDataAccess
    {
        private static readonly string db = ((AppSettingsSection)ConfigurationManager.OpenExeConfiguration("称重软件3.0.exe").GetSection("appSettings")).Settings["DBSelect"].Value;

        #region CarLabelModel

        public static CarLabelModel LoadCarLabel(string labelNo)
        {
            IDbConnection cnn;

            if (db == "mysql")
                cnn = new MySqlConnection(LoadConnectionString("AWSMYSQL"));
            else
                cnn = new SQLiteConnection(LoadConnectionString());

            var output = cnn.QueryFirstOrDefault<CarLabelModel>("select * from CarLabel where LabelNo = '" + labelNo + "';", new DynamicParameters());

            cnn.Close();

            return output;
        }

        #endregion

        private static string LoadConnectionString(string id = "AWSDB") => ConfigurationManager.ConnectionStrings[id].ConnectionString;
    }
}
