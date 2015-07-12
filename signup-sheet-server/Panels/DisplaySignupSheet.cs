using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

namespace signup_sheet_server.Panels
{
    public partial class DisplaySignupSheet : UserControl
    {
        private string filePath;
        private SignupSheet sheet = null;

        private const int nameIndex = 0;
        private const int signedIndex = 1;
        private const int timestampIndex = 2;

        private bool dataLoaded;
        private bool isDirty;

        public DisplaySignupSheet(Control parent, string filePath, SignupSheet newSheet)
        {
            this.dataLoaded = false;

            InitializeComponent();

            // Reassign its parent.
            this.Parent = parent;

            // Generate the new file path.
            string startTimeString = newSheet.StartTime.ToString("yyyy-MM-dd-HH-mm-ss");
            string endTimeString = newSheet.EndTime.ToString("yyyy-MM-dd-HH-mm-ss");
            filePath += '\\' + newSheet.SessionName + '_' + startTimeString + '_' + endTimeString + ".json";
            Console.WriteLine("New signup sheet location: " + filePath);

            // Store the file path, which is the basis of a document.
            this.filePath = filePath;

            // Use the assigned sheet instead of dump it from the file.
            this.sheet = newSheet;
            // Bind the data source after instantiate the BindingList.
            this.dataViewer.DataSource = new BindingSource(this.sheet.Entries, null);
            this.dataLoaded = true;

            // Save immediately, since this file is newly created.
            SaveToFile();
        }

        public DisplaySignupSheet(Control parent, string filePath)
        {
            this.dataLoaded = false;

            InitializeComponent();

            // Reassign its parent.
            this.Parent = parent;

            // Store the file path, which is the basis of a document.
            this.filePath = filePath;

            // Load the file after the construction.
            LoadFromFile();
        }

        #region File operations.

        public void LoadFromFile()
        {
            // If the file doesn't exist, the table IS dirty upon opening.
            if(File.Exists(this.filePath))
            {
                this.isDirty = false;
                using(StreamReader reader = new StreamReader(this.filePath))
                {
                    string content = reader.ReadToEnd();
                    sheet = JsonConvert.DeserializeObject<SignupSheet>(content);
                }
            }
            else
            {
                this.isDirty = true;
            }

            // Bind the data source after instantiate the BindingList.
            this.dataViewer.DataSource = new BindingSource(this.sheet.Entries, null);

            this.dataLoaded = true;
        }
        public void SaveToFile()
        {
            using(StreamWriter writer = new StreamWriter(this.filePath, false))
            {
                string content = JsonConvert.SerializeObject(this.sheet);
                writer.Write(content);
            }

            // Clear the dirty flag.
            this.isDirty = false;
        }

        #endregion

        #region Context menus.

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dataViewer.Refresh();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveToFile();
        }
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(isDirty)
            {
                DialogResult status = MessageBox.Show("Save before close?",
                                                      "File modified.",
                                                      MessageBoxButtons.YesNoCancel,
                                                      MessageBoxIcon.Question);
                if(status == DialogResult.Yes)
                {
                    SaveToFile();
                }
                else if(status == DialogResult.Cancel)
                {
                    return;
                }
            }

            // Rest of the options lead to the disposal of this form.
            this.Dispose();
        }

        #endregion

        #region Data Grid View monitor.

        private void dataViewer_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if(this.dataLoaded)
            {
                this.isDirty = true;

                if(this.dataViewer.CurrentCell.ColumnIndex == signedIndex)
                {
                    this.dataViewer.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }
        private void dataViewer_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(this.dataLoaded)
            {
                if(e.ColumnIndex == signedIndex)
                {
                    // Update the timestamp column.
                    if((bool)this.dataViewer.Rows[e.RowIndex].Cells[signedIndex].Value)
                    {
                        this.dataViewer.Rows[e.RowIndex].Cells[timestampIndex].Value = DateTime.UtcNow.ToLocalTime();
                    }
                    else
                    {
                        this.dataViewer.Rows[e.RowIndex].Cells[timestampIndex].Value = DateTime.MinValue;
                    }
                }
            }
        }

        #endregion

        #region Signup core code.

        public bool Signup(string name)
        {
            if(InTime())
            {
                // Search for user in the signup sheet.
                foreach(DataGridViewRow row in this.dataViewer.Rows)
                {
                    if((row.Cells[nameIndex].Value as string) == name)
                    {
                        // Move the cursor and the display to here.
                        this.dataViewer.CurrentCell = row.Cells[signedIndex];
                        this.dataViewer.FirstDisplayedScrollingRowIndex = row.Index;

                        // Signup the user.
                        this.dataViewer.CurrentCell.Value = true;

                        // Mark current cell dirty.
                        this.dataViewer.NotifyCurrentCellDirty(true);
                        
                        return true;
                    }
                }

                // Can't find the user in the signup sheet.
                return false;
            }
            else
            {
                // Due.
                return false;
            }
        }
        private bool InTime()
        {
            DateTime current = DateTime.UtcNow.ToLocalTime();
            return ((DateTime.Compare(this.sheet.StartTime, current) <= 0) && (DateTime.Compare(current, this.sheet.EndTime) <= 0));
        }

        #endregion
    }
        
    public class SignupSheet
    {
        [JsonProperty("sessionName")]
        private string sessionName;
        [JsonIgnore]
        public string SessionName
        {
            get
            {
                return this.sessionName;
            }
            set
            {
                this.sessionName = value;
            }
        }

        [JsonProperty("startTime")]
        private DateTime startTime;
        [JsonIgnore]
        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }
            set
            {
                this.startTime = value;
            }
        }

        [JsonProperty("endTime")]
        private DateTime endTime;
        [JsonIgnore]
        public DateTime EndTime
        {
            get
            {
                return this.endTime;
            }
            set
            {
                this.endTime = value;
            }
        }

        [JsonProperty("entries")]
        private BindingList<Entry> entries;
        [JsonIgnore]
        public BindingList<Entry> Entries
        {
            get
            {
                return this.entries;
            }
            set
            {
                this.entries = value;
            }
        }
    }

    public class Entry
    {
        [JsonProperty("name")]
        private string name;
        [JsonIgnore]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        [JsonProperty("signed")]
        private bool signed;
        [JsonIgnore]
        public bool Signed
        {
            get
            {
                return this.signed;
            }
            set
            {
                this.signed = value;
            }
        }

        [JsonProperty("timestamp")]
        private DateTime timestamp;
        [JsonIgnore]
        public DateTime Timestamp
        {
            get
            {
                return this.timestamp;
            }
            set
            {
                this.timestamp = value;
            }
        }
    }
}
