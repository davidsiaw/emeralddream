using EmeraldLibrary;
namespace MapMaker
{
    partial class Editor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mapListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapListToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.status_Coords = new System.Windows.Forms.ToolStripStatusLabel();
            this.tileEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.floorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapListToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1140, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mapListToolStripMenuItem
            // 
            this.mapListToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapListToolStripMenuItem1,
            this.tileEditorToolStripMenuItem,
            this.floorsToolStripMenuItem});
            this.mapListToolStripMenuItem.Name = "mapListToolStripMenuItem";
            this.mapListToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.mapListToolStripMenuItem.Text = "Windows";
            // 
            // mapListToolStripMenuItem1
            // 
            this.mapListToolStripMenuItem1.Name = "mapListToolStripMenuItem1";
            this.mapListToolStripMenuItem1.Size = new System.Drawing.Size(113, 22);
            this.mapListToolStripMenuItem1.Text = "Map List";
            this.mapListToolStripMenuItem1.Click += new System.EventHandler(this.mapListToolStripMenuItem1_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1140, 510);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1140, 557);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status_Coords});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1140, 22);
            this.statusStrip1.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(109, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // status_Coords
            // 
            this.status_Coords.Name = "status_Coords";
            this.status_Coords.Size = new System.Drawing.Size(0, 17);
            // 
            // tileEditorToolStripMenuItem
            // 
            this.tileEditorToolStripMenuItem.Name = "tileEditorToolStripMenuItem";
            this.tileEditorToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.tileEditorToolStripMenuItem.Text = "Tile Editor";
            // 
            // floorsToolStripMenuItem
            // 
            this.floorsToolStripMenuItem.Name = "floorsToolStripMenuItem";
            this.floorsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.floorsToolStripMenuItem.Text = "Floors";
            this.floorsToolStripMenuItem.Click += new System.EventHandler(this.floorsToolStripMenuItem_Click);
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 581);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Editor";
            this.Text = "Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripMenuItem mapListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapListToolStripMenuItem1;
        private System.Windows.Forms.ToolStripStatusLabel status_Coords;
        private System.Windows.Forms.ToolStripMenuItem tileEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem floorsToolStripMenuItem;

    }
}