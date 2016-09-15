using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace lab05 {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int GAME_WIDTH = 800;
        public static int GAME_HEIGHT = 800;

        public Camera mainCamera { get; private set; }
        public ModelManager modelManager { get; private set; }
        private Ground ground;

        private Random rand;

        MouseState mouseState;
        MouseState prevMouseState;



        private Tank player;
        private Tank enemy;

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
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
            mainCamera = new Camera(this, new Vector3(0, 200, 75), Vector3.Zero, Vector3.Up);
            Components.Add(mainCamera);
            modelManager = new ModelManager(this);
            Components.Add(modelManager);
            ground = new Ground(Content.Load<Model>(@"Ground/ground"), Vector3.Zero);
            modelManager.AddModel(ground);

            player = new Tank(Content.Load<Model>(@"Models/Tank/tank"), new Vector3(0,0,0));
            modelManager.AddModel(player);
            enemy = new lab05.Tank(Content.Load<Model>(@"Models/Tank/tank"), new Vector3(300, 0, 300));
            modelManager.AddModel(enemy);

            GenerateObstacles(6);

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
                player.SetTargetPosition(PickedPosition());
            }

            if (player.CollidesWith(enemy.model, enemy.GetWorldMatrix())) {
                enemy.TankDestroyed();
            }
            enemy.SetTargetPosition(Behavior.PursueTargetPosition(player.GetPosition(), player.GetVelocityVector(), gameTime, 3));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Gray);

            // TODO: Add your drawing code here

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
        /// Randomly generates a given number of obstacles
        /// </summary>
        /// <param name="numObstacles"></param>
        private void GenerateObstacles(int numObstacles) {
            for (int i = 0; i < numObstacles; i++) {
                modelManager.AddModel(new Obstacle(Content.Load<Model>(@"Models/Obstacle/obstacle"), new Vector3(rand.Next(GAME_WIDTH) - GAME_WIDTH/2, rand.Next(GAME_HEIGHT) - GAME_HEIGHT / 2, 0)));
            }
        }
    }
}
