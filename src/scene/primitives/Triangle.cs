using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a triangle in a scene represented by three vertices.
    /// </summary>
    public class Triangle : SceneEntity
    {
        private Vector3 v0, v1, v2;
        private Material material;

        /// <summary>
        /// Construct a triangle object given three vertices.
        /// </summary>
        /// <param name="v0">First vertex position</param>
        /// <param name="v1">Second vertex position</param>
        /// <param name="v2">Third vertex position</param>
        /// <param name="material">Material assigned to the triangle</param>
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Material material)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the triangle, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        
        // referenced from https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/ray-triangle-intersection-geometric-solution
        public RayHit Intersect(Ray ray)
        {
            // compute normal to plane
            Vector3 normal = (this.v1 - this.v2).Cross(this.v2 - this.v0).Normalized();

            // check if ray and plane are parallel
            double normalDotRayDirection = normal.Dot(ray.Direction);
            if(Math.Abs(normalDotRayDirection) < double.Epsilon){
                return null;
            }

            // compute t
            double t = (this.v0 - ray.Origin).Dot(normal) / normalDotRayDirection;

            // check if triangle is behind ray
            if (t <= 0){
                return null;
            }

            // find intersection point
            Vector3 position = ray.Origin + t * ray.Direction;

            // inside or outside triangle test
            Vector3 edge0 = this.v1 - this.v0;
            Vector3 vp0 = position - this.v0;
            double cross = normal.Dot(edge0.Cross(vp0));
            
            if(cross < 0){
                return null;
            }

            Vector3 edge1 = this.v2 - this.v1;
            Vector3 vp1 = position - this.v1;
            double cross1 = normal.Dot(edge1.Cross(vp1));

            if(cross1 < 0){
                return null;
            }

            Vector3 edge2 = this.v0 - this.v2;
            Vector3 vp2 = position - this.v2;
            double cross2 = normal.Dot(edge2.Cross(vp2));

            if(cross2 < 0){
                return null;
            }
            // Vector3 incident = position - 2 * position.Dot(normal) * normal;
            return new RayHit(position, normal, ray.Direction, this.Material);

        }

        /// <summary>
        /// The material of the triangle.
        /// </summary>
        public Material Material { get { return this.material; } }
    }

}
