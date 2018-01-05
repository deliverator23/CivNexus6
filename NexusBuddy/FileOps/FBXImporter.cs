using Firaxis.CivTech;
using Firaxis.Error;
using System.IO;

namespace NexusBuddy.FileOps
{
    public class FBXImporter
    {
		public static bool ImportFBXFile(string inputFilename, string outputFilename, string nodeName)
		{
            // Make Parameter
            //string nodeName = "Root";

            string outputDirectory = Path.GetDirectoryName(outputFilename);
            string shortFileName = Path.GetFileNameWithoutExtension(outputFilename);
            string fgxFileName = shortFileName.Replace(".fbx", ".fgx");

            IFBXInterface fbxInterface = new Autodesk_FBXInterface();
            ResultCode resultCode = fbxInterface.ExportGeometry(inputFilename, outputDirectory, nodeName, fgxFileName);
            return true;
       }
    }
}
