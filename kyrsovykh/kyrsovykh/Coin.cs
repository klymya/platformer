using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyrsovykh
{
    sealed class Coin: MoveableObject
    {
        public Coin() : base() { }
        public Coin(int x, int y) : base(x, y, 1, EnumStates.COIN) { }
    }
}
