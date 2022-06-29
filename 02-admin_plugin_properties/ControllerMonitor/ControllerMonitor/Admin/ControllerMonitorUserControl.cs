using System;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Admin;
using VideoOS.Platform.UI;

namespace ControllerMonitor.Admin
{
    /// <summary>
    /// This UserControl only contains a configuration of the Name for the Item.
    /// The methods and properties are used by the ItemManager, and can be changed as you see fit.
    /// </summary>
    public partial class ControllerMonitorUserControl : UserControl
    {
        internal event EventHandler ConfigurationChangedByUser;


        public ControllerMonitorUserControl()
        {
            InitializeComponent();
        }

        internal String DisplayName
        {
            get { return textBoxName.Text; }
            set { textBoxName.Text = value; }
        }

        internal bool EnabledCheck
        {
            get { return checkBoxEnabled.Checked; }
            set { checkBoxEnabled.Checked = value; }
        }

        internal string IPAddress
        {
            get { return textBoxIP.Text; }
            set { textBoxIP.Text = value; }
        }

        /// <summary>
        /// Ensure that all user entries will call this method to enable the Save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnUserChange(object sender, EventArgs e)
        {
            if (ConfigurationChangedByUser != null)
                ConfigurationChangedByUser(this, new EventArgs());
        }

        internal void FillContent(Item item)
        {
            textBoxName.Text = item.Name;

            if (item.Properties.ContainsKey("IPAddress"))
            {
                IPAddress = item.Properties["IPAddress"];
            }
            if (item.Properties.ContainsKey("Enabled"))
            {
                EnabledCheck = item.Properties["Enabled"] == "Yes";
            }
        }

        internal void UpdateItem(Item item)
        {
            item.Name = DisplayName;

            item.Properties["IPAddress"] = IPAddress;
            item.Properties["Enabled"] = checkBoxEnabled.Checked ? "Yes" : "No";
        }

        internal void ClearContent()
        {
            textBoxName.Text = "";
            textBoxIP.Text = "";
            checkBoxEnabled.Checked = true;
        }

    }
}
