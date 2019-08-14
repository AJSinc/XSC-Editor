namespace XSCEditor
{
    partial class MainWindow
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
            this.MainWindowMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assemblyTextEditor = new System.Windows.Forms.RichTextBox();
            this.nativesTable = new System.Windows.Forms.ListBox();
            this.stringTable = new System.Windows.Forms.ListBox();
            this.codeTableLabel = new System.Windows.Forms.Label();
            this.stringTableLabel = new System.Windows.Forms.Label();
            this.nativesTableLabel = new System.Windows.Forms.Label();
            this.XSCNameLabel = new System.Windows.Forms.Label();
            this.staticVariablesLabel = new System.Windows.Forms.Label();
            this.addVarButton = new System.Windows.Forms.Button();
            this.deleteVarButton = new System.Windows.Forms.Button();
            this.editVarButton = new System.Windows.Forms.Button();
            this.staticVariables = new System.Windows.Forms.ListBox();
            this.varCounter = new System.Windows.Forms.Label();
            this.XSCNameTextBox = new System.Windows.Forms.TextBox();
            this.parameterCountLabel = new System.Windows.Forms.Label();
            this.paramCountTextBox = new System.Windows.Forms.TextBox();
            this.MainWindowMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainWindowMenu
            // 
            this.MainWindowMenu.BackColor = System.Drawing.SystemColors.Menu;
            this.MainWindowMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.MainWindowMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.MainWindowMenu.Location = new System.Drawing.Point(0, 0);
            this.MainWindowMenu.Name = "MainWindowMenu";
            this.MainWindowMenu.Size = new System.Drawing.Size(639, 24);
            this.MainWindowMenu.TabIndex = 3;
            this.MainWindowMenu.Text = "MainWindowMenu";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.closeFileToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.saveToolStripMenuItem.Text = "Save...";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // closeFileToolStripMenuItem
            // 
            this.closeFileToolStripMenuItem.Name = "closeFileToolStripMenuItem";
            this.closeFileToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.closeFileToolStripMenuItem.Text = "Close File";
            this.closeFileToolStripMenuItem.Click += new System.EventHandler(this.closeFileToolStripMenuItem_Click);
            // 
            // assemblyTextEditor
            // 
            this.assemblyTextEditor.AcceptsTab = true;
            this.assemblyTextEditor.BackColor = System.Drawing.SystemColors.Window;
            this.assemblyTextEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.assemblyTextEditor.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.assemblyTextEditor.DetectUrls = false;
            this.assemblyTextEditor.Font = new System.Drawing.Font("Lucida Console", 10F);
            this.assemblyTextEditor.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.assemblyTextEditor.Location = new System.Drawing.Point(12, 47);
            this.assemblyTextEditor.Name = "assemblyTextEditor";
            this.assemblyTextEditor.Size = new System.Drawing.Size(348, 430);
            this.assemblyTextEditor.TabIndex = 4;
            this.assemblyTextEditor.Text = "";
            this.assemblyTextEditor.WordWrap = false;
            this.assemblyTextEditor.TextChanged += new System.EventHandler(this.assemblyTextEditor_TextChanged);
            // 
            // nativesTable
            // 
            this.nativesTable.Enabled = false;
            this.nativesTable.FormattingEnabled = true;
            this.nativesTable.HorizontalScrollbar = true;
            this.nativesTable.ItemHeight = 14;
            this.nativesTable.Location = new System.Drawing.Point(366, 197);
            this.nativesTable.Name = "nativesTable";
            this.nativesTable.Size = new System.Drawing.Size(260, 130);
            this.nativesTable.TabIndex = 6;
            this.nativesTable.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // stringTable
            // 
            this.stringTable.Enabled = false;
            this.stringTable.FormattingEnabled = true;
            this.stringTable.HorizontalScrollbar = true;
            this.stringTable.ItemHeight = 14;
            this.stringTable.Location = new System.Drawing.Point(366, 47);
            this.stringTable.Name = "stringTable";
            this.stringTable.Size = new System.Drawing.Size(260, 130);
            this.stringTable.TabIndex = 10;
            // 
            // codeTableLabel
            // 
            this.codeTableLabel.AutoSize = true;
            this.codeTableLabel.Location = new System.Drawing.Point(9, 24);
            this.codeTableLabel.Name = "codeTableLabel";
            this.codeTableLabel.Size = new System.Drawing.Size(70, 15);
            this.codeTableLabel.TabIndex = 11;
            this.codeTableLabel.Tag = "";
            this.codeTableLabel.Text = "Code Table";
            // 
            // stringTableLabel
            // 
            this.stringTableLabel.AutoSize = true;
            this.stringTableLabel.Location = new System.Drawing.Point(363, 24);
            this.stringTableLabel.Name = "stringTableLabel";
            this.stringTableLabel.Size = new System.Drawing.Size(72, 15);
            this.stringTableLabel.TabIndex = 12;
            this.stringTableLabel.Tag = "";
            this.stringTableLabel.Text = "String Table";
            // 
            // nativesTableLabel
            // 
            this.nativesTableLabel.AutoSize = true;
            this.nativesTableLabel.Location = new System.Drawing.Point(366, 179);
            this.nativesTableLabel.Name = "nativesTableLabel";
            this.nativesTableLabel.Size = new System.Drawing.Size(81, 15);
            this.nativesTableLabel.TabIndex = 13;
            this.nativesTableLabel.Tag = "";
            this.nativesTableLabel.Text = "Natives Table";
            // 
            // XSCNameLabel
            // 
            this.XSCNameLabel.AutoSize = true;
            this.XSCNameLabel.Location = new System.Drawing.Point(94, 24);
            this.XSCNameLabel.Name = "XSCNameLabel";
            this.XSCNameLabel.Size = new System.Drawing.Size(41, 15);
            this.XSCNameLabel.TabIndex = 14;
            this.XSCNameLabel.Text = "Name";
            // 
            // staticVariablesLabel
            // 
            this.staticVariablesLabel.AutoSize = true;
            this.staticVariablesLabel.Location = new System.Drawing.Point(366, 329);
            this.staticVariablesLabel.Name = "staticVariablesLabel";
            this.staticVariablesLabel.Size = new System.Drawing.Size(91, 15);
            this.staticVariablesLabel.TabIndex = 17;
            this.staticVariablesLabel.Text = "Static Variables";
            // 
            // addVarButton
            // 
            this.addVarButton.Location = new System.Drawing.Point(582, 347);
            this.addVarButton.Name = "addVarButton";
            this.addVarButton.Size = new System.Drawing.Size(37, 37);
            this.addVarButton.TabIndex = 18;
            this.addVarButton.Text = "+";
            this.addVarButton.UseVisualStyleBackColor = true;
            this.addVarButton.Click += new System.EventHandler(this.addVarButton_Click);
            // 
            // deleteVarButton
            // 
            this.deleteVarButton.Location = new System.Drawing.Point(582, 393);
            this.deleteVarButton.Name = "deleteVarButton";
            this.deleteVarButton.Size = new System.Drawing.Size(37, 37);
            this.deleteVarButton.TabIndex = 19;
            this.deleteVarButton.Text = "-";
            this.deleteVarButton.UseVisualStyleBackColor = true;
            this.deleteVarButton.Click += new System.EventHandler(this.deleteVarButton_Click);
            // 
            // editVarButton
            // 
            this.editVarButton.Location = new System.Drawing.Point(582, 440);
            this.editVarButton.Name = "editVarButton";
            this.editVarButton.Size = new System.Drawing.Size(37, 37);
            this.editVarButton.TabIndex = 20;
            this.editVarButton.Text = "Edit";
            this.editVarButton.UseVisualStyleBackColor = true;
            this.editVarButton.Click += new System.EventHandler(this.editVarButton_Click);
            // 
            // staticVariables
            // 
            this.staticVariables.FormattingEnabled = true;
            this.staticVariables.ItemHeight = 14;
            this.staticVariables.Location = new System.Drawing.Point(366, 347);
            this.staticVariables.Name = "staticVariables";
            this.staticVariables.Size = new System.Drawing.Size(210, 130);
            this.staticVariables.TabIndex = 21;
            // 
            // varCounter
            // 
            this.varCounter.AutoSize = true;
            this.varCounter.Location = new System.Drawing.Point(463, 330);
            this.varCounter.Name = "varCounter";
            this.varCounter.Size = new System.Drawing.Size(22, 15);
            this.varCounter.TabIndex = 22;
            this.varCounter.Text = "(0)";
            // 
            // XSCNameTextBox
            // 
            this.XSCNameTextBox.Location = new System.Drawing.Point(141, 21);
            this.XSCNameTextBox.MaxLength = 64;
            this.XSCNameTextBox.Name = "XSCNameTextBox";
            this.XSCNameTextBox.Size = new System.Drawing.Size(91, 21);
            this.XSCNameTextBox.TabIndex = 23;
            // 
            // parameterCountLabel
            // 
            this.parameterCountLabel.AutoSize = true;
            this.parameterCountLabel.Location = new System.Drawing.Point(238, 24);
            this.parameterCountLabel.Name = "parameterCountLabel";
            this.parameterCountLabel.Size = new System.Drawing.Size(80, 15);
            this.parameterCountLabel.TabIndex = 24;
            this.parameterCountLabel.Text = "Param Count";
            // 
            // paramCountTextBox
            // 
            this.paramCountTextBox.Location = new System.Drawing.Point(324, 21);
            this.paramCountTextBox.MaxLength = 9;
            this.paramCountTextBox.Name = "paramCountTextBox";
            this.paramCountTextBox.Size = new System.Drawing.Size(36, 21);
            this.paramCountTextBox.TabIndex = 25;
            this.paramCountTextBox.Text = "0";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackColor = System.Drawing.SystemColors.MenuBar;
            this.ClientSize = new System.Drawing.Size(639, 487);
            this.Controls.Add(this.paramCountTextBox);
            this.Controls.Add(this.parameterCountLabel);
            this.Controls.Add(this.XSCNameTextBox);
            this.Controls.Add(this.varCounter);
            this.Controls.Add(this.staticVariables);
            this.Controls.Add(this.editVarButton);
            this.Controls.Add(this.deleteVarButton);
            this.Controls.Add(this.addVarButton);
            this.Controls.Add(this.staticVariablesLabel);
            this.Controls.Add(this.XSCNameLabel);
            this.Controls.Add(this.nativesTableLabel);
            this.Controls.Add(this.stringTableLabel);
            this.Controls.Add(this.codeTableLabel);
            this.Controls.Add(this.stringTable);
            this.Controls.Add(this.nativesTable);
            this.Controls.Add(this.assemblyTextEditor);
            this.Controls.Add(this.MainWindowMenu);
            this.Font = new System.Drawing.Font("Arial", 8.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.MainWindowMenu;
            this.Name = "MainWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XSC Editor";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.MainWindowMenu.ResumeLayout(false);
            this.MainWindowMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MainWindowMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeFileToolStripMenuItem;
        private System.Windows.Forms.RichTextBox assemblyTextEditor;
        private System.Windows.Forms.ListBox nativesTable;
        private System.Windows.Forms.ListBox stringTable;
        private System.Windows.Forms.Label codeTableLabel;
        private System.Windows.Forms.Label stringTableLabel;
        private System.Windows.Forms.Label nativesTableLabel;
        private System.Windows.Forms.Label XSCNameLabel;
        private System.Windows.Forms.Label staticVariablesLabel;
        private System.Windows.Forms.Button addVarButton;
        private System.Windows.Forms.Button deleteVarButton;
        private System.Windows.Forms.Button editVarButton;
        private System.Windows.Forms.ListBox staticVariables;
        private System.Windows.Forms.Label varCounter;
        private System.Windows.Forms.TextBox XSCNameTextBox;
        private System.Windows.Forms.Label parameterCountLabel;
        private System.Windows.Forms.TextBox paramCountTextBox;
        
    }
}

