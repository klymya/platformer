using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform;

namespace kyrsovykh
{
    abstract class Character : MoveableObject
    {
        public Character() : this(0, 0, 2, 0, EnumStates.NOTHING, null, 0) { }
        public Character(int x, int y, int _height, int maxVX, EnumStates state, uint[] _textures, int _texCount): base(x, y, _height, state, maxVX)
        {
            textures = _textures;
            texCount = _texCount;
            k = 0;
        }

        //флажки передвижения
        public bool onGround = false;
        public bool doubleJump = true;
        public bool moveRight = false;
        public bool moveLeft = false;
        public bool jump = false;
        public bool isRightLook = true;//куда смотрит персонаж
        public bool collisionX = false;

        protected uint[] textures;
        protected int texCount;
        protected float k;

        //функции
        public void MoveLeft()
        {
            isRightLook = false;
            acceleration.X = -5;
            Move();
            //k = (k + 1) % texCount;
            k += 0.5f;
            int max = texCount;
            if (form[0].State == EnumStates.PLAYER)
            {
                max = texCount - 2;
            }
            if ((int)k >= max)
            {
                k = 0;
            }
        }
        public void MoveRight()
        {
            isRightLook = true;
            acceleration.X = 5;
            Move();
            //k = (k + 0.5) % (float)texCount;
            k += 0.5f;
            int max = texCount;
            if (form[0].State == EnumStates.PLAYER)
            {
                max = texCount - 2;
            }
            if ((int)k >= max)
            {
                k = 0;
            }
        }
        public void Jump()
        {
            if (!onGround)
            {
                doubleJump = false;
            }
            if (onGround)
            {
                acceleration.Y = 40;
            }
            else
            {
                acceleration.Y = 20;
            }

            Move();
            onGround = false;
            //k = texCount - 2;
        }
       
   
        public void Gravitation()
        {
            acceleration.Y = -5;
            Move();
        }
        //обнуляет вектора состояния
        public void ReturnVectors()
        {
            acceleration.X = 0;
            acceleration.Y = 0;
            velocity.X = 0;
            //velocity.Y = 0;
        }
        //меняем координаты
        private void ChangeOfCoord(int x, int y)
        {
            //down.X = x;
            //down.Y = y;
            //up.X = down.X;
            //up.Y = down.Y + Tile.Size;
            for (int i = 0; i < heightOfObject; i++)
            {
                form[i].X = x;
                form[i].Y = y + i * Tile.Size;
            }
        }
        //проверка коллизий
        protected bool CollisionX(Map currMap)
        {
            if (velocity.X < 0)//если влево
            {
                int nW = (int)position.X / Tile.Size;
                int nH0 = currMap.Height - 1 - (form[0].Y + 2 * Tile.Size) / Tile.Size;
                int nH1 = currMap.Height - 1 - form[0].Y / Tile.Size;

                for (int i = nH0; i <= nH1; i++)
                {
                    if (currMap.map[i, nW].State == EnumStates.BLOCK)
                    {
                        velocity.X = 0;
                        position.X = currMap.map[i, nW].X + Tile.Size +1;
                        return true;
                    }
                }
            }
            else
                if (velocity.X > 0)//вправо
                {
                    int nW = ((int)position.X + Tile.Size) / Tile.Size;
                    int nH0 = currMap.Height - 1 - (form[0].Y + 2 * Tile.Size) / Tile.Size;
                    int nH1 = currMap.Height - 1 - form[0].Y / Tile.Size;

                    for (int i = nH0; i <= nH1; i++)
                    {
                        if (currMap.map[i, nW].State == EnumStates.BLOCK)
                        {
                            velocity.X = 0;
                            position.X = currMap.map[i, nW].X - Tile.Size -1;
                            return true;
                        }
                    }
                }
            return false;
        }

        protected bool CollisionY(Map currMap)
        {
            if (velocity.Y < 0)//если подаем
            {
                int nH = currMap.Height - 1 - ((int)position.Y - 1) / Tile.Size;
                int nW0 = X / Tile.Size;
                int nW1 = (X + Tile.Size) / Tile.Size;

                if (nH < 0)
                {
                    nH = 0;
                }
                if (nH >= currMap.Height)
                {
                    nH = currMap.Height - 1;
                }

                for (int i = nW0; i <= nW1; i++)
                {
                    if (currMap.map[nH, i].State == EnumStates.BLOCK || currMap.map[nH, i].State == EnumStates.PLATFORM || currMap.map[nH, i].State == EnumStates.TRAP)
                    {
                        velocity.Y = 0;
                        position.Y = currMap.map[nH, i].Y + Tile.Size;// +1;
                        onGround = true;
                        doubleJump = true;
                        if (currMap.map[nH, i].State == EnumStates.TRAP && form[0].State == EnumStates.PLAYER)
                        {
                            ((Player)this).ReductionInLife();
                        }
                        //return true;
                    }
                }
            }
            else
                if (velocity.Y > 0)
                {
                    int nH = currMap.Height - 1 - ((int)position.Y + heightOfObject * Tile.Size) / Tile.Size;
                    if (nH < 0)
                    {
                        nH = 0;
                    }
                    int nW0 = form[0].X / Tile.Size;
                    int nW1 = (form[0].X + Tile.Size) / Tile.Size;

                    for (int i = nW0; i <= nW1; i++)
                    {
                        if (currMap.map[nH, i].State == EnumStates.BLOCK)
                        {
                            velocity.Y = 0;
                            position.Y = currMap.map[nH, i].Y - heightOfObject * Tile.Size;// -1;
                            return true;
                        }
                    }

                }
            //onLadder = false;
            return false;
        }
        //функция передвижения обьекта
        private void MoveObj(Map currMap)
        {
            if (!onGround)
            {
                k = texCount - 2;
            }
            if (form[0].X != position.X || form[0].Y != position.Y)//если координаты поменялись
            {
                //если нет коллизий
                collisionX = CollisionX(currMap);
                CollisionY(currMap);
                ChangeOfCoord((int)position.X, (int)position.Y);
                position.X = form[0].X;
                position.Y = form[0].Y;

                if (velocity.Y < 0)
                {
                    onGround = false;
                }
            }
        }

        public void HandlerMovement()
        {
            if (moveRight)
            {
                MoveRight();
            }
            else if (moveLeft)
            {
                MoveLeft();
            }
            else
            {
                ReturnVectors();
                k = 0;
            }

            if (jump)
            {
                Jump();
                jump = false;
            }
            //if (hit)
            //{
            //    int x, h;
            //    Hit(out x, out h);
            //    if (hitCount >= 3)
            //    {
            //        hit = false;
            //        hitCount = 0;
            //    }
            //    else
            //    {
            //        hitCount++;
            //    }                
            //}

            Gravitation();
        }

        public void DrawC(Map currMap)
        {
            MoveObj(currMap);
            Draw(currMap);
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[(int)k]);
            if (isRightLook)
            {
                Tile.DrawTextQuad(X - 25, Y , Tile.Size + 50, Tile.Size * heightOfObject + 20);
            }
            else
            {
                Tile.DrawTextQuad(X - 25, Y, Tile.Size + 50, Tile.Size * heightOfObject + 20, isNormal: false);
            }
            Gl.glDisable(Gl.GL_TEXTURE_2D);
        }
    }
}
