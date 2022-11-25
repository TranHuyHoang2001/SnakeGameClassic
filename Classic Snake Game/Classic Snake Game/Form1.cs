using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Imaging;  // thêm cái này cho jpg compressor

namespace Classic_Snake_Game
{
    public partial class Form1 : Form
    {

        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        


        int maxWidth;
        int maxHeight;

        int score;
        int highScore;

        Random rand = new Random();

        bool goLeft, goRight, goDown, goUp;
        public Form1()
        {
            InitializeComponent();

            new Settings();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Left && Settings.directions != "right")
            {
                goLeft = true;
            }    
            if(e.KeyCode == Keys.Right && Settings.directions != "left")
            {
                goRight = true;
            }    
            if(e.KeyCode == Keys.Up && Settings.directions != "down")
            {
                goUp = true;
            }    
            if(e.KeyCode == Keys.Down && Settings.directions != "up")
            {
                goDown = true;
            }    
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }

        }

        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void TakeSnapShot(object sender, EventArgs e)
        {
            // creat new label
            Label caption = new Label();
            caption.Text = "I scored: " + score + " and my HighScore is: " + highScore + " on the Snake Game";
            caption.Font = new Font("Times New Roman",14,FontStyle.Bold);
            caption.ForeColor = Color.AliceBlue;
            caption.AutoSize = false;
            caption.Width = picCanvas .Width;
            caption.Height = 30;
            caption.TextAlign = ContentAlignment.MiddleCenter;
            // add it to picture box
            picCanvas.Controls.Add(caption);

            // creat a new save dialog box
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Snake Game SnapShot";
            dialog.DefaultExt = "jpg";
            dialog.Filter = "JPG Image File | *.jpg";
            dialog.ValidateNames = true;

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                // define width and height of picture , source of picture
                int width = Convert.ToInt32(picCanvas.Width);
                int height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(width, height);
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, width, height)); // bmp : empty bitmap file created ; 0,0 is point location of image where want to start 
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);  // to access imageformat need add system drawing imaging
                picCanvas.Controls.Remove(caption);   // remove caption can go back to playing the game again
            }    
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            // setting the directions
            if(goLeft)
            {
                Settings.directions = "left";
            }  
            if(goRight)
            {
                Settings.directions = "right";
            }    
            if(goDown)
            {
                Settings.directions = "down";
            }    
            if(goUp)
            {
                Settings.directions = "up";
            }    
            // end of directions
            for(int i = Snake.Count - 1; i >= 0; i--)
            {
                if(i == 0)
                {
                    switch(Settings.directions)
                    {
                        case "left":
                            Snake[i].x--;
                            break;
                        case "right":
                            Snake[i].x++;          
                            break;
                        case "down":
                            Snake[i].y++;       
                            break;
                        case "up":
                            Snake[i].y--;
                            break;
                    }   
                    
                    if(Snake[i].x < 0)
                    {
                        Snake[i].x = maxWidth;
                    }    
                    if(Snake[i].x > maxWidth)
                    {
                        Snake[i].x = 0;
                    } 
                    if(Snake[i].y < 0)
                    {
                        Snake[i].y = maxHeight;
                    }   
                    if(Snake[i].y > maxHeight)
                    {
                        Snake[i].y = 0;
                    }

                    if(Snake[i].x == food.x && Snake[i].y == food.y)   // head snake i , 0 is head
                    {
                        EatFood();
                    } 
                    

                    for (int j = 1; j < Snake.Count; j++)     // body snake j , 1.. is body
                    {
                        if(Snake[i].x == Snake[j].x && Snake[i].y == Snake[j].y)
                        {
                            GameOver();
                        }    
                    }

                   
                }
                else
                {
                    Snake[i].x = Snake[i - 1].x;
                    Snake[i].y = Snake[i - 1].y;
                }
            }  
            picCanvas.Invalidate();

        }

        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColour;
            // vòng lặp cho đầu và thân rắn

            for(int i = 0; i < Snake.Count;i++)
            {
                if(i==0)
                {
                    snakeColour = Brushes.Black;
                }
                else
                {
                    snakeColour = Brushes.DarkGreen;
                }

                canvas.FillEllipse(snakeColour, new Rectangle
                    (
                    Snake[i].x * Settings.Width,
                    Snake[i].y * Settings.Height,
                    Settings.Width,Settings.Height
                    ));
                    
            }
            canvas.FillEllipse(Brushes.DarkRed, new Rectangle
                    (
                    food.x * Settings.Width,
                    food.y * Settings.Height,
                    Settings.Width, Settings.Height
                    ));
            

        }

        private void RestartGame()
        {
            maxWidth = picCanvas.Width / Settings.Width - 1;
            maxHeight = picCanvas.Height / Settings.Height - 1;

            Snake.Clear();

            btnStart.Enabled = false;
            btnSnap.Enabled = false;
            score = 0;
            lblScore.Text = "Score " + score;

            Circle head = new Circle { x = 10, y = 5 }; //creat head of snake
            Snake.Add(head); // add head of snake to LIST (vị trí thứ 0)

            for(int i = 0; i < 10; i++)
            {
                Circle body = new Circle();
                Snake.Add(body);
            }

            food = new Circle { x = rand.Next(2, maxWidth), y = rand.Next(2, maxHeight)};
           



            gameTimer.Start();
        }

        private void EatFood()
        {
            score += 1;

            lblScore.Text = "Score: " + score;

            Circle body = new Circle
            {
                x = Snake[Snake.Count - 1].x,        
                y = Snake[Snake.Count - 1].y
            };

            Snake.Add(body);

            food = new Circle { x = rand.Next(2, maxWidth), y = rand.Next(2, maxHeight) };
        }

        private void GameOver()
        {
            gameTimer.Stop();
            btnStart.Enabled = true;
            btnSnap.Enabled = true;

            if(score > highScore)
            {
                highScore = score;

                lblHighScore.Text = "High Score: " + Environment.NewLine + highScore;
                lblHighScore.ForeColor = Color.Maroon;
                lblHighScore.TextAlign = ContentAlignment.MiddleCenter;

            }    
        }    
    }
}
