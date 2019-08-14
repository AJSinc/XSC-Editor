namespace XSCEditor
{
    partial class StaticVariableBox
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
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.enterButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // valueTextBox
            // 
            this.valueTextBox.Location = new System.Drawing.Point(12, 13);
            this.valueTextBox.MaxLength = 10;
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.ShortcutsEnabled = false;
            this.valueTextBox.Size = new System.Drawing.Size(114, 21);
            this.valueTextBox.TabIndex = 0;
            this.valueTextBox.TextChanged += new System.EventHandler(this.valueTextBox_TextChanged);
            this.valueTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.valueTextBox_KeyDown);
            this.valueTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.valueTextBox_KeyPress);
            // 
            // enterButton
            // 
            this.enterButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.enterButton.Location = new System.Drawing.Point(139, 11);
            this.enterButton.Name = "enterButton";
            this.enterButton.Size = new System.Drawing.Size(48, 23);
            this.enterButton.TabIndex = 1;
            this.enterButton.Text = "Enter";
            this.enterButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.enterButton.UseVisualStyleBackColor = true;
            this.enterButton.Click += new System.EventHandler(this.enterButton_Click);
            // 
            // StaticVariableBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(199, 46);
            this.Controls.Add(this.enterButton);
            this.Controls.Add(this.valueTextBox);
            this.Font = new System.Drawing.Font("Arial", 8.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "StaticVariableBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Static Variable Value Box";
            this.Load += new System.EventHandler(this.StaticVariableBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox valueTextBox;
        private System.Windows.Forms.Button enterButton;
    }
}