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
    public unsafe class GrannyMaterialWrapper
    {
        public IGrannyMaterial wrappedMaterial;
        public void* m_pkMaterial;

        public GrannyMaterialWrapper(IGrannyMaterial inputMaterial)
        {
            wrappedMaterial = inputMaterial;
            Type myType = inputMaterial.GetType();
            FieldInfo fm_pkMaterial = myType.GetField("m_pkMaterial", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Pointer pm_pkMaterial = (Pointer)fm_pkMaterial.GetValue(inputMaterial);
            m_pkMaterial = (void*)Pointer.Unbox(pm_pkMaterial);
        }

        public unsafe System.String getName()
        {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(m_pkMaterial));
        }

        public unsafe void setName(string name)
        {
            *(int*)(m_pkMaterial) = (int)MemoryUtil.getStringIntPtr(name);
        }
    }
}
