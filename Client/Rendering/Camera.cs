using Microsoft.Xna.Framework;
namespace Client.Rendering
{
    public class Camera
    {
        public BoundingFrustum BoundingFrustum { get { return new BoundingFrustum(ViewMatrix * ProjectionMatrix); } }
        /// <summary>
        /// 
        /// </summary>
        public Matrix ProjectionMatrix { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Matrix ViewMatrix
        {
            get
            {
                return Matrix.CreateLookAt(Position, Target, Vector3.Up);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Target { get; private set; }
        private Vector3 _position;
        /// <summary>
        /// Our current position.
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                // Must update.
                UpdateLookAt();
            }
        }

        private Vector3 _rotation;
        /// <summary>
        /// Our current rotation
        /// </summary>
        public Vector3 Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                UpdateLookAt();
            }
        }
        public Vector3 Direction;
        /// <summary>
        /// Updates the camera's looking vector.
        /// </summary>
        private void UpdateLookAt()
        {
            //Calculate a rotation matrix from our camera's rotation, used
            //to orient our look at vector
            Matrix rotationMatrix = Matrix.CreateRotationX(Rotation.X) *
                                  Matrix.CreateRotationY(Rotation.Y);
            //Create the look at offset vector based on the direction our camera is
            //originally looking and our rotation matrix
            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
            //Finally, build the camera's look at vector by adding
            //our current position and the look at vector offset.
            Target = Position + lookAtOffset;

            Direction = Vector3.Transform(Vector3.Forward, rotationMatrix);
        }
    }
}
