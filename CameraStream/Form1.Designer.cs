namespace CameraStream
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cb_Devices = new System.Windows.Forms.ComboBox();
            this.btn_Start = new System.Windows.Forms.Button();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.pb_Main = new System.Windows.Forms.PictureBox();
            this.cb_Profile = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Main)).BeginInit();
            this.SuspendLayout();
            // 
            // cb_Devices
            // 
            this.cb_Devices.FormattingEnabled = true;
            this.cb_Devices.Location = new System.Drawing.Point(12, 390);
            this.cb_Devices.Name = "cb_Devices";
            this.cb_Devices.Size = new System.Drawing.Size(227, 24);
            this.cb_Devices.TabIndex = 0;
            this.cb_Devices.SelectedIndexChanged += new System.EventHandler(this.cb_Devices_SelectedIndexChanged);
            this.cb_Devices.Click += new System.EventHandler(this.cb_Devices_Click);
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(484, 389);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(75, 23);
            this.btn_Start.TabIndex = 1;
            this.btn_Start.Text = "Start";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // btn_Stop
            // 
            this.btn_Stop.Location = new System.Drawing.Point(565, 389);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(75, 23);
            this.btn_Stop.TabIndex = 2;
            this.btn_Stop.Text = "Stop";
            this.btn_Stop.UseVisualStyleBackColor = true;
            // 
            // pb_Main
            // 
            this.pb_Main.Location = new System.Drawing.Point(12, 12);
            this.pb_Main.Name = "pb_Main";
            this.pb_Main.Size = new System.Drawing.Size(776, 372);
            this.pb_Main.TabIndex = 3;
            this.pb_Main.TabStop = false;
            // 
            // cb_Profile
            // 
            this.cb_Profile.FormattingEnabled = true;
            this.cb_Profile.Location = new System.Drawing.Point(245, 389);
            this.cb_Profile.Name = "cb_Profile";
            this.cb_Profile.Size = new System.Drawing.Size(227, 24);
            this.cb_Profile.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cb_Profile);
            this.Controls.Add(this.pb_Main);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.btn_Start);
            this.Controls.Add(this.cb_Devices);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pb_Main)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox cb_Devices;
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.PictureBox pb_Main;
        private System.Windows.Forms.ComboBox cb_Profile;
    }
}

