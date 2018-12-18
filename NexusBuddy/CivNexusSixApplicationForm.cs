using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

using Firaxis.Utility;
using Firaxis.Granny;
using NexusBuddy.GrannyWrappers;
using NexusBuddy.GrannyInfos;
using NexusBuddy.FileOps;

namespace NexusBuddy
{ 

    public class CivNexusSixApplicationForm : Form
	{
		public static CivNexusSixApplicationForm form;
		public static IGrannyFile loadedFile;
        public static string applicationName = "CivNexus6";
        public static int major_version = 1;
        public static int minor_version = 3;
        public static int sub_minor_version = 1;
        
	    public string modelTemplateFilename;
        public string modelTemplateFilename2;
        public string modelTemplateFilename3;
        public string dummyWigFilename;
        public string sourceTemplatePath = "E:\\mod\\3D\\templates\\";
        
		private Random rand = new Random();
        private List<string> TempFiles = new List<string>();
		private IContainer components;
        private SplitContainer masterSplitContainer;
		private PropertyGrid properties;
		private ColumnHeader animation;
		private ColumnHeader duration;

		private ColumnHeader materialTypeColumnHeader;
        private ColumnHeader materialNameColumnHeader;
        private GroupBox fileInfoGroupBox;
        private RichTextBox fileInfoTextBox;
        private Label headerFilenameLabel;
        private SplitContainer leftHandSplitContainer;
        private TabControl mainTabControl;
        private TabPage editModelTabPage;
        private TabPage editMaterialsTabPage;
        private SplitContainer editModelContainer;
        private SplitContainer editMaterialsContainer;
        private Panel mainButtonPanel;
        private Button openButton;
        private Button viewButton;
        private Button saveButton;
        private Button saveAsButton;
        private Button saveAnimationButton;
        public ListView meshList;
        public ListViewWithComboBox triangleGroupsList;
        private ColumnHeader MeshNameColumnHeader;
        private ColumnHeader MaterialColumnHeader;
        private ColumnHeader TriangleGroupCountColumnHeader;
        private ColumnHeader TriangleGroupColumnHeader;
        private SplitContainer materialListPanelContainer;
        public ListView materialList;
        private ColumnHeader materialNameHeader;
        private ColumnHeader materialTypeHeader;
        private Panel materialButtonsPanel;
        private Button addMaterialButton;
        private Button deleteMaterialButton;
        private Button clearMaterialsButton;
        private TabControl animationsTabControl;
        private TabPage grannyAnimsTabPage;
        private ListView animList;
        private ColumnHeader anim;
        private ColumnHeader timeSlice;
        private Panel panel1;
        private ToolTip viewButtonToolTip;
        private TabPage otherActionsTabPage;
        private Button exportAllModelsButton;
        private Button processTextureButton;
        private Button processTexturesInDirButton;
        private Button batchConversionButton;
        private Button exportCurrentModelButton;
        private Button br2ImportButton;
        private Button openFBXButton;
        private Button overwriteBR2Button;
        private Button overwriteMeshesButton;
        private TabPage selectModelTabPage;
        private TabPage furtherActionsTabPage;
        private ListView modelList;
        private ColumnHeader modelName;
        private ListViewItem lastModelListItemChecked;
        private Button writeGeoFileButton;
        private Int32 currentModelIndex;
        private RadioButton importExportFormatBR2NB2RadioButton;
        private RadioButton importExportFormatCN6RadioButton;
        private Label vertexFormatLabel;
        private Label importExportFiletypesLabel;
        private Button removeAnimationsButton;
	    private Button removeNamedBoneButton;
        private Button resaveAllFBXAsAnimsButton;
        private Button makeTemplateButton;
        private Button exportBR2Button;
        private Label rescaleFactorLabel;
        private Label xPositionLabel;
        private Label yPositionLabel;
        private Label zPositionLabel;
        private Button insertAdjustmentBoneButton;
        private TextBox rescaleFactorTextBox;
        private TextBox xPositionTextBox;
        private TextBox yPositionTextBox;
        private TextBox zPositionTextBox;
        private ComboBox bonesComboBox;
        private Label rescaleBoneNameLabel;
        private Button rescaleNamedBoneButton;
        private Button exportNA2Button;
        private Label endTimeTextBoxLabel;
        private Label startTimeTextBoxLabel;
        private Label textureClassLabel;
        private TextBox endTimeTextBox;
        private Label angleLabel;
        private Label axisLabel;
        private ComboBox axisComboBox;
        private Label geoClassNameLabel;
        private Label materialClassNameLabel;
        private Label assetClassNameLabel;
        private Label dsgLabel;
        private Label multiModelAssetCheckBoxLabel;
        private ComboBox geoClassNameComboBox;
        private ComboBox materialClassNameComboBox;
        private ComboBox assetClassNameComboBox;
        private ComboBox dsgComboBox;
        private ComboBox vertexFormatComboBox;
        private ComboBox textureClassComboBox;

        private TextBox angleTextBox;
        private Button concatenateNA2Button;
        private Label fpsLevel;
        private TextBox fpsTextBox;
        private Button openBR2Button;
        private Label nodeLabel;
        private TextBox nodeTextBox;
        private Button loadStringDatabaseButton;
        private TextBox startTimeTextBox;
        private CheckBox multiModelAssetCheckBox;

		public CivNexusSixApplicationForm()
		{
			form = this;
			InitializeComponent();
            Available.Startup(applicationName);
            Context.Add(new GrannyContext());
			Context.Add(new ProjectConfig());
			Context.Get<GrannyContext>();
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
            try
            {
                Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("NexusBuddy.GrannyTemplates.model_template.fgx");
                modelTemplateFilename = Path.GetTempPath() + "modelTemplate.temp.fgx";
                FileStream fileStream = File.Create(modelTemplateFilename);
                ReadWriteStream(manifestResourceStream, fileStream);
                fileStream.Close();

                manifestResourceStream = executingAssembly.GetManifestResourceStream("NexusBuddy.GrannyTemplates.model_template_2uv.fgx");
                modelTemplateFilename2 = Path.GetTempPath() + "modelTemplate_2uv.temp.fgx";
                fileStream = File.Create(modelTemplateFilename2);
                ReadWriteStream(manifestResourceStream, fileStream);
                fileStream.Close();

                manifestResourceStream = executingAssembly.GetManifestResourceStream("NexusBuddy.GrannyTemplates.model_template_3uv.fgx");
                modelTemplateFilename3 = Path.GetTempPath() + "modelTemplate_3uv.temp.fgx";
                fileStream = File.Create(modelTemplateFilename3);
                ReadWriteStream(manifestResourceStream, fileStream);
                fileStream.Close();

                manifestResourceStream = executingAssembly.GetManifestResourceStream("NexusBuddy.GrannyTemplates.dummy.wig");
                dummyWigFilename = Path.GetTempPath() + "dummy.temp.wig";
                fileStream = File.Create(dummyWigFilename);
                ReadWriteStream(manifestResourceStream, fileStream);
                fileStream.Close();
            }
            catch (Exception)
            {
            }
		}

		private void ReadWriteStream(Stream readStream, Stream writeStream)
		{
			int num = 256;
			byte[] buffer = new byte[num];
			for (int i = readStream.Read(buffer, 0, num); i > 0; i = readStream.Read(buffer, 0, num))
			{
				writeStream.Write(buffer, 0, i);
			}
			readStream.Close();
			writeStream.Close();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
            CleardownAllData();
            base.OnClosing(e);
		}

        private void CleardownAllData()
        {
            loadedFile = null;
            foreach (string current in TempFiles)
            {
                try
                {
                    File.Delete(current);
                }
                catch (Exception)
                {
                }
            }
        }

		private void AddAnimToListbox(IGrannyAnimation grannyAnimation)
		{
			animList.Items.Add(grannyAnimation.Name);
			animList.Items[animList.Items.Count - 1].SubItems.Add(grannyAnimation.Duration.ToString("f6", CultureInfo.InvariantCulture));
			animList.Items[animList.Items.Count - 1].SubItems.Add(grannyAnimation.TimeStep.ToString("f6", CultureInfo.InvariantCulture));
        }

		private void AddMeshToListbox(IGrannyMesh grannyMesh)
		{
			meshList.Items.Add(grannyMesh.Name);
            meshList.Items[meshList.Items.Count - 1].SubItems.Add(grannyMesh.TriangleMaterialGroups.Count.ToString());
            meshList.Items[meshList.Items.Count - 1].Tag = grannyMesh;
		}

        private void AddMaterialToListbox(IGrannyMaterial material)
		{
            materialList.Items.Add(material.Name);
            materialList.Items[materialList.Items.Count - 1].SubItems.Add(material.typeName);
            materialList.Items[materialList.Items.Count - 1].Tag = new MaterialProperties(material);
        }

        private void RefreshAppDataWithMessage(string message)
        {
            RefreshAppData();
            fileInfoTextBox.Text = fileInfoTextBox.Text + message;
        }

		public void RefreshAppData()
		{
            string fileInfo = "";
            if (loadedFile != null) {

                if (currentModelIndex == -1)
                {
                    currentModelIndex = 0;
                }
                
			    materialList.Items.Clear();
			    meshList.Items.Clear();
			    animList.Items.Clear();
                triangleGroupsList.Items.Clear();
			    properties.SelectedObject = null;

                if (loadedFile.Models.Count > 0 && loadedFile.Models[currentModelIndex].Skeleton != null)
                {
                    IGrannySkeleton skeleton = loadedFile.Models[currentModelIndex].Skeleton;
                    bonesComboBox.Items.Clear();
                    foreach (IGrannyBone bone in skeleton.Bones)
                    {
                        bonesComboBox.Items.Add(bone.Name);
                    }
                }

                if (modelList.Items.Count != loadedFile.Models.Count)
                {
                    foreach (IGrannyModel currentModel in loadedFile.Models)
                    {
                        modelList.Items.Add(currentModel.Name);
                    }
                    modelList.Items[currentModelIndex].Checked = true;
                }

                if (loadedFile.Models.Count > 0)
                {
                    foreach (IGrannyMesh currentMesh in loadedFile.Models[currentModelIndex].MeshBindings)
                    {
                        AddMeshToListbox(currentMesh);
                    }
                }

                if (meshList.Items.Count > 0)
                {
                    meshList.Items[0].Selected = true;
                    meshList.Select();
                }
                
			    foreach (IGrannyMaterial currentMaterial in loadedFile.Materials)
			    {
                    AddMaterialToListbox(currentMaterial); 
			    }
                foreach (IGrannyAnimation currentAnimation in loadedFile.Animations)
			    {
                    AddAnimToListbox(currentAnimation);
			    }

                IGrannyFile file = loadedFile;
                string filename = Path.GetFileName(file.Filename);
                headerFilenameLabel.Text = filename;
                fileInfo += "Models: " + file.Models.Count + "     ";
                if (file.Models.Count > 0)
                {
                    fileInfo += "Current Model: " + file.Models[currentModelIndex].Name + " (Index:" + currentModelIndex + ")" + Environment.NewLine;
                    fileInfo += "Bones (Current Model): " + file.Models[currentModelIndex].Skeleton.Bones.Count + "     ";
                    fileInfo += "Meshes (Current Model): " + file.Models[currentModelIndex].MeshBindings.Count + "     ";
                }
                fileInfo += "Meshes (Total): " + file.Meshes.Count + Environment.NewLine;
                fileInfo += "Materials: " + file.Materials.Count + Environment.NewLine;
                fileInfo += "Animations: " + file.Animations.Count + Environment.NewLine;
            }
            fileInfoTextBox.Text = fileInfo;
		}

        private void InsertAdjustmentBoneClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("Must have model file loaded to add bone.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            InsertAdjustmentBoneAction();
        }

        private void InsertAdjustmentBoneAction()
        {
            GrannySkeletonWrapper skeletonWrapper = new GrannySkeletonWrapper(loadedFile.Models[currentModelIndex].Skeleton);
            GrannySkeletonInfo skeletonInfo = skeletonWrapper.readSkeletonInfo();

            foreach (GrannyBoneInfo curBoneInfo in skeletonInfo.bones)
            {
                curBoneInfo.parentIndex = curBoneInfo.parentIndex + 1;
            }

            float scaleFactor = 1f;
            try
            {
                scaleFactor = float.Parse(rescaleFactorTextBox.Text, CultureInfo.InvariantCulture);
            }
            catch (FormatException) { }

            float xPosition = 1f;
            try
            {
                xPosition = float.Parse(xPositionTextBox.Text, CultureInfo.InvariantCulture);
            }
            catch (FormatException) { }

            float yPosition = 1f;
            try
            {
                yPosition = float.Parse(yPositionTextBox.Text, CultureInfo.InvariantCulture);
            }
            catch (FormatException) { }

            float zPosition = 1f;
            try
            {
                zPosition = float.Parse(zPositionTextBox.Text, CultureInfo.InvariantCulture);
            }
            catch (FormatException) { }

            Vector3D axisVector = new Vector3D(1.0d, 0.0d, 0.0d);
            if (axisComboBox.SelectedIndex == 1)
            {
                axisVector = new Vector3D(0.0d, 1.0d, 0.0d);
            }
            else if (axisComboBox.SelectedIndex == 2)
            {
                axisVector = new Vector3D(0.0d, 0.0d, 1.0d);
            }

            float angle = 1f;
            try
            {
                angle = float.Parse(angleTextBox.Text, CultureInfo.InvariantCulture);
            }
            catch (FormatException) { }

            Quaternion quat = new Quaternion(axisVector, angle);

            GrannyBoneInfo boneInfo = new GrannyBoneInfo();

            int j = 1;
            string adjBoneNameRoot = "CN6_ADJUSTMENT_BONE";
            foreach (IGrannyBone bone in skeletonWrapper.wrappedSkeleton.Bones)
            {
                if (bone.Name.StartsWith(adjBoneNameRoot))
                {
                    j++;
                }
            }

            boneInfo.name = adjBoneNameRoot + "_" + j;

            boneInfo.parentIndex = -1;
            GrannyTransformInfo transformInfo = new GrannyTransformInfo();
            float[] position = { xPosition, yPosition, zPosition };
            float[] orientation = { (float)quat.X, (float)quat.Y, (float)quat.Z, (float)quat.W };
            float[] scaleShear = { scaleFactor, 0f, 0f, 0f, scaleFactor, 0f, 0f, 0f, scaleFactor };
            float[] invWorldTransform = { 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f };
            transformInfo.position = position;
            transformInfo.orientation = orientation;
            transformInfo.scaleShear = scaleShear;
            transformInfo.flags = 7;
            boneInfo.localTransform = transformInfo;
            boneInfo.inverseWorldTransform = invWorldTransform;
            skeletonInfo.bones.Insert(0, boneInfo);

            skeletonWrapper.writeSkeletonInfo(skeletonInfo);
            SaveAction();
            RefreshAppDataWithMessage("ADJUSTMENT BONE INSERTED.");
        }

        private void RescaleNamedBoneButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("Must have model file loaded to add bone.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            GrannySkeletonWrapper skeletonWrapper = new GrannySkeletonWrapper(loadedFile.Models[currentModelIndex].Skeleton);
            GrannySkeletonInfo skeletonInfo = skeletonWrapper.readSkeletonInfo();

            float scaleFactor = float.Parse(rescaleFactorTextBox.Text, CultureInfo.InvariantCulture);

            GrannyBoneInfo boneInfo = new GrannyBoneInfo();
            boneInfo.name = bonesComboBox.SelectedItem + "_FX";
            boneInfo.parentIndex = bonesComboBox.SelectedIndex;
            GrannyTransformInfo transformInfo = new GrannyTransformInfo();
            float[] position = { 0f, 0f, 0f };
            float[] orientation = { 0f, 0f, 0f, 1f };
            float[] scaleShear = { scaleFactor, 0f, 0f, 0f, scaleFactor, 0f, 0f, 0f, scaleFactor };
            float[] invWorldTransform = skeletonInfo.bones[bonesComboBox.SelectedIndex].inverseWorldTransform;
            transformInfo.position = position;
            transformInfo.orientation = orientation;
            transformInfo.scaleShear = scaleShear;
            transformInfo.flags = 4;
            boneInfo.localTransform = transformInfo;
            boneInfo.inverseWorldTransform = invWorldTransform;
            skeletonInfo.bones.Add(boneInfo);

            skeletonWrapper.writeSkeletonInfo(skeletonInfo);
            SaveAction();
            RefreshAppDataWithMessage("RESCALE BONE ADDED.");
        }

        private void ExportNA2ButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("Must have model file loaded to export animation.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (loadedFile != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "fgx files (*.fgx) | *.fgx";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    IGrannyFile animationFile = OpenFileAsTempFileCopy(openFileDialog.FileName, "tempanimopen");
                    if (animationFile.Animations.Count == 1)
                    {
                        int retryLimit = 5;
                        float startTime = -1;
                        try
                        {
                            startTime = float.Parse(startTimeTextBox.Text, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException) { }

                        float endTime = -1;
                        try
                        {
                            endTime = float.Parse(endTimeTextBox.Text, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException) { }

                        int fpsInput = 0;
                        try
                        {
                            fpsInput = int.Parse(fpsTextBox.Text, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException) { }

                        int retryCount = NB2NA2FileOps.exportNA2(loadedFile.Models[currentModelIndex], animationFile.Animations[0], openFileDialog.FileName, startTime, endTime, fpsInput, retryLimit);

                        if (retryCount == retryLimit)
                        {
                            RefreshAppDataWithMessage("ANIMATION EXPORT FAILED! Retry Count: " + retryCount);
                         }
                        else
                        {
                            RefreshAppDataWithMessage("ANIMATION EXPORTED! Retry Count: " + retryCount);
                        }             
                    }
                }
                
            }
        }

        private void ProcessTexturesInDirectoryClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = Settings.Default.RecentFolder
            };
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string sourcePath = folderBrowserDialog.SelectedPath;
                Settings.Default.RecentFolder = sourcePath;
                var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(s => s.ToLower().EndsWith(".dds"));

                foreach (string filename in files)
                {
                    string output = ProcessTextureAction(filename, textureClassComboBox.Text);
                    RefreshAppDataWithMessage(output);
                }
            }
        }

        private void ProcessTextureClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.dds;*.tga;*.jpg)|*.png;*.dds;*.tga;*.jpg";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog.FileName;
                string output = ProcessTextureAction(filename, textureClassComboBox.Text);
                RefreshAppDataWithMessage(output);
            }
        }

        private string ProcessTextureAction(string filename, string textureClassName)
        {
            string shortFilename = Path.GetFileName(filename);

            string directory = Path.GetDirectoryName(filename);

            string outputPath = directory + "\\Textures";

            Directory.CreateDirectory(outputPath);
            
            TextureClass textureClass = TextureClass.GetAllTextureClasses()[textureClassName];

            Dictionary<string, string> inputImageMetadata = GetImageMetadata(directory, shortFilename);

            string output = "FAILED!";

            if (inputImageMetadata.Count != 0)
            {

                int inHeight = Int32.Parse(inputImageMetadata["height"]);
                int inWidth = Int32.Parse(inputImageMetadata["width"]);

                if (inHeight > textureClass.MaxHeight)
                {
                    MessageBox.Show("Height of input image is too high for " + textureClassName + ". Should be less than " + textureClass.MaxHeight + ".", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return "Failed";
                }
                if (inHeight < textureClass.MinHeight)
                {
                    MessageBox.Show("Height of input image is too low for " + textureClassName + ". Should be greater than " + textureClass.MinHeight + ".", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return "Failed";
                }
                if (inWidth > textureClass.MaxWidth)
                {
                    MessageBox.Show("Height of input image is too high for " + textureClassName + ". Should be less than " + textureClass.MaxWidth + ".", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return "Failed";
                }
                if (inWidth < textureClass.MinWidth)
                {
                    MessageBox.Show("Height of input image is too low for " + textureClassName + ". Should be greater than " + textureClass.MinWidth + ".", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return "Failed";
                }
                if (textureClass.RequireSquare && inWidth != inHeight)
                {
                    MessageBox.Show("Input image needs to be square for " + textureClassName + ".", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return "Failed";
                }

                // Convert image using texconv.exe
                ProcessStartInfo texconvProcessInfo = new ProcessStartInfo();
                texconvProcessInfo.CreateNoWindow = true;
                texconvProcessInfo.UseShellExecute = false;
                texconvProcessInfo.FileName = "texconv.exe";
                texconvProcessInfo.RedirectStandardOutput = true;
                texconvProcessInfo.WindowStyle = ProcessWindowStyle.Normal;
                texconvProcessInfo.WorkingDirectory = directory;
                texconvProcessInfo.Arguments = "\"" + shortFilename + "\" -f " + textureClass.PixelFormat + " -o \"" + outputPath + "\" -y";

                if (!textureClass.AllowArtistMips)
                {
                    texconvProcessInfo.Arguments += " -m 1";
                }

                if (textureClass.ExportGammaIn == 2.2f && textureClass.ExportGammaOut == 2.2f)
                {
                    texconvProcessInfo.Arguments += " -srgbo";
                }

                Process proc = new Process
                {
                    StartInfo = texconvProcessInfo
                };
                proc.Start();

                output = texconvProcessInfo.FileName + " " + texconvProcessInfo.Arguments + "\n";
                while (!proc.StandardOutput.EndOfStream)
                {
                    //output = proc.StandardOutput.ReadToEnd();
                    string line = proc.StandardOutput.ReadLine();
                    if (line.StartsWith("reading") || line.StartsWith("writing"))
                    {
                        output = output + line + "\n";
                    }
                }

                proc.Close();

                string filenameWithoutExt = Path.GetFileNameWithoutExtension(shortFilename);
                string outputShortFilename = filenameWithoutExt + ".dds";

                Dictionary<string, string> outputImageMetadataDictionary = GetImageMetadata(outputPath, outputShortFilename);

                MetadataWriter.WriteTextureFile(outputPath, Path.GetFileNameWithoutExtension(shortFilename), outputImageMetadataDictionary, textureClassName, textureClass);
            }

            return output;
        }

        private static Dictionary<string, string> GetImageMetadata(string path, string shortFilename)
        {
            // Get image info using texdiag.exe
            ProcessStartInfo texdiagProcessInfo = new ProcessStartInfo();
            texdiagProcessInfo.CreateNoWindow = true;
            texdiagProcessInfo.UseShellExecute = false;
            texdiagProcessInfo.FileName = "texdiag.exe";
            texdiagProcessInfo.RedirectStandardOutput = true;
            texdiagProcessInfo.WindowStyle = ProcessWindowStyle.Normal;
            texdiagProcessInfo.WorkingDirectory = path;
            texdiagProcessInfo.Arguments = "info \"" + shortFilename + "\"";

            Process proc = new Process
            {
                StartInfo = texdiagProcessInfo
            };
            proc.Start();

            Dictionary<string, string> outputImageMetadataDictionary = new Dictionary<string, string>();
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                if (line.Contains("="))
                {
                    string[] parts = line.Split('=');
                    string name = parts[0].Trim();
                    string value = parts[1].Trim();
                    outputImageMetadataDictionary[name] = value;
                }
            }

            return outputImageMetadataDictionary;
        }

        private void OpenButtonClick(object sender, EventArgs e)
        {
            OpenButtonDialogAction("fgx/gr2 files (*.fgx)|*.fgx;*.gr2|br2 files (*.br2)|*.br2|cn6 files (*.cn6)|*.cn6|All files (*.*)|*.*");
        }

        private void OpenFBXButtonClick(object sender, EventArgs e)
        {
            //Blender 2.7

            //After.nb2 import.

            //1) Bone Roll -90
            //2) Export to FBX - Y Forward, Z Up
            //3) Open FBX
            //4) Overwrite BR2

            if (System.String.IsNullOrEmpty(nodeTextBox.Text))
            {
                MessageBox.Show("Must specify Node for FBX import!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            OpenButtonDialogAction("FBX files (*.fbx)|*.fbx|All files (*.*)|*.*");
        }

        private void OpenBR2ButtonClick(object sender, EventArgs e)
        {
            OpenButtonDialogAction("br2 files (*.br2)|*.br2|All files (*.*)|*.*");       
        }

        private void OpenCN6ButtonClick(object sender, EventArgs e)
        {
            OpenButtonDialogAction("cn6 files (*.cn6)|*.cn6|All files (*.*)|*.*");
        }

        private void OpenButtonDialogAction(string filterString)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = filterString;
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string openFilename = openFileDialog.FileName;
                currentModelIndex = -1;
                modelList.Items.Clear();
                loadedFile = OpenFileAction(openFilename);

                RefreshAppDataWithMessage("FILE OPENED: " + openFilename);
            }
        }
        
        public IGrannyFile OpenFileAction(string openFilename)
        {
            IGrannyFile targetFile = OpenFileAsTempFileCopy(openFilename, "tempopen");
            bool saveFile = false;

            if (openFilename.ToLower().Contains(".fbx"))
            {
                openFilename = openFilename.Replace(".fbx", ".fgx");
                openFilename = openFilename.Replace(".FBX", ".fgx");
                saveFile = true;
            }
            else if (openFilename.ToLower().Contains(".br2"))
            {
                openFilename = openFilename.Replace(".br2", ".fgx");
                openFilename = openFilename.Replace(".BR2", ".fgx");
            }
            else if (openFilename.ToLower().Contains(".cn6"))
            {
                openFilename = openFilename.Replace(".cn6", ".fgx");
                openFilename = openFilename.Replace(".CN6", ".fgx");
            }
            else if (openFilename.ToLower().Contains(".gr2"))
            {
                openFilename = openFilename.Replace(".gr2", ".fgx");
                openFilename = openFilename.Replace(".GR2", ".fgx");
                saveFile = true;
            }

            targetFile.Filename = openFilename;

            if (saveFile)
            {
                targetFile.Save();
            }
        
            return targetFile;
        }

        public static string GetVersionString()
        {
            return major_version + "." + minor_version + "." + sub_minor_version;
        }

        public static void SetExporterInfo(GrannyFileWrapper fileWrapper)
        {
            fileWrapper.setExporterInfo(applicationName, major_version, minor_version, sub_minor_version, 0);
        }

        public IGrannyFile OpenFileAsTempFileCopy(string openFilename, string prefix)
        {
            GrannyContext grannyContext = Context.Get<GrannyContext>();
            string tempPath = Path.GetTempPath();
            string tempFilename = tempPath + prefix;
            string nodeName = "Root";

            if (openFilename.ToLower().Contains(".fbx"))
            {
                if (!System.String.IsNullOrEmpty(nodeTextBox.Text))
                {
                    nodeName = nodeTextBox.Text;
                }
                string tempFilename2 = tempFilename;
                tempFilename2 = AppendRandomNumberAndFGXExtension(tempFilename2);

                FBXImporter.ImportFBXFile(openFilename, tempFilename2, nodeName);

                IGrannyFile fbxfgxfile = grannyContext.LoadGrannyFile(tempFilename2);
                
                GrannyFileWrapper fileWrapper = new GrannyFileWrapper(fbxfgxfile);
                fileWrapper.setNumMaterials(0);

                //fileWrapper.setFromArtToolInfo("Blender", 2, 0);
                //float[] matrix = { 1f, 0f, 0f, 0f, 0f, 1f, 0f, -1f, 0f };
                //fileWrapper.setMatrix(matrix);
                //SetExporterInfo(fileWrapper);
                //fileWrapper.setFromFileName(openFilename);

                tempFilename = AppendRandomNumberAndFGXExtension(tempFilename);

                SaveAsAction(fbxfgxfile, tempFilename, false);

                TempFiles.Add(tempFilename);
                TempFiles.Add(tempFilename2);
            }
            else if (openFilename.ToLower().Contains(".br2"))
            {
                tempFilename = AppendRandomNumberAndFGXExtension(tempFilename);
                BR2FileOps.importBR2(openFilename, tempFilename, grannyContext);
                TempFiles.Add(tempFilename);
            }
            else if (openFilename.ToLower().Contains(".cn6"))
            {
                tempFilename = AppendRandomNumberAndFGXExtension(tempFilename);
                CN6FileOps.importCN6(openFilename, tempFilename, grannyContext, vertexFormatComboBox.SelectedIndex);
                TempFiles.Add(tempFilename);
            }
            else
            {
                tempFilename = AppendRandomNumberAndFGXExtension(tempFilename);
                TempFiles.Add(tempFilename);
                try
                {
                    if (File.Exists(tempFilename))
                    {
                        File.Delete(tempFilename);
                    }
                }
                catch (Exception)
                {
                }
                File.Copy(openFilename, tempFilename);
            }

            IGrannyFileLoader grannyFileLoader = new GrannyFileLoader();
            IGrannyFile targetFile = grannyFileLoader.LoadGrannyFile(tempFilename);
            return targetFile;
        }

        private string FindRootString(List<string> strings)
        {
            if (strings.Count > 0) { 
                string currentRoot = strings.First();
                strings.Remove(currentRoot);

                foreach (string curString in strings)
                {
                    char[] rootArray = currentRoot.ToCharArray();
                    char[] currArray = curString.ToCharArray();
                    List<char> newRootCharList = new List<char>();
                    for (int i = 0; i < currArray.Length && i < rootArray.Length; i++)
                    {
                        if (currArray[i].Equals(rootArray[i]))
                        {
                            newRootCharList.Add(currArray[i]);
                        }
                    }
                    currentRoot = new string(newRootCharList.ToArray());
                }

                return currentRoot.TrimEnd('_');
            } else
            {
                return "no_animations";
            }
        }

        private void BatchConversionAction(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "dat files (*.dat) | *.dat";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string datFileName = openFileDialog.FileName;
                string baseDirectory = Path.GetDirectoryName(datFileName);

                string modbuddyDirectory = baseDirectory + "\\Modbuddy";
                Directory.CreateDirectory(modbuddyDirectory);

                StreamReader streamReader = new StreamReader(datFileName);

                string regexString = "(.*);(.*);(.*);(.*)";

                RefreshAppDataWithMessage("Batch conversion start!");

                while (!streamReader.EndOfStream)
                {
                    string currentLine = streamReader.ReadLine();

                    Regex regex = new Regex(regexString);
                    MatchCollection mc = regex.Matches(currentLine);

                    //for each row in model_conv.dat
                    foreach (Match m in mc)
                    {
                        string gr2Filename = m.Groups[1].Value.Trim().ToLower();
                        string gr2ModelName = gr2Filename.Replace(".gr2", "");
                        string animationsString = m.Groups[2].Value;
                        string texturesString = m.Groups[3].Value;
                        string prettyName = m.Groups[4].Value.Trim();

                        bool hasPrettyName = prettyName.Length > 0;

                        List<string> processedTextureFilenames = new List<string>();
                        HashSet<string> textureSet = new HashSet<string>();

                        //Find all matching files and loop
                        string cn6DirectMatchFilename = baseDirectory + "\\" + gr2ModelName + ".cn6";
                        var cn6Filenames = new List<string>();
                        cn6Filenames.AddRange(Directory.GetFiles(baseDirectory, gr2ModelName + "__*.cn6"));
                        if (cn6Filenames.Count == 0)
                        {
                            cn6Filenames.Add(cn6DirectMatchFilename);
                        }

                        List<IGrannyFile> assetGrannyFiles = new List<IGrannyFile>();
                        List<Dictionary<string, string>> materialMappingsList = new List<Dictionary<string, string>>();
                        Dictionary<string, string> civ6ShortNameToLongNameLookup = null;

                        bool multiModelAsset = false;
                        if (cn6Filenames.Count > 1)
                        {
                            multiModelAsset = true;
                        }

                        cn6Filenames.RemoveAll(x => x.Contains("_batch.cn6"));

                        //for each matching cn6 file
                        foreach (string cn6Filepath in cn6Filenames)
                        {                          
                            string cn6Filename = Path.GetFileName(cn6Filepath);
                            string modelName = cn6Filename.Replace(".cn6", "");

                            string assetClass = assetClassNameComboBox.Text;
                            string geometryClass = geoClassNameComboBox.Text;
                            string materialClass = materialClassNameComboBox.Text;
                            string textureClass = textureClassComboBox.Text;
                            string dsgName = dsgComboBox.Text;

                            RefreshAppDataWithMessage("Processing " + modelName + "...");

                            string modelDirectory = baseDirectory + "\\" + modelName;

                            if (!hasPrettyName)
                            {
                                prettyName = modelName;
                            }
                            string[] animationFiles = animationsString.Split(',');
                            string[] textureFiles = texturesString.Split(',');
                            List<string> animationFilenames = new List<string>(); 

                            Directory.CreateDirectory(modelDirectory);

                            string cn6TargetFilename = modelDirectory + "\\" + modelName + ".cn6";

                            File.Copy(cn6Filepath, cn6TargetFilename, true);

                            List<string> animationFilePaths = new List<string>();

                            if (cn6Filename.ToLower().Contains("float"))
                            {
                                int z = 0;
                                z = z + 1;
                            }

                            foreach (string animationFile in animationFiles)
                            {
                                if (animationFile.Length > 4)
                                {
                                    string sourceAnimPath = baseDirectory + "\\" + animationFile;
                                    string targetAnimPath = modelDirectory + "\\" + animationFile.ToLower();
                                    if (File.Exists(sourceAnimPath))
                                    {
                                        File.Copy(sourceAnimPath, targetAnimPath, true);
                                        animationFilePaths.Add(targetAnimPath.ToLower());
                                    }
                                    animationFilenames.Add(animationFile.Replace(".gr2",""));
                                }
                            }

                            string animationRoot = null;
                            if (animationFilenames.Count.Equals(1))
                            {
                                string[] splitModelNameBits = modelName.Split('_');
                                if (splitModelNameBits.Length > 1)
                                {
                                    animationRoot = splitModelNameBits[0];
                                }
                            }
                            if (animationRoot == null)
                            {
                                animationRoot = FindRootString(animationFilenames);
                            }
                            
                            List<string> textureFilePaths = new List<string>();
                            foreach (string textureFile in textureFiles)
                            {
                                string textureFile2 = textureFile.Replace(".tga", ".dds");
                                string sourceTexPath = baseDirectory + "\\" + textureFile2;
                                string targetTexPath = modelDirectory + "\\" + textureFile2;
                                if (File.Exists(sourceTexPath))
                                {
                                    File.Copy(sourceTexPath, targetTexPath.ToLower(), true);
                                    textureFilePaths.Add(targetTexPath.ToLower());
                                }
                            }

                            //Geometry
                            loadedFile = OpenFileAction(cn6TargetFilename);
                            SaveAction();
                            InsertAdjustmentBoneAction();

                            //Class type overrides
                            if (cn6TargetFilename.Contains("_DCL_"))
                            {
                                geometryClass = "DecalGeometry";
                                materialClass = "DecalMaterial";
                                textureClass = "Decal_BaseColor";
                            }

                            MetadataWriter.WriteGeoAnimFile(loadedFile, currentModelIndex, geometryClass);

                            assetGrannyFiles.Add(loadedFile);

                            //Materials
                            string materialsDirectory = modelDirectory + "\\Materials";
                            Directory.CreateDirectory(materialsDirectory);
                            List<string> savedMaterials = new List<string>();
                            List<string> fgxMaterialNames = new List<string>();
                            Dictionary<string, string> materialBindingToMtlDict = new Dictionary<string, string>();
                            foreach (IGrannyMaterial material in loadedFile.Materials)
                            {
                                string fgxMaterialName = material.Name;
                                
                                string materialName = material.Name;

                                if (materialName.Contains("."))
                                {
                                    string[] parts = materialName.Split('.');
                                    materialName = parts[0];
                                }

                                string mtlName = materialName;

                                foreach (string textureFile in textureFiles)
                                {
                                    if (textureFile.StartsWith(materialName))
                                    {
                                        mtlName = textureFile.Replace(".tga", ".dds").Replace(".dds", "");
                                        materialBindingToMtlDict.Add(fgxMaterialName, mtlName);
                                        break;
                                    }
                                }

                                if (!savedMaterials.Contains(mtlName) && mtlName.Length > 0)
                                {
                                    MetadataWriter.WriteMaterialFile(materialsDirectory, mtlName + ".mtl", mtlName, materialClass, false);
                                    savedMaterials.Add(mtlName);
                                }

                                fgxMaterialNames.Add(mtlName.ToLower());
                            }

                            string geometriesDirectory = modelDirectory + "\\Geometries";
                            Directory.CreateDirectory(geometriesDirectory);
                            string fgxFilename = Path.GetFileName(cn6TargetFilename.Replace(".cn6", ".fgx"));
                            File.Copy(modelDirectory + "\\" + fgxFilename, geometriesDirectory + "\\" + fgxFilename, true);
                            string geoFilename = Path.GetFileName(cn6TargetFilename.Replace(".cn6", ".geo"));
                            File.Copy(modelDirectory + "\\" + geoFilename, geometriesDirectory + "\\" + geoFilename, true);
                            
                            //Animations
                            var animOutputDirectory = "Animations";

                            civ6ShortNameToLongNameLookup = BuildShortNameToLongNameLookup(animationRoot, animationFiles);

                            if (loadedFile != null && (!multiModelAsset || !multiModelAssetCheckBox.Checked))
                            {
                                MetadataWriter.WriteAssetFile(loadedFile, civ6ShortNameToLongNameLookup, assetClass, dsgName, prettyName, new List<Dictionary<string, string>> { materialBindingToMtlDict });
                            }

                            materialMappingsList.Add(materialBindingToMtlDict);

                            if (animationFilenames.Count() > 0)
                            {
                                ResaveAnimationGR2FilesToOutputDirectory(modelDirectory, animOutputDirectory, animationFiles);
                            }
                           
                            CleardownAllData();

                            foreach (string animationPath in animationFilePaths)
                            {
                                File.Delete(animationPath);
                                File.Delete(animationPath.Replace(".gr2", ".fgx"));
                            }

                            //Asset
                            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                            string assetFilename = textInfo.ToTitleCase(prettyName) + ".ast";
                            string assetsDirectory = modelDirectory + "\\Assets";
                            Directory.CreateDirectory(assetsDirectory);
                            if (!multiModelAsset || !multiModelAssetCheckBox.Checked)
                            {
                                File.Copy(modelDirectory + "\\" + assetFilename, assetsDirectory + "\\" + assetFilename, true);
                            }

                            //Textures
                            textureSet = new HashSet<string>(textureFilePaths);
                            foreach (string texturePath in textureSet)
                            {
                                string texShortFilename = Path.GetFileNameWithoutExtension(texturePath);
                                bool shouldProcess = fgxMaterialNames.Contains(texShortFilename);
                                if (shouldProcess && !processedTextureFilenames.Contains(texShortFilename))
                                {
                                    ProcessTextureAction(texturePath, textureClass);
                                    processedTextureFilenames.Add(texShortFilename);
                                }
                            }

                            File.Delete(modelDirectory + "\\" + fgxFilename);
                            File.Delete(modelDirectory + "\\" + geoFilename);
                            File.Delete(modelDirectory + "\\" + assetFilename);
                            File.Delete(cn6TargetFilename);

                            string SourcePath = modelDirectory;
                            string DestinationPath = modbuddyDirectory;

                            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
                            {
                                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
                            }

                            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
                            {
                                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
                            }

                            Directory.Delete(modelDirectory, true); 
                        }

                        foreach (string textureFile in textureSet.ToList())
                        {
                            string textureFile2 = textureFile.Replace(".tga", ".dds");
                            string texShortFilename = Path.GetFileNameWithoutExtension(textureFile2);
                            string leftoverTexPath = modbuddyDirectory + "\\" + Path.GetFileName(textureFile2);
                            
                            if (File.Exists(leftoverTexPath)) {
                                if (!processedTextureFilenames.Contains(texShortFilename))
                                {
                                    ProcessTextureAction(leftoverTexPath, textureClassComboBox.Text);
                                }
                                File.Delete(leftoverTexPath);
                            }
                        }

                        if (multiModelAsset && multiModelAssetCheckBox.Checked)
                        {
                            string modbuddyAssetsDirectory = modbuddyDirectory + "\\Assets";
                            MetadataWriter.WriteMultiModelAssetFile(modbuddyAssetsDirectory, gr2ModelName, assetGrannyFiles, civ6ShortNameToLongNameLookup, assetClassNameComboBox.Text, dsgComboBox.Text, null, materialMappingsList);
                            
                        }
                    }
                }

                RefreshAppDataWithMessage("Batch conversion complete!");
            }
        }

        private string AppendRandomNumberAndFGXExtension(string tempFilename)
        {
            tempFilename += rand.Next(1000000).ToString();
            tempFilename += rand.Next(1000000).ToString();
            tempFilename += rand.Next(1000000).ToString();
            tempFilename += ".fgx";
            return tempFilename;
        }

        private void ViewButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("No file is currently loaded.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            ViewAction();
        }

        private void ViewAction()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "fgxviewer.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            if (loadedFile != null)
            {
                startInfo.Arguments = "\"" + loadedFile.Filename + "\"";
                Process.Start(startInfo);
            }
        }
		
		private void SaveAsButtonClick(object sender, EventArgs e)
		{
            if (loadedFile == null)
            {
                MessageBox.Show("No file is currently loaded.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "fgx files (*.fgx)|*.fgx|All files (*.*)|*.*";
			saveFileDialog.FilterIndex = 0;
			saveFileDialog.RestoreDirectory = true;
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = saveFileDialog.FileName;
                SaveAsAction(loadedFile, fileName, true);       
			}
		}

        private void SaveButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("No file is currently loaded.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            DialogResult dialogResult = DialogResult.Yes;
            if (File.Exists(loadedFile.Filename))
            {
                dialogResult = MessageBox.Show("Are you sure you want to overwrite this file?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            }
            if (dialogResult == DialogResult.Yes)
            {
                SaveAction();
            }
        }

        public void SaveAction()
        {
            //GrannyFileWrapper fileWrapper = new GrannyFileWrapper(loadedFile);
            //float[] matrix = { 1f, 0f, 0f, 0f, 0f, 1f, 0f, -1f, 0f };
            //fileWrapper.setMatrix(matrix);
            string filename = loadedFile.Filename;
            SaveAsAction(loadedFile, filename, true);      
        }     

        public IGrannyFile SaveAsAction(IGrannyFile fileToSave, string fileName, bool updateAppDataAndSaveMessage)
        {
            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(fileToSave);
            SetExporterInfo(fileWrapper);

            string tempStagingFileName = fileName.Replace(".fgx", "_temp_" + rand.Next(1000000) + rand.Next(1000000) + ".fgx");
            fileToSave.Filename = tempStagingFileName;
            fileToSave.Source = "Tool";
            fileToSave.Save();
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            File.Move(tempStagingFileName, fileName);
            fileToSave.Filename = fileName;
            
            GrannyContext grannyContext = Context.Get<GrannyContext>();
            string tempPath = Path.GetTempPath();
            string tempFileName = tempPath + "tempopen";
            tempFileName += rand.Next(1000000).ToString();
            tempFileName += rand.Next(1000000).ToString();
            tempFileName += ".fgx";
            TempFiles.Add(tempFileName);
            File.Copy(fileName, tempFileName);
            IGrannyFile savedFile = grannyContext.LoadGrannyFile(tempFileName);

            savedFile.Filename = fileName;
            savedFile.Source = "Tool";

            loadedFile = savedFile;
            modelList.Items.Clear();

            if (updateAppDataAndSaveMessage)
            {
                RefreshAppDataWithMessage("FILE SAVED: " + fileName);
            }

            return loadedFile;
        }

        public string GetApplicationNameWithVersionNumber()
        {
            return applicationName + " " + GetVersionString();
        }

        private string getShortFilenameForLoadedFile()
        {
            return Path.GetFileName(loadedFile.Filename);
        }

        private void SaveAnimationClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("No file is currently loaded.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "fgx files (*.fgx)|*.fgx|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = getShortFilenameForLoadedFile().Replace(".fgx", "_anim.fgx");
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveFileDialog.FileName;
                SaveAnimationsAction(loadedFile, fileName);

                RefreshAppDataWithMessage("ANIMATION FILE SAVED.");
            }
        }

        private static List<IGrannyFile> SaveAnimationsAction(IGrannyFile grannyFile, string filename)
        {
            GrannyContext grannyContext = Context.Get<GrannyContext>();
            int i = 0;
            List<IGrannyFile> animationFiles = new List<IGrannyFile>();
            foreach (IGrannyAnimation animation in grannyFile.Animations)
            {
                string extension_string = "";
                if (i > 0)
                {
                    extension_string = i + "";
                }
                IGrannyFile animationFile = grannyContext.CreateEmptyGrannyFile(filename.Replace(".fgx", extension_string + ".fgx"));
                animationFile.AddAnimationReference(animation);
                animationFile.AddArtToolAndExporterReference(grannyFile);
                animationFile.Save();
                animationFiles.Add(animationFile);
                i++;
            }
            return animationFiles;
        }

        private void OverwriteMeshesButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can overwrite meshes!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            GrannyContext grannyContext = Context.Get<GrannyContext>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (importExportFormatBR2NB2RadioButton.Checked) { 
                openFileDialog.Filter = "br2 files (*.br2)|*.br2";
            }
            else {
                openFileDialog.Filter = "cn6 files (*.cn6)|*.cn6";
            }
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string inputFilename = openFileDialog.FileName;
                if (importExportFormatBR2NB2RadioButton.Checked)
                {
                    BR2FileOps.overwriteMeshes(loadedFile, inputFilename, grannyContext, currentModelIndex);
                } else
                {
                    CN6FileOps.overwriteMeshes(loadedFile, inputFilename, grannyContext, currentModelIndex, vertexFormatComboBox.SelectedIndex);
                }               
                RefreshAppDataWithMessage("MESHES OVERWRITTEN from " + inputFilename);
            }
        }

        private void OverwriteMeshesButtonCN6Click(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can overwrite meshes!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            GrannyContext grannyContext = Context.Get<GrannyContext>();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "cn6 files (*.cn6)|*.cn6";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string meshBR2filename = openFileDialog.FileName;
                CN6FileOps.overwriteMeshes(loadedFile, meshBR2filename, grannyContext, currentModelIndex, vertexFormatComboBox.SelectedIndex);
                RefreshAppDataWithMessage("MESHES OVERWRITTEN from " + meshBR2filename);
            }
        }

        private void ConvertParticlePSBClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "psb files (*.psb)|*.psb";
            openFileDialog.FilterIndex = 0;
            openFileDialog.RestoreDirectory = true;
            HashSet<string> materialsSet = new HashSet<string>();
            HashSet<string> textureSet = new HashSet<string>();
            string inputFilename = "blank";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                inputFilename = openFileDialog.FileName;
                string allText = File.ReadAllText(inputFilename, Encoding.ASCII);
                var names = Regex.Matches(allText, @"([a-zA-Z0-9._]+)");

                foreach (Match name in names)
                {
                    string matchString = name.ToString();

                    if (matchString.Length > 4)
                    { 
                        if (matchString.Contains("."))
                        {
                            textureSet.Add(name.ToString());
                        }
                        else if (matchString.Contains("_"))
                        {
                            materialsSet.Add(name.ToString());
                        }
                    }
                }

            }

            using (TextWriter textWriter = new StreamWriter(inputFilename + ".txs", false))
            {
                foreach (string selectedString in textureSet)
                {
                    textWriter.WriteLine(selectedString);
                }
            }

            using (TextWriter textWriter = new StreamWriter(inputFilename + ".mts", false))
            {
                foreach (string selectedString in materialsSet)
                {
                    textWriter.WriteLine(selectedString);
                }
            }
        }

        private void RemoveNamedBoneButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("Must have model file loaded to remove bone.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (
                MessageBox.Show("Are you sure you want to delete the selected bone? There is no undo.", "Warning!",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                GrannySkeletonWrapper skeletonWrapper = new GrannySkeletonWrapper(loadedFile.Models[currentModelIndex].Skeleton);
                GrannySkeletonInfo skeletonInfo = skeletonWrapper.readSkeletonInfo();

                int boneIdToDelete = bonesComboBox.SelectedIndex;
                skeletonInfo.bones.RemoveAt(boneIdToDelete);
                skeletonWrapper.writeSkeletonInfo(skeletonInfo);
                SaveAction();
                RefreshAppDataWithMessage("BONE REMOVED.");
            }
        }

        private void RemoveAnimationsButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("No file is currently loaded.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(loadedFile);
            fileWrapper.setNumAnimations(0);
            fileWrapper.setNumTrackGroups(0);
            SaveAction();
        }

        private void ConcatenateNA2ButtonClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = Settings.Default.RecentFolder
            };
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string tempFilename = "combined" + rand.Next(100000) + ".na2";
                Settings.Default.RecentFolder = folderBrowserDialog.SelectedPath;
                var files = Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.*", SearchOption.TopDirectoryOnly)
                            .Where(s => s.ToLower().EndsWith(".na2"));

                TextWriter textWriter = new StreamWriter(tempFilename);
                textWriter.WriteLine("// Animation NA2 - Exported from " + GetApplicationNameWithVersionNumber());

                int frameSets = 0;
                foreach (string filename in files)
                {
                    if (filename.LastIndexOf("__") > 0)
                    {
                        frameSets++;
                    }
                }

                textWriter.WriteLine("FrameSets: " + frameSets);

                string outputFilename = null;

                foreach (string filename in files)
                {
                    if (filename.LastIndexOf("__") > 0 ) { 
                        outputFilename = filename.Substring(0, filename.LastIndexOf("__")) + ".na2";
                        StreamReader fileReader = new StreamReader(filename);
                        while (fileReader.Peek() >= 0)
                        {
                            string fileLine = fileReader.ReadLine();
                            if (!fileLine.StartsWith("//") && !fileLine.StartsWith("FrameSets"))
                            {
                                textWriter.Write(fileLine + "\n");
                            }
                        }
                        fileReader.Close();
                    }
                }
                textWriter.Close();

                if (File.Exists(outputFilename))
                {
                    File.Delete(outputFilename);
                }
                    if (File.Exists(tempFilename))
                {
                    File.Copy(tempFilename, outputFilename);
                    File.Delete(tempFilename);
                }
                RefreshAppDataWithMessage("NA2 FILES CONCATENATED.");
            }
        }

        private void ResaveAllGR2FilesInDirAsAnimsClick(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = Settings.Default.RecentFolder
            };
            folderBrowserDialog.ShowNewFolderButton = false;
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                string sourcePath = folderBrowserDialog.SelectedPath;
                Settings.Default.RecentFolder = sourcePath;
                var filenamesList = Directory.GetFiles(sourcePath, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.ToLower().EndsWith(".gr2"));
                var outputDirectory = "fgx_anm_output";
              
                if (loadedFile != null)
                {
                    string loadedFilename = Path.GetFileNameWithoutExtension(loadedFile.Filename);
                    Dictionary<string, string> civ6ShortNameToLongNameLookup = BuildShortNameToLongNameLookup(loadedFilename, filenamesList);
                    MetadataWriter.WriteAssetFile(loadedFile, civ6ShortNameToLongNameLookup, assetClassNameComboBox.Text, dsgComboBox.Text, null, null);
                }

                ResaveAnimationGR2FilesToOutputDirectory(sourcePath, outputDirectory, filenamesList);

                CleardownAllData();

                RefreshAppDataWithMessage("ALL GR2 FILES IN DIRECTORY SAVED AS .fgx/.anm");
            }
        }

        private static Dictionary<string, string> BuildShortNameToLongNameLookup(string animationRoot, IEnumerable<string> filenamesList)
        {

            List<string> civ5AnimationNames = new List<string>();

            foreach (string filename in filenamesList)
            {
                civ5AnimationNames.Add(Path.GetFileNameWithoutExtension(filename).ToLower());
            }

            Dictionary<string, List<string>> civ6ToCiv5ShortNameLookup = GetCiv6ToCiv5ShortNameLookup();
            Dictionary<string, string> civ5ShortNameToLongNameLookup = new Dictionary<string, string>();
            Dictionary<string, string> civ6ShortNameToLongNameLookup = new Dictionary<string, string>();

            foreach (string civ6ShortName in civ6ToCiv5ShortNameLookup.Keys)
            {
                List<string> civ5ShortNameAnimNames = civ6ToCiv5ShortNameLookup[civ6ShortName];

                foreach (string civ5ShortAnimName in civ5ShortNameAnimNames)
                {

                    if (!civ5ShortNameToLongNameLookup.Keys.Contains(civ5ShortAnimName))
                    {
                        foreach (string longAnimName in civ5AnimationNames)
                        {
                            if (longAnimName.Equals((animationRoot + civ5ShortAnimName).ToLower()))
                            {
                                civ5ShortNameToLongNameLookup.Add(civ5ShortAnimName, longAnimName);
                                break;
                            }
                        }
                    }
                    if (civ5ShortNameToLongNameLookup.ContainsKey(civ5ShortAnimName))
                    {
                        civ6ShortNameToLongNameLookup.Add(civ6ShortName, civ5ShortNameToLongNameLookup[civ5ShortAnimName]);
                        break;
                    }
                }
            }

            return civ6ShortNameToLongNameLookup;
        }

        private string FindFileInFolderList(IEnumerable<string> folderList, string filename)
        {
            try { 
                foreach (string currentFolderPath in folderList)
                {
                    string testPath = currentFolderPath + "\\" + filename;
                    if (File.Exists(testPath))
                    {
                        return testPath;
                    }
                }
            } catch (Exception e)
            {
                return null;
            }
            return null;
        }

        private void ResaveAnimationGR2FilesToOutputDirectory(string selectedPath, string outputDirectory, IEnumerable<string> files)
        {
            Directory.CreateDirectory(selectedPath + "\\" + outputDirectory);

            string secondaryAnimPath = "D:\\mod\\Civ5Unpacks\\UnitModels_All\\resaveBatch\\";
            List<string> animationFolders = new List<string> { selectedPath, secondaryAnimPath };

            foreach (string sourceAnimationFilename in files)
            {
                string shortFilename = Path.GetFileName(sourceAnimationFilename).ToLower();

                var animationFilepath = FindFileInFolderList(animationFolders, shortFilename);

                if (animationFilepath != null)
                {
                    IGrannyFile grannyFile = OpenFileAction(animationFilepath);
                    string savefilename = Path.GetFileName(animationFilepath);

                    if (savefilename.ToLower().Contains(".gr2"))
                    {
                        savefilename = savefilename.Replace(".gr2", ".fgx");
                        savefilename = savefilename.Replace(".GR2", ".fgx");
                    }

                    string outputFilename = selectedPath + "\\" + outputDirectory + "\\" + savefilename;

                    grannyFile.Filename = outputFilename;

                    List<IGrannyFile> animationFiles = SaveAnimationsAction(grannyFile, outputFilename);
                    foreach (IGrannyFile animationFile in animationFiles)
                    {
                        MetadataWriter.WriteGeoAnimFile(animationFile, 0, geoClassNameComboBox.Text);
                    }
                }
            }
        }

        private static Dictionary<string, List<string>> GetCiv6ToCiv5ShortNameLookup()
        {
            Dictionary<string, List<string>> civ6ToCiv5ShortNameLookup = new Dictionary<string, List<string>>();
            StreamReader animCodesStreamReader = new StreamReader("Civ6ToCiv5AnimMap.txt");
            while (animCodesStreamReader.Peek() >= 0)
            {
                string currentLine = animCodesStreamReader.ReadLine();

                string[] split1 = currentLine.Split(':');
                string[] split2 = split1[1].Split(';');
                civ6ToCiv5ShortNameLookup.Add(split1[0], split2.ToList());
            }
            return civ6ToCiv5ShortNameLookup;
        }

        private void WriteGeoFileButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can write .geo/.anm file!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            MetadataWriter.WriteGeoAnimFile(loadedFile, currentModelIndex, geoClassNameComboBox.Text);
            RefreshAppDataWithMessage("WRITE .geo/.anm FILE COMPLETE!");
        }

        private void WriteGeometrySetDataClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can write GeometrySet data!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            MetadataWriter.WriteGeometrySet(loadedFile, assetClassNameComboBox.Text);
            RefreshAppDataWithMessage("WRITE GeometrySet XML DATA COMPLETE!");
        }

        private void ClearMaterialsClick(object sender, EventArgs e)
        {   
            if (MessageBox.Show("Are you sure you want to delete all materials? There is no undo.", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                ClearMaterials();
            }
            RefreshAppData();
        }

        public void ClearMaterials()
        {
            List<ListViewItem> list = new List<ListViewItem>();
            foreach (object selectedMaterialItem in materialList.Items)
            {
                MaterialProperties materialProperties = (selectedMaterialItem as ListViewItem).Tag as MaterialProperties;
                foreach (IGrannyMesh mesh in loadedFile.Meshes)
                {
                    mesh.RemoveMaterialBinding(materialProperties.GetMaterial());
                }

                loadedFile.RemoveMaterial(materialProperties.GetMaterial());
                loadedFile.Materials.Remove(materialProperties.GetMaterial());
                list.Add(selectedMaterialItem as ListViewItem);

                foreach (object meshListItem in meshList.Items)
                {
                    if ((meshListItem as ListViewItem).SubItems[1].Text == materialProperties.Name)
                    {
                        (meshListItem as ListViewItem).SubItems[1].Text = "<unassigned>";
                    }
                }
            }
            foreach (ListViewItem materialListItem in list)
            {
                materialList.Items.Remove(materialListItem);
            }
            properties.SelectedObject = null;
        }

        private void DeleteMaterialClick(object sender, EventArgs e)
		{
			if (materialList.SelectedItems.Count > 0 && MessageBox.Show("Are you sure you want to delete the selected materials? There is no undo.", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                DeletedSelectedMaterial();
            }
            RefreshAppData();
		}

        public void DeletedSelectedMaterial()
        {
            List<ListViewItem> materialsToDelete = new List<ListViewItem>();

            foreach (ListViewItem selectedMaterialItem in materialList.Items)
            {
                if (selectedMaterialItem.Selected)
                {
                    MaterialProperties materialProperties = (selectedMaterialItem as ListViewItem).Tag as MaterialProperties;
                    foreach (IGrannyMesh mesh in loadedFile.Meshes)
                    {
                        mesh.RemoveMaterialBinding(materialProperties.GetMaterial());
                    }

                    loadedFile.RemoveMaterial(materialProperties.GetMaterial());
                    loadedFile.Materials.Remove(materialProperties.GetMaterial());
                    materialsToDelete.Add(selectedMaterialItem as ListViewItem);

                    foreach (object meshListItem in meshList.Items)
                    {
                        if ((meshListItem as ListViewItem).SubItems[1].Text == materialProperties.Name)
                        {
                            (meshListItem as ListViewItem).SubItems[1].Text = "<unassigned>";
                        }
                    }
                }
            }

            foreach (ListViewItem materialListItem in materialsToDelete)
            {
                materialList.Items.Remove(materialListItem);
            }
            properties.SelectedObject = null;
        }

        private void AddMaterialClick(object sender, EventArgs e)
        {
            addMaterialButton.Enabled = false;

            if (loadedFile == null)
            {
                return;
            }

            AddNewMaterial();

            RefreshAppData();

            addMaterialButton.Enabled = true;
        }

        public void AddNewMaterial()
        {
            AddNewMaterial(null);
        }

        public IGrannyMaterial AddNewMaterial(string inputMaterialName)
        {
            List<string> materialNames = new List<string>();
            foreach (IGrannyMaterial currentMaterial in loadedFile.Materials)
            {
                materialNames.Add(currentMaterial.Name);
            }

            IGrannyMaterial materialFromTemplateFile = GetMaterialFromTemplateFile();

            if (inputMaterialName != null)
            {
                materialFromTemplateFile.Name = inputMaterialName;
            }
            else
            {
                string newMaterialName = "Default_Material";
                for (int i = 1; i < 1000000; i++)
                {
                    string tryMaterialName = "Default_Material_" + i;
                    if (!materialNames.Contains(tryMaterialName))
                    {
                        newMaterialName = tryMaterialName;
                        break;
                    }
                }
                materialFromTemplateFile.Name = newMaterialName;
            }

            loadedFile.AddMaterialReference(materialFromTemplateFile);
            AddMaterialToListbox(materialFromTemplateFile);

            return materialFromTemplateFile;
        }

        public IGrannyMaterial GetMaterialFromTemplateFile()
        {
        	if (loadedFile == null)
        	{
        		return null;
        	}
        	GrannyContext grannyContext = Context.Get<GrannyContext>();
        	string tempPath = Path.GetTempPath();
        	string text = tempPath + "temptemplate";
        	text += rand.Next(10000000).ToString();
        	text += rand.Next(10000000).ToString();
        	text += rand.Next(10000000).ToString();
        	text += rand.Next(10000000).ToString();
        	text += ".fgx";
        	TempFiles.Add(text);

            File.Copy(modelTemplateFilename, text);
        	IGrannyFile grannyFile = grannyContext.LoadGrannyFile(text);
            IGrannyMaterial templateMaterial = null;

            foreach (IGrannyMaterial material in grannyFile.Materials)
        	{
                if (material.Name == "Default_Material")
                {
                    templateMaterial = material;
                }
        	}

            if (templateMaterial == null)
            {
                templateMaterial = grannyFile.Materials[0];
            }
            return templateMaterial;
        }

        private void ExportAllModels(object sender, EventArgs e)
        {
            if (importExportFormatBR2NB2RadioButton.Checked)
            {
                ExportNB2ButtonClick(sender, e);
            } else
            {
                ExportCN6ButtonClick(sender, e);
            }
        }

        private void ExportModel(object sender, EventArgs e)
        {
            if (importExportFormatBR2NB2RadioButton.Checked)
            {
                ExportNB2CurrentModelButtonClick(sender, e);
            }
            else
            {
                ExportCN6CurrentModelButtonClick(sender, e);
            }
        }

        private void ImportModel(object sender, EventArgs e)
        {
            if (importExportFormatBR2NB2RadioButton.Checked)
            {
                OpenBR2ButtonClick(sender, e);
            }
            else
            {
                OpenCN6ButtonClick(sender, e);
            }
        }

        private void ExportNB2ButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can export to NB2!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            NB2NA2FileOps.exportAllModelsToNB2(loadedFile);
            RefreshAppDataWithMessage("EXPORT TO NB2 COMPLETE.");
        }

        private void ExportCN6ButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can export to CN6!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            CN6FileOps.exportAllModelsToCN6(loadedFile);
            RefreshAppDataWithMessage("EXPORT TO CN6 COMPLETE.");
        }

        private void ExportNB2CurrentModelButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can export to NB2!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            NB2NA2FileOps.exportNB2(loadedFile, currentModelIndex);
            RefreshAppDataWithMessage("EXPORT TO NB2 COMPLETE.");
        }

        private void ExportCN6CurrentModelButtonClick(object sender, EventArgs e)
        {
            if (loadedFile == null)
            {
                MessageBox.Show("You need to open a file before you can export to CN6!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            CN6FileOps.cn6Export(loadedFile, currentModelIndex);
            RefreshAppDataWithMessage("EXPORT TO CN6 COMPLETE.");
        }

        //Make Model Template
        private void MakeTemplateButtonClick(object sender, EventArgs e)
        {
            string filename = sourceTemplatePath + "IMP_Chateau_Main.fgx";

            IGrannyFile file = form.OpenFileAsTempFileCopy(filename, "tempimport");

            GrannyMeshWrapper meshWrapper = new GrannyMeshWrapper(file.Meshes[0]);
            meshWrapper.setName("BLANK_MESH");
            meshWrapper.setNumVertices(1);
            meshWrapper.setNumIndices16(1);
            //meshWrapper.setGroup0TriCount(1);
            meshWrapper.setGroupTriCountForIndex(0, 1);
            meshWrapper.setNumMaterialBindings(1);

            foreach(IGrannyMaterial material in file.Meshes[0].MaterialBindings)
            {
                if (material.Name == "IMP_Chateau")
                {
                    GrannyMaterialWrapper materialWrapper = new GrannyMaterialWrapper(material);
                    materialWrapper.setName("Default_Material");
                }
            }

            GrannyModelWrapper modelWrapper = new GrannyModelWrapper(file.Models[0]);
            GrannySkeletonWrapper skeletonWrapper = new GrannySkeletonWrapper(modelWrapper.wrappedModel.Skeleton);

            modelWrapper.setNumMeshBindings(1);
            modelWrapper.setName("BLANK_MODEL");

            skeletonWrapper.setNumBones(1);
            skeletonWrapper.setName("BLANK_SKELETON");

            GrannyFileWrapper fileWrapper = new GrannyFileWrapper(file);
            fileWrapper.setNumMeshes(1);
            fileWrapper.setNumVertexDatas(1);
            fileWrapper.setNumSkeletons(1);
            fileWrapper.setNumTriTopologies(1);
            //fileWrapper.setNumMaterials(1);
            fileWrapper.setNumTextures(1);

            //file.Meshes.Clear();

            SaveAsAction(file, sourceTemplatePath + "model_template_3uv.fgx", true);
        }

        public void SelectMesh(int meshIndex)
        {
            meshList.Items[meshIndex].Selected = true;
            meshList.Select();
        }

        public void SelectMaterial(int materialIndex)
        {
            materialList.Items[materialIndex].Selected = true;
            materialList.Select();
        }

        public void UpdateMaterialBindings()
		{
            if (meshList.SelectedItems.Count == 0 || meshList.SelectedItems.Count > 1)
            {
            	return;
            }

            IGrannyMesh currentMesh = meshList.SelectedItems[0].Tag as IGrannyMesh;

            int assignedMaterials = 0;
            for (int i = 0; i < triangleGroupsList.Items.Count; i++)
            {
                if (triangleGroupsList.Items[i].SubItems[1].Text != "<unassigned>")
                {
                    assignedMaterials += 1;
                }
            }

            if (assignedMaterials == triangleGroupsList.Items.Count) { 

                IGrannyMaterial[] materialBindingsArray = currentMesh.MaterialBindings.ToArray();

                for (int i = 0; i < materialBindingsArray.Length; i++)
                {
              	        IGrannyMaterial kMaterial = materialBindingsArray[i];
               	        currentMesh.RemoveMaterialBinding(kMaterial);
                }

                for (int i = 0; i < triangleGroupsList.Items.Count; i++)
                {
                    for (int k = 0; k < loadedFile.Materials.Count; k++)
                    {
                        IGrannyMaterial checkMaterial = loadedFile.Materials[k];
                        string checkMaterialName = checkMaterial.Name + " (" + checkMaterial.typeName + ")";
                        if (triangleGroupsList.Items[i].SubItems[1].Text.Equals(checkMaterialName))
                        {
                            currentMesh.AddMaterialBinding(checkMaterial);
                            break;
                        }
                    }
                }
            }
        }

		private void PropertiesPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.Label == "Name" && properties.SelectedObject is MaterialProperties)
			{
				bool flag = true;
				foreach (ListViewItem listViewItem in materialList.Items)
				{
					if (listViewItem.Text == (string)e.ChangedItem.Value && properties.SelectedObject as MaterialProperties != listViewItem.Tag as MaterialProperties)
					{
						(properties.SelectedObject as MaterialProperties).Name = (e.OldValue as string);
						MessageBox.Show("A material of this name already exists.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
						flag = false;
					}
				}
				if (flag)
				{
					foreach (ListViewItem listViewItem2 in materialList.Items)
					{
						if (listViewItem2.Text == (string)e.OldValue)
						{
							listViewItem2.Text = (e.ChangedItem.Value as string);
						}
					}
					foreach (ListViewItem listViewItem3 in meshList.Items)
					{
						if (listViewItem3.SubItems[1].Text == (string)e.OldValue)
						{
							listViewItem3.SubItems[1].Text = (e.ChangedItem.Value as string);
						}
					}
				}
			}
		}

		private void MeshListSelectedIndexChanged(object sender, EventArgs e)
		{
			List<object> list = new List<object>();
			if (meshList.SelectedItems.Count > 0)
			{
				foreach (object current in meshList.SelectedItems)
				{
					IGrannyMesh grannyMesh = (current as ListViewItem).Tag as IGrannyMesh;
					if (grannyMesh != null)
					{
						list.Add(grannyMesh);
                        RefreshTriangleGroupsList(grannyMesh);

                    }
				}
				properties.SelectedObjects = list.ToArray();
			}
		}

        private void RefreshTriangleGroupsList(IGrannyMesh mesh)
        {
            triangleGroupsList.Items.Clear();
            for (int j = 0; j < mesh.TriangleMaterialGroups.Count; j++) 
            {
                IGrannyTriMaterialGroup triangleGroup = mesh.TriangleMaterialGroups[j];
                triangleGroupsList.Items.Add(j.ToString() + " (" + triangleGroup.TriFirst + " - " + (triangleGroup.TriFirst + triangleGroup.TriCount - 1) + ")");
                if (mesh.MaterialBindings.ElementAtOrDefault(triangleGroup.MaterialIndex) != null)
                {
                    IGrannyMaterial boundMaterial = mesh.MaterialBindings[triangleGroup.MaterialIndex];
                    int boundMaterialIndex = -1;
                    for (int k = 0; k < loadedFile.Materials.Count; k++)
                    {
                        IGrannyMaterial checkMaterial = loadedFile.Materials[k];
                        if (boundMaterial.Name.Equals(checkMaterial.Name) && boundMaterial.typeName.Equals(checkMaterial.typeName)) {
                            boundMaterialIndex = k;
                            break;
                        }
                    }
                    triangleGroupsList.Items[triangleGroupsList.Items.Count - 1].SubItems.Add(boundMaterial.Name + " (" + boundMaterial.typeName + ")").Tag = boundMaterialIndex;
                } else
                {
                    triangleGroupsList.Items[triangleGroupsList.Items.Count - 1].SubItems.Add("<unassigned>");
                }
            }
        }
        
        private void MaterialListSelectedIndexChanged(object sender, EventArgs e)
        {
            List<object> list = new List<object>();
            if (materialList.SelectedItems.Count > 0)
            {
                foreach (object current in materialList.SelectedItems)
                {
                    MaterialProperties indieMaterial = (current as ListViewItem).Tag as MaterialProperties;
                    if (indieMaterial != null)
                    {
                        list.Add(indieMaterial);
                    }
                    
                }
                properties.SelectedObjects = list.ToArray();
            }
        }

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

        private void ModelListCheckChanged(object sender, ItemCheckEventArgs e)
        {
            SelectModel(e.Index);
        }

        public void SelectModel(int inputIndex)
        {
            int preCurrentModelIndex = currentModelIndex;

            // if we have the lastItem set as checked, and it is different
            // item than the one that fired the event, uncheck it
            if (lastModelListItemChecked != null && lastModelListItemChecked.Checked
                && lastModelListItemChecked != modelList.Items[inputIndex])
            {
                // uncheck the last item and store the new one
                lastModelListItemChecked.Checked = false;
            }

            // store current item
            lastModelListItemChecked = modelList.Items[inputIndex];

            currentModelIndex = lastModelListItemChecked.Index;

            if (preCurrentModelIndex != currentModelIndex)
            {
                RefreshAppData();
            }
        }

        private void InitializeComponent()
		{
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(CivNexusSixApplicationForm));
            masterSplitContainer = new SplitContainer();
            leftHandSplitContainer = new SplitContainer();
            mainButtonPanel = new Panel();
            openFBXButton = new Button();
            overwriteBR2Button = new Button();
            openButton = new Button();
            saveButton = new Button();
            saveAsButton = new Button();
            saveAnimationButton = new Button();
            mainTabControl = new TabControl();
            editModelTabPage = new TabPage();
            editMaterialsTabPage = new TabPage();
            editModelContainer = new SplitContainer();
            editMaterialsContainer = new SplitContainer();
            meshList = new ListView();
            triangleGroupsList = new ListViewWithComboBox();
            MeshNameColumnHeader = new ColumnHeader();
            MaterialColumnHeader = new ColumnHeader();
            TriangleGroupCountColumnHeader = new ColumnHeader();
            TriangleGroupColumnHeader = new ColumnHeader();
            materialListPanelContainer = new SplitContainer();
            materialList = new ListView();
            materialNameHeader = new ColumnHeader();
            materialTypeHeader = new ColumnHeader();
            materialButtonsPanel = new Panel();
            addMaterialButton = new Button();
            deleteMaterialButton = new Button();
            clearMaterialsButton = new Button();
            animationsTabControl = new TabControl();
            grannyAnimsTabPage = new TabPage();
            animList = new ListView();
            anim = new ColumnHeader();
            timeSlice = new ColumnHeader();
            otherActionsTabPage = new TabPage();
            nodeLabel = new Label();
            nodeTextBox = new TextBox();
            openBR2Button = new Button();
            fpsLevel = new Label();
            fpsTextBox = new TextBox();
            concatenateNA2Button = new Button();
            angleLabel = new Label();
            axisLabel = new Label();
            axisComboBox = new ComboBox();
            geoClassNameComboBox = new ComboBox();
            assetClassNameComboBox = new ComboBox();
            materialClassNameComboBox = new ComboBox();
            dsgComboBox = new ComboBox();
            multiModelAssetCheckBox = new CheckBox();
            vertexFormatComboBox = new ComboBox();
            endTimeTextBoxLabel = new Label();
            startTimeTextBoxLabel = new Label();
            geoClassNameLabel = new Label();
            assetClassNameLabel = new Label();
            materialClassNameLabel = new Label();
            dsgLabel = new Label();
            multiModelAssetCheckBoxLabel = new Label();
            textureClassLabel = new Label();
            endTimeTextBox = new TextBox();
            startTimeTextBox = new TextBox();
            exportNA2Button = new Button();
            exportBR2Button = new Button();
            rescaleBoneNameLabel = new Label();
            rescaleNamedBoneButton = new Button();
            bonesComboBox = new ComboBox();
            textureClassComboBox = new ComboBox();
            rescaleFactorLabel = new Label();
            insertAdjustmentBoneButton = new Button();
            angleTextBox = new TextBox();
            rescaleFactorTextBox = new TextBox();
            xPositionLabel = new Label();
            yPositionLabel = new Label();
            zPositionLabel = new Label();
            xPositionTextBox = new TextBox();
            yPositionTextBox = new TextBox();
            zPositionTextBox = new TextBox();
            makeTemplateButton = new Button();
            removeAnimationsButton = new Button();
		    removeNamedBoneButton = new Button();
            resaveAllFBXAsAnimsButton = new Button();
            vertexFormatLabel = new Label();
            importExportFiletypesLabel = new Label();
            importExportFormatBR2NB2RadioButton = new RadioButton();
            importExportFormatCN6RadioButton = new RadioButton();
            //useUnitTemplateRadioButton = new RadioButton();
            writeGeoFileButton = new Button();
            exportCurrentModelButton = new Button();
            overwriteMeshesButton = new Button();
            exportAllModelsButton = new Button();
            processTextureButton = new Button();
            processTexturesInDirButton = new Button();
            batchConversionButton = new Button();
            selectModelTabPage = new TabPage();
            furtherActionsTabPage = new TabPage();
            modelList = new ListView();
            modelName = new ColumnHeader();
            panel1 = new Panel();
            headerFilenameLabel = new Label();
            viewButton = new Button();
            fileInfoGroupBox = new GroupBox();
            fileInfoTextBox = new RichTextBox();
            properties = new PropertyGrid();
            animation = new ColumnHeader();
            duration = new ColumnHeader();
            materialTypeColumnHeader = new ColumnHeader();
            materialNameColumnHeader = new ColumnHeader();
            viewButtonToolTip = new ToolTip(components);
            br2ImportButton = new Button();
            loadStringDatabaseButton = new Button();
            masterSplitContainer.BeginInit();
            masterSplitContainer.Panel1.SuspendLayout();
            masterSplitContainer.Panel2.SuspendLayout();
            masterSplitContainer.SuspendLayout();
            leftHandSplitContainer.BeginInit();
            leftHandSplitContainer.Panel1.SuspendLayout();
            leftHandSplitContainer.Panel2.SuspendLayout();
            leftHandSplitContainer.SuspendLayout();
            mainButtonPanel.SuspendLayout();
            mainTabControl.SuspendLayout();
            editModelTabPage.SuspendLayout();
            editMaterialsTabPage.SuspendLayout();
            editModelContainer.BeginInit();
            editModelContainer.Panel1.SuspendLayout();
            editModelContainer.SuspendLayout();
            editMaterialsContainer.BeginInit();
            editMaterialsContainer.Panel1.SuspendLayout();
            editMaterialsContainer.SuspendLayout();
            materialListPanelContainer.BeginInit();
            materialListPanelContainer.Panel1.SuspendLayout();
            materialListPanelContainer.SuspendLayout();
            materialButtonsPanel.SuspendLayout();
            animationsTabControl.SuspendLayout();
            grannyAnimsTabPage.SuspendLayout();
            otherActionsTabPage.SuspendLayout();
            selectModelTabPage.SuspendLayout();
            furtherActionsTabPage.SuspendLayout();
            panel1.SuspendLayout();
            fileInfoGroupBox.SuspendLayout();
            SuspendLayout();
 
            masterSplitContainer.Dock = DockStyle.Fill;
            masterSplitContainer.Location = new Point(0, 0);
            masterSplitContainer.Name = "masterSplitContainer";
            masterSplitContainer.Panel1.Controls.Add(leftHandSplitContainer);
            masterSplitContainer.Panel2.Controls.Add(panel1);
            masterSplitContainer.Panel2.Controls.Add(fileInfoGroupBox);
            masterSplitContainer.Panel2.Controls.Add(properties);
            masterSplitContainer.Size = new Size(1008, 618);
            masterSplitContainer.SplitterDistance = 492;
            masterSplitContainer.TabIndex = 1;
            // 
            // leftHandSplitContainer
            // 
            leftHandSplitContainer.Dock = DockStyle.Fill;
            leftHandSplitContainer.Location = new Point(0, 0);
            leftHandSplitContainer.Name = "leftHandSplitContainer";
            leftHandSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // leftHandSplitContainer.Panel1
            // 
            leftHandSplitContainer.Panel1.Controls.Add(mainButtonPanel);
            // 
            // leftHandSplitContainer.Panel2
            // 
            leftHandSplitContainer.Panel2.Controls.Add(mainTabControl);
            leftHandSplitContainer.Size = new Size(492, 618);
            leftHandSplitContainer.SplitterDistance = 42;
            leftHandSplitContainer.TabIndex = 2;
            // 
            // mainButtonPanel
            // 
            mainButtonPanel.Controls.Add(openFBXButton);
            mainButtonPanel.Controls.Add(overwriteBR2Button);
            mainButtonPanel.Controls.Add(openButton);
            mainButtonPanel.Controls.Add(saveButton);
            mainButtonPanel.Controls.Add(saveAsButton);
            mainButtonPanel.Controls.Add(saveAnimationButton);
            mainButtonPanel.Dock = DockStyle.Fill;
            mainButtonPanel.Location = new Point(0, 0);
            mainButtonPanel.Name = "mainButtonPanel";
            mainButtonPanel.Size = new Size(492, 42);
            mainButtonPanel.TabIndex = 9;
            // 
            // openFBXButton
            // 
            openFBXButton.Location = new Point(155, 5);
            openFBXButton.Name = "openFBXButton";
            openFBXButton.Size = new Size(87, 33);
            openFBXButton.TabIndex = 7;
            openFBXButton.Text = "Import";
            openFBXButton.UseVisualStyleBackColor = true;
            openFBXButton.Click += ImportModel;
            // 
            // overwriteBR2Button
            // 
            overwriteBR2Button.Location = new Point(67, 5);
            overwriteBR2Button.Name = "overwriteBR2Button";
            overwriteBR2Button.Size = new Size(82, 33);
            overwriteBR2Button.TabIndex = 6;
            overwriteBR2Button.Text = "Overwite";
            overwriteBR2Button.UseVisualStyleBackColor = true;
            overwriteBR2Button.Click += OverwriteMeshesButtonClick;
            // 
            // openButton
            // 
            openButton.Location = new Point(4, 5);
            openButton.Name = "openButton";
            openButton.Size = new Size(57, 33);
            openButton.TabIndex = 2;
            openButton.Text = "Open";
            openButton.UseVisualStyleBackColor = true;
            openButton.Click += OpenButtonClick;
            // 
            // saveButton
            // 
            saveButton.Location = new Point(248, 5);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(66, 33);
            saveButton.TabIndex = 3;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += SaveButtonClick;
            // 
            // saveAsButton
            // 
            saveAsButton.Location = new Point(320, 5);
            saveAsButton.Name = "saveAsButton";
            saveAsButton.Size = new Size(70, 33);
            saveAsButton.TabIndex = 4;
            saveAsButton.Text = "Save As";
            saveAsButton.UseVisualStyleBackColor = true;
            saveAsButton.Click += SaveAsButtonClick;
            // 
            // saveAnimationButton
            // 
            saveAnimationButton.Location = new Point(396, 5);
            saveAnimationButton.Name = "saveAnimationButton";
            saveAnimationButton.Size = new Size(90, 33);
            saveAnimationButton.TabIndex = 5;
            saveAnimationButton.Text = "Save Anim.";
            saveAnimationButton.UseVisualStyleBackColor = true;
            saveAnimationButton.Click += SaveAnimationClick;
            // 
            // mainTabControl
            // 
            mainTabControl.Controls.Add(editModelTabPage);
            mainTabControl.Controls.Add(editMaterialsTabPage);
            mainTabControl.Controls.Add(otherActionsTabPage);
            mainTabControl.Controls.Add(furtherActionsTabPage);
            mainTabControl.Controls.Add(selectModelTabPage);


            mainTabControl.Dock = DockStyle.Fill;
            mainTabControl.Location = new Point(0, 0);
            mainTabControl.Name = "mainTabControl";
            mainTabControl.SelectedIndex = 0;
            mainTabControl.Size = new Size(492, 572);
            mainTabControl.TabIndex = 2;

            editModelTabPage.Controls.Add(editModelContainer);
            editModelTabPage.Location = new Point(4, 25);
            editModelTabPage.Name = "editModelTabPage";
            editModelTabPage.Padding = new Padding(3);
            editModelTabPage.Size = new Size(484, 543);
            editModelTabPage.TabIndex = 0;
            editModelTabPage.Text = "Edit Mesh Material Bindings";
            editModelTabPage.UseVisualStyleBackColor = true;

            editMaterialsTabPage.Controls.Add(editMaterialsContainer);
            editMaterialsTabPage.Location = new Point(4, 25);
            editMaterialsTabPage.Name = "editMaterialsTabPage";
            editMaterialsTabPage.Padding = new Padding(3);
            editMaterialsTabPage.Size = new Size(484, 543);
            editMaterialsTabPage.TabIndex = 1;
            editMaterialsTabPage.Text = "Edit Materials";
            editMaterialsTabPage.UseVisualStyleBackColor = true;

            editModelContainer.Dock = DockStyle.Left;
            editModelContainer.Location = new Point(3, 3);
            editModelContainer.Name = "editModelContainer";
            editModelContainer.Orientation = Orientation.Horizontal;
            editModelContainer.Size = new Size(480, 537);
            editModelContainer.SplitterDistance = 537;
            editModelContainer.TabIndex = 0;
            editModelContainer.Panel1.Controls.Add(meshList);
            editModelContainer.Panel1.Controls.Add(triangleGroupsList);


            editMaterialsContainer.Dock = DockStyle.Left;
            editMaterialsContainer.Location = new Point(3, 3);
            editMaterialsContainer.Name = "editMaterialsContainer";
            editMaterialsContainer.Orientation = Orientation.Horizontal;
            editMaterialsContainer.Size = new Size(480, 537);
            editMaterialsContainer.SplitterDistance = 537;
            editMaterialsContainer.TabIndex = 0;
            editMaterialsContainer.Panel1.Controls.Add(materialListPanelContainer);

            // 
            // meshList
            // 
            meshList.Columns.AddRange(new ColumnHeader[] {MeshNameColumnHeader, TriangleGroupCountColumnHeader});
            meshList.Dock = DockStyle.Top;
            meshList.FullRowSelect = true;
            meshList.GridLines = true;
            meshList.Location = new Point(0, 0);
            meshList.MultiSelect = false;
            meshList.Name = "meshList";
            meshList.Size = new Size(480, 250);
            meshList.TabIndex = 2;
            meshList.UseCompatibleStateImageBehavior = false;
            meshList.View = View.Details;
            meshList.SelectedIndexChanged += MeshListSelectedIndexChanged;

            TriangleGroupColumnHeader.Tag = "Triangle Group";
            TriangleGroupColumnHeader.Text = "Triangle Group";
            TriangleGroupColumnHeader.Width = 120;

            MaterialColumnHeader.Text = "Material";
            MaterialColumnHeader.Width = 356;

            triangleGroupsList.Columns.AddRange(new ColumnHeader[] { TriangleGroupColumnHeader, MaterialColumnHeader });
            triangleGroupsList.Dock = DockStyle.Bottom;
            triangleGroupsList.FullRowSelect = true;
            triangleGroupsList.GridLines = true;
            triangleGroupsList.Location = new Point(0, 260);
            triangleGroupsList.MultiSelect = false;
            triangleGroupsList.Name = "triangleGroupsList";
            triangleGroupsList.Size = new Size(480, 250);
            triangleGroupsList.TabIndex = 3;
            triangleGroupsList.UseCompatibleStateImageBehavior = false;
            triangleGroupsList.View = View.Details;
            //triangleGroupsList.SelectedIndexChanged += TriangleGroupsSelectedIndexChanged;

            // 
            // MeshName
            // 
            MeshNameColumnHeader.Tag = "Mesh Name";
            MeshNameColumnHeader.Text = "Mesh Name";
            MeshNameColumnHeader.Width = 326;
            // 
            // Material
            // 
            TriangleGroupCountColumnHeader.Text = "Num Triangle Groups";
            TriangleGroupCountColumnHeader.Width = 150;

            materialListPanelContainer.Dock = DockStyle.Top;
            materialListPanelContainer.Location = new Point(0, 0);
            materialListPanelContainer.Name = "materialListPanelContainer";
            materialListPanelContainer.Orientation = Orientation.Horizontal;
            materialListPanelContainer.Panel1.Controls.Add(materialList);
            materialListPanelContainer.Panel1.Controls.Add(materialButtonsPanel);
            materialListPanelContainer.SplitterDistance = 537;
            materialListPanelContainer.Size = new Size(480, 537);

            materialList.Columns.AddRange(new ColumnHeader[] {materialNameHeader, materialTypeHeader});
            materialList.Dock = DockStyle.Fill;
            materialList.FullRowSelect = true;
            materialList.GridLines = true;
            materialList.Location = new Point(0, 0);
            materialList.Name = "materialList";
            materialList.Size = new Size(480, 573);
            materialList.TabIndex = 6;
            materialList.UseCompatibleStateImageBehavior = false;
            materialList.View = View.Details;
            materialList.SelectedIndexChanged += MaterialListSelectedIndexChanged;
            // 
            // materialNameHeader
            // 
            materialNameHeader.Text = "Material";
            materialNameHeader.Width = 180;
            // 
            // materialTypeHeader
            // 
            materialTypeHeader.Text = "Type";
            materialTypeHeader.Width = 200;
            // 
            // materialButtonsPanel
            // 
            materialButtonsPanel.Controls.Add(addMaterialButton);
            materialButtonsPanel.Controls.Add(deleteMaterialButton);
            materialButtonsPanel.Controls.Add(clearMaterialsButton);
            materialButtonsPanel.Dock = DockStyle.Bottom;
            materialButtonsPanel.Location = new Point(0, 131);
            materialButtonsPanel.Name = "materialButtonsPanel";
            materialButtonsPanel.Size = new Size(480, 31);
            materialButtonsPanel.TabIndex = 7;
            // 
            // addMaterial
            // 
            addMaterialButton.Location = new Point(12, 5);
            addMaterialButton.Name = "addMaterial";
            addMaterialButton.Size = new Size(75, 23);
            addMaterialButton.TabIndex = 1;
            addMaterialButton.Text = "Add";
            addMaterialButton.UseVisualStyleBackColor = true;
            addMaterialButton.Click += AddMaterialClick;
            // 
            // deleteMaterial
            // 
            deleteMaterialButton.Location = new Point(95, 5);
            deleteMaterialButton.Name = "deleteMaterial";
            deleteMaterialButton.Size = new Size(75, 23);
            deleteMaterialButton.TabIndex = 0;
            deleteMaterialButton.Text = "Delete";
            deleteMaterialButton.UseVisualStyleBackColor = true;
            deleteMaterialButton.Click += DeleteMaterialClick;
            // 
            // clearMaterials
            // 
            clearMaterialsButton.Location = new Point(178, 5);
            clearMaterialsButton.Name = "deleteMaterial";
            clearMaterialsButton.Size = new Size(75, 23);
            clearMaterialsButton.TabIndex = 0;
            clearMaterialsButton.Text = "Delete All";
            clearMaterialsButton.UseVisualStyleBackColor = true;
            clearMaterialsButton.Click += ClearMaterialsClick;
            // 
            // animationsTabControl
            // 
            animationsTabControl.Controls.Add(grannyAnimsTabPage);
            animationsTabControl.Dock = DockStyle.Fill;
            animationsTabControl.Location = new Point(0, 0);
            animationsTabControl.Name = "animationsTabControl";
            animationsTabControl.SelectedIndex = 0;
            animationsTabControl.Size = new Size(480, 172);
            animationsTabControl.TabIndex = 1;
            // 
            // grannyAnimsTabPage
            // 
            grannyAnimsTabPage.Controls.Add(animList);
            grannyAnimsTabPage.Location = new Point(4, 25);
            grannyAnimsTabPage.Name = "grannyAnimsTabPage";
            grannyAnimsTabPage.Padding = new Padding(3);
            grannyAnimsTabPage.Size = new Size(472, 143);
            grannyAnimsTabPage.TabIndex = 0;
            grannyAnimsTabPage.Text = "Granny Anims";
            grannyAnimsTabPage.UseVisualStyleBackColor = true;
            // 
            // animList
            // 
            animList.Columns.AddRange(new ColumnHeader[] {anim, timeSlice});
            animList.Dock = DockStyle.Fill;
            animList.FullRowSelect = true;
            animList.GridLines = true;
            animList.Location = new Point(3, 3);
            animList.Name = "animList";
            animList.Size = new Size(466, 137);
            animList.TabIndex = 10;
            animList.UseCompatibleStateImageBehavior = false;
            animList.View = View.Details;
            // 
            // anim
            // 
            anim.Text = "Animation";
            anim.Width = 100;
            // 
            // timeSlice
            // 
            timeSlice.Text = "Time Slice";
            timeSlice.Width = 98;



            // 
            // otherActionsTabPage
            // 
            otherActionsTabPage.Controls.Add(nodeLabel);
            otherActionsTabPage.Controls.Add(nodeTextBox);
            otherActionsTabPage.Controls.Add(openBR2Button);
            otherActionsTabPage.Controls.Add(fpsLevel);
            otherActionsTabPage.Controls.Add(fpsTextBox);
            otherActionsTabPage.Controls.Add(concatenateNA2Button);
            otherActionsTabPage.Controls.Add(angleLabel);
            otherActionsTabPage.Controls.Add(axisLabel);
            otherActionsTabPage.Controls.Add(axisComboBox);
            otherActionsTabPage.Controls.Add(geoClassNameComboBox);
            otherActionsTabPage.Controls.Add(vertexFormatComboBox);
            otherActionsTabPage.Controls.Add(endTimeTextBoxLabel);
            otherActionsTabPage.Controls.Add(startTimeTextBoxLabel);
            otherActionsTabPage.Controls.Add(geoClassNameLabel);
            otherActionsTabPage.Controls.Add(endTimeTextBox);
            otherActionsTabPage.Controls.Add(startTimeTextBox);
            otherActionsTabPage.Controls.Add(exportNA2Button);
            otherActionsTabPage.Controls.Add(rescaleBoneNameLabel);
            otherActionsTabPage.Controls.Add(rescaleNamedBoneButton);
            otherActionsTabPage.Controls.Add(bonesComboBox);
            otherActionsTabPage.Controls.Add(rescaleFactorLabel);
            otherActionsTabPage.Controls.Add(insertAdjustmentBoneButton);
            otherActionsTabPage.Controls.Add(exportBR2Button);
            otherActionsTabPage.Controls.Add(angleTextBox);
            otherActionsTabPage.Controls.Add(rescaleFactorTextBox);
            otherActionsTabPage.Controls.Add(xPositionLabel);
            otherActionsTabPage.Controls.Add(yPositionLabel);
            otherActionsTabPage.Controls.Add(zPositionLabel);
            otherActionsTabPage.Controls.Add(xPositionTextBox);
            otherActionsTabPage.Controls.Add(yPositionTextBox);
            otherActionsTabPage.Controls.Add(zPositionTextBox);
            otherActionsTabPage.Controls.Add(makeTemplateButton);
            otherActionsTabPage.Controls.Add(removeAnimationsButton);
		    otherActionsTabPage.Controls.Add(removeNamedBoneButton);
            otherActionsTabPage.Controls.Add(resaveAllFBXAsAnimsButton);
            otherActionsTabPage.Controls.Add(vertexFormatLabel);
            otherActionsTabPage.Controls.Add(importExportFiletypesLabel);
            otherActionsTabPage.Controls.Add(importExportFormatBR2NB2RadioButton);
            otherActionsTabPage.Controls.Add(importExportFormatCN6RadioButton);
            otherActionsTabPage.Controls.Add(writeGeoFileButton);
            otherActionsTabPage.Controls.Add(loadStringDatabaseButton);
            otherActionsTabPage.Controls.Add(exportCurrentModelButton);
            otherActionsTabPage.Controls.Add(overwriteMeshesButton);
            otherActionsTabPage.Controls.Add(exportAllModelsButton);
            otherActionsTabPage.Location = new Point(4, 25);
            otherActionsTabPage.Name = "otherActionsTabPage";
            otherActionsTabPage.Padding = new Padding(3);
            otherActionsTabPage.Size = new Size(484, 543);
            otherActionsTabPage.TabIndex = 2;
            otherActionsTabPage.Text = "Additional Actions";
            otherActionsTabPage.UseVisualStyleBackColor = true;


            furtherActionsTabPage.Location = new Point(4, 25);
            furtherActionsTabPage.Name = "texturesTabPage";
            furtherActionsTabPage.Size = new Size(484, 543);
            furtherActionsTabPage.TabIndex = 3;
            furtherActionsTabPage.Text = "Further Actions";
            furtherActionsTabPage.UseVisualStyleBackColor = true;
            furtherActionsTabPage.Controls.Add(processTextureButton);
            furtherActionsTabPage.Controls.Add(processTexturesInDirButton);
            furtherActionsTabPage.Controls.Add(batchConversionButton);
            furtherActionsTabPage.Controls.Add(textureClassComboBox);
            furtherActionsTabPage.Controls.Add(textureClassLabel);
            furtherActionsTabPage.Controls.Add(assetClassNameComboBox);
            furtherActionsTabPage.Controls.Add(assetClassNameLabel);
            furtherActionsTabPage.Controls.Add(materialClassNameComboBox);
            furtherActionsTabPage.Controls.Add(materialClassNameLabel);
            furtherActionsTabPage.Controls.Add(dsgComboBox);
            furtherActionsTabPage.Controls.Add(dsgLabel);
            furtherActionsTabPage.Controls.Add(multiModelAssetCheckBox);
            furtherActionsTabPage.Controls.Add(multiModelAssetCheckBoxLabel);

            selectModelTabPage.Controls.Add(modelList);
            selectModelTabPage.Location = new Point(4, 25);
            selectModelTabPage.Name = "selectModelTabPage";
            selectModelTabPage.Size = new Size(484, 543);
            selectModelTabPage.TabIndex = 4;
            selectModelTabPage.Text = "Select Model";
            selectModelTabPage.UseVisualStyleBackColor = true;

            nodeLabel.AutoSize = true;
            nodeLabel.Location = new Point(6, 328);
            nodeLabel.Name = "nodeLabel";
            nodeLabel.Size = new Size(42, 17);
            nodeLabel.TabIndex = 47;
            nodeLabel.Text = "Node";
            // 
            // nodeTextBox
            // 
            nodeTextBox.Location = new Point(52, 325);
            nodeTextBox.Name = "nodeTextBox";
            nodeTextBox.Size = new Size(204, 22);
            nodeTextBox.TabIndex = 46;
            // 
            // openBR2Button
            // 
            openBR2Button.Location = new Point(6, 274);
            openBR2Button.Name = "openBR2Button";
            openBR2Button.Size = new Size(250, 40);
            openBR2Button.TabIndex = 45;
            openBR2Button.Text = "Open FBX";
            openBR2Button.UseVisualStyleBackColor = true;
            openBR2Button.Click += OpenFBXButtonClick;
            // 
            // fpsLevel
            // 
            fpsLevel.AutoSize = true;
            fpsLevel.Location = new Point(13, 197);
            fpsLevel.Name = "fpsLevel";
            fpsLevel.Size = new Size(93, 17);
            fpsLevel.TabIndex = 44;
            fpsLevel.Text = "FPS Override";
            // 
            // fpsTextBox
            // 
            fpsTextBox.ForeColor = SystemColors.ActiveCaptionText;
            fpsTextBox.Location = new Point(117, 194);
            fpsTextBox.Name = "fpsTextBox";
            fpsTextBox.Size = new Size(139, 22);
            fpsTextBox.TabIndex = 43;
            fpsTextBox.Text = "60";
            // 
            // concatenateNA2Button
            // 
            concatenateNA2Button.Location = new Point(6, 228);
            concatenateNA2Button.Name = "concatenateNA2Button";
            concatenateNA2Button.Size = new Size(250, 40);
            concatenateNA2Button.TabIndex = 42;
            concatenateNA2Button.Text = "Concatenate All NA2 Files in Directory";
            concatenateNA2Button.UseVisualStyleBackColor = true;
            concatenateNA2Button.Click += ConcatenateNA2ButtonClick;
            // 
            // angleLabel
            // 
            angleLabel.AutoSize = true;
            angleLabel.Location = new Point(260, 387);
            angleLabel.Name = "angleLabel";
            angleLabel.Size = new Size(101, 17);
            angleLabel.TabIndex = 41;
            angleLabel.Text = "Rotation Angle";
            // 
            // axisLabel
            // 
            axisLabel.AutoSize = true;
            axisLabel.Location = new Point(260, 357);
            axisLabel.Name = "axisLabel";
            axisLabel.Size = new Size(33, 17);
            axisLabel.TabIndex = 41;
            axisLabel.Text = "Axis";
            // 
            // axisComboBox
            // 
            axisComboBox.FormattingEnabled = true;
            axisComboBox.Items.AddRange(new object[] {
            "X (Left-Right)",
            "Y (Forward-Back)",
            "Z (Up-Down)"});
            axisComboBox.Location = new Point(316, 354);
            axisComboBox.Name = "axisComboBox";
            axisComboBox.Size = new Size(159, 24);
            axisComboBox.TabIndex = 40;
            axisComboBox.Text = "Z (Up-Down)";

            // 
            // classNameTextBoxLabel
            // 
            geoClassNameLabel.AutoSize = true;
            geoClassNameLabel.Location = new Point(260, 106);
            geoClassNameLabel.Name = "classNameTextBoxLabel";
            geoClassNameLabel.Size = new Size(99, 17);
            geoClassNameLabel.TabIndex = 48;
            geoClassNameLabel.Text = "Geo Class";
            geoClassNameLabel.TextAlign = ContentAlignment.MiddleRight;

            // 
            // classNameComboBox
            // 
            geoClassNameComboBox.FormattingEnabled = true;
            geoClassNameComboBox.Items.AddRange(new object[] {
            "DecalGeometry",
            "LandmarkModel",
            "LandmarkObstructionProfile",
            "Leader",
            "Leader_ShadowVolume",
            "UILensModel",
            "Unit",
            "VFXModel",
            "WonderMovieModel"
            });
            geoClassNameComboBox.Location = new Point(316, 102);
            geoClassNameComboBox.Name = "classNameComboBox";
            geoClassNameComboBox.Size = new Size(159, 24);
            geoClassNameComboBox.TabIndex = 47;
            geoClassNameComboBox.Text = "Unit";

            assetClassNameLabel.AutoSize = true;
            assetClassNameLabel.Location = new Point(10, 120);
            assetClassNameLabel.Name = "assetClassNameLabel";
            assetClassNameLabel.Size = new Size(99, 17);
            assetClassNameLabel.TabIndex = 48;
            assetClassNameLabel.Text = "Asset Class"; 
            assetClassNameLabel.TextAlign = ContentAlignment.MiddleRight;


            assetClassNameComboBox.FormattingEnabled = true;
            assetClassNameComboBox.Location = new Point(90, 116);
            assetClassNameComboBox.Items.AddRange(new object[] {
            "CityBlock",
            "Clutter",
            "Landmark",
            "Leader",
            "RouteDoodad",
            "StrategicView_DirectedAsset",
            "StrategicView_Route",
            "StrategicView_TerrainBlend",
            "StrategicView_TerrainBlendCorners",
            "TerrainElementAsset",
            "TileBase",
            "UILensAsset",
            "Unit",
            "VFX",
            "WonderMovie"
            });

            assetClassNameComboBox.Name = "assetClassNameComboBox";
            assetClassNameComboBox.Size = new Size(159, 24);
            assetClassNameComboBox.TabIndex = 47;
            assetClassNameComboBox.Text = "Unit";


            materialClassNameLabel.AutoSize = true;
            materialClassNameLabel.Location = new Point(10, 150);
            materialClassNameLabel.Name = "materialClassNameLabel";
            materialClassNameLabel.Size = new Size(99, 17);
            materialClassNameLabel.TabIndex = 48;
            materialClassNameLabel.Text = "Material Class";
            materialClassNameLabel.TextAlign = ContentAlignment.MiddleRight;
            materialClassNameLabel.Hide();

            materialClassNameComboBox.FormattingEnabled = true;
            materialClassNameComboBox.Location = new Point(90, 146);
            materialClassNameComboBox.Items.AddRange(new object[] {
            "BurnMaterial",
            "DecalMaterial",
            "FOWLineDrawing",
            "Landmark",
            "Leader",
            "Leader_Cloth",
            "Leader_Glass",
            "Leader_Hair",
            "Leader_Matte",
            "Leader_Skin",
            "RiverWater",
            "SnowMaterial",
            "TerrainEditMaterial",
            "TerrainElement",
            "TerrainMaterial",
            "UILensMaterial",
            "Unit",
            "VFXModel",
            "VFXModel_FX",
            "Water",
            "WaveMaterial"
            });

            materialClassNameComboBox.Name = "materialClassNameComboBox";
            materialClassNameComboBox.Size = new Size(159, 24);
            materialClassNameComboBox.TabIndex = 47;
            materialClassNameComboBox.Text = "Unit";
            materialClassNameComboBox.Hide();



            dsgLabel.AutoSize = true;
            dsgLabel.Location = new Point(10, 180);
            dsgLabel.Name = "dsgLabel";
            dsgLabel.Size = new Size(99, 17);
            dsgLabel.TabIndex = 48;
            dsgLabel.Text = "DSG";
            dsgLabel.TextAlign = ContentAlignment.MiddleRight;

            dsgComboBox.FormattingEnabled = true;
            dsgComboBox.Location = new Point(90, 176);
            dsgComboBox.Items.AddRange(new object[] {
            "Landmark_Activated",
            "Landmark_WaterMill",
            "potential_any_graph",
            "Standard_Biped",
            "Standard_Broadside",
            "Standard_CannonBlast",
            "Standard_Clutter",
            "Standard_Dog",
            "Standard_GunMuzzleFlash",
            "Standard_Landmark",
            "Standard_LandmarkNoRandomOffset",
            "Standard_Leader",
            "Standard_LeaderBackground",
            "Standard_Naval",
            "Standard_NonCombat",
            "Standard_NonCombatNaval",
            "Standard_SwordTrail",
            "Standard_TerrainElementAsset",
            "Standard_VFX",
            "Standard_VFX_RandomOffset",
            "Standard_WonderMovie",
            ""
            });

            dsgComboBox.Name = "dsgComboBox";
            dsgComboBox.Size = new Size(159, 24);
            dsgComboBox.TabIndex = 47;
            dsgComboBox.Text = "potential_any_graph";

            multiModelAssetCheckBoxLabel.AutoSize = true;
            multiModelAssetCheckBoxLabel.Location = new Point(50, 270);
            multiModelAssetCheckBoxLabel.Name = "multiModelAssetCheckBox";
            multiModelAssetCheckBoxLabel.Size = new Size(99, 17);
            multiModelAssetCheckBoxLabel.TabIndex = 47;
            multiModelAssetCheckBoxLabel.Text = "Multi Model Asset Mode";
            multiModelAssetCheckBoxLabel.TextAlign = ContentAlignment.MiddleRight;
            multiModelAssetCheckBoxLabel.Hide();

            multiModelAssetCheckBox.Checked = false;
            multiModelAssetCheckBox.Location = new Point(180, 265);
            multiModelAssetCheckBox.Hide();

            vertexFormatComboBox.FormattingEnabled = true;
            vertexFormatComboBox.Items.AddRange(new object[] {
            "Bone Bindings, 1 UV",
            "No Bone Bindings, 2 UVs",
            "No Bone Bindings, 3 UVs"
            });
            vertexFormatComboBox.Location = new Point(8, 382);
            vertexFormatComboBox.Name = "vertexFormatComboBox";
            vertexFormatComboBox.Size = new Size(159, 24);
            vertexFormatComboBox.TabIndex = 47;
            vertexFormatComboBox.Text = "Bone Bindings, 1 UV";

            // 
            // endTImeTextBoxLabel
            // 
            endTimeTextBoxLabel.AutoSize = true;
            endTimeTextBoxLabel.Location = new Point(13, 170);
            endTimeTextBoxLabel.Name = "endTimeTextBoxLabel";
            endTimeTextBoxLabel.Size = new Size(99, 17);
            endTimeTextBoxLabel.TabIndex = 39;
            endTimeTextBoxLabel.Text = "NA2 End Time";
            endTimeTextBoxLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // startTimeTextBoxLabel
            // 
            startTimeTextBoxLabel.AutoSize = true;
            startTimeTextBoxLabel.Location = new Point(13, 143);
            startTimeTextBoxLabel.Name = "startTimeTextBoxLabel";
            startTimeTextBoxLabel.Size = new Size(104, 17);
            startTimeTextBoxLabel.TabIndex = 38;
            startTimeTextBoxLabel.Text = "NA2 Start Time";
            startTimeTextBoxLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // endTimeTextBox
            // 
            endTimeTextBox.Location = new Point(117, 167);
            endTimeTextBox.Name = "endTimeTextBox";
            endTimeTextBox.Size = new Size(139, 22);
            endTimeTextBox.TabIndex = 37;
            // 
            // startTimeTextBox
            // 
            startTimeTextBox.Location = new Point(117, 140);
            startTimeTextBox.Name = "startTimeTextBox";
            startTimeTextBox.Size = new Size(139, 22);
            startTimeTextBox.TabIndex = 37;
            // 
            // exportNA2Button
            // 
            exportNA2Button.Location = new Point(6, 94);
            exportNA2Button.Name = "exportNA2Button";
            exportNA2Button.Size = new Size(250, 40);
            exportNA2Button.TabIndex = 36;
            exportNA2Button.Text = "Export Animation to NA2";
            exportNA2Button.UseVisualStyleBackColor = true;
            exportNA2Button.Click += ExportNA2ButtonClick;
            // 
            // rescaleBoneNameLabel
            // 
            rescaleBoneNameLabel.AutoSize = true;
            rescaleBoneNameLabel.Location = new Point(174, 505);
            rescaleBoneNameLabel.Name = "rescaleBoneNameLabel";
            rescaleBoneNameLabel.Size = new Size(87, 17);
            rescaleBoneNameLabel.TabIndex = 35;
            rescaleBoneNameLabel.Text = "Parent Bone";
            // 
            // rescaleNamedBoneButton
            // 
            rescaleNamedBoneButton.Location = new Point(262, 458);
            rescaleNamedBoneButton.Name = "rescaleNamedBoneButton";
            rescaleNamedBoneButton.Size = new Size(213, 38);
            rescaleNamedBoneButton.TabIndex = 34;
            rescaleNamedBoneButton.Text = "Add Scaled FX Bone and Save";
            rescaleNamedBoneButton.UseVisualStyleBackColor = true;
            rescaleNamedBoneButton.Click += RescaleNamedBoneButtonClick;
            // 
            // bonesComboBox
            // 
            bonesComboBox.FormattingEnabled = true;
            bonesComboBox.Location = new Point(262, 502);
            bonesComboBox.Name = "bonesComboBox";
            bonesComboBox.Size = new Size(213, 24);
            bonesComboBox.TabIndex = 33;
            // 
            // rescaleFactorLabel
            // 
            rescaleFactorLabel.AutoSize = true;
            rescaleFactorLabel.Location = new Point(260, 328);
            rescaleFactorLabel.Name = "rescaleFactorLabel";
            rescaleFactorLabel.Size = new Size(103, 17);
            rescaleFactorLabel.TabIndex = 32;
            rescaleFactorLabel.Text = "Rescale Factor";
            // 
            // insertAdjustmentBoneButton
            // 
            insertAdjustmentBoneButton.Location = new Point(262, 228);
            insertAdjustmentBoneButton.Name = "insertAdjustmentBoneButton";
            insertAdjustmentBoneButton.Size = new Size(213, 40);
            insertAdjustmentBoneButton.TabIndex = 31;
            insertAdjustmentBoneButton.Text = "Insert Adjustment Bone and Save";
            insertAdjustmentBoneButton.UseVisualStyleBackColor = true;
            insertAdjustmentBoneButton.Click += InsertAdjustmentBoneClick;
            // 
            // angleTextBox
            // 
            angleTextBox.Location = new Point(366, 384);
            angleTextBox.Name = "angleTextBox";
            angleTextBox.Size = new Size(109, 22);
            angleTextBox.TabIndex = 30;
            angleTextBox.Text = "90";
            // 
            // rescaleFactorTextBox
            // 
            rescaleFactorTextBox.Location = new Point(366, 325);
            rescaleFactorTextBox.Name = "rescaleFactorTextBox";
            rescaleFactorTextBox.Size = new Size(109, 22);
            rescaleFactorTextBox.TabIndex = 30;
            rescaleFactorTextBox.Text = "1";

            // 
            // rescaleFactorLabel
            // 
            xPositionLabel.Location = new Point(260, 287);
            xPositionLabel.Name = "xPositionLabel";
            xPositionLabel.Size = new Size(28, 17);
            xPositionLabel.TabIndex = 32;
            xPositionLabel.Text = "X";

            yPositionLabel.Location = new Point(335, 287);
            yPositionLabel.Name = "xPositionLabel";
            yPositionLabel.Size = new Size(28, 17);
            yPositionLabel.TabIndex = 32;
            yPositionLabel.Text = "Y";

            zPositionLabel.Location = new Point(410, 287);
            zPositionLabel.Name = "xPositionLabel";
            zPositionLabel.Size = new Size(28, 17);
            zPositionLabel.TabIndex = 32;
            zPositionLabel.Text = "Z";

            xPositionTextBox.Location = new Point(288, 284);
            xPositionTextBox.Name = "xPositionTextBox";
            xPositionTextBox.Size = new Size(40, 22);
            xPositionTextBox.TabIndex = 40;
            xPositionTextBox.Text = "0";

            yPositionTextBox.Location = new Point(363, 284);
            yPositionTextBox.Name = "yPositionTextBox";
            yPositionTextBox.Size = new Size(40, 22);
            yPositionTextBox.TabIndex = 30;
            yPositionTextBox.Text = "0";

            zPositionTextBox.Location = new Point(438, 284);
            zPositionTextBox.Name = "zPositionTextBox";
            zPositionTextBox.Size = new Size(40, 22);
            zPositionTextBox.TabIndex = 30;
            zPositionTextBox.Text = "0";

            // 
            // makeTemplateButton
            // 
            makeTemplateButton.Location = new Point(262, 186);
            makeTemplateButton.Name = "makeTemplateButton";
            makeTemplateButton.Size = new Size(213, 38);
            makeTemplateButton.TabIndex = 29;
            makeTemplateButton.Text = "Make Template";
            makeTemplateButton.UseVisualStyleBackColor = true;
            makeTemplateButton.Click += MakeTemplateButtonClick;
            makeTemplateButton.Hide();
   
            // 
            // removeAnimationsButton
            // 
            removeAnimationsButton.Location = new Point(262, 182);
            removeAnimationsButton.Name = "removeAnimationsButton";
            removeAnimationsButton.Size = new Size(213, 40);
            removeAnimationsButton.TabIndex = 28;
            removeAnimationsButton.Text = "Remove Animations and Save";
            removeAnimationsButton.UseVisualStyleBackColor = true;
            removeAnimationsButton.Click += RemoveAnimationsButtonClick;
            //removeAnimationsButton.Click += MakeTemplateButtonClick;

            this.exportBR2Button.Location = new System.Drawing.Point(262, 228);
            this.exportBR2Button.Name = "exportBR2Button";
            this.exportBR2Button.Size = new System.Drawing.Size(213, 40);
            this.exportBR2Button.TabIndex = 46;
            this.exportBR2Button.Text = "Create Particle .ptl from .psb";
            this.exportBR2Button.UseVisualStyleBackColor = true;
            this.exportBR2Button.Click += new System.EventHandler(ConvertParticlePSBClick);
            this.exportBR2Button.Hide();

            // 
            // removeNamedBoneButton
            // 
            this.removeNamedBoneButton.Location = new System.Drawing.Point(262, 412);
            this.removeNamedBoneButton.Name = "removeNamedBoneButton";
            this.removeNamedBoneButton.Size = new System.Drawing.Size(213, 38);
            this.removeNamedBoneButton.TabIndex = 34;
            this.removeNamedBoneButton.Text = "Remove Bone";
            this.removeNamedBoneButton.UseVisualStyleBackColor = true;
            this.removeNamedBoneButton.Click += new System.EventHandler(this.RemoveNamedBoneButtonClick);

            // 
            // resaveAllFBXAsAnimsButton
            // 
            resaveAllFBXAsAnimsButton.Location = new Point(262, 136);
            resaveAllFBXAsAnimsButton.Name = "resaveAllFBXAsAnimsButton";
            resaveAllFBXAsAnimsButton.Size = new Size(213, 40);
            resaveAllFBXAsAnimsButton.TabIndex = 27;
            resaveAllFBXAsAnimsButton.Text = "Convert all .gr2 Files in Directory to .fgx/.anm";
            resaveAllFBXAsAnimsButton.UseVisualStyleBackColor = true;
            resaveAllFBXAsAnimsButton.Click += ResaveAllGR2FilesInDirAsAnimsClick;
            //resaveAllFBXAsAnimsButton.Hide();
            // 
            // templateBR2OverwriteLabel
            // 
            vertexFormatLabel.AutoSize = true;
            vertexFormatLabel.Font = new Font("Microsoft Sans Serif", 7.3125F, FontStyle.Regular, GraphicsUnit.Point, 0);
            vertexFormatLabel.Location = new Point(6, 360);
            vertexFormatLabel.Name = "templateBR2OverwriteLabel";
            vertexFormatLabel.Size = new Size(198, 16);
            vertexFormatLabel.TabIndex = 24;
            vertexFormatLabel.Text = "Mesh Vertex Format";

            importExportFiletypesLabel.AutoSize = true;
            importExportFiletypesLabel.Font = new Font("Microsoft Sans Serif", 7.3125F, FontStyle.Regular, GraphicsUnit.Point, 0);
            importExportFiletypesLabel.Location = new Point(6, 420);
            importExportFiletypesLabel.Name = "templateBR2OverwriteLabel2";
            importExportFiletypesLabel.Size = new Size(198, 16);
            importExportFiletypesLabel.TabIndex = 24;
            importExportFiletypesLabel.Text = "Import/Export Filetype(s)";
            // 
            // importExportFormatBR2NB2RadioButton
            // 
            importExportFormatBR2NB2RadioButton.AutoSize = true;
            importExportFormatBR2NB2RadioButton.Location = new Point(29, 472);
            importExportFormatBR2NB2RadioButton.Name = "useSceneTemplateRadioButton";
            importExportFormatBR2NB2RadioButton.Size = new Size(161, 21);
            importExportFormatBR2NB2RadioButton.TabIndex = 23;
            importExportFormatBR2NB2RadioButton.TabStop = true;
            importExportFormatBR2NB2RadioButton.Text = "Nexus Buddy 2 (.br2/.nb2)";
            importExportFormatBR2NB2RadioButton.UseVisualStyleBackColor = true;
            // 
            // importExportFormatCN6RadioButton
            // 
            importExportFormatCN6RadioButton.AutoSize = true;
            importExportFormatCN6RadioButton.Checked = true;
            importExportFormatCN6RadioButton.Location = new Point(29, 446);
            importExportFormatCN6RadioButton.Name = "useLeaderTemplateRadioButton";
            importExportFormatCN6RadioButton.Size = new Size(166, 21);
            importExportFormatCN6RadioButton.TabIndex = 22;
            importExportFormatCN6RadioButton.TabStop = true;
            importExportFormatCN6RadioButton.Text = "CivNexus6 (.cn6)";
            importExportFormatCN6RadioButton.UseVisualStyleBackColor = true;

            // 
            // resaveAllFilesInDirButton
            // 
            writeGeoFileButton.Location = new Point(262, 50);
            writeGeoFileButton.Name = "writeGeoFileButton";
            writeGeoFileButton.Size = new Size(213, 40);
            writeGeoFileButton.TabIndex = 18;
            writeGeoFileButton.Text = "Create Geometry/Animation (.geo/.anm) File";
            writeGeoFileButton.UseVisualStyleBackColor = true;
            writeGeoFileButton.Click += WriteGeoFileButtonClick;
            // 
            // exportNB2CurrentModelButton
            // 
            exportCurrentModelButton.Location = new Point(6, 50);
            exportCurrentModelButton.Name = "exportNB2CurrentModelButton";
            exportCurrentModelButton.Size = new Size(250, 40);
            exportCurrentModelButton.TabIndex = 15;
            exportCurrentModelButton.Text = "Export Model (Current Model)";
            exportCurrentModelButton.UseVisualStyleBackColor = true;
            exportCurrentModelButton.Click += ExportModel;
            // 
            // overwriteMeshesButton
            // 
            overwriteMeshesButton.Location = new System.Drawing.Point(262, 228);
            overwriteMeshesButton.Name = "overwriteMeshesButton";
            overwriteMeshesButton.Size = new Size(213, 40);
            overwriteMeshesButton.TabIndex = 14;
            overwriteMeshesButton.Text = "Overwrite Meshes";
            overwriteMeshesButton.UseVisualStyleBackColor = true;
            overwriteMeshesButton.Click += OverwriteMeshesButtonClick;
            overwriteMeshesButton.Hide();
            // 
            // exportNB2Button
            // 
            exportAllModelsButton.Location = new Point(6, 6);
            exportAllModelsButton.Name = "exportNB2Button";
            exportAllModelsButton.Size = new Size(250, 40);
            exportAllModelsButton.TabIndex = 11;
            exportAllModelsButton.Text = "Export Models (All Models)";
            exportAllModelsButton.UseVisualStyleBackColor = true;
            exportAllModelsButton.Click += ExportAllModels;

            processTextureButton.Location = new Point(6, 50);
            processTextureButton.Name = "processTextureButton";
            processTextureButton.Size = new Size(250, 40);
            processTextureButton.TabIndex = 11;
            processTextureButton.Text = "Process Texture into .dds/.tex";
            processTextureButton.UseVisualStyleBackColor = true;
            processTextureButton.Click += ProcessTextureClick;

            processTexturesInDirButton.Location = new Point(260, 50);
            processTexturesInDirButton.Name = "processTexturesInDirButton";
            processTexturesInDirButton.Size = new Size(210, 40);
            processTexturesInDirButton.TabIndex = 11;
            processTexturesInDirButton.Text = "Process All Textures in Directory";
            processTexturesInDirButton.UseVisualStyleBackColor = true;
            processTexturesInDirButton.Click += ProcessTexturesInDirectoryClick;

            batchConversionButton.Location = new Point(6, 210);
            batchConversionButton.Name = "batchConversionButton";
            batchConversionButton.Size = new Size(250, 40);
            batchConversionButton.TabIndex = 11;
            batchConversionButton.Text = "Batch Conversion Civ 5 -> Civ 6";
            batchConversionButton.UseVisualStyleBackColor = true;
            batchConversionButton.Click += BatchConversionAction;
            batchConversionButton.Hide();

            textureClassComboBox.FormattingEnabled = true;
            textureClassComboBox.Location = new Point(80, 12);
            textureClassComboBox.Name = "textureClassComboBox";
            textureClassComboBox.Size = new Size(213, 24);
            textureClassComboBox.TabIndex = 33;
            
            textureClassComboBox.Items.AddRange(TextureClass.GetAllTextureClasses().Keys.ToArray());
            textureClassComboBox.Text = "Generic_BaseColor";

            textureClassLabel.AutoSize = true;
            textureClassLabel.Location = new Point(6, 15);
            textureClassLabel.Name = "textureClassLabel";
            textureClassLabel.Size = new Size(198, 16);
            textureClassLabel.TabIndex = 24;
            textureClassLabel.Text = "Texture Class";

            // 
            // modelList
            // 
            modelList.CheckBoxes = true;
            modelList.Columns.AddRange(new ColumnHeader[] {modelName});
            modelList.Dock = DockStyle.Fill;
            modelList.FullRowSelect = true;
            modelList.GridLines = true;
            modelList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            modelList.Location = new Point(0, 0);
            modelList.MultiSelect = false;
            modelList.Name = "modelList";
            modelList.Size = new Size(484, 543);
            modelList.TabIndex = 0;
            modelList.UseCompatibleStateImageBehavior = false;
            modelList.View = View.Details;
            modelList.ItemCheck += ModelListCheckChanged;
            // 
            // modelName
            // 
            modelName.Text = "Model Name";
            modelName.Width = 300;
            // 
            // panel1
            // 
            panel1.Controls.Add(headerFilenameLabel);
            panel1.Controls.Add(viewButton);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(512, 42);
            panel1.TabIndex = 8;
            // 
            // headerFilenameLabel
            // 
            headerFilenameLabel.AutoSize = true;
            headerFilenameLabel.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            headerFilenameLabel.Location = new Point(3, 12);
            headerFilenameLabel.Name = "headerFilenameLabel";
            headerFilenameLabel.Size = new Size(110, 20);
            headerFilenameLabel.TabIndex = 3;
            headerFilenameLabel.Text = "No file open";
            // 
            // viewButton
            // 
            viewButton.Location = new Point(412, 3);
            viewButton.Name = "viewButton";
            viewButton.Size = new Size(97, 35);
            viewButton.TabIndex = 6;
            viewButton.Text = "View";
            viewButtonToolTip.SetToolTip(viewButton, "View .fbx in Granny Viewer");
            viewButton.UseVisualStyleBackColor = true;
            viewButton.Click += ViewButtonClick;
            // 
            // fileInfoGroupBox
            // 
            fileInfoGroupBox.Controls.Add(fileInfoTextBox);
            fileInfoGroupBox.Dock = DockStyle.Bottom;
            fileInfoGroupBox.Location = new Point(0, 46);
            fileInfoGroupBox.Name = "fileInfoGroupBox";
            fileInfoGroupBox.Size = new Size(512, 159);
            fileInfoGroupBox.TabIndex = 2;
            fileInfoGroupBox.TabStop = false;
            fileInfoGroupBox.Text = "Current File Info";
            // 
            // fileInfoTextBox
            // 
            fileInfoTextBox.Dock = DockStyle.Fill;
            fileInfoTextBox.Enabled = false;
            fileInfoTextBox.Location = new Point(3, 18);
            fileInfoTextBox.Name = "fileInfoTextBox";
            fileInfoTextBox.Size = new Size(506, 138);
            fileInfoTextBox.TabIndex = 0;
            fileInfoTextBox.Text = "";
            // 
            // properties
            // 
            properties.Dock = DockStyle.Bottom;
            properties.Location = new Point(0, 205);
            properties.Name = "properties";
            properties.Size = new Size(512, 413);
            properties.TabIndex = 0;
            properties.PropertyValueChanged += PropertiesPropertyValueChanged;
            
            // 
            // animation
            // 
            animation.Tag = "";
            animation.Text = "Animation";
            animation.Width = 149;
            // 
            // duration
            // 
            duration.Text = "Duration";
            duration.Width = 94;
           
            // 
            // materialTypeColumnHeader
            // 
            materialTypeColumnHeader.Text = "Material Type";
            materialTypeColumnHeader.Width = 138;
            // 
            // materialNameColumnHeader
            // 
            materialNameColumnHeader.Tag = "";
            materialNameColumnHeader.Text = "Material Name";
            materialNameColumnHeader.Width = 149;
            // 
            // br2ImportButton
            // 
            br2ImportButton.Location = new Point(0, 0);
            br2ImportButton.Name = "br2ImportButton";
            br2ImportButton.Size = new Size(75, 23);
            br2ImportButton.TabIndex = 0;
            // 
            // loadStringDatabaseButton
            // 
            loadStringDatabaseButton.Location = new Point(262, 6);
            loadStringDatabaseButton.Name = "loadStringDatabaseButton";
            loadStringDatabaseButton.Size = new Size(213, 40);
            loadStringDatabaseButton.TabIndex = 17;
            loadStringDatabaseButton.Text = "Create GeometrySet .xml for .ast file";
            loadStringDatabaseButton.UseVisualStyleBackColor = true;
            loadStringDatabaseButton.Click += WriteGeometrySetDataClick;
            // 
            // CivNexusSixApplicationForm
            // 
            ClientSize = new Size(1008, 618);
            Controls.Add(masterSplitContainer);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "CivNexusSixApplicationForm";
            Text = GetApplicationNameWithVersionNumber() + " - Graphics Tool for Civilization VI";
            masterSplitContainer.Panel1.ResumeLayout(false);
            masterSplitContainer.Panel2.ResumeLayout(false);
            ((ISupportInitialize)masterSplitContainer).EndInit();
            masterSplitContainer.ResumeLayout(false);
            leftHandSplitContainer.Panel1.ResumeLayout(false);
            leftHandSplitContainer.Panel2.ResumeLayout(false);
            ((ISupportInitialize)leftHandSplitContainer).EndInit();
            leftHandSplitContainer.ResumeLayout(false);
            mainButtonPanel.ResumeLayout(false);
            mainTabControl.ResumeLayout(false);
            editModelTabPage.ResumeLayout(false);
            editModelContainer.Panel1.ResumeLayout(false);
            editModelContainer.Panel2.ResumeLayout(false);
            ((ISupportInitialize)editModelContainer).EndInit();
            editModelContainer.ResumeLayout(false);
            materialListPanelContainer.Panel1.ResumeLayout(false);
            ((ISupportInitialize)materialListPanelContainer).EndInit();
            materialListPanelContainer.ResumeLayout(false);
            materialButtonsPanel.ResumeLayout(false);
            animationsTabControl.ResumeLayout(false);
            grannyAnimsTabPage.ResumeLayout(false);
            otherActionsTabPage.ResumeLayout(false);
            otherActionsTabPage.PerformLayout();
            selectModelTabPage.ResumeLayout(false);
            furtherActionsTabPage.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            fileInfoGroupBox.ResumeLayout(false);
            ResumeLayout(false);
		}

    }
}