using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework.Content;

namespace Eggo.Entities
{
    class Shredder : HotSpot
    {
        private const int type = 1;

        const int ANIM_SPEED = 200;

        Texture2D animationSheet;

        List<Color[]> animationTextureData = new List<Color[]>();

        List<Rectangle> pictureAnim = new List<Rectangle>();

        public Shredder()
        {
            Rectangle rect;

            for (int i = 0; i < 8; i++)
            {
                rect = new Rectangle(191 * i + 4, 0, 191, 198);
                pictureAnim.Add(rect);
            }
            scale = 0.5f;

            myBody = BodyFactory.CreateCircle(Eggo.world, 100f * scale, 1f);
            myBody.BodyType = BodyType.Dynamic;
            myBody.Position = new Vector2(220, 120);
        }

        public override int getType()
        {
            return type;
        }

        public override void load(ContentManager content)
        {
            animationSheet = content.Load<Texture2D>("farram");

            fillTextureData(animationTextureData, pictureAnim, animationSheet);


            Move();
            currentFlip = SpriteEffects.None;
        }

        public void Move()
        {
            mSpriteTexture = animationSheet;
            mTextureData = animationTextureData[0];
            mDrawingRectangle = pictureAnim[0];

            mSpeed = Vector2.Zero;
            mDirection = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            //updateMovement(aCurrentKeyboardState);
            updateSprites(gameTime);

            base.update(gameTime, mSpeed, mDirection);
        }

        private void updateSprites(GameTime gameTime)
        {
            timeSinceLastUpdate += gameTime.ElapsedGameTime.TotalMilliseconds;
            mSpriteTexture = animationSheet;

            if (timeSinceLastUpdate > ANIM_SPEED)
            {
                timeSinceLastUpdate = 0;
                if (currentAnimIndex >= pictureAnim.Count)
                    currentAnimIndex = 0;

                mDrawingRectangle = pictureAnim[currentAnimIndex];
                mTextureData = animationTextureData[currentAnimIndex];
                currentAnimIndex++;
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            float rotation = 0.0f;
            spriteBatch.Draw(mSpriteTexture, myBody.Position, mDrawingRectangle, Color.White, rotation, new Vector2(0, 0), scale, currentFlip, 0.0f);
        }
    }
}
