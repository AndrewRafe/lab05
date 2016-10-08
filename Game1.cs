using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace lab05 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int GAME_WIDTH = 1000;
        public static int GAME_HEIGHT = 1000;
        public static float TILE_SIZE = 50.0f;

        public Camera mainCamera { get; private set; }
        public ModelManager modelManager { get; private set; }
        private Ground ground;

        public Random rand;
        private Grid grid;

        MouseState mouseState;
        MouseState prevMouseState;



        private Tank player;
        private NPC enemy;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1900;
            graphics.PreferredBackBufferHeight = 1000;
            graphics.IsFullScreen = false;
            rand = new Random();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            grid = new Grid(TILE_SIZE, GAME_WIDTH / (int)TILE_SIZE, GAME_HEIGHT / (int)TILE_SIZE, Vector3.Zero);
            grid.GenerateGrid(rand, Content.Load<Model>(@"Models/Obstacle/obstacle"));
            mainCamera = new Camera(this, new Vector3(0, 600, 100), Vector3.Zero, Vector3.UnitY);
            Components.Add(mainCamera);
            modelManager = new ModelManager(this);
            Components.Add(modelManager);
            ground = new Ground(Content.Load<Model>(@"Ground/ground"), new Vector3(0, 0, -TILE_SIZE/2));
            modelManager.AddModel(ground);

            player = new Tank(Content.Load<Model>(@"Models/Tank/tank"), new Vector3(0,0,0), this, enemy);
            modelManager.AddModel(player);

            enemy = new NPC(Content.Load<Model>(@"Models/Tank/tank"), new Vector3(300, 0, 300), player, grid, this);
            modelManager.AddModel(enemy);

            
            Debug.Write(grid.ToString());
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released) {
                player.SetTargetPosition(grid.PickedTile(new Vector3(player.position.X, player.position.Y, player.position.Z)), grid.PickedTile(PickedPosition()), grid);
            }

            // DEBUG: Right CLick to print the tile clicked to the debug 
            if (mouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released) {
                Debug.WriteLine(grid.PickedTile(PickedPosition()).ToString());
                Debug.WriteLine("Tank at position: " + grid.PickedTile(player.GetPosition()) + " " + player.ToString());
            }

            //Debug.WriteLine(player.GetPosition().ToString());
            prevMouseState = mouseState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Gray);

            // TODO: Add your drawing code here
            grid.Draw(PickedPosition(), mainCamera, gameTime);
            base.Draw(gameTime);
        }

        private Vector3 PickedPosition() {

            Vector3 nearsource = new Vector3((float)mouseState.Position.X, (float)mouseState.Position.Y, 0f);
            Vector3 farsource = new Vector3((float)mouseState.Position.X, (float)mouseState.Position.Y, 1f);

            Matrix world = Matrix.CreateTranslation(0, 0, 0);

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, mainCamera.projection, mainCamera.view, world);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, mainCamera.projection, mainCamera.view, world);

            // Create a ray from the near clip plane to the far clip plane.
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            Ray pickRay = new Ray(nearPoint, direction);

            // calcuate distance of plane intersection point from ray origin
            float? distance = pickRay.Intersects(ground.groundPlane);

            if (distance != null) {
                Vector3 pickedPosition = nearPoint + direction * (float)distance;
                return pickedPosition;
            }
            else {
                return Vector3.Zero;
            }
        }

        /// <summary>
        /// Private helper method to conver the mouse picked position to the global coordinate system
        /// </summary>
        /// <param name="pickedPosition">The picked position of the mouse</param>
        /// <returns>The fix to fit the global coordinate system</returns>
        public static Vector3 PickedPositionTranslation(Vector3 pickedPosition) {
            return new Vector3(pickedPosition.X, -pickedPosition.Z, pickedPosition.Y);
        }

    }
}
