using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoOS.Platform.Client;

namespace ControllerMonitor.Client
{
    public class ControllerMonitorWorkSpaceViewItemManager : ViewItemManager
    {
        public ControllerMonitorWorkSpaceViewItemManager() : base("ControllerMonitorWorkSpaceViewItemManager")
        {
        }

        public override ViewItemUserControl GenerateViewItemUserControl()
        {
            return new ControllerMonitorWorkSpaceViewItemUserControl();
        }

        public override PropertiesUserControl GeneratePropertiesUserControl()
        {
            return new PropertiesUserControl(); //no special properties
        }

    }
}
