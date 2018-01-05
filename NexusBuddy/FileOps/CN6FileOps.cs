using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using NexusBuddy.GrannyWrappers;
using NexusBuddy.GrannyInfos;
using NexusBuddy.Utils;
using Firaxis.Granny;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace NexusBuddy.FileOps
{
    class CN6FileOps
    {
        public static void exportAllModelsToCN6(IGrannyFile grannyFile)
        {
            for (int i = 0; i < grannyFile.Models.Count; i++)
            {
                cn6Export(grannyFile, i);
            }
        }

        private static string getTemplateFilename(int vertexFormat)
        {
            string templateFilename = CivNexusSixApplicationForm.form.modelTemplateFilename;
            if (vertexFormat == 1)
            {
                templateFilename = CivNexusSixApplicationForm.form.modelTemplateFilename2;
            }
            else if (vertexFormat == 2)
            {
                templateFilename = CivNexusSixApplicationForm.form.modelTemplateFilename3;
            }
            return templateFilename;
        }

        public unsafe static void importCN6(string sourceFilename, string outputFilename, GrannyContext grannyContext, int vertexFormat)
        {
            string templateFilename = getTemplateFilename(vertexFormat);
            IGrannyFile file = CivNexusSixApplicationForm.form.OpenFileAsTempFileCopy(templateFilename, "tempimport");

            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(file);

            fileWrapper.setNumMeshes(0);
            fileWrapper.setNumVertexDatas(0);
            fileWrapper.setNumSkeletons(0);
            fileWrapper.setNumTriTopologies(0);

            GrannyModelInfo modelInfo = loadModelInfo(sourceFilename, vertexFormat);
            doBoneBindings(modelInfo);

            List<IGrannyFile> meshFileList = new List<IGrannyFile>();

            foreach (GrannyMeshInfo meshInfo in modelInfo.meshBindings)
            {
                meshFileList.Add(writeMesh(meshInfo, templateFilename));
            }

            IGrannyModel model = file.Models[0];
            GrannyModelWrapper modelWrapper = new GrannyModelWrapper(model);

            modelWrapper.setNumMeshBindings(0);
            model.MeshBindings.Clear();

            foreach (IGrannyFile meshFile in meshFileList)
            {
                doAppendMeshBinding(file, meshFile, 0);
            }

            fileWrapper.setNumMeshes(0);
            file.Meshes.Clear();

            foreach (IGrannyMesh mesh in file.Models[0].MeshBindings)
            {
                file.AddMeshReference(mesh);
            }

            GrannySkeletonWrapper skeletonWrapper = new GrannySkeletonWrapper(file.Models[0].Skeleton);
            skeletonWrapper.writeSkeletonInfo(modelInfo.skeleton);

            string worldBoneName = modelInfo.skeleton.bones[0].name;
            modelWrapper.setName(worldBoneName);
            skeletonWrapper.setName(worldBoneName);

            fileWrapper.setFromArtToolInfo("Blender", 2, 0);
            float[] matrix = { 1f, 0f, 0f, 0f, 0f, 1f, 0f, -1f, 0f };
            fileWrapper.setMatrix(matrix);
            CivNexusSixApplicationForm.SetExporterInfo(fileWrapper);
            fileWrapper.setFromFileName(sourceFilename);

            fileWrapper.addSkeletonPointer((int)skeletonWrapper.m_pkSkeleton);

            createAndBindMaterials(outputFilename, file, modelInfo);
        }

        public static void overwriteMeshes(IGrannyFile file, string sourceFilename, GrannyContext grannyContext, int currentModelIndex, int vertexFormat)
        {
            string filename = file.Filename;

            GrannyModelInfo modelInfo = loadModelInfo(sourceFilename, vertexFormat);
            doBoneBindings(modelInfo);

            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(file);

            List<IGrannyFile> meshFileList = new List<IGrannyFile>();
            foreach (GrannyMeshInfo meshInfo in modelInfo.meshBindings)
            {
                meshFileList.Add(writeMesh(meshInfo, getTemplateFilename(vertexFormat)));
            }

            IGrannyModel model = file.Models[currentModelIndex];
            GrannyModelWrapper modelWrapper = new GrannyModelWrapper(model);

            modelWrapper.setNumMeshBindings(0);
            model.MeshBindings.Clear();

            foreach (IGrannyFile meshFile in meshFileList)
            {
                doAppendMeshBinding(file, meshFile, currentModelIndex);
            }

            fileWrapper.setNumMeshes(0);
            fileWrapper.setNumTriTopologies(0);
            fileWrapper.setNumVertexDatas(0);
            file.Meshes.Clear();

            int meshesCount = 0;
            foreach (IGrannyModel loopModel in file.Models) {
                foreach (IGrannyMesh mesh in loopModel.MeshBindings)
                {
                    file.AddMeshReference(mesh);
                    meshesCount++;
                }
            }

            fileWrapper.setFromArtToolInfo("Blender", 2, 0);
            //fileWrapper.setUnitsPerMeter(10.7f);
            CivNexusSixApplicationForm.SetExporterInfo(fileWrapper);
            fileWrapper.setFromFileName(sourceFilename);

            fileWrapper.setNumMeshes(meshesCount);
            CivNexusSixApplicationForm.form.SaveAsAction(file, filename, false);

            createAndBindMaterials(filename, file, modelInfo);
        }

        private static unsafe void createAndBindMaterials(string outputFilename, IGrannyFile file, GrannyModelInfo modelInfo)
        {
            GrannyFileWrapper fileWrapper2 = new GrannyFileWrapper(CivNexusSixApplicationForm.form.SaveAsAction(file, outputFilename, false));

            CivNexusSixApplicationForm.form.RefreshAppData();

            for (int meshIndex = 0; meshIndex < modelInfo.meshBindings.Count; meshIndex++)
            {
                GrannyMeshInfo meshInfo = modelInfo.meshBindings[meshIndex];
                fileWrapper2.createMaterials(meshIndex, meshInfo);
            }

            GrannyFileWrapper fileWrapper3 = new GrannyFileWrapper(CivNexusSixApplicationForm.form.SaveAsAction(fileWrapper2.wrappedFile, outputFilename, false));

            fileWrapper3.pruneMaterials();

            CivNexusSixApplicationForm.form.SaveAsAction(fileWrapper3.wrappedFile, outputFilename, false);

            CivNexusSixApplicationForm.form.RefreshAppData();
        }

        public static IGrannyFile writeMesh(GrannyMeshInfo meshInfo, string templateFilename)
        {
            IGrannyFile appendFile = CivNexusSixApplicationForm.form.OpenFileAsTempFileCopy(templateFilename, "tempappend");
            appendFile.Meshes.RemoveRange(1, appendFile.Meshes.Count - 1);
            IGrannyMesh mesh = appendFile.Models[0].MeshBindings[0];

            GrannyMeshWrapper meshWrapper = new GrannyMeshWrapper(mesh);
            meshWrapper.writeMeshInfo(meshInfo, false, false);

            return appendFile;
        }

        public unsafe static void doAppendMeshBinding(IGrannyFile inputFile, IGrannyFile appendFile, Int32 currentModelIndex)
        {
            // Get wrappers
            IGrannyModel inputModel = inputFile.Models[currentModelIndex];
            GrannyModelWrapper inputModelWrapper = new GrannyModelWrapper(inputModel);

            IGrannyModel appendModel = appendFile.Models[0];
            GrannyModelWrapper appendModelWrapper = new GrannyModelWrapper(appendModel);

            // Update model 
            inputModel.MeshBindings.Add(appendFile.Meshes[0]);

            int meshCountInput = inputModelWrapper.getNumMeshBindings();
            int newMeshBindingsCount = meshCountInput + 1;
            inputModelWrapper.setNumMeshBindings(newMeshBindingsCount);
            
            // Update model mesh bindings
            int oldMeshBindingsPtr = *(int*)inputModelWrapper.getMeshBindingsPtr();
            *(int*)inputModelWrapper.getMeshBindingsPtr() = (int)Marshal.AllocHGlobal(newMeshBindingsCount * 8);
            int newMeshBindingsPtr = *(int*)inputModelWrapper.getMeshBindingsPtr();

            int modelMeshBindingsPtrAppend = *(int*)appendModelWrapper.getMeshBindingsPtr();
            if (meshCountInput > 0)
            {
                MemoryUtil.MemCpy((void*)newMeshBindingsPtr, (void*)oldMeshBindingsPtr, (uint)(meshCountInput * 8));
            }
            MemoryUtil.MemCpy((void*)(newMeshBindingsPtr + meshCountInput * 8), (void*)modelMeshBindingsPtrAppend, 8);
        }
     
        private static GrannyModelInfo loadModelInfo(string filename, int vertexFormat)
        {
            string currentLine = "";
            string numberRegex = "([-+]?[0-9]+\\.?[0-9]*) ";

            StreamReader streamReader = new StreamReader(filename);

            while (!currentLine.StartsWith("skeleton"))
            {
                currentLine = streamReader.ReadLine();
            }

            GrannyModelInfo modelInfo = new GrannyModelInfo();

            GrannySkeletonInfo skeletonInfo = new GrannySkeletonInfo();
            List<GrannyBoneInfo> skeletonBones = new List<GrannyBoneInfo>();

            while (!currentLine.StartsWith("meshes"))
            {
                currentLine = streamReader.ReadLine();
                if (!currentLine.StartsWith("meshes"))
                {
                    string regexString = "([0-9]+) \"(.+)\" ";
                    for (int i = 0; i < 24; i++)
                    {
                        regexString = regexString + numberRegex;
                    }

                    Regex regex = new Regex(regexString.Trim());
                    MatchCollection mc = regex.Matches(currentLine);
                    foreach (Match m in mc)
                    {
                        GrannyBoneInfo boneInfo = new GrannyBoneInfo();
                        GrannyTransformInfo transformInfo = new GrannyTransformInfo();

                        //int boneindex = NumberUtils.parseInt(m.Groups[1].Value.Trim());
                        string boneName = m.Groups[2].Value;
                        int parentIndex = NumberUtils.parseInt(m.Groups[3].Value.Trim());

                        float[] position = new float[3];
                        position[0] = NumberUtils.parseFloat(m.Groups[4].Value.Trim());
                        position[1] = NumberUtils.parseFloat(m.Groups[5].Value.Trim());
                        position[2] = NumberUtils.parseFloat(m.Groups[6].Value.Trim());

                        float[] orientation = new float[4];
                        orientation[0] = NumberUtils.parseFloat(m.Groups[7].Value.Trim());
                        orientation[1] = NumberUtils.parseFloat(m.Groups[8].Value.Trim());
                        orientation[2] = NumberUtils.parseFloat(m.Groups[9].Value.Trim());
                        orientation[3] = NumberUtils.parseFloat(m.Groups[10].Value.Trim());

                        float[] scaleShear = new float[9];
                        scaleShear[0] = 1.0f;
                        scaleShear[1] = 0.0f;
                        scaleShear[2] = 0.0f;
                        scaleShear[3] = 0.0f;
                        scaleShear[4] = 1.0f;
                        scaleShear[5] = 0.0f;
                        scaleShear[6] = 0.0f;
                        scaleShear[7] = 0.0f;
                        scaleShear[8] = 1.0f;

                        float[] invWorld = new float[16];

                        for (int j = 0; j < 16; j++)
                        {
                            invWorld[j] = NumberUtils.parseFloat(m.Groups[j + 11].Value.Trim());
                        }


                        bool hasPosition = true;
                        bool hasOrientation = true;
                        int flags = 0;

                        if (NumberUtils.almostEquals(position[0], 0.0f, 4) && NumberUtils.almostEquals(position[1], 0.0f, 4) && NumberUtils.almostEquals(position[2], 0.0f, 4))
                        {
                            hasPosition = false;
                        }

                        if (NumberUtils.almostEquals(orientation[0], 0.0f, 5) && NumberUtils.almostEquals(orientation[1], 0.0f, 5) &&
                            NumberUtils.almostEquals(orientation[2], 0.0f, 5) && NumberUtils.almostEquals(orientation[3], 1.0f, 5))
                        {
                            hasOrientation = false;
                        }

                        if (hasPosition)
                        {
                            flags = flags + 1;
                        }

                        if (hasOrientation)
                        {
                            flags = flags + 2;
                        }

                        transformInfo.flags = flags;
                        transformInfo.position = position;
                        transformInfo.orientation = orientation;
                        transformInfo.scaleShear = scaleShear;

                        boneInfo.name = boneName;
                        boneInfo.parentIndex = parentIndex;
                        boneInfo.localTransform = transformInfo;
                        boneInfo.inverseWorldTransform = invWorld;
                        boneInfo.LODError = 0;

                        skeletonBones.Add(boneInfo);
                    }
                }
            }

            skeletonInfo.bones = skeletonBones;

            // Read Meshes
            int numMeshes = NumberUtils.parseInt(currentLine.Replace("meshes:", ""));
            List<GrannyMeshInfo> meshInfos = new List<GrannyMeshInfo>();
            for (int meshId = 0; meshId < numMeshes; meshId++)
            {
                string meshName = "";
                while (!currentLine.StartsWith("mesh:"))
                {
                    currentLine = streamReader.ReadLine();
                }

                string regexString = "\"(.+)\"";
                Regex regex = new Regex(regexString);
                MatchCollection mc = regex.Matches(currentLine);

                foreach (Match m in mc)
                {
                    meshName = m.Groups[1].Value;
                }

                // Read Materials
                List<string> materialNames = new List<string>();
                while (!currentLine.StartsWith("vertices"))
                {
                    currentLine = streamReader.ReadLine();
                    if (!currentLine.StartsWith("materials") && !currentLine.StartsWith("vertices"))
                    {
                        mc = regex.Matches(currentLine);

                        foreach (Match m in mc)
                        {
                            string materialName = m.Groups[1].Value;
                            materialNames.Add(materialName);
                        }
                    }
                }

                // Read Vertices
                int unweightedVertexCount = 0;

                List<GrannyVertexInfo> vertexInfos = new List<GrannyVertexInfo>();
                while (!currentLine.StartsWith("triangles"))
                {
                    currentLine = streamReader.ReadLine();
                    if (!currentLine.StartsWith("vertices") && !currentLine.StartsWith("triangles"))
                    {
                        regexString = "";
                        for (int i = 0; i < 34; i++)
                        {
                            regexString = regexString + numberRegex;
                        }

                        regex = new Regex(regexString.Trim());
                        mc = regex.Matches(currentLine);
                        foreach (Match m in mc)
                        {
                            GrannyVertexInfo vertexInfo = new GrannyVertexInfo();

                            float[] position = new float[3];
                            position[0] = NumberUtils.parseFloat(m.Groups[1].Value.Trim());
                            position[1] = NumberUtils.parseFloat(m.Groups[2].Value.Trim());
                            position[2] = NumberUtils.parseFloat(m.Groups[3].Value.Trim());

                            float[] normal = new float[3];
                            normal[0] = NumberUtils.parseFloat(m.Groups[4].Value.Trim());
                            normal[1] = NumberUtils.parseFloat(m.Groups[5].Value.Trim());
                            normal[2] = NumberUtils.parseFloat(m.Groups[6].Value.Trim());

                            float[] tangent = new float[3];
                            tangent[0] = NumberUtils.parseFloat(m.Groups[7].Value.Trim());
                            tangent[1] = NumberUtils.parseFloat(m.Groups[8].Value.Trim());
                            tangent[2] = NumberUtils.parseFloat(m.Groups[9].Value.Trim());

                            float[] binormal = new float[3];
                            binormal[0] = NumberUtils.parseFloat(m.Groups[10].Value.Trim());
                            binormal[1] = NumberUtils.parseFloat(m.Groups[11].Value.Trim());
                            binormal[2] = NumberUtils.parseFloat(m.Groups[12].Value.Trim());

                            float[] uv = new float[2];
                            uv[0] = NumberUtils.parseFloat(m.Groups[13].Value.Trim());
                            uv[1] = NumberUtils.parseFloat(m.Groups[14].Value.Trim());

                            float[] uv2 = new float[2];
                            uv2[0] = NumberUtils.parseFloat(m.Groups[15].Value.Trim());
                            uv2[1] = NumberUtils.parseFloat(m.Groups[16].Value.Trim());

                            float[] uv3 = new float[2];
                            uv3[0] = NumberUtils.parseFloat(m.Groups[17].Value.Trim());
                            uv3[1] = NumberUtils.parseFloat(m.Groups[18].Value.Trim());

                            int[] boneIndices = new int[8];
                            for (int j = 0; j < 8; j++)
                            {
                                boneIndices[j] = NumberUtils.parseInt(m.Groups[j+19].Value.Trim());
                            }

                            int[] boneWeights = new int[8];
                            for (int j = 0; j < 8; j++)
                            {
                                boneWeights[j] = NumberUtils.parseInt(m.Groups[j+27].Value.Trim());
                            }

                            // Assign unweighted vertices to root bone and record count
                            if (boneWeights[0] == -1)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    boneIndices[j] = 0;
                                }

                                boneWeights[0] = 255;

                                for (int j = 1; j < 8; j++)
                                {
                                    boneWeights[j] = 0;
                                }

                                unweightedVertexCount++;
                            }
                            
                            vertexInfo.position = position;
                            vertexInfo.normal = normal;
                            vertexInfo.tangent = tangent;
                            vertexInfo.binormal = binormal;

                            vertexInfo.uv = uv;
                            vertexInfo.uv2 = uv2;
                            vertexInfo.uv3 = uv3;

                            vertexInfo.boneIndices = boneIndices;
                            vertexInfo.boneWeights = boneWeights;

                            vertexInfos.Add(vertexInfo);
                        }
                    }
                }

                if (unweightedVertexCount > 0 && vertexFormat == 0) 
                {
                    MessageBox.Show(unweightedVertexCount + " unweighted vertices have been assigned to root bone.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }

                // Read Triangles
                List<int[]> triangles = new List<int[]>();
                while (!currentLine.StartsWith("mesh") && !currentLine.StartsWith("end"))
                {
                    currentLine = streamReader.ReadLine();
                    if (!currentLine.StartsWith("mesh") && !currentLine.StartsWith("end"))
                    {
                        regexString = "";
                        for (int i = 0; i < 4; i++)
                        {
                            regexString = regexString + "([-+]?[0-9]+\\.?[0-9]*) ";
                        }

                        regex = new Regex(regexString.Trim());
                        mc = regex.Matches(currentLine);
                        

                        foreach (Match m in mc)
                        {
                            int[] triangle = new int[4];
                            triangle[0] = NumberUtils.parseInt(m.Groups[1].Value.Trim());
                            triangle[1] = NumberUtils.parseInt(m.Groups[2].Value.Trim());
                            triangle[2] = NumberUtils.parseInt(m.Groups[3].Value.Trim());
                            triangle[3] = NumberUtils.parseInt(m.Groups[4].Value.Trim());

                            triangles.Add(triangle);
                        }

                    }
                }

                List<PrimaryTopologyGroupInfo> groupInfos = new List<PrimaryTopologyGroupInfo>();

                int maxMaterialIndex = 0;
                int groupIndexStart = 0;
                int triIndex;
                for (triIndex = 0; triIndex < triangles.Count; triIndex++)
                {
                    int[] triangle = triangles[triIndex];
                    int triMaterialIndex = triangle[3];

                    if (triMaterialIndex > maxMaterialIndex)
                    {
                        groupInfos.Add(new PrimaryTopologyGroupInfo(maxMaterialIndex, groupIndexStart, triIndex - groupIndexStart));
                        maxMaterialIndex = triMaterialIndex;
                        groupIndexStart = triIndex;
                    }
                }

                groupInfos.Add(new PrimaryTopologyGroupInfo(maxMaterialIndex, groupIndexStart, triIndex - groupIndexStart));

                GrannyMeshInfo meshInfo = new GrannyMeshInfo();
                meshInfo.name = meshName;
                meshInfo.vertices = vertexInfos;
                meshInfo.triangles = triangles;
                meshInfo.primaryTopologyGroupInfos = groupInfos;
                meshInfo.materialBindingNames = materialNames;

                meshInfos.Add(meshInfo);
            }

            modelInfo.skeleton = skeletonInfo;
            modelInfo.meshBindings = meshInfos;

            streamReader.Close();

            return modelInfo;
        }

        private static void doBoneBindings(GrannyModelInfo modelInfo)
        {
            GrannySkeletonInfo skeletonInfo = modelInfo.skeleton;

            foreach (GrannyMeshInfo meshInfo in modelInfo.meshBindings)
            {
                HashSet<int> distinctBoneIds = new HashSet<int>();

                foreach (GrannyVertexInfo vertexInfo in meshInfo.vertices)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        distinctBoneIds.Add(vertexInfo.boneIndices[i]);
                    }
                }

                Dictionary<int, int> globalToLocalBoneIdDict = new Dictionary<int, int>();
                List<int> distinctBoneIdList = distinctBoneIds.ToList();
                List<string> boneBindingNames = new List<string>();
                for (int i = 0; i < distinctBoneIdList.Count; i++)
                {
                    boneBindingNames.Add(skeletonInfo.bones[distinctBoneIdList[i]].name);
                    globalToLocalBoneIdDict.Add(distinctBoneIdList[i], i);
                }

                foreach (GrannyVertexInfo vertexInfo in meshInfo.vertices)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        int localBoneId = globalToLocalBoneIdDict[vertexInfo.boneIndices[i]];
                        vertexInfo.boneIndices[i] = localBoneId;
                    }
                }

                meshInfo.boneBindings = boneBindingNames;
            }
        }

        public static void cn6Export(IGrannyFile grannyFile, int currentModelIndex)
        {
            string fileExtension = ".cn6";
            string outputFilename = "";
            string numberFormat = "f8";

            if (grannyFile.Models.Count > 1)
            {
                outputFilename = grannyFile.Filename.Replace(".fgx", "_model" + currentModelIndex + fileExtension);
                outputFilename = outputFilename.Replace(".FGX", "_model" + currentModelIndex + fileExtension);
            }
            else
            {
                outputFilename = grannyFile.Filename.Replace(".fgx", fileExtension);
                outputFilename = outputFilename.Replace(".FGX", fileExtension);
            }

            StreamWriter outputWriter = new StreamWriter(new FileStream(outputFilename, FileMode.Create));

            IGrannyModel model = grannyFile.Models[currentModelIndex];
            IGrannySkeleton skeleton = model.Skeleton;

            // Lookup so we can identify the meshes belonging the current model in the list of file meshes
            Dictionary<int, int> meshBindingToMesh = new Dictionary<int, int>();
            HashSet<string> distinctMeshNames = new HashSet<string>();
            for (int i = 0; i < model.MeshBindings.Count; i++)
            {
                for (int j = 0; j < grannyFile.Meshes.Count; j++)
                {
                    GrannyMeshWrapper modelMesh = new GrannyMeshWrapper(model.MeshBindings[i]);
                    IGrannyMesh fileMesh = grannyFile.Meshes[j];
                    if (modelMesh.meshEqual(fileMesh))
                    {
                        meshBindingToMesh.Add(i, j);
                    }
                }
                distinctMeshNames.Add(model.MeshBindings[i].Name);
            }

            // Used to give meshes distinct names where we have multiple meshes with the same name in our source gr2
            Dictionary<string, int> meshNameCount = new Dictionary<string, int>();
            foreach (string meshName in distinctMeshNames)
            {
                meshNameCount.Add(meshName, 0);
            }

            List<GrannyMeshInfo> grannyMeshInfos = new List<GrannyMeshInfo>();
            for (int i = 0; i < model.MeshBindings.Count; i++)
            {
                GrannyMeshWrapper meshWrapper = new GrannyMeshWrapper(model.MeshBindings[i]);
                grannyMeshInfos.Add(meshWrapper.getMeshInfo());
            }

            BiLookup<int, string> boneLookup = new BiLookup<int, string>();
            for (int i = 0; i < skeleton.Bones.Count; i++)
            {
                boneLookup.Add(i, skeleton.Bones[i].Name);
            }

            Dictionary<string, double[]> boneNameToPositionMap = new Dictionary<string, double[]>();
            foreach (IGrannyBone bone in skeleton.Bones)
            {
                double[] bonePosition = NB2NA2FileOps.getBoneWorldPosition(bone);
                boneNameToPositionMap.Add(bone.Name, bonePosition);

            }

            outputWriter.WriteLine("// CivNexus6 CN6 - Exported from " + CivNexusSixApplicationForm.form.GetApplicationNameWithVersionNumber());
            outputWriter.WriteLine("skeleton");

            // Write Bones
            for (int boneIndex = 0; boneIndex < skeleton.Bones.Count; boneIndex++)
            {
                IGrannyBone bone = skeleton.Bones[boneIndex];
                string boneName = bone.Name;
                IGrannyTransform transform = bone.LocalTransform;
                float[] orientation = transform.Orientation;
                float[] position = transform.Position;
                float[] invWorldTransform = bone.InverseWorldTransform;

                StringBuilder boneStringBuilder = new StringBuilder(boneIndex + " \"" + boneName + "\" " + bone.ParentIndex + " " + position[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + position[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + position[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                                orientation[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + orientation[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + orientation[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + orientation[3].ToString(numberFormat, CultureInfo.InvariantCulture));

                foreach (float j in invWorldTransform)
                {
                    boneStringBuilder.Append(" " + j.ToString(numberFormat, CultureInfo.InvariantCulture));
                }

                outputWriter.WriteLine(boneStringBuilder);
            }

            // Write Meshes
            outputWriter.WriteLine("meshes:" + model.MeshBindings.Count);
            for (int mi = 0; mi < grannyMeshInfos.Count; mi++)
            {

                GrannyMeshInfo grannyMeshInfo = grannyMeshInfos[mi];
                string meshName = model.MeshBindings[mi].Name;

                meshNameCount[meshName]++;
                if (meshNameCount[meshName] > 1)
                {
                    meshName += meshNameCount[meshName];
                }

                outputWriter.WriteLine("mesh:\"" + meshName + "\"");

                // Write Materials
                outputWriter.WriteLine("materials");
                foreach (IGrannyMaterial material in model.MeshBindings[mi].MaterialBindings)
                {
                    outputWriter.WriteLine("\"" + material.Name + "\"");
                }

                // Write Vertices
                outputWriter.WriteLine("vertices");
                for (int vi = 0; vi < grannyMeshInfo.vertices.Count; vi++)
                {
                    GrannyVertexInfo vertex = grannyMeshInfo.vertices[vi];

                    string[] boneNames = new string[8];
                    float[] boneWeights = new float[8];
                    int[] boneIds = new int[8];

                    for (int z = 0; z < 8; z++)
                    {
                        boneNames[z] = grannyMeshInfo.boneBindings[vertex.boneIndices[z]];
                        boneWeights[z] = (float)vertex.boneWeights[z] / 255;
                        boneIds[z] = NB2NA2FileOps.getBoneIdForBoneName(boneLookup, boneNameToPositionMap, boneNames[z], boneWeights[z], vertex.position);
                    }

                    float[] tangents = new float[3];
                    if (vertex.tangent == null)
                    {
                        tangents[0] = vertex.normal[0];
                        tangents[1] = vertex.normal[1];
                        tangents[2] = vertex.normal[2];
                    }
                    else
                    {
                        tangents[0] = vertex.tangent[0];
                        tangents[1] = vertex.tangent[1];
                        tangents[2] = vertex.tangent[2];
                    }

                    float[] binormals = new float[3];
                    if (vertex.binormal == null)
                    {
                        binormals[0] = vertex.normal[0];
                        binormals[1] = vertex.normal[1];
                        binormals[2] = vertex.normal[2];
                    }
                    else
                    {
                        binormals[0] = vertex.binormal[0];
                        binormals[1] = vertex.binormal[1];
                        binormals[2] = vertex.binormal[2];
                    }

                    outputWriter.WriteLine(vertex.position[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.position[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.position[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                           vertex.normal[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.normal[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.normal[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                           tangents[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + tangents[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + tangents[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                           binormals[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + binormals[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + binormals[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                           vertex.uv[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.uv[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                           vertex.uv2[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.uv2[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                           vertex.uv3[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.uv3[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                           boneIds[0] + " " + boneIds[1] + " " + boneIds[2] + " " + boneIds[3] + " " + boneIds[4] + " " + boneIds[5] + " " + boneIds[6] + " " + boneIds[7] + " " +
                                           vertex.boneWeights[0] + " " + vertex.boneWeights[1] + " " + vertex.boneWeights[2] + " " + vertex.boneWeights[3] + " " + vertex.boneWeights[4] + " " + vertex.boneWeights[5] + " " + vertex.boneWeights[6] + " " + vertex.boneWeights[7] + " "
                                           );
                }

                // Write Triangles
                outputWriter.WriteLine("triangles");
                for (int ti = 0; ti < grannyMeshInfo.triangles.Count; ti++)
                {
                    int[] triangle = grannyMeshInfo.triangles[ti];
                    outputWriter.WriteLine(triangle[0] + " " + triangle[1] + " " + triangle[2] + " " + triangle[3]);
                }
            }
            outputWriter.WriteLine("end");

            outputWriter.Close();
        }

    }
}
