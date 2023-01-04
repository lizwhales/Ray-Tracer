using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent an (infinite) plane in a scene.
    /// </summary>
    public class Sphere : SceneEntity
    {
        private Vector3 center;
        private double radius;
        private Material material;

        /// <summary>
        /// Construct a sphere given its center point and a radius.
        /// </summary>
        /// <param name="center">Center of the sphere</param>
        /// <param name="radius">Radius of the spher</param>
        /// <param name="material">Material assigned to the sphere</param>
        public Sphere(Vector3 center, double radius, Material material)
        {
            this.center = center;
            this.radius = radius;
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the sphere, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>

        // referenced from https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection
        public RayHit Intersect(Ray ray)
        {
            // solutions for t if ray intersects
            double t0, t1; 
            Vector3 L = center - ray.Origin;
            double tca = L.Dot(ray.Direction);

            if(tca < 0){
                return null;
            }

            double d2 = L.Dot(L) - tca * tca;
            if(d2 > radius * radius){
                return null;
            }

            double thc = Math.Sqrt(radius * radius - d2);
            t0 = tca - thc;
            t1 = tca + thc;

            if(t0 > t1){
                double tempt1 = t1;
                t1 = t0;
                t0 = tempt1;
            }
            if(t0 <= 0){
                t0 = t1;
                if(t0 <= 0){
                    return null;
                }
            }

            double t = t0;
            Vector3 position = ray.Origin + t * ray.Direction;
            Vector3 normal = (position - this.center).Normalized();

            // Vector3 incident = position - 2 * position.Dot(normal) * normal;
            return new RayHit(position, normal, ray.Direction, this.material);
        }

        /// <summary>
        /// The material of the sphere.
        /// </summary>
        public Material Material { get { return this.material; } }
    }

}
