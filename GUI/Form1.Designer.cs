namespace GUI
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
			this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
			this.materialLabel4 = new MaterialSkin.Controls.MaterialLabel();
			this.materialLabel5 = new MaterialSkin.Controls.MaterialLabel();
			this.materialRaisedButton2 = new MaterialSkin.Controls.MaterialRaisedButton();
			this.materialRaisedButton3 = new MaterialSkin.Controls.MaterialRaisedButton();
			this.materialLabel3 = new MaterialSkin.Controls.MaterialLabel();
			this.materialRaisedButton1 = new MaterialSkin.Controls.MaterialRaisedButton();
			this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// materialLabel2
			// 
			this.materialLabel2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.materialLabel2.AutoSize = true;
			this.materialLabel2.Depth = 0;
			this.materialLabel2.Font = new System.Drawing.Font("Roboto", 11F);
			this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.materialLabel2.Location = new System.Drawing.Point(505, 114);
			this.materialLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
			this.materialLabel2.Name = "materialLabel2";
			this.materialLabel2.Size = new System.Drawing.Size(124, 24);
			this.materialLabel2.TabIndex = 1;
			this.materialLabel2.Text = "Select File 2 :";
			this.materialLabel2.Click += new System.EventHandler(this.materialLabel2_Click);
			// 
			// materialLabel4
			// 
			this.materialLabel4.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.materialLabel4.AutoSize = true;
			this.materialLabel4.Depth = 0;
			this.materialLabel4.Font = new System.Drawing.Font("Roboto", 11F);
			this.materialLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.materialLabel4.Location = new System.Drawing.Point(548, 147);
			this.materialLabel4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.materialLabel4.MouseState = MaterialSkin.MouseState.HOVER;
			this.materialLabel4.Name = "materialLabel4";
			this.materialLabel4.Size = new System.Drawing.Size(150, 24);
			this.materialLabel4.TabIndex = 3;
			this.materialLabel4.Text = "-no file selected-";
			// 
			// materialLabel5
			// 
			this.materialLabel5.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.materialLabel5.AutoSize = true;
			this.materialLabel5.Depth = 0;
			this.materialLabel5.Font = new System.Drawing.Font("Roboto", 11F);
			this.materialLabel5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.materialLabel5.Location = new System.Drawing.Point(307, 351);
			this.materialLabel5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.materialLabel5.MouseState = MaterialSkin.MouseState.HOVER;
			this.materialLabel5.Name = "materialLabel5";
			this.materialLabel5.Size = new System.Drawing.Size(154, 24);
			this.materialLabel5.TabIndex = 4;
			this.materialLabel5.Text = "Plagiarism Score";
			this.materialLabel5.Click += new System.EventHandler(this.materialLabel5_Click);
			// 
			// materialRaisedButton2
			// 
			this.materialRaisedButton2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.materialRaisedButton2.Depth = 0;
			this.materialRaisedButton2.Location = new System.Drawing.Point(637, 112);
			this.materialRaisedButton2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.materialRaisedButton2.MouseState = MaterialSkin.MouseState.HOVER;
			this.materialRaisedButton2.Name = "materialRaisedButton2";
			this.materialRaisedButton2.Primary = true;
			this.materialRaisedButton2.Size = new System.Drawing.Size(129, 31);
			this.materialRaisedButton2.TabIndex = 6;
			this.materialRaisedButton2.Text = "Browse";
			this.materialRaisedButton2.UseVisualStyleBackColor = true;
			this.materialRaisedButton2.Click += new System.EventHandler(this.materialRaisedButton2_Click);
			// 
			// materialRaisedButton3
			// 
			this.materialRaisedButton3.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.materialRaisedButton3.Depth = 0;
			this.materialRaisedButton3.Location = new System.Drawing.Point(305, 237);
			this.materialRaisedButton3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.materialRaisedButton3.MouseState = MaterialSkin.MouseState.HOVER;
			this.materialRaisedButton3.Name = "materialRaisedButton3";
			this.materialRaisedButton3.Primary = true;
			this.materialRaisedButton3.Size = new System.Drawing.Size(156, 64);
			this.materialRaisedButton3.TabIndex = 7;
			this.materialRaisedButton3.Text = "Check";
			this.materialRaisedButton3.UseVisualStyleBackColor = true;
			this.materialRaisedButton3.Click += new System.EventHandler(this.materialRaisedButton3_Click);
			// 
			// materialLabel3
			// 
			this.materialLabel3.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.materialLabel3.AutoSize = true;
			this.materialLabel3.Depth = 0;
			this.materialLabel3.Font = new System.Drawing.Font("Roboto", 11F);
			this.materialLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.materialLabel3.Location = new System.Drawing.Point(72, 147);
			this.materialLabel3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.materialLabel3.MouseState = MaterialSkin.MouseState.HOVER;
			this.materialLabel3.Name = "materialLabel3";
			this.materialLabel3.Size = new System.Drawing.Size(150, 24);
			this.materialLabel3.TabIndex = 2;
			this.materialLabel3.Text = "-no file selected-";
			this.materialLabel3.Click += new System.EventHandler(this.materialLabel3_Click);
			// 
			// materialRaisedButton1
			// 
			this.materialRaisedButton1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.materialRaisedButton1.Depth = 0;
			this.materialRaisedButton1.Location = new System.Drawing.Point(167, 112);
			this.materialRaisedButton1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.materialRaisedButton1.MouseState = MaterialSkin.MouseState.HOVER;
			this.materialRaisedButton1.Name = "materialRaisedButton1";
			this.materialRaisedButton1.Primary = true;
			this.materialRaisedButton1.Size = new System.Drawing.Size(129, 31);
			this.materialRaisedButton1.TabIndex = 5;
			this.materialRaisedButton1.Text = "Browse";
			this.materialRaisedButton1.UseVisualStyleBackColor = true;
			this.materialRaisedButton1.Click += new System.EventHandler(this.materialRaisedButton1_Click);
			// 
			// materialLabel1
			// 
			this.materialLabel1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.materialLabel1.AutoSize = true;
			this.materialLabel1.Depth = 0;
			this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
			this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.materialLabel1.Location = new System.Drawing.Point(13, 114);
			this.materialLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
			this.materialLabel1.Name = "materialLabel1";
			this.materialLabel1.Size = new System.Drawing.Size(124, 24);
			this.materialLabel1.TabIndex = 10;
			this.materialLabel1.Text = "Select File 1 :";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(362, 305);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 46);
			this.label1.TabIndex = 11;
			this.label1.Text = "0";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(779, 446);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.materialLabel1);
			this.Controls.Add(this.materialRaisedButton3);
			this.Controls.Add(this.materialRaisedButton2);
			this.Controls.Add(this.materialRaisedButton1);
			this.Controls.Add(this.materialLabel5);
			this.Controls.Add(this.materialLabel4);
			this.Controls.Add(this.materialLabel3);
			this.Controls.Add(this.materialLabel2);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "Form1";
			this.Text = "Plagiarism Checker";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialLabel materialLabel4;
        private MaterialSkin.Controls.MaterialLabel materialLabel5;
        private MaterialSkin.Controls.MaterialRaisedButton materialRaisedButton2;
        private MaterialSkin.Controls.MaterialRaisedButton materialRaisedButton3;
        private MaterialSkin.Controls.MaterialLabel materialLabel3;
        private MaterialSkin.Controls.MaterialRaisedButton materialRaisedButton1;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Label label1;
    }
}

