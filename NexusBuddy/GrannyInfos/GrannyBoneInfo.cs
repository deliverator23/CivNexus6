using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NexusBuddy.GrannyInfos
{
    public class GrannyBoneInfo
    {
        public string name;
        public GrannyTransformInfo localTransform;
        public float[] inverseWorldTransform;
        public float LODError;
        public int parentIndex;
    }
}
