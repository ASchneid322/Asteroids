namespace Asteroids1._0
{
    partial class AsteroidsForm
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
            this.components = new System.ComponentModel.Container();
            this.UI_TAst = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // UI_TAst
            // 
            this.UI_TAst.Enabled = true;
            this.UI_TAst.Interval = 25;
            this.UI_TAst.Tick += new System.EventHandler(this.UI_TAst_Tick);
            // 
            // AsteroidsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.MaximumSize = new System.Drawing.Size(1024, 768);
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "AsteroidsForm";
            this.Text = "Asteroids";
            this.Load += new System.EventHandler(this.Asteroids_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AsteroidsForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AsteroidsForm_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AsteroidsForm_MouseClick);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer UI_TAst;
    }
}

