using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Media;

using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform;

namespace kyrsovykh
{
    sealed class Player: Character
    {
        public Player() : this(0, 0, null, 0) { }
        public Player(int x, int y, uint[] _textures, int _texCount)
            : base(x, y, 2, 15, EnumStates.PLAYER, _textures, _texCount) 
        {
            life = 100;
            coinSound = new SoundPlayer();
            coinSound.Stream = kyrsovykh.Properties.Resources.Pickup_Coin24;
            hitSound = new SoundPlayer();
            hitSound.Stream = kyrsovykh.Properties.Resources.Hit_Hurt12;
            reductSound = new SoundPlayer();
            reductSound.Stream = kyrsovykh.Properties.Resources.Blip_Select2;
        }

        ///////
        SoundPlayer coinSound;
        SoundPlayer hitSound;
        SoundPlayer reductSound;


        public uint coins = 0;
        public bool hit = false;//флажок удара
        private byte hitCount = 0;//счетчик прорисовки удара
        public byte kd = 0;
        public void Hit(out int _x, out int _h)
        {
            int h = (Tile.Size / 4) * 3;
            int x;
            int y = form[heightOfObject - 1].Y;// -h / 4;
            if (isRightLook)
            {
                x = form[0].X + Tile.Size;
            }
            else
            {
                x = form[0].X - h;
            }

            //Gl.glColor4f(0.0f, 0.1f, 1.0f, 1.0f);
            //Gl.glBegin(Gl.GL_QUADS);
            //    Gl.glVertex2d(x, y);
            //    Gl.glVertex2d(x, y + h / 2);
            //    Gl.glVertex2d(x + h, y + h / 2);
            //    Gl.glVertex2d(x + h, y);
            //Gl.glEnd();

            k = texCount - 1;

            hitSound.Play();

            _x = x;
            _h = h;
        }

        public void ReductionInLife()
        {
            if (life > 0)
            {
                life -= Math.Min(life, (sbyte)7);
                reductSound.Play();
            }
        }

        public void HandlerMovementWithHit(ref ArrayList arrayOfEnemy,ref ArrayList arrayOfCoins) 
        {
            HandlerMovement();
            if (hit)
            {
                if (coins < 5)
                {
                    return ;
                }
                int x, h;
                Hit(out x, out h);


                //CollisionEnemy(arrayOfEnemy, x, h, ref arrayOfCoins);

                if (hitCount >= 3)
                {
                    hit = false;
                    hitCount = 0;
                    kd = 5;
                    CollisionHit(arrayOfEnemy, x, h, ref arrayOfCoins);
                }
                else
                {
                    hitCount++;
                }
            }
            else
            {
                if (kd > 0)
                {
                    kd--;
                }
            }
        }

        private bool Collision(int x, int y, int objHeight)
        {
            //нижний левый
            if ((x >= X && x <= X + Tile.Size) && (y >= Y && y <= Y + heightOfObject * Tile.Size))
            {
                return true;
            }
            //нижний правый
            if ((x + Tile.Size >= X && x + Tile.Size <= X + Tile.Size) && (y >= Y && y <= Y + heightOfObject * Tile.Size))
            {
                return true;
            }
            //верхний левый
            if ((x >= X && x <= X + Tile.Size) && (y + objHeight * Tile.Size >= Y && y + objHeight <= Y + heightOfObject * Tile.Size))
            {
                return true;
            }
            //верхний правый
            if ((x + Tile.Size >= X && x + Tile.Size <= X + Tile.Size) && (y + objHeight * Tile.Size >= Y && y + objHeight <= Y + heightOfObject * Tile.Size))
            {
                return true;
            }
            return false;
        }

        private void CollisionCoins(ArrayList arrayOfCoins, Map currMap)
        {
            foreach (Coin i in arrayOfCoins)
            {
                if (Collision(i.X, i.Y, i.Height))//(i.X > X && i.X < X + Tile.Size)
                {
                    coins++;
                    currMap.coins--;
                    i.ZeroingCoord();
                    i.life = 0;
                    coinSound.Play();
                }
            }
        }

        private void CollisionHit(ArrayList arrayOfEnemy, int x, int h, ref ArrayList arrayOfCoins)
        {
            int tmp;
            if (isRightLook)
            {
                tmp = x + h;
            }
            else
            {
                tmp = x;
            }

            foreach (Enemy i in arrayOfEnemy)
            {
                if (Math.Abs(i.Y - Y) >= Tile.Size) continue;
                if (tmp >= i.X && tmp <= i.X + Tile.Size )
                {
                    arrayOfCoins.Add(new Coin(i.X, i.Y));// + Tile.Size / 2));
                    
                    i.life = 0;
                    i.ZeroingCoord();
                }
            }
        }

        private void CollisionEnemy(ArrayList arrayOfEnemy)
        {
            foreach (Enemy i in arrayOfEnemy)
            {
                if (Collision(i.X, i.Y, i.Height))
                {
                    ReductionInLife();
                }
            }
        }

        private void CollisionEnding(Map currMap)
        {
            if (Math.Abs(currMap.end.X - X) <= Tile.Size / 4 && Math.Abs(currMap.end.Y - Y) <= Tile.Size / 4)// && currMap.coins == 0)
            {
                currMap.ending = true;
            }
            //Console.WriteLine("{0} {1}")
        }

        public void Draw(Map currMap, ArrayList arrayOfCoins, ArrayList arrayOfEnemy)
        {            
            DrawC(currMap);
            CollisionCoins(arrayOfCoins, currMap);
            CollisionEnemy(arrayOfEnemy);
            CollisionEnding(currMap);
            
        }
    }
}
