using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using HID;
using SHX_GT12_CPS.Properties;

namespace SHX_GT12_CPS;

public class FormMain : Form
{
    public delegate void isConnectedDelegate(bool isConnected);

    public delegate void PushReceiveDataDele(byte[] datas);

    private readonly IContainer components = null;

    private readonly ushort PID = 650;

    private readonly ushort VID = 10473;

    private AppData appData;

    private connectStatusStruct connectStatus;

    private HIDInterface.HidDevice device;

    private string filePath = "";

    private bool flagAutoConnect = true;

    private HIDInterface hid;

    public isConnectedDelegate isConnectedFunc;

    private string LANG = "Chinese";

    private ToolStripMenuItem menuItem_C_AutoConnect;

    private ToolStripMenuItem menuItem_C_Connect;

    private ToolStripMenuItem menuItem_Connect;

    private ToolStripMenuItem menuItem_F_Exit;

    private ToolStripMenuItem menuItem_F_New;

    private ToolStripMenuItem menuItem_F_Open;

    private ToolStripMenuItem menuItem_F_Recent;

    private ToolStripMenuItem menuItem_F_Save;

    private ToolStripMenuItem menuItem_F_SaveAs;

    private ToolStripMenuItem menuItem_File;

    private ToolStripMenuItem menuItem_H_About;

    private ToolStripMenuItem menuItem_H_SatlliteHelper;

    private ToolStripMenuItem menuItem_Help;

    private ToolStripMenuItem menuItem_P_Read;

    private ToolStripMenuItem menuItem_P_Write;

    private ToolStripMenuItem menuItem_Program;

    private ToolStripMenuItem menuItem_Tools;

    private ToolStripMenuItem menuItem_W_ChannelList;

    private ToolStripMenuItem menuItem_W_DTMF;

    private ToolStripMenuItem menuItem_W_FM;

    private ToolStripMenuItem menuItem_W_Function;

    private ToolStripMenuItem menuItem_W_MDC1200;

    private ToolStripMenuItem menuItem_W_VFO;

    private ToolStripMenuItem menuItem_Windows;

    private MenuStrip menuStrip1;

    public PushReceiveDataDele pushReceiveData;

    private ToolStrip toolStrip1;

    private ToolStripButton tsBtn_New;

    private ToolStripButton tsBtn_Open;

    private ToolStripButton tsBtn_Read;

    private ToolStripButton tsBtn_Save;

    private ToolStripButton tsBtn_Write;

    private FormChannelList wChannelList;

    private FormDTMF wDTMF;

    private FormFM wFM;

    private FormFunction wFunction;

    private FormMDC1200 wMDC;

    private FormVFOMode wVFOMode;

    public FormMain()
    {
        InitializeComponent();
        var imageScalingSize = new Size(32, 32);
        toolStrip1.ImageScalingSize = imageScalingSize;
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
        LANG = Settings.Default.LANG;
        appData = new AppData(LANG);
        wChannelList = FormChannelList.getNewInstance(this);
        wChannelList.LoadData(appData);
        wChannelList.Show();
        hid = new HIDInterface();
        hid.StatusConnected = StatusConnected;
        isConnectedFunc = UpdateDeviceStatus;
        device.VID = VID;
        device.PID = PID;
        device.SERIAL = "";
        hid.AutoConnect(device);
    }

    private void LoadData(AppData dat)
    {
        appData = dat;
        if (wChannelList != null) wChannelList.LoadData(appData);

        if (wVFOMode != null) wVFOMode.LoadData(appData.Vfos);

        if (wFunction != null) wFunction.LoadData(appData.FunCfgs, appData.Mdcs);

        if (wFM != null) wFM.LoadData(appData.Fms);

        if (wDTMF != null) wDTMF.LoadData(appData.Dtmfs);
    }

    private void menuItem_F_New_Click(object sender, EventArgs e)
    {
        LoadData(new AppData(LANG));
        filePath = "";
        Text = "GT12-No Name";
    }

    private void menuItem_F_Open_Click(object sender, EventArgs e)
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
            LoadData(AppData.CreatObjFromFile(s));
            Text = "GT12-" + filePath;
        }
    }

    private void menuItem_F_Save_Click(object sender, EventArgs e)
    {
        if (filePath != "")
        {
            Stream stream = new FileStream(filePath, FileMode.OpenOrCreate);
            stream.Seek(0L, SeekOrigin.Begin);
            stream.SetLength(0L);
            appData.SaveToFile(stream);
            stream.Close();
        }
        else
        {
            menuItem_F_SaveAs.PerformClick();
        }
    }

    private void menuItem_F_SaveAs_Click(object sender, EventArgs e)
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
            appData.SaveToFile(stream);
            stream.Close();
            filePath = saveFileDialog.FileName;
            Text = "GT12-" + filePath;
        }
    }

    private void menuItem_F_Exit_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void menuItem_W_ChannelList_Click(object sender, EventArgs e)
    {
        if (wChannelList == null)
        {
            wChannelList = FormChannelList.getNewInstance(this);
            wChannelList.Show();
        }
        else
        {
            wChannelList.Show();
            wChannelList.BringToFront();
        }
    }

    private void menuItem_W_VFO_Click(object sender, EventArgs e)
    {
        if (wVFOMode == null)
        {
            wVFOMode = new FormVFOMode(this);
            wVFOMode.LoadData(appData.Vfos);
            wVFOMode.Show();
        }
        else
        {
            wVFOMode.Show();
            wVFOMode.BringToFront();
        }
    }

    private void menuItem_W_Function_Click(object sender, EventArgs e)
    {
        if (wFunction == null)
        {
            wFunction = new FormFunction(this);
            wFunction.LoadData(appData.FunCfgs, appData.Mdcs);
            wFunction.Show();
        }
        else
        {
            wFunction.Show();
            wFunction.BringToFront();
        }
    }

    private void menuItem_W_FM_Click(object sender, EventArgs e)
    {
        if (wFM == null)
        {
            wFM = new FormFM(this);
            wFM.Show();
            wFM.LoadData(appData.Fms);
        }
        else
        {
            wFM.Show();
            wFM.BringToFront();
        }
    }

    private void menuItem_W_DTMF_Click(object sender, EventArgs e)
    {
        if (wDTMF == null)
        {
            wDTMF = new FormDTMF(this);
            wDTMF.Show();
            wDTMF.LoadData(appData.Dtmfs);
        }
        else
        {
            wDTMF.Show();
            wDTMF.BringToFront();
        }
    }

    private void menuItem_W_MDC1200_Click(object sender, EventArgs e)
    {
        if (wMDC == null)
        {
            wMDC = new FormMDC1200(this);
            wMDC.Show();
            wMDC.LoadData(appData.Mdcs);
        }
        else
        {
            wMDC.Show();
            wMDC.BringToFront();
        }
    }

    private void menuItem_P_Read_Click(object sender, EventArgs e)
    {
        if (!connectStatus.curStatus)
        {
            MessageBox.Show("设备未连接!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return;
        }

        var formProgress = new FormProgress();
        formProgress.LoadData(appData, hid);
        formProgress.Text = "读频";
        if (DialogResult.OK == formProgress.ShowDialog())
        {
            appData = formProgress.GetData();
            if (wChannelList != null) wChannelList.LoadData(appData);

            if (wVFOMode != null) wVFOMode.LoadData(appData.Vfos);

            if (wFunction != null) wFunction.LoadData(appData.FunCfgs, appData.Mdcs);

            if (wFM != null) wFM.LoadData(appData.Fms);

            if (wDTMF != null) wDTMF.LoadData(appData.Dtmfs);

            if (wMDC != null) wMDC.LoadData(appData.Mdcs);
        }
    }

    private void menuItem_P_Write_Click(object sender, EventArgs e)
    {
        if (!connectStatus.curStatus)
        {
            MessageBox.Show("设备未连接!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return;
        }

        var formProgress = new FormProgress();
        formProgress.LoadData(appData, hid);
        formProgress.Text = "写频";
        formProgress.ShowDialog();
    }

    private void menuItem_H_SatlliteHelper_Click(object sender, EventArgs e)
    {
        new FormSatelliteHelper().ShowDialog();
    }

    private void menuItem_H_About_Click(object sender, EventArgs e)
    {
        var formAbout = new FormAbout();
        formAbout.ShowDialog();
    }

    public void StatusConnected(object sender, bool isConnect)
    {
        connectStatus.curStatus = isConnect;
        if (connectStatus.curStatus != connectStatus.preStatus)
        {
            connectStatus.preStatus = connectStatus.curStatus;
            if (connectStatus.curStatus)
                Invoke(isConnectedFunc, true);
            else
                Invoke(isConnectedFunc, false);
        }
    }

    private void UpdateDeviceStatus(bool isConnect)
    {
        if (isConnect)
        {
            Text = "GT12-设备已连接";
            menuItem_C_Connect.Text = "断开设备";
        }
        else
        {
            Text = "GT12-设备已断开";
            menuItem_C_Connect.Text = "连接设备";
        }
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (wChannelList != null) wChannelList.Close();

        if (wVFOMode != null) wVFOMode.Close();

        if (wFunction != null) wFunction.Close();

        if (wFM != null) wFM.Close();

        if (wDTMF != null) wDTMF.Close();

        e.Cancel = false;
    }

    private void menuItem_C_Connect_Click(object sender, EventArgs e)
    {
        if (connectStatus.curStatus)
        {
            hid.DisConnect();
            hid.StopAutoConnect();
            flagAutoConnect = false;
            menuItem_C_AutoConnect.Checked = false;
            connectStatus.curStatus = false;
            connectStatus.preStatus = connectStatus.curStatus;
            Invoke(isConnectedFunc, false);
        }
        else
        {
            hid.oSp.DataReceived = hid.HidDataReceived;
            hid.oSp.DeviceRemoved = hid.HidDeviceRemoved;
            if (hid.Connect(device))
            {
                connectStatus.curStatus = true;
                connectStatus.preStatus = connectStatus.curStatus;
                Invoke(isConnectedFunc, true);
            }
            else
            {
                MessageBox.Show("连接失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }

    private void menuItem_C_AutoConnect_Click(object sender, EventArgs e)
    {
        if (menuItem_C_AutoConnect.Checked)
        {
            menuItem_C_AutoConnect.Checked = false;
            flagAutoConnect = false;
            hid.StopAutoConnect();
            return;
        }

        menuItem_C_AutoConnect.Checked = true;
        flagAutoConnect = true;
        if (!connectStatus.curStatus)
        {
            hid.oSp.DataReceived = hid.HidDataReceived;
            hid.oSp.DeviceRemoved = hid.HidDeviceRemoved;
        }

        hid.AutoConnect(device);
    }

    private void toolStrip1_Click(object sender, EventArgs e)
    {
        wChannelList.dGV_ChannelList.CurrentCell = null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var resources =
            new ComponentResourceManager(typeof(FormMain));
        menuStrip1 = new MenuStrip();
        menuItem_File = new ToolStripMenuItem();
        menuItem_Tools = new ToolStripMenuItem();
        menuItem_H_SatlliteHelper = new ToolStripMenuItem();
        menuItem_F_New = new ToolStripMenuItem();
        menuItem_F_Open = new ToolStripMenuItem();
        menuItem_F_Save = new ToolStripMenuItem();
        menuItem_F_SaveAs = new ToolStripMenuItem();
        menuItem_F_Recent = new ToolStripMenuItem();
        menuItem_F_Exit = new ToolStripMenuItem();
        menuItem_Windows = new ToolStripMenuItem();
        menuItem_W_ChannelList = new ToolStripMenuItem();
        menuItem_W_VFO = new ToolStripMenuItem();
        menuItem_W_Function = new ToolStripMenuItem();
        menuItem_W_FM = new ToolStripMenuItem();
        menuItem_W_DTMF = new ToolStripMenuItem();
        menuItem_W_MDC1200 = new ToolStripMenuItem();
        menuItem_Program = new ToolStripMenuItem();
        menuItem_P_Read = new ToolStripMenuItem();
        menuItem_P_Write = new ToolStripMenuItem();
        menuItem_Connect = new ToolStripMenuItem();
        menuItem_C_Connect = new ToolStripMenuItem();
        menuItem_C_AutoConnect = new ToolStripMenuItem();
        menuItem_Help = new ToolStripMenuItem();
        menuItem_H_About = new ToolStripMenuItem();
        toolStrip1 = new ToolStrip();
        tsBtn_New = new ToolStripButton();
        tsBtn_Open = new ToolStripButton();
        tsBtn_Save = new ToolStripButton();
        tsBtn_Read = new ToolStripButton();
        tsBtn_Write = new ToolStripButton();
        menuStrip1.SuspendLayout();
        toolStrip1.SuspendLayout();
        SuspendLayout();
        menuStrip1.ImageScalingSize = new Size(20, 20);
        menuStrip1.Items.AddRange(new ToolStripItem[6]
        {
            menuItem_File, menuItem_Windows, menuItem_Program, menuItem_Connect, menuItem_Tools, menuItem_Help
        });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Padding = new Padding(5, 2, 0, 2);
        menuStrip1.Size = new Size(1191, 28);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        menuItem_File.DropDownItems.AddRange(new ToolStripItem[6]
        {
            menuItem_F_New, menuItem_F_Open, menuItem_F_Save, menuItem_F_SaveAs,
            menuItem_F_Recent, menuItem_F_Exit
        });
        menuItem_File.Name = "menuItem_File";
        menuItem_File.Size = new Size(69, 24);
        menuItem_File.Text = "文件(&F)";
        menuItem_F_New.Name = "menuItem_F_New";
        menuItem_F_New.ShortcutKeys = Keys.N | Keys.Control;
        menuItem_F_New.Size = new Size(204, 26);
        menuItem_F_New.Text = "新建";
        menuItem_F_New.Click += menuItem_F_New_Click;
        menuItem_F_Open.Name = "menuItem_F_Open";
        menuItem_F_Open.ShortcutKeys = Keys.O | Keys.Control;
        menuItem_F_Open.Size = new Size(204, 26);
        menuItem_F_Open.Text = "打开";
        menuItem_F_Open.Click += menuItem_F_Open_Click;
        menuItem_F_Save.Name = "menuItem_F_Save";
        menuItem_F_Save.ShortcutKeys = Keys.S | Keys.Control;
        menuItem_F_Save.Size = new Size(204, 26);
        menuItem_F_Save.Text = "保存";
        menuItem_F_Save.Click += menuItem_F_Save_Click;
        menuItem_F_SaveAs.Name = "menuItem_F_SaveAs";
        menuItem_F_SaveAs.Size = new Size(204, 26);
        menuItem_F_SaveAs.Text = "另存为...";
        menuItem_F_SaveAs.Click += menuItem_F_SaveAs_Click;
        menuItem_F_Recent.Name = "menuItem_F_Recent";
        menuItem_F_Recent.Size = new Size(204, 26);
        menuItem_F_Recent.Text = "最近使用过的文件";
        menuItem_F_Exit.Name = "menuItem_F_Exit";
        menuItem_F_Exit.ShortcutKeys = Keys.X | Keys.Control;
        menuItem_F_Exit.Size = new Size(204, 26);
        menuItem_F_Exit.Text = "退出";
        menuItem_F_Exit.Click += menuItem_F_Exit_Click;
        menuItem_Windows.DropDownItems.AddRange(new ToolStripItem[6]
        {
            menuItem_W_ChannelList, menuItem_W_VFO, menuItem_W_Function, menuItem_W_FM,
            menuItem_W_DTMF, menuItem_W_MDC1200
        });
        menuItem_Windows.Name = "menuItem_Windows";
        menuItem_Windows.Size = new Size(76, 24);
        menuItem_Windows.Text = "窗口(&W)";
        menuItem_W_ChannelList.Name = "menuItem_W_ChannelList";
        menuItem_W_ChannelList.Size = new Size(197, 26);
        menuItem_W_ChannelList.Text = "信道列表";
        menuItem_W_ChannelList.Click += menuItem_W_ChannelList_Click;
        menuItem_W_VFO.Name = "menuItem_W_VFO";
        menuItem_W_VFO.Size = new Size(197, 26);
        menuItem_W_VFO.Text = "频率模式";
        menuItem_W_VFO.Click += menuItem_W_VFO_Click;
        menuItem_W_Function.Name = "menuItem_W_Function";
        menuItem_W_Function.Size = new Size(197, 26);
        menuItem_W_Function.Text = "可选功能";
        menuItem_W_Function.Click += menuItem_W_Function_Click;
        menuItem_W_FM.Name = "menuItem_W_FM";
        menuItem_W_FM.Size = new Size(197, 26);
        menuItem_W_FM.Text = "收音机";
        menuItem_W_FM.Click += menuItem_W_FM_Click;
        menuItem_W_DTMF.Name = "menuItem_W_DTMF";
        menuItem_W_DTMF.Size = new Size(197, 26);
        menuItem_W_DTMF.Text = "双音多频(DTMF)";
        menuItem_W_DTMF.Click += menuItem_W_DTMF_Click;
        menuItem_W_MDC1200.Name = "menuItem_W_MDC1200";
        menuItem_W_MDC1200.Size = new Size(197, 26);
        menuItem_W_MDC1200.Text = "MDC1200";
        menuItem_W_MDC1200.Visible = false;
        menuItem_W_MDC1200.Click += menuItem_W_MDC1200_Click;
        menuItem_Program.DropDownItems.AddRange(new ToolStripItem[2]
            { menuItem_P_Read, menuItem_P_Write });
        menuItem_Program.Name = "menuItem_Program";
        menuItem_Program.Size = new Size(70, 24);
        menuItem_Program.Text = "编程(&P)";
        menuItem_P_Read.Name = "menuItem_P_Read";
        menuItem_P_Read.ShortcutKeys = Keys.R | Keys.Control;
        menuItem_P_Read.Size = new Size(175, 26);
        menuItem_P_Read.Text = "读频";
        menuItem_P_Read.Click += menuItem_P_Read_Click;
        menuItem_P_Write.Name = "menuItem_P_Write";
        menuItem_P_Write.ShortcutKeys = Keys.W | Keys.Control;
        menuItem_P_Write.Size = new Size(175, 26);
        menuItem_P_Write.Text = "写频";
        menuItem_P_Write.Click += menuItem_P_Write_Click;
        menuItem_Connect.DropDownItems.AddRange(new ToolStripItem[2]
            { menuItem_C_Connect, menuItem_C_AutoConnect });
        menuItem_Connect.Name = "menuItem_Connect";
        menuItem_Connect.Size = new Size(101, 24);
        menuItem_Connect.Text = "连接设备(&C)";
        menuItem_C_Connect.Name = "menuItem_C_Connect";
        menuItem_C_Connect.Size = new Size(144, 26);
        menuItem_C_Connect.Text = "连接设备";
        menuItem_C_Connect.Click += menuItem_C_Connect_Click;
        menuItem_C_AutoConnect.Checked = true;
        menuItem_C_AutoConnect.CheckState = CheckState.Checked;
        menuItem_C_AutoConnect.Name = "menuItem_C_AutoConnect";
        menuItem_C_AutoConnect.Size = new Size(144, 26);
        menuItem_C_AutoConnect.Text = "自动连接";
        menuItem_C_AutoConnect.Click += menuItem_C_AutoConnect_Click;
        menuItem_Help.DropDownItems.AddRange(new ToolStripItem[1] { menuItem_H_About });
        menuItem_Help.Name = "menuItem_Help";
        menuItem_Help.Size = new Size(73, 24);
        menuItem_Help.Text = "帮助(&H)";
        menuItem_H_About.Name = "menuItem_H_About";
        menuItem_H_About.ShortcutKeys = Keys.F1 | Keys.Control;
        menuItem_H_About.Size = new Size(189, 26);
        menuItem_H_About.Text = "关于...";
        menuItem_H_About.Click += menuItem_H_About_Click;

        menuItem_Tools.Name = "menuItem_Tools";
        menuItem_Tools.Size = new Size(73, 24);
        menuItem_Tools.Text = "工具(&T)";
        menuItem_Tools.DropDownItems.AddRange(new ToolStripItem[] { menuItem_H_SatlliteHelper });
        menuItem_H_SatlliteHelper.Name = "menuItem_H_SatlliteHelper";
        menuItem_H_SatlliteHelper.Size = new Size(189, 26);
        menuItem_H_SatlliteHelper.Text = "打星助手";
        menuItem_H_SatlliteHelper.Click += menuItem_H_SatlliteHelper_Click;

        toolStrip1.ImageScalingSize = new Size(20, 20);
        toolStrip1.Items.AddRange(new ToolStripItem[5]
            { tsBtn_New, tsBtn_Open, tsBtn_Save, tsBtn_Read, tsBtn_Write });
        toolStrip1.Location = new Point(0, 28);
        toolStrip1.Name = "toolStrip1";
        toolStrip1.Size = new Size(1191, 27);
        toolStrip1.TabIndex = 1;
        toolStrip1.Text = "toolStrip1";
        toolStrip1.Click += toolStrip1_Click;
        tsBtn_New.DisplayStyle = ToolStripItemDisplayStyle.Image;
        tsBtn_New.Image = (Image)resources.GetObject("tsBtn_New.Image");
        tsBtn_New.ImageTransparentColor = Color.Magenta;
        tsBtn_New.Name = "tsBtn_New";
        tsBtn_New.Size = new Size(24, 24);
        tsBtn_New.Text = "toolStripButton1";
        tsBtn_New.Click += menuItem_F_New_Click;
        tsBtn_Open.DisplayStyle = ToolStripItemDisplayStyle.Image;
        tsBtn_Open.Image = (Image)resources.GetObject("tsBtn_Open.Image");
        tsBtn_Open.ImageTransparentColor = Color.Magenta;
        tsBtn_Open.Name = "tsBtn_Open";
        tsBtn_Open.Size = new Size(24, 24);
        tsBtn_Open.Text = "toolStripButton2";
        tsBtn_Open.Click += menuItem_F_Open_Click;
        tsBtn_Save.DisplayStyle = ToolStripItemDisplayStyle.Image;
        tsBtn_Save.Image = (Image)resources.GetObject("tsBtn_Save.Image");
        tsBtn_Save.ImageTransparentColor = Color.Magenta;
        tsBtn_Save.Name = "tsBtn_Save";
        tsBtn_Save.Size = new Size(24, 24);
        tsBtn_Save.Text = "toolStripButton3";
        tsBtn_Save.Click += menuItem_F_Save_Click;
        tsBtn_Read.DisplayStyle = ToolStripItemDisplayStyle.Image;
        tsBtn_Read.Image = (Image)resources.GetObject("tsBtn_Read.Image");
        tsBtn_Read.ImageTransparentColor = Color.Magenta;
        tsBtn_Read.Name = "tsBtn_Read";
        tsBtn_Read.Size = new Size(24, 24);
        tsBtn_Read.Text = "toolStripButton4";
        tsBtn_Read.Click += menuItem_P_Read_Click;
        tsBtn_Write.DisplayStyle = ToolStripItemDisplayStyle.Image;
        tsBtn_Write.Image = (Image)resources.GetObject("tsBtn_Write.Image");
        tsBtn_Write.ImageTransparentColor = Color.Magenta;
        tsBtn_Write.Name = "tsBtn_Write";
        tsBtn_Write.Size = new Size(24, 24);
        tsBtn_Write.Text = "toolStripButton5";
        tsBtn_Write.Click += menuItem_P_Write_Click;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1191, 758);
        Controls.Add(toolStrip1);
        Controls.Add(menuStrip1);
        Icon = (Icon)resources.GetObject("$this.Icon");
        IsMdiContainer = true;
        MainMenuStrip = menuStrip1;
        Margin = new Padding(3, 2, 3, 2);
        Name = "FormMain";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "SHX-GT12";
        WindowState = FormWindowState.Maximized;
        FormClosing += FormMain_FormClosing;
        Load += FormMain_Load;
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        toolStrip1.ResumeLayout(false);
        toolStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}