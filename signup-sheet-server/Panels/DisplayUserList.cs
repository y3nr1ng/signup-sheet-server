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
using signup_sheet_server.CardReader;

namespace signup_sheet_server.Panels
{
    public partial class DisplayUserList : UserControl
    {
        private string filePath;
        private BindingList<UserInfo> userList = null;

        private bool isDirty;

        private MainForm form = null;

        public DisplayUserList(Control parent, string filePath)
        {
            InitializeComponent();

            // Reassign its parent.
            this.Parent = parent;
            // Find the MainForm.
            form = FindMainForm(this.Parent) as MainForm;

            // Store the file path, which is the basis of a document.
            this.filePath = filePath;

            // Load the file after the construction.
            LoadFromFile();
        }
        private Control FindMainForm(Control child)
        {
            Control parent = child;
            while(!(parent is MainForm))
            {
                parent = parent.Parent;
            }

            return parent;
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
                    userList = JsonConvert.DeserializeObject<BindingList<UserInfo>>(content);
                }
            }
            else
            {
                this.isDirty = true;
            }

            // Bind the data source after instantiate the BindingList.
            this.dataViewer.DataSource = new BindingSource(this.userList, null);
        }
        public void SaveToFile()
        {
            if(this.userList == null)
            {
                // Prevent the file stores a null inside.
                this.userList = new BindingList<UserInfo>();
            }

            using(StreamWriter writer = new StreamWriter(this.filePath, false))
            {
                string content = JsonConvert.SerializeObject(this.userList);
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

            // Need to close the signup sheet if it's opened.
            if(form.IsSignupSheetLoaded())
            {
                form.CloseSignupSheet();
            }

            // Rest of the options lead to the disposal of this form.
            this.Dispose();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Remain the possibility for multiselection.
            foreach(DataGridViewRow item in this.dataViewer.SelectedRows)
            {
                this.form.SetStatusMessage("Reg ID \"" + item.Cells["regId"].Value + "\" deleted.", Color.Black);
                this.dataViewer.Rows.RemoveAt(item.Index);
            }

            this.isDirty = true;
        }

        #region Card reader.

        private void updateCardIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReaderWrapper cardReader = new ReaderWrapper();
            cardReader.Open();

            string newCardId;
            if(!cardReader.Scan(out newCardId))
            {
                cardReader.Close();

                DialogResult status = MessageBox.Show("Can't scan the card, please make sure the reader is plugged.",
                                                      "Failed to scan card.",
                                                      MessageBoxButtons.RetryCancel,
                                                      MessageBoxIcon.Stop);
                if(status == DialogResult.Retry)
                {
                    this.updateCardIDToolStripMenuItem.PerformClick();
                }
                return;
            }

            // Remain the possibility for multiselection.
            // Every selected items will have the same card ID.
            foreach(DataGridViewRow item in this.dataViewer.SelectedRows)
            {
                this.form.SetStatusMessage("Card ID updated for \"" + item.Cells["regId"].Value.ToString() + "\".", Color.Black);
                item.Cells["cardId"].Value = newCardId;
            }

            this.isDirty = true;
        }

        #endregion

        #endregion

        #region Exposed public functions.

        public BindingList<UserInfo> GetUsers()
        {
            return this.userList;
        }

        private delegate void InvokeDelegate();
        private string cardIdTransporter = string.Empty;
        public UserInfo GetUserInfo(string cardId)
        {
            // Iterate through all enlisted users.
            foreach(UserInfo user in this.userList)
            {
                if(user.CardId == cardId)
                {
                    // Since the callee of this is in another thread, invoke delegate is needed.
                    this.BeginInvoke(new InvokeDelegate(SelectUser));
                    this.cardIdTransporter = cardId;
                    return user;
                }
            }

            return null;
        }
        public void SelectUser()
        {
            // Find the list.
            int index = 0;
            foreach(DataGridViewRow row in this.dataViewer.Rows)
            {
                if((row.Cells["cardId"].Value as string) == this.cardIdTransporter)
                {
                    index = row.Index;
                    break;
                }
            }
            // Select the user.
            this.dataViewer.FirstDisplayedScrollingRowIndex = index;
        }

        public void ForceSave()
        {
            this.isDirty = true;
            this.saveToolStripMenuItem.PerformClick();
        }

        private void dataViewer_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            this.isDirty = true;
        }

        #endregion
    }

    public class UserInfo
    {
        [JsonProperty("firstName")]
        private string firstName;
        [JsonIgnore]
        public string FirstName
        {
            get
            {
                return this.firstName;
            }
            set
            {
                this.firstName = value;
            }
        }

        [JsonProperty("lastName")]
        private string lastName;
        [JsonIgnore]
        public string LastName
        {
            get
            {
                return this.lastName;
            }
            set
            {
                this.lastName = value;
            }
        }

        [JsonProperty("regId")]
        private string regId;
        [JsonIgnore]
        public string RegId
        {
            get
            {
                return this.regId;
            }
            set
            {
                this.regId = value;
            }
        }

        [JsonProperty("cardId")]
        private string cardId;
        [JsonIgnore]
        public string CardId
        {
            get
            {
                return this.cardId;
            }
            set
            {
                this.cardId = value;
            }
        }
    }
}
