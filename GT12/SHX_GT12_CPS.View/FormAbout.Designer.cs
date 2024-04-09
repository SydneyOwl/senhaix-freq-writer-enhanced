using System.ComponentModel;

namespace SHX_GT12_CPS.View;

partial class FormAbout
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
        this.label1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.label4 = new System.Windows.Forms.Label();
        this.label5 = new System.Windows.Forms.Label();
        this.label6 = new System.Windows.Forms.Label();
        this.ver = new System.Windows.Forms.Label();
        this.commit = new System.Windows.Forms.Label();
        this.ctime = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // label1
        // 
        this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
        this.label1.Location = new System.Drawing.Point(21, 50);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(45, 50);
        this.label1.TabIndex = 0;
        // 
        // label2
        // 
        this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.label2.Location = new System.Drawing.Point(89, 27);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(246, 47);
        this.label2.TabIndex = 1;
        this.label2.Text = "森海克斯GT12写频软件修改版";
        // 
        // label3
        // 
        this.label3.Location = new System.Drawing.Point(101, 73);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(75, 27);
        this.label3.TabIndex = 2;
        this.label3.Text = "版本号：";
        // 
        // label4
        // 
        this.label4.Location = new System.Drawing.Point(101, 101);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(75, 21);
        this.label4.TabIndex = 3;
        this.label4.Text = "提交Hash:";
        // 
        // label5
        // 
        this.label5.Location = new System.Drawing.Point(45, 188);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(323, 76);
        this.label5.TabIndex = 4;
        this.label5.Text = "https://github.com/SydneyOwl/senhaix-freq-writer-enhanced";
        // 
        // label6
        // 
        this.label6.Location = new System.Drawing.Point(101, 131);
        this.label6.Name = "label6";
        this.label6.Size = new System.Drawing.Size(217, 44);
        this.label6.TabIndex = 5;
        this.label6.Text = "编译时间：";
        this.label6.Click += new System.EventHandler(this.label6_Click_1);
        // 
        // ver
        // 
        this.ver.Location = new System.Drawing.Point(156, 73);
        this.ver.Name = "ver";
        this.ver.Size = new System.Drawing.Size(114, 25);
        this.ver.TabIndex = 6;
        this.ver.Text = "（未知）";
        // 
        // commit
        // 
        this.commit.Location = new System.Drawing.Point(156, 101);
        this.commit.Name = "commit";
        this.commit.Size = new System.Drawing.Size(114, 28);
        this.commit.TabIndex = 7;
        this.commit.Text = "（未知）";
        // 
        // ctime
        // 
        this.ctime.Location = new System.Drawing.Point(156, 131);
        this.ctime.Name = "ctime";
        this.ctime.Size = new System.Drawing.Size(114, 25);
        this.ctime.TabIndex = 8;
        this.ctime.Text = "（未知）";
        // 
        // FormAbout
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(347, 223);
        this.Controls.Add(this.ctime);
        this.Controls.Add(this.commit);
        this.Controls.Add(this.ver);
        this.Controls.Add(this.label6);
        this.Controls.Add(this.label5);
        this.Controls.Add(this.label4);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.label1);
        this.Name = "FormAbout";
        this.Text = "关于";
        this.ResumeLayout(false);
    }

    private System.Windows.Forms.Label ver;
    private System.Windows.Forms.Label commit;
    private System.Windows.Forms.Label ctime;

    private System.Windows.Forms.Label label6;

    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;

    private System.Windows.Forms.Label label2;

    private System.Windows.Forms.Label label1;

    #endregion
}