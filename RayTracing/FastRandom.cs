// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	FastRandom.cs
=============================================================================*/


using System;

namespace RayTracing
{

    public class FastRandom
    {

        private const double RealUnitInt = 1.0 / (int.MaxValue + 1.0);

        private const double RealUnitUInt = 1.0 / (uint.MaxValue + 1.0);


        private const uint Y = 842502087;


        private const uint Z = 3579807591;


        private const uint W = 273326509;


        private uint x;


        private uint y;


        private uint z;


        private uint w;


        private uint bitBuffer;


        private uint bitMask = 1;


        private int seed;


        public int Seed
        {
            get => seed;

            set
            {
                seed = value;
                Reinitialise(seed);
            }
        }


        public FastRandom()
        {
        }


        public FastRandom(int seed)
        {
            this.seed = seed;
            Reinitialise(this.seed);
        }


        internal void Reinitialise(int seed)
        {
            // The only stipulation stated for the xorshift RNG is that at least one of
            // the seeds x,y,z,w is non-zero. We fulfill that requirement by only allowing
            // resetting of the x seed
            x = (uint)seed;
            y = Y;
            z = Z;
            w = W;

            bitBuffer = 0;
            bitMask = 1;
        }

        public int Next()
        {
            uint t = x ^ (x << 11);
            x = y;
            y = z;
            z = w;
            w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

            // Handle the special case where the value int.MaxValue is generated. This is outside of
            // the range of permitted values, so we therefore call Next() to try again.
            uint rtn = w & 0x7FFFFFFF;
            if (rtn == 0x7FFFFFFF)
            {
                return Next();
            }

            return (int)rtn;
        }

        public int Next(int upperBound)
        {
            if (upperBound < 0)
            {
                throw new ArgumentOutOfRangeException("upperBound", "upperBound must be >=0");
            }

            uint t = x ^ (x << 11);
            x = y;
            y = z;
            z = w;

            // The explicit int cast before the first multiplication gives better performance.
            // See comments in NextDouble.
            return (int)((RealUnitInt * (int)(0x7FFFFFFF & (w = (w ^ (w >> 19)) ^ (t ^ (t >> 8))))) * upperBound);
        }

        public int Next(int lowerBound, int upperBound)
        {
            if (lowerBound > upperBound)
            {
                throw new ArgumentOutOfRangeException("upperBound", "upperBound must be >=lowerBound");
            }

            uint t = x ^ (x << 11);
            x = y;
            y = z;
            z = w;

            // The explicit int cast before the first multiplication gives better performance.
            // See comments in NextDouble.
            int range = upperBound - lowerBound;
            if (range < 0)
            {
                // If range is <0 then an overflow has occurred and must resort to using long integer arithmetic instead (slower).
                // We also must use all 32 bits of precision, instead of the normal 31, which again is slower.
                return lowerBound + (int)((RealUnitUInt * (w = (w ^ (w >> 19)) ^ (t ^ (t >> 8)))) * (upperBound - (long)lowerBound));
            }

            // 31 bits of precision will suffice if range<=int.MaxValue. This allows us to cast to an int and gain
            // a little more performance.
            return lowerBound + (int)((RealUnitInt * (int)(0x7FFFFFFF & (w = (w ^ (w >> 19)) ^ (t ^ (t >> 8))))) * range);
        }

        public double NextDouble()
        {
            uint t = x ^ (x << 11);
            x = y;
            y = z;
            z = w;

            // Here we can gain a 2x speed improvement by generating a value that can be cast to
            // an int instead of the more easily available uint. If we then explicitly cast to an
            // int the compiler will then cast the int to a double to perform the multiplication,
            // this final cast is a lot faster than casting from a uint to a double. The extra cast
            // to an int is very fast (the allocated bits remain the same) and so the overall effect
            // of the extra cast is a significant performance improvement.
            //
            // Also note that the loss of one bit of precision is equivalent to what occurs within
            // System.Random.
            return RealUnitInt * (int)(0x7FFFFFFF & (w = (w ^ (w >> 19)) ^ (t ^ (t >> 8))));
        }

        public float NextFloat()
        {
            return (float)NextDouble();
        }
    }
}
