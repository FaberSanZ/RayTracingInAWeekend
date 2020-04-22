// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Lambertian.cs
=============================================================================*/


using System.Numerics;

namespace RayTracing
{
    public class Lambertian : Material
    {
        internal Vector3 Albedo;

        public Lambertian(Vector3 albedo)
        {
            Albedo = albedo;
        }

        public override bool Scatter(ref Ray ray, ref HitRecord hit, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 target = hit.Position + hit.Normal + Program.RandomInUnitSphere();
            scattered = new Ray(hit.Position, target - hit.Position);
            attenuation = Albedo;

            return true;
        }
    }
}
