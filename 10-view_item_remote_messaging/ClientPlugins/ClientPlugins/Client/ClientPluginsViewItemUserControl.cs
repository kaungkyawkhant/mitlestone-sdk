using System;
using System.Drawing;
using System.Windows.Forms;
using VideoOS.Platform;
using VideoOS.Platform.Client;
using VideoOS.Platform.Messaging;

namespace ClientPlugins.Client
{
    /// <summary>
    /// The ViewItemUserControl is instantiated for every position it is created on the current visible view. When a user select another view or viewlayout, this class will be disposed.  No permanent settings can be saved in this class.
    /// The Init() method is called when the class is initiated and handle has been created for the UserControl. Please perform resource initialization in this method.
    /// <br/>
    /// If Message communication is performed, register the MessageReceivers during the Init() method and UnRegister the receivers during the Close() method.
    /// <br/>
    /// The Close() method can be used to Dispose resources in a controlled manor.
    /// <br/>
    /// Mouse events not used by this control, should be passed on to the Smart Client by issuing the following methods:<br/>
    /// FireClickEvent() for single click<br/>
    ///	FireDoubleClickEvent() for double click<br/>
    /// The single click will be interpreted by the Smart Client as a selection of the item, and the double click will be interpreted to expand the current viewitem to fill the entire View.
    /// </summary>
    public partial class ClientPluginsViewItemUserControl : ViewItemUserControl
    {
        #region Component private class variables

        private ClientPluginsViewItemManager _viewItemManager;
        private object _themeChangedReceiver;

        // messaging
        private MessageCommunication _messageCommunication;
        private object _colorChange;

        #endregion

        #region Component constructors + dispose

        /// <summary>
        /// Constructs a ClientPluginsViewItemUserControl instance
        /// </summary>
        public ClientPluginsViewItemUserControl(ClientPluginsViewItemManager viewItemManager)
        {
            _viewItemManager = viewItemManager;

            InitializeComponent();

            ClientControl.Instance.RegisterUIControlForAutoTheming(panelMain);

            panelHeader.BackColor = ClientControl.Instance.Theme.ViewItemHeaderColor;
            panelHeader.ForeColor = ClientControl.Instance.Theme.ViewItemHeaderTextColor;

        }

        private void SetUpApplicationEventListeners()
        {
            //set up ViewItem event listeners
            _viewItemManager.PropertyChangedEvent += new EventHandler(ViewItemManagerPropertyChangedEvent);

            _themeChangedReceiver = EnvironmentManager.Instance.RegisterReceiver(new MessageReceiver(ThemeChangedIndicationHandler),
                                             new MessageIdFilter(MessageId.SmartClient.ThemeChangedIndication));

            MessageCommunicationManager.Start(EnvironmentManager.Instance.MasterSite.ServerId);
            _messageCommunication = MessageCommunicationManager.Get(EnvironmentManager.Instance.MasterSite.ServerId);
            _colorChange = _messageCommunication.RegisterCommunicationFilter(ColorChangeHandler, new CommunicationIdFilter(ClientPluginsDefinition.ColorChange));
        }

        private void RemoveApplicationEventListeners()
        {
            //remove ViewItem event listeners
            _viewItemManager.PropertyChangedEvent -= new EventHandler(ViewItemManagerPropertyChangedEvent);

            EnvironmentManager.Instance.UnRegisterReceiver(_themeChangedReceiver);
            _themeChangedReceiver = null;

            EnvironmentManager.Instance.UnRegisterReceiver(_colorChange);
            _colorChange = null;
        }
        
        private object ColorChangeHandler(VideoOS.Platform.Messaging.Message message, FQID dest, FQID source)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MessageReceiver(ColorChangeHandler), message, dest, source);
            }
            else
            {
                try
                {
                    Color colordata = (Color)message.Data;
                    if (colordata != null)
                    {
                        panelMain.BackColor = colordata;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return null;
        }

        /// <summary>
        /// Method that is called immediately after the view item is displayed.
        /// </summary>
        public override void Init()
        {
            SetUpApplicationEventListeners();
        }

        /// <summary>
        /// Method that is called when the view item is closed. The view item should free all resources when the method is called.
        /// Is called when userControl is not displayed anymore. Either because of 
        /// user clicking on another View or Item has been removed from View.
        /// </summary>
        public override void Close()
        {
            RemoveApplicationEventListeners();
        }

        #endregion

        #region Print method
        /// <summary>
        /// Method that is called when print is activated while the content holder is selected.
        /// Base implementation calls 'DrawToBitmap" to get a bitmap from the entire UserControl,
        /// and pass on to Print method.
        /// <code>
        /// Bitmap bitmap = new Bitmap(this.Width, this.Height);
        /// this.DrawToBitmap(bitmap, new Rectangle(0, 0, this.Width, this.Height));
        /// ClientControl.Instance.Print(bitmap, this.Name, "");
        /// bitmap.Dispose();
        /// </code>
        /// This implementation does not work for usercontrols with embedded ImageViewerControl.cs
        /// </summary>
        public override void Print()
        {
            Print("Name of this item", "Some extra information");
        }

        #endregion


        #region Component events

        private void ViewItemUserControlMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                FireClickEvent();
            }
            else if (e.Button == MouseButtons.Right)
            {
                FireRightClickEvent(e);
            }
        }

        private void ViewItemUserControlMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                FireDoubleClickEvent();
            }
        }


        /// <summary>
        /// Signals that the form is right clicked
        /// </summary>
        public event EventHandler RightClickEvent;

        /// <summary>
        /// Activates the RightClickEvent
        /// </summary>
        /// <param name="e">Event args</param>
        protected virtual void FireRightClickEvent(EventArgs e)
        {
            if (RightClickEvent != null)
            {
                RightClickEvent(this, e);
            }
        }

        void ViewItemManagerPropertyChangedEvent(object sender, EventArgs e)
        {
            labelName.Text = _viewItemManager.SomeName;
        }

        private object ThemeChangedIndicationHandler(VideoOS.Platform.Messaging.Message message, FQID destination, FQID source)
        {
            this.Selected = _selected;
            return null;
        }


        #endregion

        #region Component properties

        /// <summary>
        /// Gets boolean indicating whether the view item can be maximized or not. <br/>
        /// The content holder should implement the click and double click events even if it is not maximizable. 
        /// </summary>
        public override bool Maximizable
        {
            get { return true; }
        }

        /// <summary>
        /// Tell if ViewItem is selectable
        /// </summary>
        public override bool Selectable
        {
            get { return true; }
        }

        /// <summary>
        /// Make support for Theme colors to show if this ViewItem is selected or not.
        /// </summary>
        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                base.Selected = value;
                if (value)
                {
                    panelHeader.BackColor = ClientControl.Instance.Theme.ViewItemSelectedHeaderColor;
                    panelHeader.ForeColor = ClientControl.Instance.Theme.ViewItemSelectedHeaderTextColor;
                }
                else
                {
                    panelHeader.BackColor = ClientControl.Instance.Theme.ViewItemHeaderColor;
                    panelHeader.ForeColor = ClientControl.Instance.Theme.ViewItemHeaderTextColor;
                }
            }
        }


        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            panelMain.BackColor = Color.Red;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panelMain.BackColor = Color.Blue;
        }

        // use local messaging to inform the Smart Client to minimize
        private void button5_Click(object sender, EventArgs e)
        {
            EnvironmentManager.Instance.SendMessage(
                new VideoOS.Platform.Messaging.Message(
                    MessageId.SmartClient.ApplicationControlCommand
                    )
                { Data = ApplicationControlCommandData.Minimize });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            VideoOS.Platform.Messaging.Message colorChange = new VideoOS.Platform.Messaging.Message(ClientPluginsDefinition.ColorChange);
            colorChange.Data = Color.Red;
            _messageCommunication.TransmitMessage(colorChange, null, null, null);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            VideoOS.Platform.Messaging.Message colorChange = new VideoOS.Platform.Messaging.Message(ClientPluginsDefinition.ColorChange);
            colorChange.Data = Color.Blue;
            _messageCommunication.TransmitMessage(colorChange, null, null, null);
        }
    }
}
