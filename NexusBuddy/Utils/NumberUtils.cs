using System;
using System.Globalization;

namespace NexusBuddy.Utils
{
    public static class NumberUtils
    {
        public static float halfToFloat(UInt16 u)
        {
            int sign = (u >> 15) & 0x00000001;
            int exp = (u >> 10) & 0x0000001F;
            int mant = u & 0x000003FF;
            exp = exp + (127 - 15);
            int i = (sign << 31) | (exp << 23) | (mant << 13);
            byte[] buff = BitConverter.GetBytes(i);
            return BitConverter.ToSingle(buff, 0);
        }

        // http://stackoverflow.com/questions/6162651/half-precision-floating-point-in-java/6162687#6162687
        public static UInt16 floatToHalf(float f)
        {
            int fbits = BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
            int sign = (int)((uint)fbits >> 16 & 0x8000);
            int val = (fbits & 0x7fffffff) + 0x1000; // rounded value

            if (val >= 0x47800000)               // might be or become NaN/Inf
            {                                     // avoid Inf due to rounding
                if ((fbits & 0x7fffffff) >= 0x47800000)
                {                                 // is or must become NaN/Inf
                    if (val < 0x7f800000)        // was value but too large
                        return (ushort)(sign | 0x7c00);     // make it +/-Inf
                    return (ushort)(sign | 0x7c00 |        // remains +/-Inf or NaN
                        (int)((uint)(fbits & 0x007fffff) >> 13)); // keep NaN (and Inf) bits
                }
                return (ushort)(sign | 0x7bff);             // unrounded not quite Inf
            }

            if (val >= 0x38800000)               // remains normalized value
                return (ushort)(sign | (int)((uint)val - 0x38000000 >> 13)); // exp - 127 + 15

            if (val < 0x33000000)                // too small for subnormal
                return (ushort)sign;                      // becomes +/-0

            val = (int)((uint)(fbits & 0x7fffffff) >> 23);  // tmp exp for subnormal calc

            return (ushort)(sign | (int)((uint)(fbits & 0x7fffff | 0x800000) // add subnormal bit
                 + (int)((uint)0x800000 >> val - 102)     // round depending on cut off
                 >> 126 - val));   // div by 2^(1-(exp-127+15)) and >> 13 | exp=0
        }

        public static UInt16 floatToHalfOld(float f)
        {
            byte[] bytes = BitConverter.GetBytes((double)f);
            ulong bits = BitConverter.ToUInt64(bytes, 0);
            ulong exponent = bits & 0x7ff0000000000000L;
            ulong mantissa = bits & 0x000fffffffffffffL;
            ulong sign = bits & 0x8000000000000000L;
            int placement = (int)((exponent >> 52) - 1023);
            if (placement > 15 || placement < -14)
                return floatToHalf(-1.0f); //Whatever counts as error
            UInt16 exponentBits = (UInt16)((15 + placement) << 10);
            UInt16 mantissaBits = (UInt16)(mantissa >> 42);
            UInt16 signBits = (UInt16)(sign >> 48);
            return (UInt16)(exponentBits | mantissaBits | signBits);
        }

        public static int parseInt(string str)
        {
            return int.Parse(str, CultureInfo.InvariantCulture);
        }

        public static float parseFloat(string str)
        {
            return float.Parse(str, CultureInfo.InvariantCulture);
        }

        public static bool almostEquals(this float float1, float float2, int precision)
        {
            double epsilon = Math.Pow(10.0, -precision);
            return (Math.Abs(float1 - float2) <= epsilon);
        }
    }
}
