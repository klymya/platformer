using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Media;
using System.Diagnostics;

using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform;
using Tao.DevIl;
using Tao.Sdl;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace kyrsovykh
{
    struct TScore
    {
        public string name;
        public uint score; 
    };

    class Program
    {
        //таймер игры
        static Stopwatch time = new Stopwatch();
        static int maxNumOfScore = 7;
        static TScore currScore = new TScore();
        static string scoreFile = @"materials\Score.txt";
        static void SaveScore(TScore s)
        {
            TimeSpan t = time.Elapsed;

            //TScore s = new TScore();
            //s.name = str;
            //s.score = (uint)(1000 / t.Seconds) * (uint)Math.Max((sbyte)1, player1.life) * player1.coins;

            if (!File.Exists(scoreFile))
            {
                StreamWriter outF = new StreamWriter(scoreFile);
                outF.WriteLine(1);
                outF.WriteLine(s.name + " " + Convert.ToString(s.score));
                outF.Close();
                return ;
            }

            StreamReader inf = new StreamReader(scoreFile);
            int n = Convert.ToInt32(inf.ReadLine());

            TScore[] tmp = new TScore[n + 1];
            for (int i = 0; i < n; i++)
            {
                tmp[i] = new TScore();
                string curr = inf.ReadLine();
                string[] buf = curr.Split(new Char[] { ' ' });
                tmp[i].name = buf[0];
                tmp[i].score = Convert.ToUInt32(buf[1]);
            }
            inf.Close();
            tmp[n] = s;
            //сортируем
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (tmp[j].score < tmp[j + 1].score)
                    {
                        TScore buf = tmp[j];
                        tmp[j] = tmp[j + 1];
                        tmp[j + 1] = buf;
                    }
                }
            }

            //презаписываем
            StreamWriter outf = new StreamWriter(scoreFile);
            outf.WriteLine(Math.Min(maxNumOfScore, n + 1));
            for (int i = 0; i < Math.Min(maxNumOfScore, n + 1); i++)
            {
                outf.WriteLine(tmp[i].name + " " + Convert.ToString(tmp[i].score));
            }                
            outf.Close();
            
        }
        static uint bonus = 0;
        //чтение с клавиатуры
        static int N_key = 256;
        static bool[] keys = new bool[N_key];
        static char Read()
        {
            char ch = (char)0;
            for (int i = 0; i < N_key; i++)
            {
                if (keys[i])
                {
                    ch = Convert.ToChar(i);
                    keys[i] = false;
                    break;
                }
            }

            if (ch >= '0' && ch <= '}')//(ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z')
            {
                buffer += Convert.ToString(ch);
            }

            return ch;
        }
        //static string name;
        static char s;
        static string buffer;

        //размеры окна
        static int WIDTH = 21 * Tile.Size;
        static int HEIGHT = 12 * Tile.Size;
        // координаты для камеры
        static float X = 0.0f;
        static float Y = 0.0f;

        //флажки состояния игры
        enum EnumStateOfGame
        {
            BEGINNING,
            PAUSE,
            GAME,
            END,
            DEATH,
            SCORE,
            TOP_SCORE
        }
        static EnumStateOfGame gameState = EnumStateOfGame.BEGINNING;

        //флажки состояния меню
        enum EnumStateofMenu
        {
            NEW_GAME,
            QUIT,
            SCORE,
            TOP_SCORE
        }
        static EnumStateofMenu menuState = EnumStateofMenu.NEW_GAME;

        //число уровней
        static int N_levels = 2;
        //загрузкак уровней
        static Map[] initLevels(int n, ref ArrayList[] arrayOfCoins, ref ArrayList[] arrayOfEnemy, uint[] textures, int textureCount)
        {
            Map[] levels = new Map[n];
            for (int i = 0; i < n; i++)
            {
                levels[i] = new Map(@"materials\levels\" + (i + 1) + ".txt", textures, textureCount);
                arrayOfCoins[i] = new ArrayList();
                arrayOfEnemy[i] = new ArrayList();
                levels[i].LoadMap(ref arrayOfCoins[i],ref arrayOfEnemy[i], texNormalEnemy, countNormalEnemy, texEasyEnemy, countEasyEnemy);
            }
            return levels;
        }
        static Map[] levels;
        static int k = 0;

        //загрузка текстур
        static int textureCount = 13; 
        static uint[] textures;
        static void loadTexture(ref uint[] textures, int textureCount)
        {
            textures = new uint[textureCount];
            Bitmap bitmap = kyrsovykh.Properties.Resources.stonebricksmooth_mossy;
            BitmapData bitmapData;

            for (int i = 0; i < textureCount; i++)
            {
                Gl.glGenTextures(1, out textures[i]);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[i]);

                switch (i)
                {
                    case (int)EnumStates.NOTHING: bitmap = kyrsovykh.Properties.Resources.stonebricksmooth_mossy; break;
                    case (int)EnumStates.BLOCK: bitmap = kyrsovykh.Properties.Resources.brick; break;
                    case (int)EnumStates.PLATFORM: bitmap = kyrsovykh.Properties.Resources.wood; break;
                    case (int)EnumStates.TRAP: bitmap = kyrsovykh.Properties.Resources.furnace_front_lit; break;
                    case (int)EnumStates.END_LOWER: bitmap = kyrsovykh.Properties.Resources.doorWood_lower; break;
                    case (int)EnumStates.END_UPPER: bitmap = kyrsovykh.Properties.Resources.doorWood_upper; break;
                    case (int)EnumStates.COIN: bitmap = kyrsovykh.Properties.Resources.ingotGold; break;
                    case 7: bitmap = kyrsovykh.Properties.Resources.wasted_d76097158; break;//смерть
                    case 8: bitmap = kyrsovykh.Properties.Resources.media_pause; break;//пауза
                    case 9: bitmap = kyrsovykh.Properties.Resources.end; break;//конец игры
                    case 10: bitmap = kyrsovykh.Properties.Resources.New_game_Button; break;//new game
                    case 11: bitmap = kyrsovykh.Properties.Resources.Quit_Game_Button; break;//quit
                    case 12: bitmap = kyrsovykh.Properties.Resources.Score_Button; break;//score
                }

                //bitmap = kyrsovykh.Properties.Resources.stonebricksmooth_mossy;
                var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                bitmapData = bitmap.LockBits(
                    rect,
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, bitmapData.Width, bitmapData.Height, 0,
                                Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
                bitmap.UnlockBits(bitmapData);
            }
        }
        static void loadTexture(ref uint[] textures, int textureCount, string way)
        {
            textures = new uint[textureCount];
            Bitmap bitmap;// = kyrsovykh.Properties.Resources.stonebricksmooth_mossy;
            BitmapData bitmapData;

            for (int i = 0; i < textureCount; i++)
            {
                Gl.glGenTextures(1, out textures[i]);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[i]);

                string path = way + i + ".png";
                if (!File.Exists(path))
                {
                    throw new FileReadException(path);
                }

                bitmap = new Bitmap(path, true);

                //bitmap = kyrsovykh.Properties.Resources.stonebricksmooth_mossy;
                var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                bitmapData = bitmap.LockBits(
                    rect,
                    ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, bitmapData.Width, bitmapData.Height, 0,
                                Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP);
                bitmap.UnlockBits(bitmapData);
            }
        }
        static uint[] texNormalEnemy;
        static int countNormalEnemy = 4;
        static uint[] texEasyEnemy;
        static int countEasyEnemy = 6;
        static uint[] texPlayer;
        static int countPlayer = 19;


        static ArrayList arrayOfCoins = new ArrayList();
        static ArrayList arrayOfEnemy = new ArrayList();
        static ArrayList[] coins = new ArrayList[N_levels];
        static ArrayList[] enemies = new ArrayList[N_levels];
        static Player player1;// = new Player((int)myMap.begin.X, (int)myMap.begin.Y);

        static void DrawString(double x, double y, string str)
        {
            if (str == null)
            {
                return ;
            }
            Gl.glColor3f(1.0f, 1.0f, 1.0f);
            for (int i = 0; i < str.Length; i++)
            {
                Gl.glRasterPos2d(x + i * 14, y);
                Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_HELVETICA_18, str[i]);

            }
        }
        //отрисовка игры
        static void DrawGame()
        {
            if (levels[k].ending)
            {
                k++;
                bonus += 150;
                if (k < N_levels)
                {
                    player1.ReturnVectors();
                    player1.X = (int)levels[k].begin.X;
                    player1.Y = (int)levels[k].begin.Y;
                    player1.life = 100;
                    X = 0;
                    Y = 0;
                }
                else
                {
                    //Environment.Exit(0);
                    gameState = EnumStateOfGame.END;
                    return ;
                }

            }


            //myMap.Draw();
            levels[k].Draw();
            foreach (Coin i in coins[k])//arrayOfCoins)
            {
                i.Draw(levels[k]);
            }

            foreach (Enemy i in enemies[k])//arrayOfEnemy)
            {
                if (i.life == 0) continue;
                //if (i is NormalEnemy)
                //{
                //    ((NormalEnemy)i).MoveEnemy(levels[k], player1.X, player1.Y);
                //}
                //else
                //{
                //    i.MoveEnemy(levels[k]);
                //}
                //i.HandlerMovement();
                i.DrawC(levels[k]);
            }

            DrawString((int)X + 50, (int)Y + 700, "LIFE:+" + Convert.ToString(player1.life));
            DrawString((int)X + 50, (int)Y + 675, "COINS:" + Convert.ToString(player1.coins));
            //TimeSpan tmp = time.Elapsed;
            //DrawString((int)X + 50, (int)Y + 6, "TIME:" + Convert.ToString(tmp.Seconds) + "." + Convert.ToString(tmp.Milliseconds));

            //player1.HandlerMovementWithHit(ref enemies[k], ref coins[k]);//(ref arrayOfEnemy, ref arrayOfCoins);
            player1.Draw(levels[k], coins[k], enemies[k]);//(myMap, arrayOfCoins, arrayOfEnemy);

            //пережвижение камеры
            //X
            if (Math.Abs(X - player1.X + WIDTH) < WIDTH / 2 && X + WIDTH < levels[k].Width * Tile.Size)
            {
                while (Math.Abs(X - player1.X + WIDTH) < WIDTH / 2 && X + WIDTH < levels[k].Width * Tile.Size)
                {
                    X += 3;
                }
            }
            if (Math.Abs(X - player1.X) < WIDTH / 2 && X > 0)
            {
                while (Math.Abs(X - player1.X) < WIDTH / 2 && X > 0)
                {
                    X -= 3;
                }
            }

            if (player1.life <= 0 && gameState == EnumStateOfGame.GAME)
            {
                //Environment.Exit(0);
                player1.ZeroingCoord();
                gameState = EnumStateOfGame.DEATH;
                //player1.ZeroingCoord();
            }
        }
        static void MoveInGame()
        {
            foreach (Enemy i in enemies[k])//arrayOfEnemy)
            {
                if (i.life == 0) continue;
                if (i is NormalEnemy)
                {
                    ((NormalEnemy)i).MoveEnemy(levels[k], player1.X, player1.Y);
                }
                else
                {
                    i.MoveEnemy(levels[k]);
                }
                i.HandlerMovement();
                //i.DrawC(levels[k]);
            }

            player1.HandlerMovementWithHit(ref enemies[k], ref coins[k]);//(ref arrayOfEnemy, ref arrayOfCoins);
        }
        //отрисовка разных меню
        static void DrawPause()
        {
            //Gl.glColor3f(0.7f, 0.7f, 0.7f);
            //Gl.glBegin(Gl.GL_QUADS);
            //    Gl.glVertex2f(X, Y);
            //    Gl.glVertex2f(X, Y + HEIGHT);
            //    Gl.glVertex2f(X + WIDTH, Y + HEIGHT);
            //    Gl.glVertex2f(X + WIDTH, Y);
            //Gl.glEnd();
            //levels[k].Draw();
            DrawGame();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[8]);
            Tile.DrawTextQuad(X + WIDTH / 2.5, Y + HEIGHT / 3, Tile.Size * 5, Tile.Size * 5);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }
        static void DrawDeath()
        {
            //Gl.glColor3f(0.7f, 0.7f, 0.7f);
            //Gl.glBegin(Gl.GL_QUADS);
            //    Gl.glVertex2f(X, Y);
            //    Gl.glVertex2f(X, Y + HEIGHT);
            //    Gl.glVertex2f(X + WIDTH, Y + HEIGHT);
            //    Gl.glVertex2f(X + WIDTH, Y);
            //Gl.glEnd();
            DrawGame();
            //levels[k].Draw();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[7]);
            Tile.DrawTextQuad(X + WIDTH / 3, Y + HEIGHT / 3, Tile.Size * 7, Tile.Size * 7 );
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }
        static void DrawEnd()
        {
            levels[k - 1].Draw();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[9]);
            Tile.DrawTextQuad(X + WIDTH / 2.7, Y + HEIGHT / 2, Tile.Size * 6, Tile.Size * 3);
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }
        static void DrawBegin()
        {
            k = 0;
            X = 0;
            Y = 0;
            levels[k].Draw();

            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[10]);
            if (menuState == EnumStateofMenu.NEW_GAME)
            {
                Tile.DrawTextQuad(X + WIDTH / 2.8, Y + HEIGHT / 1.5, Tile.Size * 6, Tile.Size * 1.5, r: 0.2f, g: 0.7f, b: 1.0f);
            }
            else
            {
                Tile.DrawTextQuad(X + WIDTH / 2.8, Y + HEIGHT / 1.5, Tile.Size * 6, Tile.Size * 1.5);
            }                
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[11]);
            if (menuState == EnumStateofMenu.QUIT)
            {
                Tile.DrawTextQuad(X + WIDTH / 2.8, Y + HEIGHT / 2, Tile.Size * 6, Tile.Size * 1.5, r: 0.2f, g: 0.7f, b: 1.0f);
            }
            else
            {
                Tile.DrawTextQuad(X + WIDTH / 2.8, Y + HEIGHT / 2, Tile.Size * 6, Tile.Size * 1.5);
            }
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[12]);
            if (menuState == EnumStateofMenu.SCORE)
            {
                Tile.DrawTextQuad(X + WIDTH / 2.8, Y + HEIGHT / 3.2, Tile.Size * 6, Tile.Size * 1.5, r: 0.2f, g: 0.7f, b: 1.0f);
            }
            else
            {
                Tile.DrawTextQuad(X + WIDTH / 2.8, Y + HEIGHT / 3.2, Tile.Size * 6, Tile.Size * 1.5);
            }          
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }
        static void DrawScore()
        {
            if (k >= N_levels)
            {
                k = N_levels - 1;
            }
            levels[k].Draw();

            TimeSpan tmp = time.Elapsed;
            currScore.score = (uint)(10 * player1.coins * player1.coins / tmp.Seconds) * (uint)Math.Max((sbyte)2, player1.life) / 2 + bonus;
            DrawString((int)X + 500, (int)Y + 540, "Your score- " +  Convert.ToString(currScore.score));
            DrawString((int)X + 500, (int)Y + 500, "Enter name _ ");
            s = Read();
            //name += Convert.ToString(s);
            DrawString((int)X + 500, (int)Y + 450, buffer);            
        }
        static void DrawTopScore()
        {
            levels[k].Draw();
            DrawString((int)X + WIDTH / 4, (int)Y + HEIGHT / 1.5, "TOP SCORE:");
            if (File.Exists(scoreFile))
            {
                
                StreamReader inf = new StreamReader(scoreFile);
                int n = Convert.ToInt32(inf.ReadLine());
                
                for (int i = 0; i < n; i++)
                {
                    DrawString((int)X + WIDTH / 4, (int)Y + HEIGHT / 1.7 - i * 25, Convert.ToString(i + 1) + ")" + inf.ReadLine());
                }
                inf.Close();
            }
        }


        //Инициализация 
        static void init()
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGBA | Glut.GLUT_DOUBLE);//Glut.GLUT_SINGLE
            Glut.glutInitWindowSize(WIDTH, HEIGHT);
            Glut.glutInitWindowPosition(0, 0);
            Glut.glutCreateWindow("Kursovykh");

            Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            Gl.glShadeModel(Gl.GL_FLAT);


            //myMap.LoadMap(ref arrayOfCoins, ref arrayOfEnemy);
            loadTexture(ref texNormalEnemy, countNormalEnemy, @"materials\sprites\superAndroid\android");
            loadTexture(ref texEasyEnemy, countEasyEnemy, @"materials\sprites\jelly\jelly");
            loadTexture(ref texPlayer, countPlayer, @"materials\sprites\aquamen\aquamen");

            loadTexture(ref textures, textureCount);
            levels = initLevels(N_levels, ref coins, ref enemies, textures, textureCount);
            player1 = new Player((int)levels[0].begin.X, (int)levels[0].begin.Y, texPlayer, countPlayer);
        }
        //Изменение размеров окна 
        static void reshape(int w, int h)
        {
            Gl.glViewport(0, 0, w, h);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            ////Gl.glFrustum(-2.0, 2.0, -1.0, 1.0, 1.5, 25.0);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
        } 
        //отображение
        static void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            //Очистить матрицу 
            Gl.glLoadIdentity();

            //Gl.glEnable(Gl.GL_DEPTH_TEST);

            Gl.glEnable(Gl.GL_ALPHA_TEST);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Glu.gluOrtho2D(0.0, WIDTH, 0.0, HEIGHT);
            Glu.gluLookAt(X , Y, 0, X , Y , -10.0, 0.0, 1.0, 0.0);

            switch (gameState)
            {
                case EnumStateOfGame.GAME:
                    //MoveInGame(); 
                    DrawGame();
                    time.Start();
                    if (gameState != EnumStateOfGame.END)
                    {
                        MoveInGame();
                    }
                    // 
                    break;
                case EnumStateOfGame.PAUSE:
                    DrawPause();
                    time.Stop();
                    break;
                case EnumStateOfGame.DEATH:
                    DrawDeath();
                    time.Stop();
                    break;
                case EnumStateOfGame.END:
                    DrawEnd();
                     time.Stop();
                     break;
                case EnumStateOfGame.BEGINNING:
                     DrawBegin();
                    time.Restart();
                    break;
                case EnumStateOfGame.SCORE: DrawScore(); break;
                case EnumStateOfGame.TOP_SCORE: DrawTopScore(); break;
            }

            Gl.glDisable(Gl.GL_ALPHA_TEST);
            Gl.glDisable(Gl.GL_BLEND);
            //Glut.glutPostRedisplay(); 
            Gl.glFlush();
            Glut.glutSwapBuffers(); // перемикання буферів
        }

        private static void specialKey(int key, int x, int y)
        {
            switch (key)
            {
                case Glut.GLUT_KEY_LEFT:  // Rotate on x axis
                    X -= 10;
                    break;
                case Glut.GLUT_KEY_RIGHT:    // Rotate on x axis (opposite)
                    X += 10;
                    break;
                case Glut.GLUT_KEY_UP:       // Rotate on y axis 
                    Y += 10;
                    break;
                case Glut.GLUT_KEY_DOWN:     // Rotate on y axis (opposite)
                    Y -= 10;
                    break;
            }
            Glut.glutPostRedisplay();      // Redraw the scene
        }

        public static void pressKey(byte key, int x, int y)
        {
            keys[key] = true;

            switch (key)
            {
                case 8:
                    if (buffer.Length >= 1)
                    {
                        buffer = buffer.Remove(buffer.Length - 1, 1);
                    }
                    break;
                case (byte)'d':
                case (byte)'D': player1.moveRight = true; break;
                case (byte)'a':
                case (byte)'A': player1.moveLeft = true; break;
                case (byte)'w':
                case (byte)'W':
                    if (gameState == EnumStateOfGame.GAME)
                    {
                        if (player1.onGround || player1.doubleJump)
                        {
                            player1.jump = true;
                        }
                    }
                    else
                    {
                        menuState--;
                        if (menuState < 0)
                        {
                            menuState = EnumStateofMenu.SCORE;
                        }
                    }
                    break;
                case (byte)'s':
                case (byte)'S':
                    menuState++;
                    if (menuState > EnumStateofMenu.SCORE)
                    {
                        menuState = 0;
                    }
                    break;
                case (byte)' ': //if (player1.kd == 0) { player1.hit = true; } break;
                    switch (gameState)
                    {
                        case EnumStateOfGame.GAME: if (player1.kd == 0) { player1.hit = true; }; break;
                        case EnumStateOfGame.BEGINNING: //gameState = EnumStateOfGame.GAME; break;
                            switch (menuState)
                            {
                                case EnumStateofMenu.NEW_GAME: gameState = EnumStateOfGame.GAME; break;
                                case EnumStateofMenu.QUIT: Environment.Exit(0); break;
                                case EnumStateofMenu.SCORE: gameState = EnumStateOfGame.TOP_SCORE ; break;
                            }
                            break;
                        case EnumStateOfGame.DEATH:
                        case EnumStateOfGame.END:
                            gameState = EnumStateOfGame.SCORE;
                            //gameState = EnumStateOfGame.BEGINNING; 
                            //levels = initLevels(N, ref coins, ref enemies, textures, textureCount);
                            //player1 = new Player((int)levels[0].begin.X, (int)levels[0].begin.Y, texPlayer, countPlayer);
                            break;
                        case EnumStateOfGame.SCORE:
                            currScore.name = buffer;
                            SaveScore(currScore);
                            gameState = EnumStateOfGame.BEGINNING;
                            gameState = EnumStateOfGame.BEGINNING; 
                            levels = initLevels(N_levels, ref coins, ref enemies, textures, textureCount);
                            player1 = new Player((int)levels[0].begin.X, (int)levels[0].begin.Y, texPlayer, countPlayer);
                            break;
                    }
                    break;
                case 27:// pause = !pause; break;
                    switch (gameState)
                    {
                        case EnumStateOfGame.GAME: gameState = EnumStateOfGame.PAUSE; SystemSounds.Asterisk.Play(); break;
                        case EnumStateOfGame.PAUSE: gameState = EnumStateOfGame.GAME; SystemSounds.Beep.Play(); break;
                        //case EnumStateOfGame.DEATH: Environment.Exit(0); break;
                        //case EnumStateOfGame.END: Environment.Exit(0); break;
                        case EnumStateOfGame.BEGINNING: Environment.Exit(0); break;
                        case EnumStateOfGame.TOP_SCORE: gameState = EnumStateOfGame.BEGINNING; break;
                    }
                    break;

            }
        }

        public static void releaseKey(byte key, int x, int y)
        {
            keys[key] = false;

            switch (key)
            {
                case (byte)'d':
                case (byte)'D': player1.moveRight = false; break;
                case (byte)'a':
                case (byte)'A': player1.moveLeft = false; break;
                //case (byte)'w':
                //case (byte)'W':  obj.moveUp = false;; break;

            }
        }
  
        static void on_timer(int value)
        {
            Draw();                     // перемалюємо екран
            Glut.glutTimerFunc(33, on_timer, 0); // через 33мс викличеться ця функція
        }

        static void Main(string[] args)
        {
            try
            {
                init();
                Glut.glutDisplayFunc(Draw);
                Glut.glutReshapeFunc(reshape);
                Glut.glutKeyboardFunc(new Glut.KeyboardCallback(pressKey));
                Glut.glutSpecialFunc(new Glut.SpecialCallback(specialKey));

                Glut.glutIgnoreKeyRepeat(1);
                Glut.glutKeyboardUpFunc(new Glut.KeyboardUpCallback(releaseKey));

                Glut.glutTimerFunc(33, on_timer, 0);
                Glut.glutMainLoop();
            }
            catch(FileReadException e)
            {
                Console.WriteLine(e.exception);
            }
            catch
            {
                Console.WriteLine("Some unknow expection...=(");
            }
            
        }
    }
}
