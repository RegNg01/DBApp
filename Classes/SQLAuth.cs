using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DBApp
{
    internal class SQLAuth
    {
        public MySqlConnection connection;
        
      public SQLAuth() 
        {
            string connStr = "server=ition-x.ru;" +
                            "user=user_smpc;" +
                            "database=db_smpc;" +
                            "port=22;" +
                            "password=smpc!practica@";
            connection = new MySqlConnection(connStr);
            try
            { 
                connection.Open();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
