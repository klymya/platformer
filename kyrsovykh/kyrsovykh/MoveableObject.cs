using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace kyrsovykh
{
    abstract class MoveableObject : MaterialPoint
    {
        public MoveableObject() : this(0, 0, 0, EnumStates.NOTHING) { }
        public MoveableObject(int x, int y, int _height,  EnumStates currState, int maxVX = 0)
        {
            heightOfObject = _height;
            form = new Tile[heightOfObject];
            for (int i = 0; i < heightOfObject; i++)
            {
                form[i] = new Tile(x, y + i * Tile.Size, currState);
            }
            position.X = form[0].X;
            position.Y = form[0].Y;
            maxVelocity.X = maxVX;
            //maxVelocity.Y = maxVY;
        }
        protected int heightOfObject;
        protected Tile[] form;

        //внутренее состояние
        public sbyte life = 1;

        //свойства
        public Point Coordinate
        {
            get { return form[0].Coordinate; }
        }
        public int X
        {
            get { return form[0].X; }
            set 
            {
                position.X = value;
                for (int i = 0; i < heightOfObject; i++)
                {
                    form[i].X = value;
                }
            }
        }
        public int Y
        {
            get { return form[0].Y; }
            set
            {
                position.Y = value;
                for (int i = 0; i < heightOfObject; i++)
                {
                    form[i].Y = value + i * Tile.Size;
                }
            }
        }
        public int Height
        {
            get { return heightOfObject; }
        }
        public EnumStates State
        {
            get { return form[0].State; }
        }

        public void ZeroingCoord()
        {
            for (int i = 0; i < heightOfObject; i++)
            {
                form[i].X = 0;
                form[i].Y = 0;
            }
        }
        
        //функции
        public void Draw(Map currMap)
        {
            if (life <= 0)
            {
                return;
            }
            for (int i = 0; i < heightOfObject; i++)
            {
                form[i].Draw(currMap.textures);
            }
        }
        
    }
}
