using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// An adjacency list style implementation of a grid of tiles
/// So each tile will have a list of tiles that it is adjacent to
/// </summary>
namespace lab05 {
    public class Grid {

        private float tileSize;
        private int height;
        private int width;
        public Vector3 gridCenter;
        public LinkedList<Tile> grid { get; private set; }

        public Grid(float tileSize, int height, int width, Vector3 gridCenter) {
            this.tileSize = tileSize;
            this.height = height;
            this.width = width;
            this.gridCenter = gridCenter;
            grid = new LinkedList<Tile>();
        }

        /// <summary>
        /// Given a moused picked position it will return the tile that it corresponds to
        /// or null if that tile does not exist
        /// </summary>
        /// <param name="mousePickPosition">The position that the player has clicked 
        /// in the game world</param>
        /// <returns>The associated tile or null if the tile does not exist</returns>
        public Tile PickedTile(Vector3 pickPosition) {
            foreach (Tile tile in grid) {
                if (Math.Abs(pickPosition.X - tile.centerPosition.X) <= Game1.TILE_SIZE/2 &&
                    Math.Abs(pickPosition.Z - tile.centerPosition.Y) <= Game1.TILE_SIZE/2) {
                    return tile;
                }
            }
             return null;
        }

        /// <summary>
        /// Private helper function to generate the grid adjacency 
        /// matrix with specified height width and tile size
        /// </summary>
        public void GenerateGrid(Random rand, Model tileModelNotWalkable) {
            bool nextTileWalkable;
            for (int i = -width / 2; i <= width / 2; i++) {
                for (int j = -height/2; j <= height/2; j++) {
                    if (rand.Next() % Tile.CHANCE_OF_NOT_WALKABLE == 0 && !(Math.Abs(i) <= 1 && Math.Abs(j) <= 1)) {
                        nextTileWalkable = false;
                    } else {
                        nextTileWalkable = true;
                    }
                    grid.AddLast(new Tile(new Vector3(gridCenter.X + i * tileSize, gridCenter.Y + j * tileSize, 0), new Vector2(i, j), 
                        nextTileWalkable, tileModelNotWalkable));
                }
            }

            //Now populate the adjacancency list of each tile
            foreach (Tile tileUpTo in grid) {
                if (!tileUpTo.isWalkable) {
                    continue;
                }

                foreach (Tile compareToTile in grid) {
                    if (!tileUpTo.Equals(compareToTile)) {
                        if (!compareToTile.isWalkable) {
                            continue;
                        }
                        //Diagonals are not adjacent
                        if (Math.Abs(tileUpTo.gridPosition.X - compareToTile.gridPosition.X) <= 1 &&
                            Math.Abs(tileUpTo.gridPosition.Y - compareToTile.gridPosition.Y) <= 1 &&
                            !(Math.Abs(tileUpTo.gridPosition.X - compareToTile.gridPosition.X) == 1 &&
                            Math.Abs(tileUpTo.gridPosition.Y - compareToTile.gridPosition.Y) == 1)) {
                            tileUpTo.AddAdjacentTile(compareToTile);
                        }
                    }
                }
            }
        }

        public void Draw(Vector3 pickedPosition, Camera camera, GameTime gameTime) {
            foreach(Tile tile in grid) {
                if (!tile.isWalkable) {
                    tile.Draw(camera, gameTime);
                }
            }
        }

        public void ResetTileCosts() {
            foreach (Tile tile in grid) {
                tile.ResetCost();
            }
        }

    }
}
