using Firaxis.Granny;
using System.IO;
using NexusBuddy.GrannyWrappers;
using System.Globalization;
using System.Collections.Generic;

namespace NexusBuddy.FileOps
{
    class MetadataWriter
    {
        public static void WriteGeoAnimFile(IGrannyFile file, int currentModelIndex, string className)
        {
            string fgxFilename = Path.GetFileName(file.Filename);
            string filenameNoExt = Path.GetFileNameWithoutExtension(file.Filename);

            string directory = Path.GetDirectoryName(file.Filename);

            string numberFormat = "f6";

            bool isAnimation = false;
            string instanceName = "Geometry";
            string fileExtension = ".geo";

            if (file.Animations.Count > 0 && file.Models.Count == 0)
            {
                isAnimation = true;
                instanceName = "Animation";
                fileExtension = ".anm";
            }

            string geoFilename = filenameNoExt + fileExtension;

            string wigFilename = "";
            if (className.Equals("Leader"))
            {
                wigFilename = filenameNoExt + ".wig";
                string wigFullPath = directory + "\\" + wigFilename;
                File.Copy(CivNexusSixApplicationForm.form.dummyWigFilename, wigFullPath, true);
            }

            StreamWriter outputWriter = new StreamWriter(new FileStream(directory + "\\" + geoFilename, FileMode.Create));

            WriteAssetHeader(instanceName, outputWriter);

            WriteCookParamsAndVersion(outputWriter);

            if (isAnimation)
            {
                outputWriter.WriteLine("<m_fDuration>" + file.Animations[0].Duration.ToString(numberFormat, CultureInfo.InvariantCulture) + "</m_fDuration>");
            }
            else
            {
                outputWriter.WriteLine("<m_Meshes>");
                foreach (IGrannyMesh mesh in file.Meshes)
                {
                    GrannyMeshWrapper meshWrapper = new GrannyMeshWrapper(mesh);

                    outputWriter.WriteLine("<Element>");
                    outputWriter.WriteLine("<m_Name text=\"" + mesh.Name + "\"/>");

                    outputWriter.WriteLine("<m_Groups>");

                    int totalTriangles = 0;
                    foreach (IGrannyTriMaterialGroup triMaterialGroup in mesh.TriangleMaterialGroups)
                    {
                        outputWriter.WriteLine("<Element>");
                        outputWriter.WriteLine("<m_Name text=\"" + mesh.MaterialBindings[triMaterialGroup.MaterialIndex].Name + "\"/>");
                        outputWriter.WriteLine("<m_nFirstPrim>" + triMaterialGroup.TriFirst + "</m_nFirstPrim>");
                        outputWriter.WriteLine("<m_nPrims>" + triMaterialGroup.TriCount + "</m_nPrims>");
                        outputWriter.WriteLine("</Element>");

                        totalTriangles += triMaterialGroup.TriCount;
                    }
                    outputWriter.WriteLine("</m_Groups>");

                    outputWriter.WriteLine("<m_nBoundBoneCount>" + mesh.BoneBindings.Count + "</m_nBoundBoneCount>");
                    outputWriter.WriteLine("<m_nPrimitiveCount>" + totalTriangles + "</m_nPrimitiveCount>");
                    outputWriter.WriteLine("<m_nVertexCount>" + mesh.VertexCount + "</m_nVertexCount>");

                    outputWriter.WriteLine("</Element>");
                }
                outputWriter.WriteLine("</m_Meshes>");

                outputWriter.WriteLine("<m_Bones>");
                foreach (IGrannyBone bone in file.Models[currentModelIndex].Skeleton.Bones)
                {
                    outputWriter.WriteLine("<Element text=\"" + bone.Name + "\"/>");
                }
                outputWriter.WriteLine("</m_Bones>");
                outputWriter.WriteLine("<m_ModelName text=\"" + file.Models[currentModelIndex].Name + "\"/>");
            }

            WriteSourcePathAndTimes(outputWriter);

            WriteClassName(className, outputWriter);

            outputWriter.WriteLine("<m_DataFiles>");
            outputWriter.WriteLine("<Element>");
            outputWriter.WriteLine("<m_ID text=\"GR2\"/>");
            outputWriter.WriteLine("<m_RelativePath text=\"" + fgxFilename + "\"/>");
            outputWriter.WriteLine("</Element>");

            if (className.Equals("Leader"))
            {
                outputWriter.WriteLine("<Element>");
                outputWriter.WriteLine("<m_ID text=\"WIG\"/>");
                outputWriter.WriteLine("<m_RelativePath text=\"" + wigFilename + "\"/>");
                outputWriter.WriteLine("</Element>");
            }

            outputWriter.WriteLine("</m_DataFiles>");

            WriteFooter(className, filenameNoExt, instanceName, outputWriter);

            outputWriter.Close();
        }

        public static void WriteTextureFile(string outputDirectory, string filenameNoExt, Dictionary<string, string> imageMetadataDict, string className)
        {
            string numberFormat = "f6";

            string instanceName = "Texture";
            string fileExtension = ".tex";
            string texFilename = filenameNoExt + fileExtension;

            StreamWriter outputWriter = new StreamWriter(new FileStream(outputDirectory + "\\" + texFilename, FileMode.Create));

            WriteAssetHeader("Texture", outputWriter);

            outputWriter.WriteLine("<m_ExportSettings>");
            outputWriter.WriteLine("<ePixelformat>PF_" + imageMetadataDict["format"] + "</ePixelformat>");
            outputWriter.WriteLine("<eFilterType>FT_GAUSSIAN</eFilterType>");
            outputWriter.WriteLine("<bUseMips>false</bUseMips>");
            outputWriter.WriteLine("<iNumManualMips>0</iNumManualMips>");
            outputWriter.WriteLine("<bCompleteMipChain>false</bCompleteMipChain>");
            outputWriter.WriteLine("<fValueClampMin>0.000000</fValueClampMin>");
            outputWriter.WriteLine("<fValueClampMax>1.000000</fValueClampMax>");
            outputWriter.WriteLine("<fSupportScale>1.000000</fSupportScale>");
            outputWriter.WriteLine("<fGammaIn>1.000000</fGammaIn>");
            outputWriter.WriteLine("<fGammaOut>1.000000</fGammaOut>");
            outputWriter.WriteLine("<iSlabWidth>0</iSlabWidth>");
            outputWriter.WriteLine("<iSlabHeight>0</iSlabHeight>");
            outputWriter.WriteLine("<iColorKeyX>64</iColorKeyX>");
            outputWriter.WriteLine("<iColorKeyY>64</iColorKeyY>");
            outputWriter.WriteLine("<iColorKeyZ>64</iColorKeyZ>");
            outputWriter.WriteLine("<eExportMode>TEXTURE_2D</eExportMode>");
            outputWriter.WriteLine("<bSampleFromTopLayer>false</bSampleFromTopLayer>");
            outputWriter.WriteLine("</m_ExportSettings>");

            WriteCookParamsAndVersion(outputWriter);

            outputWriter.WriteLine("<m_Height>" + imageMetadataDict["height"] + "</m_Height>");
            outputWriter.WriteLine("<m_Width>" + imageMetadataDict["width"] + "</m_Width>");
            outputWriter.WriteLine("<m_Depth>" + imageMetadataDict["depth"] + "</m_Depth>");
            outputWriter.WriteLine("<m_NumMipMaps>" + imageMetadataDict["mipLevels"] + "</m_NumMipMaps>");

            WriteSourcePathAndTimes(outputWriter);

            WriteClassName(className, outputWriter);

            outputWriter.WriteLine("<m_DataFiles>");
            outputWriter.WriteLine("<Element>");
            outputWriter.WriteLine("<m_ID text=\"DDS\"/>");
            outputWriter.WriteLine("<m_RelativePath text=\"" + filenameNoExt + ".dds\"/>");
            outputWriter.WriteLine("</Element>");
            outputWriter.WriteLine("</m_DataFiles>");

            WriteFooter(className, filenameNoExt, instanceName, outputWriter);

            outputWriter.Close();
        }

        private static void WriteSourcePathAndTimes(StreamWriter outputWriter)
        {
            outputWriter.WriteLine("<m_SourceFilePath text=\"\"/>");
            outputWriter.WriteLine("<m_SourceObjectName text=\"\"/>");
            outputWriter.WriteLine("<m_ImportedTime>0</m_ImportedTime>");
            outputWriter.WriteLine("<m_ExportedTime>0</m_ExportedTime>");
        }

        private static void WriteFooter(string className, string assetName, string instanceName, StreamWriter outputWriter)
        {
            outputWriter.WriteLine("<m_Name text=\"" + assetName + "\"/>");
            outputWriter.WriteLine("<m_Description text=\"" + assetName + "\"/>");

            outputWriter.WriteLine("<m_Tags>");
            outputWriter.WriteLine("<Element text=\"" + className + "\"/>");
            outputWriter.WriteLine("</m_Tags>");

            outputWriter.WriteLine("<m_Groups/>");
            outputWriter.WriteLine("</AssetObjects:" + instanceName + "Instance>");
        }

        private static void WriteClassName(string className, StreamWriter outputWriter)
        {
            outputWriter.WriteLine("<m_ClassName text=\"" + className + "\"/>");
        }

        private static void WriteCookParamsAndVersion(StreamWriter outputWriter)
        {
            outputWriter.WriteLine("<m_CookParams>");
            outputWriter.WriteLine("<m_Values/>");
            outputWriter.WriteLine("</m_CookParams>");

            outputWriter.WriteLine("<m_Version>");
            outputWriter.WriteLine("<major>0</major>");
            outputWriter.WriteLine("<minor>0</minor>");
            outputWriter.WriteLine("<build>0</build>");
            outputWriter.WriteLine("<revision>0</revision>");
            outputWriter.WriteLine("</m_Version>");
        }

        private static void WriteAssetHeader(string instanceName, StreamWriter outputWriter)
        {
            outputWriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            outputWriter.WriteLine("<AssetObjects:" + instanceName + "Instance>");
        }

        public static void WriteAssetFile(IGrannyFile file, Dictionary<string, string> civ6ShortNameToLongNameLookup, string className)
        {
            string filenameNoExt = Path.GetFileNameWithoutExtension(file.Filename);
            string directory = Path.GetDirectoryName(file.Filename);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            string geoFilename = textInfo.ToTitleCase(filenameNoExt) + ".ast";
            string instanceName = "Asset";

            StreamWriter outputWriter = new StreamWriter(new FileStream(directory + "\\" + geoFilename, FileMode.Create));
            WriteAssetHeader(instanceName, outputWriter);
            WriteBehaviorMetadataToStream(civ6ShortNameToLongNameLookup, outputWriter);
            WriteGeometrySetMetadataToStream(file, outputWriter);
            WriteCookParamsAndVersion(outputWriter);

            outputWriter.WriteLine("<m_ParticleEffects/>");
            outputWriter.WriteLine("<m_Geometries/>");
            outputWriter.WriteLine("<m_Animations/>");
            outputWriter.WriteLine("<m_Materials/>");

            WriteClassName(className, outputWriter);

            outputWriter.WriteLine("<m_DataFiles/>");

            WriteFooter(className, textInfo.ToTitleCase(filenameNoExt), instanceName, outputWriter);

            outputWriter.Close();
        }

        public static void WriteGeometrySet(IGrannyFile file)
        {
            string filenameNoExt = Path.GetFileNameWithoutExtension(file.Filename);
            string directory = Path.GetDirectoryName(file.Filename);
            string fileExtension = "_GeometrySet.xml";
            string geoFilename = filenameNoExt + fileExtension;

            StreamWriter outputWriter = new StreamWriter(new FileStream(directory + "\\" + geoFilename, FileMode.Create));
            WriteGeometrySetMetadataToStream(file, outputWriter);
            outputWriter.Close();
        }

        public static void WriteBehaviorMetadata(string outputDirectory, Dictionary<string, string> civ6ShortNameToLongNameLookup)
        {
            StreamWriter outputWriter = new StreamWriter(new FileStream(outputDirectory + "\\m_BehaviorData.xml", FileMode.Create));
            WriteBehaviorMetadataToStream(civ6ShortNameToLongNameLookup, outputWriter);
            outputWriter.Close();
        }

        private static void WriteGeometrySetMetadataToStream(IGrannyFile file, StreamWriter outputWriter)
        {
            string filenameNoExt = Path.GetFileNameWithoutExtension(file.Filename);

            outputWriter.WriteLine("<m_GeometrySet>");
            outputWriter.WriteLine("<m_ModelInstances>");

            foreach (IGrannyModel model in file.Models)
            {
                outputWriter.WriteLine("<Element>");
                outputWriter.WriteLine("<m_Name text=\"" + model.Name + "\"/>");
                outputWriter.WriteLine("<m_GeoName text=\"" + filenameNoExt + "\"/>");

                outputWriter.WriteLine("<m_GroupStates>");

                foreach (IGrannyMesh mesh in model.MeshBindings)
                {
                    foreach (IGrannyTriMaterialGroup triMaterialGroup in mesh.TriangleMaterialGroups)
                    {
                        outputWriter.WriteLine("<Element>");

                        outputWriter.WriteLine("<m_Values>");
                        outputWriter.WriteLine("<m_Values>");

                        outputWriter.WriteLine("<Element class=\"AssetObjects:ObjectValue\">");
                        outputWriter.WriteLine("<m_ObjectName text=\"" + mesh.MaterialBindings[triMaterialGroup.MaterialIndex].Name + "\"/>");
                        outputWriter.WriteLine("<m_eObjectType>MATERIAL</m_eObjectType>");
                        outputWriter.WriteLine("<m_ParamName text=\"Material\"/>");
                        outputWriter.WriteLine("</Element>");

                        outputWriter.WriteLine("</m_Values>");
                        outputWriter.WriteLine("</m_Values>");

                        outputWriter.WriteLine("<m_GroupName text=\"" + mesh.MaterialBindings[triMaterialGroup.MaterialIndex].Name + "\"/>");

                        outputWriter.WriteLine("<m_MeshName text=\"" + mesh.Name + "\"/>");

                        outputWriter.WriteLine("<m_StateName text=\"Default\"/>");

                        outputWriter.WriteLine("</Element>");
                    }
                }

                outputWriter.WriteLine("</m_GroupStates>");

                outputWriter.WriteLine("</Element>");

            }
            outputWriter.WriteLine("</m_ModelInstances>");
            outputWriter.WriteLine("</m_GeometrySet>");
        }

        private static void WriteBehaviorMetadataToStream(Dictionary<string, string> civ6ShortNameToLongNameLookup, StreamWriter outputWriter)
        {
            outputWriter.WriteLine("<m_BehaviorData>");
            outputWriter.WriteLine("<m_behaviorDataSets>");

            outputWriter.WriteLine("<m_animationBindings>");
            outputWriter.WriteLine("<m_Bindings>");

            foreach (string civ6AnimName in civ6ShortNameToLongNameLookup.Keys)
            {
                outputWriter.WriteLine("<Element>");
                outputWriter.WriteLine("<m_SlotName text=\"" + civ6AnimName + "\"/> ");
                outputWriter.WriteLine("<m_AnimationName text=\"" + civ6ShortNameToLongNameLookup[civ6AnimName] + "\"/> ");
                outputWriter.WriteLine("</Element>");
            }

            outputWriter.WriteLine("</m_Bindings>");
            outputWriter.WriteLine("</m_animationBindings>");

            outputWriter.WriteLine("<m_timelineBindings>");
            outputWriter.WriteLine("<m_Bindings>");

            foreach (string civ6AnimName in civ6ShortNameToLongNameLookup.Keys)
            {
                outputWriter.WriteLine("<Element>");
                outputWriter.WriteLine("<m_SlotName text=\"" + civ6AnimName + "\"/>");
                outputWriter.WriteLine("<m_TimelineName text=\"" + civ6AnimName + "\"/>");
                outputWriter.WriteLine("</Element>");
            }

            outputWriter.WriteLine("</m_Bindings>");
            outputWriter.WriteLine("</m_timelineBindings>");

            outputWriter.WriteLine("<m_timelines>");
            outputWriter.WriteLine("<m_Timelines>");

            foreach (string civ6AnimName in civ6ShortNameToLongNameLookup.Keys)
            {
                outputWriter.WriteLine("<Element>");
                outputWriter.WriteLine("<m_Name text=\"" + civ6AnimName + "\"/>");
                outputWriter.WriteLine("<m_Description text=\"\"/>");
                outputWriter.WriteLine("<m_AnimationName text=\"" + civ6ShortNameToLongNameLookup[civ6AnimName] + "\"/>");
                outputWriter.WriteLine("<m_fDuration>0.000000</m_fDuration>");
                outputWriter.WriteLine("<m_Triggers/>");
                outputWriter.WriteLine("</Element>");
            }

            outputWriter.WriteLine("</m_Timelines>");
            outputWriter.WriteLine("</m_timelines>");

            outputWriter.WriteLine("<m_attachmentPoints>");
            outputWriter.WriteLine("<m_Points>");
            outputWriter.WriteLine("</m_Points>");
            outputWriter.WriteLine("</m_attachmentPoints>");
            outputWriter.WriteLine("</m_behaviorDataSets>");
            outputWriter.WriteLine("<m_behaviorInstances/>");
            outputWriter.WriteLine("<m_dsgName text=\"potential_any_graph\"/>");
            outputWriter.WriteLine("<m_referenceGeometryNames/>");
            outputWriter.WriteLine("</m_BehaviorData>");
        }
    }
}
