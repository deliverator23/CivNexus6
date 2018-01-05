using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace NexusBuddy
{
	public class IndieAnimDef
	{
		private List<int> eventCodes = new List<int>();
		[Category("Animation Info"), DisplayName("Animation Name")]
		public string Name
		{
			get;
			set;
		}
		[Category("Frame Info"), DisplayName("Start Frame")]
		public string StartFrame
		{
			get;
			set;
		}
		[Category("Frame Info"), DisplayName("End Frame")]
		public string EndFrame
		{
			get;
			set;
		}
		[Category("Event Info"), DisplayName("Event Codes")]
		public string EventCodes
		{
			get
			{
				string text = "";
				for (int i = 0; i < this.eventCodes.Count; i++)
				{
					text += this.eventCodes[i];
					if (i < this.eventCodes.Count - 1)
					{
						text += ", ";
					}
				}
				return text;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					return;
				}
				this.eventCodes.Clear();
				string[] array = value.Split(new char[]
				{
					','
				});
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string value2 = array2[i];
					this.eventCodes.Add(Convert.ToInt32(value2));
				}
			}
		}
	}
}
