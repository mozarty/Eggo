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
using FarseerPhysics.Collision.Shapes;
namespace Eggo.Entities
{
    

    class Player : GameObject
    {
        
        const int WALKING_SPEED = 100;
        const int ATTACK_SPEED = 100;
        const int JUMP_SPEED = 100;

        Texture2D walkingSheet;
        Texture2D hitSheet;
        Texture2D idleSheet;

        List<Color[]> walkingTextureData = new List<Color[]>();
        List<Color[]> hitTextureData = new List<Color[]>();
        List<Color[]> idleTextureData = new List<Color[]>();

        List<Rectangle> walkingAnim = new List<Rectangle>();
        List<Rectangle> hitAnim = new List<Rectangle>();
        List<Rectangle> idleAnim = new List<Rectangle>();
        

        private void createBody(){
        //myBody = CreateBodyFromImage(Eggo.getInstance().Content, idleSheet);
            myBody = BodyFactory.CreateRectangle(Eggo.world, 200, 205, 1);
            myBody.BodyType = BodyType.Static;
            myBody.Position = new Vector2(100, 350);
        }
        public Player() {

            
            
            //myBody.ApplyForce(new Vector2(100f, 100f));
            //myBody.ApplyTorque(100f);

            Rectangle rect;

            for (int i = 0; i < 10; i++) { 
                rect = new Rectangle(200*i, 0, 200, 205);
                walkingAnim.Add(rect);
            }
            for (int i = 0; i < 6; i++)
            {
                rect = new Rectangle(200 * i, 0, 200, 205);
                hitAnim.Add(rect);
            }

            rect = new Rectangle(0, 0, 200, 205);
            idleAnim.Add(rect);

            

            scale = 0.5f;
        }
        
         public override void load(ContentManager content)
        {
            walkingSheet = content.Load<Texture2D>("walking-10-frame-eggo-spritesheet");
            hitSheet = content.Load<Texture2D>("hit sprite sheet");
            idleSheet = content.Load<Texture2D>("idle");

            fillTextureData(walkingTextureData, walkingAnim, walkingSheet);
            fillTextureData(idleTextureData ,idleAnim, idleSheet);
            fillTextureData(hitTextureData , hitAnim, hitSheet);

            mSpriteTexture = idleSheet;
            mTextureData = idleTextureData[0];
            mDrawingRectangle = idleAnim[0];

            createBody();
        }

         

         public void update(GameTime gameTime)
        {
            aCurrentKeyboardState = Keyboard.GetState();

            updateMovement(aCurrentKeyboardState);
            UpdateJump(aCurrentKeyboardState);
            updateSprites(gameTime);
            
            
            base.update(gameTime, mSpeed, mDirection);

            // Prevent the person from moving off of the screen
            myBody.Position = new Vector2(MathHelper.Clamp(myBody.Position.X,
                Eggo.safeBounds.Left, Eggo.safeBounds.Right - (mDrawingRectangle.Width * scale)), myBody.Position.Y);

        }

         private void UpdateJump(KeyboardState aCurrentKeyboardState)
         {
             if (mCurrentState == State.Walking || mCurrentState == State.Standing)
             {
                 if (aCurrentKeyboardState.IsKeyDown(Keys.Up) && mPreviousKeyboardState.IsKeyUp(Keys.Up) )
                 {
                     Jump();
                 }
             }

             if (mCurrentState == State.Jumping)
             {
                 if (mActionStartingPosition.Y - myBody.Position.Y > 50)
                 {
                     mDirection.Y = MOVE_DOWN;
                 }

                 if (myBody.Position.Y > mActionStartingPosition.Y)
                 {
                     myBody.Position = new Vector2(myBody.Position.X, mActionStartingPosition.Y);
                     mCurrentState = State.Walking;
                     mDirection = Vector2.Zero;
                 }
             }
         }

         private void Jump()
         {
             if (mCurrentState != State.Jumping)
             {
                 mCurrentState = State.Jumping;
                 mActionStartingPosition = myBody.Position;
                 if (currentFlip == SpriteEffects.None) 
                 mDirection.X = MOVE_RIGHT;
                 else mDirection.X = MOVE_LEFT;
                 mDirection.Y = MOVE_UP;
                 mSpeed = new Vector2(JUMP_SPEED, JUMP_SPEED);
             }
         }

       /***
        * player is hit
        **/
         public void hit(Enemy enemy) {
            // if (mCurrentState == State.Attacking) {
            //	 enemy.die();
            // }

         }

         private void updateSprites(GameTime gameTime)
         {
             timeSinceLastUpdate+=gameTime.ElapsedGameTime.TotalMilliseconds;
             switch (mCurrentState) { 
                 case State.Walking:
                      mSpriteTexture = walkingSheet;
                      
                      if (timeSinceLastUpdate > WALKING_SPEED || (currentAnimIndex%2)!=0)
                      {
                          timeSinceLastUpdate = 0;
                          if (currentAnimIndex >= walkingAnim.Count)
                              currentAnimIndex = 0;

                            mDrawingRectangle = walkingAnim[currentAnimIndex];
                            mTextureData = walkingTextureData[currentAnimIndex];
                            currentAnimIndex++;
                      }
                      break;
                 case State.Standing:
                      mSpriteTexture = idleSheet;
                      mTextureData = idleTextureData[0];
                      mDrawingRectangle = idleAnim[0];
                      break;
                 case State.Attacking:
                      mSpriteTexture = hitSheet;
                      
                      if (timeSinceLastUpdate > ATTACK_SPEED)
                      {
                          timeSinceLastUpdate =0;
                          if (currentAnimIndex == 5) {
                              hurtSurroundingEnemies();
                          }
                          if (currentAnimIndex >= hitAnim.Count)
                          {
                              mCurrentState = State.Standing;
                              break;
                          }

                          mDrawingRectangle = hitAnim[currentAnimIndex];
                          mTextureData = hitTextureData[currentAnimIndex];
                          currentAnimIndex++;
                          
                      }
                      break;
             }
         }

         private void hurtSurroundingEnemies()
         {
             foreach (Enemy enemy in Eggo.getInstance().enemies)
             {
                 Boolean isIntersect = IntersectPixels(this, enemy);
                 Boolean hitRight = (enemy.position.X >= position.X && currentFlip == SpriteEffects.None);
                 Boolean hitLeft= (enemy.position.X <= position.X && currentFlip == SpriteEffects.FlipHorizontally);
                 // Check collision with person
                 if  (isIntersect&& ( hitRight|| hitLeft ))
                 {
                     enemy.die();
                 }
             }

         }

         private void updateMovement(KeyboardState aCurrentKeyboardState)
         {
                 //mSpeed = Vector2.Zero;
                 //mDirection = Vector2.Zero;
             
                 if (aCurrentKeyboardState.IsKeyDown(Keys.Left) == true && mCurrentState != State.Attacking && mCurrentState != State.Jumping)
                 {
                     mCurrentState = State.Walking;
                     mSpeed.X = WALKING_SPEED;
                     mDirection.X = MOVE_LEFT;
                     currentFlip = SpriteEffects.FlipHorizontally;
                 }
                 else if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true && mCurrentState != State.Attacking && mCurrentState != State.Jumping)
                 {
                     mCurrentState = State.Walking;
                     mSpeed.X = WALKING_SPEED;
                     mDirection.X = MOVE_RIGHT;
                     currentFlip = SpriteEffects.None;
                 }

                 if (aCurrentKeyboardState.IsKeyDown(Keys.Up) == true && mCurrentState != State.Attacking && mCurrentState != State.Jumping)
                 {
                 //    mCurrentState = State.Walking;
                 //    mSpeed.Y = WALKING_SPEED;
                 //      mDirection.Y = MOVE_UP;
                 }
                 else if (aCurrentKeyboardState.IsKeyDown(Keys.Down) == true && mCurrentState != State.Attacking && mCurrentState != State.Jumping)
                 {
                   //  mCurrentState = State.Walking;
                  //   mSpeed.Y = WALKING_SPEED;
                  //   mDirection.Y = MOVE_DOWN;
                 }

                 if (//aCurrentKeyboardState.IsKeyUp(Keys.Up) &&
                     aCurrentKeyboardState.IsKeyUp(Keys.Down) &&
                         aCurrentKeyboardState.IsKeyUp(Keys.Left) && aCurrentKeyboardState.IsKeyUp(Keys.Right) && mCurrentState!=State.Attacking && mCurrentState!=State.Jumping)
                 {
                     currentAnimIndex = 0;
                     mCurrentState = State.Standing;
                     mSpeed = Vector2.Zero;
                     mDirection = Vector2.Zero;
                 }

                 if (mPreviousKeyboardState.IsKeyUp(Keys.Space) && aCurrentKeyboardState.IsKeyDown(Keys.Space) && mCurrentState != State.Attacking && mCurrentState != State.Jumping)
                 {
                     mSpeed = Vector2.Zero;
                     mDirection = Vector2.Zero;
                     mCurrentState = State.Attacking;
                     currentAnimIndex = 0;
                 }
             
                 
             
         }

         public override void draw(SpriteBatch spriteBatch)
        {
            /* 
                 VertexPositionColor[] vertices=new VertexPositionColor[((PolygonShape)myBody.FixtureList[0].Shape).Vertices.Count];
             
             for(int i=0;i<((PolygonShape)myBody.FixtureList[0].Shape).Vertices.Count;i++){
                 vertices[i] = new VertexPositionColor(new Vector3(((PolygonShape)myBody.FixtureList[0].Shape).Vertices[i], 1), Color.Black);
             }

             DrawLine(vertices);*/
           // DrawIntersections(spriteBatch);
            spriteBatch.Draw(mSpriteTexture, myBody.Position, mDrawingRectangle, Color.White, 0.0f, new Vector2(0, 0), scale, currentFlip, 0.0f);
        }

        private void DrawIntersections(SpriteBatch spriteBatch ){
        foreach (Enemy enemy in Eggo.getInstance().enemies)
            {
                Rectangle rectangleA = boundingRectangle; Color[] dataA = TextureData;
                Rectangle rectangleB = enemy.boundingRectangle; Color[] dataB = enemy.TextureData;
                // Find the bounds of the rectangle intersection
                
                int top = Math.Max(rectangleA.Top, rectangleB.Top);
                int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
                int left = Math.Max(rectangleA.Left, rectangleB.Left);
                int right = Math.Min(rectangleA.Right, rectangleB.Right);
                if (right - left > 0 && bottom - top > 0)
                {
                    Rectangle intersect = new Rectangle();
                    Rectangle.Intersect(ref rectangleA,ref rectangleB,out intersect);
                    drawRect(intersect, spriteBatch);
                }
                // Check collision with person
            }}

    }
}
