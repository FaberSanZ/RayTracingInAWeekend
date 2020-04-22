// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Scene.cs
=============================================================================*/

namespace RayTracing
{
    public struct Scene
    {
        public Sphere[] Spheres;

        public Scene(Sphere[] spheres)
        {
            Spheres = spheres;
        }

        public bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
        {
            rec = HitRecord.Empty;
            bool hitAnything = false;
            float closestSoFar = tMax;
            for (int i = 0; i < Spheres.Length; i++)
            {
                Sphere elem = Spheres[i];
                if (elem.Hit(r, tMin, closestSoFar, out HitRecord tempRec))
                {
                    hitAnything = true;
                    closestSoFar = tempRec.T;
                    rec = tempRec;
                }
            }

            return hitAnything;
        }
    }
}
