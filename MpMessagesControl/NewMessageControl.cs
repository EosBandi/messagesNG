using MPMessageControlBoxNS;
using MpMessageControlStoreNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MpMessagesControl
{
    public partial class NewMessageControl : UserControl
    {

        public event EventHandler settingsSaveClicked;

        public PluginSettings settings = new PluginSettings(@"plugins\messageBoxSettings.json");
        
        private bool _allowDeleteAll = false;

        Form settingsForm;

        public NewMessageControl()
        {
            InitializeComponent();
            mpMessageControlBox1.linesRefreshed += MpMessageControlBox1_linesRefreshed;
        }


        public void MpMessageControlBox1_linesRefreshed(object sender, EventArgs e)
        {
            btnAll.BeginInvoke(new Action(() =>
            {
                btnAll.Text = "All\n" + mpMessageControlBox1.msgCountBySeverity[SeverityLevels.All].ToString();
                btnAll.Invalidate();
            }));

            btnInfo.BeginInvoke(new Action(() =>
            {
                btnInfo.Text = "Inf\n" + mpMessageControlBox1.msgCountBySeverity[SeverityLevels.Info].ToString();
                btnInfo.Invalidate();
            }));

            btnWarn.BeginInvoke(new Action(() =>
            {
                btnWarn.Text = "Wrn\n" + mpMessageControlBox1.msgCountBySeverity[SeverityLevels.Warning].ToString();
                btnWarn.Invalidate();
            }));

            btnError.BeginInvoke(new Action(() =>
            {
                btnError.Text = "Err\n" + mpMessageControlBox1.msgCountBySeverity[SeverityLevels.Error].ToString();
                btnError.Invalidate();
            }));

            btnEnW.BeginInvoke(new Action(() =>
            {
                btnEnW.Text = "E/W\n" + mpMessageControlBox1.msgCountBySeverity[SeverityLevels.ErrorAndWarning].ToString();
                btnEnW.Invalidate();
            }));

        }

        public int MaxMessages
        {
            get { return mpMessageControlBox1.MaxMessages; }
            set { mpMessageControlBox1.MaxMessages = value; }
        }

        public List<GCSMessage> getLastNMessages(int num)
        {
            return mpMessageControlBox1.getLastNMessages(num);
        }

        public void AddNewMessage(GCSMessage msg)
        {
            mpMessageControlBox1.addMessage(msg);
        }
        public void clearIgnoreList()
        {
            mpMessageControlBox1.clearIgnoreList();
        }

        public void selAllowDeletaAll(bool b)
        {
            _allowDeleteAll = b;
        }
        public void setDisplayFormat(DisplayFormat f)
        {
            mpMessageControlBox1.DisplayFormat = f;
        }
        public void setColorMode(ColorMode m)
        {
            mpMessageControlBox1.ColorMode = m;
        }
        public void setMaxMessages(int l)
        {
            mpMessageControlBox1.MaxMessages = l;
        }   

        public int getZoom()
        {
            return mpMessageControlBox1.Zoom;
        }
        public void setZoom(int z)
        {
            mpMessageControlBox1.Zoom = z;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            mpMessageControlBox1.ShowSeverity = SeverityLevels.All;
            btnAll.FlatAppearance.BorderColor = Color.Green;
            btnInfo.FlatAppearance.BorderColor = Color.Black;
            btnWarn.FlatAppearance.BorderColor = Color.Black;
            btnError.FlatAppearance.BorderColor = Color.Black;
            btnEnW.FlatAppearance.BorderColor = Color.Black;
        }
        private void btnEnW_Click(object sender, EventArgs e)
        {
            mpMessageControlBox1.ShowSeverity = SeverityLevels.ErrorAndWarning;
            btnAll.FlatAppearance.BorderColor = Color.Black;
            btnInfo.FlatAppearance.BorderColor = Color.Black;
            btnWarn.FlatAppearance.BorderColor = Color.Black;
            btnError.FlatAppearance.BorderColor = Color.Black;
            btnEnW.FlatAppearance.BorderColor = Color.Green;

        }
        private void btnError_Click(object sender, EventArgs e)
        {
            mpMessageControlBox1.ShowSeverity = SeverityLevels.Error;
            btnAll.FlatAppearance.BorderColor = Color.Black;
            btnInfo.FlatAppearance.BorderColor = Color.Black;
            btnWarn.FlatAppearance.BorderColor = Color.Black;
            btnError.FlatAppearance.BorderColor = Color.Green;
            btnEnW.FlatAppearance.BorderColor = Color.Black;

        }

        private void btnWarn_Click(object sender, EventArgs e)
        {
            mpMessageControlBox1.ShowSeverity = SeverityLevels.Warning;
            btnAll.FlatAppearance.BorderColor = Color.Black;
            btnInfo.FlatAppearance.BorderColor = Color.Black;
            btnWarn.FlatAppearance.BorderColor = Color.Green;
            btnError.FlatAppearance.BorderColor = Color.Black;
            btnEnW.FlatAppearance.BorderColor = Color.Black;

        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            mpMessageControlBox1.ShowSeverity = SeverityLevels.Info;
            btnAll.FlatAppearance.BorderColor = Color.Black;
            btnInfo.FlatAppearance.BorderColor = Color.Green;
            btnWarn.FlatAppearance.BorderColor = Color.Black;
            btnError.FlatAppearance.BorderColor = Color.Black;
            btnEnW.FlatAppearance.BorderColor = Color.Black;

        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            showSettings();
        }

        //TODO: move it up to the plugin level, instead of the custom control (not sure)
        private void showSettings()
        {

            //This is a hack - setting zoom value from the control box 
            settings.setSetting("NMzoom", mpMessageControlBox1.Zoom.ToString());    

            settingsForm = new Form();
            settingsForm.Text = "Settings";
            settingsForm.StartPosition = FormStartPosition.CenterParent;

            //Add a tooltip control
            ToolTip toolTip = new ToolTip();

            float maxlength = 0;
            SizeF size = new SizeF();

            foreach (SettingsItem i in settings.SettingsTable)
            {
                size = TextRenderer.MeasureText(i.displayName, settingsForm.Font);

                if (size.Width > maxlength)
                    maxlength = size.Width;
            }

            int valuePos = (int)maxlength + 30;

            int y = 0;
            //Add all settings items to the form in a table
            for (int x = 0; x < settings.SettingsTable.Count; x++)
            {
                y = settingsForm.Controls.Count;

                //Add a label
                Label l = new Label();
                l.Text = settings.SettingsTable[x].displayName;
                l.Location = new Point(10, 13 + 15 * y);
                l.Size = new Size((int)maxlength + 20 , 20);
                toolTip.SetToolTip(l, settings.SettingsTable[x].description);
                settingsForm.Controls.Add(l);

                switch (settings.SettingsTable[x].fieldType)
                {
                    case FieldType.Text:
                        {
                            //Add a textbox
                            TextBox t = new TextBox();
                            t.Text = settings.SettingsTable[x].value;
                            t.Location = new Point(valuePos, 10 + 15 * y);
                            settingsForm.Controls.Add(t);
                            settings.SettingsTable[x].control = t;
                            break;
                        }
                    case FieldType.Boolean:
                        {
                            //Add a checkbox
                            CheckBox c = new CheckBox();
                            c.Checked = Convert.ToBoolean(settings.SettingsTable[x].value);
                            c.Location = new Point(valuePos, 10 + 15 * y);
                            settingsForm.Controls.Add(c);
                            settings.SettingsTable[x].control = c;
                            break;
                        }
                    case FieldType.Number:
                        {
                            //Add a textbox
                            TextBox t = new TextBox();
                            t.Text = settings.SettingsTable[x].value;
                            t.Location = new Point(valuePos, 10 + 15 * y);
                            settingsForm.Controls.Add(t);
                            settings.SettingsTable[x].control = t;
                            break;
                        }
                    case FieldType.List:
                        {
                            //Add a combobox
                            ComboBox c = new ComboBox();
                            //Disable text enter into the combobox
                            c.DropDownStyle = ComboBoxStyle.DropDownList;
                            c.Items.AddRange(settings.SettingsTable[x].fixValues);
                            c.SelectedIndex = c.Items.IndexOf(settings.SettingsTable[x].value);
                            c.Location = new Point(valuePos, 10 + 15 * y);
                            settingsForm.Controls.Add(c);
                            settings.SettingsTable[x].control = c;
                            break;
                        }
                }
            }
            y = y + 2;

            Button btnSave = new Button();
            btnSave.Text = "Save";
            btnSave.Location = new Point(10, 20 + 15 * y);
            btnSave.Click += BtnSave_Click;
            settingsForm.Controls.Add(btnSave);

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Location = new Point(90, 20 + 15 * y);
            btnCancel.Click += BtnCancel_Click;
            settingsForm.Controls.Add(btnCancel);

            var maxFormWidth = settingsForm.Controls.Cast<Control>().Max(c => c.Right);
            var maxFormHeight = settingsForm.Controls.Cast<Control>().Max(c => c.Bottom);
            settingsForm.ClientSize = new Size(maxFormWidth + 10, maxFormHeight + 10);

            settingsForm.ShowDialog();

        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            foreach (SettingsItem i in settings.SettingsTable)
            {
                switch (i.fieldType)
                {
                    case FieldType.Text:
                        {
                            i.value = ((TextBox)i.control).Text;
                            break;
                        }
                    case FieldType.Boolean:
                        {
                            i.value = ((CheckBox)i.control).Checked.ToString();
                            break;
                        }
                    case FieldType.Number:
                        {
                            //TODO: Check if value is in range
                            i.value = ((TextBox)i.control).Text;
                            break;
                        }
                    case FieldType.List:
                        {
                            i.value = ((ComboBox)i.control).SelectedItem.ToString();
                            break;
                        }
                }
            }
            this.OnSettingsSaveClicked(EventArgs.Empty);
            settingsForm.Close();
        }

        protected virtual void OnSettingsSaveClicked(EventArgs e)
        {
            EventHandler handler = this.settingsSaveClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            //Close the form
            settingsForm.Close();

        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            SeverityLevels displaySeverity = mpMessageControlBox1.ShowSeverity;
            //Do not delete all id all messages are selected TODO: make this a selectable setting
            if (displaySeverity == SeverityLevels.All && !_allowDeleteAll) return;
            mpMessageControlBox1.deleteAllMessages(displaySeverity);
        }


    }
}
