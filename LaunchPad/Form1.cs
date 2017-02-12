using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace LaunchPad
{
    public partial class frmLaunchPad : Form
    {
        public frmLaunchPad()
        {
            InitializeComponent();
        }
        //class level strings are delcared
        //folderList for the folders and filesList for the files
        List<string> folderList = new List<string>();
        List<string> fileList = new List<string>();

        //Four utility mehtods are created

        //method with three parameters
        public void ResourceToListAndListBox(StreamReader sr, ListBox lb, List<string> ls)
        {
            lb.Items.Clear(); //clears the listbox
            ls.Clear();
            while (sr.Peek() > -1)
            {
                ls.Add(sr.ReadLine());
            }
            lb.Items.AddRange(ls.ToArray()); //adds new items to the whole list

        }
        //methods with two parameters
        public void RefreshListBox(ListBox lb, string[] sa)
        {
            lb.Items.Clear(); //this clears the list box
            lb.Items.AddRange(sa); //this adds new item to the list box
        }
        public void UpdateRecourceFile (string[] sa, string resourcePath)
        {
            StreamWriter sw = new StreamWriter(resourcePath, false);
            foreach (string line in sa)
            {
                sw.WriteLine(line);
            }
            sw.Close();
        }
        public void ListStringAddToOrSwitchToFirst(List<string> lst, string item)
        {
            if (lst.Contains(item))
            {
                lst.Remove(item);
            }
            lst.Insert(0, item);
        }

        private void frmLaunchPad_Load(object sender, EventArgs e)
        {
            string specialFolder = AppDomain.CurrentDomain.BaseDirectory + @"LaunchPadFiles";
            string MRUfilesPath = specialFolder + @"MRUfiles.txt";
            string MRUfoldersPath = specialFolder + @"MRUfolders.txt";
            
            if (!Directory.Exists(specialFolder))
            {
                Directory.CreateDirectory(specialFolder);
                File.CreateText(MRUfilesPath);
                File.CreateText(MRUfoldersPath);
            }
            StreamReader foldersSR = File.OpenText(MRUfoldersPath);
            ResourceToListAndListBox(foldersSR, lstFolders, folderList);
            StreamReader fileSR = File.OpenText(MRUfilesPath);
            ResourceToListAndListBox(fileSR, lstFiles, fileList);
            foldersSR.Close();
            fileSR.Close();
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            OpenFileDialog oFile = new OpenFileDialog();
            oFile.Title = "Please choose a file to launch.";
            oFile.InitialDirectory = Environment.SpecialFolder.MyComputer.ToString();
            if (oFile.ShowDialog() == DialogResult.OK)
            {
                string newFile = oFile.FileName;
                ListStringAddToOrSwitchToFirst(fileList, newFile);
                RefreshListBox(lstFiles, fileList.ToArray());
                int lastSlash = newFile.LastIndexOf("\\");
                string newFolder = newFile.Substring(0, lastSlash);
               
                if (!newFolder.Contains("\\"))
                    newFolder = newFolder + "\\";
                ListStringAddToOrSwitchToFirst(folderList, newFolder);
                RefreshListBox(lstFolders, folderList.ToArray());
                Process proc = new Process();
                proc.StartInfo.FileName = newFile;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
        }

        private void lstFolders_DoubleClick(object sender, EventArgs e)
        {
            if (lstFolders.SelectedItem != null)

                    if (Directory.Exists(lstFolders.SelectedItem.ToString()))
                {
                    Process.Start(@lstFolders.SelectedItem.ToString());
                    ListStringAddToOrSwitchToFirst(folderList, lstFolders.SelectedItem.ToString());
                    RefreshListBox(lstFolders, folderList.ToArray());
                }
            else
                {
                    folderList.Remove(lstFolders.SelectedItem.ToString());
                    RefreshListBox(lstFolders, folderList.ToArray());
                }
        }

        private void lstFiles_DoubleClick(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItem != null)

                if (File.Exists(lstFiles.SelectedItem.ToString()))
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = lstFiles.SelectedItem.ToString();
                    proc.StartInfo.UseShellExecute = true;
                    proc.Start();
                    ListStringAddToOrSwitchToFirst(fileList, lstFiles.SelectedItem.ToString());
                    RefreshListBox(lstFiles, fileList.ToArray());
                }
            else
                {
                    fileList.Remove(lstFiles.SelectedItem.ToString());
                    RefreshListBox(lstFiles, fileList.ToArray());
                }
        }

        private void frmLaunchPad_FormClosing(object sender, FormClosingEventArgs e)
        {
            string specialFolder = AppDomain.CurrentDomain.BaseDirectory + @"LaunchPadFiles";
            string MRUfilesPath = specialFolder + @"\NRUfile.txt";
            string MRUfoldersPath = specialFolder + @"MRUfolders.txt";
            UpdateRecourceFile(fileList.ToArray(), MRUfilesPath);
            UpdateRecourceFile(folderList.ToArray(), MRUfoldersPath);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    
}
