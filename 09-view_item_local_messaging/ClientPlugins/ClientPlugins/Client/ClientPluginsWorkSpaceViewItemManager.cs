using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoOS.Platform.Client;

namespace ClientPlugins.Client
{
    public class ClientPluginsWorkSpaceViewItemManager : ViewItemManager
    {
        public ClientPluginsWorkSpaceViewItemManager() : base("ClientPluginsWorkSpaceViewItemManager")
        {
        }

        public override ViewItemUserControl GenerateViewItemUserControl()
        {
            return new ClientPluginsWorkSpaceViewItemUserControl();
        }

        public override PropertiesUserControl GeneratePropertiesUserControl()
        {
            return new PropertiesUserControl(); //no special properties
        }

    }
}
