using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyrsovykh
{
    abstract class EasyEnemy: Enemy
    {
        public EasyEnemy() : this(0, 0, true, null, 0) { }
        public EasyEnemy(int x, int y, bool toRight, uint[] _textures, int _texCount) : base(x, y, Tile.Size * 3, 1, EnumStates.EASY_ENEMY, toRight, _textures, _texCount) { }
    }
}
