using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a ray traced scene, including the objects,
    /// light sources, and associated rendering logic.
    /// </summary>
    public class Scene
    {
        private SceneOptions options;
        private ISet<SceneEntity> entities;
        private ISet<PointLight> lights;

        /// <summary>
        /// Construct a new scene with provided options.
        /// </summary>
        /// <param name="options">Options data</param>
        public Scene(SceneOptions options = new SceneOptions())
        {
            this.options = options;
            this.entities = new HashSet<SceneEntity>();
            this.lights = new HashSet<PointLight>();
        }

        /// <summary>
        /// Add an entity to the scene that should be rendered.
        /// </summary>
        /// <param name="entity">Entity object</param>
        public void AddEntity(SceneEntity entity)
        {
            this.entities.Add(entity);
        }

        /// <summary>
        /// Add a point light to the scene that should be computed.
        /// </summary>
        /// <param name="light">Light structure</param>
        public void AddPointLight(PointLight light)
        {
            this.lights.Add(light);
        }

        /// <summary>
        /// Render the scene to an output image. This is where the bulk
        /// of your ray tracing logic should go... though you may wish to
        /// break it down into multiple functions as it gets more complex!
        /// </summary>
        /// <param name="outputImage">Image to store render output</param>
        public void Render(Image outputImage)
        {
            // Begin writing your code here...

            // custom camera attempt
            double cameraAngle = 60;
            //double cameraAngle = options.CameraAngle;
            Vector3 cameraAxis = options.CameraAxis;
            Vector3 orientationAx = new Vector3(0, 0, 1);
            // needs camera axis but need to convert the vector
            
            
            int depth = 0;
            double antiAliasScale = options.AAMultiplier;

            // timer to check render time
            Stopwatch timer = new Stopwatch();
            timer.Start();

         
            for(int y = 0; y < outputImage.Height; y++){
                for(int x = 0; x < outputImage.Width; x++){

                Color pixelColor = new Color(0, 0, 0);
                
                // anti alienising
                    for(int j = 0; j < antiAliasScale; j++){
                        for(int i = 0; i < antiAliasScale; i++){

                            double size = 1/ antiAliasScale;  
                            Ray ray = CameraRayDirection(x + i * size, y + j * size, outputImage, cameraAngle);
                            SceneEntity entity = closestHit(ray);

                            if(entity == null){
                                continue;
                            }
                            RayHit hit = entity.Intersect(ray); 
                            pixelColor += CastRay(hit, entity, new Color(0, 0 ,0), depth);
                        }
                    }
                
                pixelColor /= (antiAliasScale * antiAliasScale);
                outputImage.SetPixel(x, y, pixelColor);   
                }
            
            }
            
        timer.Stop();
        // write to cmd the time elasped
        Console.WriteLine("Time elasped: {0:hh\\:mm\\:ss}", timer.Elapsed);
        }

        // referenced from COMP30019 Lecture 4

        // add custom camera function here tmr      
        private Ray CameraRayDirection(double x, double y, Image outputImage, double fovAngle){

            double fov = fovAngle * (Math.PI / 180);
            double aspectRatio = outputImage.Width / outputImage.Height;

            // normalise  pixel space
            double pixelX = (x + 0.5) / outputImage.Width;
            double pixelY = (y + 0.5) / outputImage.Height;

            // scale to (-1, 1)
            pixelX = (pixelX * 2 ) - 1;
            pixelY = 1 - (pixelY * 2);
       
            // adjust orientation 
            double Xpos = pixelX * Math.Tan(fov / 2);
            double Ypos = pixelY * Math.Tan(fov / 2) / aspectRatio;;
            double Zpos = 1.0f;

            // normalised
            Vector3 direction = new Vector3(Xpos, Ypos, Zpos).Normalized();

            return new Ray(new Vector3(0, 0, 0), direction);
            
        }
    
    // check for closest hit
    private SceneEntity closestHit(Ray ray){
        
        SceneEntity firstHit = null;
        double closestHit = double.PositiveInfinity;

        // modified from COMP30019 Project 1 Spec Stage 1.5
        foreach (SceneEntity entity in this.entities){
            RayHit hit = entity.Intersect(ray);
            if(hit != null){
                // check if is closest hit
                double distance  = (hit.Position - ray.Origin).LengthSq();
                if(distance < closestHit && distance != 0){
                    closestHit = distance;
                    firstHit = entity;
                }             
            } 
    }
    return firstHit;
    }

    // REFLECT LOGIC
    // referenced from https://www.scratchapixel.com/lessons/3d-basic-rendering/introduction-to-shading/reflection-refraction-fresnel

    private Color ReflectionColor(RayHit hit, SceneEntity entity, Color color, int depth){
        Vector3 reflectDirection = hit.Reflect();
        double offset = 0.0001;
        Color newColor = new Color(0, 0, 0);
        Vector3 origin = hit.Position + offset * reflectDirection;
        Ray ray = new Ray(origin, reflectDirection);
        SceneEntity newEntity = closestHit(ray);
        if (newEntity != null && depth < 10)
        {
            RayHit newHit = newEntity.Intersect(ray);
            depth++;
            return CastRay(newHit, newEntity, newColor, depth);
        }

    
        return NormalizeColor(newColor);
    }

    private Color RefractionColor(RayHit hit, SceneEntity entity, Color color, int depth){
        Vector3 refractDirection = hit.Refract(entity.Material);
        double offset = 0.0001;
        Color newColor = new Color(0, 0, 0);
        Vector3 origin = hit.Position + offset * refractDirection;
        Ray ray = new Ray(origin, refractDirection);
        SceneEntity newEntity = closestHit(ray);
        if(newEntity != null && depth < 10)
        {
            RayHit newHit = newEntity.Intersect(ray);
            depth++;
            newColor = CastRay(newHit, newEntity, newColor, depth);
        }

        return NormalizeColor(newColor);
    }
    
    // made glossy from meshing diffuse materials but scaling its diffussion up to 
    // mimick a glossy surface 
    private Color GlossyMaterial(RayHit hit, SceneEntity entity, Color color, int depth){

        Color newColor = new Color(0, 0, 0);    
        foreach (PointLight light in this.lights){
            Vector3 L = (light.Position - hit.Position).Normalized();
            Vector3 N = hit.Normal;
            Color Cm = entity.Material.Color;
            Color Cl = light.Color;
            if(NoShadow(hit.Position, light.Position)){
                newColor += Cm * Cl * (N.Dot(L));
            }
        
            }
        newColor += ((ReflectionColor(hit, entity, newColor, depth)) * 0.5);
        newColor = NormalizeColor(newColor);
        return newColor;   
        
    }

    private Color CastRay(RayHit hit, SceneEntity entity, Color color, int depth){
        Material material = entity.Material;
        Color newColor = new Color(0, 0, 0);
        int maxDepth = 10;

        if(depth > maxDepth){
            NormalizeColor(newColor); 
        }

        // DIFFUSE MATERIAL         
        if(material.Type == Material.MaterialType.Diffuse){
            foreach (PointLight light in this.lights){
                Vector3 L = (light.Position - hit.Position).Normalized();
                Vector3 N = hit.Normal;
                Color Cm = entity.Material.Color;
                Color Cl = light.Color;
                if(NoShadow(hit.Position, light.Position)){
                    newColor += Cm * Cl * (N.Dot(L));
                }
                
            }
        
        return NormalizeColor(newColor);
        }
        // REFLECT
    
        if(material.Type == Material.MaterialType.Reflective){
            return ReflectionColor(hit, entity, newColor, depth);
        }

        // REFRACT and reflect?
        // referenced from https://www.scratchapixel.com/code.php?id=13&origin=/lessons/3d-basic-rendering/introduction-to-shading line 497-530
        
        if(material.Type == Material.MaterialType.Refractive){

 /*
            Color refractColor = new Color(0, 0, 0);
            refractColor = RefractionColor(hit, entity, newColor, depth);
            newColor = refractColor;
            return NormalizeColor(newColor); 
*/
            // FRESHNEL

            double kr = Fresnel(hit, entity.Material);
            Color refractColor = new Color(0, 0, 0);

            if(kr < 1){

                refractColor = RefractionColor(hit, entity, newColor, depth);
            }

            Color reflectColor = new Color(0, 0, 0);
            reflectColor = ReflectionColor(hit, entity, newColor, depth);

            newColor = reflectColor * kr + refractColor * (1 - kr);
            return NormalizeColor(newColor); 
            

        }

        if(material.Type == Material.MaterialType.Glossy){
            return GlossyMaterial(hit, entity, newColor, depth);
        }

        return NormalizeColor(newColor);
    }

    // referenced from https://www.scratchapixel.com/code.php?id=13&origin=/lessons/3d-basic-rendering/introduction-to-shading line429-455
    private double Fresnel(RayHit hit, Material material){

        double kr;
        double cosi = Math.Clamp(hit.Incident.Dot(hit.Normal), -1, 1);
        double etai = 1, etat = material.RefractiveIndex;

        // swap etai and etat
        if(cosi > 0){
            double temp;
            temp = etai;
            etai = etat;
            etat = temp;
        }

        // find sini using Snell's law
        double sint =  etai / etat * Math.Sqrt(Math.Max(0.0, 1 - cosi * cosi));

        // total interal reflection
        if(sint >= 1){
            kr = 1;
        }else{
            double cost = Math.Sqrt(Math.Max(0.0, 1 - sint * sint));
            cosi = Math.Abs(cosi);
            double Rs = ((etat * cosi) - (etai * cost)) / ((etat * cosi) + (etai * cost));
            double Rp = ((etai * cosi) - (etat * cost)) / ((etai * cosi) + (etat * cost)); 
            kr = (Rs * Rs + Rp * Rp) / 2;
        }
        return kr;

    }

    private Color NormalizeColor(Color color)
    {
        if (color.R > 1) color = new Color(1, color.G, color.B);
        if (color.G > 1) color = new Color(color.R, 1, color.B);
        if (color.B > 1) color = new Color(color.R, color.G, 1);
        if (color.R < 0) color = new Color(0, color.G, color.B);
        if (color.G < 0) color = new Color(color.R, 0, color.B);
        if (color.B < 0) color = new Color(color.R, color.G, 0);

        return color;
    }

    private Boolean NoShadow(Vector3 hitPosition, Vector3 lightPosition){
        Vector3 positionToLight = (lightPosition - hitPosition).Normalized();
        double offset = 0.0001;
        //Ray ray = new Ray(hitPosition, positionToLight);

        Ray ray = new Ray(hitPosition + positionToLight * offset, positionToLight);

        foreach(SceneEntity entity in this.entities){
            RayHit hit = entity.Intersect(ray);

            if(hit != null && (hitPosition - hit.Position).LengthSq() < (lightPosition - hitPosition).LengthSq()){
                return false;
            }
        }
        return true;
    }



    }
}
