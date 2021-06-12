namespace FileCopy {
	partial class FileCopy {
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxSrc = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxDst = new System.Windows.Forms.TextBox();
			this.buttonCopy = new System.Windows.Forms.Button();
			this.radioButtonOverwrite = new System.Windows.Forms.RadioButton();
			this.groupBoxCopyMethod = new System.Windows.Forms.GroupBox();
			this.radioButtonUpdate = new System.Windows.Forms.RadioButton();
			this.radioButtonSkip = new System.Windows.Forms.RadioButton();
			this.buttonSuspension = new System.Windows.Forms.Button();
			this.checkBoxCopyCreateTime = new System.Windows.Forms.CheckBox();
			this.groupBoxCopyInfo = new System.Windows.Forms.GroupBox();
			this.checkBoxCopyLastAccessTime = new System.Windows.Forms.CheckBox();
			this.checkBoxCopyLastWrittenTime = new System.Windows.Forms.CheckBox();
			this.checkBoxCopySubDirectory = new System.Windows.Forms.CheckBox();
			this.buttonCountFiles = new System.Windows.Forms.Button();
			this.buttonRecoverDate = new System.Windows.Forms.Button();
			this.treeViewWpdSrc = new System.Windows.Forms.TreeView();
			this.treeViewWpdDst = new System.Windows.Forms.TreeView();
			this.progressBarDownload = new System.Windows.Forms.ProgressBar();
			this.labelDownload = new System.Windows.Forms.Label();
			this.groupBoxCopyMethod.SuspendLayout();
			this.groupBoxCopyInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(34, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "コピー元フォルダ";
			// 
			// textBoxSrc
			// 
			this.textBoxSrc.Location = new System.Drawing.Point(36, 60);
			this.textBoxSrc.Name = "textBoxSrc";
			this.textBoxSrc.Size = new System.Drawing.Size(372, 19);
			this.textBoxSrc.TabIndex = 1;
			this.textBoxSrc.TextChanged += new System.EventHandler(this.textBoxSrc_TextChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(459, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "コピー先フォルダ";
			// 
			// textBoxDst
			// 
			this.textBoxDst.Location = new System.Drawing.Point(459, 61);
			this.textBoxDst.Name = "textBoxDst";
			this.textBoxDst.Size = new System.Drawing.Size(372, 19);
			this.textBoxDst.TabIndex = 3;
			this.textBoxDst.TextChanged += new System.EventHandler(this.textBoxDst_TextChanged);
			// 
			// buttonCopy
			// 
			this.buttonCopy.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonCopy.Location = new System.Drawing.Point(758, 461);
			this.buttonCopy.Name = "buttonCopy";
			this.buttonCopy.Size = new System.Drawing.Size(73, 34);
			this.buttonCopy.TabIndex = 6;
			this.buttonCopy.Text = "コピー";
			this.buttonCopy.UseVisualStyleBackColor = true;
			this.buttonCopy.Click += new System.EventHandler(this.buttonCopy_Click);
			// 
			// radioButtonOverwrite
			// 
			this.radioButtonOverwrite.AutoSize = true;
			this.radioButtonOverwrite.Location = new System.Drawing.Point(7, 62);
			this.radioButtonOverwrite.Name = "radioButtonOverwrite";
			this.radioButtonOverwrite.Size = new System.Drawing.Size(56, 16);
			this.radioButtonOverwrite.TabIndex = 7;
			this.radioButtonOverwrite.TabStop = true;
			this.radioButtonOverwrite.Text = "上書き";
			this.radioButtonOverwrite.UseVisualStyleBackColor = true;
			this.radioButtonOverwrite.CheckedChanged += new System.EventHandler(this.radioButtonOverwrite_CheckedChanged);
			// 
			// groupBoxCopyMethod
			// 
			this.groupBoxCopyMethod.Controls.Add(this.radioButtonUpdate);
			this.groupBoxCopyMethod.Controls.Add(this.radioButtonSkip);
			this.groupBoxCopyMethod.Controls.Add(this.radioButtonOverwrite);
			this.groupBoxCopyMethod.Location = new System.Drawing.Point(36, 461);
			this.groupBoxCopyMethod.Name = "groupBoxCopyMethod";
			this.groupBoxCopyMethod.Size = new System.Drawing.Size(248, 128);
			this.groupBoxCopyMethod.TabIndex = 8;
			this.groupBoxCopyMethod.TabStop = false;
			this.groupBoxCopyMethod.Text = "同名ファイル発見時の動作";
			// 
			// radioButtonUpdate
			// 
			this.radioButtonUpdate.AutoSize = true;
			this.radioButtonUpdate.Location = new System.Drawing.Point(7, 93);
			this.radioButtonUpdate.Name = "radioButtonUpdate";
			this.radioButtonUpdate.Size = new System.Drawing.Size(108, 16);
			this.radioButtonUpdate.TabIndex = 9;
			this.radioButtonUpdate.TabStop = true;
			this.radioButtonUpdate.Text = "新しければ上書き";
			this.radioButtonUpdate.UseVisualStyleBackColor = true;
			this.radioButtonUpdate.CheckedChanged += new System.EventHandler(this.radioButtonUpdate_CheckedChanged);
			// 
			// radioButtonSkip
			// 
			this.radioButtonSkip.AutoSize = true;
			this.radioButtonSkip.Checked = true;
			this.radioButtonSkip.Location = new System.Drawing.Point(7, 30);
			this.radioButtonSkip.Name = "radioButtonSkip";
			this.radioButtonSkip.Size = new System.Drawing.Size(58, 16);
			this.radioButtonSkip.TabIndex = 8;
			this.radioButtonSkip.TabStop = true;
			this.radioButtonSkip.Text = "スキップ";
			this.radioButtonSkip.UseVisualStyleBackColor = true;
			this.radioButtonSkip.CheckedChanged += new System.EventHandler(this.radioButtonSkip_CheckedChanged);
			// 
			// buttonSuspension
			// 
			this.buttonSuspension.Enabled = false;
			this.buttonSuspension.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonSuspension.Location = new System.Drawing.Point(758, 516);
			this.buttonSuspension.Name = "buttonSuspension";
			this.buttonSuspension.Size = new System.Drawing.Size(73, 34);
			this.buttonSuspension.TabIndex = 9;
			this.buttonSuspension.Text = "中断";
			this.buttonSuspension.UseVisualStyleBackColor = true;
			this.buttonSuspension.Click += new System.EventHandler(this.buttonSuspension_Click);
			// 
			// checkBoxCopyCreateTime
			// 
			this.checkBoxCopyCreateTime.AutoSize = true;
			this.checkBoxCopyCreateTime.Checked = true;
			this.checkBoxCopyCreateTime.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCopyCreateTime.Location = new System.Drawing.Point(16, 31);
			this.checkBoxCopyCreateTime.Name = "checkBoxCopyCreateTime";
			this.checkBoxCopyCreateTime.Size = new System.Drawing.Size(72, 16);
			this.checkBoxCopyCreateTime.TabIndex = 0;
			this.checkBoxCopyCreateTime.Text = "作成日時";
			this.checkBoxCopyCreateTime.UseVisualStyleBackColor = true;
			this.checkBoxCopyCreateTime.CheckedChanged += new System.EventHandler(this.checkBoxCopyCreateTime_CheckedChanged);
			// 
			// groupBoxCopyInfo
			// 
			this.groupBoxCopyInfo.Controls.Add(this.checkBoxCopyLastAccessTime);
			this.groupBoxCopyInfo.Controls.Add(this.checkBoxCopyLastWrittenTime);
			this.groupBoxCopyInfo.Controls.Add(this.checkBoxCopyCreateTime);
			this.groupBoxCopyInfo.ForeColor = System.Drawing.SystemColors.ControlText;
			this.groupBoxCopyInfo.Location = new System.Drawing.Point(344, 461);
			this.groupBoxCopyInfo.Name = "groupBoxCopyInfo";
			this.groupBoxCopyInfo.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.groupBoxCopyInfo.Size = new System.Drawing.Size(219, 128);
			this.groupBoxCopyInfo.TabIndex = 10;
			this.groupBoxCopyInfo.TabStop = false;
			this.groupBoxCopyInfo.Text = "以下の情報をコピーする";
			// 
			// checkBoxCopyLastAccessTime
			// 
			this.checkBoxCopyLastAccessTime.AutoSize = true;
			this.checkBoxCopyLastAccessTime.Checked = true;
			this.checkBoxCopyLastAccessTime.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCopyLastAccessTime.Location = new System.Drawing.Point(16, 92);
			this.checkBoxCopyLastAccessTime.Name = "checkBoxCopyLastAccessTime";
			this.checkBoxCopyLastAccessTime.Size = new System.Drawing.Size(84, 16);
			this.checkBoxCopyLastAccessTime.TabIndex = 2;
			this.checkBoxCopyLastAccessTime.Text = "アクセス日時";
			this.checkBoxCopyLastAccessTime.UseVisualStyleBackColor = true;
			this.checkBoxCopyLastAccessTime.CheckedChanged += new System.EventHandler(this.checkBoxCopyLastAccessTime_CheckedChanged);
			// 
			// checkBoxCopyLastWrittenTime
			// 
			this.checkBoxCopyLastWrittenTime.AutoSize = true;
			this.checkBoxCopyLastWrittenTime.Checked = true;
			this.checkBoxCopyLastWrittenTime.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCopyLastWrittenTime.Location = new System.Drawing.Point(16, 62);
			this.checkBoxCopyLastWrittenTime.Name = "checkBoxCopyLastWrittenTime";
			this.checkBoxCopyLastWrittenTime.Size = new System.Drawing.Size(72, 16);
			this.checkBoxCopyLastWrittenTime.TabIndex = 1;
			this.checkBoxCopyLastWrittenTime.Text = "更新日時";
			this.checkBoxCopyLastWrittenTime.UseVisualStyleBackColor = true;
			this.checkBoxCopyLastWrittenTime.CheckedChanged += new System.EventHandler(this.checkBoxCopyLastWrittenTime_CheckedChanged);
			// 
			// checkBoxCopySubDirectory
			// 
			this.checkBoxCopySubDirectory.AutoSize = true;
			this.checkBoxCopySubDirectory.Checked = true;
			this.checkBoxCopySubDirectory.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCopySubDirectory.Location = new System.Drawing.Point(36, 86);
			this.checkBoxCopySubDirectory.Name = "checkBoxCopySubDirectory";
			this.checkBoxCopySubDirectory.Size = new System.Drawing.Size(147, 16);
			this.checkBoxCopySubDirectory.TabIndex = 11;
			this.checkBoxCopySubDirectory.Text = "サブディレクトリもコピーする";
			this.checkBoxCopySubDirectory.UseVisualStyleBackColor = true;
			this.checkBoxCopySubDirectory.CheckedChanged += new System.EventHandler(this.checkBoxCopySubDirectory_CheckedChanged);
			// 
			// buttonCountFiles
			// 
			this.buttonCountFiles.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonCountFiles.Location = new System.Drawing.Point(665, 566);
			this.buttonCountFiles.Name = "buttonCountFiles";
			this.buttonCountFiles.Size = new System.Drawing.Size(166, 40);
			this.buttonCountFiles.TabIndex = 12;
			this.buttonCountFiles.Text = "コピーするファイル数を調べる";
			this.buttonCountFiles.UseVisualStyleBackColor = true;
			this.buttonCountFiles.Click += new System.EventHandler(this.buttonCountFiles_Click);
			// 
			// buttonRecoverDate
			// 
			this.buttonRecoverDate.Location = new System.Drawing.Point(693, 684);
			this.buttonRecoverDate.Name = "buttonRecoverDate";
			this.buttonRecoverDate.Size = new System.Drawing.Size(75, 23);
			this.buttonRecoverDate.TabIndex = 13;
			this.buttonRecoverDate.Text = "日付修正";
			this.buttonRecoverDate.UseVisualStyleBackColor = true;
			this.buttonRecoverDate.Click += new System.EventHandler(this.buttonRecoverDate_Click);
			// 
			// treeViewWpdSrc
			// 
			this.treeViewWpdSrc.Location = new System.Drawing.Point(36, 124);
			this.treeViewWpdSrc.Name = "treeViewWpdSrc";
			this.treeViewWpdSrc.Size = new System.Drawing.Size(372, 304);
			this.treeViewWpdSrc.TabIndex = 15;
			this.treeViewWpdSrc.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewWpdSrc_BeforeExpand);
			this.treeViewWpdSrc.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewWpdSrc_BeforeSelect);
			// 
			// treeViewWpdDst
			// 
			this.treeViewWpdDst.Location = new System.Drawing.Point(459, 124);
			this.treeViewWpdDst.Name = "treeViewWpdDst";
			this.treeViewWpdDst.Size = new System.Drawing.Size(372, 304);
			this.treeViewWpdDst.TabIndex = 16;
			this.treeViewWpdDst.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewWpdDst_BeforeExpand);
			this.treeViewWpdDst.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewWpdDst_BeforeSelect);
			// 
			// progressBarDownload
			// 
			this.progressBarDownload.Location = new System.Drawing.Point(36, 646);
			this.progressBarDownload.Name = "progressBarDownload";
			this.progressBarDownload.Size = new System.Drawing.Size(795, 23);
			this.progressBarDownload.TabIndex = 17;
			// 
			// labelDownload
			// 
			this.labelDownload.AutoSize = true;
			this.labelDownload.Location = new System.Drawing.Point(34, 617);
			this.labelDownload.Name = "labelDownload";
			this.labelDownload.Size = new System.Drawing.Size(35, 12);
			this.labelDownload.TabIndex = 18;
			this.labelDownload.Text = "label3";
			// 
			// FileCopy
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(889, 725);
			this.Controls.Add(this.labelDownload);
			this.Controls.Add(this.progressBarDownload);
			this.Controls.Add(this.treeViewWpdDst);
			this.Controls.Add(this.treeViewWpdSrc);
			this.Controls.Add(this.buttonRecoverDate);
			this.Controls.Add(this.buttonCountFiles);
			this.Controls.Add(this.checkBoxCopySubDirectory);
			this.Controls.Add(this.groupBoxCopyInfo);
			this.Controls.Add(this.buttonSuspension);
			this.Controls.Add(this.groupBoxCopyMethod);
			this.Controls.Add(this.buttonCopy);
			this.Controls.Add(this.textBoxDst);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxSrc);
			this.Controls.Add(this.label1);
			this.Name = "FileCopy";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.FileCopy_Load);
			this.groupBoxCopyMethod.ResumeLayout(false);
			this.groupBoxCopyMethod.PerformLayout();
			this.groupBoxCopyInfo.ResumeLayout(false);
			this.groupBoxCopyInfo.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxSrc;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxDst;
		private System.Windows.Forms.Button buttonCopy;
		private System.Windows.Forms.RadioButton radioButtonOverwrite;
		private System.Windows.Forms.GroupBox groupBoxCopyMethod;
		private System.Windows.Forms.RadioButton radioButtonUpdate;
		private System.Windows.Forms.RadioButton radioButtonSkip;
		private System.Windows.Forms.Button buttonSuspension;
		private System.Windows.Forms.CheckBox checkBoxCopyCreateTime;
		private System.Windows.Forms.GroupBox groupBoxCopyInfo;
		private System.Windows.Forms.CheckBox checkBoxCopyLastAccessTime;
		private System.Windows.Forms.CheckBox checkBoxCopyLastWrittenTime;
		private System.Windows.Forms.CheckBox checkBoxCopySubDirectory;
		private System.Windows.Forms.Button buttonCountFiles;
		private System.Windows.Forms.Button buttonRecoverDate;
		private System.Windows.Forms.TreeView treeViewWpdSrc;
		private System.Windows.Forms.TreeView treeViewWpdDst;
		private System.Windows.Forms.ProgressBar progressBarDownload;
		private System.Windows.Forms.Label labelDownload;
	}
}

