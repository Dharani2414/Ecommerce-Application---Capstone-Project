using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class DBPropertyUtil
    {
            public static string GetPropertyString()
            {
                return ConfigurationManager.ConnectionStrings["MyConnection"]?.ConnectionString;
            }
        }
    }



