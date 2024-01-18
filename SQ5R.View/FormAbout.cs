using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SQ5R.Properties;

namespace SQ5R.View;

public class FormAbout : Form
{
    private readonly IContainer components = null;

    private Label label_MsgLine1;

    private Label label_MsgLine2;

    private Label label_MsgLine3;

    private Label label_MsgLine4;

    private PictureBox pictureBox1;

    public FormAbout()
    {
        InitializeComponent();
    }

    private void FormAbout_Load(object sender, EventArgs e)
    {
        var language = Settings.Default.language;
        if (language == "中文")
        {
            label_MsgLine1.Text = "SHX8800 编程软件(修改版)";
            label_MsgLine2.Text = "版本 v0.1";
            label_MsgLine3.Text = "无蓝牙支持";
#if NET461
            label_MsgLine3.Text = "蓝牙支持";
#endif
            label_MsgLine4.Text = "de SydneyOwl tu 73";
        }
        else
        {
            label_MsgLine1.Text = "SHX8800 Program Software";
            label_MsgLine2.Text = "Version v2.4";
            label_MsgLine3.Text = "No Bluetooth plugin";
#if NET461
            label_MsgLine3.Text = "Bluetooth plugin";
#endif
            label_MsgLine4.Text = "de SydneyOwl tu 73";
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
        label_MsgLine1.Location = new Point(106, 23);
        label_MsgLine1.Name = "label_MsgLine1";
        label_MsgLine1.Size = new Size(55, 15);
        label_MsgLine1.TabIndex = 0;
        label_MsgLine1.Text = "label1";
        label_MsgLine2.AutoSize = true;
        label_MsgLine2.Location = new Point(106, 51);
        label_MsgLine2.Name = "label_MsgLine2";
        label_MsgLine2.Size = new Size(55, 15);
        label_MsgLine2.TabIndex = 1;
        label_MsgLine2.Text = "label1";
        label_MsgLine3.AutoSize = true;
        label_MsgLine3.Location = new Point(106, 79);
        label_MsgLine3.Name = "label_MsgLine3";
        label_MsgLine3.Size = new Size(55, 15);
        label_MsgLine3.TabIndex = 2;
        label_MsgLine3.Text = "label1";
        label_MsgLine4.AutoSize = true;
        label_MsgLine4.Location = new Point(106, 107);
        label_MsgLine4.Name = "label_MsgLine4";
        label_MsgLine4.Size = new Size(55, 15);
        label_MsgLine4.TabIndex = 3;
        label_MsgLine4.Text = "label1";
        pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
        pictureBox1.BackgroundImageLayout = ImageLayout.Center;
        pictureBox1.Location = new Point(34, 33);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(49, 50);
        pictureBox1.TabIndex = 4;
        pictureBox1.TabStop = false;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(478, 149);
        Controls.Add(pictureBox1);
        Controls.Add(label_MsgLine4);
        Controls.Add(label_MsgLine3);
        Controls.Add(label_MsgLine2);
        Controls.Add(label_MsgLine1);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormAbout";
        StartPosition = FormStartPosition.CenterParent;
        Text = "SHX8800";
        Load += FormAbout_Load;
        ((ISupportInitialize)pictureBox1).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}