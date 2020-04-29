using System;
using Firaxis.Granny;
using System.Reflection;
using System.Runtime.InteropServices;
using NexusBuddy.Utils;
using NexusBuddy.GrannyInfos;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NexusBuddy.GrannyWrappers
{
    public unsafe class GrannyFileWrapper
    {
        public IGrannyFile wrappedFile;
        public void* m_info;

        public GrannyFileWrapper(IGrannyFile inputFile)
        {
            wrappedFile = inputFile;
            Type myType = inputFile.GetType();
            FieldInfo fm_info = myType.GetField("m_info", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Pointer pm_info = (Pointer)fm_info.GetValue(inputFile);
            m_info = Pointer.Unbox(pm_info);
        }

        public IntPtr getArtToolInfoPtr()
        {
            return ((IntPtr)m_info + 0);
        }

        public unsafe void setFromArtToolInfo(string toolName, int majorNum, int minorNum)
        {
            setFromArtToolName(toolName);
            setArtToolRevisionNumbers(majorNum, minorNum);
        }

        public unsafe System.String getArtToolInfoName()
        {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(getArtToolInfoPtr())));
        }

        public unsafe void setFromArtToolName(string meshName)
        {
            sbyte* stringPointer = (sbyte*)Marshal.StringToHGlobalAnsi(meshName).ToPointer();
            IntPtr spointer = new IntPtr((void*)stringPointer);
            *(int*)*(int*)(getArtToolInfoPtr()) = (int)spointer;
        }

        public unsafe void setArtToolRevisionNumbers(int majorNum, int minorNum)
        {
            *(int*)(*(int*)(getArtToolInfoPtr()) + 8) = majorNum;
            *(int*)(*(int*)(getArtToolInfoPtr()) + 12) = minorNum;
        }

        public unsafe void setUnitsPerMeter(float unitsPerMeter)
        {
            *(float*)(*(int*)(getArtToolInfoPtr()) + 20) = unitsPerMeter;
        }

        public unsafe void setOrigin(float[] origin)
        {
            *(float*)(*(int*)(getArtToolInfoPtr()) + 24) = origin[0];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 28) = origin[1];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 32) = origin[2];
        }

        public unsafe void setMatrix(float[] matrix)
        {
            *(float*)(*(int*)(getArtToolInfoPtr()) + 36) = matrix[0];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 40) = matrix[1];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 44) = matrix[2];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 48) = matrix[3];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 52) = matrix[4];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 56) = matrix[5];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 60) = matrix[6];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 64) = matrix[7];
            *(float*)(*(int*)(getArtToolInfoPtr()) + 68) = matrix[8];
        }

        public IntPtr getExporterInfoPtr()
        {
            return ((IntPtr)m_info + 8);
        }

        public unsafe void setExporterInfo(string name, int majorNum, int minorNum, int customization, int build)
        {
            setExporterName(name);
            setExporterNumbers(majorNum, minorNum, customization, build);
        }
        
        public unsafe string getExporterName() {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(getExporterInfoPtr())));
        }

        public unsafe void setExporterName(string meshName)
        {
            sbyte* stringPointer = (sbyte*)Marshal.StringToHGlobalAnsi(meshName).ToPointer();
            IntPtr spointer = new IntPtr((void*)stringPointer);
            *(int*)*(int*)(getExporterInfoPtr()) = (int)spointer;
        }

        public unsafe void setExporterNumbers(int majorNum, int minorNum, int customization, int build)
        {
            *(int*)(*(int*)(getExporterInfoPtr()) + 8) = majorNum;
            *(int*)(*(int*)(getExporterInfoPtr()) + 12) = minorNum;
            *(int*)(*(int*)(getExporterInfoPtr()) + 16) = customization;
            *(int*)(*(int*)(getExporterInfoPtr()) + 20) = build;
        }

        public unsafe void setFromFileName(string meshName)
        {
            sbyte* stringPointer = (sbyte*)Marshal.StringToHGlobalAnsi(meshName).ToPointer();
            IntPtr spointer = new IntPtr((void*)stringPointer);
            *(int*)((IntPtr)m_info + 16) = (int)spointer;
        }

        public unsafe string getFromFileName()
        {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)((IntPtr)m_info + 16));
        }

        public int getNumTextures()
        {
            return *(int*)((IntPtr)m_info + 24);
        }

        public void setNumTextures(int num)
        {
            *(int*)((IntPtr)m_info + 24) = num;
        }

        public IntPtr getTexturesPtr()
        {
            return ((IntPtr)m_info + 28);
        }

        public int getNumMaterials()
        {
            return *(int*)((IntPtr)m_info + 36);
        }

        public void setNumMaterials(int num)
        {
            *(int*)((IntPtr)m_info + 36) = num;
        }

        public IntPtr getMaterialsPtr()
        {
            return ((IntPtr)m_info + 40);
        }

        public int getNumSkeletons()
        {
            return *(int*)((IntPtr)m_info + 48);
        }

        public void setNumSkeletons(int num)
        {
            *(int*)((IntPtr)m_info + 48) = num;
        }

        public IntPtr getSkeletonsPtr()
        {
            return ((IntPtr)m_info + 52);
        }

        public void addSkeletonPointer(long skeletonPtr)
        {
            int oldNumSkeletons = getNumSkeletons();
            int newNumSkeletons = oldNumSkeletons + 1;

            int oldSkeletonsPtr = *(int*)getSkeletonsPtr();
            *(int*)(getSkeletonsPtr()) = (int)Marshal.AllocHGlobal(newNumSkeletons * 8);
            int newSkeletonsPtr = *(int*)getSkeletonsPtr();

             MemoryUtil.MemCpy((void*)newSkeletonsPtr, (void*)oldSkeletonsPtr, (uint)oldNumSkeletons * 8);

             *(long*)(newSkeletonsPtr + oldNumSkeletons * 8) = skeletonPtr;

              setNumSkeletons(newNumSkeletons);
        }


        public void addModelPointer(long modelPtr)
        {
            int oldNumModels = getNumModels();
            int newNumModels = oldNumModels + 1;

            int oldModelsPtr = *(int*)getModelsPtr();
            *(int*)(getModelsPtr()) = (int)Marshal.AllocHGlobal(newNumModels * 8);
            int newModelsPtr = *(int*)getModelsPtr();

            MemoryUtil.MemCpy((void*)newModelsPtr, (void*)oldModelsPtr, (uint)oldNumModels * 8);

            *(long*)(newModelsPtr + oldNumModels * 8) = modelPtr;

            setNumModels(newNumModels);
        }


        public int getNumVertexDatas()
        {
            return *(int*)((IntPtr)m_info + 60);
        }

        public void setNumVertexDatas(int num)
        {
            *(int*)((IntPtr)m_info + 60) = num;
        }

        public IntPtr getVertexDatasPtr()
        {
            return ((IntPtr)m_info + 64);
        }


        public int getNumTriTopologies()
        {
            return *(int*)((IntPtr)m_info + 72);
        }

        public void setNumTriTopologies(int num)
        {
            *(int*)((IntPtr)m_info + 72) = num;
        }

        public IntPtr getTriTopologiesPtr()
        {
            return ((IntPtr)m_info + 76);
        }


        public int getNumMeshes()
        {
            return *(int*)((IntPtr)m_info + 84);
        }

        public void setNumMeshes(int numMeshes)
        {
            *(int*)((IntPtr)m_info + 84) = numMeshes;
        }

        public IntPtr getMeshesPtr()
        {
            return ((IntPtr)m_info + 88);
        }


        public int getNumModels()
        {
            return *(int*)((IntPtr)m_info + 96);
        }

        public void setNumModels(int num)
        {
            *(int*)((IntPtr)m_info + 96) = num;
        }

        public IntPtr getModelsPtr()
        {
            return ((IntPtr)m_info + 100);
        }

        public int getNumTrackGroups()
        {
            return *(int*)((IntPtr)m_info + 108);
        }

        public void setNumTrackGroups(int num)
        {
            *(int*)((IntPtr)m_info + 108) = num;
        }

        public IntPtr getTrackGroupsPtr()
        {
            return ((IntPtr)m_info + 112);
        }

        public int getNumAnimations()
        {
            return *(int*)((IntPtr)m_info + 120);
        }

        public void setNumAnimations(int num)
        {
            *(int*)((IntPtr)m_info + 120) = num;
        }

        public IntPtr getAnimationsPtr()
        {
            return ((IntPtr)m_info + 124);
        }

        public void setAnimationsPtr(int num)
        {
            *(int*)((IntPtr)m_info + 124) = num;
        }
        
        public void addVertexDatasPointer(int skeletonPtr)
        {
            int oldNumVertexDatas = getNumVertexDatas();
            int newNumVertexDatas = oldNumVertexDatas + 1;

            int oldVertexDatasPtr = *(int*)getVertexDatasPtr();
            *(int*)(getVertexDatasPtr()) = (int)Marshal.AllocHGlobal(newNumVertexDatas * 4);
            int newVertexDatasPtr = *(int*)getVertexDatasPtr();

            MemoryUtil.MemCpy((void*)newVertexDatasPtr, (void*)oldVertexDatasPtr, (uint)oldNumVertexDatas * 4);

            *(int*)(newVertexDatasPtr + oldNumVertexDatas * 4) = skeletonPtr;

            setNumVertexDatas(newNumVertexDatas);
        }

        public void addTriTopologiesPointer(int skeletonPtr)
        {
            int oldNumTriTopologies = getNumTriTopologies();
            int newNumTriTopologies = oldNumTriTopologies + 1;

            int oldTriTopologiesPtr = *(int*)getTriTopologiesPtr();
            *(int*)(getTriTopologiesPtr()) = (int)Marshal.AllocHGlobal(newNumTriTopologies * 4);
            int newTriTopologiesPtr = *(int*)getTriTopologiesPtr();

            MemoryUtil.MemCpy((void*)newTriTopologiesPtr, (void*)oldTriTopologiesPtr, (uint)oldNumTriTopologies * 4);

            *(int*)(newTriTopologiesPtr + oldNumTriTopologies * 4) = skeletonPtr;

            setNumTriTopologies(newNumTriTopologies);
        }

        public void createMaterials(int modelIndex, int meshIndex, GrannyMeshInfo grannyMeshInfo)
        {
            // Clear All Mesh Material Bindings
            CivNexusSixApplicationForm.form.SelectModel(modelIndex);

            CivNexusSixApplicationForm.form.SelectMesh(meshIndex);

            IGrannyMesh currentMesh = CivNexusSixApplicationForm.form.meshList.SelectedItems[0].Tag as IGrannyMesh;

            IGrannyMaterial[] materialBindingsArray = currentMesh.MaterialBindings.ToArray();

            for (int i = 0; i < materialBindingsArray.Length; i++)
            {
                IGrannyMaterial kMaterial = materialBindingsArray[i];
                currentMesh.RemoveMaterialBinding(kMaterial);
            }

            // Create Materials and Material Bindings
            for (int materialBindingIndex = 0; materialBindingIndex < grannyMeshInfo.materialBindingNames.Count; materialBindingIndex++)
            {
                string materialName = grannyMeshInfo.materialBindingNames[materialBindingIndex];

                IGrannyMaterial materialToBind = null;

                bool materialExists = false;
                for (int k = 0; k < wrappedFile.Materials.Count; k++)
                {
                    IGrannyMaterial checkMaterial = wrappedFile.Materials[k];
                    string checkMaterialName = checkMaterial.Name;
                    if (materialName.Equals(checkMaterialName))
                    {
                        materialExists = true;
                        materialToBind = wrappedFile.Materials[k];
                        break;
                    }
                }

                if (!materialExists)
                {
                    materialToBind = CivNexusSixApplicationForm.form.AddNewMaterial(materialName);
                }
                
                string text = materialToBind.Name + " (" + materialToBind.typeName + ")";

                CivNexusSixApplicationForm.form.triangleGroupsList.Items[materialBindingIndex].Text = text;

                for (int k = 0; k < wrappedFile.Materials.Count; k++)
                {
                    IGrannyMaterial checkMaterial = wrappedFile.Materials[k];
                    string checkMaterialName = checkMaterial.Name + " (" + checkMaterial.typeName + ")";
                    if (text.Equals(checkMaterialName))
                    {
                        currentMesh.AddMaterialBinding(checkMaterial);
                        break;
                    }
                }
            }

            CivNexusSixApplicationForm.form.SelectModel(0);
        }

        public void pruneMaterials()
        {
            List<IGrannyMaterial> allBoundMaterials = new List<IGrannyMaterial>();

            foreach (IGrannyMesh mesh in wrappedFile.Meshes)
            {
                foreach (IGrannyMaterial material in mesh.MaterialBindings)
                {
                    allBoundMaterials.Add(material);
                }
            }

            List<ListViewItem> materialItemsToDelete = new List<ListViewItem>();
            List<IGrannyMaterial> materialsToDelete = new List<IGrannyMaterial>();

            for (int k = 0; k < wrappedFile.Materials.Count; k++)
            {
                IGrannyMaterial fileMaterial = wrappedFile.Materials[k];
                string fileMaterialName = fileMaterial.Name + " (" + fileMaterial.typeName + ")";
                bool unused = true;
                for (int j = 0; j < allBoundMaterials.Count; j++)
                {
                    IGrannyMaterial checkMaterial = allBoundMaterials[j];
                    string checkMaterialName = checkMaterial.Name + " (" + checkMaterial.typeName + ")";
                    if (fileMaterialName.Equals(checkMaterialName))
                    {
                        unused = false;
                    }
                }

                if (unused)
                {
                    materialsToDelete.Add(fileMaterial);
                    materialItemsToDelete.Add(CivNexusSixApplicationForm.form.materialList.Items[k]);
                }
            }

            foreach (IGrannyMaterial delMaterial in materialsToDelete)
            {
                wrappedFile.RemoveMaterial(delMaterial);
                wrappedFile.Materials.Remove(delMaterial);
            }

            foreach (ListViewItem delMaterialItem in materialItemsToDelete)
            {
                CivNexusSixApplicationForm.form.materialList.Items.Remove(delMaterialItem);
            }
        }
    }
}
