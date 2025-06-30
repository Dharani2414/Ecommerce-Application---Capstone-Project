using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class DBConnUtil
    {
        
            private static SqlConnection connection;

            public static SqlConnection GetConnection()
            {
                if (connection == null)
                {
                    string connString = DBPropertyUtil.GetPropertyString();
                    connection = new SqlConnection(connString);
                }
                return connection;
            }
        }
    }



