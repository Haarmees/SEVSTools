using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SEVSTools.OptionsPage
{
	public partial class GeneralOptions : UserControl
	{
		public GeneralOptions()
		{
			InitializeComponent();
		}

		internal OptionPage OptionPage;

		public void Initialize()
		{
			this.gamePathTextBox.Text = this.OptionPage.GameDirPath;
			this.buildPathTextBox.Text = this.OptionPage.BuildDirPath;
		}

		private void gamePathButton_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.SelectedPath = this.gamePathTextBox.Text;
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				this.gamePathTextBox.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void buildPathButton_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.SelectedPath = this.buildPathTextBox.Text;
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				this.buildPathTextBox.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void folderBrowserDialog_HelpRequest(object sender, EventArgs e)
		{

		}
	}
}
