using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using WF_FRAM_KDH;

namespace SQ5R.View;

public class FormOther : Form
{
    private readonly OtherImfData bufData;

    private readonly IContainer components = null;

    private readonly MySerialPort sP = new();
    private Button btn_Close;

    private Button btn_Read;

    private Button btn_Write;

    private CheckBox cb_enableTxOver480M;

    private CheckBox cb_enableTxVHF;

    private ComboBox cbB_RangeOfVHF;

    private GroupBox groupBox1;

    private GroupBox groupBox2;

    private Label label1;

    private Label label10;

    private Label label11;

    private Label label12;

    private Label label2;

    private Label label4;

    private Label label6;

    private Label label7;

    private Label label8;

    private Label label9;

    private string language = "中文";

    private Panel panel1;

    private Panel panel2;

    private Panel panel3;

    private Panel panel4;

    private Panel panel5;

    private Panel panel6;

    private string portName;

    private ProgressBarX progressBar;

    private TextBox tb_MaxFreqOfUHF;

    private TextBox tb_MaxFreqOfVHF;

    private TextBox tb_MinFreqOfUHF;

    private TextBox tb_MinFreqOfVHF;

    private TextBox tb_PowerUpChar1;

    private TextBox tb_PowerUpChar2;

    private Thread threadProgress;

    private Thread threadWF;

    private WriFreq wF;

    public FormOther(OtherImfData data)
    {
        InitializeComponent();
        StartPosition = FormStartPosition.Manual;
        bufData = data;
        dataBings();
    }

    public static FormOther getInstance(Form father, OtherImfData data)
    {
        var formOther = new FormOther(data);
        formOther.MdiParent = father;
        return formOther;
    }

    private void dataBings()
    {
        cbB_RangeOfVHF.DataBindings.Add("SelectedIndex", bufData, "TheRangeOfVHF", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_MinFreqOfVHF.DataBindings.Add("Text", bufData, "TheMinFreqOfVHF", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_MaxFreqOfVHF.DataBindings.Add("Text", bufData, "TheMaxFreqOfVHF", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_MinFreqOfUHF.DataBindings.Add("Text", bufData, "TheMinFreqOfUHF", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_MaxFreqOfUHF.DataBindings.Add("Text", bufData, "TheMaxFreqOfUHF", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_PowerUpChar1.DataBindings.Add("Text", bufData, "PowerUpChar1", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_PowerUpChar2.DataBindings.Add("Text", bufData, "PowerUpChar2", false,
            DataSourceUpdateMode.OnPropertyChanged);
        cb_enableTxVHF.DataBindings.Add("Checked", bufData, "EnableTxVHF", false,
            DataSourceUpdateMode.OnPropertyChanged);
        cb_enableTxOver480M.DataBindings.Add("Checked", bufData, "EnableTxOver480M", false,
            DataSourceUpdateMode.OnPropertyChanged);
    }

    private void upDataBings()
    {
        cbB_RangeOfVHF.DataBindings[0].ReadValue();
        tb_MinFreqOfVHF.DataBindings[0].ReadValue();
        tb_MaxFreqOfVHF.DataBindings[0].ReadValue();
        tb_MinFreqOfUHF.DataBindings[0].ReadValue();
        tb_MaxFreqOfUHF.DataBindings[0].ReadValue();
        tb_PowerUpChar1.DataBindings[0].ReadValue();
        tb_PowerUpChar2.DataBindings[0].ReadValue();
        cb_enableTxOver480M.DataBindings[0].ReadValue();
    }

    private void tb_PowerUpChar_TextChanged(object sender, EventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        if (text.Length != 0)
        {
            var text2 = text;
            foreach (var c in text2)
                if ((c < '0' || c > '9') && (c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '-')
                {
                    var startIndex = text.IndexOf(c);
                    text = text.Remove(startIndex, 1);
                }

            textBox.Text = text;
        }

        textBox.Text = text.ToUpper();
        textBox.Select(text.Length + 1, text.Length + 1);
    }

    public void changeLanguage(string language)
    {
        var visible = Visible;
        Visible = false;
        SuspendLayout();
        this.language = language;
        if (language == "中文")
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
        else
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        var componentResourceManager = new ComponentResourceManager(typeof(FormOther));
        componentResourceManager.ApplyResources(this, "$this");
        foreach (Control control3 in Controls) componentResourceManager.ApplyResources(control3, control3.Name);

        foreach (Control control4 in groupBox1.Controls)
            componentResourceManager.ApplyResources(control4, control4.Name);

        ResumeLayout(false);
        Visible = visible;
    }

    private void FormOther_FormClosing(object sender, FormClosingEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    private void tb_MinFreqOfVHF_TextChanged(object sender, EventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        if (text != null && text.Length != 0 && (text[text.Length - 1] < '0' || text[text.Length - 1] > '9'))
            textBox.Text = text.Remove(text.Length - 1, 1);
    }

    private void tb_MaxFreqOfVHF_TextChanged(object sender, EventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        if (text != null && text.Length != 0 && (text[text.Length - 1] < '0' || text[text.Length - 1] > '9'))
            textBox.Text = text.Remove(text.Length - 1, 1);
    }

    private void tb_MinFreqOfUHF_TextChanged(object sender, EventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        if (text != null && text.Length != 0 && (text[text.Length - 1] < '0' || text[text.Length - 1] > '9'))
            textBox.Text = text.Remove(text.Length - 1, 1);
    }

    private void tb_MaxFreqOfUHF_TextChanged(object sender, EventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        if (text != null && text.Length != 0 && (text[text.Length - 1] < '0' || text[text.Length - 1] > '9'))
            textBox.Text = text.Remove(text.Length - 1, 1);
    }

    private void cbB_RangeOfUHF_SelectedIndexChanged(object sender, EventArgs e)
    {
        var array = new string[2] { "400", "433" };
        var array2 = new string[2] { "520", "449" };
        bufData.TheMinFreqOfUHF = array[cbB_RangeOfVHF.SelectedIndex];
        bufData.TheMaxFreqOfUHF = array2[cbB_RangeOfVHF.SelectedIndex];
    }

    private void cbB_RangeOfVHF_SelectedIndexChanged(object sender, EventArgs e)
    {
        var array = new string[4] { "136", "140", "136", "144" };
        var array2 = new string[4] { "174", "149", "260", "146" };
        var array3 = new string[4] { "400", "430", "400", "430" };
        var array4 = new string[4] { "520", "439", "520", "432" };
        bufData.TheMinFreqOfVHF = array[cbB_RangeOfVHF.SelectedIndex];
        bufData.TheMaxFreqOfVHF = array2[cbB_RangeOfVHF.SelectedIndex];
        bufData.TheMinFreqOfUHF = array3[cbB_RangeOfVHF.SelectedIndex];
        bufData.TheMaxFreqOfUHF = array4[cbB_RangeOfVHF.SelectedIndex];
    }

    private void tb_MinFreqOfVHF_Leave(object sender, EventArgs e)
    {
    }

    private void tb_MaxFreqOfVHF_Leave(object sender, EventArgs e)
    {
    }

    private void tb_MinFreqOfUHF_Leave(object sender, EventArgs e)
    {
    }

    private void tb_MaxFreqOfUHF_Leave(object sender, EventArgs e)
    {
    }

    private void ConfigComPort(MySerialPort sP)
    {
#if NET461
        if (BleCore.BleInstance().CurrentDevice != null) return;
#endif
        sP.PortName = ((FormMain)MdiParent).portName;
        sP.BaudRate = 9600;
        sP.DataBits = 8;
        sP.Parity = Parity.None;
        sP.StopBits = StopBits.One;
        sP.WriteBufferSize = 1024;
        sP.ReadBufferSize = 1024;
        sP.DtrEnable = true;
        sP.RtsEnable = true;
        try
        {
            sP.Open();
        }
        catch
        {
            MessageBox.Show("请检查蓝牙或写频线是否正确连接! ", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Hand);
        }
    }

    private void CloseComPort(MySerialPort sP)
    {
        if (sP.IsOpen) sP.CloseSerial();
    }

    private void btn_Read_Click(object sender, EventArgs e)
    {
        btn_Read.Enabled = false;
        btn_Write.Enabled = false;
        btn_Close.Enabled = false;
        progressBar.Value = 0;
        ConfigComPort(sP);
        threadWF = new Thread(Task_ReadConfig);
        threadWF.Start();
        threadProgress = new Thread(Task_GetProgress);
        threadProgress.Start();
    }

    private void btn_Write_Click(object sender, EventArgs e)
    {
        btn_Read.Enabled = false;
        btn_Write.Enabled = false;
        btn_Close.Enabled = false;
        progressBar.Value = 0;
        try
        {
            ConfigComPort(sP);
            threadWF = new Thread(Task_WriteConfig);
            threadWF.Start();
            threadProgress = new Thread(Task_GetProgress);
            threadProgress.Start();
        }
        catch
        {
            MessageBox.Show("请检查蓝牙或写频线是否正确连接 ", "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Hand);
            btn_Read.Enabled = true;
            btn_Write.Enabled = true;
            btn_Close.Enabled = true;
        }
    }

    private void btn_Close_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void Task_ReadConfig()
    {
        var flag = false;
        wF = new WriFreq(sP, bufData, OPERATION_TYPE.READ_CONFIG);
        flag = wF.DoIt();
        Invoke(new getWriteFreqResult(HandleWFResult), flag);
    }

    private void Task_WriteConfig()
    {
        var flag = false;
        wF = new WriFreq(sP, bufData, OPERATION_TYPE.WRITE_CONFIG);
        flag = wF.DoIt();
        Invoke(new getWriteFreqResult(HandleWFResult), flag);
    }

    private void Task_GetProgress()
    {
        var flag = false;
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
                    string text3 = null;
                    text3 = !(language == "中文") ? "hand shake..." : "握手...";
                    flag = false;
                    Invoke(new getWFProgressText(UpdataWFProgressText), text3);
                    Invoke(new getWFProgress(UpdataWFProgress), 0);
                    break;
                }
                case STATE.HandShakeStep4:
                    if (!flag)
                    {
                        string text2 = null;
                        text2 = !(language == "中文") ? "progress..." : "进度（受限于蓝牙功能，如卡在4%请点击取消，重试）...";
                        flag = true;
                        Invoke(new getWFProgress(UpdataWFProgress), 20);
                        Invoke(new getWFProgressText(UpdataWFProgressText), text2 + 20 + "%");
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
                        if (wF.eepAddr == 8128)
                        {
                            Invoke(new getWFProgress(UpdataWFProgress), 50);
                            Invoke(new getWFProgressText(UpdataWFProgressText), text + 50 + "%");
                        }
                        else if (wF.eepAddr == 7872 || wF.eepAddr == 7904)
                        {
                            Invoke(new getWFProgress(UpdataWFProgress), 100);
                            Invoke(new getWFProgressText(UpdataWFProgressText), text + 100 + "%");
                        }
                    }

                    break;
            }
    }

    private void HandleWFResult(bool result)
    {
        if (result)
        {
            if (language == "中文")
                progressBar.Text = "成功!";
            else
                progressBar.Text = "Success!";

            upDataBings();
        }
        else if (language == "中文")
        {
            progressBar.Text = "失败!";
        }
        else
        {
            progressBar.Text = "failure!";
        }

        threadWF.Abort();
        threadProgress.Abort();
        CloseComPort(sP);
        btn_Read.Enabled = true;
        btn_Write.Enabled = true;
        btn_Close.Enabled = true;
    }

    private void UpdataWFProgress(int value)
    {
        progressBar.Value = value;
    }

    private void UpdataWFProgressText(string text)
    {
        progressBar.Text = text;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var resources =
            new ComponentResourceManager(typeof(FormOther));
        groupBox1 = new GroupBox();
        cbB_RangeOfVHF = new ComboBox();
        label12 = new Label();
        cb_enableTxVHF = new CheckBox();
        label6 = new Label();
        cb_enableTxOver480M = new CheckBox();
        panel6 = new Panel();
        tb_MaxFreqOfUHF = new TextBox();
        panel5 = new Panel();
        tb_MinFreqOfUHF = new TextBox();
        panel4 = new Panel();
        tb_MaxFreqOfVHF = new TextBox();
        panel3 = new Panel();
        tb_MinFreqOfVHF = new TextBox();
        label11 = new Label();
        label10 = new Label();
        label9 = new Label();
        label8 = new Label();
        label7 = new Label();
        label4 = new Label();
        label2 = new Label();
        label1 = new Label();
        groupBox2 = new GroupBox();
        panel2 = new Panel();
        tb_PowerUpChar2 = new TextBox();
        panel1 = new Panel();
        tb_PowerUpChar1 = new TextBox();
        btn_Read = new Button();
        btn_Write = new Button();
        btn_Close = new Button();
        progressBar = new ProgressBarX();
        groupBox1.SuspendLayout();
        panel6.SuspendLayout();
        panel5.SuspendLayout();
        panel4.SuspendLayout();
        panel3.SuspendLayout();
        groupBox2.SuspendLayout();
        panel2.SuspendLayout();
        panel1.SuspendLayout();
        SuspendLayout();
        groupBox1.Controls.Add(cbB_RangeOfVHF);
        groupBox1.Controls.Add(label12);
        groupBox1.Controls.Add(cb_enableTxVHF);
        groupBox1.Controls.Add(label6);
        groupBox1.Controls.Add(cb_enableTxOver480M);
        groupBox1.Controls.Add(panel6);
        groupBox1.Controls.Add(panel5);
        groupBox1.Controls.Add(panel4);
        groupBox1.Controls.Add(panel3);
        groupBox1.Controls.Add(label11);
        groupBox1.Controls.Add(label10);
        groupBox1.Controls.Add(label9);
        groupBox1.Controls.Add(label8);
        groupBox1.Controls.Add(label7);
        groupBox1.Controls.Add(label4);
        groupBox1.Controls.Add(label2);
        groupBox1.Controls.Add(label1);
        resources.ApplyResources(groupBox1, "groupBox1");
        groupBox1.Name = "groupBox1";
        groupBox1.TabStop = false;
        cbB_RangeOfVHF.BackColor = SystemColors.Control;
        cbB_RangeOfVHF.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_RangeOfVHF.FormattingEnabled = true;
        cbB_RangeOfVHF.Items.AddRange(new object[4]
        {
            resources.GetString("cbB_RangeOfVHF.Items"),
            resources.GetString("cbB_RangeOfVHF.Items1"),
            resources.GetString("cbB_RangeOfVHF.Items2"),
            resources.GetString("cbB_RangeOfVHF.Items3")
        });
        resources.ApplyResources(cbB_RangeOfVHF, "cbB_RangeOfVHF");
        cbB_RangeOfVHF.Name = "cbB_RangeOfVHF";
        cbB_RangeOfVHF.SelectedIndexChanged += cbB_RangeOfVHF_SelectedIndexChanged;
        resources.ApplyResources(label12, "label12");
        label12.Name = "label12";
        resources.ApplyResources(cb_enableTxVHF, "cb_enableTxVHF");
        cb_enableTxVHF.Checked = true;
        cb_enableTxVHF.CheckState = CheckState.Checked;
        cb_enableTxVHF.Name = "cb_enableTxVHF";
        cb_enableTxVHF.UseVisualStyleBackColor = true;
        resources.ApplyResources(label6, "label6");
        label6.Name = "label6";
        resources.ApplyResources(cb_enableTxOver480M, "cb_enableTxOver480M");
        cb_enableTxOver480M.Checked = true;
        cb_enableTxOver480M.CheckState = CheckState.Checked;
        cb_enableTxOver480M.Name = "cb_enableTxOver480M";
        cb_enableTxOver480M.UseVisualStyleBackColor = true;
        panel6.BackColor = SystemColors.Window;
        panel6.BorderStyle = BorderStyle.Fixed3D;
        panel6.Controls.Add(tb_MaxFreqOfUHF);
        resources.ApplyResources(panel6, "panel6");
        panel6.Name = "panel6";
        tb_MaxFreqOfUHF.BackColor = SystemColors.Window;
        tb_MaxFreqOfUHF.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_MaxFreqOfUHF, "tb_MaxFreqOfUHF");
        tb_MaxFreqOfUHF.Name = "tb_MaxFreqOfUHF";
        tb_MaxFreqOfUHF.TextChanged += tb_MaxFreqOfUHF_TextChanged;
        tb_MaxFreqOfUHF.Leave += tb_MaxFreqOfUHF_Leave;
        panel5.BackColor = SystemColors.Window;
        panel5.BorderStyle = BorderStyle.Fixed3D;
        panel5.Controls.Add(tb_MinFreqOfUHF);
        resources.ApplyResources(panel5, "panel5");
        panel5.Name = "panel5";
        tb_MinFreqOfUHF.BackColor = SystemColors.Window;
        tb_MinFreqOfUHF.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_MinFreqOfUHF, "tb_MinFreqOfUHF");
        tb_MinFreqOfUHF.Name = "tb_MinFreqOfUHF";
        tb_MinFreqOfUHF.TextChanged += tb_MinFreqOfUHF_TextChanged;
        tb_MinFreqOfUHF.Leave += tb_MinFreqOfUHF_Leave;
        panel4.BackColor = SystemColors.Window;
        panel4.BorderStyle = BorderStyle.Fixed3D;
        panel4.Controls.Add(tb_MaxFreqOfVHF);
        resources.ApplyResources(panel4, "panel4");
        panel4.Name = "panel4";
        tb_MaxFreqOfVHF.BackColor = SystemColors.Window;
        tb_MaxFreqOfVHF.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_MaxFreqOfVHF, "tb_MaxFreqOfVHF");
        tb_MaxFreqOfVHF.Name = "tb_MaxFreqOfVHF";
        tb_MaxFreqOfVHF.TextChanged += tb_MaxFreqOfVHF_TextChanged;
        tb_MaxFreqOfVHF.Leave += tb_MaxFreqOfVHF_Leave;
        panel3.BackColor = SystemColors.Window;
        panel3.BorderStyle = BorderStyle.Fixed3D;
        panel3.Controls.Add(tb_MinFreqOfVHF);
        resources.ApplyResources(panel3, "panel3");
        panel3.Name = "panel3";
        tb_MinFreqOfVHF.BackColor = SystemColors.Window;
        tb_MinFreqOfVHF.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_MinFreqOfVHF, "tb_MinFreqOfVHF");
        tb_MinFreqOfVHF.Name = "tb_MinFreqOfVHF";
        tb_MinFreqOfVHF.TextChanged += tb_MinFreqOfVHF_TextChanged;
        tb_MinFreqOfVHF.Leave += tb_MinFreqOfVHF_Leave;
        resources.ApplyResources(label11, "label11");
        label11.Name = "label11";
        resources.ApplyResources(label10, "label10");
        label10.Name = "label10";
        resources.ApplyResources(label9, "label9");
        label9.Name = "label9";
        resources.ApplyResources(label8, "label8");
        label8.Name = "label8";
        resources.ApplyResources(label7, "label7");
        label7.Name = "label7";
        resources.ApplyResources(label4, "label4");
        label4.Name = "label4";
        resources.ApplyResources(label2, "label2");
        label2.Name = "label2";
        resources.ApplyResources(label1, "label1");
        label1.Name = "label1";
        groupBox2.Controls.Add(panel2);
        groupBox2.Controls.Add(panel1);
        resources.ApplyResources(groupBox2, "groupBox2");
        groupBox2.Name = "groupBox2";
        groupBox2.TabStop = false;
        panel2.BackColor = SystemColors.Window;
        panel2.BorderStyle = BorderStyle.Fixed3D;
        panel2.Controls.Add(tb_PowerUpChar2);
        resources.ApplyResources(panel2, "panel2");
        panel2.Name = "panel2";
        tb_PowerUpChar2.BackColor = SystemColors.Window;
        tb_PowerUpChar2.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_PowerUpChar2, "tb_PowerUpChar2");
        tb_PowerUpChar2.Name = "tb_PowerUpChar2";
        tb_PowerUpChar2.TextChanged += tb_PowerUpChar_TextChanged;
        panel1.BackColor = SystemColors.Window;
        panel1.BorderStyle = BorderStyle.Fixed3D;
        panel1.Controls.Add(tb_PowerUpChar1);
        resources.ApplyResources(panel1, "panel1");
        panel1.Name = "panel1";
        tb_PowerUpChar1.BackColor = SystemColors.Window;
        tb_PowerUpChar1.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_PowerUpChar1, "tb_PowerUpChar1");
        tb_PowerUpChar1.Name = "tb_PowerUpChar1";
        tb_PowerUpChar1.TextChanged += tb_PowerUpChar_TextChanged;
        resources.ApplyResources(btn_Read, "btn_Read");
        btn_Read.Name = "btn_Read";
        btn_Read.UseVisualStyleBackColor = true;
        btn_Read.Click += btn_Read_Click;
        resources.ApplyResources(btn_Write, "btn_Write");
        btn_Write.Name = "btn_Write";
        btn_Write.UseVisualStyleBackColor = true;
        btn_Write.Click += btn_Write_Click;
        resources.ApplyResources(btn_Close, "btn_Close");
        btn_Close.Name = "btn_Close";
        btn_Close.UseVisualStyleBackColor = true;
        btn_Close.Click += btn_Close_Click;
        progressBar.BackgroundStyle.CornerType = eCornerType.Square;
        progressBar.ChunkColor = Color.DeepSkyBlue;
        resources.ApplyResources(progressBar, "progressBar");
        progressBar.Name = "progressBar";
        progressBar.Style = eDotNetBarStyle.Office2000;
        progressBar.TextVisible = true;
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(progressBar);
        Controls.Add(btn_Close);
        Controls.Add(btn_Write);
        Controls.Add(btn_Read);
        Controls.Add(groupBox1);
        Controls.Add(groupBox2);
        Name = "FormOther";
        FormClosing += FormOther_FormClosing;
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        panel6.ResumeLayout(false);
        panel6.PerformLayout();
        panel5.ResumeLayout(false);
        panel5.PerformLayout();
        panel4.ResumeLayout(false);
        panel4.PerformLayout();
        panel3.ResumeLayout(false);
        panel3.PerformLayout();
        groupBox2.ResumeLayout(false);
        panel2.ResumeLayout(false);
        panel2.PerformLayout();
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        ResumeLayout(false);
    }

    private delegate void getWriteFreqResult(bool result);

    private delegate void getWFProgress(int value);

    private delegate void getWFProgressText(string text);
}