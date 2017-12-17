using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyrsovykh
{
    sealed class EasyEnemyL: EasyEnemy
    {
        public EasyEnemyL(int x, int y, uint[] _textures, int _texCount) : base(x, y, false, _textures, _texCount) { }
    }
}
