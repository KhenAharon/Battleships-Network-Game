using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Battleships
{
    public partial class Form1 : Form
    {
        private const byte boardDividing = 10; 
        private int row; 
        private int column; 
        private PictureBox tempPic; 
        private bool isShipHorizontal; 
        private byte size;
        private byte shipNo; 
        private bool[] isShipPlaced_byNumber = new bool[5]; 
                                                            
        private bool shipHasChosen;
        private Logic logic;
        private int placingTurns;

        ///////////////

        private Thread outputThread;
        private TcpClient connection; 
        private NetworkStream stream;
        private BinaryWriter writer; 
        private BinaryReader reader;
        private char myMark; 
        private bool myTurn;
        private bool done = false;
        //////////////
        private int pixX;
        private int pixY;
        private bool isPanelEntered; 
        private int fireCounter;
        private bool locFireAtHitter; 
        private bool locMissAtHitter;
        private bool locMissAtHitted;
        private System.Media.SoundPlayer myPlayer;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            myPlayer= new System.Media.SoundPlayer();
            locMissAtHitted = false;
            locMissAtHitter = false;
            isPanelEntered = false;
            locFireAtHitter = false;
            fireCounter = 1;
            tempPic = null;
            isShipHorizontal = true;
            for (int i = 0; i < 5; i++)
            {
                isShipPlaced_byNumber[i] = false;
            }
            shipHasChosen = false;
            shipNo = 10; 
                          
            logic = new Logic();
            placingTurns = 0;
            
            connection = new TcpClient("127.0.0.1", 50000);
            stream = connection.GetStream();
            writer = new BinaryWriter(stream);
            reader = new BinaryReader(stream);
            
            outputThread = new Thread(new ThreadStart(Run));
            outputThread.Start();
            
        }

        private void PaintBoard(object sender, PaintEventArgs e)
        {
            
            if(isPanelEntered)
            {
                switch (fireCounter)
                {
                    case 1: pictureBox6.Location = new System.Drawing.Point(pixX, pixY); pictureBox6.Visible = true;  break;
                    case 2: pictureBox7.Location = new System.Drawing.Point(pixX, pixY); pictureBox7.Visible = true; break;
                    case 3: pictureBox8.Location = new System.Drawing.Point(pixX, pixY); pictureBox8.Visible = true; break;
                    case 4: pictureBox9.Location = new System.Drawing.Point(pixX, pixY); pictureBox9.Visible = true; break;
                    case 5: pictureBox10.Location = new System.Drawing.Point(pixX, pixY); pictureBox10.Visible = true; break;
                    case 6: pictureBox11.Location = new System.Drawing.Point(pixX, pixY); pictureBox11.Visible = true; break;
                    case 7: pictureBox12.Location = new System.Drawing.Point(pixX, pixY); pictureBox12.Visible = true; break;
                    case 8: pictureBox13.Location = new System.Drawing.Point(pixX, pixY); pictureBox13.Visible = true; break;
                    case 9: pictureBox14.Location = new System.Drawing.Point(pixX, pixY); pictureBox14.Visible = true; break;
                    case 10: pictureBox15.Location = new System.Drawing.Point(pixX, pixY); pictureBox15.Visible = true; break;
                    case 11: pictureBox16.Location = new System.Drawing.Point(pixX, pixY); pictureBox16.Visible = true; break;
                    case 12: pictureBox17.Location = new System.Drawing.Point(pixX, pixY); pictureBox17.Visible = true; break;
                    case 13: pictureBox18.Location = new System.Drawing.Point(pixX, pixY); pictureBox18.Visible = true; break;
                    case 14: pictureBox19.Location = new System.Drawing.Point(pixX, pixY); pictureBox19.Visible = true; break;
                    case 15: pictureBox20.Location = new System.Drawing.Point(pixX, pixY); pictureBox20.Visible = true; break;
                    case 16: pictureBox21.Location = new System.Drawing.Point(pixX, pixY); pictureBox21.Visible = true; break;
                    case 17: pictureBox22.Location = new System.Drawing.Point(pixX, pixY); pictureBox22.Visible = true; break;
                }
                isPanelEntered = false;
            }
            else if (locFireAtHitter)
            {
                locFireAtHitter = false;

                PictureBox temp;
                temp = new System.Windows.Forms.PictureBox();
                temp.BackColor = System.Drawing.Color.Transparent;
                temp.Image = global::Battleships.Properties.Resources.Fire;
                temp.Location = new System.Drawing.Point(pixX, pixY);
                temp.Name = "fire";
                temp.Size = new System.Drawing.Size(29, 29);
                temp.TabIndex = 8;
                this.Controls.Add(temp);
            }
            else if (locMissAtHitter)
            {
                locMissAtHitter = false;
                PictureBox temp;
                temp = new System.Windows.Forms.PictureBox();
                temp.BackColor = System.Drawing.Color.Transparent;
                temp.Image = global::Battleships.Properties.Resources.miss;
                temp.Location = new System.Drawing.Point(pixX, pixY);
                temp.Name = "miss";
                temp.Size = new System.Drawing.Size(29, 29);
                temp.TabIndex = 8;
                this.Controls.Add(temp);
            }
            else if (locMissAtHitted)
            {
                locMissAtHitted = false;
                PictureBox temp;
                temp = new System.Windows.Forms.PictureBox();
                temp.BackColor = System.Drawing.Color.Transparent;
                temp.Image = global::Battleships.Properties.Resources.miss;
                temp.Location = new System.Drawing.Point(pixX, pixY);
                temp.Name = "miss";
                temp.Size = new System.Drawing.Size(29, 29);
                temp.TabIndex = 8;
                this.Controls.Add(temp);
            }


            

            float width = 300; 
            float height = this.Size.Height - 35 - 60;
            float gape = width / boardDividing; 
            Pen p = new Pen(Color.Navy, 1);
            for (float i = 0; i <= width; i += gape)
            {
                CreateGraphics().DrawLine(p, i, 0, i, height);
                CreateGraphics().DrawLine(p, 0, i, width, i);
            }
            width = this.Size.Width - 9;
            Pen p2 = new Pen(Color.DarkRed, 1);
            for (float i = 301, j = 0; i <= width; i += gape, j += gape)
            {
                CreateGraphics().DrawLine(p2, i, 0, i, height);
                CreateGraphics().DrawLine(p2, 301, j, width, j);
            }
            Font a1 = new Font("Arial", 20);
            Brush b = new SolidBrush(Color.Yellow);
            Point p1 = new Point(358, 315);
            e.Graphics.DrawString("BATTLESHIPS", a1, b, p1);
        }

        public void SendClickedChoice(int location)
        {
            if (myTurn)
            {
                writer.Write(location);
                pixY = ((location/10) * 30 + 1);
                pixX = ((location%10) * 30 + 1+301);
            }
        }
        private void MouseClickS(object sender, MouseEventArgs e)
        {
            int squareLength = (this.Size.Width - 9 - 301) / boardDividing;
            if (placingTurns == 5)
            {
                if (e.Button == MouseButtons.Left)
                {
                    row = e.Y / squareLength; 
                    column = (e.X - 301) / squareLength;
                    if (e.X < 301 || e.Y > 300)
                    {
                        MessageBox.Show("You must blew at your enemy board only.");
                    }
                    else
                        SendClickedChoice(((e.Y / (squareLength)) - 1) * boardDividing + e.X / (squareLength));
                }
                else
                    MessageBox.Show("You must use left mouse button to blow.");
            }
            else
            {
                if (shipNo == 10)
                {
                    MessageBox.Show("Please choose a ship to place.");
                    return; 
                }
                if (e.X / (squareLength) < 10)
                {
                    row = e.Y / squareLength;
                    column = e.X / squareLength;
                    if (!isShipPlaced_byNumber[shipNo - 1])
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            if (isShipHorizontal)
                            {
                                switch (shipNo)
                                {
                                    case 1: tempPic.Image = Image.FromFile("../../pics/ships/warship5a.gif"); break;
                                    case 2: tempPic.Image = Image.FromFile("../../pics/ships/warship4a.gif"); break;
                                    case 3: tempPic.Image = Image.FromFile("../../pics/ships/warship3a.gif"); break;
                                    case 4: tempPic.Image = Image.FromFile("../../pics/ships/warship3a2.gif"); break;
                                    case 5: tempPic.Image = Image.FromFile("../../pics/ships/warship2a.gif"); break;
                                    default: break;
                                }
                                isShipHorizontal = false;
                            }
                            else
                            {
                                switch (shipNo)
                                {
                                    case 1: tempPic.Image = Image.FromFile("../../pics/ships/warship5b.gif"); break;
                                    case 2: tempPic.Image = Image.FromFile("../../pics/ships/warship4b.gif"); break;
                                    case 3: tempPic.Image = Image.FromFile("../../pics/ships/warship3b.gif"); break;
                                    case 4: tempPic.Image = Image.FromFile("../../pics/ships/warship3b2.gif"); break;
                                    case 5: tempPic.Image = Image.FromFile("../../pics/ships/warship2b.gif"); break;
                                    default: break;
                                }
                                isShipHorizontal = true;
                            }
                        }
                        else
                        {
                            bool problem = false;
                            if (CheckIfExceedMatBorderLimits(row, column))
                            {
                                MessageBox.Show("The ship can not be located at the place you desire,\nbecause its size exceeds the borders of the game's board.");
                                problem = true;
                            }
                            else if (logic.CheckWetherShipAlreadyLocated(size, row, column, isShipHorizontal))
                            {
                                MessageBox.Show("The ship can not be located on another ship.\nPlease find an absolut empty place.");
                                problem = true;
                            }
                            else if (!logic.CheckIfShipIsNotNearOtherShip(size,row,column,isShipHorizontal))
                            {
                                MessageBox.Show("Ship cannot be located near another ship.");
                                problem = true;
                            }
                            if (!problem)
                                ApplyLeftClick();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please choose a ship to place.");
                    }
                }
                else
                    MessageBox.Show("You must place your ship at your board only (blue border).");
            }
        }
        private void ApplyLeftClick()
        {
            logic.CreateShipInArr(shipNo, size, row, column, isShipHorizontal, tempPic);
            tempPic.Location = new Point((column) * 30 + 1, (row) * 30 + 1);
            isShipPlaced_byNumber[shipNo - 1] = true;
            shipHasChosen = false;
            logic.AddShipToMat(shipNo, size, row, column, isShipHorizontal);
            placingTurns++;
        }
        private bool CheckIfExceedMatBorderLimits(int row, int column)
        {
            int subtrahend;
            if (isShipHorizontal)
                subtrahend = column;
            else
                subtrahend = row;
            
            if (boardDividing - subtrahend - size >= 0 && row < 10 && column < 10)
                return false; 
            else
                return true;
        }
        private void LeftClickWhenAllShipArePlaced(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int squareLength = (this.Size.Width - 9 - 301) / boardDividing; 
                row = e.Y / squareLength; 
                column = (e.X - 301) / squareLength; 
                if (e.X < 301 || e.Y > 300)
                {
                    MessageBox.Show("You must blew at your enemy board only.");
                }
            }
        }
        private void MouseClickShip5(object sender, MouseEventArgs e)
        {
            if (!shipHasChosen)
            {
                shipNo = 1;
                if (isShipPlaced_byNumber[shipNo - 1])
                    MessageBox.Show("This ship has already been placed!\nPlease choose another ship to place.");
                else
                {
                    shipHasChosen = true;
                    isShipHorizontal = true;
                    tempPic = (PictureBox)sender;
                    size = 5;
                }
            }
            else
                MessageBox.Show("Please locate the ship you have already chosen.");
        }

        private void MouseClickShip4(object sender, MouseEventArgs e)
        {
            if (!shipHasChosen)
            {
                shipNo = 2;
                if (isShipPlaced_byNumber[shipNo - 1])
                    MessageBox.Show("This ship has already been placed!\nPlease Choose another ship to place.");
                else
                {
                    shipHasChosen = true;
                    tempPic = (PictureBox)sender;
                    size = 4;
                    isShipHorizontal = true;
                }
            }
            else
                MessageBox.Show("Please locate the ship you have already chosen.");
        }

        private void MouseClickShip3(object sender, MouseEventArgs e)
        {
            if (!shipHasChosen)
            {
                shipNo = 3;
                if (isShipPlaced_byNumber[shipNo - 1])
                    MessageBox.Show("This ship has already been placed!\nPlease Choose another ship to place.");
                else
                {
                    shipHasChosen = true;
                    tempPic = (PictureBox)sender;
                    size = 3;
                    isShipHorizontal = true;
                }
            }
            else
                MessageBox.Show("Please locate the ship you have already chosen.");

        }
        private void MouseClickShip3No2(object sender, MouseEventArgs e)
        {
            if (!shipHasChosen)
            {
                shipNo = 4;
                if (isShipPlaced_byNumber[shipNo - 1])
                    MessageBox.Show("This ship has already been placed!\nPlease Choose another ship to place.");
                else
                {
                    shipHasChosen = true;
                    tempPic = (PictureBox)sender;
                    size = 3;
                    isShipHorizontal = true;
                }
            }
            else
                MessageBox.Show("Please locate the ship you have already chosen.");

        }

        private void MouseClickShip2(object sender, MouseEventArgs e)
        {
            if (!shipHasChosen)
            {
                shipNo = 5;
                if (isShipPlaced_byNumber[shipNo - 1])
                    MessageBox.Show("This ship has already been placed!\nPlease Choose another ship to place.");
                else
                {
                    shipHasChosen = true;
                    tempPic = (PictureBox)sender;
                    size = 2;
                    isShipHorizontal = true;
                }
            }
            else
                MessageBox.Show("Please locate the ship you have already chosen.");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ClientClosing(object sender, FormClosingEventArgs e)
        {
            done = true;
            System.Environment.Exit(System.Environment.ExitCode);
        }
        private delegate void DisplayDelegate(string message);
        private void DisplayMessage(string message)
        {
            if (textBox1.InvokeRequired)
            {
                
                Invoke(new DisplayDelegate(DisplayMessage), new object[] { message });
            }
            else
            {
                textBox1.Text += message;
            }
        }
        private delegate void ChangeIdLabelDelegate(string message);
        private void ChangeIdLabel(string message)
        {
            if (idLabel.InvokeRequired)
            {
               
                Invoke(new ChangeIdLabelDelegate(ChangeIdLabel), new object[] { message });
            }
            else
            {
                idLabel.Text = message;
            }
        }
        public void Run()
        {
            myMark = reader.ReadChar();//מקבלים את סימון השחקן איקס או עיגול
            ChangeIdLabel("You are player \"" + myMark + "\"");
            myTurn = (myMark == 'X' ? true : false);
            
            try
            {
                
                while (!done)
                {
                    ProcessMessage(reader.ReadString());
                }
                Thread.Sleep(1000);
            }
            catch (IOException)
            {
                MessageBox.Show("Server is down, game over", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        public bool IsTargetHitted(int location)
        {
            
            return true;
        }
        public void ProcessMessage(string message)
        {
            if (message == "Oponent trying to blow up.")
            {
                int loca = reader.ReadInt32();
                int row = loca / 10;
                int col = loca % 10;
                bool isShipFired = logic.FireSquare(row, col);
                pixY = ((row) * 30 + 1);
                pixX = ((col) * 30 + 1);
                DisplayMessage("Need to check if opponent succeeded " + loca);
                if (isShipFired)
                {

                    isPanelEntered = true;

                    
                    Refresh();
                    fireCounter++;
                    myPlayer.SoundLocation = @"..\..\sound\explosion.wav";
                    myPlayer.Play();

                    if (logic.isLastPartOfShipFired(row, col))
                    {
                        if (logic.AreAllShipsBlew) 
                        {
                            writer.Write(-5);
                            MessageBox.Show("You lost this game!");
                        }
                        else
                            writer.Write(-4);
                    }
                    else
                    {
                        writer.Write(-1);
                    }
                }
                else
                {
                    locMissAtHitted = true;
                    Refresh();
                    writer.Write(-2);
                    myPlayer.SoundLocation = @"..\..\sound\water.wav";
                    myPlayer.Play();
                }

            }
            else if (message == "Opponent said you have a hit.")
            {
                DisplayMessage("You have another try.\r\n");
                myTurn = true;
                locFireAtHitter = true;
                Refresh();
                myPlayer.SoundLocation = @"..\..\sound\explosion.wav";
                myPlayer.Play();
            }
            else if (message == "Opponent said you have a hit, and you blew up last part of ship.")
            {
                DisplayMessage("You have another try.\nYou blew up the last part of the ship.\r\n");
                myTurn = true;
                locFireAtHitter = true;
                Refresh();
                myPlayer.SoundLocation = @"..\..\sound\explosion.wav";
                myPlayer.Play();
                MessageBox.Show("You blew up the last part of the ship.");
            }
            else if (message == "You missed. You lost your turn.")
            {
                locMissAtHitter = true;
                Refresh();
                myPlayer.SoundLocation = @"..\..\sound\water.wav";
                myPlayer.Play();
                DisplayMessage("Not a good try, please wait.");
                myTurn = false;
                writer.Write(-3);
            }
            else if (message == "Turn is moving to you.")
            {
                myTurn = true;
                DisplayMessage("Opponent missed, it is your turn.");
            }
            else if (message == "Congradulations! You are the WINNER!")
            {
                DisplayMessage("You won this game.\r\n");
                myTurn = false;
                locFireAtHitter = true;
                Refresh();
                myPlayer.SoundLocation = @"..\..\sound\explosion.wav";
                myPlayer.Play();
                MessageBox.Show("Congradulations! You are the WINNER!");
                writer.Write(-6);
                done = true;
            }
            else if (message == "Shut down server.")
            {
                done = true;
            }
            else
            {
                DisplayMessage(message + "\r\n");
            }
        }
    }
}