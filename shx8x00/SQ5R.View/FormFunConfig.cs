using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using DevComponents.Editors;

namespace SQ5R.View;

public class FormFunConfig : Form
{
    private readonly IContainer components = null;
    private Button btn_Close;

    private Button btn_Default;

    private FormFunCFGData bufData;

    private CheckBox cB_AlarmSound;

    private CheckBox cB_AutoLock;

    private CheckBox cB_FMRadioEnable;

    private CheckBox cB_LockKeyBoard;

    private CheckBox cB_SoundOfBi;

    private CheckBox cB_StopSendOnBusy;

    private CheckBox cB_TDR;

    private ComboBoxEx cbB_1750Hz;

    private ComboBoxEx cbB_A_CHBand;

    private ComboBoxEx cbB_A_FHSS;

    private ComboBoxEx cbB_A_FreqStep;

    private ComboBoxEx cbB_A_Power;

    private ComboBoxEx cbB_A_RemainDir;

    private ComboBoxEx cbB_A_RxQT;

    private ComboBoxEx cbB_A_SignalingEnCoder;

    private ComboBoxEx cbB_A_TxQT;

    private ComboBoxEx cbB_AlarmMode;

    private ComboBoxEx cbB_AutoBackLight;

    private ComboBoxEx cbB_B_CHBand;

    private ComboBoxEx cbB_B_FHSS;

    private ComboBoxEx cbB_B_FreqStep;

    private ComboBoxEx cbB_B_Power;

    private ComboBoxEx cbB_B_RemainDir;

    private ComboBoxEx cbB_B_RxQT;

    private ComboBoxEx cbB_B_SignalingEnCoder;

    private ComboBoxEx cbB_B_TxQT;

    private ComboBoxEx cbB_CH_A_DisplayMode;

    private ComboBoxEx cbB_CH_B_DisplayMode;

    private ComboBoxEx cbB_DTMF;

    private ComboBoxEx cbB_KeySide;

    private ComboBoxEx cbB_KeySideL;

    private ComboBoxEx cbB_Language;

    private ComboBoxEx cbB_MicGain;

    private ComboBoxEx cbB_PassRepetNoiseClear;

    private ComboBoxEx cbB_PassRepetNoiseDetect;

    private ComboBoxEx cbB_PowerOnMsg;

    private ComboBoxEx cbB_PTTID;

    private ComboBoxEx cbB_SaveMode;

    private ComboBoxEx cbB_Scan;

    private ComboBoxEx cbB_SendIDDelay;

    private ComboBoxEx cbB_SoundOfTxEnd;

    private ComboBoxEx cbB_SQL;

    private ComboItem cbB_SQL_Items;

    private ComboItem cbB_SQL_Items1;

    private ComboItem cbB_SQL_Items2;

    private ComboItem cbB_SQL_Items3;

    private ComboItem cbB_SQL_Items4;

    private ComboItem cbB_SQL_Items5;

    private ComboItem cbB_SQL_Items6;

    private ComboItem cbB_SQL_Items7;

    private ComboItem cbB_SQL_Items8;

    private ComboItem cbB_SQL_Items9;

    private ComboBoxEx cbB_TailNoiseClear;

    private ComboBoxEx cbB_TimerMenuQuit;

    private ComboBoxEx cbB_TOT;

    private ComboItem cbB_TOT_Items;

    private ComboItem cbB_TOT_Items0;

    private ComboItem cbB_TOT_Items1;

    private ComboItem cbB_TOT_Items2;

    private ComboItem cbB_TOT_Items3;

    private ComboItem cbB_TOT_Items4;

    private ComboItem cbB_TOT_Items5;

    private ComboItem cbB_TOT_Items6;

    private ComboItem cbB_TOT_Items7;

    private ComboBoxEx cbB_VoicSwitch;

    private ComboBoxEx cbB_VOX;

    private ComboItem cbB_Vox_Items;

    private ComboItem cbB_Vox_Items1;

    private ComboItem cbB_Vox_Items2;

    private ComboItem cbB_Vox_Items3;

    private ComboBoxEx cbB_VoxDelay;

    private ComboBoxEx cbB_WorkModeA;

    private ComboBoxEx cbB_WorkModeB;
    public MyDelegate ChangeCHCounts;

    private GroupBox gB_FreqMode_A;

    private GroupBox gB_FreqMode_B;

    private GroupBox groupBox1;

    private GroupBox groupBox4;

    private Label label_1750Hz;

    private Label label_A_CHBand;

    private Label label_A_CurFreq;

    private Label label_A_FHSS;

    private Label label_A_FreqStep;

    private Label label_A_Power;

    private Label label_A_RemainDir;

    private Label label_A_RemainFreq;

    private Label label_A_RxQT;

    private Label label_A_SignalingEnCoder;

    private Label label_A_TxQT;

    private Label label_AlarmMode;

    private Label label_AutoBackLight;

    private Label label_B_CHBand;

    private Label label_B_CurFreq;

    private Label label_B_FHSS;

    private Label label_B_FreqStep;

    private Label label_B_Power;

    private Label label_B_RemainDir;

    private Label label_B_RemainFreq;

    private Label label_B_RxQT;

    private Label label_B_SignalingEnCoder;

    private Label label_B_TxQT;

    private Label label_CH_A_DisplayMode;

    private Label label_CH_B_DisplayMode;

    private Label label_DTMF;

    private Label label_KeySide;

    private Label label_KeySideL;

    private Label label_Language;

    private Label label_MicGain;

    private Label label_PassRepetNoiseClear;

    private Label label_PassRepetNoiseDetect;

    private Label label_PowerOnMsg;

    private Label label_PTTID;

    private Label label_SaveMode;

    private Label label_Scan;

    private Label label_SendIDDelay;

    private Label label_SoundOfTxEnd;

    private Label label_SQL;

    private Label label_TailNoiseClear;

    private Label label_TimerMenuQuit;

    private Label label_TOT;

    private Label label_VoicSwitch;

    private Label label_VOX;

    private Label label_VoxDelay;

    private Label label_WorkModeA;

    private Label label_WorkModeB;

    private Label label25;

    private Label label26;

    private Label label48;

    private Label label49;

    private string language = "中文";

    private Point[] points = new Point[150];

    private Size[] sizes = new Size[150];

    private TextBox tB_A_CurFreq;

    private TextBox tB_A_RemainFreq;

    private TextBox tB_B_CurFreq;

    private TextBox tB_B_RemainFreq;

    public FormFunConfig(FormFunCFGData data)
    {
        InitializeComponent();
        StartPosition = FormStartPosition.CenterParent;
        bindingTheControls(data);
    }

    public bool FlagDataModify { get; set; } = false;

    public static FormFunConfig getInstance(Form father, FormFunCFGData data)
    {
        var formFunConfig = new FormFunConfig(data);
        formFunConfig.MdiParent = father;
        return formFunConfig;
    }

    private void TryToBingdingControl(Control c, string propertyName, object dataSource, string dataMember,
        object defaultVal)
    {
        try
        {
            c.DataBindings.Add(new Binding(propertyName, dataSource, dataMember, false,
                DataSourceUpdateMode.OnPropertyChanged));
        }
        catch
        {
            bufData.GetType().GetProperty(dataMember).SetValue(bufData, defaultVal, null);
            c.DataBindings.Add(new Binding(propertyName, dataSource, dataMember, false,
                DataSourceUpdateMode.OnPropertyChanged));
        }
    }

    private void bindingTheControls(FormFunCFGData dat)
    {
        bufData = dat;
        TryToBingdingControl(cbB_TOT, "SelectedIndex", bufData, "CbB_TOT", 3);
        TryToBingdingControl(cbB_SQL, "SelectedIndex", bufData, "CbB_SQL", 3);
        TryToBingdingControl(cbB_VOX, "SelectedIndex", bufData, "CbB_VOX", 0);
        TryToBingdingControl(cbB_VoicSwitch, "SelectedIndex", bufData, "CbB_VoicSwitch", 1);
        TryToBingdingControl(cbB_Language, "SelectedIndex", bufData, "CbB_Language", 2);
        TryToBingdingControl(cbB_AutoBackLight, "SelectedIndex", bufData, "CbB_AutoBackLight", 5);
        TryToBingdingControl(cbB_WorkModeA, "SelectedIndex", bufData, "CbB_WorkModeA", 0);
        TryToBingdingControl(cbB_WorkModeB, "SelectedIndex", bufData, "CbB_WorkModeB", 0);
        TryToBingdingControl(cbB_CH_A_DisplayMode, "SelectedIndex", bufData, "CbB_CH_A_DisplayMode", 1);
        TryToBingdingControl(cbB_CH_B_DisplayMode, "SelectedIndex", bufData, "CbB_CH_B_DisplayMode", 1);
        TryToBingdingControl(cbB_PowerOnMsg, "SelectedIndex", bufData, "CbB_PowerOnMsg", 0);
        TryToBingdingControl(cbB_VoxDelay, "SelectedIndex", bufData, "CbB_VoxDelay", 5);
        TryToBingdingControl(cbB_TimerMenuQuit, "SelectedIndex", bufData, "CbB_TimerMenuQuit", 1);
        TryToBingdingControl(cbB_MicGain, "SelectedIndex", bufData, "CbB_MicGain", 2);
        TryToBingdingControl(cbB_DTMF, "SelectedIndex", bufData, "CbB_DTMF", 3);
        TryToBingdingControl(cbB_SaveMode, "SelectedIndex", bufData, "CbB_SaveMode", 1);
        TryToBingdingControl(cbB_Scan, "SelectedIndex", bufData, "CbB_Scan", 1);
        TryToBingdingControl(cbB_PTTID, "SelectedIndex", bufData, "CbB_PTTID", 0);
        TryToBingdingControl(cbB_SendIDDelay, "SelectedIndex", bufData, "CbB_SendIDDelay", 5);
        TryToBingdingControl(cbB_KeySide, "SelectedIndex", bufData, "CbB_KeySide", 0);
        TryToBingdingControl(cbB_KeySideL, "SelectedIndex", bufData, "CbB_KeySideL", 1);
        TryToBingdingControl(cB_LockKeyBoard, "Checked", bufData, "CB_LockKeyBoard", false);
        TryToBingdingControl(cB_AutoLock, "Checked", bufData, "CB_AutoLock", false);
        TryToBingdingControl(cB_StopSendOnBusy, "Checked", bufData, "CB_StopSendOnBusy", false);
        TryToBingdingControl(cB_SoundOfBi, "Checked", bufData, "CB_SoundOfBi", true);
        TryToBingdingControl(tB_A_CurFreq, "Text", bufData, "TB_A_CurFreq", "146.02500");
        TryToBingdingControl(cbB_A_FHSS, "SelectedIndex", bufData, "CbB_A_FHSS", 0);
        TryToBingdingControl(cbB_A_Power, "SelectedIndex", bufData, "CbB_A_Power", 1);
        TryToBingdingControl(cbB_A_RxQT, "Text", bufData, "CbB_A_RxQT", "OFF");
        TryToBingdingControl(cbB_A_TxQT, "Text", bufData, "CbB_A_TxQT", "OFF");
        TryToBingdingControl(cbB_A_CHBand, "SelectedIndex", bufData, "CbB_A_CHBand", 0);
        TryToBingdingControl(cbB_A_FreqStep, "SelectedIndex", bufData, "CbB_A_FreqStep", 5);
        TryToBingdingControl(tB_A_RemainFreq, "Text", bufData, "TB_A_RemainFreq", "00.0000");
        TryToBingdingControl(cbB_A_RemainDir, "SelectedIndex", bufData, "CbB_A_RemainDir", 0);
        TryToBingdingControl(cbB_A_SignalingEnCoder, "SelectedIndex", bufData, "CbB_A_SignalingEnCoder", 0);
        TryToBingdingControl(tB_B_CurFreq, "Text", bufData, "TB_B_CurFreq", "440.02500");
        TryToBingdingControl(cbB_B_FHSS, "SelectedIndex", bufData, "CbB_B_FHSS", 0);
        TryToBingdingControl(cbB_B_Power, "SelectedIndex", bufData, "CbB_B_Power", 1);
        TryToBingdingControl(cbB_B_RxQT, "Text", bufData, "CbB_B_RxQT", "OFF");
        TryToBingdingControl(cbB_B_TxQT, "Text", bufData, "CbB_B_TxQT", "OFF");
        TryToBingdingControl(cbB_B_CHBand, "SelectedIndex", bufData, "CbB_B_CHBand", 0);
        TryToBingdingControl(cbB_B_FreqStep, "SelectedIndex", bufData, "CbB_B_FreqStep", 5);
        TryToBingdingControl(tB_B_RemainFreq, "Text", bufData, "TB_B_RemainFreq", "00.0000");
        TryToBingdingControl(cbB_B_RemainDir, "SelectedIndex", bufData, "CbB_B_RemainDir", 0);
        TryToBingdingControl(cbB_B_SignalingEnCoder, "SelectedIndex", bufData, "CbB_B_SignalingEnCoder", 0);
        TryToBingdingControl(cbB_TailNoiseClear, "SelectedIndex", bufData, "CbB_TailNoiseClear", 1);
        TryToBingdingControl(cbB_PassRepetNoiseClear, "SelectedIndex", bufData, "CbB_PassRepetNoiseClear", 5);
        TryToBingdingControl(cbB_PassRepetNoiseDetect, "SelectedIndex", bufData, "CbB_PassRepetNoiseDetect", 0);
        TryToBingdingControl(cbB_1750Hz, "SelectedIndex", bufData, "CbB_1750Hz", 2);
        TryToBingdingControl(cB_FMRadioEnable, "Checked", bufData, "CB_FMRadioEnable", true);
        TryToBingdingControl(cB_AlarmSound, "Checked", bufData, "CB_AlarmSound", true);
        TryToBingdingControl(cbB_AlarmMode, "SelectedIndex", bufData, "CbB_AlarmMode", 1);
        TryToBingdingControl(cbB_SoundOfTxEnd, "SelectedIndex", bufData, "CbB_SoundOfTxEnd", 0);
        TryToBingdingControl(cB_TDR, "Checked", bufData, "CB_TDR", false);
    }

    public void updataTheBingdingValue(FormFunCFGData data)
    {
        removeTheBingdings();
        bindingTheControls(data);
    }

    private void removeTheBingdings()
    {
        cbB_TOT.DataBindings.RemoveAt(0);
        cbB_SQL.DataBindings.RemoveAt(0);
        cbB_VOX.DataBindings.RemoveAt(0);
        cbB_VoicSwitch.DataBindings.RemoveAt(0);
        cbB_Language.DataBindings.RemoveAt(0);
        cbB_AutoBackLight.DataBindings.RemoveAt(0);
        cbB_WorkModeA.DataBindings.RemoveAt(0);
        cbB_WorkModeB.DataBindings.RemoveAt(0);
        cbB_PowerOnMsg.DataBindings.RemoveAt(0);
        cbB_VoxDelay.DataBindings.RemoveAt(0);
        cbB_TimerMenuQuit.DataBindings.RemoveAt(0);
        cbB_MicGain.DataBindings.RemoveAt(0);
        cbB_CH_A_DisplayMode.DataBindings.RemoveAt(0);
        cbB_CH_B_DisplayMode.DataBindings.RemoveAt(0);
        cbB_DTMF.DataBindings.RemoveAt(0);
        cbB_SaveMode.DataBindings.RemoveAt(0);
        cbB_Scan.DataBindings.RemoveAt(0);
        cbB_PTTID.DataBindings.RemoveAt(0);
        cbB_SendIDDelay.DataBindings.RemoveAt(0);
        cbB_KeySide.DataBindings.RemoveAt(0);
        cbB_KeySideL.DataBindings.RemoveAt(0);
        cB_LockKeyBoard.DataBindings.RemoveAt(0);
        cB_AutoLock.DataBindings.RemoveAt(0);
        cB_StopSendOnBusy.DataBindings.RemoveAt(0);
        cB_SoundOfBi.DataBindings.RemoveAt(0);
        tB_A_CurFreq.DataBindings.RemoveAt(0);
        cbB_A_Power.DataBindings.RemoveAt(0);
        cbB_A_RxQT.DataBindings.RemoveAt(0);
        cbB_A_TxQT.DataBindings.RemoveAt(0);
        cbB_A_CHBand.DataBindings.RemoveAt(0);
        cbB_A_FreqStep.DataBindings.RemoveAt(0);
        cbB_A_RemainDir.DataBindings.RemoveAt(0);
        tB_A_RemainFreq.DataBindings.RemoveAt(0);
        cbB_A_SignalingEnCoder.DataBindings.RemoveAt(0);
        cbB_A_FHSS.DataBindings.RemoveAt(0);
        tB_B_CurFreq.DataBindings.RemoveAt(0);
        cbB_B_Power.DataBindings.RemoveAt(0);
        cbB_B_RxQT.DataBindings.RemoveAt(0);
        cbB_B_TxQT.DataBindings.RemoveAt(0);
        cbB_B_CHBand.DataBindings.RemoveAt(0);
        cbB_B_FreqStep.DataBindings.RemoveAt(0);
        cbB_B_RemainDir.DataBindings.RemoveAt(0);
        tB_B_RemainFreq.DataBindings.RemoveAt(0);
        cbB_B_SignalingEnCoder.DataBindings.RemoveAt(0);
        cbB_B_FHSS.DataBindings.RemoveAt(0);
        cbB_TailNoiseClear.DataBindings.RemoveAt(0);
        cbB_PassRepetNoiseClear.DataBindings.RemoveAt(0);
        cbB_PassRepetNoiseDetect.DataBindings.RemoveAt(0);
        cbB_1750Hz.DataBindings.RemoveAt(0);
        cB_FMRadioEnable.DataBindings.RemoveAt(0);
        cB_AlarmSound.DataBindings.RemoveAt(0);
        cbB_AlarmMode.DataBindings.RemoveAt(0);
        cbB_SoundOfTxEnd.DataBindings.RemoveAt(0);
        cB_TDR.DataBindings.RemoveAt(0);
    }

    private void btn_Default_Click(object sender, EventArgs e)
    {
        var funCfgData = new FormFunCFGData();
        ((FormMain)MdiParent).theRadioData.funCfgData = funCfgData;
        bufData = ((FormMain)MdiParent).theRadioData.funCfgData;
        removeTheBingdings();
        bindingTheControls(bufData);
    }

    private void btn_Close_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void cbB_KeyPress(object sender, KeyPressEventArgs e)
    {
        e.Handled = true;
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

        var componentResourceManager = new ComponentResourceManager(typeof(FormFunConfig));
        componentResourceManager.ApplyResources(this, "$this");
        foreach (Control control6 in Controls) componentResourceManager.ApplyResources(control6, control6.Name);

        foreach (Control control7 in groupBox1.Controls)
            if (control7.GetType().ToString() != "DevComponents.DotNetBar.Controls.ComboBoxEx")
                componentResourceManager.ApplyResources(control7, control7.Name);

        foreach (Control control8 in groupBox4.Controls)
            if (control8.GetType().ToString() != "DevComponents.DotNetBar.Controls.ComboBoxEx")
                componentResourceManager.ApplyResources(control8, control8.Name);

        foreach (Control control9 in gB_FreqMode_A.Controls)
            if (control9.GetType().ToString() == "System.Windows.Forms.Label")
                componentResourceManager.ApplyResources(control9, control9.Name);

        foreach (Control control10 in gB_FreqMode_B.Controls)
            if (control10.GetType().ToString() == "System.Windows.Forms.Label")
                componentResourceManager.ApplyResources(control10, control10.Name);

        cbB_TOT.Items[0] = componentResourceManager.GetString("cbB_TOT_Items.Text");
        cbB_VoicSwitch.Items[0] = componentResourceManager.GetString("cbB_VoicSwitch.Items");
        for (var i = 1; i < cbB_VoicSwitch.Items.Count; i++)
            cbB_VoicSwitch.Items[i] = componentResourceManager.GetString("cbB_VoicSwitch.Items" + i);

        cbB_Language.Items[0] = componentResourceManager.GetString("cbB_Language.Items");
        for (var j = 1; j < cbB_Language.Items.Count; j++)
            cbB_Language.Items[j] = componentResourceManager.GetString("cbB_Language.Items" + j);

        cbB_WorkModeA.Items[0] = componentResourceManager.GetString("cbB_WorkModeA.Items");
        for (var k = 1; k < cbB_WorkModeA.Items.Count; k++)
            cbB_WorkModeA.Items[k] = componentResourceManager.GetString("cbB_WorkModeA.Items" + k);

        cbB_WorkModeB.Items[0] = componentResourceManager.GetString("cbB_WorkModeB.Items");
        for (var l = 1; l < cbB_WorkModeB.Items.Count; l++)
            cbB_WorkModeB.Items[l] = componentResourceManager.GetString("cbB_WorkModeB.Items" + l);

        cbB_CH_A_DisplayMode.Items[0] = componentResourceManager.GetString("cbB_CH_A_DisplayMode.Items");
        for (var m = 1; m < cbB_CH_A_DisplayMode.Items.Count; m++)
            cbB_CH_A_DisplayMode.Items[m] = componentResourceManager.GetString("cbB_CH_A_DisplayMode.Items" + m);

        cbB_CH_B_DisplayMode.Items[0] = componentResourceManager.GetString("cbB_CH_B_DisplayMode.Items");
        for (var n = 1; n < cbB_CH_B_DisplayMode.Items.Count; n++)
            cbB_CH_B_DisplayMode.Items[n] = componentResourceManager.GetString("cbB_CH_B_DisplayMode.Items" + n);

        cbB_DTMF.Items[0] = componentResourceManager.GetString("cbB_DTMF.Items");
        for (var num = 1; num < cbB_DTMF.Items.Count; num++)
            cbB_DTMF.Items[num] = componentResourceManager.GetString("cbB_DTMF.Items" + num);

        cbB_Scan.Items[0] = componentResourceManager.GetString("cbB_Scan.Items");
        for (var num2 = 1; num2 < cbB_Scan.Items.Count; num2++)
            cbB_Scan.Items[num2] = componentResourceManager.GetString("cbB_Scan.Items" + num2);

        cbB_PTTID.Items[0] = componentResourceManager.GetString("cbB_PTTID.Items");
        for (var num3 = 1; num3 < cbB_PTTID.Items.Count; num3++)
            cbB_PTTID.Items[num3] = componentResourceManager.GetString("cbB_PTTID.Items" + num3);

        cbB_KeySide.Items[0] = componentResourceManager.GetString("cbB_KeySide.Items");
        for (var num4 = 1; num4 < cbB_KeySide.Items.Count; num4++)
            cbB_KeySide.Items[num4] = componentResourceManager.GetString("cbB_KeySide.Items" + num4);

        cbB_KeySideL.Items[0] = componentResourceManager.GetString("cbB_KeySide.Items");
        for (var num5 = 1; num5 < cbB_KeySideL.Items.Count; num5++)
            cbB_KeySideL.Items[num5] = componentResourceManager.GetString("cbB_KeySide.Items" + num5);

        cbB_A_Power.Items[0] = componentResourceManager.GetString("cbB_A_Power.Items");
        for (var num6 = 1; num6 < cbB_A_Power.Items.Count; num6++)
            cbB_A_Power.Items[num6] = componentResourceManager.GetString("cbB_A_Power.Items" + num6);

        cbB_A_CHBand.Items[0] = componentResourceManager.GetString("cbB_A_CHBand.Items");
        for (var num7 = 1; num7 < cbB_A_CHBand.Items.Count; num7++)
            cbB_A_CHBand.Items[num7] = componentResourceManager.GetString("cbB_A_CHBand.Items" + num7);

        cbB_A_FHSS.Items[0] = componentResourceManager.GetString("cbB_A_FHSS.Items");
        cbB_A_FHSS.Items[1] = componentResourceManager.GetString("cbB_A_FHSS.Items1");
        cbB_B_Power.Items[0] = componentResourceManager.GetString("cbB_B_Power.Items");
        for (var num8 = 1; num8 < cbB_B_Power.Items.Count; num8++)
            cbB_B_Power.Items[num8] = componentResourceManager.GetString("cbB_B_Power.Items" + num8);

        cbB_B_CHBand.Items[0] = componentResourceManager.GetString("cbB_B_CHBand.Items");
        for (var num9 = 1; num9 < cbB_B_CHBand.Items.Count; num9++)
            cbB_B_CHBand.Items[num9] = componentResourceManager.GetString("cbB_B_CHBand.Items" + num9);

        cbB_B_FHSS.Items[0] = componentResourceManager.GetString("cbB_B_FHSS.Items");
        cbB_B_FHSS.Items[1] = componentResourceManager.GetString("cbB_B_FHSS.Items1");
        cbB_TailNoiseClear.Items[0] = componentResourceManager.GetString("cbB_TailNoiseClear.Items");
        cbB_TailNoiseClear.Items[1] = componentResourceManager.GetString("cbB_TailNoiseClear.Items1");
        cbB_PowerOnMsg.Items[0] = componentResourceManager.GetString("cbB_PowerOnMsg.Items");
        cbB_PowerOnMsg.Items[1] = componentResourceManager.GetString("cbB_PowerOnMsg.Items1");
        cbB_PowerOnMsg.Items[2] = componentResourceManager.GetString("cbB_PowerOnMsg.Items2");
        cbB_AlarmMode.Items[0] = componentResourceManager.GetString("cbB_AlarmMode.Items");
        for (var num10 = 1; num10 < cbB_AlarmMode.Items.Count; num10++)
            cbB_AlarmMode.Items[num10] = componentResourceManager.GetString("cbB_AlarmMode.Items" + num10);

        cbB_SoundOfTxEnd.Items[0] = componentResourceManager.GetString("cbB_SoundOfTxEnd.Items");
        cbB_SoundOfTxEnd.Items[1] = componentResourceManager.GetString("cbB_SoundOfTxEnd.Items1");
        ResumeLayout(false);
        Visible = visible;
    }

    private void FormFunConfig_FormClosing(object sender, FormClosingEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    private void cbB_A_RxQT_Leave(object sender, EventArgs e)
    {
        var comboBoxEx = (ComboBoxEx)sender;
        var num = comboBoxEx.Items.IndexOf(comboBoxEx.Text);
        if (num == -1)
        {
            comboBoxEx.Text = "OFF";
            bufData.CbB_A_RxQT = "OFF";
        }
    }

    private void cbB_A_TxQT_Leave(object sender, EventArgs e)
    {
        var comboBoxEx = (ComboBoxEx)sender;
        var num = comboBoxEx.Items.IndexOf(comboBoxEx.Text);
        if (num == -1)
        {
            comboBoxEx.Text = "OFF";
            bufData.CbB_A_TxQT = "OFF";
        }
    }

    private void cbB_B_RxQT_Leave(object sender, EventArgs e)
    {
        var comboBoxEx = (ComboBoxEx)sender;
        var num = comboBoxEx.Items.IndexOf(comboBoxEx.Text);
        if (num == -1)
        {
            comboBoxEx.Text = "OFF";
            bufData.CbB_B_RxQT = "OFF";
        }
    }

    private void cbB_B_TxQT_Leave(object sender, EventArgs e)
    {
        var comboBoxEx = (ComboBoxEx)sender;
        var num = comboBoxEx.Items.IndexOf(comboBoxEx.Text);
        if (num == -1)
        {
            comboBoxEx.Text = "OFF";
            bufData.CbB_B_TxQT = "OFF";
        }
    }

    private void cbB_AlarmMode_SelectedValueChanged(object sender, EventArgs e)
    {
        if (cbB_AlarmMode.SelectedIndex == 0)
            cB_AlarmSound.Enabled = false;
        else
            cB_AlarmSound.Enabled = true;
    }

    private void tB_A_CurFreq_TextChanged(object sender, EventArgs e)
    {
        var text = tB_A_CurFreq.Text;
        var length = text.Length;
        if (length <= 0) return;

        if (length != 4)
        {
            if (text[length - 1] < '0' || text[length - 1] > '9')
            {
                tB_A_CurFreq.Text = text.Remove(length - 1, 1);
                bufData.TB_A_CurFreq = tB_A_CurFreq.Text;
            }
        }
        else if (text[length - 1] != '.')
        {
            tB_A_CurFreq.Text = text.Remove(length - 1, 1);
            bufData.TB_A_CurFreq = tB_A_CurFreq.Text;
        }

        tB_A_CurFreq.Select(tB_A_CurFreq.Text.Length, 1);
    }

    private void tB_A_CurFreq_Leave(object sender, EventArgs e)
    {
        var textBox = (TextBox)sender;
        if (textBox.Text != "")
        {
            var num = double.Parse(textBox.Text);
            var flag = false;
            if (num < 100.0 || num >= 520.0)
            {
                if (language == "中文")
                    MessageBox.Show("超出频率范围!\n频率范围: 100 - 520MHz");
                else
                    MessageBox.Show("The Value is out of the range!\n Frequency Range: 100 - 520MHz");

                textBox.Text = "400.12500";
            }
            else
            {
                flag = true;
            }

            if (1 == 0) return;

            var length = textBox.Text.Length;
            var text = textBox.Text;
            if (length <= 9 && length >= 4)
            {
                for (var i = 0; i < 9 - length; i++) text += "0";
            }
            else
            {
                text += ".";
                for (var j = 0; j < 9 - (length + 1); j++) text += "0";
            }

            var num2 = double.Parse(text) * 100000.0;
            if (num2 % 625.0 != 0.0 && num2 % 500.0 != 0.0)
            {
                var num3 = (short)(num2 % 625.0);
                var num4 = (short)(num2 % 500.0);
                short num5 = 0;
                num5 = (short)(num3 < num4 ? 625 : 500);
                var num6 = (int)(num2 / num5);
                num2 = num6 * num5;
                var text2 = (num2 / 100000.0).ToString();
                for (var k = text2.Length; k < 9; k++) text2 = k != 3 ? text2.Insert(k, "0") : text2.Insert(k, ".");

                textBox.Text = text2;
            }
            else
            {
                textBox.Text = text;
            }
        }
        else
        {
            textBox.Text = "400.12500";
        }
    }

    private void tB_B_CurFreq_TextChanged(object sender, EventArgs e)
    {
        var text = tB_B_CurFreq.Text;
        var length = text.Length;
        if (length <= 0) return;

        if (length != 4)
        {
            if (text[length - 1] < '0' || text[length - 1] > '9')
            {
                tB_B_CurFreq.Text = text.Remove(length - 1, 1);
                bufData.TB_B_CurFreq = tB_B_CurFreq.Text;
            }
        }
        else if (text[length - 1] != '.')
        {
            tB_B_CurFreq.Text = text.Remove(length - 1, 1);
            bufData.TB_B_CurFreq = tB_B_CurFreq.Text;
        }

        tB_B_CurFreq.Select(tB_B_CurFreq.Text.Length, 1);
    }

    private void FormFunConfig_KeyPress(object sender, KeyPressEventArgs e)
    {
        if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '.' && e.KeyChar != 'D' && e.KeyChar != 'I' &&
            e.KeyChar != 'N')
            e.Handled = true;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Return) groupBox1.Focus();

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void tB_A_RemainFreq_Leave(object sender, EventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        var num = text.IndexOf('.');
        if (text != "")
        {
            if (num != -1)
            {
                if (num == 0) text = "0" + text;

                var num2 = double.Parse(text);
                var num3 = (int)(num2 * 10000.0);
                num3 /= 5;
                num3 *= 5;
                if (num3 > 100000)
                {
                    text = num3.ToString().Insert(2, ".");
                }
                else if (num3 > 0)
                {
                    if (num3 < 10000)
                    {
                        text = num3.ToString();
                        text = num3 >= 1000
                            ? "00." + text
                            : num3 >= 100
                                ? "00.0" + text
                                : num3 < 10
                                    ? "00.000" + text
                                    : "00.00" + text;
                    }
                    else
                    {
                        text = "0" + num3.ToString().Insert(1, ".");
                    }
                }
                else
                {
                    text = "00.0000";
                }

                textBox.Text = text;
                if ((string)textBox.Tag == "A")
                    bufData.TB_A_RemainFreq = textBox.Text;
                else
                    bufData.TB_B_RemainFreq = textBox.Text;

                return;
            }

            var num4 = int.Parse(text);
            if (num4 > 99)
            {
                if (language == "中文")
                    MessageBox.Show("超出范围 0 -- 99.99875MHz", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                else
                    MessageBox.Show("Out Of Range: 0 -- 99.99875MHz", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Hand);

                textBox.Text = "00.0000";
                if ((string)textBox.Tag == "A")
                    bufData.TB_A_RemainFreq = textBox.Text;
                else
                    bufData.TB_B_RemainFreq = textBox.Text;

                return;
            }

            if (num4 > 0)
            {
                text = (num4 * 1000).ToString();
                text.Insert(text.Length - 3, ".");
            }
            else
            {
                text = "00.0000";
            }

            textBox.Text = text;
            if ((string)textBox.Tag == "A")
                bufData.TB_A_RemainFreq = textBox.Text;
            else
                bufData.TB_B_RemainFreq = textBox.Text;
        }
        else
        {
            textBox.Text = "00.0000";
            if ((string)textBox.Tag == "A")
                bufData.TB_A_RemainFreq = textBox.Text;
            else
                bufData.TB_B_RemainFreq = textBox.Text;
        }
    }

    private void tB_A_RemainFreq_KeyPress(object sender, KeyPressEventArgs e)
    {
        var textBox = (TextBox)sender;
        if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
        {
            e.Handled = true;
            return;
        }

        var num = textBox.Text.IndexOf('.');
        if (e.KeyChar == '.')
        {
            if (num != -1) e.Handled = true;
        }
        else
        {
            if (e.KeyChar == '\b') return;

            if (num == -1)
            {
                if (textBox.Text.Length == 2) e.Handled = true;

                return;
            }

            var num2 = textBox.Text.Length - num - 1;
            if (num2 >= 4) e.Handled = true;
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
            new ComponentResourceManager(typeof(FormFunConfig));
        groupBox1 = new GroupBox();
        cbB_WorkModeB = new ComboBoxEx();
        label_WorkModeB = new Label();
        cbB_VoxDelay = new ComboBoxEx();
        label_VoxDelay = new Label();
        cbB_WorkModeA = new ComboBoxEx();
        label_WorkModeA = new Label();
        cbB_VoicSwitch = new ComboBoxEx();
        label_VoicSwitch = new Label();
        cbB_AutoBackLight = new ComboBoxEx();
        cbB_VOX = new ComboBoxEx();
        cbB_Vox_Items = new ComboItem();
        cbB_Vox_Items1 = new ComboItem();
        cbB_Vox_Items2 = new ComboItem();
        cbB_Vox_Items3 = new ComboItem();
        cbB_SQL = new ComboBoxEx();
        cbB_SQL_Items = new ComboItem();
        cbB_SQL_Items1 = new ComboItem();
        cbB_SQL_Items2 = new ComboItem();
        cbB_SQL_Items3 = new ComboItem();
        cbB_SQL_Items4 = new ComboItem();
        cbB_SQL_Items5 = new ComboItem();
        cbB_TOT = new ComboBoxEx();
        cbB_TOT_Items = new ComboItem();
        cbB_TOT_Items0 = new ComboItem();
        cbB_TOT_Items1 = new ComboItem();
        cbB_TOT_Items2 = new ComboItem();
        cbB_TOT_Items3 = new ComboItem();
        cbB_TOT_Items4 = new ComboItem();
        cbB_TOT_Items5 = new ComboItem();
        cbB_TOT_Items6 = new ComboItem();
        cbB_TOT_Items7 = new ComboItem();
        label_AutoBackLight = new Label();
        label_VOX = new Label();
        label_SQL = new Label();
        label_TOT = new Label();
        cbB_Scan = new ComboBoxEx();
        label_Scan = new Label();
        cbB_SaveMode = new ComboBoxEx();
        label_SaveMode = new Label();
        cbB_Language = new ComboBoxEx();
        label_Language = new Label();
        cbB_SQL_Items6 = new ComboItem();
        cbB_SQL_Items7 = new ComboItem();
        cbB_SQL_Items8 = new ComboItem();
        cbB_SQL_Items9 = new ComboItem();
        groupBox4 = new GroupBox();
        cbB_MicGain = new ComboBoxEx();
        label_MicGain = new Label();
        cbB_TimerMenuQuit = new ComboBoxEx();
        label_TimerMenuQuit = new Label();
        cbB_PowerOnMsg = new ComboBoxEx();
        label_PowerOnMsg = new Label();
        label_CH_B_DisplayMode = new Label();
        label_CH_A_DisplayMode = new Label();
        cbB_CH_B_DisplayMode = new ComboBoxEx();
        cbB_CH_A_DisplayMode = new ComboBoxEx();
        cbB_KeySideL = new ComboBoxEx();
        label_KeySideL = new Label();
        cbB_KeySide = new ComboBoxEx();
        label_KeySide = new Label();
        cB_TDR = new CheckBox();
        cbB_SoundOfTxEnd = new ComboBoxEx();
        cbB_AlarmMode = new ComboBoxEx();
        label_SoundOfTxEnd = new Label();
        cbB_TailNoiseClear = new ComboBoxEx();
        label_TailNoiseClear = new Label();
        label_AlarmMode = new Label();
        cB_AlarmSound = new CheckBox();
        cB_FMRadioEnable = new CheckBox();
        cbB_PassRepetNoiseDetect = new ComboBoxEx();
        cB_SoundOfBi = new CheckBox();
        cB_StopSendOnBusy = new CheckBox();
        cbB_PassRepetNoiseClear = new ComboBoxEx();
        cB_AutoLock = new CheckBox();
        cB_LockKeyBoard = new CheckBox();
        label_PassRepetNoiseClear = new Label();
        cbB_PTTID = new ComboBoxEx();
        cbB_SendIDDelay = new ComboBoxEx();
        label_PassRepetNoiseDetect = new Label();
        cbB_DTMF = new ComboBoxEx();
        label_SendIDDelay = new Label();
        label_PTTID = new Label();
        label_DTMF = new Label();
        gB_FreqMode_A = new GroupBox();
        tB_A_RemainFreq = new TextBox();
        cbB_A_FHSS = new ComboBoxEx();
        label_A_FHSS = new Label();
        label48 = new Label();
        label25 = new Label();
        tB_A_CurFreq = new TextBox();
        cbB_A_SignalingEnCoder = new ComboBoxEx();
        cbB_A_RemainDir = new ComboBoxEx();
        cbB_A_FreqStep = new ComboBoxEx();
        cbB_A_CHBand = new ComboBoxEx();
        cbB_A_TxQT = new ComboBoxEx();
        cbB_A_RxQT = new ComboBoxEx();
        cbB_A_Power = new ComboBoxEx();
        label_A_SignalingEnCoder = new Label();
        label_A_RemainFreq = new Label();
        label_A_RemainDir = new Label();
        label_A_FreqStep = new Label();
        label_A_CHBand = new Label();
        label_A_TxQT = new Label();
        label_A_RxQT = new Label();
        label_A_Power = new Label();
        label_A_CurFreq = new Label();
        gB_FreqMode_B = new GroupBox();
        tB_B_RemainFreq = new TextBox();
        cbB_B_FHSS = new ComboBoxEx();
        label_B_FHSS = new Label();
        label49 = new Label();
        label26 = new Label();
        tB_B_CurFreq = new TextBox();
        cbB_B_SignalingEnCoder = new ComboBoxEx();
        cbB_B_RemainDir = new ComboBoxEx();
        cbB_B_FreqStep = new ComboBoxEx();
        cbB_B_CHBand = new ComboBoxEx();
        cbB_B_TxQT = new ComboBoxEx();
        cbB_B_RxQT = new ComboBoxEx();
        cbB_B_Power = new ComboBoxEx();
        label_B_SignalingEnCoder = new Label();
        label_B_RemainFreq = new Label();
        label_B_RemainDir = new Label();
        label_B_FreqStep = new Label();
        label_B_CHBand = new Label();
        label_B_TxQT = new Label();
        label_B_RxQT = new Label();
        label_B_Power = new Label();
        label_B_CurFreq = new Label();
        btn_Default = new Button();
        btn_Close = new Button();
        cbB_1750Hz = new ComboBoxEx();
        label_1750Hz = new Label();
        groupBox1.SuspendLayout();
        groupBox4.SuspendLayout();
        gB_FreqMode_A.SuspendLayout();
        gB_FreqMode_B.SuspendLayout();
        SuspendLayout();
        groupBox1.Controls.Add(cbB_WorkModeB);
        groupBox1.Controls.Add(label_WorkModeB);
        groupBox1.Controls.Add(cbB_VoxDelay);
        groupBox1.Controls.Add(label_VoxDelay);
        groupBox1.Controls.Add(cbB_WorkModeA);
        groupBox1.Controls.Add(label_WorkModeA);
        groupBox1.Controls.Add(cbB_VoicSwitch);
        groupBox1.Controls.Add(label_VoicSwitch);
        groupBox1.Controls.Add(cbB_AutoBackLight);
        groupBox1.Controls.Add(cbB_VOX);
        groupBox1.Controls.Add(cbB_SQL);
        groupBox1.Controls.Add(cbB_TOT);
        groupBox1.Controls.Add(label_AutoBackLight);
        groupBox1.Controls.Add(label_VOX);
        groupBox1.Controls.Add(label_SQL);
        groupBox1.Controls.Add(label_TOT);
        groupBox1.Controls.Add(cbB_Scan);
        groupBox1.Controls.Add(label_Scan);
        groupBox1.Controls.Add(cbB_SaveMode);
        groupBox1.Controls.Add(label_SaveMode);
        resources.ApplyResources(groupBox1, "groupBox1");
        groupBox1.Name = "groupBox1";
        groupBox1.TabStop = false;
        cbB_WorkModeB.DisplayMember = "Text";
        cbB_WorkModeB.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_WorkModeB.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_WorkModeB.FormattingEnabled = true;
        resources.ApplyResources(cbB_WorkModeB, "cbB_WorkModeB");
        cbB_WorkModeB.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_WorkModeB.Items"),
            resources.GetString("cbB_WorkModeB.Items1")
        });
        cbB_WorkModeB.Name = "cbB_WorkModeB";
        cbB_WorkModeB.Style = eDotNetBarStyle.StyleManagerControlled;
        resources.ApplyResources(label_WorkModeB, "label_WorkModeB");
        label_WorkModeB.Name = "label_WorkModeB";
        cbB_VoxDelay.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_VoxDelay.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_VoxDelay, "cbB_VoxDelay");
        cbB_VoxDelay.Items.AddRange(new object[16]
        {
            resources.GetString("cbB_VoxDelay.Items"),
            resources.GetString("cbB_VoxDelay.Items1"),
            resources.GetString("cbB_VoxDelay.Items2"),
            resources.GetString("cbB_VoxDelay.Items3"),
            resources.GetString("cbB_VoxDelay.Items4"),
            resources.GetString("cbB_VoxDelay.Items5"),
            resources.GetString("cbB_VoxDelay.Items6"),
            resources.GetString("cbB_VoxDelay.Items7"),
            resources.GetString("cbB_VoxDelay.Items8"),
            resources.GetString("cbB_VoxDelay.Items9"),
            resources.GetString("cbB_VoxDelay.Items10"),
            resources.GetString("cbB_VoxDelay.Items11"),
            resources.GetString("cbB_VoxDelay.Items12"),
            resources.GetString("cbB_VoxDelay.Items13"),
            resources.GetString("cbB_VoxDelay.Items14"),
            resources.GetString("cbB_VoxDelay.Items15")
        });
        cbB_VoxDelay.Name = "cbB_VoxDelay";
        resources.ApplyResources(label_VoxDelay, "label_VoxDelay");
        label_VoxDelay.Name = "label_VoxDelay";
        cbB_WorkModeA.DisplayMember = "Text";
        cbB_WorkModeA.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_WorkModeA.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_WorkModeA.FormattingEnabled = true;
        resources.ApplyResources(cbB_WorkModeA, "cbB_WorkModeA");
        cbB_WorkModeA.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_WorkModeA.Items"),
            resources.GetString("cbB_WorkModeA.Items1")
        });
        cbB_WorkModeA.Name = "cbB_WorkModeA";
        cbB_WorkModeA.Style = eDotNetBarStyle.StyleManagerControlled;
        resources.ApplyResources(label_WorkModeA, "label_WorkModeA");
        label_WorkModeA.Name = "label_WorkModeA";
        cbB_VoicSwitch.DisplayMember = "Text";
        cbB_VoicSwitch.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_VoicSwitch.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_VoicSwitch.FormattingEnabled = true;
        resources.ApplyResources(cbB_VoicSwitch, "cbB_VoicSwitch");
        cbB_VoicSwitch.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_VoicSwitch.Items"),
            resources.GetString("cbB_VoicSwitch.Items1")
        });
        cbB_VoicSwitch.Name = "cbB_VoicSwitch";
        cbB_VoicSwitch.Style = eDotNetBarStyle.StyleManagerControlled;
        resources.ApplyResources(label_VoicSwitch, "label_VoicSwitch");
        label_VoicSwitch.Name = "label_VoicSwitch";
        cbB_AutoBackLight.DisplayMember = "Text";
        cbB_AutoBackLight.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_AutoBackLight.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_AutoBackLight.FormattingEnabled = true;
        resources.ApplyResources(cbB_AutoBackLight, "cbB_AutoBackLight");
        cbB_AutoBackLight.Items.AddRange(new object[9]
        {
            resources.GetString("cbB_AutoBackLight.Items"),
            resources.GetString("cbB_AutoBackLight.Items1"),
            resources.GetString("cbB_AutoBackLight.Items2"),
            resources.GetString("cbB_AutoBackLight.Items3"),
            resources.GetString("cbB_AutoBackLight.Items4"),
            resources.GetString("cbB_AutoBackLight.Items5"),
            resources.GetString("cbB_AutoBackLight.Items6"),
            resources.GetString("cbB_AutoBackLight.Items7"),
            resources.GetString("cbB_AutoBackLight.Items8")
        });
        cbB_AutoBackLight.Name = "cbB_AutoBackLight";
        cbB_AutoBackLight.Style = eDotNetBarStyle.StyleManagerControlled;
        cbB_VOX.DisplayMember = "Text";
        cbB_VOX.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_VOX.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_VOX.FormattingEnabled = true;
        resources.ApplyResources(cbB_VOX, "cbB_VOX");
        cbB_VOX.Items.AddRange(new object[4]
            { cbB_Vox_Items, cbB_Vox_Items1, cbB_Vox_Items2, cbB_Vox_Items3 });
        cbB_VOX.Name = "cbB_VOX";
        cbB_VOX.Style = eDotNetBarStyle.StyleManagerControlled;
        resources.ApplyResources(cbB_Vox_Items, "cbB_Vox_Items");
        resources.ApplyResources(cbB_Vox_Items1, "cbB_Vox_Items1");
        resources.ApplyResources(cbB_Vox_Items2, "cbB_Vox_Items2");
        resources.ApplyResources(cbB_Vox_Items3, "cbB_Vox_Items3");
        cbB_SQL.DisplayMember = "Text";
        cbB_SQL.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_SQL.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_SQL.FormattingEnabled = true;
        resources.ApplyResources(cbB_SQL, "cbB_SQL");
        cbB_SQL.Items.AddRange(new object[6]
        {
            cbB_SQL_Items, cbB_SQL_Items1, cbB_SQL_Items2, cbB_SQL_Items3, cbB_SQL_Items4,
            cbB_SQL_Items5
        });
        cbB_SQL.Name = "cbB_SQL";
        cbB_SQL.Style = eDotNetBarStyle.StyleManagerControlled;
        resources.ApplyResources(cbB_SQL_Items, "cbB_SQL_Items");
        resources.ApplyResources(cbB_SQL_Items1, "cbB_SQL_Items1");
        resources.ApplyResources(cbB_SQL_Items2, "cbB_SQL_Items2");
        resources.ApplyResources(cbB_SQL_Items3, "cbB_SQL_Items3");
        resources.ApplyResources(cbB_SQL_Items4, "cbB_SQL_Items4");
        resources.ApplyResources(cbB_SQL_Items5, "cbB_SQL_Items5");
        cbB_TOT.DisplayMember = "Text";
        cbB_TOT.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_TOT.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_TOT.FormattingEnabled = true;
        resources.ApplyResources(cbB_TOT, "cbB_TOT");
        cbB_TOT.Items.AddRange(new object[9]
        {
            cbB_TOT_Items, cbB_TOT_Items0, cbB_TOT_Items1, cbB_TOT_Items2, cbB_TOT_Items3,
            cbB_TOT_Items4, cbB_TOT_Items5, cbB_TOT_Items6, cbB_TOT_Items7
        });
        cbB_TOT.Name = "cbB_TOT";
        cbB_TOT.Style = eDotNetBarStyle.StyleManagerControlled;
        resources.ApplyResources(cbB_TOT_Items, "cbB_TOT_Items");
        resources.ApplyResources(cbB_TOT_Items0, "cbB_TOT_Items0");
        resources.ApplyResources(cbB_TOT_Items1, "cbB_TOT_Items1");
        resources.ApplyResources(cbB_TOT_Items2, "cbB_TOT_Items2");
        resources.ApplyResources(cbB_TOT_Items3, "cbB_TOT_Items3");
        resources.ApplyResources(cbB_TOT_Items4, "cbB_TOT_Items4");
        resources.ApplyResources(cbB_TOT_Items5, "cbB_TOT_Items5");
        resources.ApplyResources(cbB_TOT_Items6, "cbB_TOT_Items6");
        resources.ApplyResources(cbB_TOT_Items7, "cbB_TOT_Items7");
        resources.ApplyResources(label_AutoBackLight, "label_AutoBackLight");
        label_AutoBackLight.Name = "label_AutoBackLight";
        resources.ApplyResources(label_VOX, "label_VOX");
        label_VOX.Name = "label_VOX";
        resources.ApplyResources(label_SQL, "label_SQL");
        label_SQL.Name = "label_SQL";
        resources.ApplyResources(label_TOT, "label_TOT");
        label_TOT.Name = "label_TOT";
        cbB_Scan.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_Scan.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_Scan, "cbB_Scan");
        cbB_Scan.Items.AddRange(new object[3]
        {
            resources.GetString("cbB_Scan.Items"),
            resources.GetString("cbB_Scan.Items1"),
            resources.GetString("cbB_Scan.Items2")
        });
        cbB_Scan.Name = "cbB_Scan";
        cbB_Scan.KeyPress += cbB_KeyPress;
        resources.ApplyResources(label_Scan, "label_Scan");
        label_Scan.Name = "label_Scan";
        cbB_SaveMode.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_SaveMode.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_SaveMode, "cbB_SaveMode");
        cbB_SaveMode.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_SaveMode.Items"),
            resources.GetString("cbB_SaveMode.Items1")
        });
        cbB_SaveMode.Name = "cbB_SaveMode";
        cbB_SaveMode.KeyPress += cbB_KeyPress;
        resources.ApplyResources(label_SaveMode, "label_SaveMode");
        label_SaveMode.Name = "label_SaveMode";
        cbB_Language.DisplayMember = "Text";
        cbB_Language.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_Language.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_Language.FormattingEnabled = true;
        resources.ApplyResources(cbB_Language, "cbB_Language");
        cbB_Language.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_Language.Items"),
            resources.GetString("cbB_Language.Items1")
        });
        cbB_Language.Name = "cbB_Language";
        cbB_Language.Style = eDotNetBarStyle.StyleManagerControlled;
        resources.ApplyResources(label_Language, "label_Language");
        label_Language.Name = "label_Language";
        resources.ApplyResources(cbB_SQL_Items6, "cbB_SQL_Items6");
        resources.ApplyResources(cbB_SQL_Items7, "cbB_SQL_Items7");
        resources.ApplyResources(cbB_SQL_Items8, "cbB_SQL_Items8");
        resources.ApplyResources(cbB_SQL_Items9, "cbB_SQL_Items9");
        groupBox4.Controls.Add(cbB_1750Hz);
        groupBox4.Controls.Add(label_1750Hz);
        groupBox4.Controls.Add(cbB_MicGain);
        groupBox4.Controls.Add(label_MicGain);
        groupBox4.Controls.Add(cbB_TimerMenuQuit);
        groupBox4.Controls.Add(label_TimerMenuQuit);
        groupBox4.Controls.Add(cbB_PowerOnMsg);
        groupBox4.Controls.Add(label_PowerOnMsg);
        groupBox4.Controls.Add(label_CH_B_DisplayMode);
        groupBox4.Controls.Add(label_CH_A_DisplayMode);
        groupBox4.Controls.Add(cbB_CH_B_DisplayMode);
        groupBox4.Controls.Add(cbB_CH_A_DisplayMode);
        groupBox4.Controls.Add(cbB_KeySideL);
        groupBox4.Controls.Add(label_KeySideL);
        groupBox4.Controls.Add(cbB_KeySide);
        groupBox4.Controls.Add(label_KeySide);
        groupBox4.Controls.Add(cB_TDR);
        groupBox4.Controls.Add(cbB_SoundOfTxEnd);
        groupBox4.Controls.Add(cbB_AlarmMode);
        groupBox4.Controls.Add(label_SoundOfTxEnd);
        groupBox4.Controls.Add(cbB_TailNoiseClear);
        groupBox4.Controls.Add(label_TailNoiseClear);
        groupBox4.Controls.Add(label_AlarmMode);
        groupBox4.Controls.Add(cB_AlarmSound);
        groupBox4.Controls.Add(cB_FMRadioEnable);
        groupBox4.Controls.Add(cbB_PassRepetNoiseDetect);
        groupBox4.Controls.Add(cB_SoundOfBi);
        groupBox4.Controls.Add(cB_StopSendOnBusy);
        groupBox4.Controls.Add(cbB_PassRepetNoiseClear);
        groupBox4.Controls.Add(cB_AutoLock);
        groupBox4.Controls.Add(cB_LockKeyBoard);
        groupBox4.Controls.Add(label_PassRepetNoiseClear);
        groupBox4.Controls.Add(cbB_PTTID);
        groupBox4.Controls.Add(cbB_SendIDDelay);
        groupBox4.Controls.Add(label_PassRepetNoiseDetect);
        groupBox4.Controls.Add(cbB_DTMF);
        groupBox4.Controls.Add(label_SendIDDelay);
        groupBox4.Controls.Add(label_PTTID);
        groupBox4.Controls.Add(label_DTMF);
        resources.ApplyResources(groupBox4, "groupBox4");
        groupBox4.Name = "groupBox4";
        groupBox4.TabStop = false;
        cbB_MicGain.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_MicGain.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_MicGain, "cbB_MicGain");
        cbB_MicGain.Items.AddRange(new object[5]
        {
            resources.GetString("cbB_MicGain.Items"),
            resources.GetString("cbB_MicGain.Items1"),
            resources.GetString("cbB_MicGain.Items2"),
            resources.GetString("cbB_MicGain.Items3"),
            resources.GetString("cbB_MicGain.Items4")
        });
        cbB_MicGain.Name = "cbB_MicGain";
        resources.ApplyResources(label_MicGain, "label_MicGain");
        label_MicGain.Name = "label_MicGain";
        cbB_TimerMenuQuit.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_TimerMenuQuit.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_TimerMenuQuit, "cbB_TimerMenuQuit");
        cbB_TimerMenuQuit.Items.AddRange(new object[11]
        {
            resources.GetString("cbB_TimerMenuQuit.Items"),
            resources.GetString("cbB_TimerMenuQuit.Items1"),
            resources.GetString("cbB_TimerMenuQuit.Items2"),
            resources.GetString("cbB_TimerMenuQuit.Items3"),
            resources.GetString("cbB_TimerMenuQuit.Items4"),
            resources.GetString("cbB_TimerMenuQuit.Items5"),
            resources.GetString("cbB_TimerMenuQuit.Items6"),
            resources.GetString("cbB_TimerMenuQuit.Items7"),
            resources.GetString("cbB_TimerMenuQuit.Items8"),
            resources.GetString("cbB_TimerMenuQuit.Items9"),
            resources.GetString("cbB_TimerMenuQuit.Items10")
        });
        cbB_TimerMenuQuit.Name = "cbB_TimerMenuQuit";
        resources.ApplyResources(label_TimerMenuQuit, "label_TimerMenuQuit");
        label_TimerMenuQuit.Name = "label_TimerMenuQuit";
        cbB_PowerOnMsg.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_PowerOnMsg.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_PowerOnMsg, "cbB_PowerOnMsg");
        cbB_PowerOnMsg.Items.AddRange(new object[3]
        {
            resources.GetString("cbB_PowerOnMsg.Items"),
            resources.GetString("cbB_PowerOnMsg.Items1"),
            resources.GetString("cbB_PowerOnMsg.Items2")
        });
        cbB_PowerOnMsg.Name = "cbB_PowerOnMsg";
        resources.ApplyResources(label_PowerOnMsg, "label_PowerOnMsg");
        label_PowerOnMsg.Name = "label_PowerOnMsg";
        resources.ApplyResources(label_CH_B_DisplayMode, "label_CH_B_DisplayMode");
        label_CH_B_DisplayMode.Name = "label_CH_B_DisplayMode";
        resources.ApplyResources(label_CH_A_DisplayMode, "label_CH_A_DisplayMode");
        label_CH_A_DisplayMode.Name = "label_CH_A_DisplayMode";
        cbB_CH_B_DisplayMode.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_CH_B_DisplayMode.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_CH_B_DisplayMode, "cbB_CH_B_DisplayMode");
        cbB_CH_B_DisplayMode.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_CH_B_DisplayMode.Items"),
            resources.GetString("cbB_CH_B_DisplayMode.Items1")
        });
        cbB_CH_B_DisplayMode.Name = "cbB_CH_B_DisplayMode";
        cbB_CH_A_DisplayMode.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_CH_A_DisplayMode.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_CH_A_DisplayMode, "cbB_CH_A_DisplayMode");
        cbB_CH_A_DisplayMode.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_CH_A_DisplayMode.Items"),
            resources.GetString("cbB_CH_A_DisplayMode.Items1")
        });
        cbB_CH_A_DisplayMode.Name = "cbB_CH_A_DisplayMode";
        cbB_KeySideL.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_KeySideL.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_KeySideL, "cbB_KeySideL");
        cbB_KeySideL.Items.AddRange(new object[5]
        {
            resources.GetString("cbB_KeySideL.Items"),
            resources.GetString("cbB_KeySideL.Items1"),
            resources.GetString("cbB_KeySideL.Items2"),
            resources.GetString("cbB_KeySideL.Items3"),
            resources.GetString("cbB_KeySideL.Items4")
        });
        cbB_KeySideL.Name = "cbB_KeySideL";
        resources.ApplyResources(label_KeySideL, "label_KeySideL");
        label_KeySideL.Name = "label_KeySideL";
        cbB_KeySide.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_KeySide.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_KeySide, "cbB_KeySide");
        cbB_KeySide.Items.AddRange(new object[5]
        {
            resources.GetString("cbB_KeySide.Items"),
            resources.GetString("cbB_KeySide.Items1"),
            resources.GetString("cbB_KeySide.Items2"),
            resources.GetString("cbB_KeySide.Items3"),
            resources.GetString("cbB_KeySide.Items4")
        });
        cbB_KeySide.Name = "cbB_KeySide";
        resources.ApplyResources(label_KeySide, "label_KeySide");
        label_KeySide.Name = "label_KeySide";
        resources.ApplyResources(cB_TDR, "cB_TDR");
        cB_TDR.Name = "cB_TDR";
        cB_TDR.UseVisualStyleBackColor = true;
        cbB_SoundOfTxEnd.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_SoundOfTxEnd.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_SoundOfTxEnd, "cbB_SoundOfTxEnd");
        cbB_SoundOfTxEnd.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_SoundOfTxEnd.Items"),
            resources.GetString("cbB_SoundOfTxEnd.Items1")
        });
        cbB_SoundOfTxEnd.Name = "cbB_SoundOfTxEnd";
        cbB_AlarmMode.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_AlarmMode.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_AlarmMode, "cbB_AlarmMode");
        cbB_AlarmMode.Items.AddRange(new object[3]
        {
            resources.GetString("cbB_AlarmMode.Items"),
            resources.GetString("cbB_AlarmMode.Items1"),
            resources.GetString("cbB_AlarmMode.Items2")
        });
        cbB_AlarmMode.Name = "cbB_AlarmMode";
        resources.ApplyResources(label_SoundOfTxEnd, "label_SoundOfTxEnd");
        label_SoundOfTxEnd.Name = "label_SoundOfTxEnd";
        cbB_TailNoiseClear.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_TailNoiseClear.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_TailNoiseClear, "cbB_TailNoiseClear");
        cbB_TailNoiseClear.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_TailNoiseClear.Items"),
            resources.GetString("cbB_TailNoiseClear.Items1")
        });
        cbB_TailNoiseClear.Name = "cbB_TailNoiseClear";
        cbB_TailNoiseClear.KeyPress += cbB_KeyPress;
        resources.ApplyResources(label_TailNoiseClear, "label_TailNoiseClear");
        label_TailNoiseClear.Name = "label_TailNoiseClear";
        resources.ApplyResources(label_AlarmMode, "label_AlarmMode");
        label_AlarmMode.Name = "label_AlarmMode";
        cB_AlarmSound.Checked = true;
        cB_AlarmSound.CheckState = CheckState.Checked;
        resources.ApplyResources(cB_AlarmSound, "cB_AlarmSound");
        cB_AlarmSound.Name = "cB_AlarmSound";
        cB_AlarmSound.UseVisualStyleBackColor = true;
        cB_FMRadioEnable.Checked = true;
        cB_FMRadioEnable.CheckState = CheckState.Checked;
        resources.ApplyResources(cB_FMRadioEnable, "cB_FMRadioEnable");
        cB_FMRadioEnable.Name = "cB_FMRadioEnable";
        cB_FMRadioEnable.UseVisualStyleBackColor = true;
        cbB_PassRepetNoiseDetect.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_PassRepetNoiseDetect.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_PassRepetNoiseDetect, "cbB_PassRepetNoiseDetect");
        cbB_PassRepetNoiseDetect.Items.AddRange(new object[11]
        {
            resources.GetString("cbB_PassRepetNoiseDetect.Items"),
            resources.GetString("cbB_PassRepetNoiseDetect.Items1"),
            resources.GetString("cbB_PassRepetNoiseDetect.Items2"),
            resources.GetString("cbB_PassRepetNoiseDetect.Items3"),
            resources.GetString("cbB_PassRepetNoiseDetect.Items4"),
            resources.GetString("cbB_PassRepetNoiseDetect.Items5"),
            resources.GetString("cbB_PassRepetNoiseDetect.Items6"),
            resources.GetString("cbB_PassRepetNoiseDetect.Items7"),
            resources.GetString("cbB_PassRepetNoiseDetect.Items8"),
            resources.GetString("cbB_PassRepetNoiseDetect.Items9"),
            resources.GetString("cbB_PassRepetNoiseDetect.Items10")
        });
        cbB_PassRepetNoiseDetect.Name = "cbB_PassRepetNoiseDetect";
        cbB_PassRepetNoiseDetect.KeyPress += cbB_KeyPress;
        cB_SoundOfBi.Checked = true;
        cB_SoundOfBi.CheckState = CheckState.Checked;
        resources.ApplyResources(cB_SoundOfBi, "cB_SoundOfBi");
        cB_SoundOfBi.Name = "cB_SoundOfBi";
        cB_SoundOfBi.UseVisualStyleBackColor = true;
        resources.ApplyResources(cB_StopSendOnBusy, "cB_StopSendOnBusy");
        cB_StopSendOnBusy.Name = "cB_StopSendOnBusy";
        cB_StopSendOnBusy.UseVisualStyleBackColor = true;
        cbB_PassRepetNoiseClear.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_PassRepetNoiseClear.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_PassRepetNoiseClear, "cbB_PassRepetNoiseClear");
        cbB_PassRepetNoiseClear.Items.AddRange(new object[11]
        {
            resources.GetString("cbB_PassRepetNoiseClear.Items"),
            resources.GetString("cbB_PassRepetNoiseClear.Items1"),
            resources.GetString("cbB_PassRepetNoiseClear.Items2"),
            resources.GetString("cbB_PassRepetNoiseClear.Items3"),
            resources.GetString("cbB_PassRepetNoiseClear.Items4"),
            resources.GetString("cbB_PassRepetNoiseClear.Items5"),
            resources.GetString("cbB_PassRepetNoiseClear.Items6"),
            resources.GetString("cbB_PassRepetNoiseClear.Items7"),
            resources.GetString("cbB_PassRepetNoiseClear.Items8"),
            resources.GetString("cbB_PassRepetNoiseClear.Items9"),
            resources.GetString("cbB_PassRepetNoiseClear.Items10")
        });
        cbB_PassRepetNoiseClear.Name = "cbB_PassRepetNoiseClear";
        cbB_PassRepetNoiseClear.KeyPress += cbB_KeyPress;
        resources.ApplyResources(cB_AutoLock, "cB_AutoLock");
        cB_AutoLock.Name = "cB_AutoLock";
        cB_AutoLock.UseVisualStyleBackColor = true;
        resources.ApplyResources(cB_LockKeyBoard, "cB_LockKeyBoard");
        cB_LockKeyBoard.Name = "cB_LockKeyBoard";
        cB_LockKeyBoard.UseVisualStyleBackColor = true;
        resources.ApplyResources(label_PassRepetNoiseClear, "label_PassRepetNoiseClear");
        label_PassRepetNoiseClear.Name = "label_PassRepetNoiseClear";
        cbB_PTTID.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_PTTID.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_PTTID, "cbB_PTTID");
        cbB_PTTID.Items.AddRange(new object[4]
        {
            resources.GetString("cbB_PTTID.Items"),
            resources.GetString("cbB_PTTID.Items1"),
            resources.GetString("cbB_PTTID.Items2"),
            resources.GetString("cbB_PTTID.Items3")
        });
        cbB_PTTID.Name = "cbB_PTTID";
        cbB_PTTID.KeyPress += cbB_KeyPress;
        cbB_SendIDDelay.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_SendIDDelay.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_SendIDDelay, "cbB_SendIDDelay");
        cbB_SendIDDelay.Items.AddRange(new object[31]
        {
            resources.GetString("cbB_SendIDDelay.Items"),
            resources.GetString("cbB_SendIDDelay.Items1"),
            resources.GetString("cbB_SendIDDelay.Items2"),
            resources.GetString("cbB_SendIDDelay.Items3"),
            resources.GetString("cbB_SendIDDelay.Items4"),
            resources.GetString("cbB_SendIDDelay.Items5"),
            resources.GetString("cbB_SendIDDelay.Items6"),
            resources.GetString("cbB_SendIDDelay.Items7"),
            resources.GetString("cbB_SendIDDelay.Items8"),
            resources.GetString("cbB_SendIDDelay.Items9"),
            resources.GetString("cbB_SendIDDelay.Items10"),
            resources.GetString("cbB_SendIDDelay.Items11"),
            resources.GetString("cbB_SendIDDelay.Items12"),
            resources.GetString("cbB_SendIDDelay.Items13"),
            resources.GetString("cbB_SendIDDelay.Items14"),
            resources.GetString("cbB_SendIDDelay.Items15"),
            resources.GetString("cbB_SendIDDelay.Items16"),
            resources.GetString("cbB_SendIDDelay.Items17"),
            resources.GetString("cbB_SendIDDelay.Items18"),
            resources.GetString("cbB_SendIDDelay.Items19"),
            resources.GetString("cbB_SendIDDelay.Items20"),
            resources.GetString("cbB_SendIDDelay.Items21"),
            resources.GetString("cbB_SendIDDelay.Items22"),
            resources.GetString("cbB_SendIDDelay.Items23"),
            resources.GetString("cbB_SendIDDelay.Items24"),
            resources.GetString("cbB_SendIDDelay.Items25"),
            resources.GetString("cbB_SendIDDelay.Items26"),
            resources.GetString("cbB_SendIDDelay.Items27"),
            resources.GetString("cbB_SendIDDelay.Items28"),
            resources.GetString("cbB_SendIDDelay.Items29"),
            resources.GetString("cbB_SendIDDelay.Items30")
        });
        cbB_SendIDDelay.Name = "cbB_SendIDDelay";
        cbB_SendIDDelay.KeyPress += cbB_KeyPress;
        resources.ApplyResources(label_PassRepetNoiseDetect, "label_PassRepetNoiseDetect");
        label_PassRepetNoiseDetect.Name = "label_PassRepetNoiseDetect";
        cbB_DTMF.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_DTMF.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_DTMF, "cbB_DTMF");
        cbB_DTMF.Items.AddRange(new object[4]
        {
            resources.GetString("cbB_DTMF.Items"),
            resources.GetString("cbB_DTMF.Items1"),
            resources.GetString("cbB_DTMF.Items2"),
            resources.GetString("cbB_DTMF.Items3")
        });
        cbB_DTMF.Name = "cbB_DTMF";
        cbB_DTMF.KeyPress += cbB_KeyPress;
        resources.ApplyResources(label_SendIDDelay, "label_SendIDDelay");
        label_SendIDDelay.Name = "label_SendIDDelay";
        resources.ApplyResources(label_PTTID, "label_PTTID");
        label_PTTID.Name = "label_PTTID";
        resources.ApplyResources(label_DTMF, "label_DTMF");
        label_DTMF.Name = "label_DTMF";
        gB_FreqMode_A.Controls.Add(tB_A_RemainFreq);
        gB_FreqMode_A.Controls.Add(cbB_A_FHSS);
        gB_FreqMode_A.Controls.Add(label_A_FHSS);
        gB_FreqMode_A.Controls.Add(label48);
        gB_FreqMode_A.Controls.Add(label25);
        gB_FreqMode_A.Controls.Add(tB_A_CurFreq);
        gB_FreqMode_A.Controls.Add(cbB_A_SignalingEnCoder);
        gB_FreqMode_A.Controls.Add(cbB_A_RemainDir);
        gB_FreqMode_A.Controls.Add(cbB_A_FreqStep);
        gB_FreqMode_A.Controls.Add(cbB_A_CHBand);
        gB_FreqMode_A.Controls.Add(cbB_A_TxQT);
        gB_FreqMode_A.Controls.Add(cbB_A_RxQT);
        gB_FreqMode_A.Controls.Add(cbB_A_Power);
        gB_FreqMode_A.Controls.Add(label_A_SignalingEnCoder);
        gB_FreqMode_A.Controls.Add(label_A_RemainFreq);
        gB_FreqMode_A.Controls.Add(label_A_RemainDir);
        gB_FreqMode_A.Controls.Add(label_A_FreqStep);
        gB_FreqMode_A.Controls.Add(label_A_CHBand);
        gB_FreqMode_A.Controls.Add(label_A_TxQT);
        gB_FreqMode_A.Controls.Add(label_A_RxQT);
        gB_FreqMode_A.Controls.Add(label_A_Power);
        gB_FreqMode_A.Controls.Add(label_A_CurFreq);
        resources.ApplyResources(gB_FreqMode_A, "gB_FreqMode_A");
        gB_FreqMode_A.Name = "gB_FreqMode_A";
        gB_FreqMode_A.TabStop = false;
        resources.ApplyResources(tB_A_RemainFreq, "tB_A_RemainFreq");
        tB_A_RemainFreq.Name = "tB_A_RemainFreq";
        tB_A_RemainFreq.Tag = "A";
        tB_A_RemainFreq.KeyPress += tB_A_RemainFreq_KeyPress;
        tB_A_RemainFreq.Leave += tB_A_RemainFreq_Leave;
        cbB_A_FHSS.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_A_FHSS.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_A_FHSS, "cbB_A_FHSS");
        cbB_A_FHSS.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_A_FHSS.Items"),
            resources.GetString("cbB_A_FHSS.Items1")
        });
        cbB_A_FHSS.Name = "cbB_A_FHSS";
        resources.ApplyResources(label_A_FHSS, "label_A_FHSS");
        label_A_FHSS.Name = "label_A_FHSS";
        resources.ApplyResources(label48, "label48");
        label48.Name = "label48";
        resources.ApplyResources(label25, "label25");
        label25.Name = "label25";
        resources.ApplyResources(tB_A_CurFreq, "tB_A_CurFreq");
        tB_A_CurFreq.Name = "tB_A_CurFreq";
        tB_A_CurFreq.Tag = "1";
        tB_A_CurFreq.TextChanged += tB_A_CurFreq_TextChanged;
        tB_A_CurFreq.Leave += tB_A_CurFreq_Leave;
        cbB_A_SignalingEnCoder.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_A_SignalingEnCoder.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_A_SignalingEnCoder, "cbB_A_SignalingEnCoder");
        cbB_A_SignalingEnCoder.Items.AddRange(new object[15]
        {
            resources.GetString("cbB_A_SignalingEnCoder.Items"),
            resources.GetString("cbB_A_SignalingEnCoder.Items1"),
            resources.GetString("cbB_A_SignalingEnCoder.Items2"),
            resources.GetString("cbB_A_SignalingEnCoder.Items3"),
            resources.GetString("cbB_A_SignalingEnCoder.Items4"),
            resources.GetString("cbB_A_SignalingEnCoder.Items5"),
            resources.GetString("cbB_A_SignalingEnCoder.Items6"),
            resources.GetString("cbB_A_SignalingEnCoder.Items7"),
            resources.GetString("cbB_A_SignalingEnCoder.Items8"),
            resources.GetString("cbB_A_SignalingEnCoder.Items9"),
            resources.GetString("cbB_A_SignalingEnCoder.Items10"),
            resources.GetString("cbB_A_SignalingEnCoder.Items11"),
            resources.GetString("cbB_A_SignalingEnCoder.Items12"),
            resources.GetString("cbB_A_SignalingEnCoder.Items13"),
            resources.GetString("cbB_A_SignalingEnCoder.Items14")
        });
        cbB_A_SignalingEnCoder.Name = "cbB_A_SignalingEnCoder";
        cbB_A_SignalingEnCoder.KeyPress += cbB_KeyPress;
        cbB_A_RemainDir.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_A_RemainDir.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_A_RemainDir, "cbB_A_RemainDir");
        cbB_A_RemainDir.Items.AddRange(new object[3]
        {
            resources.GetString("cbB_A_RemainDir.Items"),
            resources.GetString("cbB_A_RemainDir.Items1"),
            resources.GetString("cbB_A_RemainDir.Items2")
        });
        cbB_A_RemainDir.Name = "cbB_A_RemainDir";
        cbB_A_RemainDir.KeyPress += cbB_KeyPress;
        cbB_A_FreqStep.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_A_FreqStep.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_A_FreqStep, "cbB_A_FreqStep");
        cbB_A_FreqStep.Items.AddRange(new object[8]
        {
            resources.GetString("cbB_A_FreqStep.Items"),
            resources.GetString("cbB_A_FreqStep.Items1"),
            resources.GetString("cbB_A_FreqStep.Items2"),
            resources.GetString("cbB_A_FreqStep.Items3"),
            resources.GetString("cbB_A_FreqStep.Items4"),
            resources.GetString("cbB_A_FreqStep.Items5"),
            resources.GetString("cbB_A_FreqStep.Items6"),
            resources.GetString("cbB_A_FreqStep.Items7")
        });
        cbB_A_FreqStep.Name = "cbB_A_FreqStep";
        cbB_A_FreqStep.KeyPress += cbB_KeyPress;
        cbB_A_CHBand.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_A_CHBand.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_A_CHBand, "cbB_A_CHBand");
        cbB_A_CHBand.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_A_CHBand.Items"),
            resources.GetString("cbB_A_CHBand.Items1")
        });
        cbB_A_CHBand.Name = "cbB_A_CHBand";
        cbB_A_CHBand.KeyPress += cbB_KeyPress;
        cbB_A_TxQT.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_A_TxQT.DropDownHeight = 100;
        resources.ApplyResources(cbB_A_TxQT, "cbB_A_TxQT");
        cbB_A_TxQT.Items.AddRange(new object[252]
        {
            resources.GetString("cbB_A_TxQT.Items"),
            resources.GetString("cbB_A_TxQT.Items1"),
            resources.GetString("cbB_A_TxQT.Items2"),
            resources.GetString("cbB_A_TxQT.Items3"),
            resources.GetString("cbB_A_TxQT.Items4"),
            resources.GetString("cbB_A_TxQT.Items5"),
            resources.GetString("cbB_A_TxQT.Items6"),
            resources.GetString("cbB_A_TxQT.Items7"),
            resources.GetString("cbB_A_TxQT.Items8"),
            resources.GetString("cbB_A_TxQT.Items9"),
            resources.GetString("cbB_A_TxQT.Items10"),
            resources.GetString("cbB_A_TxQT.Items11"),
            resources.GetString("cbB_A_TxQT.Items12"),
            resources.GetString("cbB_A_TxQT.Items13"),
            resources.GetString("cbB_A_TxQT.Items14"),
            resources.GetString("cbB_A_TxQT.Items15"),
            resources.GetString("cbB_A_TxQT.Items16"),
            resources.GetString("cbB_A_TxQT.Items17"),
            resources.GetString("cbB_A_TxQT.Items18"),
            resources.GetString("cbB_A_TxQT.Items19"),
            resources.GetString("cbB_A_TxQT.Items20"),
            resources.GetString("cbB_A_TxQT.Items21"),
            resources.GetString("cbB_A_TxQT.Items22"),
            resources.GetString("cbB_A_TxQT.Items23"),
            resources.GetString("cbB_A_TxQT.Items24"),
            resources.GetString("cbB_A_TxQT.Items25"),
            resources.GetString("cbB_A_TxQT.Items26"),
            resources.GetString("cbB_A_TxQT.Items27"),
            resources.GetString("cbB_A_TxQT.Items28"),
            resources.GetString("cbB_A_TxQT.Items29"),
            resources.GetString("cbB_A_TxQT.Items30"),
            resources.GetString("cbB_A_TxQT.Items31"),
            resources.GetString("cbB_A_TxQT.Items32"),
            resources.GetString("cbB_A_TxQT.Items33"),
            resources.GetString("cbB_A_TxQT.Items34"),
            resources.GetString("cbB_A_TxQT.Items35"),
            resources.GetString("cbB_A_TxQT.Items36"),
            resources.GetString("cbB_A_TxQT.Items37"),
            resources.GetString("cbB_A_TxQT.Items38"),
            resources.GetString("cbB_A_TxQT.Items39"),
            resources.GetString("cbB_A_TxQT.Items40"),
            resources.GetString("cbB_A_TxQT.Items41"),
            resources.GetString("cbB_A_TxQT.Items42"),
            resources.GetString("cbB_A_TxQT.Items43"),
            resources.GetString("cbB_A_TxQT.Items44"),
            resources.GetString("cbB_A_TxQT.Items45"),
            resources.GetString("cbB_A_TxQT.Items46"),
            resources.GetString("cbB_A_TxQT.Items47"),
            resources.GetString("cbB_A_TxQT.Items48"),
            resources.GetString("cbB_A_TxQT.Items49"),
            resources.GetString("cbB_A_TxQT.Items50"),
            resources.GetString("cbB_A_TxQT.Items51"),
            resources.GetString("cbB_A_TxQT.Items52"),
            resources.GetString("cbB_A_TxQT.Items53"),
            resources.GetString("cbB_A_TxQT.Items54"),
            resources.GetString("cbB_A_TxQT.Items55"),
            resources.GetString("cbB_A_TxQT.Items56"),
            resources.GetString("cbB_A_TxQT.Items57"),
            resources.GetString("cbB_A_TxQT.Items58"),
            resources.GetString("cbB_A_TxQT.Items59"),
            resources.GetString("cbB_A_TxQT.Items60"),
            resources.GetString("cbB_A_TxQT.Items61"),
            resources.GetString("cbB_A_TxQT.Items62"),
            resources.GetString("cbB_A_TxQT.Items63"),
            resources.GetString("cbB_A_TxQT.Items64"),
            resources.GetString("cbB_A_TxQT.Items65"),
            resources.GetString("cbB_A_TxQT.Items66"),
            resources.GetString("cbB_A_TxQT.Items67"),
            resources.GetString("cbB_A_TxQT.Items68"),
            resources.GetString("cbB_A_TxQT.Items69"),
            resources.GetString("cbB_A_TxQT.Items70"),
            resources.GetString("cbB_A_TxQT.Items71"),
            resources.GetString("cbB_A_TxQT.Items72"),
            resources.GetString("cbB_A_TxQT.Items73"),
            resources.GetString("cbB_A_TxQT.Items74"),
            resources.GetString("cbB_A_TxQT.Items75"),
            resources.GetString("cbB_A_TxQT.Items76"),
            resources.GetString("cbB_A_TxQT.Items77"),
            resources.GetString("cbB_A_TxQT.Items78"),
            resources.GetString("cbB_A_TxQT.Items79"),
            resources.GetString("cbB_A_TxQT.Items80"),
            resources.GetString("cbB_A_TxQT.Items81"),
            resources.GetString("cbB_A_TxQT.Items82"),
            resources.GetString("cbB_A_TxQT.Items83"),
            resources.GetString("cbB_A_TxQT.Items84"),
            resources.GetString("cbB_A_TxQT.Items85"),
            resources.GetString("cbB_A_TxQT.Items86"),
            resources.GetString("cbB_A_TxQT.Items87"),
            resources.GetString("cbB_A_TxQT.Items88"),
            resources.GetString("cbB_A_TxQT.Items89"),
            resources.GetString("cbB_A_TxQT.Items90"),
            resources.GetString("cbB_A_TxQT.Items91"),
            resources.GetString("cbB_A_TxQT.Items92"),
            resources.GetString("cbB_A_TxQT.Items93"),
            resources.GetString("cbB_A_TxQT.Items94"),
            resources.GetString("cbB_A_TxQT.Items95"),
            resources.GetString("cbB_A_TxQT.Items96"),
            resources.GetString("cbB_A_TxQT.Items97"),
            resources.GetString("cbB_A_TxQT.Items98"),
            resources.GetString("cbB_A_TxQT.Items99"),
            resources.GetString("cbB_A_TxQT.Items100"),
            resources.GetString("cbB_A_TxQT.Items101"),
            resources.GetString("cbB_A_TxQT.Items102"),
            resources.GetString("cbB_A_TxQT.Items103"),
            resources.GetString("cbB_A_TxQT.Items104"),
            resources.GetString("cbB_A_TxQT.Items105"),
            resources.GetString("cbB_A_TxQT.Items106"),
            resources.GetString("cbB_A_TxQT.Items107"),
            resources.GetString("cbB_A_TxQT.Items108"),
            resources.GetString("cbB_A_TxQT.Items109"),
            resources.GetString("cbB_A_TxQT.Items110"),
            resources.GetString("cbB_A_TxQT.Items111"),
            resources.GetString("cbB_A_TxQT.Items112"),
            resources.GetString("cbB_A_TxQT.Items113"),
            resources.GetString("cbB_A_TxQT.Items114"),
            resources.GetString("cbB_A_TxQT.Items115"),
            resources.GetString("cbB_A_TxQT.Items116"),
            resources.GetString("cbB_A_TxQT.Items117"),
            resources.GetString("cbB_A_TxQT.Items118"),
            resources.GetString("cbB_A_TxQT.Items119"),
            resources.GetString("cbB_A_TxQT.Items120"),
            resources.GetString("cbB_A_TxQT.Items121"),
            resources.GetString("cbB_A_TxQT.Items122"),
            resources.GetString("cbB_A_TxQT.Items123"),
            resources.GetString("cbB_A_TxQT.Items124"),
            resources.GetString("cbB_A_TxQT.Items125"),
            resources.GetString("cbB_A_TxQT.Items126"),
            resources.GetString("cbB_A_TxQT.Items127"),
            resources.GetString("cbB_A_TxQT.Items128"),
            resources.GetString("cbB_A_TxQT.Items129"),
            resources.GetString("cbB_A_TxQT.Items130"),
            resources.GetString("cbB_A_TxQT.Items131"),
            resources.GetString("cbB_A_TxQT.Items132"),
            resources.GetString("cbB_A_TxQT.Items133"),
            resources.GetString("cbB_A_TxQT.Items134"),
            resources.GetString("cbB_A_TxQT.Items135"),
            resources.GetString("cbB_A_TxQT.Items136"),
            resources.GetString("cbB_A_TxQT.Items137"),
            resources.GetString("cbB_A_TxQT.Items138"),
            resources.GetString("cbB_A_TxQT.Items139"),
            resources.GetString("cbB_A_TxQT.Items140"),
            resources.GetString("cbB_A_TxQT.Items141"),
            resources.GetString("cbB_A_TxQT.Items142"),
            resources.GetString("cbB_A_TxQT.Items143"),
            resources.GetString("cbB_A_TxQT.Items144"),
            resources.GetString("cbB_A_TxQT.Items145"),
            resources.GetString("cbB_A_TxQT.Items146"),
            resources.GetString("cbB_A_TxQT.Items147"),
            resources.GetString("cbB_A_TxQT.Items148"),
            resources.GetString("cbB_A_TxQT.Items149"),
            resources.GetString("cbB_A_TxQT.Items150"),
            resources.GetString("cbB_A_TxQT.Items151"),
            resources.GetString("cbB_A_TxQT.Items152"),
            resources.GetString("cbB_A_TxQT.Items153"),
            resources.GetString("cbB_A_TxQT.Items154"),
            resources.GetString("cbB_A_TxQT.Items155"),
            resources.GetString("cbB_A_TxQT.Items156"),
            resources.GetString("cbB_A_TxQT.Items157"),
            resources.GetString("cbB_A_TxQT.Items158"),
            resources.GetString("cbB_A_TxQT.Items159"),
            resources.GetString("cbB_A_TxQT.Items160"),
            resources.GetString("cbB_A_TxQT.Items161"),
            resources.GetString("cbB_A_TxQT.Items162"),
            resources.GetString("cbB_A_TxQT.Items163"),
            resources.GetString("cbB_A_TxQT.Items164"),
            resources.GetString("cbB_A_TxQT.Items165"),
            resources.GetString("cbB_A_TxQT.Items166"),
            resources.GetString("cbB_A_TxQT.Items167"),
            resources.GetString("cbB_A_TxQT.Items168"),
            resources.GetString("cbB_A_TxQT.Items169"),
            resources.GetString("cbB_A_TxQT.Items170"),
            resources.GetString("cbB_A_TxQT.Items171"),
            resources.GetString("cbB_A_TxQT.Items172"),
            resources.GetString("cbB_A_TxQT.Items173"),
            resources.GetString("cbB_A_TxQT.Items174"),
            resources.GetString("cbB_A_TxQT.Items175"),
            resources.GetString("cbB_A_TxQT.Items176"),
            resources.GetString("cbB_A_TxQT.Items177"),
            resources.GetString("cbB_A_TxQT.Items178"),
            resources.GetString("cbB_A_TxQT.Items179"),
            resources.GetString("cbB_A_TxQT.Items180"),
            resources.GetString("cbB_A_TxQT.Items181"),
            resources.GetString("cbB_A_TxQT.Items182"),
            resources.GetString("cbB_A_TxQT.Items183"),
            resources.GetString("cbB_A_TxQT.Items184"),
            resources.GetString("cbB_A_TxQT.Items185"),
            resources.GetString("cbB_A_TxQT.Items186"),
            resources.GetString("cbB_A_TxQT.Items187"),
            resources.GetString("cbB_A_TxQT.Items188"),
            resources.GetString("cbB_A_TxQT.Items189"),
            resources.GetString("cbB_A_TxQT.Items190"),
            resources.GetString("cbB_A_TxQT.Items191"),
            resources.GetString("cbB_A_TxQT.Items192"),
            resources.GetString("cbB_A_TxQT.Items193"),
            resources.GetString("cbB_A_TxQT.Items194"),
            resources.GetString("cbB_A_TxQT.Items195"),
            resources.GetString("cbB_A_TxQT.Items196"),
            resources.GetString("cbB_A_TxQT.Items197"),
            resources.GetString("cbB_A_TxQT.Items198"),
            resources.GetString("cbB_A_TxQT.Items199"),
            resources.GetString("cbB_A_TxQT.Items200"),
            resources.GetString("cbB_A_TxQT.Items201"),
            resources.GetString("cbB_A_TxQT.Items202"),
            resources.GetString("cbB_A_TxQT.Items203"),
            resources.GetString("cbB_A_TxQT.Items204"),
            resources.GetString("cbB_A_TxQT.Items205"),
            resources.GetString("cbB_A_TxQT.Items206"),
            resources.GetString("cbB_A_TxQT.Items207"),
            resources.GetString("cbB_A_TxQT.Items208"),
            resources.GetString("cbB_A_TxQT.Items209"),
            resources.GetString("cbB_A_TxQT.Items210"),
            resources.GetString("cbB_A_TxQT.Items211"),
            resources.GetString("cbB_A_TxQT.Items212"),
            resources.GetString("cbB_A_TxQT.Items213"),
            resources.GetString("cbB_A_TxQT.Items214"),
            resources.GetString("cbB_A_TxQT.Items215"),
            resources.GetString("cbB_A_TxQT.Items216"),
            resources.GetString("cbB_A_TxQT.Items217"),
            resources.GetString("cbB_A_TxQT.Items218"),
            resources.GetString("cbB_A_TxQT.Items219"),
            resources.GetString("cbB_A_TxQT.Items220"),
            resources.GetString("cbB_A_TxQT.Items221"),
            resources.GetString("cbB_A_TxQT.Items222"),
            resources.GetString("cbB_A_TxQT.Items223"),
            resources.GetString("cbB_A_TxQT.Items224"),
            resources.GetString("cbB_A_TxQT.Items225"),
            resources.GetString("cbB_A_TxQT.Items226"),
            resources.GetString("cbB_A_TxQT.Items227"),
            resources.GetString("cbB_A_TxQT.Items228"),
            resources.GetString("cbB_A_TxQT.Items229"),
            resources.GetString("cbB_A_TxQT.Items230"),
            resources.GetString("cbB_A_TxQT.Items231"),
            resources.GetString("cbB_A_TxQT.Items232"),
            resources.GetString("cbB_A_TxQT.Items233"),
            resources.GetString("cbB_A_TxQT.Items234"),
            resources.GetString("cbB_A_TxQT.Items235"),
            resources.GetString("cbB_A_TxQT.Items236"),
            resources.GetString("cbB_A_TxQT.Items237"),
            resources.GetString("cbB_A_TxQT.Items238"),
            resources.GetString("cbB_A_TxQT.Items239"),
            resources.GetString("cbB_A_TxQT.Items240"),
            resources.GetString("cbB_A_TxQT.Items241"),
            resources.GetString("cbB_A_TxQT.Items242"),
            resources.GetString("cbB_A_TxQT.Items243"),
            resources.GetString("cbB_A_TxQT.Items244"),
            resources.GetString("cbB_A_TxQT.Items245"),
            resources.GetString("cbB_A_TxQT.Items246"),
            resources.GetString("cbB_A_TxQT.Items247"),
            resources.GetString("cbB_A_TxQT.Items248"),
            resources.GetString("cbB_A_TxQT.Items249"),
            resources.GetString("cbB_A_TxQT.Items250"),
            resources.GetString("cbB_A_TxQT.Items251")
        });
        cbB_A_TxQT.Name = "cbB_A_TxQT";
        cbB_A_TxQT.Leave += cbB_A_TxQT_Leave;
        cbB_A_RxQT.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_A_RxQT.DropDownHeight = 100;
        resources.ApplyResources(cbB_A_RxQT, "cbB_A_RxQT");
        cbB_A_RxQT.Items.AddRange(new object[252]
        {
            resources.GetString("cbB_A_RxQT.Items"),
            resources.GetString("cbB_A_RxQT.Items1"),
            resources.GetString("cbB_A_RxQT.Items2"),
            resources.GetString("cbB_A_RxQT.Items3"),
            resources.GetString("cbB_A_RxQT.Items4"),
            resources.GetString("cbB_A_RxQT.Items5"),
            resources.GetString("cbB_A_RxQT.Items6"),
            resources.GetString("cbB_A_RxQT.Items7"),
            resources.GetString("cbB_A_RxQT.Items8"),
            resources.GetString("cbB_A_RxQT.Items9"),
            resources.GetString("cbB_A_RxQT.Items10"),
            resources.GetString("cbB_A_RxQT.Items11"),
            resources.GetString("cbB_A_RxQT.Items12"),
            resources.GetString("cbB_A_RxQT.Items13"),
            resources.GetString("cbB_A_RxQT.Items14"),
            resources.GetString("cbB_A_RxQT.Items15"),
            resources.GetString("cbB_A_RxQT.Items16"),
            resources.GetString("cbB_A_RxQT.Items17"),
            resources.GetString("cbB_A_RxQT.Items18"),
            resources.GetString("cbB_A_RxQT.Items19"),
            resources.GetString("cbB_A_RxQT.Items20"),
            resources.GetString("cbB_A_RxQT.Items21"),
            resources.GetString("cbB_A_RxQT.Items22"),
            resources.GetString("cbB_A_RxQT.Items23"),
            resources.GetString("cbB_A_RxQT.Items24"),
            resources.GetString("cbB_A_RxQT.Items25"),
            resources.GetString("cbB_A_RxQT.Items26"),
            resources.GetString("cbB_A_RxQT.Items27"),
            resources.GetString("cbB_A_RxQT.Items28"),
            resources.GetString("cbB_A_RxQT.Items29"),
            resources.GetString("cbB_A_RxQT.Items30"),
            resources.GetString("cbB_A_RxQT.Items31"),
            resources.GetString("cbB_A_RxQT.Items32"),
            resources.GetString("cbB_A_RxQT.Items33"),
            resources.GetString("cbB_A_RxQT.Items34"),
            resources.GetString("cbB_A_RxQT.Items35"),
            resources.GetString("cbB_A_RxQT.Items36"),
            resources.GetString("cbB_A_RxQT.Items37"),
            resources.GetString("cbB_A_RxQT.Items38"),
            resources.GetString("cbB_A_RxQT.Items39"),
            resources.GetString("cbB_A_RxQT.Items40"),
            resources.GetString("cbB_A_RxQT.Items41"),
            resources.GetString("cbB_A_RxQT.Items42"),
            resources.GetString("cbB_A_RxQT.Items43"),
            resources.GetString("cbB_A_RxQT.Items44"),
            resources.GetString("cbB_A_RxQT.Items45"),
            resources.GetString("cbB_A_RxQT.Items46"),
            resources.GetString("cbB_A_RxQT.Items47"),
            resources.GetString("cbB_A_RxQT.Items48"),
            resources.GetString("cbB_A_RxQT.Items49"),
            resources.GetString("cbB_A_RxQT.Items50"),
            resources.GetString("cbB_A_RxQT.Items51"),
            resources.GetString("cbB_A_RxQT.Items52"),
            resources.GetString("cbB_A_RxQT.Items53"),
            resources.GetString("cbB_A_RxQT.Items54"),
            resources.GetString("cbB_A_RxQT.Items55"),
            resources.GetString("cbB_A_RxQT.Items56"),
            resources.GetString("cbB_A_RxQT.Items57"),
            resources.GetString("cbB_A_RxQT.Items58"),
            resources.GetString("cbB_A_RxQT.Items59"),
            resources.GetString("cbB_A_RxQT.Items60"),
            resources.GetString("cbB_A_RxQT.Items61"),
            resources.GetString("cbB_A_RxQT.Items62"),
            resources.GetString("cbB_A_RxQT.Items63"),
            resources.GetString("cbB_A_RxQT.Items64"),
            resources.GetString("cbB_A_RxQT.Items65"),
            resources.GetString("cbB_A_RxQT.Items66"),
            resources.GetString("cbB_A_RxQT.Items67"),
            resources.GetString("cbB_A_RxQT.Items68"),
            resources.GetString("cbB_A_RxQT.Items69"),
            resources.GetString("cbB_A_RxQT.Items70"),
            resources.GetString("cbB_A_RxQT.Items71"),
            resources.GetString("cbB_A_RxQT.Items72"),
            resources.GetString("cbB_A_RxQT.Items73"),
            resources.GetString("cbB_A_RxQT.Items74"),
            resources.GetString("cbB_A_RxQT.Items75"),
            resources.GetString("cbB_A_RxQT.Items76"),
            resources.GetString("cbB_A_RxQT.Items77"),
            resources.GetString("cbB_A_RxQT.Items78"),
            resources.GetString("cbB_A_RxQT.Items79"),
            resources.GetString("cbB_A_RxQT.Items80"),
            resources.GetString("cbB_A_RxQT.Items81"),
            resources.GetString("cbB_A_RxQT.Items82"),
            resources.GetString("cbB_A_RxQT.Items83"),
            resources.GetString("cbB_A_RxQT.Items84"),
            resources.GetString("cbB_A_RxQT.Items85"),
            resources.GetString("cbB_A_RxQT.Items86"),
            resources.GetString("cbB_A_RxQT.Items87"),
            resources.GetString("cbB_A_RxQT.Items88"),
            resources.GetString("cbB_A_RxQT.Items89"),
            resources.GetString("cbB_A_RxQT.Items90"),
            resources.GetString("cbB_A_RxQT.Items91"),
            resources.GetString("cbB_A_RxQT.Items92"),
            resources.GetString("cbB_A_RxQT.Items93"),
            resources.GetString("cbB_A_RxQT.Items94"),
            resources.GetString("cbB_A_RxQT.Items95"),
            resources.GetString("cbB_A_RxQT.Items96"),
            resources.GetString("cbB_A_RxQT.Items97"),
            resources.GetString("cbB_A_RxQT.Items98"),
            resources.GetString("cbB_A_RxQT.Items99"),
            resources.GetString("cbB_A_RxQT.Items100"),
            resources.GetString("cbB_A_RxQT.Items101"),
            resources.GetString("cbB_A_RxQT.Items102"),
            resources.GetString("cbB_A_RxQT.Items103"),
            resources.GetString("cbB_A_RxQT.Items104"),
            resources.GetString("cbB_A_RxQT.Items105"),
            resources.GetString("cbB_A_RxQT.Items106"),
            resources.GetString("cbB_A_RxQT.Items107"),
            resources.GetString("cbB_A_RxQT.Items108"),
            resources.GetString("cbB_A_RxQT.Items109"),
            resources.GetString("cbB_A_RxQT.Items110"),
            resources.GetString("cbB_A_RxQT.Items111"),
            resources.GetString("cbB_A_RxQT.Items112"),
            resources.GetString("cbB_A_RxQT.Items113"),
            resources.GetString("cbB_A_RxQT.Items114"),
            resources.GetString("cbB_A_RxQT.Items115"),
            resources.GetString("cbB_A_RxQT.Items116"),
            resources.GetString("cbB_A_RxQT.Items117"),
            resources.GetString("cbB_A_RxQT.Items118"),
            resources.GetString("cbB_A_RxQT.Items119"),
            resources.GetString("cbB_A_RxQT.Items120"),
            resources.GetString("cbB_A_RxQT.Items121"),
            resources.GetString("cbB_A_RxQT.Items122"),
            resources.GetString("cbB_A_RxQT.Items123"),
            resources.GetString("cbB_A_RxQT.Items124"),
            resources.GetString("cbB_A_RxQT.Items125"),
            resources.GetString("cbB_A_RxQT.Items126"),
            resources.GetString("cbB_A_RxQT.Items127"),
            resources.GetString("cbB_A_RxQT.Items128"),
            resources.GetString("cbB_A_RxQT.Items129"),
            resources.GetString("cbB_A_RxQT.Items130"),
            resources.GetString("cbB_A_RxQT.Items131"),
            resources.GetString("cbB_A_RxQT.Items132"),
            resources.GetString("cbB_A_RxQT.Items133"),
            resources.GetString("cbB_A_RxQT.Items134"),
            resources.GetString("cbB_A_RxQT.Items135"),
            resources.GetString("cbB_A_RxQT.Items136"),
            resources.GetString("cbB_A_RxQT.Items137"),
            resources.GetString("cbB_A_RxQT.Items138"),
            resources.GetString("cbB_A_RxQT.Items139"),
            resources.GetString("cbB_A_RxQT.Items140"),
            resources.GetString("cbB_A_RxQT.Items141"),
            resources.GetString("cbB_A_RxQT.Items142"),
            resources.GetString("cbB_A_RxQT.Items143"),
            resources.GetString("cbB_A_RxQT.Items144"),
            resources.GetString("cbB_A_RxQT.Items145"),
            resources.GetString("cbB_A_RxQT.Items146"),
            resources.GetString("cbB_A_RxQT.Items147"),
            resources.GetString("cbB_A_RxQT.Items148"),
            resources.GetString("cbB_A_RxQT.Items149"),
            resources.GetString("cbB_A_RxQT.Items150"),
            resources.GetString("cbB_A_RxQT.Items151"),
            resources.GetString("cbB_A_RxQT.Items152"),
            resources.GetString("cbB_A_RxQT.Items153"),
            resources.GetString("cbB_A_RxQT.Items154"),
            resources.GetString("cbB_A_RxQT.Items155"),
            resources.GetString("cbB_A_RxQT.Items156"),
            resources.GetString("cbB_A_RxQT.Items157"),
            resources.GetString("cbB_A_RxQT.Items158"),
            resources.GetString("cbB_A_RxQT.Items159"),
            resources.GetString("cbB_A_RxQT.Items160"),
            resources.GetString("cbB_A_RxQT.Items161"),
            resources.GetString("cbB_A_RxQT.Items162"),
            resources.GetString("cbB_A_RxQT.Items163"),
            resources.GetString("cbB_A_RxQT.Items164"),
            resources.GetString("cbB_A_RxQT.Items165"),
            resources.GetString("cbB_A_RxQT.Items166"),
            resources.GetString("cbB_A_RxQT.Items167"),
            resources.GetString("cbB_A_RxQT.Items168"),
            resources.GetString("cbB_A_RxQT.Items169"),
            resources.GetString("cbB_A_RxQT.Items170"),
            resources.GetString("cbB_A_RxQT.Items171"),
            resources.GetString("cbB_A_RxQT.Items172"),
            resources.GetString("cbB_A_RxQT.Items173"),
            resources.GetString("cbB_A_RxQT.Items174"),
            resources.GetString("cbB_A_RxQT.Items175"),
            resources.GetString("cbB_A_RxQT.Items176"),
            resources.GetString("cbB_A_RxQT.Items177"),
            resources.GetString("cbB_A_RxQT.Items178"),
            resources.GetString("cbB_A_RxQT.Items179"),
            resources.GetString("cbB_A_RxQT.Items180"),
            resources.GetString("cbB_A_RxQT.Items181"),
            resources.GetString("cbB_A_RxQT.Items182"),
            resources.GetString("cbB_A_RxQT.Items183"),
            resources.GetString("cbB_A_RxQT.Items184"),
            resources.GetString("cbB_A_RxQT.Items185"),
            resources.GetString("cbB_A_RxQT.Items186"),
            resources.GetString("cbB_A_RxQT.Items187"),
            resources.GetString("cbB_A_RxQT.Items188"),
            resources.GetString("cbB_A_RxQT.Items189"),
            resources.GetString("cbB_A_RxQT.Items190"),
            resources.GetString("cbB_A_RxQT.Items191"),
            resources.GetString("cbB_A_RxQT.Items192"),
            resources.GetString("cbB_A_RxQT.Items193"),
            resources.GetString("cbB_A_RxQT.Items194"),
            resources.GetString("cbB_A_RxQT.Items195"),
            resources.GetString("cbB_A_RxQT.Items196"),
            resources.GetString("cbB_A_RxQT.Items197"),
            resources.GetString("cbB_A_RxQT.Items198"),
            resources.GetString("cbB_A_RxQT.Items199"),
            resources.GetString("cbB_A_RxQT.Items200"),
            resources.GetString("cbB_A_RxQT.Items201"),
            resources.GetString("cbB_A_RxQT.Items202"),
            resources.GetString("cbB_A_RxQT.Items203"),
            resources.GetString("cbB_A_RxQT.Items204"),
            resources.GetString("cbB_A_RxQT.Items205"),
            resources.GetString("cbB_A_RxQT.Items206"),
            resources.GetString("cbB_A_RxQT.Items207"),
            resources.GetString("cbB_A_RxQT.Items208"),
            resources.GetString("cbB_A_RxQT.Items209"),
            resources.GetString("cbB_A_RxQT.Items210"),
            resources.GetString("cbB_A_RxQT.Items211"),
            resources.GetString("cbB_A_RxQT.Items212"),
            resources.GetString("cbB_A_RxQT.Items213"),
            resources.GetString("cbB_A_RxQT.Items214"),
            resources.GetString("cbB_A_RxQT.Items215"),
            resources.GetString("cbB_A_RxQT.Items216"),
            resources.GetString("cbB_A_RxQT.Items217"),
            resources.GetString("cbB_A_RxQT.Items218"),
            resources.GetString("cbB_A_RxQT.Items219"),
            resources.GetString("cbB_A_RxQT.Items220"),
            resources.GetString("cbB_A_RxQT.Items221"),
            resources.GetString("cbB_A_RxQT.Items222"),
            resources.GetString("cbB_A_RxQT.Items223"),
            resources.GetString("cbB_A_RxQT.Items224"),
            resources.GetString("cbB_A_RxQT.Items225"),
            resources.GetString("cbB_A_RxQT.Items226"),
            resources.GetString("cbB_A_RxQT.Items227"),
            resources.GetString("cbB_A_RxQT.Items228"),
            resources.GetString("cbB_A_RxQT.Items229"),
            resources.GetString("cbB_A_RxQT.Items230"),
            resources.GetString("cbB_A_RxQT.Items231"),
            resources.GetString("cbB_A_RxQT.Items232"),
            resources.GetString("cbB_A_RxQT.Items233"),
            resources.GetString("cbB_A_RxQT.Items234"),
            resources.GetString("cbB_A_RxQT.Items235"),
            resources.GetString("cbB_A_RxQT.Items236"),
            resources.GetString("cbB_A_RxQT.Items237"),
            resources.GetString("cbB_A_RxQT.Items238"),
            resources.GetString("cbB_A_RxQT.Items239"),
            resources.GetString("cbB_A_RxQT.Items240"),
            resources.GetString("cbB_A_RxQT.Items241"),
            resources.GetString("cbB_A_RxQT.Items242"),
            resources.GetString("cbB_A_RxQT.Items243"),
            resources.GetString("cbB_A_RxQT.Items244"),
            resources.GetString("cbB_A_RxQT.Items245"),
            resources.GetString("cbB_A_RxQT.Items246"),
            resources.GetString("cbB_A_RxQT.Items247"),
            resources.GetString("cbB_A_RxQT.Items248"),
            resources.GetString("cbB_A_RxQT.Items249"),
            resources.GetString("cbB_A_RxQT.Items250"),
            resources.GetString("cbB_A_RxQT.Items251")
        });
        cbB_A_RxQT.Name = "cbB_A_RxQT";
        cbB_A_RxQT.Leave += cbB_A_RxQT_Leave;
        cbB_A_Power.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_A_Power.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_A_Power, "cbB_A_Power");
        cbB_A_Power.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_A_Power.Items"),
            resources.GetString("cbB_A_Power.Items1")
        });
        cbB_A_Power.Name = "cbB_A_Power";
        cbB_A_Power.KeyPress += cbB_KeyPress;
        resources.ApplyResources(label_A_SignalingEnCoder, "label_A_SignalingEnCoder");
        label_A_SignalingEnCoder.Name = "label_A_SignalingEnCoder";
        resources.ApplyResources(label_A_RemainFreq, "label_A_RemainFreq");
        label_A_RemainFreq.Name = "label_A_RemainFreq";
        resources.ApplyResources(label_A_RemainDir, "label_A_RemainDir");
        label_A_RemainDir.Name = "label_A_RemainDir";
        resources.ApplyResources(label_A_FreqStep, "label_A_FreqStep");
        label_A_FreqStep.Name = "label_A_FreqStep";
        resources.ApplyResources(label_A_CHBand, "label_A_CHBand");
        label_A_CHBand.Name = "label_A_CHBand";
        resources.ApplyResources(label_A_TxQT, "label_A_TxQT");
        label_A_TxQT.Name = "label_A_TxQT";
        resources.ApplyResources(label_A_RxQT, "label_A_RxQT");
        label_A_RxQT.Name = "label_A_RxQT";
        resources.ApplyResources(label_A_Power, "label_A_Power");
        label_A_Power.Name = "label_A_Power";
        resources.ApplyResources(label_A_CurFreq, "label_A_CurFreq");
        label_A_CurFreq.Name = "label_A_CurFreq";
        gB_FreqMode_B.Controls.Add(tB_B_RemainFreq);
        gB_FreqMode_B.Controls.Add(cbB_B_FHSS);
        gB_FreqMode_B.Controls.Add(label_B_FHSS);
        gB_FreqMode_B.Controls.Add(label49);
        gB_FreqMode_B.Controls.Add(label26);
        gB_FreqMode_B.Controls.Add(tB_B_CurFreq);
        gB_FreqMode_B.Controls.Add(cbB_B_SignalingEnCoder);
        gB_FreqMode_B.Controls.Add(cbB_B_RemainDir);
        gB_FreqMode_B.Controls.Add(cbB_B_FreqStep);
        gB_FreqMode_B.Controls.Add(cbB_B_CHBand);
        gB_FreqMode_B.Controls.Add(cbB_B_TxQT);
        gB_FreqMode_B.Controls.Add(cbB_B_RxQT);
        gB_FreqMode_B.Controls.Add(cbB_B_Power);
        gB_FreqMode_B.Controls.Add(label_B_SignalingEnCoder);
        gB_FreqMode_B.Controls.Add(label_B_RemainFreq);
        gB_FreqMode_B.Controls.Add(label_B_RemainDir);
        gB_FreqMode_B.Controls.Add(label_B_FreqStep);
        gB_FreqMode_B.Controls.Add(label_B_CHBand);
        gB_FreqMode_B.Controls.Add(label_B_TxQT);
        gB_FreqMode_B.Controls.Add(label_B_RxQT);
        gB_FreqMode_B.Controls.Add(label_B_Power);
        gB_FreqMode_B.Controls.Add(label_B_CurFreq);
        resources.ApplyResources(gB_FreqMode_B, "gB_FreqMode_B");
        gB_FreqMode_B.Name = "gB_FreqMode_B";
        gB_FreqMode_B.TabStop = false;
        resources.ApplyResources(tB_B_RemainFreq, "tB_B_RemainFreq");
        tB_B_RemainFreq.Name = "tB_B_RemainFreq";
        tB_B_RemainFreq.Tag = "B";
        tB_B_RemainFreq.KeyPress += tB_A_RemainFreq_KeyPress;
        tB_B_RemainFreq.Leave += tB_A_RemainFreq_Leave;
        cbB_B_FHSS.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_B_FHSS.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_B_FHSS, "cbB_B_FHSS");
        cbB_B_FHSS.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_B_FHSS.Items"),
            resources.GetString("cbB_B_FHSS.Items1")
        });
        cbB_B_FHSS.Name = "cbB_B_FHSS";
        resources.ApplyResources(label_B_FHSS, "label_B_FHSS");
        label_B_FHSS.Name = "label_B_FHSS";
        resources.ApplyResources(label49, "label49");
        label49.Name = "label49";
        resources.ApplyResources(label26, "label26");
        label26.Name = "label26";
        resources.ApplyResources(tB_B_CurFreq, "tB_B_CurFreq");
        tB_B_CurFreq.Name = "tB_B_CurFreq";
        tB_B_CurFreq.Tag = "2";
        tB_B_CurFreq.TextChanged += tB_B_CurFreq_TextChanged;
        tB_B_CurFreq.Leave += tB_A_CurFreq_Leave;
        cbB_B_SignalingEnCoder.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_B_SignalingEnCoder.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_B_SignalingEnCoder, "cbB_B_SignalingEnCoder");
        cbB_B_SignalingEnCoder.Items.AddRange(new object[15]
        {
            resources.GetString("cbB_B_SignalingEnCoder.Items"),
            resources.GetString("cbB_B_SignalingEnCoder.Items1"),
            resources.GetString("cbB_B_SignalingEnCoder.Items2"),
            resources.GetString("cbB_B_SignalingEnCoder.Items3"),
            resources.GetString("cbB_B_SignalingEnCoder.Items4"),
            resources.GetString("cbB_B_SignalingEnCoder.Items5"),
            resources.GetString("cbB_B_SignalingEnCoder.Items6"),
            resources.GetString("cbB_B_SignalingEnCoder.Items7"),
            resources.GetString("cbB_B_SignalingEnCoder.Items8"),
            resources.GetString("cbB_B_SignalingEnCoder.Items9"),
            resources.GetString("cbB_B_SignalingEnCoder.Items10"),
            resources.GetString("cbB_B_SignalingEnCoder.Items11"),
            resources.GetString("cbB_B_SignalingEnCoder.Items12"),
            resources.GetString("cbB_B_SignalingEnCoder.Items13"),
            resources.GetString("cbB_B_SignalingEnCoder.Items14")
        });
        cbB_B_SignalingEnCoder.Name = "cbB_B_SignalingEnCoder";
        cbB_B_SignalingEnCoder.KeyPress += cbB_KeyPress;
        cbB_B_RemainDir.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_B_RemainDir.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_B_RemainDir, "cbB_B_RemainDir");
        cbB_B_RemainDir.Items.AddRange(new object[3]
        {
            resources.GetString("cbB_B_RemainDir.Items"),
            resources.GetString("cbB_B_RemainDir.Items1"),
            resources.GetString("cbB_B_RemainDir.Items2")
        });
        cbB_B_RemainDir.Name = "cbB_B_RemainDir";
        cbB_B_RemainDir.KeyPress += cbB_KeyPress;
        cbB_B_FreqStep.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_B_FreqStep.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_B_FreqStep, "cbB_B_FreqStep");
        cbB_B_FreqStep.Items.AddRange(new object[8]
        {
            resources.GetString("cbB_B_FreqStep.Items"),
            resources.GetString("cbB_B_FreqStep.Items1"),
            resources.GetString("cbB_B_FreqStep.Items2"),
            resources.GetString("cbB_B_FreqStep.Items3"),
            resources.GetString("cbB_B_FreqStep.Items4"),
            resources.GetString("cbB_B_FreqStep.Items5"),
            resources.GetString("cbB_B_FreqStep.Items6"),
            resources.GetString("cbB_B_FreqStep.Items7")
        });
        cbB_B_FreqStep.Name = "cbB_B_FreqStep";
        cbB_B_FreqStep.KeyPress += cbB_KeyPress;
        cbB_B_CHBand.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_B_CHBand.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_B_CHBand, "cbB_B_CHBand");
        cbB_B_CHBand.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_B_CHBand.Items"),
            resources.GetString("cbB_B_CHBand.Items1")
        });
        cbB_B_CHBand.Name = "cbB_B_CHBand";
        cbB_B_CHBand.KeyPress += cbB_KeyPress;
        cbB_B_TxQT.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_B_TxQT.DropDownHeight = 100;
        resources.ApplyResources(cbB_B_TxQT, "cbB_B_TxQT");
        cbB_B_TxQT.Items.AddRange(new object[210]
        {
            resources.GetString("cbB_B_TxQT.Items"),
            resources.GetString("cbB_B_TxQT.Items1"),
            resources.GetString("cbB_B_TxQT.Items2"),
            resources.GetString("cbB_B_TxQT.Items3"),
            resources.GetString("cbB_B_TxQT.Items4"),
            resources.GetString("cbB_B_TxQT.Items5"),
            resources.GetString("cbB_B_TxQT.Items6"),
            resources.GetString("cbB_B_TxQT.Items7"),
            resources.GetString("cbB_B_TxQT.Items8"),
            resources.GetString("cbB_B_TxQT.Items9"),
            resources.GetString("cbB_B_TxQT.Items10"),
            resources.GetString("cbB_B_TxQT.Items11"),
            resources.GetString("cbB_B_TxQT.Items12"),
            resources.GetString("cbB_B_TxQT.Items13"),
            resources.GetString("cbB_B_TxQT.Items14"),
            resources.GetString("cbB_B_TxQT.Items15"),
            resources.GetString("cbB_B_TxQT.Items16"),
            resources.GetString("cbB_B_TxQT.Items17"),
            resources.GetString("cbB_B_TxQT.Items18"),
            resources.GetString("cbB_B_TxQT.Items19"),
            resources.GetString("cbB_B_TxQT.Items20"),
            resources.GetString("cbB_B_TxQT.Items21"),
            resources.GetString("cbB_B_TxQT.Items22"),
            resources.GetString("cbB_B_TxQT.Items23"),
            resources.GetString("cbB_B_TxQT.Items24"),
            resources.GetString("cbB_B_TxQT.Items25"),
            resources.GetString("cbB_B_TxQT.Items26"),
            resources.GetString("cbB_B_TxQT.Items27"),
            resources.GetString("cbB_B_TxQT.Items28"),
            resources.GetString("cbB_B_TxQT.Items29"),
            resources.GetString("cbB_B_TxQT.Items30"),
            resources.GetString("cbB_B_TxQT.Items31"),
            resources.GetString("cbB_B_TxQT.Items32"),
            resources.GetString("cbB_B_TxQT.Items33"),
            resources.GetString("cbB_B_TxQT.Items34"),
            resources.GetString("cbB_B_TxQT.Items35"),
            resources.GetString("cbB_B_TxQT.Items36"),
            resources.GetString("cbB_B_TxQT.Items37"),
            resources.GetString("cbB_B_TxQT.Items38"),
            resources.GetString("cbB_B_TxQT.Items39"),
            resources.GetString("cbB_B_TxQT.Items40"),
            resources.GetString("cbB_B_TxQT.Items41"),
            resources.GetString("cbB_B_TxQT.Items42"),
            resources.GetString("cbB_B_TxQT.Items43"),
            resources.GetString("cbB_B_TxQT.Items44"),
            resources.GetString("cbB_B_TxQT.Items45"),
            resources.GetString("cbB_B_TxQT.Items46"),
            resources.GetString("cbB_B_TxQT.Items47"),
            resources.GetString("cbB_B_TxQT.Items48"),
            resources.GetString("cbB_B_TxQT.Items49"),
            resources.GetString("cbB_B_TxQT.Items50"),
            resources.GetString("cbB_B_TxQT.Items51"),
            resources.GetString("cbB_B_TxQT.Items52"),
            resources.GetString("cbB_B_TxQT.Items53"),
            resources.GetString("cbB_B_TxQT.Items54"),
            resources.GetString("cbB_B_TxQT.Items55"),
            resources.GetString("cbB_B_TxQT.Items56"),
            resources.GetString("cbB_B_TxQT.Items57"),
            resources.GetString("cbB_B_TxQT.Items58"),
            resources.GetString("cbB_B_TxQT.Items59"),
            resources.GetString("cbB_B_TxQT.Items60"),
            resources.GetString("cbB_B_TxQT.Items61"),
            resources.GetString("cbB_B_TxQT.Items62"),
            resources.GetString("cbB_B_TxQT.Items63"),
            resources.GetString("cbB_B_TxQT.Items64"),
            resources.GetString("cbB_B_TxQT.Items65"),
            resources.GetString("cbB_B_TxQT.Items66"),
            resources.GetString("cbB_B_TxQT.Items67"),
            resources.GetString("cbB_B_TxQT.Items68"),
            resources.GetString("cbB_B_TxQT.Items69"),
            resources.GetString("cbB_B_TxQT.Items70"),
            resources.GetString("cbB_B_TxQT.Items71"),
            resources.GetString("cbB_B_TxQT.Items72"),
            resources.GetString("cbB_B_TxQT.Items73"),
            resources.GetString("cbB_B_TxQT.Items74"),
            resources.GetString("cbB_B_TxQT.Items75"),
            resources.GetString("cbB_B_TxQT.Items76"),
            resources.GetString("cbB_B_TxQT.Items77"),
            resources.GetString("cbB_B_TxQT.Items78"),
            resources.GetString("cbB_B_TxQT.Items79"),
            resources.GetString("cbB_B_TxQT.Items80"),
            resources.GetString("cbB_B_TxQT.Items81"),
            resources.GetString("cbB_B_TxQT.Items82"),
            resources.GetString("cbB_B_TxQT.Items83"),
            resources.GetString("cbB_B_TxQT.Items84"),
            resources.GetString("cbB_B_TxQT.Items85"),
            resources.GetString("cbB_B_TxQT.Items86"),
            resources.GetString("cbB_B_TxQT.Items87"),
            resources.GetString("cbB_B_TxQT.Items88"),
            resources.GetString("cbB_B_TxQT.Items89"),
            resources.GetString("cbB_B_TxQT.Items90"),
            resources.GetString("cbB_B_TxQT.Items91"),
            resources.GetString("cbB_B_TxQT.Items92"),
            resources.GetString("cbB_B_TxQT.Items93"),
            resources.GetString("cbB_B_TxQT.Items94"),
            resources.GetString("cbB_B_TxQT.Items95"),
            resources.GetString("cbB_B_TxQT.Items96"),
            resources.GetString("cbB_B_TxQT.Items97"),
            resources.GetString("cbB_B_TxQT.Items98"),
            resources.GetString("cbB_B_TxQT.Items99"),
            resources.GetString("cbB_B_TxQT.Items100"),
            resources.GetString("cbB_B_TxQT.Items101"),
            resources.GetString("cbB_B_TxQT.Items102"),
            resources.GetString("cbB_B_TxQT.Items103"),
            resources.GetString("cbB_B_TxQT.Items104"),
            resources.GetString("cbB_B_TxQT.Items105"),
            resources.GetString("cbB_B_TxQT.Items106"),
            resources.GetString("cbB_B_TxQT.Items107"),
            resources.GetString("cbB_B_TxQT.Items108"),
            resources.GetString("cbB_B_TxQT.Items109"),
            resources.GetString("cbB_B_TxQT.Items110"),
            resources.GetString("cbB_B_TxQT.Items111"),
            resources.GetString("cbB_B_TxQT.Items112"),
            resources.GetString("cbB_B_TxQT.Items113"),
            resources.GetString("cbB_B_TxQT.Items114"),
            resources.GetString("cbB_B_TxQT.Items115"),
            resources.GetString("cbB_B_TxQT.Items116"),
            resources.GetString("cbB_B_TxQT.Items117"),
            resources.GetString("cbB_B_TxQT.Items118"),
            resources.GetString("cbB_B_TxQT.Items119"),
            resources.GetString("cbB_B_TxQT.Items120"),
            resources.GetString("cbB_B_TxQT.Items121"),
            resources.GetString("cbB_B_TxQT.Items122"),
            resources.GetString("cbB_B_TxQT.Items123"),
            resources.GetString("cbB_B_TxQT.Items124"),
            resources.GetString("cbB_B_TxQT.Items125"),
            resources.GetString("cbB_B_TxQT.Items126"),
            resources.GetString("cbB_B_TxQT.Items127"),
            resources.GetString("cbB_B_TxQT.Items128"),
            resources.GetString("cbB_B_TxQT.Items129"),
            resources.GetString("cbB_B_TxQT.Items130"),
            resources.GetString("cbB_B_TxQT.Items131"),
            resources.GetString("cbB_B_TxQT.Items132"),
            resources.GetString("cbB_B_TxQT.Items133"),
            resources.GetString("cbB_B_TxQT.Items134"),
            resources.GetString("cbB_B_TxQT.Items135"),
            resources.GetString("cbB_B_TxQT.Items136"),
            resources.GetString("cbB_B_TxQT.Items137"),
            resources.GetString("cbB_B_TxQT.Items138"),
            resources.GetString("cbB_B_TxQT.Items139"),
            resources.GetString("cbB_B_TxQT.Items140"),
            resources.GetString("cbB_B_TxQT.Items141"),
            resources.GetString("cbB_B_TxQT.Items142"),
            resources.GetString("cbB_B_TxQT.Items143"),
            resources.GetString("cbB_B_TxQT.Items144"),
            resources.GetString("cbB_B_TxQT.Items145"),
            resources.GetString("cbB_B_TxQT.Items146"),
            resources.GetString("cbB_B_TxQT.Items147"),
            resources.GetString("cbB_B_TxQT.Items148"),
            resources.GetString("cbB_B_TxQT.Items149"),
            resources.GetString("cbB_B_TxQT.Items150"),
            resources.GetString("cbB_B_TxQT.Items151"),
            resources.GetString("cbB_B_TxQT.Items152"),
            resources.GetString("cbB_B_TxQT.Items153"),
            resources.GetString("cbB_B_TxQT.Items154"),
            resources.GetString("cbB_B_TxQT.Items155"),
            resources.GetString("cbB_B_TxQT.Items156"),
            resources.GetString("cbB_B_TxQT.Items157"),
            resources.GetString("cbB_B_TxQT.Items158"),
            resources.GetString("cbB_B_TxQT.Items159"),
            resources.GetString("cbB_B_TxQT.Items160"),
            resources.GetString("cbB_B_TxQT.Items161"),
            resources.GetString("cbB_B_TxQT.Items162"),
            resources.GetString("cbB_B_TxQT.Items163"),
            resources.GetString("cbB_B_TxQT.Items164"),
            resources.GetString("cbB_B_TxQT.Items165"),
            resources.GetString("cbB_B_TxQT.Items166"),
            resources.GetString("cbB_B_TxQT.Items167"),
            resources.GetString("cbB_B_TxQT.Items168"),
            resources.GetString("cbB_B_TxQT.Items169"),
            resources.GetString("cbB_B_TxQT.Items170"),
            resources.GetString("cbB_B_TxQT.Items171"),
            resources.GetString("cbB_B_TxQT.Items172"),
            resources.GetString("cbB_B_TxQT.Items173"),
            resources.GetString("cbB_B_TxQT.Items174"),
            resources.GetString("cbB_B_TxQT.Items175"),
            resources.GetString("cbB_B_TxQT.Items176"),
            resources.GetString("cbB_B_TxQT.Items177"),
            resources.GetString("cbB_B_TxQT.Items178"),
            resources.GetString("cbB_B_TxQT.Items179"),
            resources.GetString("cbB_B_TxQT.Items180"),
            resources.GetString("cbB_B_TxQT.Items181"),
            resources.GetString("cbB_B_TxQT.Items182"),
            resources.GetString("cbB_B_TxQT.Items183"),
            resources.GetString("cbB_B_TxQT.Items184"),
            resources.GetString("cbB_B_TxQT.Items185"),
            resources.GetString("cbB_B_TxQT.Items186"),
            resources.GetString("cbB_B_TxQT.Items187"),
            resources.GetString("cbB_B_TxQT.Items188"),
            resources.GetString("cbB_B_TxQT.Items189"),
            resources.GetString("cbB_B_TxQT.Items190"),
            resources.GetString("cbB_B_TxQT.Items191"),
            resources.GetString("cbB_B_TxQT.Items192"),
            resources.GetString("cbB_B_TxQT.Items193"),
            resources.GetString("cbB_B_TxQT.Items194"),
            resources.GetString("cbB_B_TxQT.Items195"),
            resources.GetString("cbB_B_TxQT.Items196"),
            resources.GetString("cbB_B_TxQT.Items197"),
            resources.GetString("cbB_B_TxQT.Items198"),
            resources.GetString("cbB_B_TxQT.Items199"),
            resources.GetString("cbB_B_TxQT.Items200"),
            resources.GetString("cbB_B_TxQT.Items201"),
            resources.GetString("cbB_B_TxQT.Items202"),
            resources.GetString("cbB_B_TxQT.Items203"),
            resources.GetString("cbB_B_TxQT.Items204"),
            resources.GetString("cbB_B_TxQT.Items205"),
            resources.GetString("cbB_B_TxQT.Items206"),
            resources.GetString("cbB_B_TxQT.Items207"),
            resources.GetString("cbB_B_TxQT.Items208"),
            resources.GetString("cbB_B_TxQT.Items209")
        });
        cbB_B_TxQT.Name = "cbB_B_TxQT";
        cbB_B_TxQT.Leave += cbB_B_TxQT_Leave;
        cbB_B_RxQT.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_B_RxQT.DropDownHeight = 100;
        resources.ApplyResources(cbB_B_RxQT, "cbB_B_RxQT");
        cbB_B_RxQT.Items.AddRange(new object[210]
        {
            resources.GetString("cbB_B_RxQT.Items"),
            resources.GetString("cbB_B_RxQT.Items1"),
            resources.GetString("cbB_B_RxQT.Items2"),
            resources.GetString("cbB_B_RxQT.Items3"),
            resources.GetString("cbB_B_RxQT.Items4"),
            resources.GetString("cbB_B_RxQT.Items5"),
            resources.GetString("cbB_B_RxQT.Items6"),
            resources.GetString("cbB_B_RxQT.Items7"),
            resources.GetString("cbB_B_RxQT.Items8"),
            resources.GetString("cbB_B_RxQT.Items9"),
            resources.GetString("cbB_B_RxQT.Items10"),
            resources.GetString("cbB_B_RxQT.Items11"),
            resources.GetString("cbB_B_RxQT.Items12"),
            resources.GetString("cbB_B_RxQT.Items13"),
            resources.GetString("cbB_B_RxQT.Items14"),
            resources.GetString("cbB_B_RxQT.Items15"),
            resources.GetString("cbB_B_RxQT.Items16"),
            resources.GetString("cbB_B_RxQT.Items17"),
            resources.GetString("cbB_B_RxQT.Items18"),
            resources.GetString("cbB_B_RxQT.Items19"),
            resources.GetString("cbB_B_RxQT.Items20"),
            resources.GetString("cbB_B_RxQT.Items21"),
            resources.GetString("cbB_B_RxQT.Items22"),
            resources.GetString("cbB_B_RxQT.Items23"),
            resources.GetString("cbB_B_RxQT.Items24"),
            resources.GetString("cbB_B_RxQT.Items25"),
            resources.GetString("cbB_B_RxQT.Items26"),
            resources.GetString("cbB_B_RxQT.Items27"),
            resources.GetString("cbB_B_RxQT.Items28"),
            resources.GetString("cbB_B_RxQT.Items29"),
            resources.GetString("cbB_B_RxQT.Items30"),
            resources.GetString("cbB_B_RxQT.Items31"),
            resources.GetString("cbB_B_RxQT.Items32"),
            resources.GetString("cbB_B_RxQT.Items33"),
            resources.GetString("cbB_B_RxQT.Items34"),
            resources.GetString("cbB_B_RxQT.Items35"),
            resources.GetString("cbB_B_RxQT.Items36"),
            resources.GetString("cbB_B_RxQT.Items37"),
            resources.GetString("cbB_B_RxQT.Items38"),
            resources.GetString("cbB_B_RxQT.Items39"),
            resources.GetString("cbB_B_RxQT.Items40"),
            resources.GetString("cbB_B_RxQT.Items41"),
            resources.GetString("cbB_B_RxQT.Items42"),
            resources.GetString("cbB_B_RxQT.Items43"),
            resources.GetString("cbB_B_RxQT.Items44"),
            resources.GetString("cbB_B_RxQT.Items45"),
            resources.GetString("cbB_B_RxQT.Items46"),
            resources.GetString("cbB_B_RxQT.Items47"),
            resources.GetString("cbB_B_RxQT.Items48"),
            resources.GetString("cbB_B_RxQT.Items49"),
            resources.GetString("cbB_B_RxQT.Items50"),
            resources.GetString("cbB_B_RxQT.Items51"),
            resources.GetString("cbB_B_RxQT.Items52"),
            resources.GetString("cbB_B_RxQT.Items53"),
            resources.GetString("cbB_B_RxQT.Items54"),
            resources.GetString("cbB_B_RxQT.Items55"),
            resources.GetString("cbB_B_RxQT.Items56"),
            resources.GetString("cbB_B_RxQT.Items57"),
            resources.GetString("cbB_B_RxQT.Items58"),
            resources.GetString("cbB_B_RxQT.Items59"),
            resources.GetString("cbB_B_RxQT.Items60"),
            resources.GetString("cbB_B_RxQT.Items61"),
            resources.GetString("cbB_B_RxQT.Items62"),
            resources.GetString("cbB_B_RxQT.Items63"),
            resources.GetString("cbB_B_RxQT.Items64"),
            resources.GetString("cbB_B_RxQT.Items65"),
            resources.GetString("cbB_B_RxQT.Items66"),
            resources.GetString("cbB_B_RxQT.Items67"),
            resources.GetString("cbB_B_RxQT.Items68"),
            resources.GetString("cbB_B_RxQT.Items69"),
            resources.GetString("cbB_B_RxQT.Items70"),
            resources.GetString("cbB_B_RxQT.Items71"),
            resources.GetString("cbB_B_RxQT.Items72"),
            resources.GetString("cbB_B_RxQT.Items73"),
            resources.GetString("cbB_B_RxQT.Items74"),
            resources.GetString("cbB_B_RxQT.Items75"),
            resources.GetString("cbB_B_RxQT.Items76"),
            resources.GetString("cbB_B_RxQT.Items77"),
            resources.GetString("cbB_B_RxQT.Items78"),
            resources.GetString("cbB_B_RxQT.Items79"),
            resources.GetString("cbB_B_RxQT.Items80"),
            resources.GetString("cbB_B_RxQT.Items81"),
            resources.GetString("cbB_B_RxQT.Items82"),
            resources.GetString("cbB_B_RxQT.Items83"),
            resources.GetString("cbB_B_RxQT.Items84"),
            resources.GetString("cbB_B_RxQT.Items85"),
            resources.GetString("cbB_B_RxQT.Items86"),
            resources.GetString("cbB_B_RxQT.Items87"),
            resources.GetString("cbB_B_RxQT.Items88"),
            resources.GetString("cbB_B_RxQT.Items89"),
            resources.GetString("cbB_B_RxQT.Items90"),
            resources.GetString("cbB_B_RxQT.Items91"),
            resources.GetString("cbB_B_RxQT.Items92"),
            resources.GetString("cbB_B_RxQT.Items93"),
            resources.GetString("cbB_B_RxQT.Items94"),
            resources.GetString("cbB_B_RxQT.Items95"),
            resources.GetString("cbB_B_RxQT.Items96"),
            resources.GetString("cbB_B_RxQT.Items97"),
            resources.GetString("cbB_B_RxQT.Items98"),
            resources.GetString("cbB_B_RxQT.Items99"),
            resources.GetString("cbB_B_RxQT.Items100"),
            resources.GetString("cbB_B_RxQT.Items101"),
            resources.GetString("cbB_B_RxQT.Items102"),
            resources.GetString("cbB_B_RxQT.Items103"),
            resources.GetString("cbB_B_RxQT.Items104"),
            resources.GetString("cbB_B_RxQT.Items105"),
            resources.GetString("cbB_B_RxQT.Items106"),
            resources.GetString("cbB_B_RxQT.Items107"),
            resources.GetString("cbB_B_RxQT.Items108"),
            resources.GetString("cbB_B_RxQT.Items109"),
            resources.GetString("cbB_B_RxQT.Items110"),
            resources.GetString("cbB_B_RxQT.Items111"),
            resources.GetString("cbB_B_RxQT.Items112"),
            resources.GetString("cbB_B_RxQT.Items113"),
            resources.GetString("cbB_B_RxQT.Items114"),
            resources.GetString("cbB_B_RxQT.Items115"),
            resources.GetString("cbB_B_RxQT.Items116"),
            resources.GetString("cbB_B_RxQT.Items117"),
            resources.GetString("cbB_B_RxQT.Items118"),
            resources.GetString("cbB_B_RxQT.Items119"),
            resources.GetString("cbB_B_RxQT.Items120"),
            resources.GetString("cbB_B_RxQT.Items121"),
            resources.GetString("cbB_B_RxQT.Items122"),
            resources.GetString("cbB_B_RxQT.Items123"),
            resources.GetString("cbB_B_RxQT.Items124"),
            resources.GetString("cbB_B_RxQT.Items125"),
            resources.GetString("cbB_B_RxQT.Items126"),
            resources.GetString("cbB_B_RxQT.Items127"),
            resources.GetString("cbB_B_RxQT.Items128"),
            resources.GetString("cbB_B_RxQT.Items129"),
            resources.GetString("cbB_B_RxQT.Items130"),
            resources.GetString("cbB_B_RxQT.Items131"),
            resources.GetString("cbB_B_RxQT.Items132"),
            resources.GetString("cbB_B_RxQT.Items133"),
            resources.GetString("cbB_B_RxQT.Items134"),
            resources.GetString("cbB_B_RxQT.Items135"),
            resources.GetString("cbB_B_RxQT.Items136"),
            resources.GetString("cbB_B_RxQT.Items137"),
            resources.GetString("cbB_B_RxQT.Items138"),
            resources.GetString("cbB_B_RxQT.Items139"),
            resources.GetString("cbB_B_RxQT.Items140"),
            resources.GetString("cbB_B_RxQT.Items141"),
            resources.GetString("cbB_B_RxQT.Items142"),
            resources.GetString("cbB_B_RxQT.Items143"),
            resources.GetString("cbB_B_RxQT.Items144"),
            resources.GetString("cbB_B_RxQT.Items145"),
            resources.GetString("cbB_B_RxQT.Items146"),
            resources.GetString("cbB_B_RxQT.Items147"),
            resources.GetString("cbB_B_RxQT.Items148"),
            resources.GetString("cbB_B_RxQT.Items149"),
            resources.GetString("cbB_B_RxQT.Items150"),
            resources.GetString("cbB_B_RxQT.Items151"),
            resources.GetString("cbB_B_RxQT.Items152"),
            resources.GetString("cbB_B_RxQT.Items153"),
            resources.GetString("cbB_B_RxQT.Items154"),
            resources.GetString("cbB_B_RxQT.Items155"),
            resources.GetString("cbB_B_RxQT.Items156"),
            resources.GetString("cbB_B_RxQT.Items157"),
            resources.GetString("cbB_B_RxQT.Items158"),
            resources.GetString("cbB_B_RxQT.Items159"),
            resources.GetString("cbB_B_RxQT.Items160"),
            resources.GetString("cbB_B_RxQT.Items161"),
            resources.GetString("cbB_B_RxQT.Items162"),
            resources.GetString("cbB_B_RxQT.Items163"),
            resources.GetString("cbB_B_RxQT.Items164"),
            resources.GetString("cbB_B_RxQT.Items165"),
            resources.GetString("cbB_B_RxQT.Items166"),
            resources.GetString("cbB_B_RxQT.Items167"),
            resources.GetString("cbB_B_RxQT.Items168"),
            resources.GetString("cbB_B_RxQT.Items169"),
            resources.GetString("cbB_B_RxQT.Items170"),
            resources.GetString("cbB_B_RxQT.Items171"),
            resources.GetString("cbB_B_RxQT.Items172"),
            resources.GetString("cbB_B_RxQT.Items173"),
            resources.GetString("cbB_B_RxQT.Items174"),
            resources.GetString("cbB_B_RxQT.Items175"),
            resources.GetString("cbB_B_RxQT.Items176"),
            resources.GetString("cbB_B_RxQT.Items177"),
            resources.GetString("cbB_B_RxQT.Items178"),
            resources.GetString("cbB_B_RxQT.Items179"),
            resources.GetString("cbB_B_RxQT.Items180"),
            resources.GetString("cbB_B_RxQT.Items181"),
            resources.GetString("cbB_B_RxQT.Items182"),
            resources.GetString("cbB_B_RxQT.Items183"),
            resources.GetString("cbB_B_RxQT.Items184"),
            resources.GetString("cbB_B_RxQT.Items185"),
            resources.GetString("cbB_B_RxQT.Items186"),
            resources.GetString("cbB_B_RxQT.Items187"),
            resources.GetString("cbB_B_RxQT.Items188"),
            resources.GetString("cbB_B_RxQT.Items189"),
            resources.GetString("cbB_B_RxQT.Items190"),
            resources.GetString("cbB_B_RxQT.Items191"),
            resources.GetString("cbB_B_RxQT.Items192"),
            resources.GetString("cbB_B_RxQT.Items193"),
            resources.GetString("cbB_B_RxQT.Items194"),
            resources.GetString("cbB_B_RxQT.Items195"),
            resources.GetString("cbB_B_RxQT.Items196"),
            resources.GetString("cbB_B_RxQT.Items197"),
            resources.GetString("cbB_B_RxQT.Items198"),
            resources.GetString("cbB_B_RxQT.Items199"),
            resources.GetString("cbB_B_RxQT.Items200"),
            resources.GetString("cbB_B_RxQT.Items201"),
            resources.GetString("cbB_B_RxQT.Items202"),
            resources.GetString("cbB_B_RxQT.Items203"),
            resources.GetString("cbB_B_RxQT.Items204"),
            resources.GetString("cbB_B_RxQT.Items205"),
            resources.GetString("cbB_B_RxQT.Items206"),
            resources.GetString("cbB_B_RxQT.Items207"),
            resources.GetString("cbB_B_RxQT.Items208"),
            resources.GetString("cbB_B_RxQT.Items209")
        });
        cbB_B_RxQT.Name = "cbB_B_RxQT";
        cbB_B_RxQT.Leave += cbB_B_RxQT_Leave;
        cbB_B_Power.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_B_Power.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(cbB_B_Power, "cbB_B_Power");
        cbB_B_Power.Items.AddRange(new object[2]
        {
            resources.GetString("cbB_B_Power.Items"),
            resources.GetString("cbB_B_Power.Items1")
        });
        cbB_B_Power.Name = "cbB_B_Power";
        cbB_B_Power.KeyPress += cbB_KeyPress;
        resources.ApplyResources(label_B_SignalingEnCoder, "label_B_SignalingEnCoder");
        label_B_SignalingEnCoder.Name = "label_B_SignalingEnCoder";
        resources.ApplyResources(label_B_RemainFreq, "label_B_RemainFreq");
        label_B_RemainFreq.Name = "label_B_RemainFreq";
        resources.ApplyResources(label_B_RemainDir, "label_B_RemainDir");
        label_B_RemainDir.Name = "label_B_RemainDir";
        resources.ApplyResources(label_B_FreqStep, "label_B_FreqStep");
        label_B_FreqStep.Name = "label_B_FreqStep";
        resources.ApplyResources(label_B_CHBand, "label_B_CHBand");
        label_B_CHBand.Name = "label_B_CHBand";
        resources.ApplyResources(label_B_TxQT, "label_B_TxQT");
        label_B_TxQT.Name = "label_B_TxQT";
        resources.ApplyResources(label_B_RxQT, "label_B_RxQT");
        label_B_RxQT.Name = "label_B_RxQT";
        resources.ApplyResources(label_B_Power, "label_B_Power");
        label_B_Power.Name = "label_B_Power";
        resources.ApplyResources(label_B_CurFreq, "label_B_CurFreq");
        label_B_CurFreq.Name = "label_B_CurFreq";
        resources.ApplyResources(btn_Default, "btn_Default");
        btn_Default.Name = "btn_Default";
        btn_Default.UseVisualStyleBackColor = true;
        btn_Default.Click += btn_Default_Click;
        resources.ApplyResources(btn_Close, "btn_Close");
        btn_Close.Name = "btn_Close";
        btn_Close.UseVisualStyleBackColor = true;
        btn_Close.Click += btn_Close_Click;
        cbB_1750Hz.DisplayMember = "Text";
        cbB_1750Hz.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_1750Hz.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_1750Hz.FormattingEnabled = true;
        resources.ApplyResources(cbB_1750Hz, "cbB_1750Hz");
        cbB_1750Hz.Items.AddRange(new object[4]
        {
            resources.GetString("cbB_1750Hz.Items"),
            resources.GetString("cbB_1750Hz.Items1"),
            resources.GetString("cbB_1750Hz.Items2"),
            resources.GetString("cbB_1750Hz.Items3")
        });
        cbB_1750Hz.Name = "cbB_1750Hz";
        cbB_1750Hz.Style = eDotNetBarStyle.StyleManagerControlled;
        resources.ApplyResources(label_1750Hz, "label_1750Hz");
        label_1750Hz.Name = "label_1750Hz";
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(btn_Close);
        Controls.Add(btn_Default);
        Controls.Add(gB_FreqMode_B);
        Controls.Add(gB_FreqMode_A);
        Controls.Add(cbB_Language);
        Controls.Add(label_Language);
        Controls.Add(groupBox4);
        Controls.Add(groupBox1);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "FormFunConfig";
        FormClosing += FormFunConfig_FormClosing;
        KeyPress += FormFunConfig_KeyPress;
        groupBox1.ResumeLayout(false);
        groupBox4.ResumeLayout(false);
        gB_FreqMode_A.ResumeLayout(false);
        gB_FreqMode_A.PerformLayout();
        gB_FreqMode_B.ResumeLayout(false);
        gB_FreqMode_B.PerformLayout();
        ResumeLayout(false);
    }
}