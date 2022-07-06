namespace DrawTest
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
         this.offsetLabel = new System.Windows.Forms.Label();
         this.SuspendLayout();
         // 
         // offsetLabel
         // 
         this.offsetLabel.AutoSize = true;
         this.offsetLabel.Location = new System.Drawing.Point(237, 239);
         this.offsetLabel.Name = "offsetLabel";
         this.offsetLabel.Size = new System.Drawing.Size(35, 13);
         this.offsetLabel.TabIndex = 0;
         this.offsetLabel.Text = "label1";
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(284, 261);
         this.Controls.Add(this.offsetLabel);
         this.Name = "Form1";
         this.Text = "Form1";
         this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
         this.Load += new System.EventHandler(this.Form1_Load);
         this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label offsetLabel;
   }
}

