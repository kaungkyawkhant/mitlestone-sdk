using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VideoOS.Platform.Client;

namespace ControllerMonitor.Client
{
    /// <summary>
    /// This UserControl is created by the PluginDefinition and placed on the Smart Client's options dialog when the user selects the options icon.<br/>
    /// The UserControl will be added to the owning parent UserControl and docking set to Fill.
    /// </summary>
    public partial class ControllerMonitorOptionsDialogUserControl : OptionsDialogUserControl
    {
        public ControllerMonitorOptionsDialogUserControl()
        {
            InitializeComponent();
        }

        public override void Init()
        {
        }

        public override void Close()
        {
        }

    }
}
