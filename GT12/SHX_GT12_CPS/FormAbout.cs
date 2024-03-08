using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SHX_GT12_CPS;

public class FormAbout : Form
{
    private readonly IContainer components = null;

    private Label label_MsgLine1;

    private Label label_MsgLine2;

    private Label label_MsgLine3;

    private Label label_MsgLine4;
    private readonly string lang = "中文";

    private PictureBox pictureBox1;

    public FormAbout()
    {
        InitializeComponent();
    }

    private void FormAbout_Load(object sender, EventArgs e)
    {
        if (lang == "中文")
        {
            label_MsgLine1.Text = "GT12 编程软件修改版";
            label_MsgLine2.Text = "版本 v0.1";
            label_MsgLine3.Text = "73 de SydneyOwl";
            label_MsgLine4.Text = "";
        }
        else
        {
            label_MsgLine1.Text = "GT12 Program Software";
            label_MsgLine2.Text = "Version v1.0.9";
            label_MsgLine3.Text = "Date: 1/16/2023    Copyright(C) 2023";
            label_MsgLine4.Text = "GT12 Corporation all rights reserved";
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var resources =
            new ComponentResourceManager(typeof(FormAbout));
        label_MsgLine1 = new Label();
        label_MsgLine2 = new Label();
        label_MsgLine3 = new Label();
        label_MsgLine4 = new Label();
        pictureBox1 = new PictureBox();
        ((ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        label_MsgLine1.AutoSize = true;
        label_MsgLine1.Location = new Point(132, 35);
        label_MsgLine1.Name = "label_MsgLine1";
        label_MsgLine1.Size = new Size(55, 15);
        label_MsgLine1.TabIndex = 0;
        label_MsgLine1.Text = "label1";
        label_MsgLine2.AutoSize = true;
        label_MsgLine2.Location = new Point(132, 66);
        label_MsgLine2.Name = "label_MsgLine2";
        label_MsgLine2.Size = new Size(55, 15);
        label_MsgLine2.TabIndex = 1;
        label_MsgLine2.Text = "label2";
        label_MsgLine3.AutoSize = true;
        label_MsgLine3.Location = new Point(132, 100);
        label_MsgLine3.Name = "label_MsgLine3";
        label_MsgLine3.Size = new Size(55, 15);
        label_MsgLine3.TabIndex = 2;
        label_MsgLine3.Text = "label3";
        label_MsgLine4.AutoSize = true;
        label_MsgLine4.Location = new Point(132, 132);
        label_MsgLine4.Name = "label_MsgLine4";
        label_MsgLine4.Size = new Size(55, 15);
        label_MsgLine4.TabIndex = 3;
        label_MsgLine4.Text = "label4";
        pictureBox1.BackgroundImageLayout = ImageLayout.Center;
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(46, 65);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(50, 50);
        pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
        pictureBox1.TabIndex = 4;
        pictureBox1.TabStop = false;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(529, 183);
        Controls.Add(pictureBox1);
        Controls.Add(label_MsgLine4);
        Controls.Add(label_MsgLine3);
        Controls.Add(label_MsgLine2);
        Controls.Add(label_MsgLine1);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "FormAbout";
        StartPosition = FormStartPosition.CenterParent;
        Text = "GT12";
        Load += FormAbout_Load;
        ((ISupportInitialize)pictureBox1).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}