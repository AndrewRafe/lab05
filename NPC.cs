using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab05 {
    public class NPC : Tank {

        private const float DEFAULT_HEALTH = 100.0f;
        private const float CRITICAL_HEALTH = 20.0f;
        private const String STATE_IDLE = "IDLE";
        private const String STATE_CHASE = "CHASE";
        private const String STATE_EVADE = "EVADE";
        private const String STATE_SHOOT = "SHOOT";

        private Tank player;

        private float health;
        private String currentState;
        private Grid grid;

        public NPC(Model m, Vector3 position, Tank player, Grid grid, Game game) : base(m, position, game, player) {
            this.health = DEFAULT_HEALTH;
            this.player = player;
            this.grid = grid;
            this.currentState = STATE_IDLE;
        }

        public override void Update(GameTime gameTime) {
            if (currentState == STATE_IDLE) {
                if (CanSeePlayer()) {
                    currentState = STATE_CHASE;
                }
            } else if (currentState == STATE_CHASE) {
                this.SetTargetPosition(grid.PickedTile(this.position), grid.PickedTile(player.position), grid);
                if (WithinRangeOfPlayer()) {
                    currentState = STATE_SHOOT;
                } else if (CriticalHealth()) {
                    currentState = STATE_EVADE;
                }
            } else if (currentState == STATE_SHOOT) {
                this.FireWeapon(gameTime);
                if (!WithinRangeOfPlayer()) {
                    currentState = STATE_CHASE;
                } else if (CriticalHealth()) {
                    currentState = STATE_EVADE;
                }
            } else if (currentState == STATE_EVADE) {

            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Will return true if the NPC can see the player
        /// </summary>
        /// <returns></returns>
        private bool CanSeePlayer() {

            return true;
        }

        /// <summary>
        /// Will return true if the NPC is in range of the player
        /// </summary>
        /// <returns></returns>
        private bool WithinRangeOfPlayer() {
            if (Vector3.Distance(position, player.position) < 200) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Will return true if the current NPC is on critical health
        /// </summary>
        /// <returns></returns>
        private bool CriticalHealth() {
            if (health <= CRITICAL_HEALTH) {
                return true;
            } else {
                return false;
            }
        }

    }
}
