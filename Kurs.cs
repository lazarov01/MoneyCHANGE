using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace MoneyChange
{
    class Kurs
    {
        //variables for the buy rate
        public static string UsdKursK;
        public static string EurKursK;
        public static string GbpKursK;
        public static string ChfKursK;
        public static string AudKursK;
        public static string CadKursK;

        //variables for the sell rate
        public static string UsdKursP;
        public static string EurKursP;
        public static string GbpKursP;
        public static string ChfKursP;
        public static string AudKursP;
        public static string CadKursP;

        //hashtables, containing the currency rates
        public static Hashtable kursK = new Hashtable();
        public static Hashtable kursP = new Hashtable();

        public Kurs()
        {
            GetKurs();
        }
        private void GetKurs()
        {
            //keys that are used for the hashtables
            string[] keys = { "usd", "eur", "gbp", "chf", "aud", "cad" };
            //setting up the database connection
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            cnn = new MySqlConnection(LoginForm.connStr);
            cnn.Open();

            //for each key buy rates
            foreach (String s in keys)
            {
                //sql command
                String sql = "SELECT kursK FROM " + s + " WHERE did = (SELECT MAX(did) FROM usd)";
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
                    kursK.Add(s, result);
                }
                dataReader.Close();
                
            }
            //for each key sell rates
            foreach (String s in keys)
            {
                //sql command
                String sql = "SELECT kursP FROM " + s + " WHERE did = (SELECT MAX(did) FROM usd)";
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
                    kursP.Add(s, result);
                }
                dataReader.Close();
                
            }
            //adding the bgn 
            kursK.Add("bgn", 1.00);
            kursP.Add("bgn", 1.00);

            //saving the rates in the variables
            UsdKursK = kursK["usd"].ToString();
            EurKursK = kursK["eur"].ToString();
            GbpKursK = kursK["gbp"].ToString();
            ChfKursK = kursK["chf"].ToString();
            AudKursK = kursK["aud"].ToString();
            CadKursK = kursK["cad"].ToString();

            UsdKursP = kursP["usd"].ToString();
            EurKursP = kursP["eur"].ToString();
            GbpKursP = kursP["gbp"].ToString();
            ChfKursP = kursP["chf"].ToString();
            AudKursP = kursP["aud"].ToString();
            CadKursP = kursP["cad"].ToString();
        }
    }
}
