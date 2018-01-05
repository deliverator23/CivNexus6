using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
namespace NexusBuddy
{
	internal class FilteredFileNameEditor : UITypeEditor
	{
		private OpenFileDialog ofd = new OpenFileDialog();
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			this.ofd.RestoreDirectory = true;
			this.ofd.FileName = value.ToString();
			this.ofd.Filter = "DDS File|*.dds|All Files|*.*";
			if (this.ofd.ShowDialog() == DialogResult.OK)
			{
				return this.ofd.FileName;
			}
			return base.EditValue(context, provider, value);
		}
	}
}
