using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SQ5R.View;

namespace BF_H802_Import_Picture_tools;

public class FormIPT : Form
{
    private readonly IContainer components = null;

    private readonly string portName = "";
    // private ComboBox cbB_Portlist;

    private Button btn_Import;

    private Button btn_OpenPic;

    private delUpdateProgressValue delProgressValue;

    private string filePath = "";

    private bool flagBusy;

    private Label label1;

    private Label label2;

    private OpenFileDialog openFileDialog;

    private Panel panel1;

    private PictureBox pictureBox;

    private Point posImage;

    private ProgressBar progressBar;

    public FormIPT(string port)
    {
        portName = port;
        if (string.IsNullOrEmpty(port))
        {
            MessageBox.Show("请先选择端口！");
            return;
        }

        InitializeComponent();
    }

    // private bool UpdatePortlist()
    // {
    // 	string[] portNames = SerialPort.GetPortNames();
    // 	cbB_Portlist.Items.Clear();
    // 	if (portNames.Length != 0)
    // 	{
    // 		cbB_Portlist.Items.AddRange(portNames);
    // 		if (cbB_Portlist.Items.IndexOf(portName) != -1)
    // 		{
    // 			cbB_Portlist.Text = portName;
    // 		}
    // 		else
    // 		{
    // 			cbB_Portlist.SelectedIndex = 0;
    // 			portName = cbB_Portlist.Text;
    // 		}
    // 		return true;
    // 	}
    // 	return false;
    // }

    // private void cbB_Portlist_Click(object sender, EventArgs e)
    // {
    // 	if (!UpdatePortlist())
    // 	{
    // 		portName = "";
    // 	}
    // }

    private void btn_OpenPic_Click(object sender, EventArgs e)
    {
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            filePath = openFileDialog.FileName;
            var image = Image.FromFile(filePath);
            if (image.Width != 128 || image.Height != 128)
                MessageBox.Show("该型号只支持图片尺寸为128x128", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            else
                pictureBox.Image = image;
        }
    }

    private void ProgressUpdate_ComInfo(object sender, ComInfoEventArgs e)
    {
        Invoke(delProgressValue, e.ProgressValue);
    }

    private void UpdateProgressValue(int val)
    {
        progressBar.Value = val;
    }

    private void UpdateComResult(bool result)
    {
        btn_Import.Enabled = true;
    }

    private void btn_Import_Click(object sender, EventArgs e)
    {
        if (flagBusy)
        {
            MessageBox.Show("通信繁忙,请等待当前通信完成!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return;
        }

        if (pictureBox.Image == null)
        {
            MessageBox.Show("请选择要导入的图片!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return;
        }

        // if (cbB_Portlist.Text == "")
        // {
        // 	MessageBox.Show("请选择端口!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        // 	return;
        // }
        var importBmpHelper = new ImportBmpHelper(pictureBox.Image, 0);
        if (importBmpHelper.LinkDevice(portName) != -1)
        {
            btn_Import.Enabled = false;
            delProgressValue = UpdateProgressValue;
            ParameterizedThreadStart start = Task_Communication;
            var thread = new Thread(start);
            thread.Start(importBmpHelper);
        }
        else
        {
            MessageBox.Show("端口打开失败!请确认!", "错误", MessageBoxButtons.OK);
        }
    }

    private void Task_Communication(object importHelper)
    {
        var importBmpHelper = (ImportBmpHelper)importHelper;
        importBmpHelper.ProgressUpdate.ComInfo += ProgressUpdate_ComInfo;
        if (importBmpHelper.Doit())
            MessageBox.Show("导入完成", "提示", MessageBoxButtons.OK);
        else
            MessageBox.Show("导入失败", "提示", MessageBoxButtons.OK);

        Invoke(new delUpdateComResult(UpdateComResult), true);
        flagBusy = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var resources =
            new ComponentResourceManager(typeof(FormIPT));
        pictureBox = new PictureBox();
        btn_OpenPic = new Button();
        openFileDialog = new OpenFileDialog();
        panel1 = new Panel();
        label1 = new Label();
        // this.cbB_Portlist = new System.Windows.Forms.ComboBox();
        btn_Import = new Button();
        progressBar = new ProgressBar();
        label2 = new Label();
        ((ISupportInitialize)pictureBox).BeginInit();
        panel1.SuspendLayout();
        SuspendLayout();
        pictureBox.BackColor = Color.White;
        pictureBox.Dock = DockStyle.Fill;
        pictureBox.Location = new Point(0, 0);
        pictureBox.Name = "pictureBox";
        pictureBox.Size = new Size(156, 156);
        pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
        pictureBox.TabIndex = 0;
        pictureBox.TabStop = false;
        btn_OpenPic.Location = new Point(263, 90);
        btn_OpenPic.Name = "btn_OpenPic";
        btn_OpenPic.Size = new Size(149, 39);
        btn_OpenPic.TabIndex = 2;
        btn_OpenPic.Text = "打开图片";
        btn_OpenPic.UseVisualStyleBackColor = true;
        btn_OpenPic.Click += btn_OpenPic_Click;
        openFileDialog.Filter = "BMP(*.bmp)|*.BMP";
        panel1.BorderStyle = BorderStyle.Fixed3D;
        panel1.Controls.Add(pictureBox);
        panel1.Location = new Point(51, 68);
        panel1.Name = "panel1";
        panel1.Size = new Size(160, 160);
        panel1.TabIndex = 3;
        label1.AutoSize = true;
        label1.Location = new Point(30, 26);
        label1.Name = "label1";
        label1.Size = new Size(37, 15);
        label1.TabIndex = 4;
        label1.Text = "端口:" + portName;
        // this.cbB_Portlist.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        // this.cbB_Portlist.FormattingEnabled = true;
        // this.cbB_Portlist.Location = new System.Drawing.Point(73, 22);
        // this.cbB_Portlist.Name = "cbB_Portlist";
        // this.cbB_Portlist.Size = new System.Drawing.Size(121, 23);
        // this.cbB_Portlist.TabIndex = 5;
        // this.cbB_Portlist.Click += new System.EventHandler(cbB_Portlist_Click);
        btn_Import.Location = new Point(263, 160);
        btn_Import.Name = "btn_Import";
        btn_Import.Size = new Size(149, 39);
        btn_Import.TabIndex = 6;
        btn_Import.Text = "导入";
        btn_Import.UseVisualStyleBackColor = true;
        btn_Import.Click += btn_Import_Click;
        progressBar.Dock = DockStyle.Bottom;
        progressBar.Location = new Point(0, 289);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(482, 10);
        progressBar.TabIndex = 7;
        label2.ForeColor = Color.Red;
        label2.Location = new Point(61, 242);
        label2.Name = "label2";
        label2.Size = new Size(165, 28);
        label2.TabIndex = 8;
        label2.Text = "图片尺寸: 128x128";
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(482, 299);
        Controls.Add(label2);
        Controls.Add(progressBar);
        Controls.Add(btn_Import);
        // base.Controls.Add(this.cbB_Portlist);
        Controls.Add(label1);
        Controls.Add(panel1);
        Controls.Add(btn_OpenPic);
        Name = "FormMain";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "图片导入工具 v1.3";
        ((ISupportInitialize)pictureBox).EndInit();
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        panel1.Width = 128;
        panel1.Height = 128;
        posImage = panel1.Location;
        ResumeLayout(false);
        PerformLayout();
    }

    private delegate void delUpdateProgressValue(int val);

    private delegate void delUpdateComResult(bool result);
}