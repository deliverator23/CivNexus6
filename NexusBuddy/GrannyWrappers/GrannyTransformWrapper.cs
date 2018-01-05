using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firaxis.Granny;
using System.Reflection;
using System.Runtime.InteropServices;
using NexusBuddy.Utils;

namespace NexusBuddy.GrannyWrappers
{
    public unsafe class GrannyTransformWrapper
    {
        public IGrannyTransform wrappedTransform;
        public void* m_pkTransform;

        public GrannyTransformWrapper(IGrannyTransform inputTransform)
        {
            wrappedTransform = inputTransform;
            Type myType = inputTransform.GetType();
            FieldInfo fm_pkTransform = myType.GetField("m_pkTransform", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Pointer pm_pkTransform = (Pointer)fm_pkTransform.GetValue(inputTransform);
            m_pkTransform = (void*)Pointer.Unbox(pm_pkTransform);
        }

        public IntPtr getTransformPtr()
        {
            return ((IntPtr)m_pkTransform + 0);
        }

        public virtual unsafe float[] ScaleShear
        {
            get
            {
                float[] numArray = new float[9];
                for (int index = 0; index < 9; ++index)
                    numArray[index] = *(float*)((IntPtr)this.m_pkTransform + 32 + index * 4);
                return numArray;
            }
            set {
                float[] scaleShear = value;
                for (int index = 0; index < 9; ++index)
                    *(float*)((IntPtr)this.m_pkTransform + 32 + index * 4) = scaleShear[index];
            }
        }

        public virtual unsafe float[] Orientation
        {
            get
            {
                float[] numArray = new float[4];
                for (int index = 0; index < 4; ++index)
                    numArray[index] = *(float*)((IntPtr)this.m_pkTransform + 16 + index * 4);
                return numArray;
            }
            set
            {
                float[] orientation = value;
                for (int index = 0; index < 4; ++index)
                    *(float*)((IntPtr)this.m_pkTransform + 16 + index * 4) = orientation[index];
            }
        }

        public virtual unsafe float[] Position
        {
            get
            {
                float[] numArray = new float[3];
                for (int index = 0; index < 3; ++index)
                    numArray[index] = *(float*)((IntPtr)this.m_pkTransform + 4 + index * 4);
                return numArray;
            }

            set
            {
                float[] position = value;
                for (int index = 0; index < 3; ++index)
                    *(float*)((IntPtr)this.m_pkTransform + 4 + index * 4) = position[index];
            }
        }

        public virtual unsafe int Flags
        {
            get
            {
                return *(int*)this.m_pkTransform;
            }
            set
            {
                *(int*)this.m_pkTransform = value;
            }
        }
    }
}
