// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Ray.cs
=============================================================================*/



using System.Numerics;
using System.Runtime.InteropServices;

namespace RayTracing
{
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct Ray
    {
        [FieldOffset(0)]
        public Vector3 Origin;

        [FieldOffset(16)]
        public Vector3 Direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Vector3 PointAtParameter(float t)
        {
            return Origin + t * Direction;
        }
    }
}
