using System.Collections.Generic;

namespace NexusBuddy.GrannyInfos
{
    public class GrannyMeshInfo
    {
        public string name;
        public List<GrannyVertexInfo> vertices;
        public List<int[]> triangles;
        public List<string> boneBindings;
        public string materialName;

        public List<GrannyMeshVertexStructInfo> vertexStructInfos;
        public int bytesPerVertex;
        public List<string> materialBindingNames;

        public List<PrimaryTopologyGroupInfo> primaryTopologyGroupInfos;

        public void setVertexStructInfos(List<GrannyMeshVertexStructInfo> inVertexStructInfos)
        {
            vertexStructInfos = inVertexStructInfos;
            int currentOffset = 0;
            foreach (GrannyMeshVertexStructInfo structInfo in vertexStructInfos)
            {
                structInfo.offset = currentOffset;
                currentOffset += structInfo.length * structInfo.count;
            }
            bytesPerVertex = currentOffset;
        }

        public GrannyMeshVertexStructInfo getVertexStructInfoByName(string name) {
            foreach (GrannyMeshVertexStructInfo structInfo in vertexStructInfos) {
                if (structInfo.name.Equals(name))
                {
                    return structInfo;
                }
            }
            return null;
        }
    }
}
