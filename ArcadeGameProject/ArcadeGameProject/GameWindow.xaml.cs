using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using System.Configuration;
using System.Media;

namespace ArcadeGameProject
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        # region Variabelen
        public DispatcherTimer gameTimer = new DispatcherTimer();
        private List<Rectangle> itemsToRemove = new List<Rectangle>();
        private Random rand = new Random();
        private List<Enemies> EnemiesOnScreen = new List<Enemies>();

        private Enemytype SpawnType;

        private const double cHeight = 960;

        private const int playerSpeed = 10;
        private const int bulletSpeed = 20;

        private int Innerwall = 640;
        private int outerwall = 1280;
        private int enemySpawnCounter = 50;
        private int EnemySpawnLimit = 30;
        private int Time;
        private int seconds;
        private int Backgroundseconds = 120;
        private int minutes = 2;
        private int scoreP1=0;
        private int scoreP2=0;

        public MainWindow MW;

        private bool moveLeft1, moveRight1, moveLeft2, moveRight2, Enemyspawn;
        public string Playername1;
        public string Playername2;

        public object Nameplayer1 { get; internal set; }

        //MediaPlayer Sound1 = new MediaPlayer();

        #endregion

        //private void MusicPlay1(object sender, RoutedEventArgs e)
        //{
        //    MediaPlayer Sound1 = new MediaPlayer();
        //    Sound1.Open(new Uri(@"D:\ProjectArcadeGIT\ArcadeGameProject\ArcadeGameProject\music\shrek8bit.wav"));
        //    Sound1.Play();
        //}

        public GameWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            MyCanvas.Focus();

            //timer setup
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameEngine;
            gameTimer.Start();

            //score wordt gezet op 0
            ScoreP1.Content = scoreP1;
            ScoreP2.Content = scoreP2;

            //achtergrond plaatje
            ImageBrush bg = new ImageBrush();

            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/plaatjes/Backgroundwline.png"));
            MyCanvas.Background = bg;

            ImageBrush playerimage1 = new ImageBrush();
            playerimage1.ImageSource = new BitmapImage(new Uri("pack://application:,,,/plaatjes/player1.png"));
            Player1.Fill = playerimage1;

            ImageBrush playerimage2 = new ImageBrush();
            playerimage2.ImageSource = new BitmapImage(new Uri("pack://application:,,,/plaatjes/player2.png"));
            Player2.Fill = playerimage2;
        }

        /// <summary>
        /// de methode die ervoor zorgt dat zodra een key wordt ingedrukt er een actie true wordt gezet die in de gamengine wordt verwerkt.
        /// de volgende keys zijn gebruikt:
        /// speler 1 : a voor links, d voor rechts
        /// speler 2 : pijl links, pijl rechts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                moveLeft1 = true;
            }
            if (e.Key == Key.D)
            {
                moveRight1 = true;
            }
            if (e.Key == Key.Left)
            {
                moveLeft2 = true;
            }
            if (e.Key == Key.Right)
            {
                moveRight2 = true;
            }
        }

        /// <summary>
        /// de methode die ervoor zorgt dat na het een key is ingedrukt en losgelaten er een actie true/false wordt gezet die in de gamengine wordt verwerkt
        /// de volgende keys zijn gebruikt:
        /// speler 1 : a voor links, d voor rechts worden false gezet want bewegen stopt
        /// speler 1 : w is voor schieten die een nieuwe bullet aanmaakt
        /// speler 2 : pijl links, pijl rechts worden false gezet want bewegen stopt
        /// speler 2 : pijl up is voor schieten die een nieuwe bullet aanmaakt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                moveLeft1 = false;
            }
            if (e.Key == Key.D)
            {
                moveRight1 = false;
            }
            if (e.Key == Key.Left)
            {
                moveLeft2 = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight2 = false;
            }
            if (e.Key == Key.W)
            {
                CreateBullet(Player1);
            }
            if (e.Key == Key.Up)
            {
                CreateBullet(Player2);
            }
            if (e.Key == Key.Escape)
            {
                gameTimer.Stop();
                PauseWindow PW = new PauseWindow();
                PW.Visibility = Visibility.Visible;
                this.Visibility = Visibility.Hidden;
                PW.GW = this;
                PW.MW = MW;

                PW.player1 = Playername1;
                PW.player2 = Playername2;
            }
        }

        /// <summary>
        /// de gameengine die elke 0,02 seconde wordt aangeroepen, waarin :
        /// alle movement van de spelers en enemies wordt aangepast/ weergegeven in de canvas. 
        /// de collision(raken) van bullet en enemy en bullet(enemytype3) en de player wordt bepaald.
        /// de scores van beide spelers wordt bepaald 
        /// de timer die de waves beinvloedt en bepaalt wordt berekent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GameEngine(object sender, EventArgs e)
        {
            #region timer 
            Time++;
            if (Time == 50)
            {
                //de gameengine wordt elke 0,02 seconde afgespeeld, en 50 * 0,02 = 1 seconde
                seconds--;
                Backgroundseconds--;
                Time = 0;
            }
            if (seconds < 0)
            {
                //60 seconde = 1minuut
                minutes--;
                seconds = 59;
            }
            if (seconds < 10)
            {
                Timer.Content = minutes + " : " + 0 + seconds;
            }
            else if (minutes < 0)
            {
                Timer.Content = "0 : 0";
            }
            else
            {
                Timer.Content = minutes + " : " + seconds;
            }

            #endregion

            #region movement players + shooting players
            if (moveLeft1 && Canvas.GetLeft(Player1) > 0)
            {
                Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) - playerSpeed);
            }
            if (moveRight1 && Canvas.GetLeft(Player1) + Player1.Width < Innerwall)
            {
                Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) + playerSpeed);
            }
            if (moveLeft2 && Canvas.GetLeft(Player2) > Innerwall)
            {
                Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) - playerSpeed);
            }
            if (moveRight2 && Canvas.GetLeft(Player2) + Player2.Width < outerwall - Player2.Width *0.25)
            {
                Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) + playerSpeed);
            }
            #endregion

            #region EnemySpawning
            enemySpawnCounter--;
            //background seconds is de totale secondes die voorbij zijn;
            if (Backgroundseconds >= 118 && Backgroundseconds <= 120)
            {
                ScreenMessage.Content = "Wave 1";
                Enemyspawn = false;
                enemySpawnCounter = 0;
            } //wave 1 aankondiging
            else if (Backgroundseconds >= 88 && Backgroundseconds <= 118)
            {
                ScreenMessage.Content = "";
                int b = rand.Next(1, 100);
                if (enemySpawnCounter < 0) 
                {
                    if (b == 1)
                    {
                        SpawnType = Enemytype.Enemy4;
                    }
                    else
                    {
                        SpawnType = Enemytype.Enemy1;
                    }
                }
                Enemyspawn = true;

            } //wave 1 (enemy 1)
            else if (Backgroundseconds >= 86 && Backgroundseconds <= 88)
            {
                ScreenMessage.Content = "Wave 2";
                Enemyspawn = false;
                enemySpawnCounter = 0;
            }//wave 2 aankondiging
            else if (Backgroundseconds >= 56 && Backgroundseconds <= 86)
            {
                ScreenMessage.Content = "";
                int b = rand.Next(1, 100);
                if (enemySpawnCounter < 0)
                {
                    if (b == 1)
                    {
                        SpawnType = Enemytype.Enemy4;
                    }
                    else
                    {
                        if (SpawnType == Enemytype.Enemy2)
                        {
                            SpawnType = Enemytype.Enemy1;
                        }
                        else if (SpawnType == Enemytype.Enemy1 || SpawnType == Enemytype.Enemy4)
                        {
                            int a = rand.Next(1, 3);
                            if (a == 1)
                            {
                                SpawnType = Enemytype.Enemy2;
                            }
                            else if (a == 2)
                            {
                                SpawnType = Enemytype.Enemy1;
                            }

                        }
                    }
                        
                }
                Enemyspawn = true;

            }//wave 2 (enemy 1 en 2)
            else if (Backgroundseconds >= 54 && Backgroundseconds <= 56)
            {
                ScreenMessage.Content = "Wave 3";
                Enemyspawn = false;
                enemySpawnCounter = 0;

            }//wave 3 aankondiging
            else if (Backgroundseconds >= 0 && Backgroundseconds <= 56)//wave 3
            {
                ScreenMessage.Content = "";
                int b = rand.Next(1, 100);
                if (enemySpawnCounter < 0)
                {
                    if (b == 1)
                    {
                        SpawnType = Enemytype.Enemy4;
                    }
                    else
                    {
                        if (SpawnType == Enemytype.Enemy3)
                        {
                            SpawnType = Enemytype.Enemy1;
                        }
                        else if (SpawnType == Enemytype.Enemy2)
                        {
                            SpawnType = Enemytype.Enemy1;
                        }
                        else if (SpawnType == Enemytype.Enemy1 || SpawnType == Enemytype.Enemy4)
                        {
                            int a = rand.Next(1, 4);
                            if (a == 1)
                            {
                                SpawnType = Enemytype.Enemy2;
                            }
                            else if (a == 2)
                            {
                                SpawnType = Enemytype.Enemy1;
                            }
                            else if (a == 3)
                            {
                                SpawnType = Enemytype.Enemy3;
                            }

                        }
                    }
                }
                Enemyspawn = true;

            }//wave 3 (enemy 1, 2 en 3)
            else if (Backgroundseconds <= 0)//gameover
            {

                if (Time == 0)
                {
                    string insert = "INSERT INTO Highscores (Highscore, Player) VALUES (@scoreP1, @Playername1)";
                    string insert2 = "INSERT INTO Highscores (Highscore, Player) VALUES (@scoreP2, @Playername2)";
                    //string insert2 = "INSERT INTO Highscores (Highscore) VALUES (@scoreP1)";

                    string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Projectarcade\\ArcadeGameProject\\ArcadeGameProject\\Data\\Database1.mdf;Integrated Security=True";

                    {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(insert, connection))
                        {

                            if (string.IsNullOrEmpty(ScoreP1.Content.ToString()))
                            {
                                command.Parameters.AddWithValue("@scoreP1, @Playername1", DBNull.Value);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@scoreP1", (SqlDbType)scoreP1);
                                command.Parameters.AddWithValue("@Playername1", Playername1);
                            }
                            //bgsec = *50 time = *1
                            command.ExecuteNonQuery();
                        }

                        using (SqlCommand command = new SqlCommand(insert2, connection))
                        {

                            if (string.IsNullOrEmpty(ScoreP1.Content.ToString()))
                            {
                                command.Parameters.AddWithValue("@scoreP2, @Playername2", DBNull.Value);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("@scoreP2", (SqlDbType)scoreP2);
                                command.Parameters.AddWithValue("@Playername2", Playername2);
                            }
                            //bgsec = *50 time = *1
                                
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                      }
                  }
                }

                Enemyspawn = false;
                gameTimer.Stop();
                GameOver.Content = "GameOver";

                if (scoreP1 > scoreP2)
                {
                    Winner.Content = Playername1 + " Wins With " + scoreP1 + " Points! ";
                }

                else if (scoreP2 > scoreP1)
                {
                    Winner.Content = Playername2 + " Wins With " + scoreP2 + " Points!";
                }

                else if (scoreP2 == scoreP1)
                {
                   Winner.Content = "Draw!";
                }

                Esc.Content = "Press ESC To continue";

                for (int i = 0; i < EnemiesOnScreen.Count; i++)
                {
                    itemsToRemove.Add(EnemiesOnScreen[i].rectangle);
                }


                foreach (Rectangle x in MyCanvas.Children.OfType<Rectangle>())
                {
                    if ((string)x.Tag == "BulletPlayer" || (string)x.Tag == "BulletEnemy")
                    {
                        itemsToRemove.Add(x);
                    }
                }

            } //GameOver

            if (Enemyspawn == true) //er wordt gekeken of er enmies gespawnd kunnen worden;
            {
                if (enemySpawnCounter < 0)
                {
                    CreateEnemy(0, Innerwall, SpawnType, side.left); //make enemies for player1
                    CreateEnemy(Innerwall, outerwall, SpawnType, side.right); //makes enemies for player 2
                    enemySpawnCounter = EnemySpawnLimit; //reset the enemy counter to the limit integer
                }
            }
            #endregion

            #region enemiesmovement
            //de reden voor de for-loop inplaats van de foreach is omdat de count van de enemiesonscreen list verandert in de loop, door dat er wat wordt verwijderd, wat een foreach niet aan kan en een for loop wel 
            for (int i = 0; i< EnemiesOnScreen.Count; i++)
            {
                Canvas.SetTop(EnemiesOnScreen[i].rectangle, Canvas.GetTop(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].speed);

                //checkt of enemy type 2 is en dus de horizontale movement bepaald
                if (EnemiesOnScreen[i].enemyType == Enemytype.Enemy2)
                {

                    if (Canvas.GetLeft(EnemiesOnScreen[i].rectangle) >= EnemiesOnScreen[i].InWall && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) <= EnemiesOnScreen[i].InWall + EnemiesOnScreen[i].sidespeed)
                    {
                        EnemiesOnScreen[i].ToRight = true;
                        EnemiesOnScreen[i].ToLeft = false;
                        Canvas.SetLeft(EnemiesOnScreen[i].rectangle, Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].sidespeed);
                    }
                    else if (Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].rectangle.Width <= EnemiesOnScreen[i].OutWall && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].rectangle.Width >= EnemiesOnScreen[i].OutWall - EnemiesOnScreen[i].sidespeed)
                    {
                        EnemiesOnScreen[i].ToRight = false;
                        EnemiesOnScreen[i].ToLeft = true;
                        Canvas.SetLeft(EnemiesOnScreen[i].rectangle, Canvas.GetLeft(EnemiesOnScreen[i].rectangle) - EnemiesOnScreen[i].sidespeed);
                    }
                    else if (EnemiesOnScreen[i].ToRight == true && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].rectangle.Width <= EnemiesOnScreen[i].OutWall - EnemiesOnScreen[i].sidespeed && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) >= EnemiesOnScreen[i].InWall + EnemiesOnScreen[i].sidespeed)
                    {
                        Canvas.SetLeft(EnemiesOnScreen[i].rectangle, Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].sidespeed);

                    }
                    else if (EnemiesOnScreen[i].ToLeft == true && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].rectangle.Width <= EnemiesOnScreen[i].OutWall - EnemiesOnScreen[i].sidespeed && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) >= EnemiesOnScreen[i].InWall + EnemiesOnScreen[i].sidespeed)
                    {
                        Canvas.SetLeft(EnemiesOnScreen[i].rectangle, Canvas.GetLeft(EnemiesOnScreen[i].rectangle) - EnemiesOnScreen[i].sidespeed);

                    }
                }

                if (EnemiesOnScreen[i].enemyType == Enemytype.Enemy3)
                {
                    if (EnemiesOnScreen[i].bulletcount >= 40)
                    {
                        CreateBullet(EnemiesOnScreen[i].rectangle);
                        EnemiesOnScreen[i].bulletcount = 0;
                    }
                    else
                    {
                        EnemiesOnScreen[i].bulletcount++;
                    }

                }


                //removal van de enemies met het optellen van punten
                if (Canvas.GetTop(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].rectangle.Height > cHeight)
                {
                    if (EnemiesOnScreen[i].enemyType == Enemytype.Enemy4)
                    {
                        //alles behalve type 4 wordt in de else gedaan en afgespeeld
                    }
                    else
                    {
                        itemsToRemove.Add(EnemiesOnScreen[i].rectangle);
                        if (EnemiesOnScreen[i].WhichSide == side.left)
                        {
                            scoreP1 = scoreP1 - (EnemiesOnScreen[i].score / 2);
                            if (scoreP1 < 0)
                            {
                                scoreP1 = 0;
                            }
                            ScoreP1.Content = scoreP1;
                        }
                        else if (EnemiesOnScreen[i].WhichSide == side.right)
                        {
                            scoreP2 = scoreP2 - (EnemiesOnScreen[i].score / 2);
                            if (scoreP2 < 0)
                            {
                                scoreP2 = 0;
                            }
                            ScoreP2.Content = scoreP2;
                        }
                        // de reden waarom we remove doen in de list is omdat de score dan gaat glitchen en niet meer correct doet
                        EnemiesOnScreen.Remove(EnemiesOnScreen[i]);
                    }
                }
            }
            # endregion

            #region bullets and collision
            foreach (Rectangle x in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "BulletPlayer")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - bulletSpeed);
                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }

                    for (int i = 0; i < EnemiesOnScreen.Count; i++)
                    {
                        Rect enemy = new Rect(Canvas.GetLeft(EnemiesOnScreen[i].rectangle), Canvas.GetTop(EnemiesOnScreen[i].rectangle), EnemiesOnScreen[i].rectangle.Width, EnemiesOnScreen[i].rectangle.Height);
                        if (bullet.IntersectsWith(enemy))
                        {
                            itemsToRemove.Add(x); //bullet
                            itemsToRemove.Add(EnemiesOnScreen[i].rectangle); //enemy
                            if (EnemiesOnScreen[i].WhichSide == side.left)
                            {
                                scoreP1 = scoreP1 + EnemiesOnScreen[i].score;
                                ScoreP1.Content = scoreP1;
                            }
                            else if (EnemiesOnScreen[i].WhichSide == side.right)
                            {
                                scoreP2 = scoreP2 + EnemiesOnScreen[i].score;
                                ScoreP2.Content = scoreP2;
                            }
                            EnemiesOnScreen.Remove(EnemiesOnScreen[i]);
                        }
                    }
                }

                if ((string)x.Tag == "BulletEnemy")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + bulletSpeed);
                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    for (int i = 0; i < EnemiesOnScreen.Count; i++)
                    {
                        if (Canvas.GetLeft(x)>= 0 && Canvas.GetLeft(x) + x.Width <= Innerwall)
                        {
                            Rect player = new Rect(Canvas.GetLeft(Player1), Canvas.GetTop(Player1), Player1.Width, Player1.Height);
                            if (bullet.IntersectsWith(player))
                            {
                                scoreP1 = scoreP1 - (EnemiesOnScreen[i].score / 5);
                                if (scoreP1 < 0)
                                {
                                    scoreP1 = 0;
                                }
                                ScoreP1.Content = scoreP1;
                                itemsToRemove.Add(x); //bullet
                                break;
                            }
                        }
                        else if (Canvas.GetLeft(x) >= Innerwall && Canvas.GetLeft(x) + x.Width <= outerwall)
                        {
                            Rect player = new Rect(Canvas.GetLeft(Player2), Canvas.GetTop(Player2), Player2.Width, Player2.Height);
                            if (bullet.IntersectsWith(player))
                            {
                                scoreP2 = scoreP2 - (EnemiesOnScreen[i].score / 5);
                                if (scoreP2 < 0)
                                {
                                    scoreP2 = 0;
                                }
                                ScoreP2.Content = scoreP2;
                                itemsToRemove.Add(x); //bullet
                                break;
                            }
                        }
                    }
                }
            }

            #endregion

            foreach (Rectangle r in itemsToRemove)
            {
                //removes canvas items
                MyCanvas.Children.Remove(r);
            }
        }

        /// <summary>
        /// Deze functie/method zorgt voort het creeren van een nieuwe enemy class, waaronder ook de start locatie en de richting(als hij horizontaal beweegt) wordt randombepaald
        /// </summary>
        /// <param name="wallLeft">deze parameter zorgt voor de uiterste linker kant van de enemy waarin hij zich bevindt </param>
        /// <param name="wallRight">deze parameter zorgt voor de uiterste linker kant van de enemy waarin hij zich bevindt </param>
        /// <param name="enemytype">deze parameter zorgt voor het type enemy waaraan we kunnen if statements kunnen plakken zodat specifieke types spefieke dingen doen</param>
        /// <param name="side">deze parameter zorgt er voor dat de enemy de juiste score aan de juiste player geeft </param>
        public void CreateEnemy(int wallLeft, int wallRight, Enemytype enemytype, side side)
        {

            ImageBrush vijanden = new ImageBrush();
            switch (enemytype)
            {
                case Enemytype.Enemy1:
                    vijanden.ImageSource = new BitmapImage(new Uri("pack://application:,,,/plaatjes/enemy1.png"));
                    break;
                case Enemytype.Enemy2:
                    vijanden.ImageSource = new BitmapImage(new Uri("pack://application:,,,/plaatjes/enemy2.png"));
                    break;
                case Enemytype.Enemy3:
                    vijanden.ImageSource = new BitmapImage(new Uri("pack://application:,,,/plaatjes/enemy33.png"));
                    break;
                case Enemytype.Enemy4:
                    vijanden.ImageSource = new BitmapImage(new Uri("pack://application:,,,/plaatjes/enemy4.png"));
                    break;

            }

            //de rectangle van enemy wordt hier gemaakt 
            Rectangle newEnemy = new Rectangle
            {
                Tag = "Enemy",
                Height = 50,
                Width = 62,
                Fill = vijanden
            };

            //zet locatie van de rectangle en maakt het een kind van de canvas
            int width = (int)newEnemy.Width;
            Canvas.SetTop(newEnemy, rand.Next(-100, 0));
            Canvas.SetLeft(newEnemy, rand.Next(wallLeft, wallRight - width));
            MyCanvas.Children.Add(newEnemy);

            //hier maakt het een nieuwe class en vult hij de variabellen in
            Enemies enemy = new Enemies();
            enemy.rectangle = newEnemy;
            enemy.enemyType = enemytype;
            enemy.InWall = wallLeft;
            enemy.OutWall = wallRight;
            enemy.WhichSide = side;
            //randommiser die bepaalt welke kant de enemy opgaat
            int a = rand.Next(1, 3);
            if (a == 1)
            {
                enemy.ToLeft = true;
                enemy.ToRight = false;
            }
            else if (a == 2)
            {
                enemy.ToRight = true;
                enemy.ToLeft = false;
            }

            //zet de score en speed voor het type enemy en voegt toe aan de lijst van enemiesonscreen (zou eventueel via een database kunnen maar voor nu doe ik het hier)
            if(enemytype == Enemytype.Enemy1)
            {
                enemy.score = 100;
                enemy.speed = 10;
            }
            else if (enemytype == Enemytype.Enemy2)
            {
                enemy.score = 250;
                enemy.speed = 5;
                enemy.sidespeed = 10;
            }
            else if (enemytype == Enemytype.Enemy3)
            {
                enemy.score = 750;
                enemy.speed = 5;
            }
            else if (enemytype == Enemytype.Enemy4)
            {
                enemy.score = 2000;
                enemy.speed = 20;
            }
            EnemiesOnScreen.Add(enemy);

        }

        /// <summary>
        /// de bullet creation method die een nieuwe rectangle maakt in de canvas, van de schieters locatie.
        /// </summary>
        /// <param name="player"> dit is de parameter die bepaalt wie schiet en van welk punt</param>
        public void CreateBullet(Rectangle player)
        {
            if (player == Player1 || player == Player2)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "BulletPlayer",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };
                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
                MyCanvas.Children.Add(newBullet);
            }
            else
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "BulletEnemy",
                    Height = 20,
                    Width = 5,  
                    Fill = Brushes.White,
                    Stroke = Brushes.Green
                };
                Canvas.SetTop(newBullet, Canvas.GetTop(player) + player.Height + newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
                MyCanvas.Children.Add(newBullet);
                
            }

        }

    }
}
