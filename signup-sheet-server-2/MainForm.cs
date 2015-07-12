using signup_sheet_server.DataExchange;
using signup_sheet_server.Panels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace signup_sheet_server
{
    public partial class MainForm : Form
    {
        #region Initialization.

        public MainForm()
        {
            InitializeComponent();
            InitializeDefaultView();
        }

        private void InitializeDefaultView()
        {
            // Background instruction of the panels.
            SetPanelDefaultView(this.displayRegion.Panel1, "Open > Signup Sheet", Color.DarkGray);
            SetPanelDefaultView(this.displayRegion.Panel2, "Open > User List", Color.DarkGray);

            // Server status.
            serverAddress.Text = "Server down.";
        }
        private void SetPanelDefaultView(Panel panel, string message, Color textColor)
        {
            // Create default label message.
            Label background = new Label();
            background.Text = message;
            background.Font = new Font(background.Font.FontFamily, 16);
            background.ForeColor = textColor;

            // Alignment.
            background.Dock = DockStyle.Fill;
            background.TextAlign = ContentAlignment.MiddleCenter;

            // Object property.
            background.Name = "background";

            panel.Controls.Add(background);
        }

        #endregion

        #region Create file.

        private void newSignupSheet_Click(object sender, EventArgs e)
        {
            DialogResult status;

            string filePath = ChooseFolder();
            if(filePath.Length == 0)
            {
                return;
            }

            using(SignupSheetDetail details = new SignupSheetDetail())
            {
                status = details.ShowDialog();

                if(status == DialogResult.OK)
                {
                    bool showNewSignupSheet = true;

                    if(!IsUserListLoaded())
                    {
                        status = MessageBox.Show("Please load a user list to continue.",
                                                 "No user list loaded.",
                                                 MessageBoxButtons.OKCancel,
                                                 MessageBoxIcon.Question);
                        if(status == DialogResult.OK)
                        {
                            this.openUserList.PerformClick();
                        }
                        else
                        {
                            showNewSignupSheet = false;
                        }
                    }

                    // Force save the user list.
                    DisplayUserList userList = this.displayRegion.Panel2.Controls["display"] as DisplayUserList;
                   userList.ForceSave();

                    if(showNewSignupSheet)
                    {
                        // Create a dummy signup sheet object.
                        SignupSheet newSheet = new SignupSheet();
                        newSheet.SessionName = details.SessionName;
                        newSheet.StartTime = details.StartTime;
                        newSheet.EndTime = details.EndTime;

                        // Import the users from the user list.
                        BindingList<Entry> templateEntries = new BindingList<Entry>();
                        foreach(UserInfo user in userList.GetUsers())
                        {
                            Entry newEntry = new Entry();
                            newEntry.Name = user.FirstName + ' ' + user.LastName;
                            newEntry.Signed = false;
                            templateEntries.Add(newEntry);
                        }
                        newSheet.Entries = templateEntries;

                        // Create the object and add to panel.
                        DisplaySignupSheet display = new DisplaySignupSheet(this, filePath, newSheet);
                        AddDisplayToPanel(this.displayRegion.Panel1, display);

                        // Write hint on the status bar.
                        this.applicationStatus.Text = "Right click on the list to close it.";

                        this.serverToolStripMenuItem.Enabled = true;
                    }
                    else
                    {
                        this.applicationStatus.Text = "Abort create new signup sheet.";
                        this.applicationStatus.ForeColor = Color.Red;
                    }
                }
                else
                {
                    this.applicationStatus.Text = "Abort creation.";
                }
            }
        }

        private void newUserList_Click(object sender, EventArgs e)
        {
            // Use the original file browser, but disable the fileExists block.
            this.openFile.CheckFileExists = false;

            string filePath = ChooseFile();
            if(filePath.Length == 0)
            {
                return;
            }

            OpenUserList(filePath);
        }

        #endregion

        #region Open file.

        private void openSignupSheet_Click(object sender, EventArgs e)
        {
            // Load the form into the panel.
            if(this.displayRegion.Panel1.Controls.ContainsKey("display"))
            {
                this.applicationStatus.Text = "Closing opened file...";
                //this.displayRegion.Panel1.Controls["display"].Close();
                this.displayRegion.Panel1.Controls.RemoveByKey("display");

                // Restore the backround guidance first.
                this.displayRegion.Panel1.Controls["background"].Visible = true;
            }

            // Ask for file.
            string filePath = ChooseFile();
            if(filePath.Length == 0)
            {
                return;
            }

            OpenSignupSheet(filePath);
        }
        private void OpenSignupSheet(string filePath)
        {
            try
            {
                // Create the object and add to panel.
                DisplaySignupSheet display = new DisplaySignupSheet(this, filePath);
                AddDisplayToPanel(this.displayRegion.Panel1, display);

                // Write hint on the status bar.
                this.applicationStatus.Text = "Right click on the list to close it.";

                this.serverToolStripMenuItem.Enabled = true;
            }
            catch
            {
                // Not loaded.
                this.serverToolStripMenuItem.Enabled = false;

                // Only file problem can cause issue here, so all the exceptions are catched here.
                this.applicationStatus.Text = "Failed to open the file.";
                MessageBox.Show("There are unsupported strings in the file.",
                                "Failed to open.",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Stop);
            }
        }

        private void openUserList_Click(object sender, EventArgs e)
        {
            // Check if another file is opened.
            if(this.displayRegion.Panel2.Controls.ContainsKey("display"))
            {
                this.applicationStatus.Text = "Closing opened file...";
                //this.displayRegion.Panel2.Controls["display"].Close();
                this.displayRegion.Panel2.Controls.RemoveByKey("display");

                // Restore the backround guidance first.
                this.displayRegion.Panel2.Controls["background"].Visible = true;
            }

            // Ask for file.
            string filePath = ChooseFile();
            if(filePath.Length == 0)
            {
                return;
            }

            OpenUserList(filePath);
        }
        private void OpenUserList(string filePath)
        {
            try
            {
                // Create the object and add to panel.
                DisplayUserList display = new DisplayUserList(this, filePath);
                AddDisplayToPanel(this.displayRegion.Panel2, display);

                // Write hint on the status bar.
                this.applicationStatus.Text = "Right click on the list to close it.";

                this.openSignupSheet.Enabled = true;
            }
            catch
            {
                // Not loaded.
                this.openSignupSheet.Enabled = false;

                // Only file problem can cause issue here, so all the exceptions are catched here.
                this.applicationStatus.Text = "Failed to open the file.";
                MessageBox.Show("There are unsupported strings in the file.",
                                "Failed to open.",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Stop);
            }
        }

        private void AddDisplayToPanel(Panel parent, Control display)
        {
            // Hide the background hint.
            parent.Controls["background"].Visible = false;

            // Change the docking style and its key.
            display.Dock = DockStyle.Fill;
            display.Name = "display";
            display.Disposed += new EventHandler(Disposed_RestoreBackground);

            // Add to the panel.
            parent.Controls.Add(display);
        }
        void Disposed_RestoreBackground(object sender, EventArgs e)
        {
            // Hardcoded for panel1 and panel2.
            if(!this.displayRegion.Panel1.Controls.ContainsKey("display"))
            {
                this.serverToolStripMenuItem.Enabled = false;
                this.displayRegion.Panel1.Controls["background"].Visible = true;
            }
            if(!this.displayRegion.Panel2.Controls.ContainsKey("display"))
            {
                this.openSignupSheet.Enabled = false;
                this.displayRegion.Panel2.Controls["background"].Visible = true;
            }

            // Wipe the status bar.
            this.applicationStatus.Text = string.Empty;
        }

        #endregion

        #region GUI support functions.

        public bool IsUserListLoaded()
        {
            return this.displayRegion.Panel2.Controls.ContainsKey("display");
        }

        public bool IsSignupSheetLoaded()
        {
            return this.displayRegion.Panel1.Controls.ContainsKey("display");
        }

        #endregion

        #region Dialog helper function.

        private string ChooseFolder()
        {
            DialogResult status = this.folderBrowser.ShowDialog();
            if(status == DialogResult.OK)
            {
                return this.folderBrowser.SelectedPath;
            }
            else
            {
                return string.Empty;
            }
        }

        private string ChooseFile()
        {
            DialogResult status = this.openFile.ShowDialog();
            if(status == DialogResult.OK)
            {
                return this.openFile.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region Server control.

        private Communication server = new Communication();
        private int port = 12000;

        private void startServer_Click(object sender, EventArgs e)
        {
            // Toggle control switch.
            this.startServer.Enabled = false;

            this.server.StartServer(this, this.port);
            // Set the address tag.
            this.serverAddress.Text = this.server.GetIp() + ':' + this.port.ToString();
            this.serverAddress.ForeColor = Color.Blue;

            // Enable the other one.
            this.stopServer.Enabled = true;
        }
        private void stopServer_Click(object sender, EventArgs e)
        {
            // Toggle control switch.  
            this.stopServer.Enabled = false;

            this.server.StopServer();
            // Restore the server status.
            this.serverAddress.Text = "Server down.";
            this.serverAddress.ForeColor = Color.Black;

            // Enable the other one.
            this.startServer.Enabled = true;
        }
        private void editPort_Click(object sender, EventArgs e)
        {
            using(PortInquiry newPort = new PortInquiry(this.port))
            {
                DialogResult status = newPort.ShowDialog();
                if(status == DialogResult.OK)
                {
                    Console.WriteLine("Updated.");

                    this.port = newPort.Port;
                }
            }
        }

        #endregion

        #region Exposed external functions for form crosstalks.

        public void Signup(string name)
        {
            DisplaySignupSheet sheet = this.displayRegion.Panel1.Controls["display"] as DisplaySignupSheet;
            sheet.Signup(name);
        }

        public UserInfo GetUserInfo(string cardId)
        {
            DisplayUserList userList = this.displayRegion.Panel2.Controls["display"] as DisplayUserList;
            return userList.GetUserInfo(cardId);
        }

        public void SetStatusMessage(string message, Color textColor)
        {
            this.applicationStatus.Text = message;
            this.applicationStatus.ForeColor = textColor;
        }

        public void CloseSignupSheet()
        {
            DisplaySignupSheet sheet = this.displayRegion.Panel1.Controls["display"] as DisplaySignupSheet;
            sheet.SaveToFile();
            sheet.Dispose();
        }

        #endregion
    }
}
