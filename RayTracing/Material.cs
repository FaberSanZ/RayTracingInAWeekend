// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Material.cs
=============================================================================*/


using System.Numerics;

namespace RayTracing
{
    public abstract class Material
    {
        public abstract bool Scatter(ref Ray ray, ref HitRecord hit, out Vector3 attenuation, out Ray scattered);
    }
}
