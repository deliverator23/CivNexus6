namespace NexusBuddy.GrannyInfos
{
    public class PrimaryTopologyGroupInfo
    {
        public PrimaryTopologyGroupInfo() { }

        public PrimaryTopologyGroupInfo(int groupMaterialIndex, int groupTriFirst, int groupTriCount)
        {
            this.groupMaterialIndex = groupMaterialIndex;
            this.groupTriCount = groupTriCount;
            this.groupTriFirst = groupTriFirst;
        }

        public int groupMaterialIndex;
        public int groupTriFirst;
        public int groupTriCount;
    }
}
