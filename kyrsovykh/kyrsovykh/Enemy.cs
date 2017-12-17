using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyrsovykh
{
    abstract class Enemy: Character
    {
        public Enemy() : this(0, 0, 0, 0, EnumStates.NOTHING, true, null, 0) { }
        public Enemy(int x, int y, int _radius, int _height, EnumStates currState, bool toRight, uint[] _textures, int _texCount) : base(x, y, _height, 5, currState, _textures, _texCount) 
        {
            radius = _radius;
            startingX = x;
            if (toRight)
            {
                moveRight = true;
            }
            else
            {
                moveLeft = true;
            }
        }

        protected int radius;
        protected int startingX;
        

        public void MoveEnemy(Map currMap)
        {
            if (moveRight)
            {
                if (collisionX || Math.Abs(startingX - (X + Tile.Size)) >= radius)
                {
                    moveRight = false;
                    return ;
                }
            }
            if (moveLeft)
            {
                if (collisionX || Math.Abs(startingX - X) >= radius)
                {
                    moveLeft = false;
                    return ;
                }
            }

            if (!moveRight && !moveLeft)
            {
                if (isRightLook)
                {
                    moveLeft = true;
                }
                else
                {
                    moveRight = true;
                }
            }
        }
    }
}
