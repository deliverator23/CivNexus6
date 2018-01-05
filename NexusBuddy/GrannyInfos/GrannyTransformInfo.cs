using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firaxis.Granny;

namespace NexusBuddy.GrannyInfos
{
    public class GrannyTransformInfo
    {
		public float[] position; // float[3] x,y,z
        public float[] orientation; // float[4] quaternion
        public float[] scaleShear; //float[9] - quaternion for first 4 values - then identity matrix                
        public int flags; // public int flags; // hasOrientationEtc

        public static int getFlagsInt(ETransformFlags flags)
        {
            int output = 0;
            if (flags.HasFlag(ETransformFlags.GrannyHasPosition))
            {
                output = output + 1;
            }
            if (flags.HasFlag(ETransformFlags.GrannyHasOrientation))
            {
                output = output + 2;
            }
            if (flags.HasFlag(ETransformFlags.GrannyHasScaleShear))
            {
                output = output + 4;
            }
            return output;
        }
    }
}
