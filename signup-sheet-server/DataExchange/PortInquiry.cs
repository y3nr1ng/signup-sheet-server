using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace signup_sheet_server.DataExchange
{
    public partial class PortInquiry : Form
    {
        private int port;

        public PortInquiry(int originalPort)
        {
            InitializeComponent();

            // Store the original port.
            this.port = originalPort;
            // ... and display it, select it.
            this.portInput.Text = this.port.ToString();
            this.portInput.SelectAll();
        }

        #region Button functions.

        private void okButton_Click(object sender, EventArgs e)
        {
            string newPort = this.portInput.Text;
            int parsedNewPort;
            // Testing whether the port is valid.
            if(Int32.TryParse(newPort, out parsedNewPort))
            {
                if(parsedNewPort != this.port)
                {
                    // Write result only when new port and the original port are different.
                    this.port = parsedNewPort;
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }
            }
            else
            {
                DialogResult status = MessageBox.Show("Entered port can't parse as integer, numbers only.",
                                                      "Invalid port number.",
                                                      MessageBoxButtons.RetryCancel,
                                                      MessageBoxIcon.Exclamation);
                if(status == DialogResult.Retry)
                {
                    return;
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }
            }
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void portInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                this.okButton.PerformClick();
            }
        }

        #endregion

        #region Exposed public functions.

        public int Port
        {
            get
            {
                return this.port;
            }
        }

        #endregion  
    }
}
