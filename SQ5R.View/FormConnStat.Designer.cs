using System.ComponentModel;

namespace SQ5R.View;

partial class FormConnStat
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

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
        this.labelStatus = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // labelStatus
        // 
        this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.labelStatus.Location = new System.Drawing.Point(-3, -3);
        this.labelStatus.Name = "labelStatus";
        this.labelStatus.Size = new System.Drawing.Size(421, 197);
        this.labelStatus.TabIndex = 0;
        this.labelStatus.Text = "连接中...";
        this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // FormConnStat
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(418, 193);
        this.ControlBox = false;
        this.Controls.Add(this.labelStatus);
        this.Name = "FormConnStat";
        this.Text = "连接状态";
        this.ResumeLayout(false);
    }

    public System.Windows.Forms.Label labelStatus;

    #endregion
}