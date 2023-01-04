using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent ray hit data, including the position and
    /// normal of a hit (and optionally, other computed vectors).
    /// </summary>
    public class RayHit
    {
        private Vector3 position;
        private Vector3 normal;
        private Vector3 incident;

        public RayHit(Vector3 position, Vector3 normal, Vector3 incident, Material material)
        {
            this.position = position;
            this.normal = normal;
            this.incident = incident;
        }

        // You may wish to write methods to compute other vectors, 
        // e.g. reflection, transmission, etc
        public Vector3 Reflect()
        {
            return (this.incident - 2 * this.incident.Dot(this.normal) * this.normal).Normalized();
        }

        // from https://www.scratchapixel.com/code.php?id=13&origin=/lessons/3d-basic-rendering/introduction-to-shading line 418-428
        
        public Vector3 Refract(Material material){
            double cosi = Math.Clamp(this.incident.Dot(this.normal), -1, 1);
            double etai = 1;
            double etat = material.RefractiveIndex;
            double temp = 0;
            Vector3 n = this.normal;

            if(cosi < 0){
                cosi = -cosi;
            }else{
                // swap etai and etat
                temp = etai;
                etai = etat;
                etat = temp;
                n = -this.normal;
            }

            double eta = etai/ etat;
            double k = 1 - eta * eta * (1 - cosi * cosi);

            if(k < 0){
                // this is the internal reflection
                return (this.incident - 2 * this.incident.Dot(n) * n).Normalized();
                
            } 
            return (eta * this.incident + (eta * cosi - Math.Sqrt(k)) * n).Normalized();

        }
        public Vector3 Position { get { return this.position; } }

        public Vector3 Normal { get { return this.normal; } }

        public Vector3 Incident { get { return this.incident; } }
    }
}
