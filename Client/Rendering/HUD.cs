using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace Client
{
    public sealed class HUD : IPlugin
    {
        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public void Init()
        {
            Client.OnUpdate += Update;
            Client.OnDraw2D += Draw2D;
        }
        public void Stop()
        {
            Client.OnUpdate -= Update;
            Client.OnDraw2D -= Draw2D;
        }
        public void Update(object sender, UpdateEventArgs e)
        {
            elapsedTime += e.GameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }
        private static int FastFloor(double x)
        {
            return x > 0 ? (int)x : (int)x - 1;
        }
        /// <summary>
        /// Gets the cells which intersect with the specified <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">The cell-relative <see cref="Ray"/>.</param>
        /// <param name="maxDepth">The maximum search depth, which equals the maximum number of
        /// returned points.</param>
        /// <returns>An enumerable list of points, starting with the cell closest to the starting
        /// point of the ray.</returns>
        /// <remarks>
        /// <para>The position and direction of the <paramref name="ray"/> must be in the cell
        /// coordinate system.</para>
        /// <para>The first cell which is returned refers to the cell in which the ray starts.</para>
        /// </remarks>
        public IEnumerable<Vector3I> GetCellsOnRay(Ray ray, int maxDepth)
        {
            // Implementation is based on:
            // "A Fast Voxel Traversal Algorithm for Ray Tracing"
            // John Amanatides, Andrew Woo
            // http://www.cse.yorku.ca/~amana/research/grid.pdf
            // http://www.devmaster.net/articles/raytracing_series/A%20faster%20voxel%20traversal%20algorithm%20for%20ray%20tracing.pdf

            // NOTES:
            // * This code assumes that the ray's position and direction are in 'cell coordinates', which means
            //   that one unit equals one cell in all directions.
            // * When the ray doesn't start within the voxel grid, calculate the first position at which the
            //   ray could enter the grid. If it never enters the grid, there is nothing more to do here.
            // * Also, it is important to test when the ray exits the voxel grid when the grid isn't infinite.
            // * The Point3D structure is a simple structure having three integer fields (X, Y and Z).

            if (float.IsNaN(ray.Position.X) || float.IsNaN(ray.Position.Y) || float.IsNaN(ray.Position.Z)) yield break;

            int x = FastFloor(ray.Position.X);
            int y = FastFloor(ray.Position.Y + Player.EyeLevel);
            int z = FastFloor(ray.Position.Z);

            // Determine which way we go.
            int stepX = Math.Sign(ray.Direction.X);
            int stepY = Math.Sign(ray.Direction.Y);
            int stepZ = Math.Sign(ray.Direction.Z);

            // Calculate cell boundaries. When the step (i.e. direction sign) is positive,
            // the next boundary is AFTER our current position, meaning that we have to add 1.
            // Otherwise, it is BEFORE our current position, in which case we add nothing.
            //Point3D cellBoundary = new Point3D(
            Vector3I cellBoundary = new Vector3I(
                x + (stepX > 0 ? 1 : 0),
                y + (stepY > 0 ? 1 : 0),
                z + (stepZ > 0 ? 1 : 0));

            // NOTE: For the following calculations, the result will be Single.PositiveInfinity
            // when ray.Direction.X, Y or Z equals zero, which is OK. However, when the left-hand
            // value of the division also equals zero, the result is Single.NaN, which is not OK.

            // Determine how far we can travel along the ray before we hit a voxel boundary.
            Vector3 tMax = new Vector3(
                (cellBoundary.X - ray.Position.X) / ray.Direction.X,    // Boundary is a plane on the YZ axis.
                (cellBoundary.Y - ray.Position.Y) / ray.Direction.Y,    // Boundary is a plane on the XZ axis.
                (cellBoundary.Z - ray.Position.Z) / ray.Direction.Z);    // Boundary is a plane on the XY axis.
            if (Single.IsNaN(tMax.X)) tMax.X = Single.PositiveInfinity;
            if (Single.IsNaN(tMax.Y)) tMax.Y = Single.PositiveInfinity;
            if (Single.IsNaN(tMax.Z)) tMax.Z = Single.PositiveInfinity;

            // Determine how far we must travel along the ray before we have crossed a gridcell.
            Vector3 tDelta = new Vector3(
                stepX / ray.Direction.X,                    // Crossing the width of a cell.
                stepY / ray.Direction.Y,                    // Crossing the height of a cell.
                stepZ / ray.Direction.Z);                    // Crossing the depth of a cell.
            if (Single.IsNaN(tDelta.X)) tDelta.X = Single.PositiveInfinity;
            if (Single.IsNaN(tDelta.Y)) tDelta.Y = Single.PositiveInfinity;
            if (Single.IsNaN(tDelta.Z)) tDelta.Z = Single.PositiveInfinity;

            // For each step, determine which distance to the next voxel boundary is lowest (i.e.
            // which voxel boundary is nearest) and walk that way.
            for (int i = 0; i < maxDepth; i++)
            {
                // Return it.
                yield return new Vector3(x, y, z).ToBlockCoords();

                // Do the next step.
                if (tMax.X < tMax.Y && tMax.X < tMax.Z)
                {
                    // tMax.X is the lowest, an YZ cell boundary plane is nearest.
                    x += stepX;
                    tMax.X += tDelta.X;
                }
                else if (tMax.Y < tMax.Z)
                {
                    // tMax.Y is the lowest, an XZ cell boundary plane is nearest.
                    y += stepY;
                    tMax.Y += tDelta.Y;
                }
                else
                {
                    // tMax.Z is the lowest, an XY cell boundary plane is nearest.
                    z += stepZ;
                    tMax.Z += tDelta.Z;
                }
            }
        }
        public void Draw2D(object sender, Draw2DEventArgs e)
        {
            frameCounter++;

            e.SpriteBatch.Begin();

            if (!Client.Paused)
            {
                // Crosshair
                e.SpriteBatch.Draw(Client.CrosshairTexture,
                    Client.WindowCenter, // Center of screen
                    null, // Source rectangle
                    Color.White, // Color
                    0f, // Rotation
                    new Vector2(Client.CrosshairTexture.Width, Client.CrosshairTexture.Height) / 2, // Image center
                    1f, // Scale
                    SpriteEffects.None,
                    0f // Depth
                    );
                e.SpriteBatch.Draw(Client.EmptyTexture, Client.WindowCenter, Color.Red);
                string fps = string.Format("FPS: {0}", frameRate);
                e.SpriteBatch.DrawString(Client.Font, fps, Vector2.Zero, Color.White);

                MouseState m = Mouse.GetState();
                int mouseX = m.X;
                int mouseY = m.Y;
                Vector3 nearsource = new Vector3((float)mouseX, (float)mouseY, 0f);
                Vector3 farsource = new Vector3((float)mouseX, (float)mouseY, 1f);

                Vector3 nearPoint = Client.Viewport.Unproject(nearsource,
                    Client.MainPlayer.Camera.ProjectionMatrix,
                    Client.MainPlayer.Camera.ViewMatrix, 
                    Matrix.Identity);

                Vector3 farPoint = Client.Viewport.Unproject(farsource, 
                    Client.MainPlayer.Camera.ProjectionMatrix,
                    Client.MainPlayer.Camera.ViewMatrix, 
                    Matrix.Identity);

                Vector3 direction = farPoint - nearPoint;
                direction.Normalize();

                //Matrix rotationMatrix = Matrix.CreateRotationX(Client.MainPlayer.Camera.Rotation.X) *
                //                        Matrix.CreateRotationY(Client.MainPlayer.Camera.Rotation.Y);
                //distance = Vector3.Transform(distance, rotationMatrix);

                Ray r = new Ray(nearPoint, direction);
                foreach (Vector3I coord in GetCellsOnRay(r, 5))
                {
                    BlockID id = Client.MainWorld[coord];
                    Vector3 renderPos = coord.ToRenderCoords();
                    Vector3 min = new Vector3(renderPos.X - 1f, renderPos.Y - 1f, renderPos.Z - 1);
                    Vector3 max = new Vector3(renderPos.X + 1f, renderPos.Y + 1f, renderPos.Z + 1f);
                    if (id != BlockID.None && id != BlockID.Air)
                    {
                        e.SpriteBatch.DrawString(Client.Font, "Looking: " + id, new Vector2(0, 80), Color.White);
                        BoundingBoxRenderer.Render(new BoundingBox(min, max),
                            Client.Device,
                            Client.MainPlayer.Camera.ViewMatrix,
                            Client.MainPlayer.Camera.ProjectionMatrix,
                            Color.Red);
                        break;
                    }
                    else
                    {
                        BoundingBoxRenderer.Render(new BoundingBox(min, max),
                            Client.Device,
                            Client.MainPlayer.Camera.ViewMatrix,
                            Client.MainPlayer.Camera.ProjectionMatrix,
                            Color.White);
                    }
                }
            }
            else
            {
                AnchoredText t = new AnchoredText(Client.Font, "PAUSED: " + Client.Version, Client.WindowCenter, TextAnchor.MiddleCenter);
                t.Draw(e.SpriteBatch);
            }
            e.SpriteBatch.End();
        }
    }
}
