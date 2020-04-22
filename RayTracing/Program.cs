using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace RayTracing
{
    class Program
    {
        public struct PixelRGB
        {
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
        }

        public static FastRandom rand = new FastRandom(Environment.TickCount);

        public static Vector3 InUnitSphere()
        {
            // 3D random vector in a unit sphere
            float r = MathF.Sqrt(rand.NextFloat());
            float u = rand.NextFloat();
            float v = rand.NextFloat();

            var phi = MathF.Acos((2f * v) - 1f);
            var theta = 2 * MathF.PI * u;

            var x = r * MathF.Cos(theta) * MathF.Sin(phi);
            var y = r * MathF.Sin(theta) * MathF.Sin(phi);
            var z = r * MathF.Cos(phi);

            Vector3 res = new Vector3((float)x, (float)y, (float)z);
            return res;

        }

        public static Vector3 RandomInUnitSphere()
        {
            Vector3 p;
            do
            {
                p = 2f * new Vector3(rand.NextFloat(), rand.NextFloat(), rand.NextFloat()) - Vector3.One;
            } while (p.LengthSquared() >= 1.0f);

            return p;
        }

        public static Vector3 Color(ref Ray r, ref Scene world, uint depth)
        {
            if (world.Hit(r, 0.001f, float.MaxValue, out HitRecord rec))
            {
                if (depth < 50 && rec.Material.Scatter(ref r, ref rec, out Vector3 attenuation, out Ray scattered))
                {
                    return attenuation * Color(ref scattered, ref world, depth + 1);
                }
                else
                {
                    return Vector3.Zero;
                }
            }
            else
            {
                Vector3 unitDirection = Vector3.Normalize(r.Direction);
                float t = (0.5f * unitDirection.Y) + 1.0f;
                return ((1.0f - t) * new Vector3(1f, 1f, 1f)) + (t * new Vector3(0.5f, 0.7f, 1f));
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("       --Ray Tracing--");

            //Vector3 lowerLeftCorner = new Vector3(-2.0f, -1.0f, -1.0f);
            //Vector3 horizontal = new Vector3(4f, 0f, 0f);
            //Vector3 vertical = new Vector3(0f, 2f, 0f);
            //Vector3 origin = Vector3.Zero;


            List<Sphere> spheres = new List<Sphere>();
            spheres.Add(new Sphere(new Vector3(0, -1000, 0), 1000, new Lambertian(new Vector3(0.5f, 0.5f, 0.5f))));

            int width = 1200;
            int height = 600; 
            int nSamples = 12;
            string filename = "Render.png";
            PixelRGB[] pixels = new PixelRGB[width * height];

            using Image<Rgba32> image = new Image<Rgba32>(width, height);
            int size = 5;
            for (int a = -size; a < size; a++)
            {
                for (int b = -size; b < size; b++)
                {
                    float choose_mat = rand.NextFloat();
                    Vector3 center = new Vector3(a + 0.9f * rand.NextFloat(), 0.2f, b + 0.9f * rand.NextFloat());
                    if ((center - new Vector3(4f, 0.2f, 0)).Length() > 0.9f)
                    {
                        if (choose_mat < 0.8f) // Diffuse
                        {
                            spheres.Add(new Sphere(center, 0.2f, new Lambertian(new Vector3(rand.NextFloat() * rand.NextFloat(), rand.NextFloat() * rand.NextFloat(), rand.NextFloat() * rand.NextFloat()))));
                        }
                        else if (choose_mat < 0.95f) // Metal
                        {
                            spheres.Add(new Sphere(center, 0.2f, new Metal(new Vector3(0.5f * (1f + rand.NextFloat()), 0.5f * (1f + rand.NextFloat()), 0.5f * (1 + rand.NextFloat())), 0.5f * rand.NextFloat())));
                        }
                        else // glass
                        {
                            spheres.Add(new Sphere(center, 0.2f, new Dielectric(1.5f, 1f)));
                        }
                    }
                }
            }

            spheres.Add(new Sphere(new Vector3(0, 1, 1), 1f, new Dielectric(1.5f, 1f)));
            spheres.Add(new Sphere(new Vector3(-4, 1, 0), 1f, new Lambertian(new Vector3(0.4f, 0.2f, 0.1f))));
            spheres.Add(new Sphere(new Vector3(4f, 1f, 0f), 1f, new Metal(new Vector3(0.7f, 0.6f, 0.5f), 0f)));

            Scene world = new Scene(spheres.ToArray());

            Vector3 lookFrom = new Vector3(9.5f, 2f, 2.5f);
            Vector3 lookat = new Vector3(3, 0.5f, 0.65f);
            float dist_to_focus = (lookFrom - lookat).Length();
            float aperture = 0.01f;
            Camera cam = Camera.Create(lookFrom, lookat, Vector3.UnitY, 25f, (float)width / height, aperture, dist_to_focus);

            for (int j = 0; j < image.Height; j++)
            {
                for (int i = 0; i < image.Width; i++)
                {
                    Vector3 col = Vector3.Zero;
                    for (int s = 0; s < nSamples; s++)
                    {
                        float u = (float)(i + rand.NextFloat()) / (float)width;
                        float v = (float)(height - (j + rand.NextFloat())) / (float)height;

                        Ray r = cam.GetRay(u, v);
                        col += Color(ref r, ref world, 0);
                    }
                    col /= (float)nSamples;

                    // Gamma correction
                    col.X = MathF.Sqrt(col.X);
                    col.Y = MathF.Sqrt(col.Y);
                    col.Z = MathF.Sqrt(col.Z);

                    int index = i + (j * width);
                    pixels[index].R = (byte)(255.99 * col.X);
                    pixels[index].G = (byte)(255.99 * col.Y);
                    pixels[index].B = (byte)(255.99 * col.Z);
                }
            }


            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    int index = (x + (y * width));
                    image[x, y] = new Rgba32(pixels[index].R, pixels[index].G, pixels[index].B);
                }
            }


            image.Save(filename); // .png

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), filename);

            ProcessStartInfo start_info = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                CreateNoWindow = true,
                Verb = string.Empty
            };
            Process.Start(start_info);
        }
    }
}
