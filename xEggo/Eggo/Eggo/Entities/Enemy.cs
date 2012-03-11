using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Eggo.Entities
{
    abstract public class Enemy : GameObject
    {
        abstract public void update(GameTime gameTime);
        abstract public void die();
        abstract public void hit();
        abstract public int getType();
    }
}
