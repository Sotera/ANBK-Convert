namespace JistBridge.UI.ANBKChart
{
	partial class ANBKContainer
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ANBKContainer));
			this.axi2LinkView1 = new AxInterop.i2NotebookView.Axi2LinkView();
			this.axi2LinkData1 = new AxInterop.i2NotebookData.Axi2LinkData();
			this.axi2LinkConnector1 = new AxInterop.i2NotebookConnector.Axi2LinkConnector();
			((System.ComponentModel.ISupportInitialize)(this.axi2LinkView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.axi2LinkData1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.axi2LinkConnector1)).BeginInit();
			this.SuspendLayout();
			// 
			// axi2LinkView1
			// 
			this.axi2LinkView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.axi2LinkView1.Enabled = true;
			this.axi2LinkView1.Location = new System.Drawing.Point(0, 0);
			this.axi2LinkView1.Name = "axi2LinkView1";
			this.axi2LinkView1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axi2LinkView1.OcxState")));
			this.axi2LinkView1.Size = new System.Drawing.Size(584, 427);
			this.axi2LinkView1.TabIndex = 0;
			// 
			// axi2LinkData1
			// 
			this.axi2LinkData1.Enabled = true;
			this.axi2LinkData1.Location = new System.Drawing.Point(397, 276);
			this.axi2LinkData1.Name = "axi2LinkData1";
			this.axi2LinkData1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axi2LinkData1.OcxState")));
			this.axi2LinkData1.Size = new System.Drawing.Size(33, 31);
			this.axi2LinkData1.TabIndex = 1;
			// 
			// axi2LinkConnector1
			// 
			this.axi2LinkConnector1.Enabled = true;
			this.axi2LinkConnector1.Location = new System.Drawing.Point(336, 276);
			this.axi2LinkConnector1.Name = "axi2LinkConnector1";
			this.axi2LinkConnector1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axi2LinkConnector1.OcxState")));
			this.axi2LinkConnector1.Size = new System.Drawing.Size(33, 31);
			this.axi2LinkConnector1.TabIndex = 2;
			// 
			// ANBKContainer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.axi2LinkConnector1);
			this.Controls.Add(this.axi2LinkData1);
			this.Controls.Add(this.axi2LinkView1);
			this.Name = "ANBKContainer";
			this.Size = new System.Drawing.Size(584, 427);
			((System.ComponentModel.ISupportInitialize)(this.axi2LinkView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.axi2LinkData1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.axi2LinkConnector1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private AxInterop.i2NotebookView.Axi2LinkView axi2LinkView1;
		private AxInterop.i2NotebookData.Axi2LinkData axi2LinkData1;
		private AxInterop.i2NotebookConnector.Axi2LinkConnector axi2LinkConnector1;
	}
}
