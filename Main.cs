using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace MoneyChange
{
    public partial class Main : Form
    {
        
        public Main()
        {
            InitializeComponent();
            Display();
        }
        private void Display()
        {
            DisplayBalance();
            DisplayCapital();
            DisplayKurs();
        }
        private void DisplayKurs()
        {
            //Displaying the currency rates of each currency
            Kurs kurs = new Kurs();
            usdkursk.Text = Kurs.UsdKursK;
            eurkursk.Text = Kurs.EurKursK;
            gbpkursk.Text = Kurs.GbpKursK;
            chfkursk.Text = Kurs.ChfKursK;
            audkursk.Text = Kurs.AudKursK;
            cadkursk.Text = Kurs.CadKursK;

            usdkursp.Text = Kurs.UsdKursP;
            eurkursp.Text = Kurs.EurKursP;
            gbpkursp.Text = Kurs.GbpKursP;
            chfkursp.Text = Kurs.ChfKursP;
            audkursp.Text = Kurs.AudKursP;
            cadkursp.Text = Kurs.CadKursP;
        }
        private void DisplayCapital()
        {
            //Displaying the user's capital
            Capital cap = new Capital();
            BGNCap.Text = Capital.BGN;
            USDCap.Text = Capital.USD;
            EURCap.Text = Capital.EUR;
            GBPCap.Text = Capital.GBP;
            CHFCap.Text = Capital.CHF;
            AUDCap.Text = Capital.AUD;
            CADCap.Text = Capital.CAD;
        }
        private void DisplayBalance()
        {
            Capital cap = new Capital();
            string balance = "";//variable that contains the old sum of the user
            //(before the new calulation with the up-to-date rates

            //making the connectiong with the database
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            cnn = new MySqlConnection(LoginForm.connStr);
            cnn.Open();

            //getting the old sum 
            string sql = "SELECT uc.usum FROM usercapital uc " +
                "WHERE uc.uid = '" + LoginForm.uid + "';";

            //inserting the command
            MySqlCommand comd = new MySqlCommand(sql, cnn);
            adapter.InsertCommand = new MySqlCommand(sql, cnn);
            adapter.InsertCommand.ExecuteNonQuery();

            //reading the output
            MySqlDataReader dataReader = comd.ExecuteReader();
            while (dataReader.Read())
            {
                balance += dataReader.GetValue(0);
            }
            cnn.Close();

            //variable, containing the up-to-date sum
            float suma = Suma();
            suma = (float)System.Math.Round(suma, 2);
            label3.Text = suma.ToString().Trim();
            Capital.UserCapital = suma.ToString().Trim();

            float sum = suma - float.Parse(balance);//variable that contains the difference between
            //the old capital and the up-to-date one
            sum = (float)System.Math.Round(sum, 2);

            //displaying the differnence
            string label = "";
            //if the user lost money, it displays '-'
            if (sum < 0)
            {
                label += sum.ToString() + " .lv";
            }
            //if he won, it displays '+'
            else if (sum > 0)
            {
                label += "+";
                label += sum.ToString() + " .lv";
            }
            else
            {
                label += "+";
                label += "0 lv.";
            }
            label5.Text = label.Trim();

        }
        public float Suma()//Function that calculates the user's capital, making it all in BGN
        {
            string balance = "";//variable for the up-to-date capital in BGN

            //making the connection with the database
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            cnn = new MySqlConnection(LoginForm.connStr);
            cnn.Open();

            //sql query calculating the new balance
            string sql = "SELECT (uc.bgn + uc.usd*usd.kursK + uc.gbp*gbp.kursK + uc.eur*eur.kursK " +
                "+ uc.chf*chf.kursK + uc.cad*cad.kursK + uc.aud*aud.kursK) AS suma " +
                "FROM usercapital uc, usd, gbp, eur, chf, cad, aud WHERE uc.uid = '" + LoginForm.uid + "' " +
                "AND usd.did = (SELECT MAX(did) FROM usd) AND gbp.did= (SELECT MAX(did) FROM usd)" +
                " AND eur.did= (SELECT MAX(did) FROM usd) " +
                "AND chf.did= (SELECT MAX(did) FROM usd) AND cad.did = (SELECT MAX(did) FROM usd)" +
                " AND aud.did = (SELECT MAX(did) FROM usd)";

            //inserting the command
            MySqlCommand comd = new MySqlCommand(sql, cnn);
            adapter.InsertCommand = new MySqlCommand(sql, cnn);
            adapter.InsertCommand.ExecuteNonQuery();

            //reading the output
            MySqlDataReader dataReader = comd.ExecuteReader();
            while (dataReader.Read())
            {
                balance += dataReader.GetValue(0);
            }
            //returning the result in float
            return float.Parse(balance);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if the 'Manage' button is clicked
            Manage manageForm = new Manage();
            var dialogResult = manageForm.ShowDialog();
            DisplayCapital();
        }
    }
}
