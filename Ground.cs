using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab05 {
    /// <summary>
    /// The ground plane of the world
    /// Inherits from the basic model class
    /// </summary>
    public class Ground : BasicModel {


        //A plane that specifies the level that the ground plane is on
        public Plane groundPlane { get; private set; }

        /// <summary>
        /// Constructor method for the ground. Takes a model and a center position
        /// for the ground plane
        /// </summary>
        /// <param name="m">The model of the ground plane</param>
        /// <param name="position">The center position of the ground plane</param>
        public Ground(Model m, Vector3 position) : base(m, position) {
            groundPlane = new Plane(Vector3.UnitY, position.Y);
        }

    }
}
