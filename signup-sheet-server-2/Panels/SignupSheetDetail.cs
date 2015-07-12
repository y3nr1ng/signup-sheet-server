using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace signup_sheet_server.Panels
{
    public partial class SignupSheetDetail : Form
    {
        private string sessionName;
        private DateTime startTime;
        private DateTime endTime;

        public SignupSheetDetail()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult status;

            if(!ValidateSessionName())
            {
                status = MessageBox.Show("Invalid session name.",
                                         "Can't create new sheet.",
                                         MessageBoxButtons.RetryCancel,
                                         MessageBoxIcon.Stop);
                if(status == DialogResult.Cancel)
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                return;
            }

            if(!ValidateDates())
            {
                status = MessageBox.Show("Invalid start/end time.",
                                         "Can't create new sheet.",
                                         MessageBoxButtons.RetryCancel,
                                         MessageBoxIcon.Stop);
                if(status == DialogResult.Cancel)
                {
                    this.DialogResult = DialogResult.Cancel;
                }
                return;
            }

            // Write the private variables.
            this.sessionName = this.sessionNameInput.Text;
            this.startTime = this.startTimePicker.Value;
            this.endTime = this.endTimePicker.Value;

            this.DialogResult = DialogResult.OK;
        }
        private bool ValidateSessionName()
        {
            if(this.sessionNameInput.TextLength == 0)
            {
                // Select all and reject the result.
                this.sessionNameInput.SelectAll();
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool ValidateDates()
        {
            return (DateTime.Compare(this.startTimePicker.Value, this.endTimePicker.Value) < 0);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        #region Exposed public functions.

        public string SessionName
        {
            get
            {
                return this.sessionName;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return this.endTime;
            }
        }

        #endregion
    }
}
