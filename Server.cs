using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace Server_MoneyChange
{
    class Server
    {
        public Server()
        {
            ServerStart();
        }
        public void ServerStart()
        {
            //Setting the socket end point
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            string ip = "192.168.0.16";
            IPAddress ipAddr = IPAddress.Parse(ip);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 8888);

            //Setting the stream socket
            //TCP/IP protocol
            Socket sock = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("here");
            try
            {
                //Connecting the sock with the end point
                sock.Bind(ipEndPoint);
                //Starting to listen
                sock.Listen(10);
                while(true)
                {
                    Socket s = sock.Accept();
                    string username, password, first;//variables where the username and password will be kept
                    byte[] msg = new byte[1024];
                    byte[] msg2 = new byte[1024];

                    //recieving message that says, whether if it is login or sign up
                    //'register' for sign up and 'not' for login
                    int bytesCount = s.Receive(msg);
                    s.Close();
                    first = Encoding.UTF8.GetString(msg, 0, bytesCount);
                    if (first == "register")
                    {
                        bool answer = false;
                        while(answer == false)
                        {
                            //if the username is taken the function returns false
                            answer = Register(sock);
                            //than the client sends another message, informing on whether
                            //it is sign up or login
                            s = sock.Accept();
                            bytesCount = s.Receive(msg);
                            first = Encoding.UTF8.GetString(msg, 0, bytesCount);
                            s.Close();
                            //if it is a login, the program continues
                            if (first == "not") break;
                        }
                        
                    }
                        
                    
                    //recieving the username and the password from the client form 
                    s = sock.Accept();
                    bytesCount = s.Receive(msg);
                    s.Close();
                    username = Encoding.UTF8.GetString(msg, 0, bytesCount);
                    Console.WriteLine("username "+username);

                    s = sock.Accept();
                    msg = new byte[1024];
                    bytesCount = s.Receive(msg);
                    password = Encoding.UTF8.GetString(msg, 0, bytesCount);
                    Console.WriteLine("password "+password);
                    
                    //checking if they match with an existing user 
                    bool TheyMatch;
                    TheyMatch = DoTheyMatch(username, password);
                    if (TheyMatch)
                    {
                        Console.WriteLine("they match");
                        //sending to the form a 'right', so it knows to wait for the token
                        string right = "right";
                        byte[] rightMsg = Encoding.UTF8.GetBytes(right);
                        s.Send(rightMsg);

                        //sending, recieving and checking the token
                        string token = Token.Make(8);//the token 
                        string tokenRecieve;//the recieved token
                        string decision;//decision for if the tokens are equal or not
                        byte[] decMsg;//binary decision
                        byte[] tokenMsg = Encoding.UTF8.GetBytes(token);//binary token
                        s.Send(tokenMsg);

                        //Recieving the token
                        bytesCount = s.Receive(msg);
                        tokenRecieve = Encoding.UTF8.GetString(msg, 0, 8);

                        if (tokenRecieve == token)
                        {
                            //sends to the form 'true', so it knows to wait for the other data
                            decision = "true";
                            decMsg = Encoding.UTF8.GetBytes(decision);
                            s.Send(decMsg);

                            //the connection string for a user with low privilegies
                            String connstr = "server=192.168.0.16;user=user;database=currencies;port=3306;password=maznislav";
                            byte[] connStrMsg = Encoding.UTF8.GetBytes(connstr);
                            s.Send(connStrMsg);
                        }
                        else
                        {
                            Console.WriteLine("the tokens dont match");
                            //sends to the form 'false', so it knows that there is a problem
                            decision = "false";
                            decMsg = Encoding.UTF8.GetBytes(decision);
                            s.Send(decMsg);
                            s.Disconnect(true);
                        }
                    }
                    else
                    {
                        //if the tokens dont match, the server sends 'wrong'
                        string wrong = "Wrong";
                        byte[] wrongMsg = Encoding.UTF8.GetBytes(wrong);
                        s.Send(wrongMsg);
                    }
                    s.Shutdown(SocketShutdown.Both);
                    s.Close();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
       /* private string Hash(string password)
        {
            var data = Encoding.ASCII.GetBytes(password);
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1data = sha1.ComputeHash(data);
            string hashed = Encoding.UTF8.GetString(sha1data);
            return hashed;
        }*/
        private bool Register(Socket sock)
        {
            
                Console.WriteLine("Registering new user...");

                //making new connection where the new username, password and capital will be sent

                Socket s = sock.Accept();

                string username, password, bgn, usd, eur, gbp, chf, aud, cad;
                double suma;//variable for the capital in lv
                byte[] msg = new byte[1024];//here the bytes, that the client sent will be cept

                //recieving the username and password
                int bytesCount = s.Receive(msg);
                username = Encoding.UTF8.GetString(msg, 0, bytesCount);

                //Check() returns good or bad, depending on wheter the username is taken
                string exist = Check(username);
                byte[] existb = Encoding.UTF8.GetBytes(exist);
                //the server sends the info to the client
                s.Send(existb);
                if (exist == "bad")
                {
                    s.Close();
                    username = "";
                    return false;
                }

                //recieving the password
                s = sock.Accept();
                bytesCount = s.Receive(msg);
                s.Close();
                password = Encoding.UTF8.GetString(msg, 0, bytesCount);

                //recieving the capital
                #region RecievingCapital
                s = sock.Accept();
                bytesCount = s.Receive(msg);
                s.Close();
                bgn = Encoding.UTF8.GetString(msg, 0, bytesCount);

                s = sock.Accept();
                bytesCount = s.Receive(msg);
                s.Close();
                usd = Encoding.UTF8.GetString(msg, 0, bytesCount);

                s = sock.Accept();
                bytesCount = s.Receive(msg);
                s.Close();
                eur = Encoding.UTF8.GetString(msg, 0, bytesCount);

                s = sock.Accept();
                bytesCount = s.Receive(msg);
                s.Close();
                gbp = Encoding.UTF8.GetString(msg, 0, bytesCount);

                s = sock.Accept();
                bytesCount = s.Receive(msg);
                s.Close();
                chf = Encoding.UTF8.GetString(msg, 0, bytesCount);

                s = sock.Accept();
                bytesCount = s.Receive(msg);
                s.Close();
                aud = Encoding.UTF8.GetString(msg, 0, bytesCount);

                s = sock.Accept();
                bytesCount = s.Receive(msg);
                s.Close();
                cad = Encoding.UTF8.GetString(msg, 0, bytesCount);
                #endregion

                CurrencyReciever.GetCurrencies();
               
                //making the capital in lv
                suma = float.Parse(bgn.Replace('.', ',')) + float.Parse(usd.Replace('.', ',')) * (float.Parse(CurrencyReciever.kursKupuva["usd"].ToString().Replace('.', ','))) +
                    float.Parse(eur.Replace('.', ',')) * (float.Parse(CurrencyReciever.kursKupuva["eur"].ToString().Replace('.', ','))) +
                    float.Parse(gbp.Replace('.', ',')) * (float.Parse(CurrencyReciever.kursKupuva["gbp"].ToString().Replace('.', ','))) +
                    float.Parse(chf.Replace('.', ',')) * (float.Parse(CurrencyReciever.kursKupuva["chf"].ToString().Replace('.', ','))) +
                    float.Parse(aud.Replace('.', ',')) * (float.Parse(CurrencyReciever.kursKupuva["aud"].ToString().Replace('.', ','))) +
                    float.Parse(cad.Replace('.', ',')) * (float.Parse(CurrencyReciever.kursKupuva["cad"].ToString().Replace('.', ',')));


                //showing the username and password
                Console.WriteLine(username);
                Console.WriteLine(password);

                //making a mysql connection
                MySqlConnection cnn;
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                string connStr = "server=localhost;user=root;database=currencies;port=3306;password=maznislav";
                cnn = new MySqlConnection(connStr);
                cnn.Open();

                //the sql query that inserts the new user
                string sql = "INSERT INTO username_password VALUES(0, '" + username + "', '" + password +
                    "');";
                //inserting the command for the username and password
                MySqlCommand comd = new MySqlCommand(sql, cnn);
                adapter.InsertCommand = new MySqlCommand(sql, cnn);
                adapter.InsertCommand.ExecuteNonQuery();

                //making the command for the user's capital
                DateTime date = DateTime.Today;//It's needed in the query 
                sql = "INSERT INTO usercapital VALUES(0, '" + bgn + "', '" + usd + "', '" + eur + "', '" + gbp + "'," +
                    " '" + chf + "', '" + aud + "', '" + cad + "', '"
                    + suma.ToString().Replace(',','.') + "', '" + date.Year + "." + date.Month + "." + date.Day + "')";
                
                //inserting the command
                comd = new MySqlCommand(sql, cnn);
                adapter.InsertCommand = new MySqlCommand(sql, cnn);
                adapter.InsertCommand.ExecuteNonQuery();
                
                //closing the connection
                cnn.Close();

                //returning 'success' to the client if the registration is successfull 
                string result = "success";
                byte[] resMsg;

                resMsg = Encoding.UTF8.GetBytes(result);
                s = sock.Accept();
                s.Send(resMsg);
                s.Close();
                Console.WriteLine("User registered successfully!");
                return true;
            
            
        }
        private string Check(string username)
        {
            //variable for the result(do the username and password match) that the database returns (0 or 1)
            String result = "";

            //making connection with the MySQL database
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            string connStr = "server=localhost;user=root;database=currencies;port=3306;password=maznislav";
            cnn = new MySqlConnection(connStr);
            cnn.Open();

            String sql = "SELECT COUNT(1) " +
                "FROM username_password " +
                "WHERE username = '" + username + "';";

            //sending the command
            MySqlCommand comd = new MySqlCommand(sql, cnn);
            adapter.InsertCommand = new MySqlCommand(sql, cnn);
            adapter.InsertCommand.ExecuteNonQuery();

            //making a datareader that recieves the result from the database and writes it in the result variable
            MySqlDataReader dataReader = comd.ExecuteReader();
            while (dataReader.Read())
            {
                result += dataReader.GetValue(0);
            }

            //if there is such user with that password, then it returns true
            if (result == "1")
                return "bad";

            else return "good";
        }
    
        private bool DoTheyMatch(String username, String password)
        {
            //variable for the result(do the username and password match) that the database returns (0 or 1)
            String result = "";
            
            //making connection with the MySQL database
            MySqlConnection cnn;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            string connStr = "server=localhost;user=root;database=currencies;port=3306;password=maznislav";
            cnn = new MySqlConnection(connStr);
            cnn.Open();

            //the string that contains the query for the database
            String sql = "SELECT COUNT(1) " +
                "FROM username_password " +
                "WHERE username = '" + username + "' AND pass = '" + password + "';";
            
            //sending the command
            MySqlCommand comd = new MySqlCommand(sql, cnn);
            adapter.InsertCommand = new MySqlCommand(sql, cnn);
            adapter.InsertCommand.ExecuteNonQuery();
            
            //making a datareader that recieves the result from the database and writes it in the result variable
            MySqlDataReader dataReader = comd.ExecuteReader();
            while (dataReader.Read())
            {
                result += dataReader.GetValue(0);
                Console.WriteLine("Result = " + result);
            }

            //if there is such user with that password, then it returns true
            if (result == "1")
                return true;

            else return false;
        }
    }
}
