using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using RayTracingInAWeekend;
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

        public static Vector3 Color(Ray r, Scene world)
        {
            if (world.Hit(r, 0, float.MaxValue, out HitRecord rec))
            {
                return 0.5f * new Vector3(rec.Normal.X + 1, rec.Normal.Y + 1, rec.Normal.Z + 1);
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

            Vector3 lowerLeftCorner = new Vector3(-2.0f, -1.0f, -1.0f);
            Vector3 horizontal = new Vector3(4f, 0f, 0f);
            Vector3 vertical = new Vector3(0f, 2f, 0f);
            Vector3 origin = Vector3.Zero;

            Sphere[] spheres = new[]
            {
                new Sphere(new Vector3(0, 0, -1f), 0.5f),
                new Sphere(new Vector3(0, -100.5f, -1f), 100f),
            };


            int width = 1200;
            int height = 600;
            string filename = "Render.png";
            PixelRGB[] pixels = new PixelRGB[width * height];

            using Image<Rgba32> image = new Image<Rgba32>(width, height);
            Scene world = new Scene(spheres);

            for (int j = 0; j < image.Height; j++)
            {
                for (int i = 0; i < image.Width; i++)
                {
                    float u = i / (float)width;
                    float v = (height - j) / (float)height;

                    Ray r = new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical);
                    Vector3 col = Color(r, world);

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
