using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VideoOS.Platform.Client;

namespace ClientPlugins.Client
{
    public class ClientPluginsWorkSpaceViewItemPlugin : ViewItemPlugin
    {
        private static System.Drawing.Image _treeNodeImage;

        public ClientPluginsWorkSpaceViewItemPlugin()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.GetName().Name;
            _treeNodeImage = System.Drawing.Image.FromStream(assembly.GetManifestResourceStream(name + ".Resources.ClientPluginsWorkSpace.bmp"));
        }

        public override Guid Id
        {
            get { return ClientPluginsDefinition.ClientPluginsWorkSpaceViewItemPluginId; }
        }

        public override System.Drawing.Image Icon
        {
            get { return _treeNodeImage; }
        }

        public override string Name
        {
            get { return "WorkSpace Plugin View Item"; }
        }

        public override bool HideSetupItem
        {
            get
            {
                return false;
            }
        }

        public override ViewItemManager GenerateViewItemManager()
        {
            return new ClientPluginsWorkSpaceViewItemManager();
        }

        public override void Init()
        {
        }

        public override void Close()
        {
        }


    }
}
