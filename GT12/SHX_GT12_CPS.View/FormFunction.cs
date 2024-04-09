using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SHX_GT12_CPS.View;

public class FormFunction : Form
{
    private readonly IContainer components = null;
    private ComboBox cbB_A_DisType;

    private ComboBox cbB_A_Workmode;

    private ComboBox cbB_AutoLock;

    private ComboBox cbB_B_DisType;

    private ComboBox cbB_B_Workmode;

    private ComboBox cbB_BacklightTime;

    private ComboBox cbB_Beep;

    private ComboBox cbB_Bright;

    private ComboBox cbB_CurBank;

    private ComboBox cbB_FM;

    private ComboBox cbB_IDDlyTime;

    private ComboBox cbB_Key1_L;

    private ComboBox cbB_Key1_S;

    private ComboBox cbB_Key2_L;

    private ComboBox cbB_Key2_S;

    private ComboBox cbB_KeyLock;

    private ComboBox cbB_MenuAutoQuit;

    private ComboBox cbB_MicGain;

    private ComboBox cbB_PowerOnDisType;

    private ComboBox cbB_PowerUpDisTime;

    private ComboBox cbB_Roger;

    private ComboBox cbB_RptClearTail;

    private ComboBox cbB_RptDetectTail;

    private ComboBox cbB_SaveMode;

    private ComboBox cbB_ScanMode;

    private ComboBox cbB_SideTone;

    private ComboBox cbB_SOSMode;

    private ComboBox cbB_SQL;

    private ComboBox cbB_TailClear;

    private ComboBox cbB_TDR;

    private ComboBox cbB_Tone;

    private ComboBox cbB_TOT;

    private ComboBox cbB_Vox;

    private ComboBox cbB_VoxDly;

    private ComboBox cbB_VoxSw;

    private ComboBox cbBBluetoothAudioGain;
    private Function funs;

    private Label label_A_DisType;

    private Label label_A_Workmode;

    private Label label_AutoLock;

    private Label label_B_DisType;

    private Label label_B_Workmode;

    private Label label_BacklightTime;

    private Label label_Beep;

    private Label label_Bright;

    private Label label_CurBank;

    private Label label_FM;

    private Label label_IDDlyTime;

    private Label label_Key1_L;

    private Label label_Key1_S;

    private Label label_Key2_L;

    private Label label_Key2_S;

    private Label label_KeyLock;

    private Label label_MenuAutoQuit;

    private Label label_MicGain;

    private Label label_PowerOnDisType;

    private Label label_Roger;

    private Label label_RptClearTail;

    private Label label_RptDetectTail;

    private Label label_Save;

    private Label label_ScanMode;

    private Label label_SideTone;

    private Label label_SQL;

    private Label label_TailClear;

    private Label label_TDR;

    private Label label_Tone;

    private Label label_TOT;

    private Label label_Vox;

    private Label label_VoxDlyTime;

    private Label label1;

    private Label label1_SOSMode;

    private Label label2;

    private Label label3;

    private Label label5;

    private Label label6;

    private MDC1200 mdcs;

    private string prevCallSign;

    private TextBox tB_SDCID;

    private TextBox tBCallSign;

    public FormFunction(Form parent)
    {
        InitializeComponent();
        for (var i = 0; i < 30; i++) cbB_CurBank.Items.Add((i + 1).ToString());

        MdiParent = parent;
    }

    private void FormFunction_Load(object sender, EventArgs e)
    {
    }

    public void LoadData(Function funs, MDC1200 mdcs)
    {
        this.funs = funs;
        this.mdcs = mdcs;
        BindingControls();
    }

    private void TryToBingdingControl(Control c, string propertyName, object dataSource, string dataMember,
        object defaultVal)
    {
        if (c.DataBindings.Count != 0) c.DataBindings.Clear();

        try
        {
            c.DataBindings.Add(new Binding(propertyName, dataSource, dataMember, false,
                DataSourceUpdateMode.OnPropertyChanged));
        }
        catch
        {
            funs.GetType().GetProperty(dataMember).SetValue(funs, defaultVal, null);
            c.DataBindings.Add(new Binding(propertyName, dataSource, dataMember, false,
                DataSourceUpdateMode.OnPropertyChanged));
        }
    }

    private void BindingControls()
    {
        TryToBingdingControl(cbB_SQL, "SelectedIndex", funs, "Sql", 3);
        TryToBingdingControl(cbB_SaveMode, "SelectedIndex", funs, "SaveMode", 1);
        TryToBingdingControl(cbB_VoxSw, "SelectedIndex", funs, "VoxSw", 0);
        TryToBingdingControl(cbB_Vox, "SelectedIndex", funs, "Vox", 0);
        TryToBingdingControl(cbB_VoxDly, "SelectedIndex", funs, "VoxDlyTime", 5);
        TryToBingdingControl(cbB_TDR, "SelectedIndex", funs, "DualStandby", 0);
        TryToBingdingControl(cbB_TOT, "SelectedIndex", funs, "Tot", 2);
        TryToBingdingControl(cbB_Beep, "SelectedIndex", funs, "Beep", 3);
        TryToBingdingControl(cbB_SideTone, "SelectedIndex", funs, "SideTone", 0);
        TryToBingdingControl(cbB_ScanMode, "SelectedIndex", funs, "ScanMode", 1);
        TryToBingdingControl(cbB_IDDlyTime, "SelectedIndex", funs, "PttDly", 6);
        TryToBingdingControl(cbB_A_DisType, "SelectedIndex", funs, "ChADisType", 0);
        TryToBingdingControl(cbB_B_DisType, "SelectedIndex", funs, "ChBDisType", 0);
        TryToBingdingControl(cbB_AutoLock, "SelectedIndex", funs, "AutoLock", 2);
        TryToBingdingControl(cbB_MicGain, "SelectedIndex", funs, "MicGain", 1);
        TryToBingdingControl(cbB_SOSMode, "SelectedIndex", funs, "AlarmMode", 0);
        TryToBingdingControl(cbB_TailClear, "SelectedIndex", funs, "TailClear", 1);
        TryToBingdingControl(cbB_RptClearTail, "SelectedIndex", funs, "RptTailClear", 5);
        TryToBingdingControl(cbB_RptDetectTail, "SelectedIndex", funs, "RptTailDet", 5);
        TryToBingdingControl(cbB_Roger, "SelectedIndex", funs, "Roger", 0);
        TryToBingdingControl(cbB_FM, "SelectedIndex", funs, "FmEnable", 0);
        TryToBingdingControl(cbB_A_Workmode, "SelectedIndex", funs, "ChAWorkmode", 0);
        TryToBingdingControl(cbB_B_Workmode, "SelectedIndex", funs, "ChBWorkmode", 0);
        TryToBingdingControl(cbB_KeyLock, "SelectedIndex", funs, "KeyLock", 0);
        TryToBingdingControl(cbB_PowerOnDisType, "SelectedIndex", funs, "PowerOnDisType", 0);
        TryToBingdingControl(cbB_Tone, "SelectedIndex", funs, "Tone", 2);
        TryToBingdingControl(cbB_BacklightTime, "SelectedIndex", funs, "Backlight", 5);
        TryToBingdingControl(cbB_MenuAutoQuit, "SelectedIndex", funs, "MenuQuitTime", 1);
        TryToBingdingControl(cbB_Key1_L, "SelectedIndex", funs, "Key1Long", 0);
        TryToBingdingControl(cbB_Key1_S, "SelectedIndex", funs, "Key1Short", 1);
        TryToBingdingControl(cbB_Key2_L, "SelectedIndex", funs, "Key2Long", 3);
        TryToBingdingControl(cbB_Key2_S, "SelectedIndex", funs, "Key2Short", 2);
        TryToBingdingControl(cbB_Bright, "SelectedIndex", funs, "Bright", 3);
        TryToBingdingControl(cbB_CurBank, "SelectedIndex", funs, "CurBank", 0);
        TryToBingdingControl(cbB_PowerUpDisTime, "SelectedIndex", funs, "PowerUpDisTime", 6);
        TryToBingdingControl(cbBBluetoothAudioGain, "SelectedIndex", funs, "BluetoothAudioGain", 2);
        TryToBingdingControl(tBCallSign, "Text", funs, "CallSign", "");
        TryToBingdingControl(tB_SDCID, "Text", mdcs, "Id", "1111");
    }

    private void FormFunction_FormClosing(object sender, FormClosingEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
        e.KeyChar = char.ToUpper(e.KeyChar);
        if (e.KeyChar != '\b')
        {
            if (tBCallSign.Text.Length >= 6)
                e.Handled = true;
            else if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar >= 'A' && e.KeyChar <= 'Z') ||
                     (e.KeyChar >= 'a' && e.KeyChar <= 'z'))
                prevCallSign = tBCallSign.Text;
            else
                e.Handled = true;
        }
    }

    private void tBCallSign_TextChanged(object sender, EventArgs e)
    {
    }

    private void tB_SDCID_KeyPress(object sender, KeyPressEventArgs e)
    {
        e.KeyChar = char.ToUpper(e.KeyChar);
        if ((e.KeyChar < '0' || e.KeyChar > '9') && (e.KeyChar < 'A' || e.KeyChar > 'F') && e.KeyChar != '\b')
            e.Handled = true;
    }

    private void cbBBluetoothAudioGain_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var resources =
            new ComponentResourceManager(typeof(FormFunction));
        label_SQL = new Label();
        label_Save = new Label();
        label_Vox = new Label();
        label_VoxDlyTime = new Label();
        label_TDR = new Label();
        label_TOT = new Label();
        label_Beep = new Label();
        label_SideTone = new Label();
        label_ScanMode = new Label();
        label_IDDlyTime = new Label();
        label_A_DisType = new Label();
        label_B_DisType = new Label();
        label_AutoLock = new Label();
        label_MicGain = new Label();
        label1_SOSMode = new Label();
        label_TailClear = new Label();
        label_RptClearTail = new Label();
        label_RptDetectTail = new Label();
        label_Roger = new Label();
        label_FM = new Label();
        label_A_Workmode = new Label();
        label_B_Workmode = new Label();
        label_KeyLock = new Label();
        label_PowerOnDisType = new Label();
        label_Tone = new Label();
        label_BacklightTime = new Label();
        label_MenuAutoQuit = new Label();
        label_Key1_S = new Label();
        label_Key1_L = new Label();
        label_Key2_S = new Label();
        label_Key2_L = new Label();
        label_Bright = new Label();
        cbB_SQL = new ComboBox();
        cbB_TOT = new ComboBox();
        cbB_SaveMode = new ComboBox();
        cbB_Vox = new ComboBox();
        cbB_VoxDly = new ComboBox();
        cbB_TDR = new ComboBox();
        cbB_Tone = new ComboBox();
        cbB_SideTone = new ComboBox();
        cbB_TailClear = new ComboBox();
        cbB_PowerOnDisType = new ComboBox();
        cbB_Beep = new ComboBox();
        cbB_Roger = new ComboBox();
        cbB_MicGain = new ComboBox();
        cbB_ScanMode = new ComboBox();
        cbB_SOSMode = new ComboBox();
        cbB_KeyLock = new ComboBox();
        cbB_FM = new ComboBox();
        cbB_AutoLock = new ComboBox();
        cbB_MenuAutoQuit = new ComboBox();
        cbB_BacklightTime = new ComboBox();
        cbB_Bright = new ComboBox();
        cbB_IDDlyTime = new ComboBox();
        cbB_A_DisType = new ComboBox();
        cbB_A_Workmode = new ComboBox();
        cbB_B_DisType = new ComboBox();
        cbB_B_Workmode = new ComboBox();
        cbB_Key1_S = new ComboBox();
        cbB_Key1_L = new ComboBox();
        cbB_Key2_S = new ComboBox();
        cbB_Key2_L = new ComboBox();
        cbB_RptClearTail = new ComboBox();
        cbB_RptDetectTail = new ComboBox();
        cbB_CurBank = new ComboBox();
        label_CurBank = new Label();
        label1 = new Label();
        cbB_VoxSw = new ComboBox();
        label2 = new Label();
        cbB_PowerUpDisTime = new ComboBox();
        label5 = new Label();
        cbBBluetoothAudioGain = new ComboBox();
        label6 = new Label();
        tBCallSign = new TextBox();
        label3 = new Label();
        tB_SDCID = new TextBox();
        SuspendLayout();
        label_SQL.Location = new Point(53, 27);
        label_SQL.Name = "label_SQL";
        label_SQL.Size = new Size(115, 28);
        label_SQL.TabIndex = 0;
        label_SQL.Text = "静噪等级";
        label_SQL.TextAlign = ContentAlignment.MiddleRight;
        label_Save.Location = new Point(53, 98);
        label_Save.Name = "label_Save";
        label_Save.Size = new Size(115, 28);
        label_Save.TabIndex = 1;
        label_Save.Text = "省电模式";
        label_Save.TextAlign = ContentAlignment.MiddleRight;
        label_Vox.Location = new Point(53, 171);
        label_Vox.Name = "label_Vox";
        label_Vox.Size = new Size(115, 28);
        label_Vox.TabIndex = 2;
        label_Vox.Text = "声控等级";
        label_Vox.TextAlign = ContentAlignment.MiddleRight;
        label_VoxDlyTime.Location = new Point(53, 205);
        label_VoxDlyTime.Name = "label_VoxDlyTime";
        label_VoxDlyTime.Size = new Size(115, 28);
        label_VoxDlyTime.TabIndex = 3;
        label_VoxDlyTime.Text = "声控延时时间";
        label_VoxDlyTime.TextAlign = ContentAlignment.MiddleRight;
        label_TDR.Location = new Point(53, 241);
        label_TDR.Name = "label_TDR";
        label_TDR.Size = new Size(115, 28);
        label_TDR.TabIndex = 4;
        label_TDR.Text = "双守功能";
        label_TDR.TextAlign = ContentAlignment.MiddleRight;
        label_TOT.Location = new Point(53, 63);
        label_TOT.Name = "label_TOT";
        label_TOT.Size = new Size(115, 28);
        label_TOT.TabIndex = 5;
        label_TOT.Text = "发射超时";
        label_TOT.TextAlign = ContentAlignment.MiddleRight;
        label_Beep.Location = new Point(53, 421);
        label_Beep.Name = "label_Beep";
        label_Beep.Size = new Size(115, 28);
        label_Beep.TabIndex = 6;
        label_Beep.Text = "提示音";
        label_Beep.TextAlign = ContentAlignment.MiddleRight;
        label_SideTone.Location = new Point(53, 312);
        label_SideTone.Name = "label_SideTone";
        label_SideTone.Size = new Size(115, 28);
        label_SideTone.TabIndex = 7;
        label_SideTone.Text = "侧音开关";
        label_SideTone.TextAlign = ContentAlignment.MiddleRight;
        label_ScanMode.Location = new Point(325, 26);
        label_ScanMode.Name = "label_ScanMode";
        label_ScanMode.Size = new Size(115, 28);
        label_ScanMode.TabIndex = 8;
        label_ScanMode.Text = "扫描模式";
        label_ScanMode.TextAlign = ContentAlignment.MiddleRight;
        label_IDDlyTime.Location = new Point(325, 356);
        label_IDDlyTime.Name = "label_IDDlyTime";
        label_IDDlyTime.Size = new Size(115, 28);
        label_IDDlyTime.TabIndex = 9;
        label_IDDlyTime.Text = "发码延迟时间";
        label_IDDlyTime.TextAlign = ContentAlignment.MiddleRight;
        label_A_DisType.Location = new Point(591, 28);
        label_A_DisType.Name = "label_A_DisType";
        label_A_DisType.Size = new Size(115, 28);
        label_A_DisType.TabIndex = 10;
        label_A_DisType.Text = "A段显示方式";
        label_A_DisType.TextAlign = ContentAlignment.MiddleRight;
        label_B_DisType.Location = new Point(591, 100);
        label_B_DisType.Name = "label_B_DisType";
        label_B_DisType.Size = new Size(115, 28);
        label_B_DisType.TabIndex = 11;
        label_B_DisType.Text = "B段显示方式";
        label_B_DisType.TextAlign = ContentAlignment.MiddleRight;
        label_AutoLock.Location = new Point(297, 211);
        label_AutoLock.Name = "label_AutoLock";
        label_AutoLock.Size = new Size(143, 28);
        label_AutoLock.TabIndex = 13;
        label_AutoLock.Text = "键盘自动锁定时间";
        label_AutoLock.TextAlign = ContentAlignment.MiddleRight;
        label_MicGain.Location = new Point(53, 458);
        label_MicGain.Name = "label_MicGain";
        label_MicGain.Size = new Size(115, 28);
        label_MicGain.TabIndex = 14;
        label_MicGain.Text = "麦克风增益";
        label_MicGain.TextAlign = ContentAlignment.MiddleRight;
        label1_SOSMode.Location = new Point(325, 61);
        label1_SOSMode.Name = "label1_SOSMode";
        label1_SOSMode.Size = new Size(115, 28);
        label1_SOSMode.TabIndex = 15;
        label1_SOSMode.Text = "报警模式";
        label1_SOSMode.TextAlign = ContentAlignment.MiddleRight;
        label_TailClear.Location = new Point(53, 349);
        label_TailClear.Name = "label_TailClear";
        label_TailClear.Size = new Size(115, 28);
        label_TailClear.TabIndex = 16;
        label_TailClear.Text = "尾音消除";
        label_TailClear.TextAlign = ContentAlignment.MiddleRight;
        label_RptClearTail.Location = new Point(584, 315);
        label_RptClearTail.Name = "label_RptClearTail";
        label_RptClearTail.Size = new Size(123, 28);
        label_RptClearTail.TabIndex = 17;
        label_RptClearTail.Text = "过中继尾音消除";
        label_RptClearTail.TextAlign = ContentAlignment.MiddleRight;
        label_RptDetectTail.Location = new Point(581, 355);
        label_RptDetectTail.Name = "label_RptDetectTail";
        label_RptDetectTail.Size = new Size(125, 28);
        label_RptDetectTail.TabIndex = 18;
        label_RptDetectTail.Text = "过中继尾音检测";
        label_RptDetectTail.TextAlign = ContentAlignment.MiddleRight;
        label_Roger.Location = new Point(312, 390);
        label_Roger.Name = "label_Roger";
        label_Roger.Size = new Size(128, 28);
        label_Roger.TabIndex = 19;
        label_Roger.Text = "发射结束提示音";
        label_Roger.TextAlign = ContentAlignment.MiddleRight;
        label_FM.Location = new Point(325, 132);
        label_FM.Name = "label_FM";
        label_FM.Size = new Size(115, 28);
        label_FM.TabIndex = 21;
        label_FM.Text = "FM收音机";
        label_FM.TextAlign = ContentAlignment.MiddleRight;
        label_A_Workmode.Location = new Point(591, 64);
        label_A_Workmode.Name = "label_A_Workmode";
        label_A_Workmode.Size = new Size(115, 28);
        label_A_Workmode.TabIndex = 22;
        label_A_Workmode.Text = "A段工作方式";
        label_A_Workmode.TextAlign = ContentAlignment.MiddleRight;
        label_B_Workmode.Location = new Point(591, 136);
        label_B_Workmode.Name = "label_B_Workmode";
        label_B_Workmode.Size = new Size(115, 28);
        label_B_Workmode.TabIndex = 23;
        label_B_Workmode.Text = "B段工作方式";
        label_B_Workmode.TextAlign = ContentAlignment.MiddleRight;
        label_KeyLock.Location = new Point(325, 98);
        label_KeyLock.Name = "label_KeyLock";
        label_KeyLock.Size = new Size(115, 28);
        label_KeyLock.TabIndex = 24;
        label_KeyLock.Text = "键盘锁定";
        label_KeyLock.TextAlign = ContentAlignment.MiddleRight;
        label_PowerOnDisType.Location = new Point(53, 385);
        label_PowerOnDisType.Name = "label_PowerOnDisType";
        label_PowerOnDisType.Size = new Size(115, 28);
        label_PowerOnDisType.TabIndex = 26;
        label_PowerOnDisType.Text = "开机显示方式";
        label_PowerOnDisType.TextAlign = ContentAlignment.MiddleRight;
        label_Tone.Location = new Point(53, 278);
        label_Tone.Name = "label_Tone";
        label_Tone.Size = new Size(115, 28);
        label_Tone.TabIndex = 27;
        label_Tone.Text = "中继导频频率";
        label_Tone.TextAlign = ContentAlignment.MiddleRight;
        label_BacklightTime.Location = new Point(325, 284);
        label_BacklightTime.Name = "label_BacklightTime";
        label_BacklightTime.Size = new Size(115, 28);
        label_BacklightTime.TabIndex = 29;
        label_BacklightTime.Text = "背光时间";
        label_BacklightTime.TextAlign = ContentAlignment.MiddleRight;
        label_MenuAutoQuit.Location = new Point(297, 248);
        label_MenuAutoQuit.Name = "label_MenuAutoQuit";
        label_MenuAutoQuit.Size = new Size(143, 28);
        label_MenuAutoQuit.TabIndex = 30;
        label_MenuAutoQuit.Text = "菜单自动退出时间";
        label_MenuAutoQuit.TextAlign = ContentAlignment.MiddleRight;
        label_Key1_S.Location = new Point(591, 171);
        label_Key1_S.Name = "label_Key1_S";
        label_Key1_S.Size = new Size(115, 28);
        label_Key1_S.TabIndex = 31;
        label_Key1_S.Text = "侧键1短按";
        label_Key1_S.TextAlign = ContentAlignment.MiddleRight;
        label_Key1_L.Location = new Point(591, 208);
        label_Key1_L.Name = "label_Key1_L";
        label_Key1_L.Size = new Size(115, 28);
        label_Key1_L.TabIndex = 32;
        label_Key1_L.Text = "侧键1长按";
        label_Key1_L.TextAlign = ContentAlignment.MiddleRight;
        label_Key2_S.Location = new Point(591, 244);
        label_Key2_S.Name = "label_Key2_S";
        label_Key2_S.Size = new Size(115, 28);
        label_Key2_S.TabIndex = 33;
        label_Key2_S.Text = "侧键2短按";
        label_Key2_S.TextAlign = ContentAlignment.MiddleRight;
        label_Key2_L.Location = new Point(591, 279);
        label_Key2_L.Name = "label_Key2_L";
        label_Key2_L.Size = new Size(115, 28);
        label_Key2_L.TabIndex = 34;
        label_Key2_L.Text = "侧键2长按";
        label_Key2_L.TextAlign = ContentAlignment.MiddleRight;
        label_Bright.Location = new Point(325, 320);
        label_Bright.Name = "label_Bright";
        label_Bright.Size = new Size(115, 28);
        label_Bright.TabIndex = 35;
        label_Bright.Text = "背光亮度";
        label_Bright.TextAlign = ContentAlignment.MiddleRight;
        cbB_SQL.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_SQL.FormattingEnabled = true;
        cbB_SQL.Items.AddRange(new object[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
        cbB_SQL.Location = new Point(173, 28);
        cbB_SQL.Margin = new Padding(3, 2, 3, 2);
        cbB_SQL.Name = "cbB_SQL";
        cbB_SQL.Size = new Size(107, 23);
        cbB_SQL.TabIndex = 36;
        cbB_TOT.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_TOT.FormattingEnabled = true;
        cbB_TOT.Items.AddRange(new object[9] { "关", "30s", "60s", "90s", "120s", "150s", "180s", "210s", "240s" });
        cbB_TOT.Location = new Point(173, 64);
        cbB_TOT.Margin = new Padding(3, 2, 3, 2);
        cbB_TOT.Name = "cbB_TOT";
        cbB_TOT.Size = new Size(107, 23);
        cbB_TOT.TabIndex = 37;
        cbB_SaveMode.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_SaveMode.FormattingEnabled = true;
        cbB_SaveMode.Items.AddRange(new object[2] { "关", "开" });
        cbB_SaveMode.Location = new Point(173, 100);
        cbB_SaveMode.Margin = new Padding(3, 2, 3, 2);
        cbB_SaveMode.Name = "cbB_SaveMode";
        cbB_SaveMode.Size = new Size(107, 23);
        cbB_SaveMode.TabIndex = 38;
        cbB_Vox.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_Vox.FormattingEnabled = true;
        cbB_Vox.Items.AddRange(new object[3] { "高灵敏度", "中灵敏度", "低灵敏度" });
        cbB_Vox.Location = new Point(173, 172);
        cbB_Vox.Margin = new Padding(3, 2, 3, 2);
        cbB_Vox.Name = "cbB_Vox";
        cbB_Vox.Size = new Size(107, 23);
        cbB_Vox.TabIndex = 39;
        cbB_VoxDly.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_VoxDly.FormattingEnabled = true;
        cbB_VoxDly.Items.AddRange(new object[16]
        {
            "0.5s", "0.6s", "0.7s", "0.8s", "0.9s", "1.0s", "1.1s", "1.2s", "1.3s", "1.4s",
            "1.5s", "1.6s", "1.7s", "1.8s", "1.9s", "2.0s"
        });
        cbB_VoxDly.Location = new Point(173, 209);
        cbB_VoxDly.Margin = new Padding(3, 2, 3, 2);
        cbB_VoxDly.Name = "cbB_VoxDly";
        cbB_VoxDly.Size = new Size(107, 23);
        cbB_VoxDly.TabIndex = 40;
        cbB_TDR.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_TDR.FormattingEnabled = true;
        cbB_TDR.Items.AddRange(new object[2] { "关", "开" });
        cbB_TDR.Location = new Point(173, 245);
        cbB_TDR.Margin = new Padding(3, 2, 3, 2);
        cbB_TDR.Name = "cbB_TDR";
        cbB_TDR.Size = new Size(107, 23);
        cbB_TDR.TabIndex = 41;
        cbB_Tone.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_Tone.FormattingEnabled = true;
        cbB_Tone.Items.AddRange(new object[4] { "1000Hz", "1450Hz", "1750Hz", "2100Hz" });
        cbB_Tone.Location = new Point(173, 281);
        cbB_Tone.Margin = new Padding(3, 2, 3, 2);
        cbB_Tone.Name = "cbB_Tone";
        cbB_Tone.Size = new Size(107, 23);
        cbB_Tone.TabIndex = 42;
        cbB_SideTone.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_SideTone.FormattingEnabled = true;
        cbB_SideTone.Items.AddRange(new object[2] { "关", "开" });
        cbB_SideTone.Location = new Point(173, 318);
        cbB_SideTone.Margin = new Padding(3, 2, 3, 2);
        cbB_SideTone.Name = "cbB_SideTone";
        cbB_SideTone.Size = new Size(107, 23);
        cbB_SideTone.TabIndex = 43;
        cbB_TailClear.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_TailClear.FormattingEnabled = true;
        cbB_TailClear.Items.AddRange(new object[2] { "关", "开" });
        cbB_TailClear.Location = new Point(173, 352);
        cbB_TailClear.Margin = new Padding(3, 2, 3, 2);
        cbB_TailClear.Name = "cbB_TailClear";
        cbB_TailClear.Size = new Size(107, 23);
        cbB_TailClear.TabIndex = 44;
        cbB_PowerOnDisType.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_PowerOnDisType.FormattingEnabled = true;
        cbB_PowerOnDisType.Items.AddRange(new object[3] { "LOGO", "电池电压", "预设信息" });
        cbB_PowerOnDisType.Location = new Point(173, 389);
        cbB_PowerOnDisType.Margin = new Padding(3, 2, 3, 2);
        cbB_PowerOnDisType.Name = "cbB_PowerOnDisType";
        cbB_PowerOnDisType.Size = new Size(107, 23);
        cbB_PowerOnDisType.TabIndex = 45;
        cbB_Beep.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_Beep.FormattingEnabled = true;
        cbB_Beep.Items.AddRange(new object[4] { "关", "BEEP 音", "语音", "全部" });
        cbB_Beep.Location = new Point(173, 425);
        cbB_Beep.Margin = new Padding(3, 2, 3, 2);
        cbB_Beep.Name = "cbB_Beep";
        cbB_Beep.Size = new Size(107, 23);
        cbB_Beep.TabIndex = 46;
        cbB_Roger.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_Roger.FormattingEnabled = true;
        cbB_Roger.Items.AddRange(new object[2] { "关", "开" });
        cbB_Roger.Location = new Point(445, 393);
        cbB_Roger.Margin = new Padding(3, 2, 3, 2);
        cbB_Roger.Name = "cbB_Roger";
        cbB_Roger.Size = new Size(107, 23);
        cbB_Roger.TabIndex = 47;
        cbB_MicGain.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_MicGain.FormattingEnabled = true;
        cbB_MicGain.Items.AddRange(new object[3] { "低增益", "中增益", "高增益" });
        cbB_MicGain.Location = new Point(173, 460);
        cbB_MicGain.Margin = new Padding(3, 2, 3, 2);
        cbB_MicGain.Name = "cbB_MicGain";
        cbB_MicGain.Size = new Size(107, 23);
        cbB_MicGain.TabIndex = 48;
        cbB_ScanMode.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_ScanMode.FormattingEnabled = true;
        cbB_ScanMode.Items.AddRange(new object[3] { "时间扫描", "载波扫描", "搜索扫描" });
        cbB_ScanMode.Location = new Point(445, 28);
        cbB_ScanMode.Margin = new Padding(3, 2, 3, 2);
        cbB_ScanMode.Name = "cbB_ScanMode";
        cbB_ScanMode.Size = new Size(107, 23);
        cbB_ScanMode.TabIndex = 49;
        cbB_SOSMode.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_SOSMode.FormattingEnabled = true;
        cbB_SOSMode.Items.AddRange(new object[3] { "现场报警", "发射报警音", "发射报警码" });
        cbB_SOSMode.Location = new Point(445, 62);
        cbB_SOSMode.Margin = new Padding(3, 2, 3, 2);
        cbB_SOSMode.Name = "cbB_SOSMode";
        cbB_SOSMode.Size = new Size(107, 23);
        cbB_SOSMode.TabIndex = 51;
        cbB_KeyLock.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_KeyLock.FormattingEnabled = true;
        cbB_KeyLock.Items.AddRange(new object[2] { "关", "开" });
        cbB_KeyLock.Location = new Point(445, 99);
        cbB_KeyLock.Margin = new Padding(3, 2, 3, 2);
        cbB_KeyLock.Name = "cbB_KeyLock";
        cbB_KeyLock.Size = new Size(107, 23);
        cbB_KeyLock.TabIndex = 52;
        cbB_FM.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_FM.FormattingEnabled = true;
        cbB_FM.Items.AddRange(new object[2] { "允许", "禁止" });
        cbB_FM.Location = new Point(445, 135);
        cbB_FM.Margin = new Padding(3, 2, 3, 2);
        cbB_FM.Name = "cbB_FM";
        cbB_FM.Size = new Size(107, 23);
        cbB_FM.TabIndex = 53;
        cbB_AutoLock.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_AutoLock.FormattingEnabled = true;
        cbB_AutoLock.Items.AddRange(new object[7] { "关", "10 s", "30 s", "1 min", "5 min", "10 min", "30 min" });
        cbB_AutoLock.Location = new Point(445, 214);
        cbB_AutoLock.Margin = new Padding(3, 2, 3, 2);
        cbB_AutoLock.Name = "cbB_AutoLock";
        cbB_AutoLock.Size = new Size(107, 23);
        cbB_AutoLock.TabIndex = 54;
        cbB_MenuAutoQuit.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_MenuAutoQuit.FormattingEnabled = true;
        cbB_MenuAutoQuit.Items.AddRange(new object[11]
        {
            "5 s", "10 s", "15 s", "20 s", "25 s", "30 s", "35 s", "40 s", "45 s", "50 s",
            "60 s"
        });
        cbB_MenuAutoQuit.Location = new Point(445, 250);
        cbB_MenuAutoQuit.Margin = new Padding(3, 2, 3, 2);
        cbB_MenuAutoQuit.Name = "cbB_MenuAutoQuit";
        cbB_MenuAutoQuit.Size = new Size(107, 23);
        cbB_MenuAutoQuit.TabIndex = 55;
        cbB_BacklightTime.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_BacklightTime.FormattingEnabled = true;
        cbB_BacklightTime.Items.AddRange(new object[9]
            { "常开", "5 s", "10 s", "15 s", "20 s", "30 s", "1 min", "2 min", "3 min" });
        cbB_BacklightTime.Location = new Point(445, 286);
        cbB_BacklightTime.Margin = new Padding(3, 2, 3, 2);
        cbB_BacklightTime.Name = "cbB_BacklightTime";
        cbB_BacklightTime.Size = new Size(107, 23);
        cbB_BacklightTime.TabIndex = 56;
        cbB_Bright.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_Bright.FormattingEnabled = true;
        cbB_Bright.Items.AddRange(new object[2] { "60 %", "100 %" });
        cbB_Bright.Location = new Point(445, 322);
        cbB_Bright.Margin = new Padding(3, 2, 3, 2);
        cbB_Bright.Name = "cbB_Bright";
        cbB_Bright.Size = new Size(107, 23);
        cbB_Bright.TabIndex = 57;
        cbB_IDDlyTime.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_IDDlyTime.FormattingEnabled = true;
        cbB_IDDlyTime.Items.AddRange(new object[16]
        {
            "100 ms", "200 ms", "300 ms", "400 ms", "500 ms", "600 ms", "700 ms", "800 ms", "900 ms", "1000 ms",
            "1100 ms", "1200 ms", "1300 ms", "1400 ms", "1500 ms", "1600 ms"
        });
        cbB_IDDlyTime.Location = new Point(445, 358);
        cbB_IDDlyTime.Margin = new Padding(3, 2, 3, 2);
        cbB_IDDlyTime.Name = "cbB_IDDlyTime";
        cbB_IDDlyTime.Size = new Size(107, 23);
        cbB_IDDlyTime.TabIndex = 58;
        cbB_A_DisType.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_A_DisType.FormattingEnabled = true;
        cbB_A_DisType.Items.AddRange(new object[3] { "信道名称", "频率", "信道号" });
        cbB_A_DisType.Location = new Point(712, 30);
        cbB_A_DisType.Margin = new Padding(3, 2, 3, 2);
        cbB_A_DisType.Name = "cbB_A_DisType";
        cbB_A_DisType.Size = new Size(107, 23);
        cbB_A_DisType.TabIndex = 60;
        cbB_A_Workmode.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_A_Workmode.FormattingEnabled = true;
        cbB_A_Workmode.Items.AddRange(new object[2] { "频率模式", "信道模式" });
        cbB_A_Workmode.Location = new Point(712, 64);
        cbB_A_Workmode.Margin = new Padding(3, 2, 3, 2);
        cbB_A_Workmode.Name = "cbB_A_Workmode";
        cbB_A_Workmode.Size = new Size(107, 23);
        cbB_A_Workmode.TabIndex = 61;
        cbB_B_DisType.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_B_DisType.FormattingEnabled = true;
        cbB_B_DisType.Items.AddRange(new object[3] { "信道名称", "频率", "信道号" });
        cbB_B_DisType.Location = new Point(712, 102);
        cbB_B_DisType.Margin = new Padding(3, 2, 3, 2);
        cbB_B_DisType.Name = "cbB_B_DisType";
        cbB_B_DisType.Size = new Size(107, 23);
        cbB_B_DisType.TabIndex = 62;
        cbB_B_Workmode.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_B_Workmode.FormattingEnabled = true;
        cbB_B_Workmode.Items.AddRange(new object[2] { "频率模式", "信道模式" });
        cbB_B_Workmode.Location = new Point(712, 138);
        cbB_B_Workmode.Margin = new Padding(3, 2, 3, 2);
        cbB_B_Workmode.Name = "cbB_B_Workmode";
        cbB_B_Workmode.Size = new Size(107, 23);
        cbB_B_Workmode.TabIndex = 63;
        cbB_Key1_S.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_Key1_S.FormattingEnabled = true;
        cbB_Key1_S.Items.AddRange(
            new object[9] { "收音机", "功率切换", "监听", "扫频", "报警", "天气预报", "扫描", "声控开关", "远程扫描亚音" });
        cbB_Key1_S.Location = new Point(712, 174);
        cbB_Key1_S.Margin = new Padding(3, 2, 3, 2);
        cbB_Key1_S.Name = "cbB_Key1_S";
        cbB_Key1_S.Size = new Size(107, 23);
        cbB_Key1_S.TabIndex = 64;
        cbB_Key1_L.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_Key1_L.FormattingEnabled = true;
        cbB_Key1_L.Items.AddRange(
            new object[9] { "收音机", "功率切换", "监听", "扫频", "报警", "天气预报", "扫描", "声控开关", "远程扫描亚音" });
        cbB_Key1_L.Location = new Point(712, 209);
        cbB_Key1_L.Margin = new Padding(3, 2, 3, 2);
        cbB_Key1_L.Name = "cbB_Key1_L";
        cbB_Key1_L.Size = new Size(107, 23);
        cbB_Key1_L.TabIndex = 65;
        cbB_Key2_S.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_Key2_S.FormattingEnabled = true;
        cbB_Key2_S.Items.AddRange(
            new object[9] { "收音机", "功率切换", "监听", "扫频", "报警", "天气预报", "扫描", "声控开关", "远程扫描亚音" });
        cbB_Key2_S.Location = new Point(712, 245);
        cbB_Key2_S.Margin = new Padding(3, 2, 3, 2);
        cbB_Key2_S.Name = "cbB_Key2_S";
        cbB_Key2_S.Size = new Size(107, 23);
        cbB_Key2_S.TabIndex = 66;
        cbB_Key2_L.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_Key2_L.FormattingEnabled = true;
        cbB_Key2_L.Items.AddRange(
            new object[9] { "收音机", "功率切换", "监听", "扫频", "报警", "天气预报", "扫描", "声控开关", "远程扫描亚音" });
        cbB_Key2_L.Location = new Point(712, 282);
        cbB_Key2_L.Margin = new Padding(3, 2, 3, 2);
        cbB_Key2_L.Name = "cbB_Key2_L";
        cbB_Key2_L.Size = new Size(107, 23);
        cbB_Key2_L.TabIndex = 67;
        cbB_RptClearTail.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_RptClearTail.FormattingEnabled = true;
        cbB_RptClearTail.Items.AddRange(new object[10]
            { "100 ms", "200 ms", "300 ms", "400 ms", "500 ms", "600 ms", "700 ms", "800 ms", "900 ms", "1000 ms" });
        cbB_RptClearTail.Location = new Point(712, 318);
        cbB_RptClearTail.Margin = new Padding(3, 2, 3, 2);
        cbB_RptClearTail.Name = "cbB_RptClearTail";
        cbB_RptClearTail.Size = new Size(107, 23);
        cbB_RptClearTail.TabIndex = 68;
        cbB_RptDetectTail.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_RptDetectTail.FormattingEnabled = true;
        cbB_RptDetectTail.Items.AddRange(new object[10]
            { "100 ms", "200 ms", "300 ms", "400 ms", "500 ms", "600 ms", "700 ms", "800 ms", "900 ms", "1000 ms" });
        cbB_RptDetectTail.Location = new Point(712, 358);
        cbB_RptDetectTail.Margin = new Padding(3, 2, 3, 2);
        cbB_RptDetectTail.Name = "cbB_RptDetectTail";
        cbB_RptDetectTail.Size = new Size(107, 23);
        cbB_RptDetectTail.TabIndex = 69;
        cbB_CurBank.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_CurBank.FormattingEnabled = true;
        cbB_CurBank.Location = new Point(445, 174);
        cbB_CurBank.Margin = new Padding(3, 2, 3, 2);
        cbB_CurBank.Name = "cbB_CurBank";
        cbB_CurBank.Size = new Size(107, 23);
        cbB_CurBank.TabIndex = 73;
        label_CurBank.Location = new Point(297, 171);
        label_CurBank.Name = "label_CurBank";
        label_CurBank.Size = new Size(143, 28);
        label_CurBank.TabIndex = 72;
        label_CurBank.Text = "当前区域";
        label_CurBank.TextAlign = ContentAlignment.MiddleRight;
        label1.Location = new Point(53, 134);
        label1.Name = "label1";
        label1.Size = new Size(115, 28);
        label1.TabIndex = 74;
        label1.Text = "声控开关";
        label1.TextAlign = ContentAlignment.MiddleRight;
        cbB_VoxSw.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_VoxSw.FormattingEnabled = true;
        cbB_VoxSw.Items.AddRange(new object[2] { "关", "开" });
        cbB_VoxSw.Location = new Point(173, 136);
        cbB_VoxSw.Margin = new Padding(3, 2, 3, 2);
        cbB_VoxSw.Name = "cbB_VoxSw";
        cbB_VoxSw.Size = new Size(107, 23);
        cbB_VoxSw.TabIndex = 75;
        label2.Location = new Point(591, 391);
        label2.Name = "label2";
        label2.Size = new Size(115, 28);
        label2.TabIndex = 76;
        label2.Text = "开机显示时间";
        label2.TextAlign = ContentAlignment.MiddleRight;
        cbB_PowerUpDisTime.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_PowerUpDisTime.FormattingEnabled = true;
        cbB_PowerUpDisTime.Items.AddRange(new object[15]
        {
            "0.2 s", "0.4 s", "0.6 s", "0.8 s", "1.0 s", "1.2 s", "1.4 s", "1.6 s", "1.8 s", "2.0 s",
            "2.2 s", "2.4 s", "2.6 s", "2.8 s", "3.0 s"
        });
        cbB_PowerUpDisTime.Location = new Point(712, 394);
        cbB_PowerUpDisTime.Margin = new Padding(3, 2, 3, 2);
        cbB_PowerUpDisTime.Name = "cbB_PowerUpDisTime";
        cbB_PowerUpDisTime.Size = new Size(107, 23);
        cbB_PowerUpDisTime.TabIndex = 77;
        label5.Location = new Point(588, 431);
        label5.Margin = new Padding(4, 0, 4, 0);
        label5.Name = "label5";
        label5.Size = new Size(117, 21);
        label5.TabIndex = 82;
        label5.Text = "蓝牙音频增益";
        label5.TextAlign = ContentAlignment.MiddleRight;
        cbBBluetoothAudioGain.DropDownStyle = ComboBoxStyle.DropDownList;
        cbBBluetoothAudioGain.FormattingEnabled = true;
        cbBBluetoothAudioGain.Items.AddRange(new object[5] { "1", "2", "3", "4", "5" });
        cbBBluetoothAudioGain.Location = new Point(712, 431);
        cbBBluetoothAudioGain.Margin = new Padding(3, 2, 3, 2);
        cbBBluetoothAudioGain.Name = "cbBBluetoothAudioGain";
        cbBBluetoothAudioGain.Size = new Size(107, 23);
        cbBBluetoothAudioGain.TabIndex = 83;
        cbBBluetoothAudioGain.SelectedIndexChanged +=
            cbBBluetoothAudioGain_SelectedIndexChanged;
        label6.Location = new Point(609, 468);
        label6.Margin = new Padding(4, 0, 4, 0);
        label6.Name = "label6";
        label6.Size = new Size(96, 21);
        label6.TabIndex = 84;
        label6.Text = "呼号";
        label6.TextAlign = ContentAlignment.MiddleRight;
        tBCallSign.Location = new Point(712, 466);
        tBCallSign.Margin = new Padding(4);
        tBCallSign.Name = "tBCallSign";
        tBCallSign.Size = new Size(107, 25);
        tBCallSign.TabIndex = 85;
        tBCallSign.TextChanged += tBCallSign_TextChanged;
        tBCallSign.KeyPress += textBox1_KeyPress;
        label3.Location = new Point(609, 504);
        label3.Margin = new Padding(4, 0, 4, 0);
        label3.Name = "label3";
        label3.Size = new Size(96, 21);
        label3.TabIndex = 86;
        label3.Text = "SDC-ID";
        label3.TextAlign = ContentAlignment.MiddleRight;
        tB_SDCID.Location = new Point(712, 502);
        tB_SDCID.MaxLength = 4;
        tB_SDCID.Name = "tB_SDCID";
        tB_SDCID.Size = new Size(107, 25);
        tB_SDCID.TabIndex = 87;
        tB_SDCID.KeyPress += tB_SDCID_KeyPress;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(855, 538);
        Controls.Add(tB_SDCID);
        Controls.Add(label3);
        Controls.Add(tBCallSign);
        Controls.Add(label6);
        Controls.Add(cbBBluetoothAudioGain);
        Controls.Add(label5);
        Controls.Add(cbB_PowerUpDisTime);
        Controls.Add(label2);
        Controls.Add(cbB_VoxSw);
        Controls.Add(label1);
        Controls.Add(cbB_CurBank);
        Controls.Add(label_CurBank);
        Controls.Add(cbB_RptDetectTail);
        Controls.Add(cbB_RptClearTail);
        Controls.Add(cbB_Key2_L);
        Controls.Add(cbB_Key2_S);
        Controls.Add(cbB_Key1_L);
        Controls.Add(cbB_Key1_S);
        Controls.Add(cbB_B_Workmode);
        Controls.Add(cbB_B_DisType);
        Controls.Add(cbB_A_Workmode);
        Controls.Add(cbB_A_DisType);
        Controls.Add(cbB_IDDlyTime);
        Controls.Add(cbB_Bright);
        Controls.Add(cbB_BacklightTime);
        Controls.Add(cbB_MenuAutoQuit);
        Controls.Add(cbB_AutoLock);
        Controls.Add(cbB_FM);
        Controls.Add(cbB_KeyLock);
        Controls.Add(cbB_SOSMode);
        Controls.Add(cbB_ScanMode);
        Controls.Add(cbB_MicGain);
        Controls.Add(cbB_Roger);
        Controls.Add(cbB_Beep);
        Controls.Add(cbB_PowerOnDisType);
        Controls.Add(cbB_TailClear);
        Controls.Add(cbB_SideTone);
        Controls.Add(cbB_Tone);
        Controls.Add(cbB_TDR);
        Controls.Add(cbB_VoxDly);
        Controls.Add(cbB_Vox);
        Controls.Add(cbB_SaveMode);
        Controls.Add(cbB_TOT);
        Controls.Add(cbB_SQL);
        Controls.Add(label_Bright);
        Controls.Add(label_Key2_L);
        Controls.Add(label_Key2_S);
        Controls.Add(label_Key1_L);
        Controls.Add(label_Key1_S);
        Controls.Add(label_MenuAutoQuit);
        Controls.Add(label_BacklightTime);
        Controls.Add(label_Tone);
        Controls.Add(label_PowerOnDisType);
        Controls.Add(label_KeyLock);
        Controls.Add(label_B_Workmode);
        Controls.Add(label_A_Workmode);
        Controls.Add(label_FM);
        Controls.Add(label_Roger);
        Controls.Add(label_RptDetectTail);
        Controls.Add(label_RptClearTail);
        Controls.Add(label_TailClear);
        Controls.Add(label1_SOSMode);
        Controls.Add(label_MicGain);
        Controls.Add(label_AutoLock);
        Controls.Add(label_B_DisType);
        Controls.Add(label_A_DisType);
        Controls.Add(label_IDDlyTime);
        Controls.Add(label_ScanMode);
        Controls.Add(label_SideTone);
        Controls.Add(label_Beep);
        Controls.Add(label_TOT);
        Controls.Add(label_TDR);
        Controls.Add(label_VoxDlyTime);
        Controls.Add(label_Vox);
        Controls.Add(label_Save);
        Controls.Add(label_SQL);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(3, 2, 3, 2);
        Name = "FormFunction";
        StartPosition = FormStartPosition.CenterParent;
        Text = "可选功能";
        FormClosing += FormFunction_FormClosing;
        Load += FormFunction_Load;
        ResumeLayout(false);
        PerformLayout();
    }
}