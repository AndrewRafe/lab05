using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab05 {
    class Tank : BasicModel {

        public float speed { get; private set; }
        private Vector3 prevPosition;
        private Vector3 targetPosition;
        private Quaternion targetRotation;

        public bool isDestroyed { get; private set;  }

        public Tank(Model m, Vector3 position) : base(m, position) {
            targetPosition = new Vector3(0, 0, 0);
            speed = 50.0f;
            isDestroyed = false;
        }

        /// <summary>
        /// Will set the target position that the tank will head towards
        /// </summary>
        /// <param name="targetPosition">The position that the target will move towards</param>
        public void SetTargetPosition(Vector3 targetPosition) {
            this.targetPosition = targetPosition;
            targetRotation = RotateToFace(position, targetPosition, Vector3.UnitY);
        }

        /// <summary>
        /// Override of the update method that will move the tank towards the target position
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime) {
            prevPosition = position;
            if (position != targetPosition && isDestroyed == false) {
                position = Behavior.ArrivalSteering(position, targetPosition, gameTime, speed, 50.0f);
                rotation = Behavior.TurnToFace(rotation, targetRotation, 0.1f);
            }

            base.Update(gameTime);
        }

        public Vector3 GetVelocityVector() {
            return position - prevPosition;
        }

        public void TankDestroyed() {
            isDestroyed = true;
        }

        

    }
}
