using System;
using System.Collections.Generic;
using Firaxis.Granny;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NexusBuddy.GrannyWrappers
{
    public unsafe class GrannyModelWrapper
    {
        public DummyGrannyModel* m_pkModel;
        public List<IGrannyMesh> m_lstMeshBindings;
        public IGrannySkeleton m_pkSkeleton;
        public IGrannyTransform m_kInitialPlacement;
        public IGrannyModel wrappedModel;

        public GrannyModelWrapper(IGrannyModel inputModel)
        {      
            wrappedModel = inputModel;

            Type myType = inputModel.GetType();
            FieldInfo fm_lstMeshBindings = myType.GetField("m_lstMeshBindings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            m_lstMeshBindings = (List<IGrannyMesh>)fm_lstMeshBindings.GetValue(inputModel);

            FieldInfo fm_pkSkeleton = myType.GetField("m_pkSkeleton", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            m_pkSkeleton = (IGrannySkeleton)fm_pkSkeleton.GetValue(inputModel);

            FieldInfo fm_kInitialPlacement = myType.GetField("m_kInitialPlacement", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            m_kInitialPlacement = (IGrannyTransform)fm_kInitialPlacement.GetValue(inputModel);

            FieldInfo fm_pkModel = myType.GetField("m_pkModel", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Pointer pm_pkModel = (Pointer)fm_pkModel.GetValue(inputModel);
            m_pkModel = (DummyGrannyModel*)Pointer.Unbox(pm_pkModel);
        }

        public IntPtr getNamePtr()
        {
            return ((IntPtr)m_pkModel);
        }

        public int getNumMeshBindings()
        {
            return *(int*)((IntPtr)m_pkModel + 84);
        }

        public void setNumMeshBindings(int num)
        {
            *(int*)((IntPtr)m_pkModel + 84) = num;
        }

        public IntPtr getMeshBindingsPtr()
        {
            return ((IntPtr)m_pkModel + 88);
        }

        public unsafe void setName(string meshName)
        {
            sbyte* stringPointer = (sbyte*)Marshal.StringToHGlobalAnsi(meshName).ToPointer();
            IntPtr spointer = new IntPtr((void*)stringPointer);
            *(int*)(m_pkModel) = (int)spointer;
        }

        public unsafe System.String getName()
        {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(m_pkModel));
        }
    }
}
