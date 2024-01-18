using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using SQ5R.Properties;
using SQ5R.View;
using WF_FRAM_KDH.View;
using Timer = System.Windows.Forms.Timer;

namespace SQ5R;

public class FormMain : Form
{
#if NET461
    private BleCore bleCore = BleCore.BleInstance();
#endif

    private IContainer components;

    private string filePath;
    private FormCHImfo instanceFormCHImfo;

    private FormDTMF instanceFormDTMF;

    private FormFunConfig instanceFormFunCFG;

    private FormOther instanceFormOther;

    private Label label_Band;

    private Label label_Date;

    private Label label_staPort;

    private Label label_UHF;

    private Label label_VHF;

    private string language = "中文";

    private MenuStrip menuStrip;

    private ToolStripMenuItem mS_Edit;

    private ToolStripMenuItem mS_File;

    private ToolStripMenuItem mS_Help;

    private ToolStripMenuItem mS_Program;

    private ToolStripMenuItem mS_Setting;

    private ToolStripMenuItem mSE_CHImfo;

    private ToolStripMenuItem mSE_DTMF;

    private ToolStripMenuItem mSE_FunSetting;

    private ToolStripMenuItem mSE_Other;

    private ToolStripMenuItem mSF_Exit;

    private ToolStripMenuItem mSF_New;

    private ToolStripMenuItem mSF_Open;

    private ToolStripMenuItem mSF_Save;

    private ToolStripMenuItem mSF_SaveAs;

    private ToolStripMenuItem mSH_About;

    private ToolStripMenuItem mSP_Read;

    private ToolStripMenuItem mSP_Write;

    private ToolStripMenuItem mSS_Bluetooth;

    private ToolStripMenuItem mSS_Language;

    private ToolStripMenuItem mSS_Port;

    private ToolStripMenuItem mSSL_Chinese;

    private ToolStripMenuItem mSSL_English;

    private Panel panel1;

    private Panel panel2;

    private Panel panel3;

    private Panel panel4;

    private Panel panel5;

    private Panel panel6;

    private Panel panel7;

    public string portName;

    public ClassTheRadioData theRadioData = new();

    private Timer timer1;

    private ToolStrip toolStrip;

    private ToolStripButton tS_New;

    private ToolStripButton tS_Open;

    private ToolStripButton tS_Read;

    private ToolStripButton tS_Save;

    private ToolStripButton tS_Write;

    public FormMain()
    {
        InitializeComponent();
        if (language == "英文")
        {
            mSSL_Chinese.Checked = false;
            mSSL_English.Checked = true;
        }
        else
        {
            mSSL_Chinese.Checked = true;
            mSSL_English.Checked = false;
        }

        var imageScalingSize = new Size(32, 32);
        toolStrip.ImageScalingSize = imageScalingSize;
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
        label_Date.Text = DateTime.Now.ToString();
        instanceFormCHImfo = FormCHImfo.getInstance(this, theRadioData.ChannelData);
        instanceFormCHImfo.Show();
        instanceFormFunCFG = FormFunConfig.getInstance(this, theRadioData.funCfgData);
        instanceFormFunCFG.ChangeCHCounts = instanceFormCHImfo.changeTheCountsOfCH;
        instanceFormDTMF = FormDTMF.getInstance(this, theRadioData.dtmfData);
        instanceFormOther = FormOther.getInstance(this, theRadioData.otherImfData);
        language = Settings.Default.language;
        changeLanguage(language);
        if (language == "英文")
        {
            mSSL_Chinese.Checked = false;
            mSSL_English.Checked = true;
        }

        var portNames = SerialPort.GetPortNames();
        if (portNames.Length != 0)
        {
            label_staPort.Text = portNames[portNames.Length - 1];
            portName = portNames[portNames.Length - 1];
        }
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        instanceFormCHImfo.Close();
        if (instanceFormFunCFG != null) instanceFormFunCFG.Close();

        if (instanceFormDTMF != null) instanceFormDTMF.Close();

        if (instanceFormOther != null) instanceFormOther.Close();

        e.Cancel = false;
    }

    private void mSF_New_Click(object sender, EventArgs e)
    {
        instanceFormCHImfo.btn_Clear.PerformClick();
    }

    private void mSF_Open_Click(object sender, EventArgs e)
    {
        var openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "dat文件(*.dat)|*.dat";
        openFileDialog.ValidateNames = true;
        openFileDialog.CheckFileExists = true;
        openFileDialog.CheckPathExists = true;
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            var fileName = openFileDialog.FileName;
            filePath = fileName;
            Stream s = new FileStream(filePath, FileMode.Open);
            theRadioData = ClassTheRadioData.CreatObjFromFile(s);
            instanceFormCHImfo.upDataChData(theRadioData.ChannelData);
            instanceFormFunCFG.updataTheBingdingValue(theRadioData.funCfgData);
            instanceFormDTMF.updataBindingDatas(theRadioData.dtmfData);
        }
    }

    private void mSF_Save_Click(object sender, EventArgs e)
    {
        if (filePath != null)
        {
            Stream stream = new FileStream(filePath, FileMode.OpenOrCreate);
            stream.Seek(0L, SeekOrigin.Begin);
            stream.SetLength(0L);
            theRadioData.SaveToFile(stream);
            stream.Close();
        }
        else
        {
            mSF_SaveAs.PerformClick();
        }
    }

    private void mSF_SaveAs_Click(object sender, EventArgs e)
    {
        var saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "dat文件(*.dat)|*.dat";
        saveFileDialog.ValidateNames = true;
        saveFileDialog.AddExtension = true;
        saveFileDialog.RestoreDirectory = true;
        saveFileDialog.CheckPathExists = true;
        Stream stream;
        if (saveFileDialog.ShowDialog() == DialogResult.OK && (stream = saveFileDialog.OpenFile()) != null)
        {
            theRadioData.SaveToFile(stream);
            stream.Close();
            filePath = saveFileDialog.FileName;
        }
    }

    private void mSF_Exit_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void mSE_CHImfo_Click(object sender, EventArgs e)
    {
        instanceFormCHImfo.Show();
    }

    private void mSE_FunSetting_Click(object sender, EventArgs e)
    {
        instanceFormFunCFG.Show();
    }

    private void mSE_DTMF_Click(object sender, EventArgs e)
    {
        instanceFormDTMF.Show();
    }

    private void mSE_Other_Click(object sender, EventArgs e)
    {
        instanceFormOther.Show();
    }

    private void mSP_Read_Click(object sender, EventArgs e)
    {
        var breaker = false;
#if NET461
        if (bleCore.CurrentDevice != null) breaker = true;
#endif
        if (!string.IsNullOrEmpty(portName) || breaker)
        {
            var instance = FormProgressBar.getInstance(theRadioData);
            if (language == "中文")
                instance.Text = "读频";
            else
                instance.Text = "Read";
#if NET461
            if (bleCore.CurrentDevice == null)
            {
#endif
            if (string.IsNullOrEmpty(portName))
            {
                MessageBox.Show("请选择端口号！");
                return;
            }

            instance.portName = portName;
#if NET461
            }
#endif
            if (DialogResult.OK != instance.ShowDialog()) return;

            for (var i = 0; i < int.Parse(theRadioData.funCfgData.TB_CountsOfCH); i++)
                if (theRadioData.ChannelData[i][2] == null)
                {
                    for (var j = 0; j < 13; j++) theRadioData.ChannelData[i][j] = null;

                    theRadioData.ChannelData[i][0] = i.ToString();
                }

            instanceFormCHImfo.upDataChData(theRadioData.ChannelData);
            instanceFormFunCFG.updataTheBingdingValue(theRadioData.funCfgData);
            instanceFormDTMF.updataBindingDatas(theRadioData.dtmfData);
        }
        else if (language == "中文")
        {
            MessageBox.Show("请接入写频线或连接蓝牙!");
        }
        else
        {
            MessageBox.Show("请接入写频线或连接蓝牙!");
        }
    }

    private void mSP_Write_Click(object sender, EventArgs e)
    {
        var breaker = false;
#if NET461
        if (bleCore.CurrentDevice != null) breaker = true;
#endif
        if (!string.IsNullOrEmpty(portName) || breaker)
        {
            var instance = FormProgressBar.getInstance(theRadioData);
            if (language == "中文")
                instance.Text = "写频";
            else
                instance.Text = "Write";

#if NET461
            if (bleCore.CurrentDevice == null)
            {
#endif
            if (string.IsNullOrEmpty(portName))
            {
                MessageBox.Show("请选择端口号！");
                return;
            }

            instance.portName = portName;
#if NET461
            }
#endif
            instance.ShowDialog();
        }
        else if (language == "中文")
        {
            MessageBox.Show("请接入写频线或连接蓝牙!");
        }
        else
        {
            MessageBox.Show("请接入写频线或连接蓝牙!");
        }
    }

    private void mSS_Port_Click(object sender, EventArgs e)
    {
        var formSerialPort = new FormSerialPort(language);
        formSerialPort.portName = portName;
        if (DialogResult.OK == formSerialPort.ShowDialog())
        {
            portName = formSerialPort.portName;
            label_staPort.Text = portName;
        }
    }
#if NET461
    private void mSS_bt_Click(object sender, EventArgs e)
    {
        var fb = new FormConnBluetooth();
        if (DialogResult.OK == fb.ShowDialog()) bleCore = fb.bleCore;
    }
#endif
    private void mSSL_Chinese_Click(object sender, EventArgs e)
    {
        if (mSSL_English.Checked)
        {
            mSSL_Chinese.Checked = true;
            mSSL_English.Checked = false;
            language = "中文";
            changeLanguage(language);
            Settings.Default.language = language;
            Settings.Default.Save();
        }
    }

    private void mSSL_English_Click(object sender, EventArgs e)
    {
        if (!mSSL_English.Checked)
        {
            mSSL_Chinese.Checked = false;
            mSSL_English.Checked = true;
            language = "英文";
            changeLanguage(language);
            Settings.Default.language = language;
            Settings.Default.Save();
        }
    }

    private void mSH_About_Click(object sender, EventArgs e)
    {
        var formAbout = new FormAbout();
        formAbout.ShowDialog();
    }

    public void changeLanguage(string language)
    {
        if (language == "中文")
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
        else
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

        var componentResourceManager = new ComponentResourceManager(typeof(FormMain));
        foreach (ToolStripMenuItem item in menuStrip.Items) componentResourceManager.ApplyResources(item, item.Name);

        foreach (ToolStripMenuItem dropDownItem in mS_File.DropDownItems)
            componentResourceManager.ApplyResources(dropDownItem, dropDownItem.Name);

        foreach (ToolStripMenuItem dropDownItem2 in mS_Edit.DropDownItems)
            componentResourceManager.ApplyResources(dropDownItem2, dropDownItem2.Name);

        foreach (ToolStripMenuItem dropDownItem3 in mS_Program.DropDownItems)
            componentResourceManager.ApplyResources(dropDownItem3, dropDownItem3.Name);

        foreach (ToolStripMenuItem dropDownItem4 in mS_Setting.DropDownItems)
            componentResourceManager.ApplyResources(dropDownItem4, dropDownItem4.Name);

        foreach (ToolStripMenuItem dropDownItem5 in mSS_Language.DropDownItems)
            componentResourceManager.ApplyResources(dropDownItem5, dropDownItem5.Name);

        foreach (ToolStripMenuItem dropDownItem6 in mS_Help.DropDownItems)
            componentResourceManager.ApplyResources(dropDownItem6, dropDownItem6.Name);

        foreach (ToolStripButton item2 in toolStrip.Items) componentResourceManager.ApplyResources(item2, item2.Name);

        if (language == "中文")
        {
            tS_New.Image = Resources._new;
            tS_Open.Image = Resources.Open;
            tS_Save.Image = Resources.Save;
            tS_Read.Image = Resources.Read;
            tS_Write.Image = Resources.Write;
        }
        else
        {
            tS_New.Image = Resources.New_Eng;
            tS_Open.Image = Resources.Open_Eng;
            tS_Save.Image = Resources.Save_Eng;
            tS_Read.Image = Resources.Read_Eng;
            tS_Write.Image = Resources.Write_Eng;
        }

        componentResourceManager.ApplyResources(label_Band, label_Band.Name);
        componentResourceManager.ApplyResources(label_staPort, label_staPort.Name);
        instanceFormCHImfo.changeLanguage(language);
        instanceFormFunCFG.changeLanguage(language);
        instanceFormDTMF.changeLanguage(language);
        instanceFormOther.changeLanguage(language);
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        label_Date.Text = DateTime.Now.ToString();
    }

    private void menuStrip_Click(object sender, EventArgs e)
    {
        if (instanceFormCHImfo.dGV.CurrentCell != null) instanceFormCHImfo.dGV.CurrentCell = null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new Container();
        var resources =
            new ComponentResourceManager(typeof(FormMain));
        menuStrip = new MenuStrip();
        mS_File = new ToolStripMenuItem();
        mSF_New = new ToolStripMenuItem();
        mSF_Open = new ToolStripMenuItem();
        mSF_Save = new ToolStripMenuItem();
        mSF_SaveAs = new ToolStripMenuItem();
        mSF_Exit = new ToolStripMenuItem();
        mS_Edit = new ToolStripMenuItem();
        mSE_CHImfo = new ToolStripMenuItem();
        mSE_FunSetting = new ToolStripMenuItem();
        mSE_DTMF = new ToolStripMenuItem();
        mSE_Other = new ToolStripMenuItem();
        mS_Program = new ToolStripMenuItem();
        mSP_Read = new ToolStripMenuItem();
        mSP_Write = new ToolStripMenuItem();
        mS_Setting = new ToolStripMenuItem();
        mSS_Port = new ToolStripMenuItem();
        mSS_Bluetooth = new ToolStripMenuItem();
        mSS_Language = new ToolStripMenuItem();
        mSSL_Chinese = new ToolStripMenuItem();
        mSSL_English = new ToolStripMenuItem();
        mS_Help = new ToolStripMenuItem();
        mSH_About = new ToolStripMenuItem();
        toolStrip = new ToolStrip();
        tS_New = new ToolStripButton();
        tS_Save = new ToolStripButton();
        tS_Open = new ToolStripButton();
        tS_Read = new ToolStripButton();
        tS_Write = new ToolStripButton();
        panel1 = new Panel();
        panel7 = new Panel();
        panel6 = new Panel();
        label_Date = new Label();
        panel5 = new Panel();
        label_VHF = new Label();
        panel4 = new Panel();
        label_UHF = new Label();
        panel2 = new Panel();
        label_staPort = new Label();
        panel3 = new Panel();
        label_Band = new Label();
        timer1 = new Timer(components);
        menuStrip.SuspendLayout();
        toolStrip.SuspendLayout();
        panel1.SuspendLayout();
        panel6.SuspendLayout();
        panel5.SuspendLayout();
        panel4.SuspendLayout();
        panel2.SuspendLayout();
        panel3.SuspendLayout();
        SuspendLayout();
        menuStrip.ImageScalingSize = new Size(20, 20);
        menuStrip.Items.AddRange(new ToolStripItem[5]
            { mS_File, mS_Edit, mS_Program, mS_Setting, mS_Help });
        resources.ApplyResources(menuStrip, "menuStrip");
        menuStrip.Name = "menuStrip";
        menuStrip.Click += menuStrip_Click;
        mS_File.DropDownItems.AddRange(new ToolStripItem[5]
            { mSF_New, mSF_Open, mSF_Save, mSF_SaveAs, mSF_Exit });
        mS_File.Name = "mS_File";
        resources.ApplyResources(mS_File, "mS_File");
        mSF_New.Name = "mSF_New";
        resources.ApplyResources(mSF_New, "mSF_New");
        mSF_New.Click += mSF_New_Click;
        mSF_Open.Name = "mSF_Open";
        resources.ApplyResources(mSF_Open, "mSF_Open");
        mSF_Open.Click += mSF_Open_Click;
        mSF_Save.Name = "mSF_Save";
        resources.ApplyResources(mSF_Save, "mSF_Save");
        mSF_Save.Click += mSF_Save_Click;
        mSF_SaveAs.Name = "mSF_SaveAs";
        resources.ApplyResources(mSF_SaveAs, "mSF_SaveAs");
        mSF_SaveAs.Click += mSF_SaveAs_Click;
        mSF_Exit.Name = "mSF_Exit";
        resources.ApplyResources(mSF_Exit, "mSF_Exit");
        mSF_Exit.Click += mSF_Exit_Click;
        mS_Edit.DropDownItems.AddRange(new ToolStripItem[4]
            { mSE_CHImfo, mSE_FunSetting, mSE_DTMF, mSE_Other });
        mS_Edit.Name = "mS_Edit";
        resources.ApplyResources(mS_Edit, "mS_Edit");
        mSE_CHImfo.Name = "mSE_CHImfo";
        resources.ApplyResources(mSE_CHImfo, "mSE_CHImfo");
        mSE_CHImfo.Click += mSE_CHImfo_Click;
        mSE_FunSetting.Name = "mSE_FunSetting";
        resources.ApplyResources(mSE_FunSetting, "mSE_FunSetting");
        mSE_FunSetting.Click += mSE_FunSetting_Click;
        mSE_DTMF.Name = "mSE_DTMF";
        resources.ApplyResources(mSE_DTMF, "mSE_DTMF");
        mSE_DTMF.Click += mSE_DTMF_Click;
        mSE_Other.Name = "mSE_Other";
        resources.ApplyResources(mSE_Other, "mSE_Other");
        mSE_Other.Click += mSE_Other_Click;
        mS_Program.DropDownItems.AddRange(new ToolStripItem[2]
            { mSP_Read, mSP_Write });
        mS_Program.Name = "mS_Program";
        resources.ApplyResources(mS_Program, "mS_Program");
        mSP_Read.Name = "mSP_Read";
        resources.ApplyResources(mSP_Read, "mSP_Read");
        mSP_Read.Click += mSP_Read_Click;
        mSP_Write.Name = "mSP_Write";
        resources.ApplyResources(mSP_Write, "mSP_Write");
        mSP_Write.Click += mSP_Write_Click;

        // we add new items here!

        mS_Setting.Name = "mS_Setting";
        resources.ApplyResources(mS_Setting, "mS_Setting");
        mSS_Port.Name = "mSS_Port";
        resources.ApplyResources(mSS_Port, "mSS_Port");
        mSS_Port.Click += mSS_Port_Click;
        mSS_Bluetooth.Text = "（该版本无蓝牙）";
#if NET461
        mSS_Bluetooth.Click += mSS_bt_Click;
        mSS_Bluetooth.Text = "蓝牙";
#endif
        mS_Setting.DropDownItems.AddRange(new ToolStripItem[2]
            { mSS_Port, mSS_Bluetooth });


        mSS_Language.DropDownItems.AddRange(new ToolStripItem[2]
            { mSSL_Chinese, mSSL_English });
        mSS_Language.Name = "mSS_Language";
        resources.ApplyResources(mSS_Language, "mSS_Language");
        mSSL_Chinese.Name = "mSSL_Chinese";
        resources.ApplyResources(mSSL_Chinese, "mSSL_Chinese");
        mSSL_Chinese.Click += mSSL_Chinese_Click;
        mSSL_English.Checked = true;
        mSSL_English.CheckState = CheckState.Checked;
        mSSL_English.Name = "mSSL_English";
        resources.ApplyResources(mSSL_English, "mSSL_English");
        mSSL_English.Click += mSSL_English_Click;
        mS_Help.DropDownItems.AddRange(new ToolStripItem[1] { mSH_About });
        mS_Help.Name = "mS_Help";
        resources.ApplyResources(mS_Help, "mS_Help");
        mSH_About.Name = "mSH_About";
        resources.ApplyResources(mSH_About, "mSH_About");
        mSH_About.Click += mSH_About_Click;
        toolStrip.ImageScalingSize = new Size(20, 20);
        toolStrip.Items.AddRange(new ToolStripItem[5]
            { tS_New, tS_Save, tS_Open, tS_Read, tS_Write });
        resources.ApplyResources(toolStrip, "toolStrip");
        toolStrip.Name = "toolStrip";
        toolStrip.Click += menuStrip_Click;
        tS_New.DisplayStyle = ToolStripItemDisplayStyle.Image;
        tS_New.Image = Resources._new;
        resources.ApplyResources(tS_New, "tS_New");
        tS_New.Name = "tS_New";
        tS_New.Click += mSF_New_Click;
        tS_Save.DisplayStyle = ToolStripItemDisplayStyle.Image;
        tS_Save.Image = Resources.Save;
        resources.ApplyResources(tS_Save, "tS_Save");
        tS_Save.Name = "tS_Save";
        tS_Save.Click += mSF_Save_Click;
        tS_Open.DisplayStyle = ToolStripItemDisplayStyle.Image;
        tS_Open.Image = Resources.Open;
        resources.ApplyResources(tS_Open, "tS_Open");
        tS_Open.Name = "tS_Open";
        tS_Open.Click += mSF_Open_Click;
        tS_Read.DisplayStyle = ToolStripItemDisplayStyle.Image;
        tS_Read.Image = Resources.Read;
        resources.ApplyResources(tS_Read, "tS_Read");
        tS_Read.Name = "tS_Read";
        tS_Read.Click += mSP_Read_Click;
        tS_Write.DisplayStyle = ToolStripItemDisplayStyle.Image;
        tS_Write.Image = Resources.Write;
        resources.ApplyResources(tS_Write, "tS_Write");
        tS_Write.Name = "tS_Write";
        tS_Write.Click += mSP_Write_Click;
        panel1.Controls.Add(panel7);
        panel1.Controls.Add(panel6);
        panel1.Controls.Add(panel5);
        panel1.Controls.Add(panel4);
        panel1.Controls.Add(panel2);
        panel1.Controls.Add(panel3);
        resources.ApplyResources(panel1, "panel1");
        panel1.Name = "panel1";
        resources.ApplyResources(panel7, "panel7");
        panel7.BorderStyle = BorderStyle.Fixed3D;
        panel7.Name = "panel7";
        resources.ApplyResources(panel6, "panel6");
        panel6.BorderStyle = BorderStyle.Fixed3D;
        panel6.Controls.Add(label_Date);
        panel6.Name = "panel6";
        resources.ApplyResources(label_Date, "label_Date");
        label_Date.Name = "label_Date";
        resources.ApplyResources(panel5, "panel5");
        panel5.BorderStyle = BorderStyle.Fixed3D;
        panel5.Controls.Add(label_VHF);
        panel5.Name = "panel5";
        resources.ApplyResources(label_VHF, "label_VHF");
        label_VHF.Name = "label_VHF";
        resources.ApplyResources(panel4, "panel4");
        panel4.BorderStyle = BorderStyle.Fixed3D;
        panel4.Controls.Add(label_UHF);
        panel4.Name = "panel4";
        resources.ApplyResources(label_UHF, "label_UHF");
        label_UHF.Name = "label_UHF";
        resources.ApplyResources(panel2, "panel2");
        panel2.BorderStyle = BorderStyle.Fixed3D;
        panel2.Controls.Add(label_staPort);
        panel2.Name = "panel2";
        resources.ApplyResources(label_staPort, "label_staPort");
        label_staPort.Name = "label_staPort";
        resources.ApplyResources(panel3, "panel3");
        panel3.BorderStyle = BorderStyle.Fixed3D;
        panel3.Controls.Add(label_Band);
        panel3.Name = "panel3";
        resources.ApplyResources(label_Band, "label_Band");
        label_Band.Name = "label_Band";
        timer1.Enabled = true;
        timer1.Interval = 1000;
        timer1.Tick += timer1_Tick;
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(panel1);
        Controls.Add(toolStrip);
        Controls.Add(menuStrip);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        IsMdiContainer = true;
        MainMenuStrip = menuStrip;
        Name = "FormMain";
        WindowState = FormWindowState.Maximized;
        FormClosing += FormMain_FormClosing;
        Load += FormMain_Load;
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        toolStrip.ResumeLayout(false);
        toolStrip.PerformLayout();
        panel1.ResumeLayout(false);
        panel6.ResumeLayout(false);
        panel5.ResumeLayout(false);
        panel4.ResumeLayout(false);
        panel2.ResumeLayout(false);
        panel3.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }
}