using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnumDescriptionAttribute : Attribute
     {
         private string description;
          public string Description { get { return description; } }
  
         public EnumDescriptionAttribute(string description)
             : base()
         {
             this.description = description;
         }
     }

     /// <summary>
     /// 获取枚举字符串
     /// </summary>
     public static class EnumHelper
     {
             public static string GetDescription(Enum value)
         {
                     if (value == null)
                         {
                             throw new ArgumentException("value");
                         }
                     string description = value.ToString();
                     var fieldInfo = value.GetType().GetField(description);
                    var attributes=(EnumDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
                    if (attributes != null && attributes.Length > 0)
                         {
                             description = attributes[0].Description;
                        }
                     return description;
                 }
         }
class Program
    {

        private static readonly int[] perm = {
        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
        151
    };
        static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }
        public static float Fbm(float x, float y, float z, int octave, float persistence = 0.5f)
        {
            Vector3 offset;
            Random rom = new Random(0);
            float total = 0.0f;
            float frequency = 1;
            float amplitude = 1;
            //用于将结果归一化
            float maxValue = 0;
            for (int i = 0; i < octave; i++)
            {
                offset = new Vector3(rom.NextDouble() * 1000, rom.NextDouble() * 1000, rom.NextDouble() * 1000);
                //Console.Write("{0:f} {1:f} {2:f}", offset.x, offset.y, offset.z);
                total += amplitude * Noise((x+offset.x) * frequency, (y+offset.y) * frequency, (z+offset.z) * frequency);
                maxValue += amplitude;
                frequency *= 2;
                amplitude *= persistence;
            }
            return total / maxValue;
        }
        static float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }
        public static float Noise(float x, float y, float z)
        {
            //计算输入点所在的“单位立方体”。0xff = 255

            var X = (int)Math.Floor(x) & 0xff;
            var Y = (int)Math.Floor(y) & 0xff;
            var Z = (int)Math.Floor(z) & 0xff;
            //左边界为(|x|,|y|,|z|)，右边界为 +1。接下来，我们计算出该点在立方体中的位置(0.0~1.0)。

            x -= (float)Math.Floor(x);
            y -= (float)Math.Floor(y);
            z -= (float)Math.Floor(z);

            //...
            var u = Fade(x);
            var v = Fade(y);
            var w = Fade(z);
            var A = (perm[X] + Y) & 0xff;
            var B = (perm[X + 1] + Y) & 0xff;
            var AA = (perm[A] + Z) & 0xff;
            var BA = (perm[B] + Z) & 0xff;
            var AB = (perm[A + 1] + Z) & 0xff;
            var BB = (perm[B + 1] + Z) & 0xff;

            var AAA = perm[AA];
            var BAA = perm[BA];
            var ABA = perm[AB];
            var BBA = perm[BB];
            var AAB = perm[AA + 1];
            var BAB = perm[BA + 1];
            var ABB = perm[AB + 1];
            var BBB = perm[BB + 1];
            float x1, x2, y1, y2;
            x1 = Lerp(Grad(AAA, x, y, z), Grad(BAA, x - 1, y, z), u);
            x2 = Lerp(Grad(ABA, x, y - 1, z), Grad(BBA, x - 1, y - 1, z), u);
            y1 = Lerp(x1, x2, v);

            x1 = Lerp(Grad(AAB, x, y, z - 1), Grad(BAB, x - 1, y, z - 1), u);
            x2 = Lerp(Grad(ABB, x, y - 1, z - 1), Grad(BBB, x - 1, y - 1, z - 1), u);
            y2 = Lerp(x1, x2, v);
            //为了方便起见，我们将结果范围设为0~1(理论上之前的min/max是[-1，1])。

            return (Lerp(y1, y2, w) + 1) / 2;

        }
        public static float Grad(int hash, float x, float y, float z)
        {
            switch (hash & 0xF)
            {
                case 0x0: return x + y;
                case 0x1: return -x + y;
                case 0x2: return x - y;
                case 0x3: return -x - y;
                case 0x4: return x + z;
                case 0x5: return -x + z;
                case 0x6: return x - z;
                case 0x7: return -x - z;
                case 0x8: return y + z;
                case 0x9: return -y + z;
                case 0xA: return y - z;
                case 0xB: return -y - z;
                case 0xC: return y + x;
                case 0xD: return -y + z;
                case 0xE: return y - x;
                case 0xF: return -y - z;
                default: return 0; // never happens
            }
        }
        public enum BlockType
        {
            [EnumDescription("◧")]
            None = 0,
            [EnumDescription("■")]
            Dirt = 1,
            [EnumDescription("◈")]
            Grass = 3,
            [EnumDescription("⬚")]
            Gravel = 4
        }
        
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            BlockType[,] map;
            int xmax = 20, ymax = 20,zmax=100;
            map = new BlockType[xmax,ymax];
            for (int x = 0; x < xmax; x++)
            {
                for (int y = 0; y < ymax; y++)
                {
                    float now= Fbm(x, y, 1, 3);
                    if (now > 0.7)
                    {
                        map[x, y] = BlockType.None;
                    } else if(now>0.5)
                    {
                        map[x, y] = BlockType.Dirt;
                    }else if(now>0.3)
                    {
                        map[x, y] = BlockType.Grass;
                    }else
                    {
                        map[x, y] = BlockType.Gravel;
                    }
                    Console.Write(EnumHelper.GetDescription(map[x,y]));
                }
                Console.WriteLine();
            }
            Console.WriteLine(map.Length);
            var Ho = Console.ReadLine();
            Console.WriteLine(Ho);
            Console.ReadKey();
            
        }
        }
    
    internal class Vector3
    {
        public float x, y, z;
        public Vector3(double x, double y, double z) 
        {
            this.x = (float)x;
            this.y = (float)y;
            this.z = (float)z;
        }
        public static Vector3 operator +(Vector3 V1, Vector3 V2) => new Vector3(V1.x + V2.x, V1.y + V2.y, V1.z + V2.z);
    }
}
