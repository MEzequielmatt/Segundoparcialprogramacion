using System;
using System.Collections.Generic;
using System.Media;
using Tao.Sdl;

namespace MyGame
{
    class Program
    {
        static Image player = Engine.LoadImage("C:\\Users\\MMeze\\OneDrive\\Desktop\\PROGRAMACION\\Proyecto Base\\assets\\player.png");
        static Image enemyImage = Engine.LoadImage("C:\\Users\\MMeze\\OneDrive\\Desktop\\PROGRAMACION\\Proyecto Base\\assets\\enemy.png");
        static Image trackingEnemyImage = Engine.LoadImage("C:\\Users\\MMeze\\OneDrive\\Desktop\\PROGRAMACION\\Proyecto Base\\assets\\tracking_enemy.png");
        static Image obstacleImage = Engine.LoadImage("C:\\Users\\MMeze\\OneDrive\\Desktop\\PROGRAMACION\\Proyecto Base\\assets\\obstacle.png");
        static Image coinImage = Engine.LoadImage("C:\\Users\\MMeze\\OneDrive\\Desktop\\PROGRAMACION\\Proyecto Base\\assets\\coin.png");
        static Font fuente;
        static SoundPlayer backgroundMusic;
        static Image menuBackground = Engine.LoadImage("C:\\Users\\MMeze\\OneDrive\\Desktop\\PROGRAMACION\\Proyecto Base\\assets\\menu_background.png");
        static Image gameBackground = Engine.LoadImage("C:\\Users\\MMeze\\OneDrive\\Desktop\\PROGRAMACION\\Proyecto Base\\assets\\game_background.png");


        static int xplayer = 100;
        static int yplayer = 100;
        static int coinCounter = 0;
        static int level = 1;

        static bool playerVisible = false;
        static bool startClicked = false;
        static bool isMouseOverStart = false;
        static bool isMouseOverExit = false;

        static List<Obstacle> obstacles = new List<Obstacle>
        {
            new Obstacle(200, 300),
            new Obstacle(400, 200),
            new Obstacle(600, 400)
        };

        static Coin coin = new Coin(300, 250);
        static List<Enemy> enemies = new List<Enemy>();

        static void Main(string[] args)
        {
            Engine.Initialize();
            
            fuente = Engine.LoadFont("C:\\Users\\MMeze\\OneDrive\\Desktop\\PROGRAMACION\\Proyecto Base\\assets\\Super Shiny.ttf", 80);
            enemies.Add(new Enemy(500, 100, enemyImage));
            backgroundMusic = new SoundPlayer("C:\\Users\\MMeze\\OneDrive\\Desktop\\PROGRAMACION\\Proyecto Base\\assets\\1.wav");
            backgroundMusic.PlayLooping();
            while (true)
            {
                CheckInputs();
                Update();
                Render();
                Sdl.SDL_Delay(20);
            }
        }

        static void CheckInputs()
        {
            int mouseX, mouseY;
            uint mouseButtons = Sdl.SDL_GetMouseState(out mouseX, out mouseY);

            isMouseOverStart = mouseX >= 350 && mouseX <= 550 && mouseY >= 400 && mouseY <= 480;
            isMouseOverExit = mouseX >= 350 && mouseX <= 550 && mouseY >= 500 && mouseY <= 580;

            if (isMouseOverStart && (mouseButtons & Sdl.SDL_BUTTON(1)) != 0)
            {
                startClicked = true;
                playerVisible = true;
                coinCounter = 0;
                level = 0;
            }

            if (isMouseOverExit && (mouseButtons & Sdl.SDL_BUTTON(1)) != 0)
                Environment.Exit(0);

            int playerSpeed = 5;
            int playerWidth = 50;
            int playerHeight = 50;

            if (Engine.KeyPress(Engine.KEY_A) && xplayer - playerSpeed >= 0 && !CheckCollision(xplayer - playerSpeed, yplayer))
                xplayer -= playerSpeed;
            if (Engine.KeyPress(Engine.KEY_D) && xplayer + playerWidth + playerSpeed <= 800 && !CheckCollision(xplayer + playerSpeed, yplayer))
                xplayer += playerSpeed;
            if (Engine.KeyPress(Engine.KEY_W) && yplayer - playerSpeed >= 0 && !CheckCollision(xplayer, yplayer - playerSpeed))
                yplayer -= playerSpeed;
            if (Engine.KeyPress(Engine.KEY_S) && yplayer + playerHeight + playerSpeed <= 600 && !CheckCollision(xplayer, yplayer + playerSpeed))
                yplayer += playerSpeed;
        }

        static bool CheckCollision(int newX, int newY)
        {
            int playerDeltaX = newX - xplayer;
            int playerDeltaY = newY - yplayer;

            foreach (var obstacle in obstacles)
            {
                if (newX + 50 > obstacle.X && newX < obstacle.X + 50 &&
                    newY + 50 > obstacle.Y && newY < obstacle.Y + 50)
                {
                    obstacle.Push(playerDeltaX, playerDeltaY);
                    return true;
                }
            }

            if (!coin.Collected && newX + 50 > coin.X && newX < coin.X + 50 &&
                newY + 50 > coin.Y && newY < coin.Y + 50)
            {
                coin.Collected = true;
                coinCounter++;
                level++;
                GenerateNewCoin();
                GenerateRandomEnemy();
                GenerateRandomObstacle();

                if (level % 3 == 0)
                {
                    GenerateTrackingEnemy();
                }

                return true;
            }

            return false;
        }

        static void GenerateNewCoin()
        {
            Random random = new Random();
            bool validPosition;

            do
            {
                coin.X = random.Next(0, 750);
                coin.Y = random.Next(0, 550);
                coin.Collected = false;

                validPosition = true;
                foreach (var obstacle in obstacles)
                {
                    if (Math.Abs(coin.X - obstacle.X) < 50 && Math.Abs(coin.Y - obstacle.Y) < 50)
                    {
                        validPosition = false;
                        break;
                    }
                }
            } while (!validPosition);
        }

        static void GenerateRandomEnemy()
        {
            Random random = new Random();
            int x = random.Next(0, 750);
            int y = random.Next(0, 550);
            enemies.Add(new Enemy(x, y, enemyImage));
        }

        static void GenerateTrackingEnemy()
        {
            Random random = new Random();
            int x = random.Next(0, 750);
            int y = random.Next(0, 550);
            enemies.Add(new TrackingEnemy(x, y, trackingEnemyImage));
        }

        static void GenerateRandomObstacle()
        {
            Random random = new Random();
            foreach (var obstacle in obstacles)
            {
                bool validPosition;
                do
                {
                    obstacle.X = random.Next(0, 750);
                    obstacle.Y = random.Next(0, 550);

                    validPosition = true;
                    if (Math.Abs(obstacle.X - coin.X) < 50 && Math.Abs(obstacle.Y - coin.Y) < 50)
                    {
                        validPosition = false;
                    }
                } while (!validPosition);
            }
        }

        static void CheckObstaclePositionWithPlayer()
        {
            Random random = new Random();
            foreach (var obstacle in obstacles)
            {
                if (Math.Abs(obstacle.X - xplayer) < 50 && Math.Abs(obstacle.Y - yplayer) < 50)
                {
                    do
                    {
                        obstacle.X = random.Next(0, 750);
                        obstacle.Y = random.Next(0, 550);
                    } while (Math.Abs(obstacle.X - xplayer) < 50 && Math.Abs(obstacle.Y - yplayer) < 50);
                }
            }
        }

        static void Update()
        {
            CheckObstaclePositionWithPlayer();

            if (playerVisible)
            {
                foreach (var enemy in enemies)
                {
                    enemy.UpdatePosition(xplayer, yplayer);  

                    if (Math.Abs(xplayer - enemy.X) < 50 && Math.Abs(yplayer - enemy.Y) < 50)
                    {
                        Engine.Clear();
                        Engine.DrawText("GAME OVER", 350, 400, 255, 0, 0, fuente);
                        Engine.Show();
                        Sdl.SDL_Delay(2000);
                        Environment.Exit(0);
                    }
                }

                if (coinCounter >= 9)
                {
                    Engine.Clear();
                    Engine.DrawText("¡Has ganado!", 350, 400, 0, 255, 0, fuente);
                    Engine.Show();
                    Sdl.SDL_Delay(2000);
                    Environment.Exit(0);
                }
            }
        }

        static void Render()
        {
            Engine.Clear();

            if (!startClicked)
            {
                Engine.Draw(menuBackground, 0, 0);
                Engine.DrawText("Inicio", 350, 400, (byte)(isMouseOverStart ? 0 : 100), (byte)100, (byte)(isMouseOverStart ? 255 : 100), fuente);
                Engine.DrawText("Salir", 350, 500, (byte)(isMouseOverExit ? 255 : 100), (byte)100, (byte)(isMouseOverExit ? 0 : 100), fuente);
            }
            else
            {
                Engine.Draw(gameBackground, 0, 0);
                foreach (var obstacle in obstacles)
                    Engine.Draw(obstacle.Image, obstacle.X, obstacle.Y);

                if (!coin.Collected)
                    Engine.Draw(coin.Image, coin.X, coin.Y);

                Engine.Draw(player, xplayer, yplayer);

                foreach (var enemy in enemies)
                    Engine.Draw(enemy.Image, enemy.X, enemy.Y);

                Engine.DrawText("Monedas: " + coinCounter, 10, 10, 255, 255, 0, fuente);
                Engine.DrawText("Nivel: " + level, 500, 0, 255, 255, 0, fuente);
            }

            Engine.Show();
        }
    }

    class Obstacle
    {
        public int X, Y;
        public Image Image;

        public Obstacle(int x, int y)
        {
            X = x;
            Y = y;
            Image = Engine.LoadImage("C:\\Users\\MMeze\\OneDrive\\Desktop\\PROGRAMACION\\Proyecto Base\\assets\\obstacle.png");
        }

        public void Push(int deltaX, int deltaY)
        {
            int pushDistance = 10;
            X += (deltaX > 0 ? pushDistance : (deltaX < 0 ? -pushDistance : 0));
            Y += (deltaY > 0 ? pushDistance : (deltaY < 0 ? -pushDistance : 0));
        }
    }

    class Coin
    {
        public int X, Y;
        public Image Image;
        public bool Collected;

        public Coin(int x, int y)
        {
            X = x;
            Y = y;
            Collected = false;
            Image = Engine.LoadImage("C:\\Users\\MMeze\\OneDrive\\Desktop\\PROGRAMACION\\Proyecto Base\\assets\\coin.png");
        }
    }

    class Enemy
    {
        public int X, Y;
        public Image Image;
        private int speedX, speedY;

        public Enemy(int x, int y, Image image)
        {
            X = x;
            Y = y;
            Image = image;
            speedX = new Random().Next(1, 5);
            speedY = new Random().Next(1, 5);
        }

        public virtual void UpdatePosition(int playerX, int playerY)
        {
            X += speedX;
            Y += speedY;

            if (X <= 0 || X >= 750) speedX = -speedX;
            if (Y <= 0 || Y >= 550) speedY = -speedY;
        }
    }

    class TrackingEnemy : Enemy
    {
        public TrackingEnemy(int x, int y, Image image) : base(x, y, image) { }

        public override void UpdatePosition(int playerX, int playerY)
        {
            int speed = 2;
            if (X < playerX) X += speed;
            if (X > playerX) X -= speed;
            if (Y < playerY) Y += speed;
            if (Y > playerY) Y -= speed;
        }
    }
}
