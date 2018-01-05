using Firaxis.Granny;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace NexusBuddy
{
	public class MaterialProperties
	{
		private IGrannyMaterial material;
		[Category("Material"), Description("Name of the material")]
		public string Name
		{
			get
			{
				return this.material.Name;
			}
			set
			{
				this.material.Name = value;
			}
		}

		public MaterialProperties(IGrannyMaterial material)
		{
			this.material = material;
		}

        public string typeName
        {
            get
            {
                return this.material.typeName;
            }
        }

        public string TextureFromFileName
        {
            get
            {
                return Path.GetFileName(this.material.Texture.FromFileName);  
            }
        }

        public System.Collections.Generic.List<IGrannyMap> Maps
        {
            get
            {
                return this.material.Maps;
            }
        }

        public virtual void AddToListView(ListView view)
		{
			view.Items.Add(this.material.Name);
			view.Items[view.Items.Count - 1].SubItems.Add(this.material.ShaderSet);
			view.Items[view.Items.Count - 1].Tag = this;
		}

		public IGrannyMaterial GetMaterial()
		{
			return this.material;
		}
	}
}
