using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyrsovykh
{
    sealed class NormalEnemyL: NormalEnemy
    {
        public NormalEnemyL(int x, int y, uint[] textures, int texCount) : base(x, y, false, textures, texCount) { }
    }
}
