using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Server_MoneyChange
{
    class CurrencySaver
    {
        public CurrencySaver()
        {
            
        }
        public static CurrencySaver Save()//Saves the current rates of the currencies
        {
            //setting up the sql connection
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            string connStr = "server=localhost;user=root;database=currencies;port=3306;password=maznislav";
            cnn = new MySqlConnection(connStr);
            cnn.Open();

            //keys of the hashtables that contain the rates
            ICollection keys = CurrencyReciever.kursKupuva.Keys;
            foreach (String s in keys)
            {
                //setting up the command
                String sql = "INSERT INTO " + s + " VALUES(0, '" + CurrencyReciever.kursKupuva[s] + "', '"
                    + CurrencyReciever.kursProdava[s] + "')";
                MySqlCommand comd = new MySqlCommand(sql, cnn);
                //inserting the command
                adapter.InsertCommand = new MySqlCommand(sql, cnn);
                adapter.InsertCommand.ExecuteNonQuery();
                comd.Dispose();
            }
            return new CurrencySaver();
        }
    }
}
