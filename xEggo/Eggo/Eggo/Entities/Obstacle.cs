using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Eggo.Entities
{
    class Obstacle
    {
        private Vector2 position;
        private int type;

        public Vector2 Position
        {
            get;
            set;
        }

        public int Type
        {
            get;
            set;
        }
    }
}
