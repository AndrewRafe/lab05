using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab05 {
    public class Tank : BasicModel {

        public float speed { get; private set; }
        private Vector3 prevPosition;
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private LinkedList<Tile> targetCheckpointPositions;
        private ModelManager bulletsFired;
        private Game game;
        private Tank targetTank;

        public bool isDestroyed { get; private set;  }

        private int millisecondsSinceLastShot;
        private int rateOfFire = 2000;

        /// <summary>
        /// Constructor method for the Tank which sets its current position in the world and the model
        /// </summary>
        /// <param name="m">The tank model</param>
        /// <param name="position">The Vector3 position of the tank</param>
        public Tank(Model m, Vector3 position, Game game, Tank targetTank) : base(m, position) {
            targetPosition = new Vector3(0, 0, 0);
            speed = 100.0f;
            isDestroyed = false;
            targetCheckpointPositions = new LinkedList<Tile>();
            millisecondsSinceLastShot = 0;
            bulletsFired = new ModelManager(game);
            this.game = game;
            this.targetTank = targetTank;
        }

        /// <summary>
        /// Will set the target position that the tank will head towards
        /// </summary>
        /// <param name="targetPosition">The position that the target will move towards</param>
        public void SetTargetPosition(Tile currentPosition, Tile targetPosition, Grid grid) {
            if (targetPosition == null) {
                return;
            } else if (!targetPosition.isWalkable) {
                Debug.WriteLine("That tile is not walkable");
                return;
            }
            this.targetPosition = targetPosition.centerPosition;
            //targetRotation = RotateToFace(position, this.targetPosition, Vector3.UnitY);
            PopulateCheckpointPositions(currentPosition, targetPosition, grid);
            
        }

        /// <summary>
        /// Will generate a list of positions that the tank has to travel through
        /// </summary>
        public void PopulateCheckpointPositions(Tile currentPosition, Tile targetPosition, Grid grid) {
            targetCheckpointPositions = Behavior.AStarPathFinding(currentPosition, targetPosition, grid);
            foreach (Tile t in targetCheckpointPositions) {
                Debug.WriteLine(t.ToString());
            }
            Debug.WriteLine("-----------");
        }

        /// <summary>
        /// Override of the update method that will move the tank towards the target position
        /// </summary>
        /// <param name="gameTime">A reference to the game time</param>
        public override void Update(GameTime gameTime) {
            prevPosition = position;
            if (targetCheckpointPositions.Count != 0) {
                if (isDestroyed == false) {
                    Vector3 nextTargetPosition = new Vector3(targetCheckpointPositions.First.Value.centerPosition.X, targetCheckpointPositions.First.Value.centerPosition.Z, targetCheckpointPositions.First.Value.centerPosition.Y);
                    RotateToFace(nextTargetPosition, Vector3.UnitY);
                    if (targetCheckpointPositions.Count == 1) {
                        position = Behavior.ArrivalSteering(position, nextTargetPosition, gameTime, speed, 50);
                    } else {
                        position = Behavior.StraightLineChase(position, nextTargetPosition, gameTime, speed);
                    }
                    if (Math.Abs(position.X - nextTargetPosition.X) <= 5 && Math.Abs(position.Z - nextTargetPosition.Z) <= 5) {
                        targetCheckpointPositions.RemoveFirst();
                    }
                }
                else {
                    
                }
            }

            millisecondsSinceLastShot += gameTime.ElapsedGameTime.Milliseconds;
            if (millisecondsSinceLastShot >= rateOfFire) {
                millisecondsSinceLastShot = rateOfFire;
            }

            base.Update(gameTime);
            bulletsFired.Update(gameTime);
        }

        public override void Draw(Camera camera, GameTime gameTime) {
            bulletsFired.Draw(gameTime);
            base.Draw(camera, gameTime); 
        }

        /// <summary>
        /// Gets the current velocity vector
        /// </summary>
        /// <returns>The velocity of the tank</returns>
        public Vector3 GetVelocityVector() {
            return position - prevPosition;
        }

        /// <summary>
        /// Sets the destroyed condition to be true
        /// </summary>
        public void TankDestroyed() {
            isDestroyed = true;
        }

        public override Matrix GetWorldMatrix() {
            return base.GetWorldMatrix();
        }

        public void FireWeapon(GameTime gameTime) {
            if (millisecondsSinceLastShot >= rateOfFire) {
                bulletsFired.AddModel(new Bullet(game.Content.Load<Model>(@"Models/Bullet/cannonBall"), new Vector3(position.X, -position.Z, position.Y), targetTank, gameTime, 10.0f));
                millisecondsSinceLastShot = 0;
            }
        }

    }
}
