using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DirectoryMonitor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		DateTime fsLastRaised;
		FileSystemWatcher fileSystemWatcher;
		string defaultDirectory;
		bool paintopen;
		bool wordopen;
		bool excelopen;

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			btnClear.Click += BtnClear_Click;
			defaultDirectory = ConfigurationSettings.AppSettings["DirectoryToMonitor"];
			_initializeMonitor();
		}

		private void BtnClear_Click(object sender, RoutedEventArgs e)
		{
			listBox.Items.Clear();
		}

		private void _initializeMonitor()
		{
			fileSystemWatcher = new FileSystemWatcher(this.defaultDirectory, "*.*");
			fileSystemWatcher.EnableRaisingEvents = true;
			fileSystemWatcher.IncludeSubdirectories = true;
			fileSystemWatcher.Created += fileSystemWatcher_Created;
		}

		// private void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
		private void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
		// protected void newfile(object fscreated, FileSystemEventArgs Eventocc)
		{
			try
			{
				//to avoid same process to be repeated ,if the time between two events is more   than 1000 milli seconds only the second process will be considered 
				if (DateTime.Now.Subtract(fsLastRaised).TotalMilliseconds > 1000)
				{
					//to get the newly created file name and extension and also the name of the event occured in the watching folder
					string CreatedFileName = e.Name;
					FileInfo createdFile = new FileInfo(CreatedFileName);
					string extension = createdFile.Extension;
					string eventoccured = e.ChangeType.ToString();

					//to note the time of event occured
					fsLastRaised = DateTime.Now;
					//Delay is given to the thread for avoiding same process to be repeated
					System.Threading.Thread.Sleep(100);
					//dispatcher invoke 
					this.Dispatcher.Invoke((Action)(() =>
					{
						//give a notification in the application about the change in folder
						listBox.Items.Add("New File: " + CreatedFileName + ";  Created :" + DateTime.Now.ToString());

						// _sendEmail(CreatedFileName, defaultDirectory);

						return;
						//if image file is created ,to open it in ms paint application
						if (extension == ".jpg" || extension == ".png")
						{
							Process.Start("mspaint", defaultDirectory + "\\" + CreatedFileName);
						}
						//if video file is created ,to open it in windows mwdia player application
						else if (extension == ".wmv" || extension == ".mov" || extension == ".avi")
						{
							Process.Start("wmplayer", defaultDirectory + "\\" + CreatedFileName);
						}
						//if ms word file is created ,to open it in ms word application
						else if (extension == ".docx")
						{
							Process.Start("WINWORD.EXE", defaultDirectory + "\\" + CreatedFileName);
						}
						//if excel file is created ,to open it in excel application
						else if (extension == ".xlsx")
						{
							Process.Start("excel.exe", defaultDirectory + "\\" + CreatedFileName);
						}
						//if pdf file is created ,to open it in PDF application
						else if (extension == ".pdf")
						{
							Process.Start("AcroRd32.exe", defaultDirectory + "\\" + CreatedFileName);
						}
						//if Flash file is created ,to open it in web browser
						else if (extension == ".swf")
						{
							Process.Start("iexplore.EXE", defaultDirectory + "\\" + CreatedFileName);
						}
					}));
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		public static void _sendEmail(string fileName, string directory)
		{
			string to = "djokinen@ryerson.ca";
			string from = "noreply-ao@ryerson.ca";
			string subject = "New AutoEavl file";
			string body = String.Format("File: {0}\nDirectory: {1}", fileName, directory);
			MailMessage mailMessage = new MailMessage(from, to, subject, body);
			SmtpClient smtpClient = new SmtpClient("localhost");
			// smtpClient.Timeout = 100;
			// Credentials are necessary if the server requires the client to authenticate before it will send e-mail on the client's behalf.
			smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;

			try
			{
				smtpClient.Send(mailMessage);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught in CreateTimeoutTestMessage(): {0}", ex.ToString());
			}
			finally
			{
				smtpClient.Dispose();
			}
		}

		public MainWindow()
		{
			InitializeComponent();
		}
	}
}