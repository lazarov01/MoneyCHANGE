using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MoneyChange
{
    public partial class Manage : Form
    {
        public Manage()
        {
            InitializeComponent();
        }
       
        public void DisplayCurrencyInfo(string nameCap, string name)//the currency name in capital letters
            //and in small letters
        {
            CurrencyName.Text = nameCap;
            //geting the current currency from the hastable in Kurs.cs
            CurrBuy.Text = Kurs.kursK[name].ToString();
            CurrSale.Text = Kurs.kursP[name].ToString();
            //the user's capital in this currency is from the hashtable in Capital 
            Usercapital.Text = Capital.cap[name].ToString();
            CurrToConvert.Text = name;

            //intitializing the currency rates in List<> for the graph
            List<double> CurRatesS = new List<double>();
            List<double> CurRatesB = new List<double>();
            List<int> TimeRates = new List<int>();

            //getting the current rates from the database
            InitializeCurRatesSell(CurRatesS, name);
            InitializeCurRatesBuy(CurRatesB, name);
            InitializeTimeRates(TimeRates, name);

            //setting up the chart
            //y - currency rate
            //x - day
            chart1.Series["Sell"].Points.DataBindXY(TimeRates, CurRatesS);
            chart1.Series["Buy"].Points.DataBindXY(TimeRates, CurRatesB);
            chart1.ChartAreas[0].AxisY.Minimum = CurRatesB.Min() - 0.01;
            chart1.ChartAreas[0].AxisY.Maximum = CurRatesS.Max() + 0.01;
        }
        private void InitializeCurRatesBuy(List<double> cur, string curname)//the list which is
            //going to content the currency,   the currency name
        {
            //setting up a connection with the database
            string result = "";
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            cnn = new MySqlConnection(LoginForm.connStr);
            cnn.Open();

            //the command that gets the last 15 buy rates
            String sql = "SELECT kursK FROM " + curname + " WHERE did > (SELECT MAX(did) - 15 FROM usd)";
            MySqlCommand comd = new MySqlCommand(sql, cnn);
            //inserting the command
            adapter.InsertCommand = new MySqlCommand(sql, cnn);
            adapter.InsertCommand.ExecuteNonQuery();
            comd.Dispose();

            //reading the results
            MySqlDataReader dataReader = comd.ExecuteReader();
            while (dataReader.Read())
            {
                result += dataReader.GetValue(0);
                cur.Add(Convert.ToDouble(result));
                result = "";
            }
            dataReader.Close();
            cnn.Close();
        }
        private void InitializeCurRatesSell(List<double> cur, string curname)//List<>, that contains
            //the last 15 currencies, the currency name
        {
            //setting the connection with the database
            string result = "";
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            cnn = new MySqlConnection(LoginForm.connStr);
            cnn.Open();

            //writing the command that gets the last 15 sell rates
            String sql = "SELECT kursP FROM " + curname + " WHERE did > (SELECT MAX(did) - 15 FROM usd)";
            MySqlCommand comd = new MySqlCommand(sql, cnn);
            adapter.InsertCommand = new MySqlCommand(sql, cnn);
            adapter.InsertCommand.ExecuteNonQuery();
            comd.Dispose();

            //reading the results
            MySqlDataReader dataReader = comd.ExecuteReader();
            while (dataReader.Read())
            {
                result += dataReader.GetValue(0);
                cur.Add(Convert.ToDouble(result));
                result = "";
            }
            dataReader.Close();
            cnn.Close();
        }
        private void InitializeTimeRates(List<int> time, string curname)//List<> containing the sequense
            //in which the rates are inserted, the current currency name
        {
            //setting up the database connection
            string result = "";
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            cnn = new MySqlConnection(LoginForm.connStr);
            cnn.Open();

            //writing the command that gets the sequense
            String sql = "SELECT did FROM " + curname + " WHERE did > (SELECT MAX(did) - 15 FROM usd)";
            MySqlCommand comd = new MySqlCommand(sql, cnn);
            //inserting the command
            adapter.InsertCommand = new MySqlCommand(sql, cnn);
            adapter.InsertCommand.ExecuteNonQuery();
            comd.Dispose();

            //reading the results
            MySqlDataReader dataReader = comd.ExecuteReader();
            while (dataReader.Read())
            {
                result += dataReader.GetValue(0);
                time.Add(Int32.Parse(result));
                result = "";
            }
            dataReader.Close();
            cnn.Close();
        }
        //when a button is clicked, it opens the function DisplayCurrencyInfo()
        //and it sends the currency name in CAPITAL letters and in small letters
        #region Currency Buttons
        private void USDbutton_Click(object sender, EventArgs e)
        {
            DisplayCurrencyInfo("USD","usd");
        }

        private void EURbutton_Click(object sender, EventArgs e)
        {
            DisplayCurrencyInfo("EUR", "eur");
        }

        private void GBPbutton_Click(object sender, EventArgs e)
        {
            DisplayCurrencyInfo("GBP", "gbp");
        }

        private void CHFbutton_Click(object sender, EventArgs e)
        {
            DisplayCurrencyInfo("CHF", "chf");
        }

        private void AUDbutton_Click(object sender, EventArgs e)
        {
            DisplayCurrencyInfo("AUD", "aud");
        }

        private void CADbutton_Click(object sender, EventArgs e)
        {
            DisplayCurrencyInfo("CAD", "cad");
        }
        #endregion

        private void ConvertButton_Click(object sender, EventArgs e)
        {
            //the errors in different scenarios
            if (ChooseCurr.Text == CurrToConvert.Text)
            {
                string caption = "Error!";
                string text = "The currencies for the convertion are the same!";
                MessageBoxButtons buttons = MessageBoxButtons.OK;

                DialogResult result;
                result = MessageBox.Show(text, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            if (SumToConvert.Text == "")
            {
                string caption = "Error!";
                string text = "Sum to convert is null!";
                MessageBoxButtons buttons = MessageBoxButtons.OK;

                DialogResult result;
                result = MessageBox.Show(text, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            if (Int32.Parse(SumToConvert.Text) == 0)
            {
                string caption = "Error!";
                string text = "Sum to convert is 0!";
                MessageBoxButtons buttons = MessageBoxButtons.OK;

                DialogResult result;
                result = MessageBox.Show(text, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            if (Int32.Parse(SumToConvert.Text) < 0)
            {
                string caption = "Error!";
                string text = "Invalid sum!";
                MessageBoxButtons buttons = MessageBoxButtons.OK;

                DialogResult result;
                result = MessageBox.Show(text, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            //if everything is correct
            if(Int32.Parse(SumToConvert.Text) > 0)
            {
                Capital cap = new Capital();
                double Cur1minus;//variable containing the capital in the currency, from which the user
                //subtracts money

                //Example: Convert.ToDouble(Capital.cap[usd])
                Cur1minus = Convert.ToDouble(Capital.cap[ChooseCurr.SelectedItem]);
                //subtracting the sum from the first currency
                Cur1minus -= Convert.ToDouble(SumToConvert.Text);
                //two symbols after decimal point
                Cur1minus = (double)System.Math.Round(Cur1minus, 2);
                //saving the result from subtracting capital from the first currency,
                //for it to be send to the other
                Capital.cap[ChooseCurr.SelectedItem] = Cur1minus.ToString();

                //variable containing the capital in the second currency where the user sends money
                //formula for finding the rate of the currency in which the user sends money
                //(example) EUR/USD = USD/BGN / EUR/BGN
                double Cur2plus = Convert.ToDouble(SumToConvert.Text) /
                    (Convert.ToDouble(Kurs.kursP[CurrToConvert.SelectedItem]) / 
                    Convert.ToDouble(Kurs.kursP[ChooseCurr.SelectedItem]));
                //two symbols after the decimal point
                Cur2plus = (double)System.Math.Round(Cur2plus, 2);
                
                //saving the result from the transaction in the variable in the currency,
                //where money are send
                Capital.cap[CurrToConvert.Text] = Convert.ToDouble(Capital.cap[CurrToConvert.SelectedItem])
                    + Cur2plus;

                //Displaying the updated capitals in each of the currencies that took 
                //part in the convertion
                FirstCurrCap.Text = Capital.cap[ChooseCurr.SelectedItem].ToString() 
                    + " " + ChooseCurr.SelectedItem;
                SecondCurrCap.Text = Capital.cap[CurrToConvert.SelectedItem].ToString() 
                    + " " + CurrToConvert.SelectedItem;

                //connecting with the database
                MySqlConnection cnn;
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                cnn = new MySqlConnection(LoginForm.connStr);
                cnn.Open();

                //making the command which updates the user capital in the database and sending it
                String sql = "UPDATE usercapital SET " + CurrToConvert.Text + " = '" + Capital.cap[CurrToConvert.Text].ToString().Replace(',','.') + "', " + ChooseCurr.SelectedItem + " = '" + Capital.cap[ChooseCurr.SelectedItem].ToString().Replace(',','.') +
                    "' WHERE uid = '" + LoginForm.uid + "';";
                MySqlCommand comd = new MySqlCommand(sql, cnn);
                adapter.InsertCommand = new MySqlCommand(sql, cnn);
                adapter.InsertCommand.ExecuteNonQuery();
                comd.Dispose();
                this.Refresh();
                this.Invalidate();
            }
            
        }

        private void ChooseCurr_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Displaying the up-to-date capital in the first currency that takes part in the converion
            FirstCurrCap.Text = Capital.cap[ChooseCurr.SelectedItem].ToString() + " " 
                + ChooseCurr.SelectedItem;
        }

        private void CurrToConvert_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Displaying the up-to-date capital in the second currency that takes part in the converion
            SecondCurrCap.Text = Capital.cap[CurrToConvert.SelectedItem].ToString() + " " 
                + CurrToConvert.SelectedItem;
        }
    }
}
