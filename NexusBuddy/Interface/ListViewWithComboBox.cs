using Firaxis.Granny;
using System;
using System.Drawing;
using System.Windows.Forms;
namespace NexusBuddy
{
	public class ListViewWithComboBox : ListView
	{
		private ListViewItem item;
		private string subItemText = "";
		private ComboBox comboBoxMaterials = new ComboBox();

		public void FillCombo()
		{
			comboBoxMaterials.Items.Clear();
			comboBoxMaterials.Items.Add("<unassigned>");
			foreach (IGrannyMaterial material in CivNexusSixApplicationForm.loadedFile.Materials)
			{
				comboBoxMaterials.Items.Add(material.Name + " (" + material.typeName + ")");
			}
		}

		public ListViewWithComboBox()
		{
			comboBoxMaterials.Size = new Size(0, 0);
			comboBoxMaterials.Location = new Point(0, 0);
			Controls.AddRange(new Control[]
			{
				comboBoxMaterials
			});
			comboBoxMaterials.SelectedIndexChanged += new EventHandler(MaterialSelected);
            comboBoxMaterials.LostFocus += new EventHandler(MaterialFocusExit);
            //comboBoxMaterials.KeyPress += new KeyPressEventHandler(MaterialKeyPress);
			comboBoxMaterials.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxMaterials.Hide();
			Size = new Size(0, 0);
			TabIndex = 0;
			View = View.Details;
			MouseDown += new MouseEventHandler(ListViewMouseDown);
			DoubleClick += new EventHandler(ListViewDoubleClick);
			Click += new EventHandler(ListViewDoubleClick);
			GridLines = true;
		}

		//private void MaterialKeyPress(object sender, KeyPressEventArgs e)
		//{
		//	if (e.KeyChar == '\r' || e.KeyChar == '\u001b')
		//	{
		//		comboBoxMaterials.Hide();
		//		CivNexusSixApplicationForm.form.UpdateMaterialBinding(item.Index, comboBoxMaterials.SelectedIndex);
		//	}
		//}

        private void MaterialSelected(object sender, EventArgs e)
		{
			int selectedIndex = comboBoxMaterials.SelectedIndex;
			if (selectedIndex >= 0)
			{ 
                item.SubItems[1].Text = comboBoxMaterials.Items[selectedIndex].ToString();
                item.SubItems[1].Tag = selectedIndex;
                CivNexusSixApplicationForm.form.UpdateMaterialBindings();
			}
		}

        private void MaterialFocusExit(object sender, EventArgs e)
		{
			comboBoxMaterials.Hide();
		}

		public void ListViewDoubleClick(object sender, EventArgs e)
		{
			int num = 0;
			int num2 = Columns[0].Width;
			for (int i = 0; i < Columns.Count; i++)
			{
                if (i == 1)
                { 
					break;
				}
				num = num2;
				num2 += Columns[i].Width;
			}
			subItemText = item.SubItems[1].Text;
			new Rectangle(num, item.Bounds.Top, num2, item.Bounds.Bottom);
			comboBoxMaterials.Size = new Size(356, item.Bounds.Bottom - item.Bounds.Top);
			comboBoxMaterials.Location = new Point(num, item.Bounds.Y);
			FillCombo();
			comboBoxMaterials.Show();
			comboBoxMaterials.Text = subItemText;
			comboBoxMaterials.SelectAll();
			comboBoxMaterials.Focus();
		}

		public void ListViewMouseDown(object sender, MouseEventArgs e)
		{
			item = GetItemAt(e.X, e.Y);
		}
	}
}
