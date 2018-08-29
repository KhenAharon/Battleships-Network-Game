using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Server
{
    public partial class ServerForm : Form
    {
        private int[,] UserGameBoard = new int[10, 10];
        private Player[] players;
        private Thread[] playerThreads; 
        private TcpListener listener; 
        private int currentPlayer; 
        private Thread getPlayers;
        internal bool disconnected = false; 
        public ServerForm()
        {
            InitializeComponent();
        }

        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            disconnected = true;
            System.Environment.Exit(System.Environment.ExitCode);
            
        }

        public bool IsHit(int location)
        {
            bool b = true;
            
            return b;
        }
        public bool GameOver()
        {
            return false;
        }

        private delegate void DisplayDelegate(string message);
        internal void DisplayMessage(string message)
        {
            if (displayTextBox.InvokeRequired)
            {
                
                Invoke(new DisplayDelegate(DisplayMessage), new object[] { message });
            }
            else
            {
                displayTextBox.Text += message;
                
            }
        }

        private void Server_FormLoad(object sender, EventArgs e)
        {
            
            players = new Player[2];
            playerThreads = new Thread[2];
            currentPlayer = 0;
            getPlayers = new Thread(new ThreadStart(SetUp));
            getPlayers.Start();
        }
        public void SetUp()
        {
            DisplayMessage("Waiting for players...\r\n");
            
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 50000);
            listener.Start();
            
            players[0] = new Player(listener.AcceptSocket(), this, 0);
           
            playerThreads[0] = new Thread(new ThreadStart(players[0].Run));
            playerThreads[0].Start();
            players[1] = new Player(listener.AcceptSocket(), this, 1);
            
            playerThreads[1] = new Thread(new ThreadStart(players[1].Run));
            playerThreads[1].Start();
            
            lock (players[0])
            {
                players[0].threadSuspended = false;
                Monitor.Pulse(players[0]);
            } 
        }

        public bool TryingToBlow(int location, int player)
        {
            lock(this)
            {
                while (currentPlayer != player)
                {
                    Monitor.Wait(this);
                }
               // if(IsHit(location))
                //{
                  //  return true;
                //}
                //else
                //MessageBox.Show("oihiohoihioh"+location);
                {
                    //סמן פיצוץ
                    currentPlayer=(currentPlayer+1)%2;
                    players[currentPlayer].OtherPlayerMoved(location);
                    Monitor.Pulse(this);
                    return true;//?
                }
            }
        }

    }
    public class Player
    {
        
        internal Socket connection; 
        private NetworkStream socketStream; 
        private ServerForm server; 
        private BinaryWriter writer; 
        private BinaryReader reader; 
        private int number; 
        private char mark;
        internal bool threadSuspended = true; 

        public void Run()
        {
            bool done = false;
            server.DisplayMessage("player "+(number==0 ? 'X':'O')+ " connected\r\n");
            
            writer.Write(mark);
            
            writer.Write("player " + ((number == 0) ? "X connected.\r\n" : "O connected, please wait.\r\n"));
            if (mark == 'X')
            {
                writer.Write("Waiting for another player.");
                
                lock (this)
                {
                    while (threadSuspended)
                    {
                        Monitor.Wait(this);
                    }
                }
                writer.Write("Other player connected. Your move.");
            }
            while (!done)
            {
                while (connection.Available == 0)
                {
                    Thread.Sleep(1000);
                    if (server.disconnected)
                    {
                        return;
                    }
                }
                int location = reader.ReadInt32();
                if (server.TryingToBlow(location, number))
                {
                    server.DisplayMessage("loc: " + location + "\r\n");
                    //writer.Write("successfull blow please continue.");
                }
                else
                {
                    writer.Write("you missed you lost your turn.");
                }
                if(server.GameOver())
                {
                    done=true;
                }

            }
            writer.Close();
            reader.Close();
            socketStream.Close();
            connection.Close();
        }
        public void OtherPlayerMoved(int location)
        {
            if (location == -1)
            {
                writer.Write("Opponent said you have a hit.");
            }
            else if (location == -2)
            {
                writer.Write("You missed. You lost your turn.");
            }
            else if (location == -3)
            {
                writer.Write("Turn is moving to you.");
            }
            else if (location == -4)
            {
                writer.Write("Opponent said you have a hit, and you blew up last part of ship.");
            }
            else if (location == -5)
            {
                writer.Write("Congradulations! You are the WINNER!");
            }
            else if (location == -6)
            {
                writer.Write("Shut down server.");
            }
            else if (location <= 99 && location >= 0)
            {
                writer.Write("Oponent trying to blow up.");
                writer.Write(location);
            }
        }
        public Player(Socket socket, ServerForm serverValue, int newNumber)
        {
            number = newNumber;
            mark = (number == 0) ? 'X' : 'O';
            connection = socket;
            server = serverValue;
            socketStream = new NetworkStream(connection);
            
            writer = new BinaryWriter(socketStream);
            reader = new BinaryReader(socketStream);
        }
    }
}