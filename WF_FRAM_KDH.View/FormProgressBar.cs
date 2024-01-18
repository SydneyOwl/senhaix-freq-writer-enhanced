using System;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using SQ5R;
using SQ5R.Properties;
using SQ5R.View;

namespace WF_FRAM_KDH.View;

public class FormProgressBar : Form
{
    private readonly IContainer components = null;

    private readonly string language = Settings.Default.language;

    private readonly MySerialPort sP;

    private readonly ClassTheRadioData theRadioData;

    private Label lab_progress;

    private Button pB_btnCancel;

    private Button pB_btnStart;

    public string portName = null;

    private ProgressBar progressBar;

    private Thread threadProgress;

    private Thread threadWF;

    private WriFreq wF;

    public FormProgressBar(ClassTheRadioData theRadioData)
    {
        InitializeComponent();
        StartPosition = FormStartPosition.CenterScreen;
        sP = new MySerialPort();
        this.theRadioData = theRadioData;
        if (language == "英文")
        {
            pB_btnCancel.Text = "Cancel(&C)";
            pB_btnStart.Text = "Start(&S)";
        }
    }

    public static FormProgressBar getInstance(ClassTheRadioData theRadioData)
    {
        return new FormProgressBar(theRadioData);
    }

    private void ConfigComPort(MySerialPort sP)
    {
#if NET461
        if (BleCore.BleInstance().CurrentDevice != null) return;
#endif
        sP.PortName = portName;
        sP.BaudRate = 9600;
        sP.DataBits = 8;
        sP.Parity = Parity.None;
        sP.StopBits = StopBits.One;
        sP.WriteBufferSize = 1024;
        sP.ReadBufferSize = 1024;
        sP.Open();
    }

    private void CloseComPort(MySerialPort sP)
    {
#if NET461
        if (BleCore.BleInstance().CurrentDevice != null) return;
#endif
        if (sP.IsOpen) sP.CloseSerial();
    }

    private void pB_btnStart_Click(object sender, EventArgs e)
    {
        pB_btnStart.Enabled = false;
        progressBar.Value = 0;
        try
        {
            ConfigComPort(sP);
            threadWF = new Thread(Task_WriteFreq);
            threadWF.Start();
            threadProgress = new Thread(Task_GetProgress);
            threadProgress.Start();
        }
        catch
        {
            MessageBox.Show("【E01】请确认写频线或蓝牙是否正确连接! ", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Hand);
            pB_btnStart.Enabled = true;
        }
    }

    private void pB_btnCancel_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void FormProgressBar_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (threadWF != null)
        {
            threadWF.Abort();
            threadProgress.Abort();
            CloseComPort(sP);
        }
    }

    private void Task_WriteFreq()
    {
        var flag = false;
        if (Text == "读频" || Text == "Read")
            wF = new WriFreq(sP, theRadioData, OPERATION_TYPE.READ);
        else
            wF = new WriFreq(sP, theRadioData, OPERATION_TYPE.WRITE);

        flag = wF.DoIt();
        Invoke(new getWriteFreqResult(HandleWFResult), flag);
    }

    private void Task_GetProgress()
    {
        var flag = false;
        var num = 3;
        while (wF == null)
        {
        }

        while (!wF.flagTransmitting)
        {
        }

        while (wF.flagTransmitting)
            switch (wF.state)
            {
                case STATE.HandShakeStep1:
                case STATE.HandShakeStep2:
                case STATE.HandShakeStep3:
                {
                    string text2 = null;
                    text2 = !(language == "中文") ? "hand shake..." : "握手...";
                    flag = false;
                    Invoke(new getWFProgressText(UpdataWFProgressText), text2);
                    Invoke(new getWFProgress(UpdataWFProgress), 0);
                    break;
                }
                case STATE.HandShakeStep4:
                    if (!flag)
                    {
                        string text3 = null;
                        text3 = !(language == "中文") ? "progress..." : "进度（受限于蓝牙功能，如卡在4%请点击取消，重试）...";
                        flag = true;
                        Invoke(new getWFProgress(UpdataWFProgress), num);
                        Invoke(new getWFProgressText(UpdataWFProgressText), text3 + num + "%");
                    }

                    break;
                case STATE.ReadStep1:
                case STATE.ReadStep3:
                case STATE.WriteStep1:
                    flag = false;
                    break;
                case STATE.ReadStep2:
                case STATE.WriteStep2:
                    if (!flag)
                    {
                        string text = null;
                        text = !(language == "中文") ? "progress..." : "进度（受限于蓝牙功能，如卡在4%请点击取消，重试）...";
                        flag = true;
                        if (wF.eepAddr % 64 == 0)
                        {
                            num++;
                            Invoke(new getWFProgress(UpdataWFProgress), num);
                            Invoke(new getWFProgressText(UpdataWFProgressText), text + num + "%");
                        }
                    }

                    break;
            }
    }

    private void HandleWFResult(bool result)
    {
        if (result)
        {
            if (Text == "读频" || Text == "Read")
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                if (language == "中文")
                    lab_progress.Text = "成功!";
                else
                    lab_progress.Text = "Success!";

                progressBar.Value = 100;
            }
        }
        else if (language == "中文")
        {
            lab_progress.Text = "失败!";
        }
        else
        {
            lab_progress.Text = "failure!";
        }

        threadWF.Abort();
        threadProgress.Abort();
        CloseComPort(sP);
        pB_btnStart.Enabled = true;
    }

    private void UpdataWFProgress(int value)
    {
        progressBar.Value = value;
    }

    private void UpdataWFProgressText(string text)
    {
        lab_progress.Text = text;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        progressBar = new ProgressBar();
        pB_btnStart = new Button();
        pB_btnCancel = new Button();
        lab_progress = new Label();
        SuspendLayout();
        progressBar.BackColor = SystemColors.Control;
        progressBar.Location = new Point(29, 38);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(450, 31);
        progressBar.TabIndex = 0;
        pB_btnStart.AutoSize = true;
        pB_btnStart.Location = new Point(81, 91);
        pB_btnStart.Name = "pB_btnStart";
        pB_btnStart.Size = new Size(89, 27);
        pB_btnStart.TabIndex = 1;
        pB_btnStart.Text = "开始";
        pB_btnStart.UseVisualStyleBackColor = true;
        pB_btnStart.Click += pB_btnStart_Click;
        pB_btnCancel.AutoSize = true;
        pB_btnCancel.Location = new Point(315, 91);
        pB_btnCancel.Name = "pB_btnCancel";
        pB_btnCancel.Size = new Size(89, 27);
        pB_btnCancel.TabIndex = 2;
        pB_btnCancel.Text = "取消";
        pB_btnCancel.UseVisualStyleBackColor = true;
        pB_btnCancel.Click += pB_btnCancel_Click;
        lab_progress.AutoSize = true;
        lab_progress.Location = new Point(29, 17);
        lab_progress.Name = "lab_progress";
        lab_progress.Size = new Size(0, 15);
        lab_progress.TabIndex = 3;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(502, 137);
        Controls.Add(lab_progress);
        Controls.Add(pB_btnCancel);
        Controls.Add(pB_btnStart);
        Controls.Add(progressBar);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormProgressBar";
        StartPosition = FormStartPosition.CenterParent;
        Text = "进度";
        FormClosing += FormProgressBar_FormClosing;
        ResumeLayout(false);
        PerformLayout();
    }

    private delegate void getWriteFreqResult(bool result);

    private delegate void getWFProgress(int value);

    private delegate void getWFProgressText(string text);
}