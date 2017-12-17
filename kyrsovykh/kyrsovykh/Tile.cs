using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Drawing;

using Tao.FreeGlut;
using Tao.OpenGl;
using Tao.Platform;

namespace kyrsovykh
{
    enum EnumStates
    {
        NOTHING,//0
        BLOCK,//1
        PLATFORM,//2
        TRAP,//3
        END_LOWER,//4
        END_UPPER,//5
        COIN,//6
        PLAYER,//7
        EASY_ENEMY,//8
        NORMAL_ENEMY,//9
        EASY_ENEMY_L,//10
        NORMAL_ENEMY_L//11
    }

    class Tile
    {
        public Tile()
        {
            begin.X = 0;
            begin.Y = 0;
            state = 0;
        }
        public Tile(int x, int y, EnumStates _state)
        {
            begin.X = x;
            begin.Y = y;
            state = _state;
        }
        private Point begin;
        private EnumStates state; 
        private static int size = 64;

        public static int Size
        {
            get { return size; }
        }
        public Point Coordinate
        {
            set { begin = value; }
            get { return begin; }
        }
        public int X
        {
            get { return (int)begin.X; }
            set{ begin.X = value; }
        }
        public int Y
        {
            get { return (int)begin.Y; }
            set { begin.Y = value; }
        }
        public EnumStates State
        {
            get { return state; }
            set { state = value; }
        }

        private void DrawTextQuad(double x = 1.0, double y = 1.0)
        {
            Gl.glColor4f(1.0f, 1.0f, 1.0f, 1.0f);
            Gl.glBegin(Gl.GL_QUADS);
                Gl.glTexCoord2d(0, y);
                Gl.glVertex2d(begin.X, begin.Y);
                Gl.glTexCoord2d(0.0, 0.0);
                Gl.glVertex2d(begin.X, begin.Y + size);
                Gl.glTexCoord2d(x, 0);
                Gl.glVertex2d(begin.X + size, begin.Y + size);
                Gl.glTexCoord2d(x, y);
                Gl.glVertex2d(begin.X + size, begin.Y);
            Gl.glEnd();
            //Gl.glTexCoord2d(x, y);
            //Gl.glVertex2d(begin.X, begin.Y);
            //Gl.glTexCoord2d(x, 0.0);
            //Gl.glVertex2d(begin.X, begin.Y + size);
            //Gl.glTexCoord2d(0, 0);
            //Gl.glVertex2d(begin.X + size, begin.Y + size);
            //Gl.glTexCoord2d(0.0, y);
            //Gl.glVertex2d(begin.X + size, begin.Y);
        }

        public static void DrawTextQuad(double beginX, double beginY, double hX, double hY, bool isNormal = true, double texX = 1, double texY = 1,
            float r = 1, float g = 1, float b = 1, float a = 1)
        {
            Gl.glColor4f(r, g, b, a);
            if (isNormal)
            {
                Gl.glBegin(Gl.GL_QUADS);
                    Gl.glTexCoord2d(0, texY);
                    Gl.glVertex2d(beginX, beginY);
                    Gl.glTexCoord2d(0.0, 0.0);
                    Gl.glVertex2d(beginX, beginY + hY);
                    Gl.glTexCoord2d(texX, 0);
                    Gl.glVertex2d(beginX + hX, beginY + hY);
                    Gl.glTexCoord2d(texX, texY);
                    Gl.glVertex2d(beginX + hX, beginY);
                Gl.glEnd();
            }
            else
            {
                Gl.glBegin(Gl.GL_QUADS);
                    Gl.glTexCoord2d(texX, texY);
                    Gl.glVertex2d(beginX, beginY);
                    Gl.glTexCoord2d(texX, 0.0);
                    Gl.glVertex2d(beginX, beginY + hY);
                    Gl.glTexCoord2d(0, 0);
                    Gl.glVertex2d(beginX + hX, beginY + hY);
                    Gl.glTexCoord2d(0.0, texY);
                    Gl.glVertex2d(beginX + hX, beginY);
                Gl.glEnd();
            }
            
            
        }

        public void Draw(uint[] textures)
        {
            switch (state)
            {
                case EnumStates.NOTHING: //Gl.glColor4f(0.8f, 0.7f, 0.6f, 1.0f);
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[(int)EnumStates.NOTHING]);
                    DrawTextQuad();
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                    break;
                case EnumStates.BLOCK : //Gl.glColor4f(1.0f, 0.7f, 0.4f, 1.0f);
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[(int)EnumStates.BLOCK]);
                    DrawTextQuad();
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                    break;
                case EnumStates.PLATFORM : //Gl.glColor4f(0.0f, 1.0f, 0.0f, 1.0f); 
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[(int)EnumStates.PLATFORM]);
                    DrawTextQuad();
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                    break;
                case EnumStates.TRAP: //Gl.glColor4f(0.7f, 0.7f, 0.7f, 1.0f);
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[(int)EnumStates.TRAP]);
                    DrawTextQuad();
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                    break;
                case EnumStates.END_LOWER: //Gl.glColor4f(0.7f, 0.3f, 0.0f, 1.0f);
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[(int)EnumStates.END_LOWER]);
                    DrawTextQuad();
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                    break;
                case EnumStates.END_UPPER: //Gl.glColor4f(0.7f, 0.3f, 0.0f, 1.0f);
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[(int)EnumStates.END_UPPER]);
                    DrawTextQuad();
                    //DrawTextQuad(begin.X, begin.Y, Size, Size);
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                    break;
                case EnumStates.COIN: //Gl.glColor4f(1.0f, 0.7f, 0.0f, 1.0f);
                    Gl.glEnable(Gl.GL_TEXTURE_2D);
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, textures[(int)EnumStates.COIN]);
                    DrawTextQuad();
                    Gl.glDisable(Gl.GL_TEXTURE_2D);
                    break;
                //case EnumStates.PLAYER: Gl.glColor4f(0.0f, 0.1f, 1.0f, 1.0f);
                //    Gl.glBegin(Gl.GL_QUADS);
                //    Gl.glVertex2d(begin.X, begin.Y);
                //    Gl.glVertex2d(begin.X, begin.Y + size);
                //    Gl.glVertex2d(begin.X + size, begin.Y + size);
                //    Gl.glVertex2d(begin.X + size, begin.Y);
                //    Gl.glEnd();
                //    break;
                //case EnumStates.EASY_ENEMY: Gl.glColor4f(0.5f, 0.0f, 0.0f, 1.0f);
                //    Gl.glBegin(Gl.GL_QUADS);
                //    Gl.glVertex2d(begin.X, begin.Y);
                //    Gl.glVertex2d(begin.X, begin.Y + size);
                //    Gl.glVertex2d(begin.X + size, begin.Y + size);
                //    Gl.glVertex2d(begin.X + size, begin.Y);
                //    Gl.glEnd();
                //    break;
                //case EnumStates.NORMAL_ENEMY: Gl.glColor4f(0.9f, 0.0f, 0.0f, 1.0f);
                //    Gl.glBegin(Gl.GL_QUADS);
                //    Gl.glVertex2d(begin.X, begin.Y);
                //    Gl.glVertex2d(begin.X, begin.Y + size);
                //    Gl.glVertex2d(begin.X + size, begin.Y + size);
                //    Gl.glVertex2d(begin.X + size, begin.Y);
                //    Gl.glEnd();
                //    break;
            }
            //Gl.glBegin(Gl.GL_QUADS);
            //    //Gl.glTexCoord2d(0.0, 0.0);
            //    Gl.glVertex2d(begin.X, begin.Y);
            //    //Gl.glTexCoord2d(0.0, 1.0);
            //    Gl.glVertex2d(begin.X, begin.Y + size);
            //    //Gl.glTexCoord2d(1.0, 1.0);
            //    Gl.glVertex2d(begin.X + size, begin.Y + size);
            //    //Gl.glTexCoord2d(1.0, 0.0);
            //    Gl.glVertex2d(begin.X + size, begin.Y);
            //Gl.glEnd();
            
        }

    }
}