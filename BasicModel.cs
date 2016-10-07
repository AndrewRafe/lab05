using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace lab05 {
    /// <summary>
    /// The basic class for a model
    /// Contains methods to draw and load in a model
    /// </summary>
    public class BasicModel {

        public Model model { get; private set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; private set; }
        public Matrix rotationMatrix { get; private set; }
        protected Matrix world = Matrix.Identity;

        /// <summary>
        /// Constructor method for the basic model class that takes a model
        /// </summary>
        /// <param name="m">The Model</param>
        /// <param name="position">The position of the model</param>
        public BasicModel(Model m, Vector3 position) {
            model = m;
            this.position = position;
            this.rotation = new Quaternion();
        }

        /// <summary>
        /// Update method to be overriden by classes that derive BasicModel
        /// </summary>
        /// <param name="gameTime">A reference to the Game Time</param>
        public virtual void Update(GameTime gameTime) {

        }

        /// <summary>
        /// Basic draw method for a generic model object
        /// </summary>
        /// <param name="camera">A reference to the camera</param>
        public virtual void Draw(Camera camera, GameTime gameTime) {

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes) {
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    effect.Projection = camera.projection;
                    effect.View = camera.view;
                    effect.World = GetWorldMatrix() * mesh.ParentBone.Transform;
                }
                mesh.Draw();
            }
        }

        /// <summary>
        /// Method to retrieve the world matrix for this object
        /// Method can be overriden by classes that derive the Basic Model Class
        /// </summary>
        /// <returns>The World Matrix</returns>
        public virtual Matrix GetWorldMatrix() {
            world = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
            return world;
        }

        /// <summary>
        /// Determines whether or not this object is colliding with the given model and that models world matrix
        /// </summary>
        /// <param name="otherModel"></param>
        /// <param name="otherWorld"></param>
        /// <returns>A boolean specifying whether this model has collided with another given model</returns>
        public bool CollidesWith(Model otherModel, Matrix otherWorld) {
            //Loop through each model meash in both objects and compare all the bounding spheres
            //for collisions
            foreach (ModelMesh thisModelMeshes in model.Meshes) {
                foreach (ModelMesh otherModelMeshes in otherModel.Meshes) {
                    if (thisModelMeshes.BoundingSphere.Transform(GetWorldMatrix()).Intersects(otherModelMeshes.BoundingSphere.Transform(otherWorld))) {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Will set the rotation of this object to face another given position
        /// </summary>
        /// <param name="targetPosition">The position that the object would like to face</param>
        /// <param name="up">The up vector of the object</param>
        public void RotateToFace(Vector3 targetPosition, Vector3 up) {

            rotation = Behavior.TurnToFace(rotation, Behavior.RotateToFace(position, targetPosition, up), 0.1f);
            rotationMatrix = Matrix.CreateFromQuaternion(rotation);
            //rotation = Behavior.RotateToFace(position, targetPosition, up);

        }

        /// <summary>
        /// Getter method to return the position of the current model
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition() {
            return this.position;
        }

    }
}
