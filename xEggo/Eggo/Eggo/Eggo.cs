using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Eggo.Entities;
using FarseerPhysics.Dynamics;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics.Joints;


namespace Eggo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Eggo : Microsoft.Xna.Framework.Game
    {

        static Eggo instance;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        // The sub-rectangle of the drawable area which should be visible on all TVs
        public static Rectangle safeBounds;
        // Percentage of the screen on every side is the safe area
        const float SafeAreaPortion = 0.04f;
        Random random = new Random();
        Ground ground;
        float enemySpawnProbability = 0.01f;
        float enemyTypeProbability = 0.5f;

        SpriteFont font;
        Vector2 scorePos;
        public static int score = 0;
        public static World world;
        Player player;
        Texture2D background;
        public AssetCreator assetCreator;
        // Blocks
        public List<Enemy> enemies = new List<Enemy>();


        public static Eggo getInstance() { if (instance == null) return new Eggo(); else return instance; }
        private Eggo()
        {
            instance = this;
            world = null;
            
            graphics = new GraphicsDeviceManager(this);

            

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            if (world == null)
            {
                world = new World(Vector2.Zero);
            }
            else
            {
                world.Clear();
            }
            world.Gravity = new Vector2(0f, 20f);

            // TODO: Add your initialization logic here
            // Calculate safe bounds based on current resolution
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            safeBounds = new Rectangle(
                (int)(viewport.Width * SafeAreaPortion),
                (int)(viewport.Height * SafeAreaPortion),
                (int)(viewport.Width * (1 - 2 * SafeAreaPortion)),
                (int)(viewport.Height * (1 - 2 * SafeAreaPortion)));

            player = new Player();
            ground = new Ground();

            assetCreator = new AssetCreator(GraphicsDevice);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
            assetCreator.LoadContent(this.Content);
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Courier New");
            background = Content.Load<Texture2D>("background-800 480");
            // TODO: Load your game content here            
            scorePos = new Vector2(graphics.GraphicsDevice.Viewport.Width /2, 10);

            // TODO: use this.Content to load your game content here
            player.load(this.Content);

            ground.load(this.Content);

            // Loading may take a while... so prevent the game from "catching up" once we finished loading
            this.ResetElapsedTime();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // variable time step but never less then 30 Hz
            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Spawn new enemies
            if (random.NextDouble() < enemySpawnProbability)
            {
                //Only zombies are supported now
                Enemy enemy = new ZombieEnemy();
                
                if (random.NextDouble() > enemyTypeProbability)
                {
                    float y = (float)random.NextDouble() *
                        (Window.ClientBounds.Height - enemy.boundingRectangle.Height);
                    enemy.position = new Vector2(Window.ClientBounds.Width, y);
                }
                else {
                    float x = (float)random.NextDouble() *
                    (Window.ClientBounds.Width - enemy.boundingRectangle.Width);
                    enemy.position = new Vector2(x, 0);
                    ((ZombieEnemy)enemy).isFalling = true;
                }
                enemy.load(this.Content);
                enemies.Add(enemy);
            }

            // TODO: Add your update logic here
            player.update(gameTime);
            ground.update(gameTime);
            //update enemies and Check for collision
            foreach (Enemy enemy in enemies){
                enemy.update(gameTime);

                // Check collision with person
                if (GameObject.IntersectPixels( player,enemy))
                {
                    player.hit(enemy);
                    enemy.hit();
                }

                // Remove this enemy if it have fallen off the screen
                if (enemy.position.X < 0 || enemy.position.Y > Window.ClientBounds.Height)
                {
                    enemies.Remove(enemy);
                    Eggo.world.RemoveBody(enemy.Body);
                    break;
                }
            }
            base.Update(gameTime);
        }

        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
              
            GraphicsDevice.Clear(Color.CadetBlue);
            spriteBatch.Draw(background, new Vector2(0, 0), Color.Wheat);
            
            player.draw(spriteBatch);
            ground.draw(spriteBatch);
            foreach (Enemy enemy in enemies) { 
                enemy.draw(spriteBatch);
                //enemy.drawBoundingBox(spriteBatch, GraphicsDevice); 
            }

            spriteBatch.DrawString(font,"Score : "+score,scorePos,Color.WhiteSmoke);
            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
