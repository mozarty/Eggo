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
    abstract public class GameObject
    {
        protected const int MOVE_UP = -1;
        protected const int MOVE_DOWN = 1;
        protected const int MOVE_LEFT = -1;
        protected const int MOVE_RIGHT = 1;
        protected int currentAnimIndex = 0;
        protected float scale = 1f;
        protected Body myBody;
        protected double timeSinceLastUpdate = 0;
        protected SpriteEffects currentFlip = SpriteEffects.None;
        //TODO remove and use body.position after applying physics 
        protected Vector2 mDirection = Vector2.Zero;
        
        protected Vector2 mSpeed = Vector2.Zero;
        protected State mCurrentState = State.Standing;
        protected Vector2 mActionStartingPosition = Vector2.Zero;
        protected enum State
        {
            Standing,
            Walking,
            Attacking,
            Jumping,
            Dying
        }
        protected KeyboardState mPreviousKeyboardState;
        protected KeyboardState aCurrentKeyboardState;
        protected Rectangle mDrawingRectangle;
        public Rectangle drawingRectangle
        {
            get
            {
                return new Rectangle(mDrawingRectangle.X, mDrawingRectangle.Y, (int)(mDrawingRectangle.Width * scale), (int)(mDrawingRectangle.Height * scale));
            }
        }
        public Rectangle boundingRectangle
        {
            get
            {
                return new Rectangle((int)myBody.Position.X, (int)myBody.Position.Y, (int)(mDrawingRectangle.Width * scale), (int)(mDrawingRectangle.Height * scale));
            }
        }
        public Vector2 position
        {
            get
            {
                return myBody.Position;
            }
            set
            {
                this.myBody.Position = value;
            }
        }
        public Body Body
        {
            get
            {
                return myBody;
            }
         }
        protected Texture2D mSpriteTexture;
        protected Color[] mTextureData;
        public Color[] TextureData
        {
            get
            {
                return mTextureData;
            }
        }


        public GameObject() {
            myBody = BodyFactory.CreateRectangle(Eggo.world, 1f, 1f, 1f);
        }

        abstract public void load(ContentManager content);

        virtual public void update(GameTime gameTime, Vector2 mSpeed, Vector2 mDirection)
        {

            myBody.Position += mDirection * mSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            mPreviousKeyboardState = aCurrentKeyboardState;
        }

        abstract public void draw(SpriteBatch spriteBatch);

        public void drawBoundingBox(SpriteBatch spriteBatch)
        {
            drawRect(boundingRectangle, spriteBatch);
        }
        public void drawRect(Rectangle rect,SpriteBatch spriteBatch)
        {
            Texture2D dummyTexture;
            dummyTexture = new Texture2D(Eggo.getInstance().GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.Black });
            spriteBatch.Draw(dummyTexture, rect, Color.Red);
        }

        public void DrawLine(VertexPositionColor[] Vertices)
        {
            Eggo.getInstance().GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Vector2 center;
            center.X = Eggo.getInstance().GraphicsDevice.Viewport.Width * 0.5f;
            center.Y = Eggo.getInstance().GraphicsDevice.Viewport.Height * 0.5f;

            Matrix View = Matrix.CreateLookAt(new Vector3(center, 0), new Vector3(center, 1), new Vector3(0, -1, 0));
            Matrix Projection = Matrix.CreateOrthographic(center.X * 2, center.Y * 2, -0.5f, 1);
            Effect EffectLines = Eggo.getInstance().Content.Load<Effect>("lines");
            EffectLines.CurrentTechnique = EffectLines.Techniques["Lines"];

            EffectLines.Parameters["xViewProjection"].SetValue(View * Projection);
            EffectLines.Parameters["xWorld"].SetValue(Matrix.Identity);

            foreach (EffectPass pass in EffectLines.CurrentTechnique.Passes)
            {
                pass.Apply();
                Eggo.getInstance().GraphicsDevice.DrawUserPrimitives<VertexPositionColor>
                    (PrimitiveType.LineList, Vertices, 0, Vertices.Length / 2);
            }
        }

        public Body CreateBodyFromImage(ContentManager content, Texture2D polygonTexture)
        {

            //Use an array to hold the textures data.
            uint[] data = new uint[polygonTexture.Width * polygonTexture.Height];

            //Transfer the texture data into the array.
            polygonTexture.GetData(data);

            //Find the verticals that make up the outline of the passed texture shape.
            Vertices vertices = PolygonTools.CreatePolygon(data, polygonTexture.Width);

            //For now we need to scale the vertices (result is in pixels, we use meters)
            Vector2 vscale = new Vector2(scale, scale);
            vertices.Scale(ref vscale);

            //Partition the concave polygon into a convex one.
            var decomposedVertices = BayazitDecomposer.ConvexPartition(vertices);

            //Create a single body, that has multiple fixtures to the polygon shapes.
            return BodyFactory.CreateCompoundPolygon(Eggo.world, decomposedVertices, 1f);

        }



        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(GameObject A,GameObject B)
        {
            Rectangle rectangleA = A.boundingRectangle; Color[] dataA = A.TextureData;
            Rectangle rectangleB = B.boundingRectangle; Color[] dataB = B.TextureData;
            // Find the bounds of the rectangle intersection
            Rectangle intersect = new Rectangle();
            Rectangle.Intersect(ref rectangleA, ref rectangleB, out intersect);
            int top = intersect.Top;
            int bottom = intersect.Bottom;
            int left = intersect.Left;
            int right = intersect.Right;
            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                   int realXforA = (int)(x/A.scale);
                   int realXforB = (int)(x / B.scale);
                   int realYforA = (int)(y / A.scale);
                   int realYforB = (int)(y / B.scale);

                   int colorAIndex = (x - rectangleA.Left) +
                                        (y - rectangleA.Top) * A.mDrawingRectangle.Width;
                   int colorBIndex = (x - rectangleB.Left) +
                                        (y - rectangleB.Top) * B.mDrawingRectangle.Width;

                    // Get the color of both pixels at this point
                    Color colorA = dataA[colorAIndex];
                    Color colorB = dataB[colorBIndex];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }

        protected void fillTextureData(List<Color[]> result, List<Rectangle> rects, Texture2D source)
        {
            for (int i = 0; i < rects.Count;i++ )
                {
                    Rectangle rect = rects[i];
                    int count = rect.Width * rect.Height;
                    Color[] retrievedColor = new Color[count];

                    source.GetData<Color>(
                        0,
                        rect,
                        retrievedColor,
                        0,
                        count);
                    result.Add(retrievedColor);
                }
        }
    }
}
