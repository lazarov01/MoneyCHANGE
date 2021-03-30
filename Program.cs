using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_MoneyChange
{
    class Program
    {
        static void Main(string[] args)
        {
            //checks when is the last change to the currencies
            CheckDate date = new CheckDate();
            if (!date.IsTrue)
            {
                //if it is not today
                //gets and saves the currencies in the MySQL database and changes the last date of change
                CurrencyReciever.GetCurrencies();
                Console.WriteLine("Currencies recieved!");

                CurrencySaver.Save();
                Console.WriteLine("Currencies saved!");
                
                date.Change();
                Console.WriteLine("Date changed");

            }
            
            //Starting the server
            Server server = new Server();
        }
    }
}
