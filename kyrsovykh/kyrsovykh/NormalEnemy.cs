using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyrsovykh
{
    abstract class NormalEnemy: Enemy
    {
        public NormalEnemy() : this(0, 0, true, null, 0) { }
        public NormalEnemy(int x, int y, bool isRight, uint[] _textures, int _texCount) : base(x, y, Tile.Size * 4, 2, EnumStates.NORMAL_ENEMY, isRight, _textures, _texCount) 
        {
            pursuit = false;
            //moveRight = false;
            pursuitRadius = 2 * radius;
        }

        private bool pursuit;
        private int pursuitRadius;

        public void MoveEnemy(Map currMap, int x, int y)
        {
            if ((Math.Abs(Y - y) < Tile.Size * 2 && Math.Abs(X - x) < radius && !collisionX) ||
                (Math.Abs(Y - y) < Tile.Size * 2 && Math.Abs(X - x) < pursuitRadius && !collisionX && pursuit))
            {
                pursuit = true;
                //radius += radius / 2;
            }
            else
            {
                pursuit = false;
            }

            if (pursuit)
            {
                //moveLeft = false;
                //moveRight = false;
                if (X < x)
                {
                    if (moveLeft)
                    {
                        moveLeft = false;
                        ReturnVectors();
                        moveRight = true; 
                    }                                   
                }
                else if(X > x)
                {
                    if (moveRight)
                    {
                        moveRight = false;
                        ReturnVectors();
                        moveLeft = true;
                    }
                }
            }
            else
            {
                base.MoveEnemy(currMap);
            }
            
        }
    }
}
