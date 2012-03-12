using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Eggo.Entities
{
    abstract public class HotSpot : GameObject
    {
        abstract public void Update(GameTime gameTime);
        abstract public int getType();
    }
}
