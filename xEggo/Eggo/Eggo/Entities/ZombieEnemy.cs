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

namespace Eggo.Entities
{
    class ZombieEnemy : Enemy
    {
        const int WALKING_SPEED = 200;
        const int ATTACK_SPEED = 100;
        const int DYING_SPEED = 100;
        
        Texture2D walkingSheet;
        Texture2D biteSheet;
        Texture2D dyingSheet;

        public Boolean isFalling = false;

        public static int numberOfAliveEnemies = 0;

        List<Color[]> walkingTextureData=new List<Color[]>();
        List<Color[]> biteTextureData=new List<Color[]>();
        List<Color[]> dyingTextureData = new List<Color[]>();

        List<Rectangle> walkingAnim = new List<Rectangle>();
        List<Rectangle> biteAnim = new List<Rectangle>();
        List<Rectangle> dyingAnim = new List<Rectangle>();

        public ZombieEnemy() {

            //myBody.ApplyForce(new Vector2(100f,100f));
            //myBody.ApplyTorque(100f);
            

            Rectangle rect;
            for (int i = 0; i < 4; i++)
            {
                rect = new Rectangle(200 * i, 0, 200, 171);
                walkingAnim.Add(rect);
            }

            rect = new Rectangle(0, 0, 200, 171);
            biteAnim.Add(rect);
            rect = new Rectangle(200 , 0, 200, 171);
            biteAnim.Add(rect);
            rect = new Rectangle(400 , 0, 200, 171);
            biteAnim.Add(rect);
            rect = new Rectangle(400, 0, 200, 171);
            biteAnim.Add(rect);
            rect = new Rectangle(600 , 0, 200, 171);
            biteAnim.Add(rect);
            rect = new Rectangle(800, 0, 200, 171);
            biteAnim.Add(rect);


            rect = new Rectangle(0, 0, 200, 217);
            dyingAnim.Add(rect);
            rect = new Rectangle(200 , 0, 200, 217);
            dyingAnim.Add(rect);
            rect = new Rectangle(400, 0, 200, 217);
            dyingAnim.Add(rect);
            rect = new Rectangle(400, 0, 200, 217);
            dyingAnim.Add(rect);
            rect = new Rectangle(600, 0, 200, 217);
            dyingAnim.Add(rect);
            rect = new Rectangle(600 , 0, 200, 217);
            dyingAnim.Add(rect);
            rect = new Rectangle(800, 0, 200, 217);
            dyingAnim.Add(rect);
            rect = new Rectangle(800, 0, 200, 217);
            dyingAnim.Add(rect);
            rect = new Rectangle(1000, 0, 200, 217);
            dyingAnim.Add(rect);
            rect = new Rectangle(1200, 0, 200, 217);
            dyingAnim.Add(rect);

            scale = 0.25f;

            myBody = BodyFactory.CreateCircle(Eggo.world, 100f * scale, 1f);
            myBody.BodyType = BodyType.Dynamic;
            myBody.Position = new Vector2(800, 120);

        }
        public override void load(ContentManager content)
        {
            walkingSheet = content.Load<Texture2D>("enemy-walking-spritesheet");
            biteSheet = content.Load<Texture2D>("enemy-bite-spritesheet");
            dyingSheet = content.Load<Texture2D>("enemy-death-spritesheet");

            fillTextureData(walkingTextureData,walkingAnim,walkingSheet);
            fillTextureData(biteTextureData,biteAnim,biteSheet);
            fillTextureData(dyingTextureData,dyingAnim,dyingSheet);

        
            walk();
            currentFlip = SpriteEffects.None;
        }

        public void walk() {
            mSpriteTexture = walkingSheet;
            mTextureData = walkingTextureData[0];
            mDrawingRectangle = walkingAnim[0];

            mCurrentState = State.Walking;
            mSpeed.X = WALKING_SPEED;
            mSpeed.Y = WALKING_SPEED;
            if (isFalling)
            {
                mDirection.Y = MOVE_DOWN;
            }
            else
            {
                if (position.X >= Level.player.position.X)
                {
                    mDirection.X = MOVE_LEFT;
                    currentFlip = SpriteEffects.None;
                }
                else { mDirection.X = MOVE_RIGHT; currentFlip = SpriteEffects.FlipHorizontally; }
                
                
            }
        }

        public override void die() {
            if (mCurrentState != State.Dying)
            {
                currentAnimIndex = 0;
                mCurrentState = State.Dying;
                mSpeed = Vector2.Zero;
                mDirection = Vector2.Zero;
                Eggo.score += 10;
                
           }
        }

        public override void hit()
        {
            if (mCurrentState != State.Attacking && mCurrentState != State.Dying)
            {
                currentAnimIndex = 0;
                mCurrentState = State.Attacking;
                mSpeed = Vector2.Zero;
                mDirection = Vector2.Zero;
            }
        }


        public override void update(GameTime gameTime)
        {
            aCurrentKeyboardState = Keyboard.GetState();

            
            //updateMovement(aCurrentKeyboardState);
            updateSprites(gameTime);

            base.update(gameTime, mSpeed, mDirection);

            
        }

        private void updateSprites(GameTime gameTime)
        {
            timeSinceLastUpdate += gameTime.ElapsedGameTime.TotalMilliseconds;
            switch (mCurrentState)
            {
                case State.Walking:
                    mSpriteTexture = walkingSheet;
                    
                    if (timeSinceLastUpdate > WALKING_SPEED)
                    {
                        timeSinceLastUpdate =0;
                        if (currentAnimIndex >= walkingAnim.Count)
                            currentAnimIndex = 0;

                        mDrawingRectangle = walkingAnim[currentAnimIndex];
                        mTextureData = walkingTextureData[currentAnimIndex];
                        currentAnimIndex++;
                    }
                    break;
                case State.Dying:
                    mSpriteTexture = dyingSheet;
                    
                    if (timeSinceLastUpdate > DYING_SPEED)
                    {
                        timeSinceLastUpdate = 0;
                        if (currentAnimIndex >= dyingAnim.Count)
                        { myBody.Position =new Vector2( -1,myBody.Position.Y); break; }

                        mDrawingRectangle = dyingAnim[currentAnimIndex];
                        mTextureData = dyingTextureData[currentAnimIndex];
                        currentAnimIndex++;
                    }
                    break;
                case State.Attacking:
                    if (boundingRectangle.Intersects(Level.player.boundingRectangle))
                    {
                        mSpriteTexture = biteSheet;

                        if (timeSinceLastUpdate > ATTACK_SPEED)
                        {
                            timeSinceLastUpdate = 0;

                            if (currentAnimIndex >= biteAnim.Count)
                                currentAnimIndex = 0;

                            mDrawingRectangle = biteAnim[currentAnimIndex];
                            mTextureData = biteTextureData[currentAnimIndex];
                            currentAnimIndex++;

                        }
                    }
                    else {
                        walk();
                    }
                    break;
            }
        }

        public override void draw(SpriteBatch spriteBatch)
        {
            float rotation = 0.0f;
            //if (isFalling) { rotation = (float) (3*Math.PI/2); }
            spriteBatch.Draw(mSpriteTexture, myBody.Position, mDrawingRectangle, Color.White, rotation, new Vector2(0, 0), scale, currentFlip, 0.0f);
        }
    }
}
