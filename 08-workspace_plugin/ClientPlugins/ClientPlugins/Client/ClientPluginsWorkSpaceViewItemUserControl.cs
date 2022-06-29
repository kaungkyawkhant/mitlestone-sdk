using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Client;

namespace ClientPlugins.Client
{
    public partial class ClientPluginsWorkSpaceViewItemUserControl : ViewItemUserControl
    {
        public ClientPluginsWorkSpaceViewItemUserControl()
        {
            InitializeComponent();

            ClientControl.Instance.RegisterUIControlForAutoTheming(this);
        }

        public override void Init()
        {
        }

        public override void Close()
        {
        }

        /// <summary>
        /// Do not show the sliding toolbar!
        /// </summary>
        public override bool ShowToolbar
        {
            get { return false; }
        }

        private void ViewItemUserControlClick(object sender, EventArgs e)
        {
            FireClickEvent();
        }

        private void ViewItemUserControlDoubleClick(object sender, EventArgs e)
        {
            FireDoubleClickEvent();
        }

    }
}
