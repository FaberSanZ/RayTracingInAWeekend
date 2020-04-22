// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	Sphere.cs
=============================================================================*/




using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace RayTracing
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Sphere
    {
        public Vector3 Center;
        public float Radius;
        public Material Material;

        public Sphere(Vector3 center, float radius, Material material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public bool Hit(Ray r, float tMin, float tMax, out HitRecord rec)
        {
            Vector3 oc = r.Origin - Center;
            float a = Vector3.Dot(r.Direction, r.Direction);
            float b = Vector3.Dot(oc, r.Direction);
            float c = Vector3.Dot(oc, oc) - Radius * Radius;
            float discriminant = (b * b) - (a * c);

            if (discriminant > 0)
            {
                float temp = (-b - MathF.Sqrt(b * b - a * c)) / a;
                if (temp < tMax && temp > tMin)
                {
                    rec.T = temp;
                    rec.Position = r.PointAtParameter(rec.T);
                    rec.Normal = (rec.Position - Center) / Radius;
                    rec.Material = Material;
                    return true;
                }

                temp = (-b + MathF.Sqrt(b * b - a * c)) / a;
                if (temp < tMax && temp > tMin)
                {
                    rec.T = temp;
                    rec.Position = r.PointAtParameter(rec.T);
                    rec.Normal = (rec.Position - Center) / Radius;
                    rec.Material = Material;
                    return true;
                }
            }

            rec = HitRecord.Empty;
            return false;
        }
    }
}
