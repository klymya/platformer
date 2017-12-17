using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyrsovykh
{
    sealed class EasyEnemyR: EasyEnemy
    {
        public EasyEnemyR(int x, int y, uint[] _textures, int _texCount) : base(x, y, true, _textures, _texCount) { }
    }
}
