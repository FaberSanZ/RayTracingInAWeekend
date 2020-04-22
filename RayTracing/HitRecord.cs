using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace RayTracingInAWeekend
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HitRecord
    {
        public Vector3 Position;
        public float T;
        public Vector3 Normal;

        public static HitRecord Empty
        {
            get
            {
                return new HitRecord()
                {
                    T = 0,
                    Position = Vector3.Zero,
                    Normal = Vector3.Zero,
                };
            }
        }
    }
}
