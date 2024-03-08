using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SHX_GT12_CPS.Properties;

namespace SHX_GT12_CPS;

public class FormVFOMode : Form
{
    private ComboBox cbB_A_Bandwide;

    private ComboBox cbB_A_Busylock;

    private ComboBox cbB_A_Dir;

    private ComboBox cbB_A_Power;

    private ComboBox cbB_A_RxCtsDcs;

    private ComboBox cbB_A_SignalGroup;

    private ComboBox cbB_A_SignalSystem;

    private ComboBox cbB_A_SQMode;

    private ComboBox cbB_A_Step;

    private ComboBox cbB_A_TxCtsDcs;

    private ComboBox cbB_B_Bandwide;

    private ComboBox cbB_B_Busylock;

    private ComboBox cbB_B_Dir;

    private ComboBox cbB_B_Power;

    private ComboBox cbB_B_RxCtsDcs;

    private ComboBox cbB_B_SignalGroup;

    private ComboBox cbB_B_SignalSystem;

    private ComboBox cbB_B_SQMode;

    private ComboBox cbB_B_Step;

    private ComboBox cbB_B_TxCtsDcs;

    private ComboBox cbB_PttID;

    private readonly IContainer components = null;

    private GroupBox groupBox1;

    private GroupBox groupBox2;

    private Label label_A_Bandwide;

    private Label label_A_Busylock;

    private Label label_A_Dir;

    private Label label_A_Freq;

    private Label label_A_Offset;

    private Label label_A_Power;

    private Label label_A_RxCtsDcs;

    private Label label_A_SignalGroup;

    private Label label_A_SignalSystem;

    private Label label_A_SQMode;

    private Label label_A_Step;

    private Label label_A_TxCtsDcs;

    private Label label_B_Bandwide;

    private Label label_B_Busylock;

    private Label label_B_Dir;

    private Label label_B_Freq;

    private Label label_B_Offset;

    private Label label_B_Power;

    private Label label_B_RxCtsDcs;

    private Label label_B_SignalGroup;

    private Label label_B_SignalSystem;

    private Label label_B_SQMode;

    private Label label_B_Step;

    private Label label_B_TxCtsDcs;

    private Label label_PTTID;

    private readonly string LANG = "Chinese";

    private readonly int maxFreq = 520;

    private readonly int minFreq = 100;

    private string oldStrValue = "";

    private TextBox tB_A_Freq;

    private TextBox tB_A_Offset;

    private TextBox tB_B_Freq;

    private TextBox tB_B_Offset;

    private readonly string[] tblCTSDCS = new string[261]
    {
        "OFF", "67.0", "69.3", "71.9", "74.4", "77.0", "79.7", "82.5", "85.4", "88.5",
        "91.5", "94.8", "97.4", "100.0", "103.5", "107.2", "110.9", "114.8", "118.8", "123.0",
        "127.3", "131.8", "136.5", "141.3", "146.2", "151.4", "156.7", "159.8", "162.2", "165.5",
        "167.9", "171.3", "173.8", "177.3", "179.9", "183.5", "186.2", "189.9", "192.8", "196.6",
        "199.5", "203.5", "206.5", "210.7", "218.1", "225.7", "229.1", "233.6", "241.8", "250.3",
        "254.1", "D023N", "D025N", "D026N", "D031N", "D032N", "D036N", "D043N", "D047N", "D051N",
        "D053N", "D054N", "D065N", "D071N", "D072N", "D073N", "D074N", "D114N", "D115N", "D116N",
        "D122N", "D125N", "D131N", "D132N", "D134N", "D143N", "D145N", "D152N", "D155N", "D156N",
        "D162N", "D165N", "D172N", "D174N", "D205N", "D212N", "D223N", "D225N", "D226N", "D243N",
        "D244N", "D245N", "D246N", "D251N", "D252N", "D255N", "D261N", "D263N", "D265N", "D266N",
        "D271N", "D274N", "D306N", "D311N", "D315N", "D325N", "D331N", "D332N", "D343N", "D346N",
        "D351N", "D356N", "D364N", "D365N", "D371N", "D411N", "D412N", "D413N", "D423N", "D431N",
        "D432N", "D445N", "D446N", "D452N", "D454N", "D455N", "D462N", "D464N", "D465N", "D466N",
        "D503N", "D506N", "D516N", "D523N", "D526N", "D532N", "D546N", "D565N", "D606N", "D612N",
        "D624N", "D627N", "D631N", "D632N", "D645N", "D654N", "D662N", "D664N", "D703N", "D712N",
        "D723N", "D731N", "D732N", "D734N", "D743N", "D754N", "D023I", "D025I", "D026I", "D031I",
        "D032I", "D036I", "D043I", "D047I", "D051I", "D053I", "D054I", "D065I", "D071I", "D072I",
        "D073I", "D074I", "D114I", "D115I", "D116I", "D122I", "D125I", "D131I", "D132I", "D134I",
        "D143I", "D145I", "D152I", "D155I", "D156I", "D162I", "D165I", "D172I", "D174I", "D205I",
        "D212I", "D223I", "D225I", "D226I", "D243I", "D244I", "D245I", "D246I", "D251I", "D252I",
        "D255I", "D261I", "D263I", "D265I", "D266I", "D271I", "D274I", "D306I", "D311I", "D315I",
        "D325I", "D331I", "D332I", "D343I", "D346I", "D351I", "D356I", "D364I", "D365I", "D371I",
        "D411I", "D412I", "D413I", "D423I", "D431I", "D432I", "D445I", "D446I", "D452I", "D454I",
        "D455I", "D462I", "D464I", "D465I", "D466I", "D503I", "D506I", "D516I", "D523I", "D526I",
        "D532I", "D546I", "D565I", "D606I", "D612I", "D624I", "D627I", "D631I", "D632I", "D645I",
        "D654I", "D662I", "D664I", "D703I", "D712I", "D723I", "D731I", "D732I", "D734I", "D743I",
        "D754I"
    };

    private VFOInfos vfos;

    public FormVFOMode(Form parent)
    {
        InitializeComponent();
        cbB_A_RxCtsDcs.Items.AddRange(tblCTSDCS);
        cbB_A_TxCtsDcs.Items.AddRange(tblCTSDCS);
        cbB_B_RxCtsDcs.Items.AddRange(tblCTSDCS);
        cbB_B_TxCtsDcs.Items.AddRange(tblCTSDCS);
        MdiParent = parent;
        LANG = Settings.Default.LANG;
    }

    private void FormVFOMode_Load(object sender, EventArgs e)
    {
    }

    public void LoadData(VFOInfos vfos)
    {
        this.vfos = vfos;
        BindingControls();
    }

    private void TryToBingdingControl(Control c, string propertyName, object dataSource, string dataMember,
        object defaultVal)
    {
        if (c.DataBindings.Count != 0) c.DataBindings.RemoveAt(0);

        try
        {
            c.DataBindings.Add(new Binding(propertyName, dataSource, dataMember, false,
                DataSourceUpdateMode.OnPropertyChanged));
        }
        catch
        {
            vfos.GetType().GetProperty(dataMember).SetValue(vfos, defaultVal, null);
            c.DataBindings.Add(new Binding(propertyName, dataSource, dataMember, false,
                DataSourceUpdateMode.OnPropertyChanged));
        }
    }

    private void BindingControls()
    {
        TryToBingdingControl(tB_A_Freq, "Text", vfos, "VfoAFreq", "440.62500");
        TryToBingdingControl(cbB_A_RxCtsDcs, "Text", vfos, "StrVFOARxCtsDcs", "OFF");
        TryToBingdingControl(cbB_A_TxCtsDcs, "Text", vfos, "StrVFOATxCtsDcs", "OFF");
        TryToBingdingControl(cbB_A_SignalSystem, "SelectedIndex", vfos, "VfoASignalSystem", 0);
        TryToBingdingControl(cbB_A_SQMode, "SelectedIndex", vfos, "VfoASQMode", 0);
        TryToBingdingControl(cbB_A_Power, "SelectedIndex", vfos, "VfoATxPower", 0);
        TryToBingdingControl(cbB_A_Bandwide, "SelectedIndex", vfos, "VfoABandwide", 0);
        TryToBingdingControl(cbB_A_Step, "SelectedIndex", vfos, "VfoAStep", 3);
        TryToBingdingControl(cbB_A_SignalGroup, "SelectedIndex", vfos, "VfoASignalGroup", 0);
        TryToBingdingControl(cbB_A_Busylock, "SelectedIndex", vfos, "VfoABusyLock", 0);
        TryToBingdingControl(cbB_A_Dir, "SelectedIndex", vfos, "VfoADir", 0);
        TryToBingdingControl(tB_A_Offset, "Text", vfos, "VfoAOffset", "00.0000");
        TryToBingdingControl(tB_B_Freq, "Text", vfos, "VfoBFreq", "145.62500");
        TryToBingdingControl(cbB_B_RxCtsDcs, "Text", vfos, "StrVFOBRxCtsDcs", "OFF");
        TryToBingdingControl(cbB_B_TxCtsDcs, "Text", vfos, "StrVFOBTxCtsDcs", "OFF");
        TryToBingdingControl(cbB_B_SignalSystem, "SelectedIndex", vfos, "VfoBSignalSystem", 0);
        TryToBingdingControl(cbB_B_SQMode, "SelectedIndex", vfos, "VfoBSQMode", 0);
        TryToBingdingControl(cbB_B_Power, "SelectedIndex", vfos, "VfoBTxPower", 0);
        TryToBingdingControl(cbB_B_Bandwide, "SelectedIndex", vfos, "VfoBBandwide", 0);
        TryToBingdingControl(cbB_B_Step, "SelectedIndex", vfos, "VfoBStep", 3);
        TryToBingdingControl(cbB_B_SignalGroup, "SelectedIndex", vfos, "VfoBSignalGroup", 0);
        TryToBingdingControl(cbB_B_Busylock, "SelectedIndex", vfos, "VfoBBusyLock", 0);
        TryToBingdingControl(cbB_B_Dir, "SelectedIndex", vfos, "VfoBDir", 0);
        TryToBingdingControl(tB_B_Offset, "Text", vfos, "VfoBOffset", "00.0000");
        TryToBingdingControl(cbB_PttID, "SelectedIndex", vfos, "Pttid", 0);
    }

    private void FormVFOMode_FormClosing(object sender, FormClosingEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    private void tB_A_Freq_Leave(object sender, EventArgs e)
    {
        var num = 0;
        var textBox = (TextBox)sender;
        var flag = false;
        var text = textBox.Text;
        if (text == "")
        {
            textBox.Text = oldStrValue;
            return;
        }

        if (text != "")
        {
            var text2 = text;
            foreach (var c in text2)
            {
                if (c != '.') continue;

                if (!flag)
                {
                    flag = true;
                    continue;
                }

                if (LANG == "Chinese")
                    MessageBox.Show("频率格式不正确!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                else
                    MessageBox.Show("Frequence's format is error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);

                textBox.Text = oldStrValue;
                break;
            }
        }

        if (!(text != "")) return;

        var array = text.Split('.');
        var list = new List<int>();
        for (var j = 0; j < array.Length; j++) list.Add(int.Parse(array[j]));

        if (list[0] < minFreq || list[0] >= maxFreq)
        {
            if (LANG == "Chinese")
                MessageBox.Show("频率错误!\n频率范围:" + minFreq + "--" + maxFreq, "错误", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
            else
                MessageBox.Show("Frequence is error!\nFreq Range:" + minFreq + "--" + maxFreq, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);

            textBox.Text = oldStrValue;
            return;
        }

        num = list[0] * 100000;
        if (list.Count > 1)
        {
            var num2 = 5 - array[1].Length;
            if (num2 > 0)
                for (var k = 0; k < num2; k++)
                    list[1] *= 10;

            num += list[1];
        }

        if (num % 125 != 0)
        {
            num /= 125;
            num *= 125;
        }

        textBox.Text = num.ToString().Insert(3, ".");
    }

    private void tB_A_Offset_Leave(object sender, EventArgs e)
    {
        var flag = false;
        var num = 0;
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        var text2 = "";
        if (text == "")
        {
            textBox.Text = oldStrValue;
            return;
        }

        if (text != "")
        {
            var text3 = text;
            foreach (var c in text3)
            {
                if (c != '.') continue;

                if (!flag)
                {
                    flag = true;
                    continue;
                }

                if (LANG == "Chinese")
                    MessageBox.Show("频率格式不正确!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                else
                    MessageBox.Show("Frequence's format is error!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);

                textBox.Text = oldStrValue;
                break;
            }
        }

        if (!(text != "")) return;

        var array = text.Split('.');
        var list = new List<int>();
        for (var j = 0; j < array.Length; j++) list.Add(int.Parse(array[j]));

        if (list[0] >= 100)
        {
            if (LANG == "Chinese")
                MessageBox.Show("范围:0 -- 100", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            else
                MessageBox.Show("Range: 0 -- 100", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);

            textBox.Text = oldStrValue;
            return;
        }

        num = list[0] * 10000;
        if (list.Count > 1)
        {
            var num2 = 4 - array[1].Length;
            if (num2 > 0)
                for (var k = 0; k < num2; k++)
                    list[1] *= 10;

            num += list[1];
        }

        if (num % 5 != 0)
        {
            num /= 5;
            num *= 5;
        }

        text2 = num.ToString();
        text2 = num >= 100000
            ? text2.Insert(2, ".")
            : num <= 0
                ? oldStrValue
                : num >= 10000
                    ? "0" + text2.Insert(1, ".")
                    : num >= 1000
                        ? "00." + text2
                        : num >= 100
                            ? "00.0" + text2
                            : num < 10
                                ? "00.000" + text2
                                : "00.00" + text2;
        textBox.Text = text2;
    }

    private void tB_A_Freq_Click(object sender, EventArgs e)
    {
        var textBox = (TextBox)sender;
        oldStrValue = textBox.Text;
    }

    private void cbB_A_RxCtsDcs_Click(object sender, EventArgs e)
    {
        var comboBox = (ComboBox)sender;
        oldStrValue = comboBox.Text;
    }

    private void tB_A_Freq_KeyPress(object sender, KeyPressEventArgs e)
    {
        if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\r' && e.KeyChar != '\b' && e.KeyChar != '.')
            e.Handled = true;
        else if (e.KeyChar == '\r') SendKeys.Send("{TAB}");
    }

    private void cbB_A_RxCtsDcs_KeyPress(object sender, KeyPressEventArgs e)
    {
        if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '\r' && e.KeyChar != 'D' &&
            e.KeyChar != 'N' && e.KeyChar != 'I')
            e.Handled = true;
        else if (e.KeyChar == '\r') SendKeys.Send("{TAB}");
    }

    private void cbB_A_RxCtsDcs_Leave(object sender, EventArgs e)
    {
        var comboBox = (ComboBox)sender;
        var text = comboBox.Text;
        if (text == "")
        {
            comboBox.Text = oldStrValue;
        }
        else
        {
            if (!(text != "")) return;

            var num = -1;
            num = cbB_A_RxCtsDcs.Items.IndexOf(text);
            if (num != -1) return;

            if (text[0] != 'D')
                try
                {
                    var num2 = double.Parse(text);
                    if (num2 >= 60.0 && num2 <= 260.0)
                    {
                        var text2 = num2.ToString();
                        var num3 = text2.IndexOf('.');
                        if (num3 == -1)
                            text2 += ".0";
                        else if (num3 == text.Length - 1)
                            text2 += "0";
                        else if (num3 != text.Length - 2) text2 = text2.Remove(num3 + 2, text2.Length - 1 - (num3 + 1));

                        comboBox.Text = text2;
                    }
                    else
                    {
                        comboBox.Text = oldStrValue;
                    }

                    return;
                }
                catch
                {
                    comboBox.Text = oldStrValue;
                    return;
                }

            comboBox.Text = oldStrValue;
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
            new ComponentResourceManager(typeof(FormVFOMode));
        label_A_Freq = new Label();
        groupBox1 = new GroupBox();
        cbB_A_Busylock = new ComboBox();
        label_A_Busylock = new Label();
        cbB_A_SignalSystem = new ComboBox();
        label_A_SignalSystem = new Label();
        tB_A_Offset = new TextBox();
        tB_A_Freq = new TextBox();
        cbB_A_Dir = new ComboBox();
        cbB_A_SignalGroup = new ComboBox();
        cbB_A_Step = new ComboBox();
        cbB_A_Bandwide = new ComboBox();
        cbB_A_Power = new ComboBox();
        cbB_A_SQMode = new ComboBox();
        cbB_A_TxCtsDcs = new ComboBox();
        cbB_A_RxCtsDcs = new ComboBox();
        label_A_SQMode = new Label();
        label_A_Offset = new Label();
        label_A_Dir = new Label();
        label_A_SignalGroup = new Label();
        label_A_Step = new Label();
        label_A_Bandwide = new Label();
        label_A_Power = new Label();
        label_A_TxCtsDcs = new Label();
        label_A_RxCtsDcs = new Label();
        groupBox2 = new GroupBox();
        cbB_B_Busylock = new ComboBox();
        label_B_Busylock = new Label();
        cbB_B_SignalSystem = new ComboBox();
        label_B_SignalSystem = new Label();
        tB_B_Offset = new TextBox();
        tB_B_Freq = new TextBox();
        cbB_B_Dir = new ComboBox();
        cbB_B_SignalGroup = new ComboBox();
        cbB_B_Step = new ComboBox();
        cbB_B_Bandwide = new ComboBox();
        cbB_B_Power = new ComboBox();
        cbB_B_SQMode = new ComboBox();
        cbB_B_TxCtsDcs = new ComboBox();
        cbB_B_RxCtsDcs = new ComboBox();
        label_B_SQMode = new Label();
        label_B_Offset = new Label();
        label_B_Dir = new Label();
        label_B_SignalGroup = new Label();
        label_B_Step = new Label();
        label_B_Bandwide = new Label();
        label_B_Power = new Label();
        label_B_TxCtsDcs = new Label();
        label_B_RxCtsDcs = new Label();
        label_B_Freq = new Label();
        cbB_PttID = new ComboBox();
        label_PTTID = new Label();
        groupBox1.SuspendLayout();
        groupBox2.SuspendLayout();
        SuspendLayout();
        label_A_Freq.Location = new Point(16, 24);
        label_A_Freq.Name = "label_A_Freq";
        label_A_Freq.Size = new Size(100, 22);
        label_A_Freq.TabIndex = 0;
        label_A_Freq.Text = "频率";
        label_A_Freq.TextAlign = ContentAlignment.MiddleRight;
        groupBox1.Controls.Add(cbB_A_Busylock);
        groupBox1.Controls.Add(label_A_Busylock);
        groupBox1.Controls.Add(cbB_A_SignalSystem);
        groupBox1.Controls.Add(label_A_SignalSystem);
        groupBox1.Controls.Add(tB_A_Offset);
        groupBox1.Controls.Add(tB_A_Freq);
        groupBox1.Controls.Add(cbB_A_Dir);
        groupBox1.Controls.Add(cbB_A_SignalGroup);
        groupBox1.Controls.Add(cbB_A_Step);
        groupBox1.Controls.Add(cbB_A_Bandwide);
        groupBox1.Controls.Add(cbB_A_Power);
        groupBox1.Controls.Add(cbB_A_SQMode);
        groupBox1.Controls.Add(cbB_A_TxCtsDcs);
        groupBox1.Controls.Add(cbB_A_RxCtsDcs);
        groupBox1.Controls.Add(label_A_SQMode);
        groupBox1.Controls.Add(label_A_Offset);
        groupBox1.Controls.Add(label_A_Dir);
        groupBox1.Controls.Add(label_A_SignalGroup);
        groupBox1.Controls.Add(label_A_Step);
        groupBox1.Controls.Add(label_A_Bandwide);
        groupBox1.Controls.Add(label_A_Power);
        groupBox1.Controls.Add(label_A_TxCtsDcs);
        groupBox1.Controls.Add(label_A_RxCtsDcs);
        groupBox1.Controls.Add(label_A_Freq);
        groupBox1.Location = new Point(35, 32);
        groupBox1.Margin = new Padding(3, 2, 3, 2);
        groupBox1.Name = "groupBox1";
        groupBox1.Padding = new Padding(3, 2, 3, 2);
        groupBox1.Size = new Size(289, 410);
        groupBox1.TabIndex = 1;
        groupBox1.TabStop = false;
        groupBox1.Text = "VFO A";
        cbB_A_Busylock.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_A_Busylock.FormattingEnabled = true;
        cbB_A_Busylock.Items.AddRange(new object[2] { "关", "开" });
        cbB_A_Busylock.Location = new Point(123, 272);
        cbB_A_Busylock.Margin = new Padding(3, 2, 3, 2);
        cbB_A_Busylock.Name = "cbB_A_Busylock";
        cbB_A_Busylock.Size = new Size(119, 23);
        cbB_A_Busylock.TabIndex = 14;
        label_A_Busylock.Location = new Point(16, 272);
        label_A_Busylock.Name = "label_A_Busylock";
        label_A_Busylock.Size = new Size(100, 22);
        label_A_Busylock.TabIndex = 13;
        label_A_Busylock.Text = "繁忙锁定";
        label_A_Busylock.TextAlign = ContentAlignment.MiddleRight;
        cbB_A_SignalSystem.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_A_SignalSystem.FormattingEnabled = true;
        cbB_A_SignalSystem.Items.AddRange(new object[3] { "无", "SDC", "DTMF" });
        cbB_A_SignalSystem.Location = new Point(123, 118);
        cbB_A_SignalSystem.Margin = new Padding(3, 2, 3, 2);
        cbB_A_SignalSystem.Name = "cbB_A_SignalSystem";
        cbB_A_SignalSystem.Size = new Size(119, 23);
        cbB_A_SignalSystem.TabIndex = 12;
        label_A_SignalSystem.Location = new Point(16, 118);
        label_A_SignalSystem.Name = "label_A_SignalSystem";
        label_A_SignalSystem.Size = new Size(100, 22);
        label_A_SignalSystem.TabIndex = 11;
        label_A_SignalSystem.Text = "信令系统";
        label_A_SignalSystem.TextAlign = ContentAlignment.MiddleRight;
        tB_A_Offset.Location = new Point(123, 365);
        tB_A_Offset.Margin = new Padding(3, 2, 3, 2);
        tB_A_Offset.MaxLength = 6;
        tB_A_Offset.Name = "tB_A_Offset";
        tB_A_Offset.Size = new Size(119, 25);
        tB_A_Offset.TabIndex = 10;
        tB_A_Offset.Click += tB_A_Freq_Click;
        tB_A_Offset.KeyPress += tB_A_Freq_KeyPress;
        tB_A_Offset.Leave += tB_A_Offset_Leave;
        tB_A_Freq.Location = new Point(123, 22);
        tB_A_Freq.Margin = new Padding(3, 2, 3, 2);
        tB_A_Freq.MaxLength = 9;
        tB_A_Freq.Name = "tB_A_Freq";
        tB_A_Freq.Size = new Size(119, 25);
        tB_A_Freq.TabIndex = 1;
        tB_A_Freq.Click += tB_A_Freq_Click;
        tB_A_Freq.KeyPress += tB_A_Freq_KeyPress;
        tB_A_Freq.Leave += tB_A_Freq_Leave;
        cbB_A_Dir.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_A_Dir.FormattingEnabled = true;
        cbB_A_Dir.Items.AddRange(new object[3] { "OFF", "+", "-" });
        cbB_A_Dir.Location = new Point(123, 334);
        cbB_A_Dir.Margin = new Padding(3, 2, 3, 2);
        cbB_A_Dir.Name = "cbB_A_Dir";
        cbB_A_Dir.Size = new Size(119, 23);
        cbB_A_Dir.TabIndex = 9;
        cbB_A_SignalGroup.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_A_SignalGroup.FormattingEnabled = true;
        cbB_A_SignalGroup.Items.AddRange(new object[20]
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20"
        });
        cbB_A_SignalGroup.Location = new Point(123, 302);
        cbB_A_SignalGroup.Margin = new Padding(3, 2, 3, 2);
        cbB_A_SignalGroup.Name = "cbB_A_SignalGroup";
        cbB_A_SignalGroup.Size = new Size(119, 23);
        cbB_A_SignalGroup.TabIndex = 8;
        cbB_A_Step.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_A_Step.FormattingEnabled = true;
        cbB_A_Step.Items.AddRange(new object[8]
            { "2.5  KHz", "5.0  KHz", "6.25 KHz", "10.0 KHz", "12.5 KHz", "20.0 KHz", "25.0 KHz", "50.0 KHz" });
        cbB_A_Step.Location = new Point(123, 241);
        cbB_A_Step.Margin = new Padding(3, 2, 3, 2);
        cbB_A_Step.Name = "cbB_A_Step";
        cbB_A_Step.Size = new Size(119, 23);
        cbB_A_Step.TabIndex = 7;
        cbB_A_Bandwide.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_A_Bandwide.FormattingEnabled = true;
        cbB_A_Bandwide.Items.AddRange(new object[2] { "宽", "窄" });
        cbB_A_Bandwide.Location = new Point(123, 210);
        cbB_A_Bandwide.Margin = new Padding(3, 2, 3, 2);
        cbB_A_Bandwide.Name = "cbB_A_Bandwide";
        cbB_A_Bandwide.Size = new Size(119, 23);
        cbB_A_Bandwide.TabIndex = 6;
        cbB_A_Power.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_A_Power.FormattingEnabled = true;
        cbB_A_Power.Items.AddRange(new object[3] { "高", "中", "低" });
        cbB_A_Power.Location = new Point(123, 179);
        cbB_A_Power.Margin = new Padding(3, 2, 3, 2);
        cbB_A_Power.Name = "cbB_A_Power";
        cbB_A_Power.Size = new Size(119, 23);
        cbB_A_Power.TabIndex = 5;
        cbB_A_SQMode.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_A_SQMode.FormattingEnabled = true;
        cbB_A_SQMode.Items.AddRange(new object[3] { "QT/DQT", "QT/DQT*DTMF", "QT/DQT+DTMF" });
        cbB_A_SQMode.Location = new Point(123, 148);
        cbB_A_SQMode.Margin = new Padding(3, 2, 3, 2);
        cbB_A_SQMode.Name = "cbB_A_SQMode";
        cbB_A_SQMode.Size = new Size(119, 23);
        cbB_A_SQMode.TabIndex = 4;
        cbB_A_TxCtsDcs.FormattingEnabled = true;
        cbB_A_TxCtsDcs.Location = new Point(123, 88);
        cbB_A_TxCtsDcs.Margin = new Padding(3, 2, 3, 2);
        cbB_A_TxCtsDcs.Name = "cbB_A_TxCtsDcs";
        cbB_A_TxCtsDcs.Size = new Size(119, 23);
        cbB_A_TxCtsDcs.TabIndex = 3;
        cbB_A_TxCtsDcs.Tag = "";
        cbB_A_TxCtsDcs.Click += cbB_A_RxCtsDcs_Click;
        cbB_A_TxCtsDcs.KeyPress += cbB_A_RxCtsDcs_KeyPress;
        cbB_A_TxCtsDcs.Leave += cbB_A_RxCtsDcs_Leave;
        cbB_A_RxCtsDcs.FormattingEnabled = true;
        cbB_A_RxCtsDcs.Location = new Point(123, 56);
        cbB_A_RxCtsDcs.Margin = new Padding(3, 2, 3, 2);
        cbB_A_RxCtsDcs.Name = "cbB_A_RxCtsDcs";
        cbB_A_RxCtsDcs.Size = new Size(119, 23);
        cbB_A_RxCtsDcs.TabIndex = 2;
        cbB_A_RxCtsDcs.Tag = "";
        cbB_A_RxCtsDcs.Click += cbB_A_RxCtsDcs_Click;
        cbB_A_RxCtsDcs.KeyPress += cbB_A_RxCtsDcs_KeyPress;
        cbB_A_RxCtsDcs.Leave += cbB_A_RxCtsDcs_Leave;
        label_A_SQMode.Location = new Point(16, 148);
        label_A_SQMode.Name = "label_A_SQMode";
        label_A_SQMode.Size = new Size(100, 22);
        label_A_SQMode.TabIndex = 9;
        label_A_SQMode.Text = "静音方式";
        label_A_SQMode.TextAlign = ContentAlignment.MiddleRight;
        label_A_Offset.Location = new Point(16, 365);
        label_A_Offset.Name = "label_A_Offset";
        label_A_Offset.Size = new Size(100, 22);
        label_A_Offset.TabIndex = 8;
        label_A_Offset.Text = "频差频率";
        label_A_Offset.TextAlign = ContentAlignment.MiddleRight;
        label_A_Dir.Location = new Point(16, 334);
        label_A_Dir.Name = "label_A_Dir";
        label_A_Dir.Size = new Size(100, 22);
        label_A_Dir.TabIndex = 7;
        label_A_Dir.Text = "频差方向";
        label_A_Dir.TextAlign = ContentAlignment.MiddleRight;
        label_A_SignalGroup.Location = new Point(16, 302);
        label_A_SignalGroup.Name = "label_A_SignalGroup";
        label_A_SignalGroup.Size = new Size(100, 22);
        label_A_SignalGroup.TabIndex = 6;
        label_A_SignalGroup.Text = "信令编码";
        label_A_SignalGroup.TextAlign = ContentAlignment.MiddleRight;
        label_A_Step.Location = new Point(16, 241);
        label_A_Step.Name = "label_A_Step";
        label_A_Step.Size = new Size(100, 22);
        label_A_Step.TabIndex = 5;
        label_A_Step.Text = "步进频率";
        label_A_Step.TextAlign = ContentAlignment.MiddleRight;
        label_A_Bandwide.Location = new Point(16, 210);
        label_A_Bandwide.Name = "label_A_Bandwide";
        label_A_Bandwide.Size = new Size(100, 22);
        label_A_Bandwide.TabIndex = 4;
        label_A_Bandwide.Text = "带宽";
        label_A_Bandwide.TextAlign = ContentAlignment.MiddleRight;
        label_A_Power.Location = new Point(16, 179);
        label_A_Power.Name = "label_A_Power";
        label_A_Power.Size = new Size(100, 22);
        label_A_Power.TabIndex = 3;
        label_A_Power.Text = "功率";
        label_A_Power.TextAlign = ContentAlignment.MiddleRight;
        label_A_TxCtsDcs.Location = new Point(16, 86);
        label_A_TxCtsDcs.Name = "label_A_TxCtsDcs";
        label_A_TxCtsDcs.Size = new Size(100, 22);
        label_A_TxCtsDcs.TabIndex = 2;
        label_A_TxCtsDcs.Text = "发射亚音";
        label_A_TxCtsDcs.TextAlign = ContentAlignment.MiddleRight;
        label_A_RxCtsDcs.Location = new Point(16, 55);
        label_A_RxCtsDcs.Name = "label_A_RxCtsDcs";
        label_A_RxCtsDcs.Size = new Size(100, 22);
        label_A_RxCtsDcs.TabIndex = 1;
        label_A_RxCtsDcs.Text = "接收亚音";
        label_A_RxCtsDcs.TextAlign = ContentAlignment.MiddleRight;
        groupBox2.Controls.Add(cbB_B_Busylock);
        groupBox2.Controls.Add(label_B_Busylock);
        groupBox2.Controls.Add(cbB_B_SignalSystem);
        groupBox2.Controls.Add(label_B_SignalSystem);
        groupBox2.Controls.Add(tB_B_Offset);
        groupBox2.Controls.Add(tB_B_Freq);
        groupBox2.Controls.Add(cbB_B_Dir);
        groupBox2.Controls.Add(cbB_B_SignalGroup);
        groupBox2.Controls.Add(cbB_B_Step);
        groupBox2.Controls.Add(cbB_B_Bandwide);
        groupBox2.Controls.Add(cbB_B_Power);
        groupBox2.Controls.Add(cbB_B_SQMode);
        groupBox2.Controls.Add(cbB_B_TxCtsDcs);
        groupBox2.Controls.Add(cbB_B_RxCtsDcs);
        groupBox2.Controls.Add(label_B_SQMode);
        groupBox2.Controls.Add(label_B_Offset);
        groupBox2.Controls.Add(label_B_Dir);
        groupBox2.Controls.Add(label_B_SignalGroup);
        groupBox2.Controls.Add(label_B_Step);
        groupBox2.Controls.Add(label_B_Bandwide);
        groupBox2.Controls.Add(label_B_Power);
        groupBox2.Controls.Add(label_B_TxCtsDcs);
        groupBox2.Controls.Add(label_B_RxCtsDcs);
        groupBox2.Controls.Add(label_B_Freq);
        groupBox2.Location = new Point(341, 32);
        groupBox2.Margin = new Padding(3, 2, 3, 2);
        groupBox2.Name = "groupBox2";
        groupBox2.Padding = new Padding(3, 2, 3, 2);
        groupBox2.Size = new Size(289, 410);
        groupBox2.TabIndex = 2;
        groupBox2.TabStop = false;
        groupBox2.Text = "VFO B";
        cbB_B_Busylock.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_B_Busylock.FormattingEnabled = true;
        cbB_B_Busylock.Items.AddRange(new object[2] { "关", "开" });
        cbB_B_Busylock.Location = new Point(123, 272);
        cbB_B_Busylock.Margin = new Padding(3, 2, 3, 2);
        cbB_B_Busylock.Name = "cbB_B_Busylock";
        cbB_B_Busylock.Size = new Size(119, 23);
        cbB_B_Busylock.TabIndex = 24;
        label_B_Busylock.Location = new Point(16, 272);
        label_B_Busylock.Name = "label_B_Busylock";
        label_B_Busylock.Size = new Size(100, 22);
        label_B_Busylock.TabIndex = 23;
        label_B_Busylock.Text = "繁忙锁定";
        label_B_Busylock.TextAlign = ContentAlignment.MiddleRight;
        cbB_B_SignalSystem.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_B_SignalSystem.FormattingEnabled = true;
        cbB_B_SignalSystem.Items.AddRange(new object[3] { "无", "SDC", "DTMF" });
        cbB_B_SignalSystem.Location = new Point(123, 118);
        cbB_B_SignalSystem.Margin = new Padding(3, 2, 3, 2);
        cbB_B_SignalSystem.Name = "cbB_B_SignalSystem";
        cbB_B_SignalSystem.Size = new Size(119, 23);
        cbB_B_SignalSystem.TabIndex = 22;
        label_B_SignalSystem.Location = new Point(16, 118);
        label_B_SignalSystem.Name = "label_B_SignalSystem";
        label_B_SignalSystem.Size = new Size(100, 22);
        label_B_SignalSystem.TabIndex = 21;
        label_B_SignalSystem.Text = "信令系统";
        label_B_SignalSystem.TextAlign = ContentAlignment.MiddleRight;
        tB_B_Offset.Location = new Point(123, 364);
        tB_B_Offset.Margin = new Padding(3, 2, 3, 2);
        tB_B_Offset.MaxLength = 6;
        tB_B_Offset.Name = "tB_B_Offset";
        tB_B_Offset.Size = new Size(119, 25);
        tB_B_Offset.TabIndex = 20;
        tB_B_Offset.Click += tB_A_Freq_Click;
        tB_B_Offset.KeyPress += tB_A_Freq_KeyPress;
        tB_B_Offset.Leave += tB_A_Offset_Leave;
        tB_B_Freq.Location = new Point(123, 21);
        tB_B_Freq.Margin = new Padding(3, 2, 3, 2);
        tB_B_Freq.MaxLength = 9;
        tB_B_Freq.Name = "tB_B_Freq";
        tB_B_Freq.Size = new Size(119, 25);
        tB_B_Freq.TabIndex = 11;
        tB_B_Freq.Click += tB_A_Freq_Click;
        tB_B_Freq.KeyPress += tB_A_Freq_KeyPress;
        tB_B_Freq.Leave += tB_A_Freq_Leave;
        cbB_B_Dir.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_B_Dir.FormattingEnabled = true;
        cbB_B_Dir.Items.AddRange(new object[3] { "OFF", "+", "-" });
        cbB_B_Dir.Location = new Point(123, 334);
        cbB_B_Dir.Margin = new Padding(3, 2, 3, 2);
        cbB_B_Dir.Name = "cbB_B_Dir";
        cbB_B_Dir.Size = new Size(119, 23);
        cbB_B_Dir.TabIndex = 19;
        cbB_B_SignalGroup.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_B_SignalGroup.FormattingEnabled = true;
        cbB_B_SignalGroup.Items.AddRange(new object[20]
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20"
        });
        cbB_B_SignalGroup.Location = new Point(123, 302);
        cbB_B_SignalGroup.Margin = new Padding(3, 2, 3, 2);
        cbB_B_SignalGroup.Name = "cbB_B_SignalGroup";
        cbB_B_SignalGroup.Size = new Size(119, 23);
        cbB_B_SignalGroup.TabIndex = 18;
        cbB_B_Step.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_B_Step.FormattingEnabled = true;
        cbB_B_Step.Items.AddRange(new object[8]
            { "2.5  KHz", "5.0  KHz", "6.25 KHz", "10.0 KHz", "12.5 KHz", "20.0 KHz", "25.0 KHz", "50.0 KHz" });
        cbB_B_Step.Location = new Point(123, 241);
        cbB_B_Step.Margin = new Padding(3, 2, 3, 2);
        cbB_B_Step.Name = "cbB_B_Step";
        cbB_B_Step.Size = new Size(119, 23);
        cbB_B_Step.TabIndex = 17;
        cbB_B_Bandwide.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_B_Bandwide.FormattingEnabled = true;
        cbB_B_Bandwide.Items.AddRange(new object[2] { "宽", "窄" });
        cbB_B_Bandwide.Location = new Point(123, 210);
        cbB_B_Bandwide.Margin = new Padding(3, 2, 3, 2);
        cbB_B_Bandwide.Name = "cbB_B_Bandwide";
        cbB_B_Bandwide.Size = new Size(119, 23);
        cbB_B_Bandwide.TabIndex = 16;
        cbB_B_Power.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_B_Power.FormattingEnabled = true;
        cbB_B_Power.Items.AddRange(new object[3] { "高", "中", "低" });
        cbB_B_Power.Location = new Point(123, 179);
        cbB_B_Power.Margin = new Padding(3, 2, 3, 2);
        cbB_B_Power.Name = "cbB_B_Power";
        cbB_B_Power.Size = new Size(119, 23);
        cbB_B_Power.TabIndex = 15;
        cbB_B_SQMode.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_B_SQMode.FormattingEnabled = true;
        cbB_B_SQMode.Items.AddRange(new object[3] { "QT/DQT", "QT/DQT*DTMF", "QT/DQT+DTMF" });
        cbB_B_SQMode.Location = new Point(123, 148);
        cbB_B_SQMode.Margin = new Padding(3, 2, 3, 2);
        cbB_B_SQMode.Name = "cbB_B_SQMode";
        cbB_B_SQMode.Size = new Size(119, 23);
        cbB_B_SQMode.TabIndex = 14;
        cbB_B_TxCtsDcs.FormattingEnabled = true;
        cbB_B_TxCtsDcs.Location = new Point(123, 85);
        cbB_B_TxCtsDcs.Margin = new Padding(3, 2, 3, 2);
        cbB_B_TxCtsDcs.Name = "cbB_B_TxCtsDcs";
        cbB_B_TxCtsDcs.Size = new Size(119, 23);
        cbB_B_TxCtsDcs.TabIndex = 13;
        cbB_B_TxCtsDcs.Tag = "";
        cbB_B_TxCtsDcs.Click += cbB_A_RxCtsDcs_Click;
        cbB_B_TxCtsDcs.KeyPress += cbB_A_RxCtsDcs_KeyPress;
        cbB_B_TxCtsDcs.Leave += cbB_A_RxCtsDcs_Leave;
        cbB_B_RxCtsDcs.FormattingEnabled = true;
        cbB_B_RxCtsDcs.Location = new Point(123, 54);
        cbB_B_RxCtsDcs.Margin = new Padding(3, 2, 3, 2);
        cbB_B_RxCtsDcs.Name = "cbB_B_RxCtsDcs";
        cbB_B_RxCtsDcs.Size = new Size(119, 23);
        cbB_B_RxCtsDcs.TabIndex = 12;
        cbB_B_RxCtsDcs.Tag = "";
        cbB_B_RxCtsDcs.Click += cbB_A_RxCtsDcs_Click;
        cbB_B_RxCtsDcs.KeyPress += cbB_A_RxCtsDcs_KeyPress;
        cbB_B_RxCtsDcs.Leave += cbB_A_RxCtsDcs_Leave;
        label_B_SQMode.Location = new Point(16, 148);
        label_B_SQMode.Name = "label_B_SQMode";
        label_B_SQMode.Size = new Size(100, 22);
        label_B_SQMode.TabIndex = 9;
        label_B_SQMode.Text = "静音方式";
        label_B_SQMode.TextAlign = ContentAlignment.MiddleRight;
        label_B_Offset.Location = new Point(16, 365);
        label_B_Offset.Name = "label_B_Offset";
        label_B_Offset.Size = new Size(100, 22);
        label_B_Offset.TabIndex = 8;
        label_B_Offset.Text = "频差频率";
        label_B_Offset.TextAlign = ContentAlignment.MiddleRight;
        label_B_Dir.Location = new Point(16, 334);
        label_B_Dir.Name = "label_B_Dir";
        label_B_Dir.Size = new Size(100, 22);
        label_B_Dir.TabIndex = 7;
        label_B_Dir.Text = "频差方向";
        label_B_Dir.TextAlign = ContentAlignment.MiddleRight;
        label_B_SignalGroup.Location = new Point(16, 302);
        label_B_SignalGroup.Name = "label_B_SignalGroup";
        label_B_SignalGroup.Size = new Size(100, 22);
        label_B_SignalGroup.TabIndex = 6;
        label_B_SignalGroup.Text = "信令编码";
        label_B_SignalGroup.TextAlign = ContentAlignment.MiddleRight;
        label_B_Step.Location = new Point(16, 241);
        label_B_Step.Name = "label_B_Step";
        label_B_Step.Size = new Size(100, 22);
        label_B_Step.TabIndex = 5;
        label_B_Step.Text = "步进频率";
        label_B_Step.TextAlign = ContentAlignment.MiddleRight;
        label_B_Bandwide.Location = new Point(16, 210);
        label_B_Bandwide.Name = "label_B_Bandwide";
        label_B_Bandwide.Size = new Size(100, 22);
        label_B_Bandwide.TabIndex = 4;
        label_B_Bandwide.Text = "带宽";
        label_B_Bandwide.TextAlign = ContentAlignment.MiddleRight;
        label_B_Power.Location = new Point(16, 179);
        label_B_Power.Name = "label_B_Power";
        label_B_Power.Size = new Size(100, 22);
        label_B_Power.TabIndex = 3;
        label_B_Power.Text = "功率";
        label_B_Power.TextAlign = ContentAlignment.MiddleRight;
        label_B_TxCtsDcs.Location = new Point(16, 84);
        label_B_TxCtsDcs.Name = "label_B_TxCtsDcs";
        label_B_TxCtsDcs.Size = new Size(100, 22);
        label_B_TxCtsDcs.TabIndex = 2;
        label_B_TxCtsDcs.Text = "发射亚音";
        label_B_TxCtsDcs.TextAlign = ContentAlignment.MiddleRight;
        label_B_RxCtsDcs.Location = new Point(16, 52);
        label_B_RxCtsDcs.Name = "label_B_RxCtsDcs";
        label_B_RxCtsDcs.Size = new Size(100, 22);
        label_B_RxCtsDcs.TabIndex = 1;
        label_B_RxCtsDcs.Text = "接收亚音";
        label_B_RxCtsDcs.TextAlign = ContentAlignment.MiddleRight;
        label_B_Freq.Location = new Point(16, 22);
        label_B_Freq.Name = "label_B_Freq";
        label_B_Freq.Size = new Size(100, 22);
        label_B_Freq.TabIndex = 0;
        label_B_Freq.Text = "频率";
        label_B_Freq.TextAlign = ContentAlignment.MiddleRight;
        cbB_PttID.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_PttID.FormattingEnabled = true;
        cbB_PttID.Items.AddRange(new object[4] { "无", "发射开始", "发射结束", "两者" });
        cbB_PttID.Location = new Point(485, 459);
        cbB_PttID.Margin = new Padding(3, 2, 3, 2);
        cbB_PttID.Name = "cbB_PttID";
        cbB_PttID.Size = new Size(119, 23);
        cbB_PttID.TabIndex = 23;
        label_PTTID.Location = new Point(380, 460);
        label_PTTID.Name = "label_PTTID";
        label_PTTID.Size = new Size(100, 22);
        label_PTTID.TabIndex = 22;
        label_PTTID.Text = "PTT-ID";
        label_PTTID.TextAlign = ContentAlignment.MiddleRight;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(672, 508);
        Controls.Add(cbB_PttID);
        Controls.Add(label_PTTID);
        Controls.Add(groupBox2);
        Controls.Add(groupBox1);
        Icon = (Icon)resources.GetObject("$this.Icon");
        KeyPreview = true;
        Margin = new Padding(3, 2, 3, 2);
        Name = "FormVFOMode";
        StartPosition = FormStartPosition.CenterParent;
        Text = "频率模式";
        FormClosing += FormVFOMode_FormClosing;
        Load += FormVFOMode_Load;
        groupBox1.ResumeLayout(false);
        groupBox1.PerformLayout();
        groupBox2.ResumeLayout(false);
        groupBox2.PerformLayout();
        ResumeLayout(false);
    }
}