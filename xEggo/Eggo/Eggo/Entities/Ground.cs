using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;

namespace Eggo.Entities
{
  
    public class Ground: GameObject
    {
        public Ground() {
            
            myBody = BodyFactory.CreateRectangle(Eggo.world,Eggo.getInstance().Window.ClientBounds.Width*2, 1f, 1f);
            myBody.BodyType = BodyType.Static;
            myBody.Position = new Microsoft.Xna.Framework.Vector2(0, Eggo.getInstance().Window.ClientBounds.Height-40);
            myBody.IsStatic = true;
            myBody.Restitution = 0.0f;
            myBody.Friction = 0.0f;
            mDrawingRectangle = new Rectangle(0, 0, Eggo.getInstance().Window.ClientBounds.Width * 2, 1);
        }

         public override void load(ContentManager content){ }

         public void update(GameTime gameTime)
         {
             aCurrentKeyboardState = Keyboard.GetState();


             //updateMovement(aCurrentKeyboardState);
             //updateSprites(gameTime);

             base.update(gameTime, mSpeed, mDirection);
         }

         public override void draw(SpriteBatch spriteBatch)
         {
             //drawBoundingBox(spriteBatch, Eggo.getInstance().GraphicsDevice);
         }

    }


}