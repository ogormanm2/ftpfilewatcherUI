using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace FileWatcher
{
    public partial class frmMain : Form
    {
        private Thread nodeThread;
        
        // Declare a Name property of type string:
        public Thread MyThread
        {
            get
            {
                return nodeThread;
            }
            set
            {
                nodeThread = value;
            }
        }
        
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            string strJson = File.ReadAllText(@"C:\Users\Matthew\Proj\Node.JS\ftpfilewatcher\config.json");
            dynamic json = JsonConvert.DeserializeObject(strJson);
            txtSourceDir.Text = json.sourcedir;
            txtFTPHost.Text = json.host;
            cmbPort.SelectedText = json.port;
            txtFTPUser.Text = json.user;
            mskFTPPassword.Text = json.password;
            
            Thread thread = new Thread(CaptureStdoutThread);
            thread.IsBackground = true;
            thread.Start();

            // Load ListBox with log
            LoadLogFileList();
        }

        public void ChooseFolder()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtSourceDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            ChooseFolder();
        }

        private void LoadLogFileList()
        {
            DataTable dt = new DataTable();
            // Clear control
            //lstLogOutput.Clear();
            CassandraDAL cassDAL = new CassandraDAL();
            cassDAL.Open();
            dt = cassDAL.GetDataTable("select * from ftpfileaudit");

            // Display items in the ListView control
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow drow = dt.Rows[i];

                // Only row that have not been deleted
                if (drow.RowState != DataRowState.Deleted)
                {
                    // Define the list items
                    ListViewItem lvi = new ListViewItem(drow["job"].ToString());
                    lvi.SubItems.Add(drow["status"].ToString());
                    lvi.SubItems.Add(drow["filename"].ToString());
                    lvi.SubItems.Add(drow["path"].ToString());
                    lvi.SubItems.Add(drow["datecreated"].ToString());

                    // Add the list items to the ListView
                    lstLogOutput.Items.Add(lvi);
                }
            }
            cassDAL.Close();
        }         

       

        private void RunNodeFileWatcher()
        {
            //int lineCount = 0;
            StringBuilder output = new StringBuilder();
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            //cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            /*
            cmd.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                // Prepend line numbers to each line of the output.
                if (!String.IsNullOrEmpty(e.Data))
                {
                    lineCount++;
                    output.Append("\n[" + lineCount + "]: " + e.Data);
                    AppendLog("\n[" + lineCount + "]: " + e.Data, Color.Blue, true);
                }
            });
             */
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.WorkingDirectory = @"C:\Users\Matthew\Proj\Node.JS\filewatcher";
            cmd.StartInfo.Arguments = "/c node app.js";
            cmd.Start();
            // Asynchronously read the standard output of the spawned process. 
            // This raises OutputDataReceived events for each line of output.
            //cmd.BeginOutputReadLine();
            cmd.WaitForExit();
            cmd.Close();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(RunNodeFileWatcher);
            thread.Start();
            MyThread = thread;
        }

        private void CaptureStdoutThread()
        {

        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            if (this.MyThread.IsAlive)
                this.MyThread.Abort();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FileWatcherConfig fc = new FileWatcherConfig();
            fc.sourcedir = txtSourceDir.Text;
            fc.host = txtFTPHost.Text;
            int selectedIndex = cmbPort.SelectedIndex;
            Object selectedItem = cmbPort.SelectedItem;
            fc.port = Convert.ToInt32(selectedItem.ToString());
            fc.username = txtFTPUser.Text;
            fc.password = mskFTPPassword.Text;
            

            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(@"C:\Users\Matthew\Proj\Node.JS\filewatcher\config.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, fc);
            }
        }
    }
}
