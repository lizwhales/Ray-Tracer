using System;

namespace RayTracer
{
    /// <summary>
    /// Immutable structure to represent a three-dimensional vector.
    /// </summary>
    public readonly struct Vector3
    {
        private readonly double x, y, z;

        /// <summary>
        /// Construct a three-dimensional vector.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Convert vector to a readable string.
        /// </summary>
        /// <returns>Vector as string in form (x, y, z)</returns>
        public override string ToString()
        {
            return "(" + this.x + "," + this.y + "," + this.z + ")";
        }

        /// <summary>
        /// Compute the length of the vector squared.
        /// This should be used if there is a way to perform a vector
        /// computation without needing the actual length, since
        /// a square root operation is expensive.
        /// </summary>
        /// <returns>Length of the vector squared</returns>
        public double LengthSq()
        {
            // Write your code here...
            return Math.Pow(this.x, 2) + Math.Pow(this.y, 2) + Math.Pow(this.z, 2);
        }

        /// <summary>
        /// Compute the length of the vector.
        /// </summary>
        /// <returns>Length of the vector</returns>
        public double Length()
        {
            // Write your code here...
            return Math.Sqrt(this.LengthSq());
        }

        /// <summary>
        /// Compute a length 1 vector in the same direction.
        /// </summary>
        /// <returns>Normalized vector</returns>
        public Vector3 Normalized()
        {
            // Write your code here...
            return new Vector3(this.x / this.Length(), this.y / this.Length(), this.z / this.Length());
        }

        /// <summary>
        /// Compute the dot product with another vector.
        /// </summary>
        /// <param name="with">Vector to dot product with</param>
        /// <returns>Dot product result</returns>
        public double Dot(Vector3 with)
        {
            // Write your code here...
            return this.x * with.x + this.y * with.y + this.z * with.z;
        }

        /// <summary>
        /// Compute the cross product with another vector.
        /// </summary>
        /// <param name="with">Vector to cross product with</param>
        /// <returns>Cross product result</returns>
        public Vector3 Cross(Vector3 with)
        {
            // Write your code here...
            double crossX = this.y * with.Z - this.z * with.Y;
            double crossY = this.z * with.X - this.x * with.z;
            double crossZ = this.x * with.Y - this.y * with.X;
            return new Vector3(crossX, crossY, crossZ);
        }

        /// <summary>
        /// Sum two vectors together (using + operator).
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Summed vector</returns>
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            // Write your code here...
            double sumX = a.X + b.X; 
            double sumY = a.Y + b.Y;
            double sumZ = a.Z + b.Z;
            return new Vector3(sumX, sumY, sumZ);
        }

        /// <summary>
        /// Negate a vector (using - operator)
        /// </summary>
        /// <param name="a">Vector to negate</param>
        /// <returns>Negated vector</returns>
        public static Vector3 operator -(Vector3 a)
        {
            // Write your code here...
            return new Vector3(-a.X, -a.Y, -a.Z);
        }

        /// <summary>
        /// Subtract one vector from another.
        /// </summary>
        /// <param name="a">Original vector</param>
        /// <param name="b">Vector to subtract</param>
        /// <returns>Subtracted vector</returns>
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            // Write your code here...
            double subX = a.X - b.X; 
            double subY = a.Y - b.Y;
            double subZ = a.Z - b.Z;
            return new Vector3(subX, subY, subZ);
        }

        /// <summary>
        /// Multiply a vector by a scalar value.
        /// </summary>
        /// <param name="a">Original vector</param>
        /// <param name="b">Scalar multiplier</param>
        /// <returns>Multiplied vector</returns>
        public static Vector3 operator *(Vector3 a, double b)
        {
            // Write your code here...
            double timesX = a.X * b;
            double timesY = a.Y * b;
            double timesZ = a.Z * b;
            return new Vector3(timesX, timesY, timesZ);
        }

        /// <summary>
        /// Multiply a vector by a scalar value (opposite operands).
        /// </summary>
        /// <param name="b">Scalar multiplier</param>
        /// <param name="a">Original vector</param>
        /// <returns>Multiplied vector</returns>
        public static Vector3 operator *(double b, Vector3 a)
        {
            // Write your code here...
            double timesX = a.X * b;
            double timesY = a.Y * b;
            double timesZ = a.Z * b;
            return new Vector3(timesX, timesY, timesZ);
        }

        /// <summary>
        /// Divide a vector by a scalar value.
        /// </summary>
        /// <param name="a">Original vector</param>
        /// <param name="b">Scalar divisor</param>
        /// <returns>Divided vector</returns>
        public static Vector3 operator /(Vector3 a, double b)
        {
            // Write your code here...
            double divX = a.X / b;
            double divY = a.Y / b;
            double divZ = a.Z / b;
            return new Vector3(divX, divY, divZ);
        }

        /// <summary>
        /// X component of the vector.
        /// </summary>
        public double X { get { return this.x; } }

        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public double Y { get { return this.y; } }

        /// <summary>
        /// Z component of the vector.
        /// </summary>
        public double Z { get { return this.z; } }
    }
}

