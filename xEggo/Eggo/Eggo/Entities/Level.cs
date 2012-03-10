using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Eggo.Entities
{
    class Level
    {
        static Level instance;
        private int levelNumber;
        private int levelDuration;
        private int maxEnemiesToSpawn;
        private Vector2 mapSize;
        private Vector2 playerPos;
        private List<InitialEnemy> initialEnemies;
        private List<LevelEnemy> levelEnemies;
        private int[] AllowedObjects;
        private int[] AllowedDObjects;
        private List<Obstacle> mapObstacles;
        private List<HotSpot> mapHotSpots;



        private Random random;

        private Texture2D background;
        public static Player player;
        Ground ground;
        public List<Enemy> enemies;

        public static Level GetInstance()
        {
            if (instance == null)
            {
                return new Level("Levels/TheInsider_XML");
            }
            else
            {
                return instance;
            }
        }

        public Level(string xmlFile)
        {
            random = new Random();
            instance = this;
            //Read the XML File and load it's information into the Level Class
            StreamReader sr = new StreamReader(xmlFile);
            string xmlContent = sr.ReadToEnd();
            XDocument obj = XDocument.Parse(xmlContent);
            int.TryParse(obj.Descendants("number").First().Value, out this.levelNumber);
            int.TryParse(obj.Descendants("maxTime").First().Value, out this.levelDuration);
            float.TryParse(obj.Descendants("size").First().FirstAttribute.Value, out mapSize.X);
            float.TryParse(obj.Descendants("size").First().LastAttribute.Value, out mapSize.Y);
            float.TryParse(obj.Descendants("playerPosition").First().Value, out playerPos.X);
            //TODO: Load the player Y position based on the level number
            //Load the obstacles
            var obstacles = obj.Descendants("obstacle");
            mapObstacles = new List<Obstacle>();
            foreach (var o in obstacles)
            {
                Obstacle ob = new Obstacle();
                Vector2 pos = new Vector2();
                float.TryParse(o.FirstAttribute.Value, out pos.X);
                float.TryParse(o.Attributes().ElementAt(1).Value, out pos.Y);
                int t;
                int.TryParse(o.LastAttribute.Value, out t);
                ob.Position = pos;
                ob.Type = t;
                mapObstacles.Add(ob);
            }

            //Load the HotSpots
            var hotspots = obj.Descendants("hotspot");
            mapHotSpots = new List<HotSpot>();
            foreach (var h in hotspots)
            {
                HotSpot hs = new HotSpot();
                Vector2 pos = new Vector2();
                float.TryParse(h.FirstAttribute.Value, out pos.X);
                float.TryParse(h.Attributes().ElementAt(1).Value, out pos.Y);
                int t;
                int.TryParse(h.LastAttribute.Value, out t);
                hs.Position = pos;
                hs.Type = t;
                mapHotSpots.Add(hs);
            }

            //Load the allowed objects
            char[] sep = { ',' };
            string allowedObjsString = obj.Descendants("objects").First().Value;
            string[] allowedObjStringList = allowedObjsString.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            AllowedObjects = new int[allowedObjStringList.Length];
            for (int i = 0; i < allowedObjStringList.Length; i++)
            {
                AllowedObjects[i] = int.Parse(allowedObjStringList[i]);
            }
            
            //Load the dangerous allowed objects
            string allowedDObjsString = obj.Descendants("dangerousObjects").First().Value;
            string[] allowedDObjStringList = allowedDObjsString.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            AllowedDObjects = new int[allowedDObjStringList.Length];
            for (int i = 0; i < allowedDObjStringList.Length; i++)
            {
                AllowedDObjects[i] = int.Parse(allowedDObjStringList[i]);
            }

            int.TryParse(obj.Descendants("maxNumber").First().Value, out maxEnemiesToSpawn);

            //Load the initial enemies list
            var initialEn = obj.Descendants("initialEnemy");
            initialEnemies = new List<InitialEnemy>();
            foreach (var i in initialEn)
            {
                int type = int.Parse(i.FirstAttribute.Value);
                int number = int.Parse(i.Attributes().ElementAt(1).Value);
                string behaviours = i.LastAttribute.Value;
                InitialEnemy en = new InitialEnemy(type, number, behaviours);
                this.initialEnemies.Add(en);
            }

            //Load the enemies types and constraints
            var enList = obj.Descendants("enemy");
            levelEnemies = new List<LevelEnemy>();
            foreach (var e in enList)
            {
                int type = int.Parse(e.FirstAttribute.Value);
                int maxNum = int.Parse(e.Attributes().ElementAt(1).Value);
                int dir = int.Parse(e.Attributes().ElementAt(2).Value);
                string behaviours = e.LastAttribute.Value;
                LevelEnemy en = new LevelEnemy(type, maxNum, behaviours, dir);
                var enConstraints = e.Descendants("constraint");
                foreach (var c in enConstraints)
                {
                    int cType = int.Parse(c.FirstAttribute.Value);
                    double prob = double.Parse(c.LastAttribute.Value);
                    GenerationConstraint constraint = new GenerationConstraint(cType, prob);
                    en.AddConstraint(constraint);
                }
                levelEnemies.Add(en);
            }


            if (player == null)
                player = new Player((int)playerPos.X);
            else
                player.position = new Vector2((int)playerPos.X, player.position.Y);
            ground = new Ground();
            enemies = new List<Enemy>();

        }

        public void Initialize()
        {
        }

        public void Load(ContentManager content, GameWindow Window)
        {
            background = content.Load<Texture2D>("background-800 480");

            //Load the initial enemies
            foreach (var e in initialEnemies)
            {
                switch (e.Type)
                {
                    case 1:
                        for (int i = 0; i < e.Number; i++)
                        {
                            ZombieEnemy z = new ZombieEnemy();
                            int currentBehaviour = e.EntryBehaviours[0];
                            if (e.EntryBehaviours.Length == 2)
                            {
                                if (random.NextDouble() > 0.5)
                                {
                                    currentBehaviour = e.EntryBehaviours[1];
                                }
                            }
                            if (currentBehaviour == 0)
                            {
                                float x = (float)random.NextDouble() *
                                    (Window.ClientBounds.Width - z.boundingRectangle.Width);
                                z.position = new Vector2(x, 0);
                                z.isFalling = true;
                                enemies.Add(z);
                            }
                            else if (currentBehaviour == 1)
                            {
                                //TODO: make it depend on the level size
                                float y = (float)random.NextDouble() *
                                    (Window.ClientBounds.Height - z.boundingRectangle.Height);
                                z.position = new Vector2(Window.ClientBounds.Width - y/2, 350);
                                z.isFalling = false;
                                enemies.Add(z);
                            }
                            ZombieEnemy.numberOfAliveEnemies++;
                        }
                        maxEnemiesToSpawn -= e.Number;
                        break;
                    //TODO: Add the rest of enemies types
                    default:
                        break;
                }
            }
            //Load player and ground
            player.load(content);
            ground.load(content);
            //Load the enemies in the list of alive enemies
            foreach (var en in enemies)
            {
                en.load(content);
            }
        }

        public void Update(ContentManager content, GameTime gameTime, GameWindow Window)
        {
            player.update(gameTime);
            ground.update(gameTime);
            //update enemies and Check for collision
            foreach (Enemy enemy in enemies)
            {
                enemy.update(gameTime);

                // Check collision with person
                if ( player.boundingRectangle.Intersects(enemy.boundingRectangle))
                {
                    player.hit(enemy);
                    enemy.hit();
                }

                // Remove this enemy if it have fallen off the screen
                if (enemy.position.X < 0 || enemy.position.Y > Window.ClientBounds.Height)
                {
                    enemies.Remove(enemy);
                    Eggo.world.RemoveBody(enemy.Body);
                    ZombieEnemy.numberOfAliveEnemies--;
                    //Load another enemy
                    //if (ZombieEnemy.numberOfAliveEnemies < levelEnemies[0].MaxAlive && maxEnemiesToSpawn >= 0)
                    {
                        ZombieEnemy en = new ZombieEnemy();
                        float y = (float)random.NextDouble() *
                                    (Window.ClientBounds.Height - en.boundingRectangle.Height);
                        en.position = new Vector2(Window.ClientBounds.Width - y/2, 350);
                        en.isFalling = false;
                        enemies.Add(en);
                        en.load(content);
                        ZombieEnemy.numberOfAliveEnemies++;
                        maxEnemiesToSpawn--;
                    }
                    break;
                }

            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Vector2(0, 0), Color.Wheat);
            player.draw(spriteBatch);
            ground.draw(spriteBatch);
            foreach (Enemy enemy in enemies)
            {
                enemy.draw(spriteBatch);
            }
        }

    }

    class InitialEnemy
    {
        private int type;
        private int[] entryBehaviour; //0: falling from the ceiling 1: coming from left or right
        private int number;
        public InitialEnemy(int type, int num, string behaviours)
        {
            char[] sep = { ',' };
            string[] entriesStrings = behaviours.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            entryBehaviour = new int[entriesStrings.Length];
            for (int i = 0; i < entryBehaviour.Length; i++)
            {
                entryBehaviour[i] = int.Parse(entriesStrings[i]);
            }
            this.type = type;
            this.number = num;
        }

        public int Type
        {
            get
            {
                return this.type;
            }
        }

        public int[] EntryBehaviours
        {
            get
            {
                return this.entryBehaviour;
            }
        }

        public int Number
        {
            get
            {
                return this.number;
            }
        }
    }

    class LevelEnemy
    {
        private int type;
        private int maxAlive;
        private int[] entryBehaviour;
        private int direction; //0 for stiller, 1 for left, 2 for right
        private List<GenerationConstraint> constraints;

        public LevelEnemy(int type, int max, string behaviours, int dir)
        {
            char[] sep = { ',' };
            string[] entriesStrings = behaviours.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            entryBehaviour = new int[entriesStrings.Length];
            for (int i = 0; i < entryBehaviour.Length; i++)
            {
                entryBehaviour[i] = int.Parse(entriesStrings[i]);
            }
            this.type = type;
            this.maxAlive = max;
            this.direction = dir;
            this.constraints = new List<GenerationConstraint>();
        }

        public void AddConstraint(GenerationConstraint contraint)
        {
            this.constraints.Add(contraint);
        }

        public List<GenerationConstraint> Contraints
        {
            get
            {
                return this.constraints;
            }
        }

        public int[] EntrBehaviours
        {
            get
            {
                return this.entryBehaviour;
            }
        }

        public int Type
        {
            get
            {
                return this.type;
            }
        }

        public int MaxAlive
        {
            get
            {
                return this.maxAlive;
            }
        }

        public int Direction
        {
            get
            {
                return this.direction;
            }
        }


    }

    class GenerationConstraint
    {
        private int type;
        private double probability;

        public GenerationConstraint(int type, double prob)
        {
            this.type = type;
            this.probability = prob;
        }

        public int Type
        {
            get
            {
                return this.type;
            }
        }

        public double Probability
        {
            get
            {
                return this.probability;
            }
        }
    }
}
