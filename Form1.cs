using System;
using System.Net.Sockets;
using System.Net;
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
    public partial class LoginForm : Form
    {
        public static string connStr;
        public static string uid = "";
        public LoginForm()
        {
            InitializeComponent();
        }
        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            // The password character is an asterisk.
            PasswordTextBox.PasswordChar = '*';
            // The control will allow no more than 14 characters.
            PasswordTextBox.MaxLength = 14;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            byte[] msg = new byte[1024];//buffer for communication with the server
            string didUserpass;
            string token;//where the recieved token will be kept
            string didTokenPass;//did the token pass
            string connString;//connection string
            string ip = "192.168.0.16";

            //loop for communication with the remote server
            //it breaks and starts again if the username or password is incorrect
            while(true)
            {
                //parsing the ip from type string to type IPAddress
                IPAddress ipp = IPAddress.Parse(ip);
                //the correct ip and port of the server
                IPEndPoint iPEndPoint = new IPEndPoint(ipp, 8888);
                //making the socket
                Socket sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //connecting with the server
                sock.Connect(iPEndPoint);
                //variable that contains the message to the server, informing it that this is not
                //sign up form
                string notreg = "not";
                byte[] notregb = Encoding.UTF8.GetBytes(notreg);

                //sending the username
                string username = UsernameTextbox.Text.Trim();
                byte[] usernameb = Encoding.UTF8.GetBytes(username);//usernameb= username in binary

                //and password
                string password = PasswordTextBox.Text.Trim();
                byte[] passwordb = Encoding.UTF8.GetBytes(password);
                //sending the message that this is not sign up form
                sock.Send(notregb);

                //closing and opening again the sock, so the server can make difference between the username and password 
                sock.Close();
                sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(iPEndPoint);

                //sending the username
                sock.Send(usernameb);

                //closing and opening again the sock, so the server can make difference between the username and password 
                sock.Close();
                sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(iPEndPoint);

                //sending the password
                sock.Send(passwordb);

                //are the username and password right
                int bytes = sock.Receive(msg);
                didUserpass = Encoding.UTF8.GetString(msg, 0, bytes);
                if (didUserpass == "Wrong")
                {
                    //if not, displaying an error message and breaking the loop
                    string caption = "Error!";
                    string text = "Wrong username or password";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;

                    DialogResult result;
                    result = MessageBox.Show(text, caption, buttons);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    }
                    break;
                }

                //recieving the token
                bytes = sock.Receive(msg);
                token = Encoding.UTF8.GetString(msg, 0, bytes);

                //sending the token
                sock.Send(msg);

                //true/false - did the token pass
                bytes = sock.Receive(msg);
                didTokenPass = Encoding.UTF8.GetString(msg, 0, bytes);


                //connection string for the database
                bytes = sock.Receive(msg);
                connString = Encoding.UTF8.GetString(msg, 0, bytes);
                connStr = connString;
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();

                //making connection to the database
                MySqlConnection cnn;
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                cnn = new MySqlConnection(connStr);
                cnn.Open();

                //command that gets the user id
                string sql = "SELECT uid FROM username_password WHERE username = '" + username + "' AND " +
                    "pass = '" + password + "';";

                //inserting the command
                MySqlCommand comd = new MySqlCommand(sql, cnn);
                adapter.InsertCommand = new MySqlCommand(sql, cnn);
                adapter.InsertCommand.ExecuteNonQuery();

                //reading from the output the result
                MySqlDataReader dataReader = comd.ExecuteReader();
                while (dataReader.Read())
                {
                    uid += dataReader.GetValue(0);
                }
                cnn.Close();

                //Opening the main form
                Main mainForm = new Main();
                var dialogResult = mainForm.ShowDialog();
                //when the main form is closed, we save the new capital (all in lv.) in the database
                ZapisNaCapital();
                this.Close();
                break;
            }
            
        }
        private void ZapisNaCapital()
        {
            //making the connection with the database
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            cnn = new MySqlConnection(LoginForm.connStr);
            cnn.Open();

            //Changing the string that contains the sum, so it has decimal point, rather than coma 
            string suma = Capital.UserCapital.Replace(',', '.');
            string sql = "UPDATE usercapital SET usum = '" + suma + "' WHERE uid = '" + LoginForm.uid
                + "';";

            //Inserting the command
            MySqlCommand comd = new MySqlCommand(sql, cnn);
            adapter.InsertCommand = new MySqlCommand(sql, cnn);
            adapter.InsertCommand.ExecuteNonQuery();
        }
        private void RegisterButton_Click(object sender, EventArgs e)
        {
            Register register = new Register();
            register.Show();
        }
    }
}
