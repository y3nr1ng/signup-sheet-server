namespace signup_sheet_server
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.applicationStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.serverAddress = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSignupSheet = new System.Windows.Forms.ToolStripMenuItem();
            this.newUserList = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSignupSheet = new System.Windows.Forms.ToolStripMenuItem();
            this.openUserList = new System.Windows.Forms.ToolStripMenuItem();
            this.serverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startServer = new System.Windows.Forms.ToolStripMenuItem();
            this.stopServer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editPort = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayRegion = new System.Windows.Forms.SplitContainer();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.openFile = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayRegion)).BeginInit();
            this.displayRegion.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applicationStatus,
            this.serverAddress});
            this.statusStrip.Location = new System.Drawing.Point(0, 390);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(984, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 0;
            // 
            // applicationStatus
            // 
            this.applicationStatus.AutoSize = false;
            this.applicationStatus.Name = "applicationStatus";
            this.applicationStatus.Size = new System.Drawing.Size(300, 17);
            this.applicationStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // serverAddress
            // 
            this.serverAddress.AutoSize = false;
            this.serverAddress.Name = "serverAddress";
            this.serverAddress.Size = new System.Drawing.Size(669, 17);
            this.serverAddress.Spring = true;
            this.serverAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.serverToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(984, 24);
            this.menuStrip.TabIndex = 1;
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSignupSheet,
            this.newUserList});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.newToolStripMenuItem.Text = "New";
            // 
            // newSignupSheet
            // 
            this.newSignupSheet.Name = "newSignupSheet";
            this.newSignupSheet.Size = new System.Drawing.Size(143, 22);
            this.newSignupSheet.Text = "Signup Sheet";
            this.newSignupSheet.Click += new System.EventHandler(this.newSignupSheet_Click);
            // 
            // newUserList
            // 
            this.newUserList.Name = "newUserList";
            this.newUserList.Size = new System.Drawing.Size(143, 22);
            this.newUserList.Text = "User List";
            this.newUserList.Click += new System.EventHandler(this.newUserList_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSignupSheet,
            this.openUserList});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // openSignupSheet
            // 
            this.openSignupSheet.Enabled = false;
            this.openSignupSheet.Name = "openSignupSheet";
            this.openSignupSheet.Size = new System.Drawing.Size(143, 22);
            this.openSignupSheet.Text = "Signup Sheet";
            this.openSignupSheet.Click += new System.EventHandler(this.openSignupSheet_Click);
            // 
            // openUserList
            // 
            this.openUserList.Name = "openUserList";
            this.openUserList.Size = new System.Drawing.Size(143, 22);
            this.openUserList.Text = "User List";
            this.openUserList.Click += new System.EventHandler(this.openUserList_Click);
            // 
            // serverToolStripMenuItem
            // 
            this.serverToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startServer,
            this.stopServer,
            this.toolStripSeparator1,
            this.editPort});
            this.serverToolStripMenuItem.Enabled = false;
            this.serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            this.serverToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.serverToolStripMenuItem.Text = "Server";
            // 
            // startServer
            // 
            this.startServer.Name = "startServer";
            this.startServer.Size = new System.Drawing.Size(98, 22);
            this.startServer.Text = "Start";
            this.startServer.Click += new System.EventHandler(this.startServer_Click);
            // 
            // stopServer
            // 
            this.stopServer.Enabled = false;
            this.stopServer.Name = "stopServer";
            this.stopServer.Size = new System.Drawing.Size(98, 22);
            this.stopServer.Text = "Stop";
            this.stopServer.Click += new System.EventHandler(this.stopServer_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(95, 6);
            // 
            // editPort
            // 
            this.editPort.Name = "editPort";
            this.editPort.Size = new System.Drawing.Size(98, 22);
            this.editPort.Tag = "12000";
            this.editPort.Text = "Port";
            this.editPort.Click += new System.EventHandler(this.editPort_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Enabled = false;
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // displayRegion
            // 
            this.displayRegion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayRegion.Location = new System.Drawing.Point(0, 24);
            this.displayRegion.Name = "displayRegion";
            // 
            // displayRegion.Panel1
            // 
            this.displayRegion.Panel1.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            // 
            // displayRegion.Panel2
            // 
            this.displayRegion.Panel2.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.displayRegion.Size = new System.Drawing.Size(984, 366);
            this.displayRegion.SplitterDistance = 493;
            this.displayRegion.SplitterWidth = 2;
            this.displayRegion.TabIndex = 2;
            // 
            // folderBrowser
            // 
            this.folderBrowser.ShowNewFolderButton = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 412);
            this.Controls.Add(this.displayRegion);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "signup-sheet-server";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayRegion)).EndInit();
            this.displayRegion.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.SplitContainer displayRegion;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newSignupSheet;
        private System.Windows.Forms.ToolStripMenuItem newUserList;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSignupSheet;
        private System.Windows.Forms.ToolStripMenuItem openUserList;
        private System.Windows.Forms.ToolStripStatusLabel applicationStatus;
        private System.Windows.Forms.ToolStripStatusLabel serverAddress;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.ToolStripMenuItem startServer;
        private System.Windows.Forms.ToolStripMenuItem stopServer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem editPort;
    }
}

