using MissionPlanner;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner.Controls.PreFlight;
using MissionPlanner.Controls;
using System.Linq;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Maps;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Globalization;
using System.Drawing;
using Microsoft.Win32;
using static MissionPlanner.Warnings.CustomWarning;
//TODO: Racionalise namespaces in the plugin
using MpMessageControlStoreNS;
using MpMessagesControl;
using MPMessageControlBoxNS;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

namespace MessagesNGPlugin
{

    public class MessagesNGPlugin : Plugin
    {
        public TabPage newMessagePage = new TabPage();
        private MpMessagesControl.NewMessageControl mpMessagesControl = new MpMessagesControl.NewMessageControl();
        private bool isUndocked = false;
        private DateTime lastDisplayedMessage = DateTime.MinValue;

        //Store for the multi part messages
        private List<MAVLink.mavlink_statustext_t> msgChunks = new List<MAVLink.mavlink_statustext_t>();

        //Last message from messagehigh
        private string lastMessageHigh = "";

        private bool _updateinProgress = false;
        private bool _messageHighWasCleared = true;

        public override string Name
        {
            get { return "MessagesNGPlugin"; }
        }

        public override string Version
        {
            get { return "1.0"; }
        }

        public override string Author
        {
            get { return "Andras \"EosBandi\" Schaffer"; }
        }

        //[DebuggerHidden]
        public override bool Init()
		//Init called when the plugin dll is loaded
        {
            loopratehz = 1;  //Loop runs every second (The value is in Hertz, so 2 means every 500ms, 0.1f means every 10 second...) 
            return true;	 // If it is false then plugin will not load
        }


        public override bool Loaded()
		//Loaded called after the plugin dll successfully loaded
        {
            newMessagePage.Size = new System.Drawing.Size(200, 150);
            newMessagePage.Text = "MessagesNG";
            newMessagePage.Name = "MessagesNG";

            mpMessagesControl.Name = "messagebox";
            mpMessagesControl.Dock = DockStyle.Fill;
            //mpMessagesControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            mpMessagesControl.Location = new System.Drawing.Point(3, 3);
            mpMessagesControl.Size = new System.Drawing.Size(180, 140);
            mpMessagesControl.TabIndex = 0;
            //mpMessagesControl.Font = new Font("Arial", 14);
            mpMessagesControl.settingsSaveClicked += settingsSaveClick;
            newMessagePage.Controls.Add(this.mpMessagesControl);
            
            Host.MainForm.FlightData.tabControlactions.TabPages.Add(newMessagePage);
            Host.MainForm.FlightData.TabListOriginal.Add(newMessagePage);

            //load settings from config.xml
            //At this point the settings table is already populated with the default values
            var settings = mpMessagesControl.settings.getSettingsTable();
            foreach (SettingsItem i in settings)
            {
                //Either get the value from the config.xml or use the default value
                i.value = Host.config.GetString(i.name, i.defaultValue);
            }
            //trigger settings processing
            settingsSaveClick(this, EventArgs.Empty);

            //Subscribe to Statustext messages. 0,0 means current primary system and component id
            //This is the primary source of messages coming from the vehicle
            var sub = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.STATUSTEXT, receivedMsgPacket, 0, 0);

            return true;     //If it is false plugin will not start (loop will not called)
        }

        private void settingsSaveClick(object sender, EventArgs e)
        {
            var settings = mpMessagesControl.settings.getSettingsTable();
            foreach (SettingsItem i in settings)
            {
                //Check if it is in the config.xml table and update if needed (ignore non persistent settings)
                if (Host.config.GetString(i.name,i.defaultValue) != i.value && i.persistent)
                {
                    // Put it in the config.xml settings table
                    Host.config[i.name] = i.value;
                }
                
                //Process settings
                switch (i.name)
                {
                    case "NMdisplayformat":
                        {
                            //Update the display format
                            //try/catch will handle the case when the value is not in the enum
                            try
                            {
                                mpMessagesControl.setDisplayFormat((DisplayFormat)Enum.Parse(typeof(DisplayFormat), i.value));
                            }
                            catch (Exception ex)
                            { }
                            break;
                        }
                    case "NMcolormode":
                        {
                            //Update the color mode
                            //try/catch will handle the case when the value is not in the enum
                            try
                            {
                                mpMessagesControl.setColorMode((ColorMode)Enum.Parse(typeof(ColorMode), i.value));
                            }
                            catch (Exception ex)
                            { }
                            break;
                        }
                    case "NMmaxlines":
                        {
                            //Check the if the value is valid
                            int value = int.TryParse(i.value, out value) ? value : 1000;
                            try
                            {
                                mpMessagesControl.setMaxMessages(value);
                            }
                            catch (Exception ex)
                            { }
                            break;
                        }
                    case "NMallowdeleteall":
                        {
                            bool value = bool.TryParse(i.value, out value) ? value : false;
                            try
                            {
                                mpMessagesControl.selAllowDeletaAll(value);
                            }
                            catch (Exception ex)
                            { }
                            break;
                        }

                    case "NMdeleteIgnoreList":
                        {
                            bool value = bool.TryParse(i.value, out value) ? value : false; 
                            if (value)
                            {
                                //Delete all messages from the ignore list
                                mpMessagesControl.clearIgnoreList();
                                i.value = "False";
                            }
                            break;
                        }
                    case "NMdetachpanel":
                        {
                            bool value = bool.TryParse(i.value, out value) ? value : false;
                            if (value)
                            {
                                //Detach the panel
                                undockDockNewMessagePage_Click(this, EventArgs.Empty);
                            }
                            else
                            {
                                //Dock the panel
                                Form p = newMessagePage.Parent.Parent as Form;
                                p?.Close();
                            }
                            break;
                        }
                    case "NMzoom":
                        {
                            int value = int.TryParse(i.value, out value) ? value : 100;
                            if (value < i.rangeMin) value = 50;
                            if (value > i.rangeMax) value = 200;
                            try
                            {
                                mpMessagesControl.setZoom(value);
                            }
                            catch (Exception ex)
                            { }
                            break;
                        }
                }
            }
        }

        private bool receivedMsgPacket(MAVLink.MAVLinkMessage mavMsg)
        {
            //mpMessagesControl.BeginInvokeIfRequired(() =>
            //{
                //just to make sure we have the right message
                if (mavMsg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.STATUSTEXT)
                {
                    var message = mavMsg.ToStructure<MAVLink.mavlink_statustext_t>();
                    //Chek if the message id is zero so we can add it to the message box
                    if (message.id == 0)
                    {
                        _updateinProgress = true;
                        GCSMessage newMsg = new GCSMessage();
                        newMsg.Text = Encoding.UTF8.GetString(message.text);
                        newMsg.Severity = message.severity;
                        newMsg.SystemId = mavMsg.sysid;
                        newMsg.LastSeen = DateTime.Now;
                        mpMessagesControl.AddNewMessage(newMsg);
                        _updateinProgress = false;
                    }
                    else
                    {
                        //We add it to the store shince it is a part of a multi message text
                        msgChunks.Add(message);
                        //check is we have the last chunk of the message
                        //Any zero characted in the text indicates the end of the message
                        if (message.text[message.text.Length - 1] == 0)
                        {
                            var chunks = msgChunks.Where(m => m.id == message.id).OrderBy(m => m.chunk_seq).ToList();



                            //concatenate the text
                            string text = "";
                            int chunkCount = 0;
                            foreach (var chunk in chunks)
                            {
                                if (chunk.chunk_seq == chunkCount)
                                {
                                    text += Encoding.UTF8.GetString(chunk.text);
                                    msgChunks.Remove(chunk);
                                    chunkCount++;
                                }
                            }
                            //delete all chunks with the given id from the list

                            GCSMessage newMsg = new GCSMessage();
                            newMsg.Text = text;
                            newMsg.Severity = message.severity;
                            newMsg.SystemId = mavMsg.sysid;
                            newMsg.LastSeen = DateTime.Now;
                            mpMessagesControl.AddNewMessage(newMsg);
                        }
                    }
                  
                }
            //});
            return true;
        }

        public override bool Loop()
		//Loop is called in regular intervalls (set by loopratehz)
        {
            // The secondary source of messages are the cs.MessageHigh, contains messages generated by the GCS
            // Some if these messages ar copied from the incoming messages, so a dupe check is done to avoid displaying the same message twice or more

            if (_updateinProgress) return true; //Do not update if the update is in progress (this is to avoid deadlocks

            //clear lastMessageHigh if the message is expired (There is a 10 second expiration time)
            if (MainV2.comPort.MAV.cs.messageHigh == null || MainV2.comPort.MAV.cs.messageHigh == "")
            {
                lastMessageHigh = "";
                _messageHighWasCleared = true;
            }

            if (lastMessageHigh != MainV2.comPort.MAV.cs.messageHigh 
                && MainV2.comPort.MAV.cs.messageHigh != null 
                && MainV2.comPort.MAV.cs.messageHigh != "")
            {
                lastMessageHigh = MainV2.comPort.MAV.cs.messageHigh;
                var lastTenMessages = mpMessagesControl.getLastNMessages(10);

                //check if lastMessageHigh is already in the last ten messages
                bool found = false;     
                foreach (var msg in lastTenMessages)
                {
                    //remove all zeros from the text
                    string trimmedMessageText = msg.Text.Replace("\0", "");
                    //Do not compare, just check if the message contains the last message high, because of multi packet messages
                    //This is just a fix for MP not handling multi packet messages correctly
                    if (trimmedMessageText.Contains(lastMessageHigh) && msg.SystemId >= 0)       
                    {
                        found = true;
                        ////Add a counter to the found message
                        //GCSMessage newMsg = new GCSMessage();
                        //newMsg.Text = msg.Text;
                        //newMsg.Severity = 0
                        //newMsg.SystemId = -1
                        //newMsg.LastSeen = DateTime.Now;
                        //mpMessagesControl.AddNewMessage(newMsg);
                        break;
                    }
                }
                if (!found)
                {
                    GCSMessage newMsg = new GCSMessage();
                    newMsg.Text = lastMessageHigh;
                    newMsg.Severity = 0;
                    newMsg.SystemId = -1;
                    newMsg.LastSeen = DateTime.Now;
                    mpMessagesControl.AddNewMessage(newMsg);
                }
            }
            return true;	//Return value is not used
        }

        public override bool Exit()
		//Exit called when plugin is terminated (usually when Mission Planner is exiting)
        {

            Host.config["MNzoom"] = mpMessagesControl.settings.getSetting("NMzoom");
            return true;	//Return value is not used
        }

        private void undockDockNewMessagePage_Click(object sender, EventArgs e)
        {
            
            //Ignore if this is already undocked
            if (isUndocked) return;


            Form dropout = new Form();
            TabControl tab = new TabControl();

            dropout.FormBorderStyle = FormBorderStyle.Sizable;
            dropout.ShowInTaskbar = false;
            dropout.Size = new Size(300, 450);
            dropout.Name = "MessagesNGform";

            tab.Appearance = TabAppearance.FlatButtons;
            tab.ItemSize = new Size(0, 0);
            tab.SizeMode = TabSizeMode.Fixed;
            tab.Size = new Size(dropout.ClientSize.Width, dropout.ClientSize.Height + 22);
            tab.Location = new Point(0, -22);

            tab.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            dropout.Text = "Messages";

            Host.MainForm.FlightData.tabControlactions.Controls.Remove(newMessagePage);

            tab.Controls.Add(newMessagePage);
            newMessagePage.BorderStyle = BorderStyle.Fixed3D;


            dropout.FormClosed += dropoutQuick_FormClosed;
            dropout.Controls.Add(tab);
            dropout.RestoreStartupLocation();
            dropout.BringToFront();
            dropout.Show();
            isUndocked = true;
            Host.MainForm.FlightData.TabListOriginal.Remove(newMessagePage);

        }

        void dropoutQuick_FormClosed(object sender, FormClosedEventArgs e)
        {
            (sender as Form).SaveStartupLocation();
            Host.MainForm.FlightData.tabControlactions.Controls.Add(newMessagePage);
            Host.MainForm.FlightData.tabControlactions.SelectedTab = newMessagePage;
            Host.MainForm.FlightData.TabListOriginal.Add(newMessagePage);
            //update the settings
            updateSetting("NMdetachpanel", "False");


            isUndocked = false;
        }

        void updateSetting(string name, string value)
        {
            mpMessagesControl.settings.setSetting(name, value);
            Host.config[name] = value;

        }




    }
}