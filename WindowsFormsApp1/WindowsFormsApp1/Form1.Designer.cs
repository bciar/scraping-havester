namespace OreillyTriviaHarvester
{
    partial class OreillyTriviaHarvester
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
            this.WebBrowser = new System.Windows.Forms.WebBrowser();
            this.BtnStart = new System.Windows.Forms.Button();
            this.PageNumPanel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.BeginPage = new System.Windows.Forms.NumericUpDown();
            this.FolderBrowserDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.BtnSaveFolder = new System.Windows.Forms.Button();
            this.SavePath = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.BeginPage)).BeginInit();
            this.SuspendLayout();
            // 
            // WebBrowser
            // 
            this.WebBrowser.Location = new System.Drawing.Point(53, 319);
            this.WebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowser.Name = "WebBrowser";
            this.WebBrowser.Size = new System.Drawing.Size(332, 264);
            this.WebBrowser.TabIndex = 0;
            this.WebBrowser.Visible = false;
            this.WebBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            // 
            // BtnStart
            // 
            this.BtnStart.Location = new System.Drawing.Point(53, 156);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(320, 44);
            this.BtnStart.TabIndex = 1;
            this.BtnStart.Text = "Start";
            this.BtnStart.UseVisualStyleBackColor = true;
            this.BtnStart.Click += new System.EventHandler(this.OnStart);
            // 
            // PageNumPanel
            // 
            this.PageNumPanel.AutoSize = true;
            this.PageNumPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 33F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PageNumPanel.Location = new System.Drawing.Point(123, 225);
            this.PageNumPanel.Name = "PageNumPanel";
            this.PageNumPanel.Size = new System.Drawing.Size(185, 52);
            this.PageNumPanel.TabIndex = 2;
            this.PageNumPanel.Text = "Ready...";
            this.PageNumPanel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 31);
            this.label1.TabIndex = 4;
            this.label1.Text = "From  Page";
            // 
            // BeginPage
            // 
            this.BeginPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BeginPage.Location = new System.Drawing.Point(222, 104);
            this.BeginPage.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.BeginPage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.BeginPage.Name = "BeginPage";
            this.BeginPage.Size = new System.Drawing.Size(100, 38);
            this.BeginPage.TabIndex = 5;
            this.BeginPage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // BtnSaveFolder
            // 
            this.BtnSaveFolder.Location = new System.Drawing.Point(276, 51);
            this.BtnSaveFolder.Name = "BtnSaveFolder";
            this.BtnSaveFolder.Size = new System.Drawing.Size(97, 23);
            this.BtnSaveFolder.TabIndex = 6;
            this.BtnSaveFolder.Text = "Where To Save?";
            this.BtnSaveFolder.UseVisualStyleBackColor = true;
            this.BtnSaveFolder.Click += new System.EventHandler(this.OnChooseSaveFolder);
            // 
            // SavePath
            // 
            this.SavePath.Location = new System.Drawing.Point(53, 52);
            this.SavePath.Name = "SavePath";
            this.SavePath.Size = new System.Drawing.Size(217, 20);
            this.SavePath.TabIndex = 7;
            // 
            // OreillyTriviaHarvester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 327);
            this.Controls.Add(this.SavePath);
            this.Controls.Add(this.BtnSaveFolder);
            this.Controls.Add(this.BeginPage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PageNumPanel);
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.WebBrowser);
            this.Name = "OreillyTriviaHarvester";
            this.Text = "Oreilly Trivia Harvester";
            ((System.ComponentModel.ISupportInitialize)(this.BeginPage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser WebBrowser;
        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Label PageNumPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown BeginPage;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDlg;
        private System.Windows.Forms.Button BtnSaveFolder;
        private System.Windows.Forms.TextBox SavePath;
    }
}

