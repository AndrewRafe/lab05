using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab05 {
    /// <summary>
    /// A representation of a single tile in a grid
    /// </summary>
    public class Tile : BasicModel{

        public static int CHANCE_OF_NOT_WALKABLE = 10;
        public static float INFINITY_COST = 1000000.0f;

        public bool isWalkable { get; private set; }
        public Vector3 centerPosition { get; private set; }
        public Vector2 gridPosition;

        //A list of all adjacent tiles
        public List<Tile> adjacentTiles { get; private set; }

        //Attributes for use in search algorithm
        public float gScore;
        public float fScore;
        public Tile cameFrom;

        /// <summary>
        /// Tile Constructor method that creates a new tile and sets it as a basic model
        /// </summary>
        /// <param name="position">The position of the tile</param>
        /// <param name="gridPosition">A 2D representation of its position in the grid</param>
        /// <param name="isWalkable">A boolean to determine whether you can walk on the tile or not</param>
        /// <param name="m">The model for the tile</param>
        public Tile(Vector3 position, Vector2 gridPosition, bool isWalkable, Model m) : base(m, position) {
            this.centerPosition = position;
            this.gridPosition = gridPosition;
            this.isWalkable = isWalkable;
            this.cameFrom = null;
            adjacentTiles = new List<Tile>();
        }

        /// <summary>
        /// A string representation of the tile for debug purposes
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "Tile: " + gridPosition.X + ", " + gridPosition.Y + " Global Coords: " + centerPosition.ToString();
        }

        /// <summary>
        /// Will reset the attribtues for the search algorithm
        /// </summary>
        public void ResetCost() {
            gScore = INFINITY_COST;
            fScore = INFINITY_COST;
            cameFrom = null;
        }

        /// <summary>
        /// Will add a given tile to this tiles adjacency list
        /// </summary>
        /// <param name="tile">The tile that is adjacent to this tile</param>
        public void AddAdjacentTile(Tile tile) {
            if (adjacentTiles.Contains(tile)) {
                //Check to see if the tile is already in the adjacency list
                return;
            } else {
                //Otherwise add it to the adjacency list
                adjacentTiles.Add(tile);
            }
        }

        /// <summary>
        /// Will check if the given tile is adjacent to this tile
        /// </summary>
        /// <param name="tile">Tile to be checked</param>
        /// <returns>Whether the tile is adjacent or not</returns>
        public Boolean IsAdjacent(Tile tile) {
            if (adjacentTiles.Contains(tile)) {
                return true;
            } else {
                return false;
            }
        }

    }
}
