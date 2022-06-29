using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace ClientPlugins.Client
{
    public class ClientPluginsWorkSpacePlugin : WorkSpacePlugin
    {

        private List<object> _messageRegistrationObjects = new List<object>();

        private bool _workSpaceSelected = false;
        private bool _workSpaceViewSelected = false;

        /// <summary>
        /// The Id.
        /// </summary>
        public override Guid Id
        {
            get { return ClientPluginsDefinition.ClientPluginsWorkSpacePluginId; }
        }

        /// <summary>
        /// The name displayed on top
        /// </summary>
        public override string Name
        {
            get { return "eLearning"; }
        }

        /// <summary>
        /// We support setup mode
        /// </summary>
        public override bool IsSetupStateSupported
        {
            get { return true; }
        }

        /// <summary>
        /// Initializa the plugin
        /// </summary>
        public override void Init()
        {
            LoadProperties(true);

            //add message listeners
            _messageRegistrationObjects.Add(EnvironmentManager.Instance.RegisterReceiver(ShownWorkSpaceChangedReceiver, new MessageIdFilter(MessageId.SmartClient.ShownWorkSpaceChangedIndication)));
            _messageRegistrationObjects.Add(EnvironmentManager.Instance.RegisterReceiver(WorkSpaceStateChangedReceiver, new MessageIdFilter(MessageId.SmartClient.WorkSpaceStateChangedIndication)));
            _messageRegistrationObjects.Add(EnvironmentManager.Instance.RegisterReceiver(SelectedViewChangedReceiver, new MessageIdFilter(MessageId.SmartClient.SelectedViewChangedIndication)));

            //build view layout - modify to your needs. Here we use a matrix of 1000x1000 to define the layout 
            List<Rectangle> rectangles = new List<Rectangle>();
            rectangles.Add(new Rectangle(700, 000, 300, 1000));      // Index 0 = view item
            rectangles.Add(new Rectangle(000, 000, 700, 1000));     // web browser
            ViewAndLayoutItem.Layout = rectangles.ToArray();
            ViewAndLayoutItem.Name = Name;

            //add viewitems to view layout

            ViewAndLayoutItem.InsertViewItemPlugin(0, new ClientPluginsViewItemPlugin(), new Dictionary<string, string>());

            Dictionary<String, String> properties2 = new Dictionary<string, string>();
            properties2.Add("URL", "https://learn.milestonesys.com");
            properties2.Add("Scaling", "3"); // fits 800x600px
            properties2.Add("Addscript", "false");
            properties2.Add("HideNavigationBar", "false");
            ViewAndLayoutItem.InsertBuiltinViewItem(1, ViewAndLayoutItem.HTMLBuiltinId, properties2);

        }

        /// <summary>
        /// Close workspace and clean up
        /// </summary>
        public override void Close()
        {
            foreach (object messageRegistrationObject in _messageRegistrationObjects)
            {
                EnvironmentManager.Instance.UnRegisterReceiver(messageRegistrationObject);
            }
            _messageRegistrationObjects.Clear();
        }

        /// <summary>
        /// User modified something in setup mode
        /// </summary>
        /// <param name="index"></param>
        public override void ViewItemConfigurationModified(int index)
        {
            base.ViewItemConfigurationModified(index);

            if (ViewAndLayoutItem.ViewItemId(index) == ViewAndLayoutItem.CameraBuiltinId)
            {
                SetProperty("Camera" + index, ViewAndLayoutItem.ViewItemConfigurationString(index));
                SaveProperties(true);
            }
        }

        /// <summary>
        /// Keep track of what workspace is selected, if this is selected the _workSpaceViewSelected is true.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="related"></param>
        /// <returns></returns>
        private object ShownWorkSpaceChangedReceiver(Message message, FQID sender, FQID related)
        {
            if (message.Data is Item && ((Item)message.Data).FQID.ObjectId == Id)
            {
                _workSpaceSelected = true;
                Notification = null;
            }
            else
            {
                _workSpaceSelected = false;
            }
            return null;
        }

        /// <summary>
        /// Keep track of current state: in setup or normal
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="related"></param>
        /// <returns></returns>
        private object WorkSpaceStateChangedReceiver(Message message, FQID sender, FQID related)
        {
            if (_workSpaceSelected && ((WorkSpaceState)message.Data) == WorkSpaceState.Normal)
            {
                // Went in or out of Setup state
            }
            return null;
        }


        /// <summary>
        /// Keep track of what workspace is selected, if this is selected the _workSpaceViewSelected is true.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="sender"></param>
        /// <param name="related"></param>
        /// <returns></returns>
        private object SelectedViewChangedReceiver(Message message, FQID sender, FQID related)
        {
            if (message.Data is Item && ((Item)message.Data).FQID.ObjectId == ViewAndLayoutItem.FQID.ObjectId)
            {
                _workSpaceViewSelected = true;
            }
            else
            {
                _workSpaceViewSelected = false;
            }
            return null;
        }

        /// <summary>
        /// A simple loop to find any camera - replace with something usefull...
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        private Item FindAnyCamera(List<Item> top)
        {
            if (top != null)
                foreach (Item item in top)
                {
                    if (item.FQID.FolderType == FolderType.No && item.FQID.Kind == Kind.Camera)
                        return item;

                    if (item.FQID.FolderType != FolderType.No)
                    {
                        Item check = FindAnyCamera(item.GetChildren());
                        if (check != null)
                            return check;
                    }
                }
            return null;
        }
    }
}
