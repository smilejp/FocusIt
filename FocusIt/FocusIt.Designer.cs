namespace FocusIt
{
    partial class FocusIt
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.time_label = new System.Windows.Forms.Label();
            this.focus_progress = new System.Windows.Forms.ProgressBar();
            this.focus_bt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // time_label
            // 
            this.time_label.Font = new System.Drawing.Font("돋움", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.time_label.Location = new System.Drawing.Point(1, 3);
            this.time_label.Margin = new System.Windows.Forms.Padding(0);
            this.time_label.Name = "time_label";
            this.time_label.Size = new System.Drawing.Size(94, 27);
            this.time_label.TabIndex = 0;
            this.time_label.Text = "00:00";
            this.time_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // focus_progress
            // 
            this.focus_progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.focus_progress.Location = new System.Drawing.Point(1, 31);
            this.focus_progress.Name = "focus_progress";
            this.focus_progress.Size = new System.Drawing.Size(97, 23);
            this.focus_progress.TabIndex = 1;
            // 
            // focus_bt
            // 
            this.focus_bt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.focus_bt.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.focus_bt.Location = new System.Drawing.Point(98, 0);
            this.focus_bt.Name = "focus_bt";
            this.focus_bt.Size = new System.Drawing.Size(45, 55);
            this.focus_bt.TabIndex = 2;
            this.focus_bt.Text = "집중 시작";
            this.focus_bt.UseVisualStyleBackColor = true;
            this.focus_bt.Click += new System.EventHandler(this.focus_bt_Click);
            // 
            // FocusIt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(144, 56);
            this.Controls.Add(this.focus_bt);
            this.Controls.Add(this.focus_progress);
            this.Controls.Add(this.time_label);
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FocusIt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FocusIt";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Focus_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Focus_Paint);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Focus_Closing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label time_label;
        private System.Windows.Forms.ProgressBar focus_progress;
        private System.Windows.Forms.Button focus_bt;
    }
}

