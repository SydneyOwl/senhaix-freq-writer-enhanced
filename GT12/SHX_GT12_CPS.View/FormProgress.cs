using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using HID;

namespace SHX_GT12_CPS.View;

public class FormProgress : Form
{
    private readonly IContainer components = null;
    private AppData appData;

    private Button btn_Cancel;

    private Button btn_Start;

    private Communication com;

    private HIDInterface hid;

    private Label label_ProgressVal;

    private ProgressBar progressBar;

    private Thread thread_Communication;

    public FormProgress()
    {
        InitializeComponent();
    }

    public void LoadData(AppData appData, HIDInterface hid)
    {
        this.appData = appData;
        this.hid = hid;
    }

    public AppData GetData()
    {
        return com.appData;
    }

    private void btn_Start_Click(object sender, EventArgs e)
    {
        btn_Start.Enabled = false;
        btn_Cancel.Enabled = false;
        progressBar.Value = 0;
        thread_Communication = new Thread(Task_Communication);
        thread_Communication.Start();
    }

    private void btn_Cancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void Task_Communication()
    {
        var flag = false;
        if (Text == "读频" || Text == "Read")
            com = new Communication(hid, appData, OP_TYPE.READ);
        else
            com = new Communication(hid, appData, OP_TYPE.WRITE);

        com.UpdateProgressBar.progressBarValue += UpdateProgressValue;
        var hIDInterface = hid;
        hIDInterface.DataReceived = (HIDInterface.DelegateDataReceived)Delegate.Combine(hIDInterface.DataReceived,
            new HIDInterface.DelegateDataReceived(com.DataReceived));
        flag = com.DoIt();
        BeginInvoke(new dgtHandleResult(HandleResult), flag);
    }

    private void HandleResult(bool result)
    {
        if (result)
        {
            if (Text == "读频" || Text == "Read")
            {
                DialogResult = DialogResult.OK;
                MessageBox.Show("读频完成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                progressBar.Text = "Success!";
                progressBar.Value = 100;
                MessageBox.Show("写频完成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        else
        {
            progressBar.Text = "failure!";
        }

        var hIDInterface = hid;
        hIDInterface.DataReceived = (HIDInterface.DelegateDataReceived)Delegate.Remove(hIDInterface.DataReceived,
            new HIDInterface.DelegateDataReceived(com.DataReceived));
        thread_Communication.Abort();
        btn_Start.Enabled = true;
        btn_Cancel.Enabled = true;
    }

    private void UpdateProgress(int value, string content)
    {
        label_ProgressVal.Text = content;
        progressBar.Value = value;
    }

    private void UpdateProgressValue(object sender, ProgressEventArgs e)
    {
        Invoke(new dgtUpdateProgress(UpdateProgress), e.Value, e.Content);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var resources =
            new ComponentResourceManager(typeof(FormProgress));
        btn_Start = new Button();
        btn_Cancel = new Button();
        progressBar = new ProgressBar();
        label_ProgressVal = new Label();
        SuspendLayout();
        btn_Start.Location = new Point(107, 122);
        btn_Start.Name = "btn_Start";
        btn_Start.Size = new Size(95, 31);
        btn_Start.TabIndex = 0;
        btn_Start.Text = "开始";
        btn_Start.UseVisualStyleBackColor = true;
        btn_Start.Click += btn_Start_Click;
        btn_Cancel.Location = new Point(379, 122);
        btn_Cancel.Name = "btn_Cancel";
        btn_Cancel.Size = new Size(95, 31);
        btn_Cancel.TabIndex = 1;
        btn_Cancel.Text = "取消";
        btn_Cancel.UseVisualStyleBackColor = true;
        btn_Cancel.Click += btn_Cancel_Click;
        progressBar.Location = new Point(42, 46);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(503, 38);
        progressBar.TabIndex = 2;
        label_ProgressVal.AutoSize = true;
        label_ProgressVal.Location = new Point(39, 23);
        label_ProgressVal.Name = "label_ProgressVal";
        label_ProgressVal.Size = new Size(0, 15);
        label_ProgressVal.TabIndex = 3;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(594, 180);
        Controls.Add(label_ProgressVal);
        Controls.Add(progressBar);
        Controls.Add(btn_Cancel);
        Controls.Add(btn_Start);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormProgress";
        StartPosition = FormStartPosition.CenterParent;
        Text = "进度";
        ResumeLayout(false);
        PerformLayout();
    }

    private delegate void dgtHandleResult(bool result);

    private delegate void dgtUpdateProgress(int value, string content);
}