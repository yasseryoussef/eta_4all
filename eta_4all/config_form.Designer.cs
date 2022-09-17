namespace eta_4all
{
    partial class config_form
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_client_id = new System.Windows.Forms.TextBox();
            this.txt_client_secrit = new System.Windows.Forms.TextBox();
            this.txt_token_pin = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.cmb_tokin_type = new System.Windows.Forms.ComboBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.lb_folder = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "client id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "client secrit";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "token type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(60, 174);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "token pin";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // txt_client_id
            // 
            this.txt_client_id.Location = new System.Drawing.Point(158, 90);
            this.txt_client_id.Name = "txt_client_id";
            this.txt_client_id.Size = new System.Drawing.Size(322, 20);
            this.txt_client_id.TabIndex = 4;
            // 
            // txt_client_secrit
            // 
            this.txt_client_secrit.Location = new System.Drawing.Point(158, 110);
            this.txt_client_secrit.Name = "txt_client_secrit";
            this.txt_client_secrit.Size = new System.Drawing.Size(322, 20);
            this.txt_client_secrit.TabIndex = 5;
            // 
            // txt_token_pin
            // 
            this.txt_token_pin.Location = new System.Drawing.Point(158, 164);
            this.txt_token_pin.Name = "txt_token_pin";
            this.txt_token_pin.Size = new System.Drawing.Size(322, 20);
            this.txt_token_pin.TabIndex = 7;
            this.txt_token_pin.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(158, 242);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmb_tokin_type
            // 
            this.cmb_tokin_type.FormattingEnabled = true;
            this.cmb_tokin_type.Items.AddRange(new object[] {
            "Egypt Trust Sealing CA",
            "Misr for central clearing,depository and registry"});
            this.cmb_tokin_type.Location = new System.Drawing.Point(158, 137);
            this.cmb_tokin_type.Name = "cmb_tokin_type";
            this.cmb_tokin_type.Size = new System.Drawing.Size(322, 21);
            this.cmb_tokin_type.TabIndex = 9;
            // 
            // lb_folder
            // 
            this.lb_folder.AutoSize = true;
            this.lb_folder.Location = new System.Drawing.Point(217, 199);
            this.lb_folder.Name = "lb_folder";
            this.lb_folder.Size = new System.Drawing.Size(35, 13);
            this.lb_folder.TabIndex = 10;
            this.lb_folder.Text = "label5";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(158, 194);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(48, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "select folder";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // config_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lb_folder);
            this.Controls.Add(this.cmb_tokin_type);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txt_token_pin);
            this.Controls.Add(this.txt_client_secrit);
            this.Controls.Add(this.txt_client_id);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "config_form";
            this.Text = "config_form";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_client_id;
        private System.Windows.Forms.TextBox txt_client_secrit;
        private System.Windows.Forms.TextBox txt_token_pin;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cmb_tokin_type;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label lb_folder;
        private System.Windows.Forms.Button button2;
    }
}