// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Metal.cs
=============================================================================*/



using System.Numerics;

namespace RayTracing
{
    public class Metal : Material
    {
        private Vector3 Albedo;
        private readonly float fuzz;

        public Metal(Vector3 albedo, float f)
        {
            Albedo = albedo;
            fuzz = f < 1 ? f : 1;
        }

        public override bool Scatter(ref Ray ray, ref HitRecord hit, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 direction = Vector3.Normalize(ray.Direction);
            Vector3 reflected = Vector3.Reflect(direction, hit.Normal);
            scattered = new Ray(hit.Position, reflected + fuzz * Program.RandomInUnitSphere());
            attenuation = Albedo;

            return (Vector3.Dot(scattered.Direction, hit.Normal) > 0);
        }
    }
}
