namespace MpMessagesControl
{
    partial class NewMessageControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewMessageControl));
            this.btnAll = new System.Windows.Forms.Button();
            this.btnWarn = new System.Windows.Forms.Button();
            this.btnError = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.btnEnW = new System.Windows.Forms.Button();
            this.mpMessageControlBox1 = new MPMessageControlBoxNS.MpMessageControlBox();
            ((System.ComponentModel.ISupportInitialize)(this.mpMessageControlBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAll
            // 
            this.btnAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAll.BackColor = System.Drawing.Color.Red;
            this.btnAll.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnAll.BackgroundImage")));
            this.btnAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAll.FlatAppearance.BorderColor = System.Drawing.Color.Green;
            this.btnAll.FlatAppearance.BorderSize = 3;
            this.btnAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAll.Location = new System.Drawing.Point(3, 363);
            this.btnAll.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnAll.Name = "btnAll";
            this.btnAll.Padding = new System.Windows.Forms.Padding(2);
            this.btnAll.Size = new System.Drawing.Size(54, 53);
            this.btnAll.TabIndex = 1;
            this.btnAll.Text = "All\r\n999";
            this.btnAll.UseVisualStyleBackColor = false;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnWarn
            // 
            this.btnWarn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnWarn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnWarn.BackgroundImage")));
            this.btnWarn.FlatAppearance.BorderSize = 3;
            this.btnWarn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWarn.Location = new System.Drawing.Point(165, 363);
            this.btnWarn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnWarn.Name = "btnWarn";
            this.btnWarn.Size = new System.Drawing.Size(54, 53);
            this.btnWarn.TabIndex = 2;
            this.btnWarn.Text = "Wrn\r\n999\r\n";
            this.btnWarn.UseVisualStyleBackColor = true;
            this.btnWarn.Click += new System.EventHandler(this.btnWarn_Click);
            // 
            // btnError
            // 
            this.btnError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnError.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnError.BackgroundImage")));
            this.btnError.FlatAppearance.BorderSize = 3;
            this.btnError.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnError.Location = new System.Drawing.Point(57, 363);
            this.btnError.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnError.Name = "btnError";
            this.btnError.Size = new System.Drawing.Size(54, 53);
            this.btnError.TabIndex = 3;
            this.btnError.Text = "Err\r\n999\r\n";
            this.btnError.UseVisualStyleBackColor = true;
            this.btnError.Click += new System.EventHandler(this.btnError_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInfo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnInfo.BackgroundImage")));
            this.btnInfo.FlatAppearance.BorderSize = 3;
            this.btnInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInfo.Location = new System.Drawing.Point(219, 363);
            this.btnInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(54, 53);
            this.btnInfo.TabIndex = 4;
            this.btnInfo.Text = "Inf\r\n999\r\n";
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSettings.BackgroundImage")));
            this.btnSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSettings.Location = new System.Drawing.Point(421, 363);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(53, 53);
            this.btnSettings.TabIndex = 5;
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteAll.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDeleteAll.BackgroundImage")));
            this.btnDeleteAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDeleteAll.Location = new System.Drawing.Point(363, 363);
            this.btnDeleteAll.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(53, 53);
            this.btnDeleteAll.TabIndex = 6;
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // btnEnW
            // 
            this.btnEnW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEnW.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnEnW.BackgroundImage")));
            this.btnEnW.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnEnW.FlatAppearance.BorderSize = 3;
            this.btnEnW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnW.Location = new System.Drawing.Point(111, 363);
            this.btnEnW.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnEnW.Name = "btnEnW";
            this.btnEnW.Size = new System.Drawing.Size(54, 53);
            this.btnEnW.TabIndex = 7;
            this.btnEnW.Text = "E/W\r\n999";
            this.btnEnW.UseVisualStyleBackColor = true;
            this.btnEnW.Click += new System.EventHandler(this.btnEnW_Click);
            // 
            // mpMessageControlBox1
            // 
            this.mpMessageControlBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mpMessageControlBox1.AutoScroll = true;
            this.mpMessageControlBox1.AutoScrollMinSize = new System.Drawing.Size(2, 0);
            this.mpMessageControlBox1.BackColor = System.Drawing.Color.Black;
            this.mpMessageControlBox1.CharHeight = 18;
            this.mpMessageControlBox1.CharWidth = 10;
            this.mpMessageControlBox1.ColorMode = MPMessageControlBoxNS.ColorMode.Dark;
            this.mpMessageControlBox1.DisplayFormat = MPMessageControlBoxNS.DisplayFormat.Folded;
            this.mpMessageControlBox1.Font = new System.Drawing.Font("Courier New", 9.75F);
            this.mpMessageControlBox1.Location = new System.Drawing.Point(0, 0);
            this.mpMessageControlBox1.Margin = new System.Windows.Forms.Padding(0);
            this.mpMessageControlBox1.MaxMessages = 1000;
            this.mpMessageControlBox1.Name = "mpMessageControlBox1";
            this.mpMessageControlBox1.PaddingBackColor = System.Drawing.Color.Empty;
            this.mpMessageControlBox1.Paddings = new System.Windows.Forms.Padding(0);
            this.mpMessageControlBox1.ShowSeverity = ((MpMessageControlStoreNS.SeverityLevels)(((MpMessageControlStoreNS.SeverityLevels.Error | MpMessageControlStoreNS.SeverityLevels.Warning) 
            | MpMessageControlStoreNS.SeverityLevels.Info)));
            this.mpMessageControlBox1.Size = new System.Drawing.Size(477, 360);
            this.mpMessageControlBox1.TabIndex = 0;
            this.mpMessageControlBox1.TextHeight = 0;
            this.mpMessageControlBox1.Zoom = 100;
            // 
            // NewMessageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.btnEnW);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnInfo);
            this.Controls.Add(this.btnError);
            this.Controls.Add(this.btnWarn);
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.mpMessageControlBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "NewMessageControl";
            this.Size = new System.Drawing.Size(477, 419);
            ((System.ComponentModel.ISupportInitialize)(this.mpMessageControlBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MPMessageControlBoxNS.MpMessageControlBox mpMessageControlBox1;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Button btnWarn;
        private System.Windows.Forms.Button btnError;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.Button btnEnW;
    }
}
