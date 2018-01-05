using System;
using System.Collections.Generic;
using Firaxis.Granny;
using System.Reflection;
using System.Runtime.InteropServices;
using NexusBuddy.GrannyInfos;
using NexusBuddy.Utils;

namespace NexusBuddy.GrannyWrappers
{
    public unsafe class GrannyMeshWrapper
    {
        public IGrannyMesh wrappedMesh;
        public DummyGrannyMesh* m_pkMesh;

        public GrannyMeshWrapper(IGrannyMesh inputMesh)
        {
            wrappedMesh = inputMesh;
            Type myType = inputMesh.GetType();
            FieldInfo fm_pkMesh = myType.GetField("m_pkMesh", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            Pointer pm_pkMesh = (Pointer)fm_pkMesh.GetValue(inputMesh);
            m_pkMesh = (DummyGrannyMesh*)Pointer.Unbox(pm_pkMesh);
        }

        public unsafe System.String getName()
        {
            return Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(long*)(m_pkMesh));
        }

        public unsafe void setName(string meshName)
        {
            *(int*)(m_pkMesh) = (int)MemoryUtil.getStringIntPtr(meshName);
        }

        public IntPtr getNamePtr()
        {
            return ((IntPtr)m_pkMesh);
        }

        public IntPtr getPrimaryVertexDataPtr()
        {
            return ((IntPtr)m_pkMesh + 8);
        }

        //public int getNumMorphTargets()
        //{
        //    return *(int*)((IntPtr)this.m_pkMesh + 8);
        //}

        public int getNumGroups()
        {
            return *(int*)((IntPtr)this.m_pkMesh + 24);
        }

        public IntPtr getPrimaryTopologyPtr()
        {
            return ((IntPtr)m_pkMesh + 28);
        }

        public void setNumMaterialBindings(int num)
        {
            *(int*)((IntPtr)m_pkMesh + 36) = num;
        }

        public int getNumMaterialBindings()
        {
            return *(int*)((IntPtr)this.m_pkMesh + 36);
        }

        public IntPtr getMaterialBindingsPtr()
        {
            return ((IntPtr)m_pkMesh + 40);
        }

        public int getNumBoneBindings()
        {
            return *(int*)((IntPtr)this.m_pkMesh + 48);
        }

        public void setNumBoneBindings(int num)
        {
            *(int*)((IntPtr)m_pkMesh + 48) = num;
        }

        public IntPtr getBoneBindingsPtr()
        {
            return ((IntPtr)m_pkMesh + 52);
        }

        public int getNumVertices()
        {
            return *(int*)(*(long*)(getPrimaryVertexDataPtr()) + 8);
        }

        public void setNumVertices(int num)
        {
            *(int*)(*(int*)(getPrimaryVertexDataPtr()) + 8) = num;
        }

        public IntPtr getVerticesPtr()
        {
            return (IntPtr)(*(long*)(getPrimaryVertexDataPtr()) + 12);
        }

        public int getNumIndices()
        {
            return *(int*)(*(int*)(getPrimaryTopologyPtr()) + 12);
        }

        public void setNumIndices(int num)
        {
            *(int*)(*(int*)(getPrimaryTopologyPtr()) + 12) = num;
        }

        public IntPtr getIndicesPtr()
        {
            return (IntPtr)(*(int*)(getPrimaryTopologyPtr()) + 16);
        }

        public int getNumIndices16()
        {
            return *(int*)(*(int*)(getPrimaryTopologyPtr()) + 24);
        }

        public void setNumIndices16(int num)
        {
            *(int*)(*(int*)(getPrimaryTopologyPtr()) + 24) = num;
        }

        public IntPtr getIndices16Ptr()
        {
            return (IntPtr)(*(int*)(getPrimaryTopologyPtr()) + 28);
        }

        public int getNumTriangleGroups()
        {
            return *(int*)(*(int*)(getPrimaryTopologyPtr()));
        }

        public void setNumTriangleGroups(int value)
        {
            *(int*)(*(int*)(getPrimaryTopologyPtr())) = value;
        }

        public IntPtr getTriangleGroupPointer()
        {
            return (IntPtr)(*(int*)(getPrimaryTopologyPtr()) + 4);
        }

        public int getGroupMaterialIndexForIndex(int groupIndex)
        {
            return *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + groupIndex * 12 + 0);
        }

        public int getGroupTriFirstForIndex(int groupIndex)
        {
            return *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + groupIndex * 12 + 4);
        }

        public int getGroupTriCountForIndex(int groupIndex)
        {
            return *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + groupIndex * 12 + 8);
        }

        public void setGroupMaterialIndexForIndex(int groupIndex, int value)
        {
            *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + groupIndex * 12 + 0) = value;
        }

        public void setGroupTriFirstForIndex(int groupIndex, int value)
        {
            *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + groupIndex * 12 + 4) = value;
        }

        public void setGroupTriCountForIndex(int groupIndex, int value)
        {
            *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + groupIndex * 12 + 8) = value;
        }       

        //public int getGroup0TriCount()
        //{
        //    return *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + 8);
        //}

        //public void setGroup0TriCount(int num)
        //{
        //    *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + 8) = num;
        //}
        
        public unsafe GrannyMeshInfo getMeshInfo()
        {
            GrannyMeshInfo meshInfo = new GrannyMeshInfo();
            meshInfo.name = Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(m_pkMesh));
            
            List<string> boneBindings = new List<string>();
            for (int i = 0; i < getNumBoneBindings(); i++)
            {
                boneBindings.Add(Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(getBoneBindingsPtr()) + i * 44)));
            }
            meshInfo.boneBindings = boneBindings;

            meshInfo.setVertexStructInfos(getVertexStructInfos());

            int vertexCount = getNumVertices();
            IntPtr vertexPtr = getVerticesPtr();

            int vertexSize = meshInfo.bytesPerVertex;

            List<GrannyVertexInfo> vertexInfos = new List<GrannyVertexInfo>();
            for (int i = 0; i < vertexCount; i++)
            {
                GrannyVertexInfo currentVertex = new GrannyVertexInfo();

                GrannyMeshVertexStructInfo structInfo = meshInfo.getVertexStructInfoByName("Position");
                if (structInfo != null)
                {
                    currentVertex.position = new float[3];
                    currentVertex.position[0] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.position[1] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                    currentVertex.position[2] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 2));
                }

                structInfo = meshInfo.getVertexStructInfoByName("BoneWeights");
                if (structInfo != null)
                {
                    currentVertex.boneWeights = new int[structInfo.count];
                    for (int j = 0; j < structInfo.count; j++)
                    {
                        currentVertex.boneWeights[j] = (byte)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * j));
                    }
                }
                else
                {
                    int structInfoCount = 8;
                    currentVertex.boneWeights = new int[structInfoCount];
                    currentVertex.boneWeights[0] = 255;
                    for (int j = 1; j < structInfoCount; j++)
                    {
                        currentVertex.boneWeights[j] = 0;
                    }
                }

                structInfo = meshInfo.getVertexStructInfoByName("BoneIndices");
                if (structInfo != null)
                {
                    currentVertex.boneIndices = new int[structInfo.count];
                    for (int j = 0; j < structInfo.count; j++)
                    {
                        currentVertex.boneIndices[j] = (byte)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * j));
                    }
                }
                else
                {
                    int structInfoCount = 8;
                    currentVertex.boneIndices = new int[structInfoCount];
                    for (int j = 0; j < structInfoCount; j++)
                    {
                        currentVertex.boneIndices[j] = 0;
                    }
                }

                structInfo = meshInfo.getVertexStructInfoByName("Normal");
                if (structInfo != null)
                {
                    currentVertex.normal = new float[3];
                    currentVertex.normal[0] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.normal[1] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                    currentVertex.normal[2] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 2));
                }

                structInfo = meshInfo.getVertexStructInfoByName("Binormal");
                if (structInfo != null)
                {
                    currentVertex.binormal = new float[3];
                    currentVertex.binormal[0] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.binormal[1] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                    currentVertex.binormal[2] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 2));
                }

                structInfo = meshInfo.getVertexStructInfoByName("Tangent");
                if (structInfo != null)
                {
                    currentVertex.tangent = new float[3];
                    currentVertex.tangent[0] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.tangent[1] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                    currentVertex.tangent[2] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 2));
                }

                structInfo = meshInfo.getVertexStructInfoByName("TextureCoordinates0");
                currentVertex.uv = new float[2];
                if (structInfo != null)
                {        
                    currentVertex.uv[0] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.uv[1] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                }

                structInfo = meshInfo.getVertexStructInfoByName("TextureCoordinates1");
                currentVertex.uv2 = new float[2];
                if (structInfo != null)
                {
                    currentVertex.uv2[0] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.uv2[1] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                } else
                {
                    currentVertex.uv2[0] = 0f;
                    currentVertex.uv2[1] = 0f;
                }

                structInfo = meshInfo.getVertexStructInfoByName("TextureCoordinates2");
                currentVertex.uv3 = new float[2];
                if (structInfo != null)
                {                   
                    currentVertex.uv3[0] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 0));
                    currentVertex.uv3[1] = (float)structInfo.convert(*(int*)(vertexPtr) + (i * vertexSize) + structInfo.offset + (structInfo.length * 1));
                }
                else
                {
                    currentVertex.uv3[0] = 0f;
                    currentVertex.uv3[1] = 0f;
                }

                vertexInfos.Add(currentVertex);
            }

            meshInfo.vertices = vertexInfos;

            if (*(int*)(getMaterialBindingsPtr()) != 0)
            {
                string materialName = Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)*(int*)*(int*)(getMaterialBindingsPtr()));
                meshInfo.materialName = materialName;
            }

            int numTriangleGroups = *(int*)*(int*)getPrimaryTopologyPtr();
            List<PrimaryTopologyGroupInfo> primaryTopologyGroupInfos = new List<PrimaryTopologyGroupInfo>();
            for (int i = 0; i < numTriangleGroups; i++)
            {
                PrimaryTopologyGroupInfo primaryTopologyGroupInfo = new PrimaryTopologyGroupInfo();
                primaryTopologyGroupInfo.groupMaterialIndex = *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + i * 12 + 0);
                primaryTopologyGroupInfo.groupTriFirst = *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + i * 12 + 4);
                primaryTopologyGroupInfo.groupTriCount = *(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 4) + i * 12 + 8);
                primaryTopologyGroupInfos.Add(primaryTopologyGroupInfo);
            }

            meshInfo.primaryTopologyGroupInfos = primaryTopologyGroupInfos;

            int numIndices16 = getNumIndices16();
            int numIndices = getNumIndices();

            List<int> indices = new List<int>();
            if (numIndices16 > 0)
            {
                for (int i = 0; i < numIndices16; i++)
                {
                    indices.Add((int)*(ushort*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 28) + i * 2));
                }
            }
            else if (numIndices > 0)
            {
                for (int i = 0; i < numIndices; i++)
                {
                    indices.Add(*(int*)(*(int*)(*(int*)(getPrimaryTopologyPtr()) + 16) + i * 4));
                }
            }

            if (indices.Count > 0)
            {
                List<int[]> triangles = new List<int[]>();
                foreach (PrimaryTopologyGroupInfo info in primaryTopologyGroupInfos)
                {
                    for (int iTriangle = info.groupTriFirst; iTriangle < info.groupTriFirst + info.groupTriCount; iTriangle++)
                    {
                        int index0 = iTriangle * 3;
                        int index1 = index0 + 1;
                        int index2 = index0 + 2;

                        int[] triangle = new int[4];
                        triangle[0] = indices[index0];
                        triangle[1] = indices[index1];
                        triangle[2] = indices[index2];
                        triangle[3] = info.groupMaterialIndex;

                        triangles.Add(triangle);
                    }
                }
                meshInfo.triangles = triangles;
            }

            return meshInfo;
        }

        unsafe private List<GrannyMeshVertexStructInfo> getVertexStructInfos()
        {
            IntPtr vertexInfoPtr = (IntPtr)(*(int*)(getPrimaryVertexDataPtr()));

            List<GrannyMeshVertexStructInfo> vertexStructInfos = new List<GrannyMeshVertexStructInfo>();
            int z = 0;
            int infosize = 44;
            while (true)
            {
                int type = *(int*)(*(int*)(vertexInfoPtr) + (z * infosize) + 0);
                if (type != 0)
                {
                    string name = Marshal.PtrToStringAnsi((IntPtr)(sbyte*)*(int*)(*(int*)(vertexInfoPtr) + (z * infosize) + 4));
                    int count = *(int*)(*(int*)(vertexInfoPtr) + (z * infosize) + 20);
                    GrannyMeshVertexStructInfo info = new GrannyMeshVertexStructInfo(type, name, count);
                    vertexStructInfos.Add(info);
                    z++;
                }
                else
                {
                    break;
                }
            }
            return vertexStructInfos;
        }

        public unsafe void writeMeshInfo(GrannyMeshInfo meshInfo, bool isLeaderFormat, bool isSceneFormat)
        {
            meshInfo.setVertexStructInfos(getVertexStructInfos());
            setName(meshInfo.name);
            writeVertices(meshInfo);

            //PrimaryTopologyGroupInfo primaryTopologyGroupInfo = new PrimaryTopologyGroupInfo();
            //primaryTopologyGroupInfo.groupMaterialIndex = 0;
            //primaryTopologyGroupInfo.groupTriFirst = 0;
            //primaryTopologyGroupInfo.groupTriCount = meshInfo.triangles.Count;
            //List<PrimaryTopologyGroupInfo> primaryTopologyGroupInfos = new List<PrimaryTopologyGroupInfo>();
            //primaryTopologyGroupInfos.Add(primaryTopologyGroupInfo);

            writeTriangleGroups(meshInfo.primaryTopologyGroupInfos); 

            writeTriangles(meshInfo.triangles, true);
            writeBoneBindings(meshInfo.boneBindings);
        }

        unsafe private void writeVertices(GrannyMeshInfo meshInfo)
        {
            int vertexCount = meshInfo.vertices.Count;
            setNumVertices(vertexCount);

            int vertexSize = meshInfo.bytesPerVertex;

            int oldVerticesPtr = *(int*)getVerticesPtr();
            *(int*)(getVerticesPtr()) = (int)Marshal.AllocHGlobal(vertexCount * vertexSize);
            int newVerticesPtr = *(int*)getVerticesPtr();

            List<GrannyVertexInfo> vertexInfos = meshInfo.vertices;
            for (int i = 0; i < vertexCount; i++)
            {
                MemoryUtil.MemCpy((void*)(newVerticesPtr + i * vertexSize), (void*)oldVerticesPtr, (uint)vertexSize);

                GrannyVertexInfo currentVertex = vertexInfos[i];

                GrannyMeshVertexStructInfo structInfo = meshInfo.getVertexStructInfoByName("Position");
                if (structInfo != null)
                {
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = currentVertex.position[0];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = currentVertex.position[1];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 2)) = currentVertex.position[2];
                }

                structInfo = meshInfo.getVertexStructInfoByName("BoneWeights");
                if (structInfo != null)
                {
                    for (int j = 0; j < structInfo.count; j++)
                    {
                        if (j < currentVertex.boneWeights.Length)
                        {
                            *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * j)) = (byte)currentVertex.boneWeights[j];
                        }
                        else
                        {
                            *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * j)) = (byte)0;
                        }
                    }

                }

                structInfo = meshInfo.getVertexStructInfoByName("BoneIndices");
                if (structInfo != null)
                {
                    for (int j = 0; j < structInfo.count; j++)
                    {
                        if (j < currentVertex.boneIndices.Length)
                        {
                            *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * j)) = (byte)currentVertex.boneIndices[j];
                        }
                        else
                        {
                            *(byte*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * j)) = (byte)0;
                        }
                    }
                }

                structInfo = meshInfo.getVertexStructInfoByName("Normal");
                if (structInfo != null)
                {
                    // granny_real32 
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = currentVertex.normal[0];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = currentVertex.normal[1];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 2)) = currentVertex.normal[2];
                }

                structInfo = meshInfo.getVertexStructInfoByName("Tangent");
                if (structInfo != null && currentVertex.tangent != null)
                {
                    // granny_real32 
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = currentVertex.tangent[0];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = currentVertex.tangent[1];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 2)) = currentVertex.tangent[2];
                }

                structInfo = meshInfo.getVertexStructInfoByName("Binormal");
                if (structInfo != null && currentVertex.binormal != null)
                {
                    // granny_real32 
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = currentVertex.binormal[0];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = currentVertex.binormal[1];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 2)) = currentVertex.binormal[2];
                }

                structInfo = meshInfo.getVertexStructInfoByName("TextureCoordinates0");
                if (structInfo != null)
                {
                    // granny_real32 
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = currentVertex.uv[0];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = currentVertex.uv[1];
                }

                structInfo = meshInfo.getVertexStructInfoByName("TextureCoordinates1");
                if (structInfo != null)
                {
                    // granny_real32 
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = currentVertex.uv2[0];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = currentVertex.uv2[1];
                }

                structInfo = meshInfo.getVertexStructInfoByName("TextureCoordinates2");
                if (structInfo != null)
                {
                    // granny_real32 
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 0)) = currentVertex.uv3[0];
                    *(float*)(newVerticesPtr + (i * vertexSize) + structInfo.offset + (structInfo.length * 1)) = currentVertex.uv3[1];
                }
            }
        }

        unsafe private void writeTriangleGroups(List<PrimaryTopologyGroupInfo> triangleGroupInfos)
        {
            //int group0TriCount = triangles.Count;
            //setGroupTriCountForIndex(0, group0TriCount);

            int oldGroupsPtr = *(int*)getTriangleGroupPointer();
            *(int*)(getTriangleGroupPointer()) = (int)Marshal.AllocHGlobal(triangleGroupInfos.Count * 12);
            int newGroupsPtr = *(int*)getTriangleGroupPointer();

            for (int i = 0; i < triangleGroupInfos.Count; i++)
            {
                MemoryUtil.MemCpy((void*)(newGroupsPtr + i * 12), (void*)oldGroupsPtr, (uint)12);
                *(int*)(newGroupsPtr + i * 12 + 0) = triangleGroupInfos[i].groupMaterialIndex;
                *(int*)(newGroupsPtr + i * 12 + 4) = triangleGroupInfos[i].groupTriFirst;
                *(int*)(newGroupsPtr + i * 12 + 8) = triangleGroupInfos[i].groupTriCount;
            }

            setNumTriangleGroups(triangleGroupInfos.Count);
        }

        unsafe private void writeTriangles(List<int[]> triangles, bool useIndices16)
        {


            //setGroup0TriCount(group0TriCount);

            if (useIndices16)
            {
                // Assumes using 2 byte indices (Indices16 - not Indices)
                int indexSize = 2;
                int numIndicies = triangles.Count * 3;
                setNumIndices16(numIndicies);

                int oldIndicesPtr = *(int*)getIndices16Ptr();
                *(int*)(getIndices16Ptr()) = (int)Marshal.AllocHGlobal(numIndicies * indexSize);
                int newIndicesPtr = *(int*)getIndices16Ptr();

                for (int iTriangle = 0; iTriangle < triangles.Count; iTriangle++)
                {
                    int index0 = iTriangle * 3;
                    int index1 = index0 + 1;
                    int index2 = index0 + 2;

                    MemoryUtil.MemCpy((void*)(newIndicesPtr + index0 * indexSize), (void*)oldIndicesPtr, (uint)indexSize * 3);

                    int[] triangle = triangles[iTriangle];
                    *(ushort*)(newIndicesPtr + index0 * indexSize) = (ushort)triangle[0];
                    *(ushort*)(newIndicesPtr + index1 * indexSize) = (ushort)triangle[1];
                    *(ushort*)(newIndicesPtr + index2 * indexSize) = (ushort)triangle[2];
                }
            }
            else
            {
                // Assumes using 4 byte indices (Indices - not Indices16)
                int indexSize = 4;
                int numIndicies = triangles.Count * 3;
                setNumIndices(numIndicies);

                int oldIndicesPtr = *(int*)getIndicesPtr();
                *(int*)(getIndicesPtr()) = (int)Marshal.AllocHGlobal(numIndicies * indexSize);
                int newIndicesPtr = *(int*)getIndicesPtr();

                for (int iTriangle = 0; iTriangle < triangles.Count; iTriangle++)
                {
                    int index0 = iTriangle * 3;
                    int index1 = index0 + 1;
                    int index2 = index0 + 2;

                    MemoryUtil.MemCpy((void*)(newIndicesPtr + index0 * indexSize), (void*)oldIndicesPtr, (uint)indexSize * 3);

                    int[] triangle = triangles[iTriangle];
                    *(int*)(newIndicesPtr + index0 * indexSize) = triangle[0];
                    *(int*)(newIndicesPtr + index1 * indexSize) = triangle[1];
                    *(int*)(newIndicesPtr + index2 * indexSize) = triangle[2];
                }
            }
        }

        unsafe private void writeBoneBindings(List<string> boneBindings)
        {
            setNumBoneBindings(boneBindings.Count);

            int oldBBPointer = *(int*)getBoneBindingsPtr();
            // Blank out OBBMin/Max fields
            *(float*)(oldBBPointer + 8) = 0.0f;
            *(float*)(oldBBPointer + 12) = 0.0f;
            *(float*)(oldBBPointer + 16) = 0.0f;
            *(float*)(oldBBPointer + 20) = 0.0f;
            *(float*)(oldBBPointer + 24) = 0.0f;
            *(float*)(oldBBPointer + 28) = 0.0f;

            *(int*)(getBoneBindingsPtr()) = (int)Marshal.AllocHGlobal(boneBindings.Count * 44);
            int newBBPointer = *(int*)getBoneBindingsPtr();

            for (int i = 0; i < boneBindings.Count; i++)
            {
                MemoryUtil.MemCpy((void*)(newBBPointer + i * 44), (void*)oldBBPointer, 44); //copy first old bone binding to ith slot
                *(int*)(newBBPointer + i * 44) = (int)MemoryUtil.getStringIntPtr(boneBindings[i]); // overwrite bone binding string
            }
        }

        public bool meshEqual(IGrannyMesh otherMesh)
        {
            if (wrappedMesh.Name == otherMesh.Name &&
                wrappedMesh.IndexCount == otherMesh.IndexCount &&
                wrappedMesh.VertexCount == otherMesh.VertexCount &&
                wrappedMesh.BoneBindings.Count == otherMesh.BoneBindings.Count &&
                wrappedMesh.MaterialBindings.Count == otherMesh.MaterialBindings.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}