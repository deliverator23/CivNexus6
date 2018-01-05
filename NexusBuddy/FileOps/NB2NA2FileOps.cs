using System;
using System.Collections.Generic;
using System.IO;
using Firaxis.Granny;
using NexusBuddy.GrannyInfos;
using NexusBuddy.GrannyWrappers;
using NexusBuddy.Utils;
using System.Globalization;
using System.Windows.Media.Media3D;
using System.Runtime.ExceptionServices;

namespace NexusBuddy.FileOps
{
    class NB2NA2FileOps
    {

        public static void exportAllModelsToNB2(IGrannyFile grannyFile)
        {
            for (int i = 0; i < grannyFile.Models.Count; i++)
            {
                exportNB2Model(grannyFile, i);
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public static Int32 exportNA2(IGrannyModel model, IGrannyAnimation anim, string filename, Single timeWindowStart, Single timeWindowEnd, Int32 fpsInput, int retryLimit)
        {
            string numberFormat = "f6";

            if (timeWindowStart == -1)
            {
                timeWindowStart = 0;
            }
            if (timeWindowEnd < 0 || timeWindowEnd > anim.Duration)
            {
                timeWindowEnd = anim.Duration;
            }

            int startFrame = -1;
            int endFrame = -1;
            int frameCount = 0;
            int fps;
            if (fpsInput > 0)
            {
                fps = fpsInput;
            }
            else
            {
                fps = (int)Math.Round(1.0f / anim.TimeStep, 0);
                
            }
            Single timestep = 1f / fps;
            Single time = timeWindowStart;
            while (time <= timeWindowEnd)
            {
                if (time >= timeWindowStart && time <= timeWindowEnd)
                {
                    if (startFrame == -1)
                    {
                        startFrame = (int)Math.Round(time / timestep, 0) + 1;
                    }
                    endFrame = (int)Math.Round(time / timestep, 0) + 1;
                    frameCount++;

                }
                time = time + timestep;
            }

            List<IGrannyBone> boneList = new List<IGrannyBone>();
            foreach (IGrannyBone bone in model.Skeleton.Bones)
            {
                boneList.Add(bone);
            }

            int retryCount = 0;
            Boolean success = false;

            List<List<float[]>> boneFrameLists = new List<List<float[]>>();
            
            while (success == false && retryCount < retryLimit) {

                int exceptionCount = 0;
                boneFrameLists = new List<List<float[]>>();
                foreach (IGrannyBone bone in boneList)
                {
                    time = timeWindowStart;
                    List<float[]> kf = new List<float[]>();
                    while (time <= timeWindowEnd)
                    {
                        if (time >= timeWindowStart && time <= timeWindowEnd)
                        {
                            try
                            {
                                float[] skf = anim.SampleBone(model, bone.Name, time);
                                kf.Add(skf);
                            }
                            catch (Exception) {
                                exceptionCount++;
                            }
                        
                        }
                        time = time + timestep;
                    }
                    boneFrameLists.Add(kf);
                }

                if (exceptionCount > 0)
                {
                    retryCount++;
                }
                else
                {
                    success = true;
                }
            }

            string fileExtension = "__" + timeWindowStart.ToString("f3") + "-" + timeWindowEnd.ToString("f3") + ".na2";

            string outputFilename = filename.Replace(".fgx", fileExtension);
            outputFilename = outputFilename.Replace(".FGX", fileExtension);

            StreamWriter outputWriter = new StreamWriter(new FileStream(outputFilename, FileMode.Create));

            outputWriter.WriteLine("// Nexus Buddy Animation NA2 - Exported from Nexus Buddy 2");

            outputWriter.WriteLine("FrameSets: 1");
            outputWriter.WriteLine("FrameCount: " + frameCount);
            outputWriter.WriteLine("FirstFrame: " + startFrame);
            outputWriter.WriteLine("LastFrame: " + endFrame);
            outputWriter.WriteLine("FPS: " + fps);
            outputWriter.WriteLine("Bones: " + model.Skeleton.Bones.Count);

            for (int bi = 0; bi < model.Skeleton.Bones.Count; bi++)
            {
                IGrannyBone bone = model.Skeleton.Bones[bi];
                outputWriter.WriteLine(bone.Name);
                foreach (float[] kff in boneFrameLists[bi])
                {
                    for (int j = 0; j < 16; j++)
                    {
                        outputWriter.Write(kff[j].ToString(numberFormat, CultureInfo.InvariantCulture) + " ");
                    }
                    outputWriter.Write("\n");
                }
            }

            outputWriter.Close();

            return retryCount;
        }

        public static void exportNB2(IGrannyFile grannyFile, Int32 currentModelIndex)
        {
            exportNB2Model(grannyFile, currentModelIndex);
        }

        public static void exportNB2Model(IGrannyFile grannyFile, int modelId)
        {
            string fileExtension = ".nb2";
            string outputFilename = "";
            string numberFormat = "f6";

            if (grannyFile.Models.Count > 1)
            {
                outputFilename = grannyFile.Filename.Replace(".fgx", "_model" + modelId + fileExtension);
                outputFilename = outputFilename.Replace(".FGX", "_model" + modelId + fileExtension);
            }
            else
            {
                outputFilename = grannyFile.Filename.Replace(".fgx", fileExtension);
                outputFilename = outputFilename.Replace(".FGX", fileExtension);
            }

            StreamWriter outputWriter = new StreamWriter(new FileStream(outputFilename, FileMode.Create));

            IGrannyModel model = grannyFile.Models[modelId];
            IGrannySkeleton skeleton = model.Skeleton;
            GrannySkeletonWrapper skeletonWrapper = new GrannySkeletonWrapper(skeleton);
            skeletonWrapper.readSkeletonInfo();

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
                double[] bonePosition = getBoneWorldPosition(bone);
                boneNameToPositionMap.Add(bone.Name, bonePosition);
            }

            outputWriter.WriteLine("// Nexus Buddy NB2 - Exported from Nexus Buddy 2");
            outputWriter.WriteLine("Frames: 30");
            outputWriter.WriteLine("Frame: 1");

            // Write Meshes
            outputWriter.WriteLine("Meshes: " + model.MeshBindings.Count);
            for (int mi = 0; mi < grannyMeshInfos.Count; mi++)
            {

                GrannyMeshInfo grannyMeshInfo = grannyMeshInfos[mi];
                string meshName = model.MeshBindings[mi].Name;

                meshNameCount[meshName]++;
                if (meshNameCount[meshName] > 1)
                {
                    meshName += meshNameCount[meshName];
                }
                outputWriter.WriteLine("\"" + meshName + "\" 0 0");

                // Write Vertices
                outputWriter.WriteLine(grannyMeshInfo.vertices.Count);
                for (int vi = 0; vi < grannyMeshInfo.vertices.Count; vi++)
                {
                    GrannyVertexInfo vertex = grannyMeshInfo.vertices[vi];

                    string boneName0 = grannyMeshInfo.boneBindings[vertex.boneIndices[0]];
                    float boneWeight0 = (float)vertex.boneWeights[0] / 255;
                    int boneId0 = getBoneIdForBoneName(boneLookup, boneNameToPositionMap, boneName0, boneWeight0, vertex.position);       
                    
                    string boneName1 = grannyMeshInfo.boneBindings[vertex.boneIndices[1]];
                    float boneWeight1 = (float)vertex.boneWeights[1] / 255;
                    int boneId1 = getBoneIdForBoneName(boneLookup, boneNameToPositionMap, boneName1, boneWeight1, vertex.position);  
                    
                    string boneName2 = grannyMeshInfo.boneBindings[vertex.boneIndices[2]];
                    float boneWeight2 = (float)vertex.boneWeights[2] / 255;
                    int boneId2 = getBoneIdForBoneName(boneLookup, boneNameToPositionMap, boneName2, boneWeight2, vertex.position);  
                    
                    string boneName3 = grannyMeshInfo.boneBindings[vertex.boneIndices[3]];
                    float boneWeight3 = (float)vertex.boneWeights[3] / 255;
                    int boneId3 = getBoneIdForBoneName(boneLookup, boneNameToPositionMap, boneName3, boneWeight3, vertex.position);  


                    outputWriter.WriteLine("0 " + vertex.position[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.position[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.position[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                           vertex.uv[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.uv[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " "
                                           + boneId0 + " " + boneWeight0.ToString(numberFormat, CultureInfo.InvariantCulture) + " " + boneId1 + " " + boneWeight1.ToString(numberFormat, CultureInfo.InvariantCulture) + " "
                                           + boneId2 + " " + boneWeight2.ToString(numberFormat, CultureInfo.InvariantCulture) + " " + boneId3 + " " + boneWeight3.ToString(numberFormat, CultureInfo.InvariantCulture)
                                           );
                }

                // Write Normals
                outputWriter.WriteLine(grannyMeshInfo.vertices.Count);
                for (int ni = 0; ni < grannyMeshInfo.vertices.Count; ni++)
                {
                    GrannyVertexInfo vertex = grannyMeshInfo.vertices[ni];

                    outputWriter.WriteLine(vertex.normal[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.normal[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + vertex.normal[2].ToString(numberFormat, CultureInfo.InvariantCulture));
                }

                // Write Triangles
                outputWriter.WriteLine(grannyMeshInfo.triangles.Count);
                for (int ti = 0; ti < grannyMeshInfo.triangles.Count; ti++)
                {
                    int[] triangle = grannyMeshInfo.triangles[ti];
                    outputWriter.WriteLine("0 " + triangle[0] + " " + triangle[1] + " " + triangle[2] + " " + triangle[0] + " " + triangle[1] + " " + triangle[2] + " 1");
                }
            }

            // Write Material
            outputWriter.WriteLine("Materials: 1");
            outputWriter.WriteLine("\"Material_0\"");
            outputWriter.WriteLine("0.200000 0.200000 0.200000 1.000000");
            outputWriter.WriteLine("0.800000 0.800000 0.800000 1.000000");
            outputWriter.WriteLine("0.000000 0.000000 0.000000 1.000000");
            outputWriter.WriteLine("0.000000 0.000000 0.000000 1.000000");
            outputWriter.WriteLine("0.000000");
            outputWriter.WriteLine("1.000000");
            outputWriter.WriteLine("\"Material_0\"");
            outputWriter.WriteLine("\"\"");

            // Write Bones
            outputWriter.WriteLine("Bones: " + skeleton.Bones.Count);
            for (int bi = 0; bi < skeleton.Bones.Count; bi++)
            {
                IGrannyBone bone = skeleton.Bones[bi];
                string boneName = bone.Name;
                outputWriter.WriteLine("\"" + boneName + "\"");

                if (bone.ParentIndex == -1)
                {
                    outputWriter.WriteLine("\"\"");
                }
                else
                {
                    string parentBoneName = skeleton.Bones[bone.ParentIndex].Name;
                    outputWriter.WriteLine("\"" + parentBoneName + "\"");
                }

                IGrannyTransform transform = bone.LocalTransform;
                float[] orientation = transform.Orientation;
                float[] position = transform.Position;

                outputWriter.WriteLine("0 " + position[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + position[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + position[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " +
                                                orientation[0].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + orientation[1].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + orientation[2].ToString(numberFormat, CultureInfo.InvariantCulture) + " " + orientation[3].ToString(numberFormat, CultureInfo.InvariantCulture));

                // number of position keys
                outputWriter.WriteLine("0");
                // number of rotation keys
                outputWriter.WriteLine("0");
            }

            outputWriter.Close();
        }

        private static Matrix3D getBoneWorldMatrix(IGrannyBone bone)
        {
            float[] iwt = bone.InverseWorldTransform;

            Matrix3D inverseWorldMatrix = new Matrix3D((double)iwt[0], (double)iwt[1], (double)iwt[2], (double)iwt[3],
                                                        (double)iwt[4], (double)iwt[5], (double)iwt[6], (double)iwt[7],
                                                        (double)iwt[8], (double)iwt[9], (double)iwt[10], (double)iwt[11],
                                                        (double)iwt[12], (double)iwt[13], (double)iwt[14], (double)iwt[15]);

            inverseWorldMatrix.Invert();

            return inverseWorldMatrix;
        }

        public static double[] getBoneWorldPosition(IGrannyBone bone)
        {
            Matrix3D worldMatrix = getBoneWorldMatrix(bone);

            double[] bonePosition = new double[3];
            bonePosition[0] = worldMatrix.OffsetX;
            bonePosition[1] = worldMatrix.OffsetY;
            bonePosition[2] = worldMatrix.OffsetZ;

            //MemoryUtil.memLogLine("IWT Inv boneId:" + bi + " x:" + inverseWorldMatrix.OffsetX + " y: " + inverseWorldMatrix.OffsetY + " z: " + inverseWorldMatrix.OffsetZ);
            return bonePosition;
        }

        public static int getBoneIdForBoneName(BiLookup<int, string> boneLookup, Dictionary<string, double[]> boneNameToPositionMap, string boneName, float boneWeight, float[] vertexPosition)
        {
            int boneId;
            if (boneName.Equals(""))
            {
                if (boneWeight == 0)
                {
                    boneId = 0;
                }
                else
                {
                    double minDistance = Double.MaxValue;
                    string closestBoneName = null;

                    foreach (string keyBoneName in boneNameToPositionMap.Keys)
                    {
                        double[] bonePosition = boneNameToPositionMap[keyBoneName];
                        double deltaX = vertexPosition[0] - bonePosition[0];
                        double deltaY = vertexPosition[1] - bonePosition[1];
                        double deltaZ = vertexPosition[2] - bonePosition[2];

                        double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestBoneName = keyBoneName;
                        }
                    }
                    if (!System.String.IsNullOrEmpty(closestBoneName))
                    {
                        boneId = boneLookup[closestBoneName][0];
                    }
                    else
                    {
                        boneId = 0;
                    }
                }
            }
            else
            {
                boneId = boneLookup[boneName][0];
            }
            return boneId;
        }

        //public static GrannyMeshInfo loadMeshInfo_gr2conv_exe(IGrannyFile grannyFile, int meshIndex)
        //{

        //    if (grannyFile != null)
        //    {
        //        string tempPath = Path.GetTempPath();
        //        string outputFilename = tempPath + "tempmesh_";
        //        outputFilename += grannyFile.Meshes[meshIndex].Name;
        //        outputFilename += "_";
        //        outputFilename += random.Next(100000).ToString();
        //        outputFilename += random.Next(100000).ToString();
        //        outputFilename += random.Next(10000).ToString();
        //        outputFilename += ".msh";

        //        Process process = new Process();

        //        ProcessStartInfo startInfo = new ProcessStartInfo();
        //        startInfo.CreateNoWindow = true;
        //        startInfo.UseShellExecute = true;
        //        startInfo.FileName = "gr2conv.exe";
        //        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //        string args = grannyFile.Filename + " " + outputFilename + " " + meshIndex;
        //        startInfo.Arguments = args;

        //        process.StartInfo = startInfo;
        //        process.Start();
        //        process.WaitForExit();

        //        GrannyMeshInfo meshInfo = loadMeshInfoFromFile(outputFilename, true);

        //        return meshInfo;
        //    }

        //    return null;
        //}

        //public static GrannyMeshInfo loadMeshInfoFromFile(string filename, bool deleteFile)
        //{
        //    string currentLine = "";
        //    GrannyMeshInfo meshInfo = new GrannyMeshInfo();

        //    // Read Bone Bindings
        //    List<string> bb = new List<string>();
        //    StreamReader streamReader = new StreamReader(filename);
        //    while (!currentLine.Equals("Vertices"))
        //    {
        //        currentLine = streamReader.ReadLine();
        //        if (!currentLine.Equals("Vertices") && !currentLine.Equals("BoneBindings"))
        //        {
        //            string boneName = currentLine.Substring(currentLine.IndexOf(":") + 1, currentLine.Length - currentLine.IndexOf(":") - 1);
        //            bb.Add(boneName.Trim());
        //        }
        //    }
        //    meshInfo.boneBindings = bb;

        //    // Read Vertex Data
        //    List<GrannyVertexInfo> vertices = new List<GrannyVertexInfo>();
        //    int lastIndex = 0;
        //    GrannyVertexInfo currentVertex = new GrannyVertexInfo();
        //    while (!currentLine.StartsWith("Material"))
        //    {
        //        //e.g.
        //        //0:Position: -20.334412 5.545011 88.681549
        //        //0:BoneWeights: 255 0 0 0
        //        //0:BoneIndices: 14 14 14 14
        //        //0:Normal: -0.166870 0.948242 0.270752
        //        //0:TextureCoordinates: 0.367676 0.697266    
        //        currentLine = streamReader.ReadLine();
        //        if (!currentLine.StartsWith("Material"))
        //        {
        //            string id = "0", type = "", number1 = null, number2 = null, number3 = null, number4 = null;

        //            Regex regex = new Regex("([0-9]+):(.+): ([-+]?[0-9]+\\.?[0-9]*) ([-+]?[0-9]+\\.?[0-9]*) ([-+]?[0-9]+\\.?[0-9]*) ([-+]?[0-9]+\\.?[0-9]*)");
        //            MatchCollection mc = regex.Matches(currentLine);
        //            foreach (Match m in mc)
        //            {
        //                id = m.Groups[1].Value.Trim();
        //                type = m.Groups[2].Value.Trim();
        //                number1 = m.Groups[3].Value.Trim();
        //                number2 = m.Groups[4].Value.Trim();
        //                number3 = m.Groups[5].Value.Trim();
        //                number4 = m.Groups[6].Value.Trim();
        //                break;
        //            }
        //            regex = new Regex("([0-9]+):(.+): ([-+]?[0-9]+\\.?[0-9]*) ([-+]?[0-9]+\\.?[0-9]*) ([-+]?[0-9]+\\.?[0-9]*)");
        //            mc = regex.Matches(currentLine);
        //            foreach (Match m in mc)
        //            {
        //                id = m.Groups[1].Value.Trim();
        //                type = m.Groups[2].Value.Trim();
        //                number1 = m.Groups[3].Value.Trim();
        //                number2 = m.Groups[4].Value.Trim();
        //                number3 = m.Groups[5].Value.Trim();
        //                break;
        //            }
        //            regex = new Regex("([0-9]+):(.+): ([-+]?[0-9]+\\.?[0-9]*) ([-+]?[0-9]+\\.?[0-9]*)");
        //            mc = regex.Matches(currentLine);
        //            foreach (Match m in mc)
        //            {
        //                id = m.Groups[1].Value.Trim();
        //                type = m.Groups[2].Value.Trim();
        //                number1 = m.Groups[3].Value.Trim();
        //                number2 = m.Groups[4].Value.Trim();
        //                break;
        //            }

        //            int index = int.Parse(id, CultureInfo.InvariantCulture);

        //            if (index != lastIndex)
        //            {
        //                vertices.Add(currentVertex);
        //                currentVertex = new GrannyVertexInfo();
        //            }

        //            if (type.Equals("Position") && number1 != null && number2 != null && number3 != null)
        //            {
        //                currentVertex.position = new float[3];
        //                currentVertex.position[0] = float.Parse(number1, CultureInfo.InvariantCulture);
        //                currentVertex.position[1] = float.Parse(number2, CultureInfo.InvariantCulture);
        //                currentVertex.position[2] = float.Parse(number3, CultureInfo.InvariantCulture);
        //            }
        //            if (type.Equals("Normal") && number1 != null && number2 != null && number3 != null)
        //            {
        //                currentVertex.normal = new float[3];
        //                currentVertex.normal[0] = float.Parse(number1, CultureInfo.InvariantCulture);
        //                currentVertex.normal[1] = float.Parse(number2, CultureInfo.InvariantCulture);
        //                currentVertex.normal[2] = float.Parse(number3, CultureInfo.InvariantCulture);
        //            }
        //            if (type.Equals("BoneWeights") && number1 != null && number2 != null && number3 != null && number4 != null)
        //            {
        //                currentVertex.boneWeights = new int[4];
        //                currentVertex.boneWeights[0] = int.Parse(number1, CultureInfo.InvariantCulture);
        //                currentVertex.boneWeights[1] = int.Parse(number2, CultureInfo.InvariantCulture);
        //                currentVertex.boneWeights[2] = int.Parse(number3, CultureInfo.InvariantCulture);
        //                currentVertex.boneWeights[3] = int.Parse(number4, CultureInfo.InvariantCulture);
        //            }
        //            if (type.Equals("BoneIndices") && number1 != null && number2 != null && number3 != null && number4 != null)
        //            {
        //                currentVertex.boneIndices = new int[4];
        //                currentVertex.boneIndices[0] = int.Parse(number1, CultureInfo.InvariantCulture);
        //                currentVertex.boneIndices[1] = int.Parse(number2, CultureInfo.InvariantCulture);
        //                currentVertex.boneIndices[2] = int.Parse(number3, CultureInfo.InvariantCulture);
        //                currentVertex.boneIndices[3] = int.Parse(number4, CultureInfo.InvariantCulture);
        //            }
        //            if (type.Equals("TextureCoordinates") && number1 != null && number2 != null)
        //            {
        //                currentVertex.uv = new float[2];
        //                currentVertex.uv[0] = float.Parse(number1, CultureInfo.InvariantCulture);
        //                currentVertex.uv[1] = float.Parse(number2, CultureInfo.InvariantCulture);
        //            }

        //            lastIndex = index;
        //        }
        //    }
        //    vertices.Add(currentVertex);
        //    meshInfo.vertices = vertices;
        //    string materialName = currentLine.Substring(currentLine.IndexOf(":") + 1, currentLine.Length - currentLine.IndexOf(":") - 1).Trim();
        //    meshInfo.materialName = materialName;

        //    // Read Triangles
        //    List<int[]> triangles = new List<int[]>();
        //    while (!currentLine.Equals("end"))
        //    {
        //        currentLine = streamReader.ReadLine();
        //        if (!currentLine.Equals("end"))
        //        {
        //            Regex regex = new Regex("([0-9]+) ([0-9]+) ([0-9]+)");
        //            MatchCollection mc = regex.Matches(currentLine);
        //            foreach (Match m in mc)
        //            {
        //                int[] triangle = new int[3];
        //                triangle[0] = int.Parse(m.Groups[1].Value.Trim(), CultureInfo.InvariantCulture);
        //                triangle[1] = int.Parse(m.Groups[2].Value.Trim(), CultureInfo.InvariantCulture);
        //                triangle[2] = int.Parse(m.Groups[3].Value.Trim(), CultureInfo.InvariantCulture);
        //                triangles.Add(triangle);
        //                break;
        //            }
        //        }
        //    }
        //    meshInfo.triangles = triangles;
        //    streamReader.Close();

        //    if (deleteFile)
        //    {
        //        try
        //        {
        //            File.Delete(filename);
        //        }
        //        catch (Exception) { }
        //    }

        //    return meshInfo;
        //}
    }
}