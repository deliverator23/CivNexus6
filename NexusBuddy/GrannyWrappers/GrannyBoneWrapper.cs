using System;
using Firaxis.Granny;
using System.Reflection;
using System.Runtime.InteropServices;
using NexusBuddy.Utils;

namespace NexusBuddy.GrannyWrappers
{
    public unsafe class GrannyBoneWrapper
    {
        public IGrannyBone wrappedBone;
        public DummyGrannyBone* m_pkBone;

        public GrannyBoneWrapper(IGrannyBone inputBone)
        {
            wrappedBone = inputBone;
            Type myType = inputBone.GetType();
            FieldInfo fm_pkBone = myType.GetField("m_pkBone", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Pointer pm_pkBone = (Pointer)fm_pkBone.GetValue(inputBone);
            m_pkBone = (DummyGrannyBone*)Pointer.Unbox(pm_pkBone);
        }

        public unsafe System.String getName()
        {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(m_pkBone));
        }

        public unsafe void setName(string name)
        {
            *(int*)(m_pkBone) = (int)MemoryUtil.getStringIntPtr(name);
        }

        public float[] getPosition()
        {
            float[] matrixValue = new float[3];
            for (int j = 0; j < 3; j++)
            {
                matrixValue[j] = *(float*)((IntPtr)m_pkBone + 16 + (j * 4));
            }

            return matrixValue;
        }

        public float[] getOrientation()
        {
            float[] matrixValue = new float[4];
            for (int j = 0; j < 4; j++)
            {
                matrixValue[j] = *(float*)((IntPtr)m_pkBone + 28 + (j * 4));
            }

            return matrixValue;
        }

        public void setScaleShear(float[] scaleShear)
        {
            for (int j = 0; j < 9; j++)
            {
                *(float*)((IntPtr)m_pkBone + 44 + (j * 4)) = scaleShear[j];              
            }
        }

        public float[] getScaleShear()
        {
            float[] matrixValue = new float[9];
            for (int j = 0; j < 9; j++)
            {
                matrixValue[j] = *(float*)((IntPtr)m_pkBone + 44 + (j * 4));
            }

            return matrixValue;
        }

        public IntPtr getInverseWorldMatrixPtr()
        {
            return ((IntPtr)m_pkBone + 80);
        }

        public float[] getInverseWorldMatrix()
        {
            float[] matrixValue = new float[16];
            for (int j = 0; j < 16; j++)
            {
                matrixValue[j] = *(float*)((IntPtr)m_pkBone + 80 + (j * 4));
            }

            return matrixValue;
        }
    }
}
