using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using SEVSTools.OptionsPage;

namespace SEVSTools.OptionsPage
{
	public class OptionPage : DialogPage
	{
		private string gameDirPath = @"C:\Program Files (x86)\Steam\SteamApps\common\SpaceEngineers";
		private string buildDirPath = "%AppData%\\SpaceEngineers\\IngameScripts\\local";

		public string GameDirPath
		{
			get { return this.gameDirPath; }
			set { this.gameDirPath = value; }
		}

		public string BuildDirPath
		{
			get { return this.buildDirPath; }
			set { this.buildDirPath = value; }
		}

		private void InitializeComponent()
		{
			
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		protected override IWin32Window Window
		{
			get
			{
				GeneralOptions page = new GeneralOptions();
				page.OptionPage = this;
				page.Initialize();
				return page;
			}
		}
	}
}
