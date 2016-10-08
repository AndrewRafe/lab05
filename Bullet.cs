using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab05 {
    /// <summary>
    /// Bullet class to maintain the position and the characteristics of a bullet
    /// </summary>
    class Bullet : BasicModel {

        public float damage { get; private set; }

        private Vector3 directionOfTravel;

        public float speed { get; private set; }
        public Tank targetTank { get; private set; }

        /// <summary>
        /// Constructor method for the bullet class
        /// Takes the regular basic model parameters and sets the bullets course towards an enemy
        /// </summary>
        /// <param name="m">Bullet Model</param>
        /// <param name="position">Starting position of the bullet</param>
        /// <param name="targetEnemy">The enemy that the bullet is directed at</param>
        /// <param name="tower">The tower needed to be protected</param>
        /// <param name="gameTime">A reference to the game time</param>
        public Bullet(Model m, Vector3 position, Tank targetTank, GameTime gameTime, float damage) : base(m, position) {
            this.targetTank = targetTank;
            this.speed = 250.0f;
            this.damage = damage;
            CreateDirectionOfTravel(gameTime);
        }

        /// <summary>
        /// Determines the direction that the bullet is going to travel
        /// </summary>
        private void CreateDirectionOfTravel(GameTime gameTime) {

            directionOfTravel = Vector3.Normalize(position - targetTank.position);
        }

        /// <summary>
        /// The algorithm used to calculate the accuracy variable for the prediction of the moving enemy
        /// </summary>
        /// <returns></returns>
        private int CalculatePredictionAccuracy() {
            return (int)Vector3.Distance(position, targetTank.position) / 10 * 2;
        }

        /// <summary>
        /// Updates the position of the bullet based on its current trajectory
        /// </summary>
        /// <param name="gameTime">A reference to the game time</param>
        public override void Update(GameTime gameTime) {
            this.position += directionOfTravel * speed * gameTime.ElapsedGameTime.Milliseconds / 1000;
            base.Update(gameTime);
        }


    }
}
