using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab05 {
    /// <summary>
    /// A variety of static behaviors to be used by non controllable characters
    /// </summary>
    public class Behavior {

        public static Tile EvadeToTile(Tile currentTile, Tile aggressorTile) {
            Tile furthestTile = currentTile.adjacentTiles.First();
            foreach (Tile adjacentTile in currentTile.adjacentTiles) {
                if (Vector3.Distance(adjacentTile.centerPosition, aggressorTile.centerPosition) > Vector3.Distance(furthestTile.centerPosition, aggressorTile.centerPosition)) {
                    furthestTile = adjacentTile;
                }
            }
            return furthestTile;
        }

        /// <summary>
        /// Static method that takes a current position and returns its new position based on 
        /// a simple chase mechanic. A speed of the agent and a reference to the gametime ensures
        /// smooth movement from frame to frame
        /// </summary>
        /// <param name="currentPosition">The current position of the agent</param>
        /// <param name="targetPosition">The position that the agent is chasing</param>
        /// <param name="gameTime">A reference to the current game time</param>
        /// <param name="speed">The speed that the agent is moving</param>
        /// <returns>newPosition</returns>
        public static Vector3 ChaseLocation(Vector3 currentPosition, Vector3 targetPosition, GameTime gameTime, float speed) {

            Vector3 newPosition = new Vector3(currentPosition.X, currentPosition.Y, currentPosition.Z);

            //Y position remains unchanged
            newPosition.Y = currentPosition.Y;

            if (targetPosition.X < currentPosition.X) {
                newPosition.X = currentPosition.X - speed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            }
            else if (targetPosition.X > currentPosition.X) {
                newPosition.X = currentPosition.X + speed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            }
            else {
                //Is in the correct X position therefore DO NOTHING
            }

            if (targetPosition.Y < currentPosition.Y) {
                newPosition.Y = currentPosition.Y - speed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            }
            else if (targetPosition.Y > currentPosition.Y) {
                newPosition.Y = currentPosition.Y + speed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            }
            else {
                //Is in the correct Z position therefore DO NOTHING
            }

            return newPosition;
        }

        /// <summary>
        /// A static method that takes a current position and returns a new position based on the
        /// target destination. This particular method will find the next spot in a straight line
        /// path to the destination
        /// </summary>
        /// <param name="currentPosition">The current position of the agent</param>
        /// <param name="targetPosition">The target position for the agent to head towards</param>
        /// <param name="gameTime">The current game time</param>
        /// <param name="speed">The speed that the agent will move at</param>
        /// <returns></returns>
        public static Vector3 StraightLineChase(Vector3 currentPosition, Vector3 targetPosition, GameTime gameTime, float speed) {
            Vector3 directionOfTravel = Vector3.Normalize(targetPosition - currentPosition);
            Vector3 newPosition = currentPosition + directionOfTravel * speed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            return newPosition;
            
        }

        /// <summary>
        /// A static method that returns the next position according to the arrival steering concept
        /// When the object is outside of the slowing radius it will move at a max speed then when it
        /// is inside the slowing radius it will gradually slow
        /// </summary>
        /// <param name="currentPosition"></param>
        /// <param name="targetPosition"></param>
        /// <param name="gameTime"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="slowRadius"></param>
        /// <returns></returns>
        public static Vector3 ArrivalSteering(Vector3 currentPosition, Vector3 targetPosition, GameTime gameTime, float maxSpeed, float slowRadius) {

            Vector3 directionOfTravel = Vector3.Normalize(targetPosition - currentPosition);
            Vector3 newPosition;
            float currentDistance = (targetPosition - currentPosition).Length();
            if (currentDistance < slowRadius) {
                newPosition = currentPosition + directionOfTravel * (maxSpeed * (currentDistance / slowRadius) * gameTime.ElapsedGameTime.Milliseconds / 1000);
            } else {
                newPosition = currentPosition + directionOfTravel * maxSpeed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            }

            return newPosition;
        }

        public static Vector3 PursueTargetPosition(Vector3 targetPosition, Vector3 targetVelocityVector, GameTime gameTime, int predictionAccuracy) {
            //Predict where the target will be according to its current velocity and prediction accuracy
            Vector3 predictionPosition = targetPosition + targetVelocityVector * predictionAccuracy;
            return predictionPosition;
        }

        /// <summary>
        /// Predict the position that the given target will be
        /// </summary>
        /// <param name="targetPosition">The position of the target</param>
        /// <param name="targetVelocityVector">The velocity of the target</param>
        /// <param name="gameTime">A reference to the game time</param>
        /// <param name="predictionAccuracy">A prediction accuracy variable</param>
        /// <returns>The position that the target will be</returns>
        public static Vector3 PredictTargetPosition(Vector3 targetPosition, Vector3 targetVelocityVector, GameTime gameTime, int predictionAccuracy) {
            //Predict where the target will be according to its current velocity and prediction accuracy
            Vector3 predictionPosition = targetPosition + targetVelocityVector * predictionAccuracy;
            return predictionPosition;
        }

        /// <summary>
        /// Will return a quaternion for the next direction for the object to face
        /// </summary>
        /// <param name="currentRotation"></param>
        /// <param name="targetRotation"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Quaternion TurnToFace(Quaternion currentRotation, Quaternion targetRotation, float speed) {
            return Quaternion.Lerp(currentRotation, targetRotation, speed);
        }

        public static LinkedList<Tile> AStarPathFinding(Tile currentTile, Tile destinationTile, Grid grid) {
            grid.ResetTileCosts();
            LinkedList<Tile> closedSet = new LinkedList<Tile>();
            LinkedList<Tile> openSet = new LinkedList<Tile>();
            float tentativeGScore = 0;
            openSet.AddLast(currentTile);
            currentTile.gScore = 0;
            currentTile.fScore = Heuristic(currentTile, destinationTile);
            LinkedList<Tile> prevTiles = new LinkedList<Tile>();
            Tile current;
            while (openSet.Count != 0) {
                current = openSet.First.Value;
                if (!current.isWalkable) {
                    openSet.Remove(current);
                    continue;
                }
                foreach(Tile tile in openSet) {
                    if (tile.fScore < current.fScore) {
                        current = tile;
                    }
                }
                if (current.Equals(destinationTile)) {
                    break;
                }
                openSet.Remove(current);
                closedSet.AddLast(current);

                foreach (Tile tile in current.adjacentTiles) {
                    if (closedSet.Contains(tile)) {
                        continue;
                    }

                    tentativeGScore = current.gScore + Vector2.Distance(current.gridPosition, tile.gridPosition);
                    if (!(openSet.Contains(tile))) {
                        openSet.AddLast(tile);
                    } else if (tentativeGScore >= tile.gScore) {
                        continue;
                    }

                    tile.cameFrom = current;
                    tile.gScore = tentativeGScore;
                    tile.fScore = tile.gScore + Heuristic(current, destinationTile);

                }

            }
            
            LinkedList<Tile> path = new LinkedList<Tile>();
            Tile workBackTile = destinationTile;
            while (workBackTile != currentTile) {
                if (workBackTile == null) {
                    Debug.WriteLine("There is no path to the target");
                    return new LinkedList<Tile>();
                }
                path.AddFirst(workBackTile);
                workBackTile = workBackTile.cameFrom;
            }
            
            return path;
            

        }

        private static float Heuristic(Tile currentTile, Tile destinationTile) {
            return (Vector2.Distance(currentTile.gridPosition, destinationTile.gridPosition));
        }

        /// <summary>
        /// Creates a quaternion to face the direction of another position in the world
        /// Is a static method so both the objects position and the target position must be provided
        /// </summary>
        /// <param name="objectPosition">The current position of the model</param>
        /// <param name="targetPosition">The position that the object would like to face</param>
        /// <param name="up">The up vector of the object that you are hoping to rotate</param>
        /// <returns>A Quaternion of the direction that is needed to face the position</returns>
        public static Quaternion RotateToFace(Vector3 objectPosition, Vector3 targetPosition, Vector3 up) {
            Vector3 direction = (objectPosition - targetPosition);
            Vector3 relativeRight = Vector3.Cross(up, direction);
            Vector3.Normalize(ref relativeRight, out relativeRight);
            Vector3 relativeBackwards = Vector3.Cross(relativeRight, up);
            Vector3.Normalize(ref relativeBackwards, out relativeBackwards);
            Vector3 newUp = Vector3.Cross(relativeBackwards, relativeRight);
            Matrix rot = new Matrix(relativeRight.X, relativeRight.Y, relativeRight.Z, 0, newUp.X, newUp.Y, newUp.Z, 0, relativeBackwards.X, relativeBackwards.Y, relativeBackwards.Z, 0, 0, 0, 0, 1);
            Quaternion rotation = Quaternion.CreateFromRotationMatrix(rot);
            return rotation;
        }

    }
}
