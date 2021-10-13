using System;
using System.Collections.Generic;
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

namespace ArcadeGameProject
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        # region Variabelen
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private List<Rectangle> itemsToRemove = new List<Rectangle>();
        private Random rand = new Random();
        private List<Enemies> EnemiesOnScreen = new List<Enemies>();

        public int Innerwall = 640;
        public int outerwall = 1280;
        public int EnemySpeed = 10;
        private int enemySpawnCounter = 50;
        private int EnemySpawnLimit = 50;
        private const double cHeight = 960;
        private int Time;
        private const int playerSpeed = 10;
        private const int bulletSpeed = 20;
        private int scoreP1=0;
        private int scoreP2=0;

        private bool moveLeft1, moveRight1, moveLeft2, moveRight2;
        # endregion

        public GameWindow()
        {
            InitializeComponent();
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
        }

        public void GameEngine(object sender, EventArgs e)
        {
            #region movement players
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

            #region enemiesmovement & spawning
            // de Time variable zorgt voor een nummer die de tijd kan weergeven in een vorm en dus een if statement mee gedaan kan worden
            Time++;
            enemySpawnCounter--;
            if (enemySpawnCounter < 0)
            {
                CreateEnemy(0, Innerwall, Enemytype.Enemy1, side.left); //make enemies for player1
                CreateEnemy(Innerwall, outerwall, Enemytype.Enemy1, side.right); //makes enemies for player 2
                enemySpawnCounter = EnemySpawnLimit; //reset the enemy counter to the limit integer
                if(Time >= 500)
                {
                    CreateEnemy(0, Innerwall, Enemytype.Enemy2, side.left); //make enemies for player1
                    CreateEnemy(Innerwall, outerwall, Enemytype.Enemy2, side.right); //makes enemies for player 2
                }
            }
            //de reden voor de for-loop inplaats van de foreach is omdat de count van de enemiesonscreen list verandert in de loop, door dat er wat wordt verwijderd, wat een foreach niet aan kan en een for loop wel 
            for(int i = 0; i< EnemiesOnScreen.Count; i++)
            {
                Canvas.SetTop(EnemiesOnScreen[i].rectangle, Canvas.GetTop(EnemiesOnScreen[i].rectangle) + EnemySpeed);

                //checkt of enemy type 2 is en dus de horizontale movement bepaald
                if (EnemiesOnScreen[i].enemyType == Enemytype.Enemy2)
                {
                    if (Canvas.GetLeft(EnemiesOnScreen[i].rectangle) >= EnemiesOnScreen[i].InWall && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) <= EnemiesOnScreen[i].InWall + EnemySpeed)
                    {
                        EnemiesOnScreen[i].ToRight = true;
                        EnemiesOnScreen[i].ToLeft = false;
                        Canvas.SetLeft(EnemiesOnScreen[i].rectangle, Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemySpeed);
                    }
                    else if (Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].rectangle.Width <= EnemiesOnScreen[i].OutWall && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].rectangle.Width >= EnemiesOnScreen[i].OutWall - EnemySpeed)
                    {
                        EnemiesOnScreen[i].ToRight = false;
                        EnemiesOnScreen[i].ToLeft = true;
                        Canvas.SetLeft(EnemiesOnScreen[i].rectangle, Canvas.GetLeft(EnemiesOnScreen[i].rectangle) - EnemySpeed);
                    }
                    else if (EnemiesOnScreen[i].ToRight == true && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].rectangle.Width <= EnemiesOnScreen[i].OutWall - EnemySpeed && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) >= EnemiesOnScreen[i].InWall + EnemySpeed)
                    {
                        Canvas.SetLeft(EnemiesOnScreen[i].rectangle, Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemySpeed);
                    }
                    else if (EnemiesOnScreen[i].ToLeft == true && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].rectangle.Width <= EnemiesOnScreen[i].OutWall - EnemySpeed && Canvas.GetLeft(EnemiesOnScreen[i].rectangle) >= EnemiesOnScreen[i].InWall + EnemySpeed)
                    {
                        Canvas.SetLeft(EnemiesOnScreen[i].rectangle, Canvas.GetLeft(EnemiesOnScreen[i].rectangle) - EnemySpeed);
                    }
                }

                //removal van de enemies met het optellen van punten
                if (Canvas.GetTop(EnemiesOnScreen[i].rectangle) + EnemiesOnScreen[i].rectangle.Height > cHeight)
                {
                    itemsToRemove.Add(EnemiesOnScreen[i].rectangle);
                    if (EnemiesOnScreen[i].WhichSide == side.left)
                    {
                        scoreP1 = scoreP1 - (EnemiesOnScreen[i].score / 2);
                        ScoreP1.Content = scoreP1;
                    }
                    else if (EnemiesOnScreen[i].WhichSide == side.right)
                    {
                        scoreP2 = scoreP2 - (EnemiesOnScreen[i].score/2);
                        ScoreP2.Content = scoreP2;
                    }
                    // de reden waarom we remove doen in de list is omdat de score dan gaat glitchen en niet meer correct doet
                    EnemiesOnScreen.Remove(EnemiesOnScreen[i]);
                }
            }
            # endregion

            #region bullets and collision
            foreach (Rectangle x in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "Bullet")
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
            }

            #endregion

            foreach (Rectangle r in itemsToRemove)
            {
                //removes canvas items
                MyCanvas.Children.Remove(r);
            }
        }

        //maakt een nieuwe enemy script aan om daar in de waardes in te bewaren voor locatie, type, welke spelers kant de enemy zit
        public void CreateEnemy(int wallLeft, int wallRight, Enemytype enemytype, side side)
        {
            
            
            int a = rand.Next(1, 3);
           
            ImageBrush vijanden = new ImageBrush();
            switch (enemytype)
            {
                case Enemytype.Enemy1:
                    vijanden.ImageSource = new BitmapImage(new Uri("pack://application:,,,/plaatjes/player3.png"));
                    break;
                case Enemytype.Enemy2:
                    vijanden.ImageSource = new BitmapImage(new Uri("pack://application:,,,/plaatjes/enemy2.png"));
                    break;

            }
            //de rectangle van enemy wordt hier gemaakt en zorgt ervoor dat het plaatje van de enemy er in staat
            Rectangle newEnemy = new Rectangle
            {
                Tag = "Enemy",
                Height = 40,
                Width = 50,
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

            //zet de score voor het type enemy en voegt toe aan de lijst van enemiesonscreen
            if (enemytype == Enemytype.Enemy1)
            {
                enemy.score = 100;
            }
            else if (enemytype == Enemytype.Enemy2)
            {
                enemy.score = 250;
              
            }
            EnemiesOnScreen.Add(enemy);


        }

        //maakt een bullet die afhankelijk is van de positie speler die shiet
        public void CreateBullet(Rectangle player)
        {
            Rectangle newBullet = new Rectangle
            {
                Tag = "Bullet",
                Height = 20,
                Width = 5,
                Fill = Brushes.White,
                Stroke = Brushes.Red
            };
            Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
            Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
            MyCanvas.Children.Add(newBullet);
        }
    }
}
