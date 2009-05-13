namespace MapMaker
{
    partial class TileSelection
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
            this.btn_savemap = new System.Windows.Forms.Button();
            this.btn_loadmap = new System.Windows.Forms.Button();
            this.flow_tiles = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_newmap = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_savemap
            // 
            this.btn_savemap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_savemap.Location = new System.Drawing.Point(12, 12);
            this.btn_savemap.Name = "btn_savemap";
            this.btn_savemap.Size = new System.Drawing.Size(315, 23);
            this.btn_savemap.TabIndex = 1;
            this.btn_savemap.Text = "Save Map";
            this.btn_savemap.UseVisualStyleBackColor = true;
            this.btn_savemap.Click += new System.EventHandler(this.btn_savemap_Click);
            // 
            // btn_loadmap
            // 
            this.btn_loadmap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_loadmap.Location = new System.Drawing.Point(12, 70);
            this.btn_loadmap.Name = "btn_loadmap";
            this.btn_loadmap.Size = new System.Drawing.Size(315, 23);
            this.btn_loadmap.TabIndex = 2;
            this.btn_loadmap.Text = "Load Map";
            this.btn_loadmap.UseVisualStyleBackColor = true;
            this.btn_loadmap.Click += new System.EventHandler(this.btn_loadmap_Click);
            // 
            // flow_tiles
            // 
            this.flow_tiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flow_tiles.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flow_tiles.Location = new System.Drawing.Point(12, 99);
            this.flow_tiles.Name = "flow_tiles";
            this.flow_tiles.Size = new System.Drawing.Size(315, 541);
            this.flow_tiles.TabIndex = 3;
            // 
            // btn_newmap
            // 
            this.btn_newmap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_newmap.Location = new System.Drawing.Point(12, 41);
            this.btn_newmap.Name = "btn_newmap";
            this.btn_newmap.Size = new System.Drawing.Size(315, 23);
            this.btn_newmap.TabIndex = 4;
            this.btn_newmap.Text = "New Map";
            this.btn_newmap.UseVisualStyleBackColor = true;
            this.btn_newmap.Click += new System.EventHandler(this.btn_newmap_Click);
            // 
            // TileSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 652);
            this.Controls.Add(this.btn_newmap);
            this.Controls.Add(this.flow_tiles);
            this.Controls.Add(this.btn_loadmap);
            this.Controls.Add(this.btn_savemap);
            this.Name = "TileSelection";
            this.Text = "TileSelection";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_savemap;
        private System.Windows.Forms.Button btn_loadmap;
        private System.Windows.Forms.FlowLayoutPanel flow_tiles;
        private System.Windows.Forms.Button btn_newmap;
    }
}