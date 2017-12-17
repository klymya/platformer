using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Collections;

using System.Drawing;
using System.Drawing.Imaging;

namespace kyrsovykh
{
    class Map
    {
        public Map(): this("", null, 0) { }
        public Map(string _fileName, uint[] _textures, int _texCount)
        {          
            fileName = _fileName;
            height = 0;
            width = 0;
            textures = _textures;
            texCount = _texCount;
        }
        private int height;
        private int width;
        internal Tile[,] map;
        private string fileName;
        public System.Windows.Point begin;
        public System.Windows.Point end;
        public bool ending = false;

        public int coins = 0;

        public int Height
        {
            get { return height; }
            //set { height = value; }
        }
        public int Width
        {
            get { return width; }
        }

        public uint[] textures;
        public int texCount;


        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public void LoadMap(ref ArrayList arrayOfCoins, ref ArrayList arrayOfEnemy, uint[] texNormalEnemy, int countNormalEnemy, uint[] texEasyEnemy, int countEasyEnemy)
        {
            if (String.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                throw new FileReadException(fileName);
            }

            StreamReader inf = new StreamReader(fileName);
            //читаем длину и высоту карты
            height = Convert.ToInt32(inf.ReadLine());
            width = Convert.ToInt32(inf.ReadLine());
            //инициализируем массив 
            map = new Tile[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    map[i, j] = new Tile();
                }
            }
            int[,] arr = new int[height, width];
            for (int i = 0; i < height; i++)
            {
                string tmp = inf.ReadLine();
                string[] buf = tmp.Split(new Char[] { ' ' });
                for (int j = 0; j < width; j++)
                {
                    arr[i, j] = Convert.ToInt32(buf[j]);
                }
            }
            inf.Close();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    switch ((EnumStates)arr[i, j])
                    {
                        case EnumStates.COIN:
                            arrayOfCoins.Add(new Coin(j * Tile.Size, (height - i - 1) * Tile.Size));
                            arr[i, j] = (int)EnumStates.NOTHING;
                            coins++;
                            break;
                        case EnumStates.EASY_ENEMY:
                            arrayOfEnemy.Add(new EasyEnemyR(j * Tile.Size, (height - i - 1) * Tile.Size, texEasyEnemy, countEasyEnemy));
                            arr[i, j] = (int)EnumStates.NOTHING;
                            coins++;
                            break;
                        case EnumStates.NORMAL_ENEMY:
                            arrayOfEnemy.Add(new NormalEnemyR(j * Tile.Size, (height - i - 1) * Tile.Size, texNormalEnemy, countNormalEnemy));
                            arr[i, j] = (int)EnumStates.NOTHING;
                            coins++;
                            break;
                        case EnumStates.EASY_ENEMY_L:
                            arrayOfEnemy.Add(new EasyEnemyL(j * Tile.Size, (height - i - 1) * Tile.Size, texEasyEnemy, countEasyEnemy));
                            arr[i, j] = (int)EnumStates.NOTHING;
                            coins++;
                            break;
                        case EnumStates.NORMAL_ENEMY_L:
                            arrayOfEnemy.Add(new NormalEnemyL(j * Tile.Size, (height - i - 1) * Tile.Size, texNormalEnemy, countNormalEnemy));
                            arr[i, j] = (int)EnumStates.NOTHING;
                            coins++;
                            break;
                        case EnumStates.PLAYER:
                            begin.X = j * Tile.Size;
                            begin.Y = (height - i - 1) * Tile.Size;
                            arr[i, j] = (int)EnumStates.NOTHING;
                            break;
                        case EnumStates.END_LOWER:
                            end.X = j * Tile.Size;
                            end.Y = (height - i - 1) * Tile.Size;
                            break;
 
                    }
                    //if ((EnumStates)arr[i, j] == EnumStates.COIN)
                    //{
                    //    arrayOfCoins.Add(new Coin(j * Tile.Size, (height - i - 1) * Tile.Size));
                    //    arr[i, j] = (int)EnumStates.NOTHING;
                    //} 
                    map[i, j].State = (EnumStates)arr[i, j];
                    map[i, j].Coordinate = new System.Windows.Point(j * Tile.Size, (height - i - 1) * Tile.Size);
                }
            }
        }

        public void Draw()
        {
            if (width == 0 || height == 0)
            {
                return;//cathc
            }
            for (int i = height - 1; i >= 0; i--)
            {
                for (int j = 0; j < width; j++)
                {
                    map[i, j].Draw(textures);
                }
            }    
        }

    }
}
