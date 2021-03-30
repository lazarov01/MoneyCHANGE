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

namespace MoneyChange
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private void SignUp_Click(object sender, EventArgs e)
        {
            //making the errors for empty 'username', 'password' and 'confirm password' fields
            if(UserNameTextBox.Text == "")
            {
                string caption = "Error!";
                string text = "Empty field for 'username'";
                MessageBoxButtons buttons = MessageBoxButtons.OK;

                DialogResult result;
                result = MessageBox.Show(text, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            if (PasswordTextBox.Text == "")
            {
                string caption = "Error!";
                string text = "Empty field for 'password'";
                MessageBoxButtons buttons = MessageBoxButtons.OK;

                DialogResult result;
                result = MessageBox.Show(text, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            if (ConfirmPassTextBox.Text == "")
            {
                string caption = "Error!";
                string text = "Empty field for 'confirm password'";
                MessageBoxButtons buttons = MessageBoxButtons.OK;

                DialogResult result;
                result = MessageBox.Show(text, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            if (BGN.Text == "")
            {
                string caption = "Error!";
                string text = "Empty field for 'BGN'";
                MessageBoxButtons buttons = MessageBoxButtons.OK;

                DialogResult result;
                result = MessageBox.Show(text, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
            //if the two passwords dont match, an error is displayed
            if(PasswordTextBox.Text == ConfirmPassTextBox.Text)
            {
                //checking if an user with the same username exists
                if (Check() == false)
                {
                    //if it exists, we make the user change the username
                    new Register();
                }
                //if it doesn't exist, the form sends command to the database to store the info of the 
                //new user
                else Send();
            }
            else
            {
                string caption = "Error!";
                string text = "Check your password!";
                MessageBoxButtons buttons = MessageBoxButtons.OK;

                DialogResult result;
                result = MessageBox.Show(text, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
            }
        }
        private bool Check()
        {
            
            
                string ip = "192.168.0.16";//the ip address of the server
                byte[] msg = new byte[1024];//buffer for the incoming message
                
                //parsing the ip from type string to type IPAddress
                IPAddress ipp = IPAddress.Parse(ip);

                IPEndPoint iPEndPoint = new IPEndPoint(ipp, 8888);

                Socket sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //connecting with the remote server
                sock.Connect(iPEndPoint);
                //declaring the variables for the different data
                string register = "register";
                byte[] registerb = Encoding.UTF8.GetBytes(register);

                string username = UserNameTextBox.Text.Trim();
                byte[] usernameb = Encoding.UTF8.GetBytes(username);//usernameb = username in bytes

                //sending message, that informs the server that th user is making a registration
                sock.Send(registerb);

                //closing and opening again the sock, so the server can make difference
                //between the data that is send to it
                sock.Close();
                sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(iPEndPoint);

                //sending the username
                sock.Send(usernameb);

                //recieving the feedback from the server
                //if the username is taken the message will be 'bad'
                int bytes = sock.Receive(msg);
                sock.Close();
                string checkUsername = Encoding.UTF8.GetString(msg, 0, bytes);
                //if the message is bad, the form displays an error message
                if (checkUsername == "bad")
                {
                    string caption = "Error!";
                    string text = "Username taken!";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;

                    DialogResult result;
                    result = MessageBox.Show(text, caption, buttons);
                    if (result == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.Close();
                    }
                    
                    return false;
                }
                return true;
            
        }
        private void Send()
        {
            string ip = "192.168.0.16";//the ip address of the server
            byte[] msg = new byte[1024];//buffer for the incoming message

            //parsing the ip from type string to type IPAddress
            IPAddress ipp = IPAddress.Parse(ip);

            IPEndPoint iPEndPoint = new IPEndPoint(ipp, 8888);

            Socket sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            sock.Connect(iPEndPoint);
            //declaring the variables for the different data
            string password = PasswordTextBox.Text.Trim();
            byte[] passwordb = Encoding.UTF8.GetBytes(password);//passwordb = password in bytes

            //declaring the variables for the capital in each currency
            string bgn = BGN.Text.Trim().Replace(',','.');
            byte[] bgnb = Encoding.UTF8.GetBytes(bgn);

            string usd = USD.Text.Trim().Replace(',', '.');
            byte[] usdb = Encoding.UTF8.GetBytes(usd);

            string eur = EUR.Text.Trim().Replace(',', '.');
            byte[] eurb = Encoding.UTF8.GetBytes(eur);
            
            string gbp = GBP.Text.Trim().Replace(',', '.');
            byte[] gbpb = Encoding.UTF8.GetBytes(gbp);

            string chf = CHF.Text.Trim().Replace(',', '.');
            byte[] chfb = Encoding.UTF8.GetBytes(chf);

            string aud = AUD.Text.Trim().Replace(',', '.');
            byte[] audb = Encoding.UTF8.GetBytes(aud);

            string cad = CAD.Text.Trim().Replace(',', '.');
            byte[] cadb = Encoding.UTF8.GetBytes(cad);

            //sending the password
            sock.Send(passwordb);

            //closing and opening again the sock, so the server can make difference
            //between the data that is send to it
            sock.Close();
            sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(iPEndPoint);

            //sending the capital
            #region SendingCapital

            sock.Send(bgnb);

            sock.Close();
            sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(iPEndPoint);

            sock.Send(usdb);

            sock.Close();
            sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(iPEndPoint);

            sock.Send(eurb);

            sock.Close();
            sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(iPEndPoint);

            sock.Send(gbpb);

            sock.Close();
            sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(iPEndPoint);

            sock.Send(chfb);

            sock.Close();
            sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(iPEndPoint);

            sock.Send(audb);

            sock.Close();
            sock = new Socket(ipp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(iPEndPoint);
            
            sock.Send(cadb);
            #endregion

            //feedback that gives info if has everything gone ok
            int bytes = sock.Receive(msg);
            string success = Encoding.UTF8.GetString(msg, 0, bytes);
            
            //if it has, the form displays message that everything is ok 
            if(success == "success")
            {
                string caption = "Success!";
                string text = "Registration successful!";
                MessageBoxButtons buttons = MessageBoxButtons.OK;

                DialogResult result;
                result = MessageBox.Show(text, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Close();
                }
                this.Close();
            }
        }

        //making the password field display '*' for the password
        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            // The password character is an asterisk.
            PasswordTextBox.PasswordChar = '*';
            // The control will allow no more than 14 characters.
            PasswordTextBox.MaxLength = 14;
        }

        private void ConfirmPassTextBox_TextChanged(object sender, EventArgs e)
        {
            // The password character is an asterisk.
            ConfirmPassTextBox.PasswordChar = '*';
            // The control will allow no more than 14 characters.
            ConfirmPassTextBox.MaxLength = 14;
        }
    }
}
