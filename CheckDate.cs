using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Server_MoneyChange
{
    class CheckDate
    {
        
        public bool IsTrue = false;
        public CheckDate()
        {
            Check();
        }
        public void Check()//checks if the currencies have been changed this day
        {
            string result = "";//variable for the result that the database returns
            string d;//variable for the latest day in the database
            DateTime date = DateTime.Today;//getting the date

            //setting the sqq connection with the database
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            string connStr = "server=localhost;user=root;database=currencies;port=3306;password=maznislav";
            cnn = new MySqlConnection(connStr);
            cnn.Open();

            //inserting the command that returns the latest day in the database 
            string sql = "SELECT MAX(ldate) FROM usercapital";
            MySqlCommand comd = new MySqlCommand(sql, cnn);
            adapter.InsertCommand = new MySqlCommand(sql, cnn);
            adapter.InsertCommand.ExecuteNonQuery();

            //getting the result
            MySqlDataReader dataReader = comd.ExecuteReader();
            while (dataReader.Read())
            {
                result += dataReader.GetValue(0);
            }
            d = date.ToString();
            //if the day is today 
            if (d == result) IsTrue = true;

            cnn.Close();
        }
        public void Change()//when the currencies are saved, this function changes the last day of change
        {
            //setting up the sql connection
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            string connStr = "server=localhost;user=root;database=currencies;port=3306;password=maznislav";
            cnn = new MySqlConnection(connStr);
            cnn.Open();

            //getting the current date
            DateTime date = DateTime.Today;
            //setting up the command
            string sql = "UPDATE usercapital SET ldate = '" + date.Year + "." +  date.Month + "." + date.Day + "';";
            MySqlCommand comd = new MySqlCommand(sql, cnn);
            //insterting it
            adapter.InsertCommand = new MySqlCommand(sql, cnn);
            adapter.InsertCommand.ExecuteNonQuery();

            cnn.Close();

        }
    }
}
