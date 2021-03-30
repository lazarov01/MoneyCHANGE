using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MoneyChange
{
    class Capital
    {
        //variables that the main form uses to display the user's capital in each currency
        public static string BGN = "";
        public static string USD = "";
        public static string EUR = "";
        public static string GBP = "";
        public static string CHF = "";
        public static string AUD = "";
        public static string CAD = "";

        public static string UserCapital = "";//saving happens in Main.DisplayBalance()

        public static Hashtable cap = new Hashtable();

        public Capital()
        {
            GetUserCapital();
        }
        public Capital (string bgn, string usd, string eur, string gbp, string chf, string aud, string cad)
        {
            BGN = bgn;
            USD = usd;
            EUR = eur;
            GBP = gbp;
            CHF = chf;
            AUD = aud;
            CAD = cad;
        }
        public Capital GetUserCapital()
        {
            string[] keys = { "bgn", "usd", "eur", "gbp", "chf", "aud", "cad" };

            //making the mysql connection with the database
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            cnn = new MySqlConnection(LoginForm.connStr);
            cnn.Open();

            foreach (String s in keys)
            {
                //the command gets the info of the current user's capital
                String sql = "SELECT " + s + " FROM usercapital WHERE uid = '" + LoginForm.uid + "'";
                MySqlCommand comd = new MySqlCommand(sql, cnn);
                //inserting the command
                adapter.InsertCommand = new MySqlCommand(sql, cnn);
                adapter.InsertCommand.ExecuteNonQuery();
                comd.Dispose();

                //getting the result
                string result = "";
                MySqlDataReader dataReader = comd.ExecuteReader();
                while (dataReader.Read())
                {
                    result += dataReader.GetValue(0);
                    cap[s] = result;
                }
                dataReader.Close();
                
            }
            //closing the connection
            cnn.Close();
            //saving the info in the variables
            return new Capital(cap["bgn"].ToString(), cap["usd"].ToString(), cap["eur"].ToString(), cap["gbp"].ToString(), cap["chf"].ToString(), cap["aud"].ToString(), cap["cad"].ToString());
        }
    }
}
