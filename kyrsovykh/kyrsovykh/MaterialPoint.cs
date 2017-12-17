using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace kyrsovykh
{
    abstract class MaterialPoint
    {
        protected Vector position;
        protected Vector velocity;
        protected Vector acceleration;
        protected Vector maxVelocity;
        protected int downVelocity = 45;

        public void Move()
        {
            position += velocity;
            if (Math.Abs(velocity.X) < maxVelocity.X)
            {
                velocity.X += acceleration.X;
            }
            if (velocity.Y > -downVelocity)
            {
                velocity.Y += acceleration.Y;  
            }
            //velocity.Y += acceleration.Y;            
        }
    }
}
