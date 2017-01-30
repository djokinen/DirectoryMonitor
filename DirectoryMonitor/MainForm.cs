using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirectoryMonitor
{
	public partial class MainForm : Form
	{
		SortedList<string, bool> _dataSource;
		FileSystemWatcher fileSystemWatcher;

		public MainForm()
		{
			InitializeComponent();
			_dataSource = new SortedList<string, bool>();
			dataGridView.DataSource = _dataSource;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			// browse directory
			// select file or folder
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
			DialogResult dialogResult = folderBrowserDialog.ShowDialog(this);
			if (dialogResult.HasFlag(DialogResult.OK) || dialogResult.HasFlag(DialogResult.Yes))
			{
				_initializeMonitor(folderBrowserDialog.SelectedPath);
			}
		}

		private void _initializeMonitor(string selectedPath)
		{
			fileSystemWatcher = new FileSystemWatcher(selectedPath, "*.*");
			fileSystemWatcher.EnableRaisingEvents = true;
			fileSystemWatcher.IncludeSubdirectories = false;
			fileSystemWatcher.Created += fileSystemWatcher_Created;
			// this.Controls.Add(fileSystemWatcher);
		}

		DateTime dateTimeLastRaised = DateTime.Now;
		private void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
		{
			try
			{
				//to avoid same process to be repeated ,if the time between two events is more   than 1000 milli seconds only the second process will be considered 
				if (DateTime.Now.Subtract(dateTimeLastRaised).TotalMilliseconds > 1000)
				{
					//to note the time of event occured
					dateTimeLastRaised = DateTime.Now;

					//to get the newly created file name and extension and also the name of the event occured in the watching folder
					string CreatedFileName = e.Name;
					FileInfo createdFile = new FileInfo(CreatedFileName);
					string extension = createdFile.Extension;
					string eventoccured = e.ChangeType.ToString();

					//Delay is given to the thread for avoiding same process to be repeated
					// System.Threading.Thread.Sleep(100);
					//give a notification in the application about the change in folder
					// listBox.Items.Add("New File: " + CreatedFileName + ";  Created :" + DateTime.Now.ToString());
					MessageBox.Show(string.Format("Created: {0}", e.FullPath));
					// DataGridViewRow dataGridViewRow = new DataGridViewRow();

					_dataSource.Add(e.FullPath, true);
					// dataGridView.DataSource..Rows.Add("x", true);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
	}
}