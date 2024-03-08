using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using DevComponents.DotNetBar.Controls;

namespace SQ5R.View;

public class FormDTMF : Form
{
    private readonly IContainer components = null;
    private Button btn_Close;

    private Button btn_Default;
    private DTMFData bufData;

    private CheckBox cb_press;

    private CheckBox cb_release;

    private ComboBoxEx cbB_GroupCall;

    private ComboBoxEx cbB_LastTime;

    private ComboBoxEx cbB_StopTime;

    private GroupBox gB_GroupDTMF;

    private GroupBox gB_IDOfHost;

    private Label label_GroupCall;

    private Label label_lastTime;

    private Label label_StopTime;

    private Label label1;

    private Label label10;

    private Label label11;

    private Label label12;

    private Label label13;

    private Label label14;

    private Label label15;

    private Label label18;

    private Label label19;

    private Label label2;

    private Label label3;

    private Label label4;

    private Label label5;

    private Label label6;

    private Label label7;

    private Label label8;

    private Label label9;

    private string language = "中文";

    private Panel panel1;

    private Panel panel10;

    private Panel panel11;

    private Panel panel12;

    private Panel panel13;

    private Panel panel14;

    private Panel panel15;

    private Panel panel16;

    private Panel panel2;

    private Panel panel3;

    private Panel panel4;

    private Panel panel5;

    private Panel panel6;

    private Panel panel7;

    private Panel panel8;

    private Panel panel9;

    private TextBox tb_DTMF1;

    private TextBox tb_DTMF2;

    private TextBox tb_DTMF3;

    private TextBox tb_DTMF4;

    private TextBox tb_DTMF5;

    private TextBox tb_DTMF6;

    private TextBox tb_DTMF7;

    private TextBox tb_DTMF8;

    private TextBox tb_DTMF9;

    private TextBox tb_DTMFA;

    private TextBox tb_DTMFB;

    private TextBox tb_DTMFC;

    private TextBox tb_DTMFD;

    private TextBox tb_DTMFE;

    private TextBox tb_DTMFF;

    private TextBox tb_IDOfHost;

    public FormDTMF(DTMFData data)
    {
        InitializeComponent();
        StartPosition = FormStartPosition.Manual;
        bingdingDatas(data);
    }

    public static FormDTMF getInstance(Form father, DTMFData data)
    {
        var formDTMF = new FormDTMF(data);
        formDTMF.MdiParent = father;
        return formDTMF;
    }

    private void bingdingDatas(DTMFData dat)
    {
        bufData = dat;
        tb_DTMF1.DataBindings.Add("Text", bufData, "GroupOfDTMF_1", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMF2.DataBindings.Add("Text", bufData, "GroupOfDTMF_2", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMF3.DataBindings.Add("Text", bufData, "GroupOfDTMF_3", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMF4.DataBindings.Add("Text", bufData, "GroupOfDTMF_4", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMF5.DataBindings.Add("Text", bufData, "GroupOfDTMF_5", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMF6.DataBindings.Add("Text", bufData, "GroupOfDTMF_6", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMF7.DataBindings.Add("Text", bufData, "GroupOfDTMF_7", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMF8.DataBindings.Add("Text", bufData, "GroupOfDTMF_8", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMF9.DataBindings.Add("Text", bufData, "GroupOfDTMF_9", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMFA.DataBindings.Add("Text", bufData, "GroupOfDTMF_A", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMFB.DataBindings.Add("Text", bufData, "GroupOfDTMF_B", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMFC.DataBindings.Add("Text", bufData, "GroupOfDTMF_C", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMFD.DataBindings.Add("Text", bufData, "GroupOfDTMF_D", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMFE.DataBindings.Add("Text", bufData, "GroupOfDTMF_E", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_DTMFF.DataBindings.Add("Text", bufData, "GroupOfDTMF_F", false,
            DataSourceUpdateMode.OnPropertyChanged);
        tb_IDOfHost.DataBindings.Add("Text", bufData, "TheIDOfLocalHost", false,
            DataSourceUpdateMode.OnPropertyChanged);
        cb_press.DataBindings.Add("Checked", bufData, "SendOnPTTPressed", false,
            DataSourceUpdateMode.OnPropertyChanged);
        cb_release.DataBindings.Add("Checked", bufData, "SendOnPTTReleased", false,
            DataSourceUpdateMode.OnPropertyChanged);
        cbB_LastTime.DataBindings.Add("SelectedIndex", bufData, "LastTimeSend", false,
            DataSourceUpdateMode.OnPropertyChanged);
        cbB_StopTime.DataBindings.Add("SelectedIndex", bufData, "LastTimeStop", false,
            DataSourceUpdateMode.OnPropertyChanged);
        cbB_GroupCall.DataBindings.Add("SelectedIndex", bufData, "GroupCall", false,
            DataSourceUpdateMode.OnPropertyChanged);
    }

    public void updataBindingDatas(DTMFData dat)
    {
        removeBindings();
        bingdingDatas(dat);
    }

    private void removeBindings()
    {
        tb_DTMF1.DataBindings.RemoveAt(0);
        tb_DTMF2.DataBindings.RemoveAt(0);
        tb_DTMF3.DataBindings.RemoveAt(0);
        tb_DTMF4.DataBindings.RemoveAt(0);
        tb_DTMF5.DataBindings.RemoveAt(0);
        tb_DTMF6.DataBindings.RemoveAt(0);
        tb_DTMF7.DataBindings.RemoveAt(0);
        tb_DTMF8.DataBindings.RemoveAt(0);
        tb_DTMF9.DataBindings.RemoveAt(0);
        tb_DTMFA.DataBindings.RemoveAt(0);
        tb_DTMFB.DataBindings.RemoveAt(0);
        tb_DTMFC.DataBindings.RemoveAt(0);
        tb_DTMFD.DataBindings.RemoveAt(0);
        tb_DTMFE.DataBindings.RemoveAt(0);
        tb_DTMFF.DataBindings.RemoveAt(0);
        tb_IDOfHost.DataBindings.RemoveAt(0);
        cb_press.DataBindings.RemoveAt(0);
        cb_release.DataBindings.RemoveAt(0);
        cbB_LastTime.DataBindings.RemoveAt(0);
        cbB_StopTime.DataBindings.RemoveAt(0);
        cbB_GroupCall.DataBindings.RemoveAt(0);
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

        var componentResourceManager = new ComponentResourceManager(typeof(FormDTMF));
        componentResourceManager.ApplyResources(this, "$this");
        foreach (Control control3 in Controls) componentResourceManager.ApplyResources(control3, control3.Name);

        foreach (Control control4 in gB_IDOfHost.Controls)
            componentResourceManager.ApplyResources(control4, control4.Name);

        ResumeLayout(false);
        Visible = visible;
    }

    private void FormDTMF_FormClosing(object sender, FormClosingEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    private void FormDTMF_KeyPress(object sender, KeyPressEventArgs e)
    {
        if ((e.KeyChar >= '0' && e.KeyChar <= '9') || (e.KeyChar >= 'A' && e.KeyChar <= 'D') ||
            (e.KeyChar >= 'a' && e.KeyChar <= 'd') || e.KeyChar == '*' || e.KeyChar == '#' || e.KeyChar == '\b')
        {
            if (e.KeyChar >= 'a' && e.KeyChar <= 'd') e.KeyChar -= ' ';
        }
        else
        {
            e.Handled = true;
        }
    }

    private void tb_IDOfHost_TextChanged(object sender, EventArgs e)
    {
        var textBox = (TextBox)sender;
        var text = textBox.Text;
        if (text != null && text.Length != 0 && (text[text.Length - 1] < '0' || text[text.Length - 1] > '9'))
            textBox.Text = text.Remove(text.Length - 1, 1);
    }

    private void btn_Default_Click(object sender, EventArgs e)
    {
        var dtmfData = new DTMFData();
        ((FormMain)MdiParent).theRadioData.dtmfData = dtmfData;
        bufData = ((FormMain)MdiParent).theRadioData.dtmfData;
        removeBindings();
        bingdingDatas(bufData);
    }

    private void btn_Close_Click(object sender, EventArgs e)
    {
        Close();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var resources =
            new ComponentResourceManager(typeof(FormDTMF));
        gB_GroupDTMF = new GroupBox();
        panel1 = new Panel();
        tb_DTMF1 = new TextBox();
        panel11 = new Panel();
        tb_DTMFF = new TextBox();
        label1 = new Label();
        panel12 = new Panel();
        tb_DTMFE = new TextBox();
        label2 = new Label();
        panel13 = new Panel();
        tb_DTMFD = new TextBox();
        label3 = new Label();
        panel14 = new Panel();
        tb_DTMFC = new TextBox();
        label4 = new Label();
        panel15 = new Panel();
        tb_DTMFB = new TextBox();
        label5 = new Label();
        panel6 = new Panel();
        tb_DTMFA = new TextBox();
        label6 = new Label();
        panel7 = new Panel();
        tb_DTMF9 = new TextBox();
        label7 = new Label();
        panel8 = new Panel();
        tb_DTMF8 = new TextBox();
        label8 = new Label();
        panel9 = new Panel();
        tb_DTMF7 = new TextBox();
        label9 = new Label();
        panel10 = new Panel();
        tb_DTMF6 = new TextBox();
        label10 = new Label();
        panel5 = new Panel();
        tb_DTMF5 = new TextBox();
        label11 = new Label();
        panel4 = new Panel();
        tb_DTMF4 = new TextBox();
        label12 = new Label();
        panel3 = new Panel();
        tb_DTMF3 = new TextBox();
        label13 = new Label();
        panel2 = new Panel();
        tb_DTMF2 = new TextBox();
        label14 = new Label();
        label15 = new Label();
        gB_IDOfHost = new GroupBox();
        cb_release = new CheckBox();
        cb_press = new CheckBox();
        panel16 = new Panel();
        tb_IDOfHost = new TextBox();
        label_lastTime = new Label();
        label_StopTime = new Label();
        cbB_LastTime = new ComboBoxEx();
        cbB_StopTime = new ComboBoxEx();
        label18 = new Label();
        label19 = new Label();
        btn_Default = new Button();
        btn_Close = new Button();
        label_GroupCall = new Label();
        cbB_GroupCall = new ComboBoxEx();
        gB_GroupDTMF.SuspendLayout();
        panel1.SuspendLayout();
        panel11.SuspendLayout();
        panel12.SuspendLayout();
        panel13.SuspendLayout();
        panel14.SuspendLayout();
        panel15.SuspendLayout();
        panel6.SuspendLayout();
        panel7.SuspendLayout();
        panel8.SuspendLayout();
        panel9.SuspendLayout();
        panel10.SuspendLayout();
        panel5.SuspendLayout();
        panel4.SuspendLayout();
        panel3.SuspendLayout();
        panel2.SuspendLayout();
        gB_IDOfHost.SuspendLayout();
        panel16.SuspendLayout();
        SuspendLayout();
        gB_GroupDTMF.Controls.Add(panel1);
        gB_GroupDTMF.Controls.Add(panel11);
        gB_GroupDTMF.Controls.Add(label1);
        gB_GroupDTMF.Controls.Add(panel12);
        gB_GroupDTMF.Controls.Add(label2);
        gB_GroupDTMF.Controls.Add(panel13);
        gB_GroupDTMF.Controls.Add(label3);
        gB_GroupDTMF.Controls.Add(panel14);
        gB_GroupDTMF.Controls.Add(label4);
        gB_GroupDTMF.Controls.Add(panel15);
        gB_GroupDTMF.Controls.Add(label5);
        gB_GroupDTMF.Controls.Add(panel6);
        gB_GroupDTMF.Controls.Add(label6);
        gB_GroupDTMF.Controls.Add(panel7);
        gB_GroupDTMF.Controls.Add(label7);
        gB_GroupDTMF.Controls.Add(panel8);
        gB_GroupDTMF.Controls.Add(label8);
        gB_GroupDTMF.Controls.Add(panel9);
        gB_GroupDTMF.Controls.Add(label9);
        gB_GroupDTMF.Controls.Add(panel10);
        gB_GroupDTMF.Controls.Add(label10);
        gB_GroupDTMF.Controls.Add(panel5);
        gB_GroupDTMF.Controls.Add(label11);
        gB_GroupDTMF.Controls.Add(panel4);
        gB_GroupDTMF.Controls.Add(label12);
        gB_GroupDTMF.Controls.Add(panel3);
        gB_GroupDTMF.Controls.Add(label13);
        gB_GroupDTMF.Controls.Add(panel2);
        gB_GroupDTMF.Controls.Add(label14);
        gB_GroupDTMF.Controls.Add(label15);
        resources.ApplyResources(gB_GroupDTMF, "gB_GroupDTMF");
        gB_GroupDTMF.Name = "gB_GroupDTMF";
        gB_GroupDTMF.TabStop = false;
        panel1.BorderStyle = BorderStyle.Fixed3D;
        panel1.Controls.Add(tb_DTMF1);
        resources.ApplyResources(panel1, "panel1");
        panel1.Name = "panel1";
        tb_DTMF1.BackColor = SystemColors.Control;
        tb_DTMF1.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMF1, "tb_DTMF1");
        tb_DTMF1.Name = "tb_DTMF1";
        panel11.BorderStyle = BorderStyle.Fixed3D;
        panel11.Controls.Add(tb_DTMFF);
        resources.ApplyResources(panel11, "panel11");
        panel11.Name = "panel11";
        tb_DTMFF.BackColor = SystemColors.Control;
        tb_DTMFF.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMFF, "tb_DTMFF");
        tb_DTMFF.Name = "tb_DTMFF";
        resources.ApplyResources(label1, "label1");
        label1.Name = "label1";
        panel12.BorderStyle = BorderStyle.Fixed3D;
        panel12.Controls.Add(tb_DTMFE);
        resources.ApplyResources(panel12, "panel12");
        panel12.Name = "panel12";
        tb_DTMFE.BackColor = SystemColors.Control;
        tb_DTMFE.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMFE, "tb_DTMFE");
        tb_DTMFE.Name = "tb_DTMFE";
        resources.ApplyResources(label2, "label2");
        label2.Name = "label2";
        panel13.BorderStyle = BorderStyle.Fixed3D;
        panel13.Controls.Add(tb_DTMFD);
        resources.ApplyResources(panel13, "panel13");
        panel13.Name = "panel13";
        tb_DTMFD.BackColor = SystemColors.Control;
        tb_DTMFD.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMFD, "tb_DTMFD");
        tb_DTMFD.Name = "tb_DTMFD";
        resources.ApplyResources(label3, "label3");
        label3.Name = "label3";
        panel14.BorderStyle = BorderStyle.Fixed3D;
        panel14.Controls.Add(tb_DTMFC);
        resources.ApplyResources(panel14, "panel14");
        panel14.Name = "panel14";
        tb_DTMFC.BackColor = SystemColors.Control;
        tb_DTMFC.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMFC, "tb_DTMFC");
        tb_DTMFC.Name = "tb_DTMFC";
        resources.ApplyResources(label4, "label4");
        label4.Name = "label4";
        panel15.BorderStyle = BorderStyle.Fixed3D;
        panel15.Controls.Add(tb_DTMFB);
        resources.ApplyResources(panel15, "panel15");
        panel15.Name = "panel15";
        tb_DTMFB.BackColor = SystemColors.Control;
        tb_DTMFB.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMFB, "tb_DTMFB");
        tb_DTMFB.Name = "tb_DTMFB";
        resources.ApplyResources(label5, "label5");
        label5.Name = "label5";
        panel6.BorderStyle = BorderStyle.Fixed3D;
        panel6.Controls.Add(tb_DTMFA);
        resources.ApplyResources(panel6, "panel6");
        panel6.Name = "panel6";
        tb_DTMFA.BackColor = SystemColors.Control;
        tb_DTMFA.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMFA, "tb_DTMFA");
        tb_DTMFA.Name = "tb_DTMFA";
        resources.ApplyResources(label6, "label6");
        label6.Name = "label6";
        panel7.BorderStyle = BorderStyle.Fixed3D;
        panel7.Controls.Add(tb_DTMF9);
        resources.ApplyResources(panel7, "panel7");
        panel7.Name = "panel7";
        tb_DTMF9.BackColor = SystemColors.Control;
        tb_DTMF9.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMF9, "tb_DTMF9");
        tb_DTMF9.Name = "tb_DTMF9";
        resources.ApplyResources(label7, "label7");
        label7.Name = "label7";
        panel8.BorderStyle = BorderStyle.Fixed3D;
        panel8.Controls.Add(tb_DTMF8);
        resources.ApplyResources(panel8, "panel8");
        panel8.Name = "panel8";
        tb_DTMF8.BackColor = SystemColors.Control;
        tb_DTMF8.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMF8, "tb_DTMF8");
        tb_DTMF8.Name = "tb_DTMF8";
        resources.ApplyResources(label8, "label8");
        label8.Name = "label8";
        panel9.BorderStyle = BorderStyle.Fixed3D;
        panel9.Controls.Add(tb_DTMF7);
        resources.ApplyResources(panel9, "panel9");
        panel9.Name = "panel9";
        tb_DTMF7.BackColor = SystemColors.Control;
        tb_DTMF7.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMF7, "tb_DTMF7");
        tb_DTMF7.Name = "tb_DTMF7";
        resources.ApplyResources(label9, "label9");
        label9.Name = "label9";
        panel10.BorderStyle = BorderStyle.Fixed3D;
        panel10.Controls.Add(tb_DTMF6);
        resources.ApplyResources(panel10, "panel10");
        panel10.Name = "panel10";
        tb_DTMF6.BackColor = SystemColors.Control;
        tb_DTMF6.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMF6, "tb_DTMF6");
        tb_DTMF6.Name = "tb_DTMF6";
        resources.ApplyResources(label10, "label10");
        label10.Name = "label10";
        panel5.BorderStyle = BorderStyle.Fixed3D;
        panel5.Controls.Add(tb_DTMF5);
        resources.ApplyResources(panel5, "panel5");
        panel5.Name = "panel5";
        tb_DTMF5.BackColor = SystemColors.Control;
        tb_DTMF5.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMF5, "tb_DTMF5");
        tb_DTMF5.Name = "tb_DTMF5";
        resources.ApplyResources(label11, "label11");
        label11.Name = "label11";
        panel4.BorderStyle = BorderStyle.Fixed3D;
        panel4.Controls.Add(tb_DTMF4);
        resources.ApplyResources(panel4, "panel4");
        panel4.Name = "panel4";
        tb_DTMF4.BackColor = SystemColors.Control;
        tb_DTMF4.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMF4, "tb_DTMF4");
        tb_DTMF4.Name = "tb_DTMF4";
        resources.ApplyResources(label12, "label12");
        label12.Name = "label12";
        panel3.BorderStyle = BorderStyle.Fixed3D;
        panel3.Controls.Add(tb_DTMF3);
        resources.ApplyResources(panel3, "panel3");
        panel3.Name = "panel3";
        tb_DTMF3.BackColor = SystemColors.Control;
        tb_DTMF3.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMF3, "tb_DTMF3");
        tb_DTMF3.Name = "tb_DTMF3";
        resources.ApplyResources(label13, "label13");
        label13.Name = "label13";
        panel2.BorderStyle = BorderStyle.Fixed3D;
        panel2.Controls.Add(tb_DTMF2);
        resources.ApplyResources(panel2, "panel2");
        panel2.Name = "panel2";
        tb_DTMF2.BackColor = SystemColors.Control;
        tb_DTMF2.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_DTMF2, "tb_DTMF2");
        tb_DTMF2.Name = "tb_DTMF2";
        resources.ApplyResources(label14, "label14");
        label14.Name = "label14";
        resources.ApplyResources(label15, "label15");
        label15.Name = "label15";
        gB_IDOfHost.Controls.Add(cb_release);
        gB_IDOfHost.Controls.Add(cb_press);
        gB_IDOfHost.Controls.Add(panel16);
        resources.ApplyResources(gB_IDOfHost, "gB_IDOfHost");
        gB_IDOfHost.Name = "gB_IDOfHost";
        gB_IDOfHost.TabStop = false;
        cb_release.Checked = true;
        cb_release.CheckState = CheckState.Checked;
        resources.ApplyResources(cb_release, "cb_release");
        cb_release.Name = "cb_release";
        cb_release.UseVisualStyleBackColor = true;
        resources.ApplyResources(cb_press, "cb_press");
        cb_press.Name = "cb_press";
        cb_press.UseVisualStyleBackColor = true;
        panel16.BorderStyle = BorderStyle.Fixed3D;
        panel16.Controls.Add(tb_IDOfHost);
        resources.ApplyResources(panel16, "panel16");
        panel16.Name = "panel16";
        tb_IDOfHost.BackColor = SystemColors.Control;
        tb_IDOfHost.BorderStyle = BorderStyle.None;
        resources.ApplyResources(tb_IDOfHost, "tb_IDOfHost");
        tb_IDOfHost.Name = "tb_IDOfHost";
        tb_IDOfHost.TextChanged += tb_IDOfHost_TextChanged;
        resources.ApplyResources(label_lastTime, "label_lastTime");
        label_lastTime.Name = "label_lastTime";
        resources.ApplyResources(label_StopTime, "label_StopTime");
        label_StopTime.Name = "label_StopTime";
        cbB_LastTime.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_LastTime.FormattingEnabled = true;
        resources.ApplyResources(cbB_LastTime, "cbB_LastTime");
        cbB_LastTime.Items.AddRange(new object[5] { "50ms", "100ms", "200ms", "300ms", "500ms" });
        cbB_LastTime.Name = "cbB_LastTime";
        cbB_StopTime.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_StopTime.FormattingEnabled = true;
        resources.ApplyResources(cbB_StopTime, "cbB_StopTime");
        cbB_StopTime.Items.AddRange(new object[5] { "50ms", "100ms", "200ms", "300ms", "500ms" });
        cbB_StopTime.Name = "cbB_StopTime";
        resources.ApplyResources(label18, "label18");
        label18.Name = "label18";
        resources.ApplyResources(label19, "label19");
        label19.Name = "label19";
        resources.ApplyResources(btn_Default, "btn_Default");
        btn_Default.Name = "btn_Default";
        btn_Default.UseVisualStyleBackColor = true;
        btn_Default.Click += btn_Default_Click;
        resources.ApplyResources(btn_Close, "btn_Close");
        btn_Close.Name = "btn_Close";
        btn_Close.UseVisualStyleBackColor = true;
        btn_Close.Click += btn_Close_Click;
        resources.ApplyResources(label_GroupCall, "label_GroupCall");
        label_GroupCall.Name = "label_GroupCall";
        cbB_GroupCall.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_GroupCall.FormattingEnabled = true;
        cbB_GroupCall.Items.AddRange(new object[16]
        {
            resources.GetString("cbB_GroupCall.Items"),
            resources.GetString("cbB_GroupCall.Items1"),
            resources.GetString("cbB_GroupCall.Items2"),
            resources.GetString("cbB_GroupCall.Items3"),
            resources.GetString("cbB_GroupCall.Items4"),
            resources.GetString("cbB_GroupCall.Items5"),
            resources.GetString("cbB_GroupCall.Items6"),
            resources.GetString("cbB_GroupCall.Items7"),
            resources.GetString("cbB_GroupCall.Items8"),
            resources.GetString("cbB_GroupCall.Items9"),
            resources.GetString("cbB_GroupCall.Items10"),
            resources.GetString("cbB_GroupCall.Items11"),
            resources.GetString("cbB_GroupCall.Items12"),
            resources.GetString("cbB_GroupCall.Items13"),
            resources.GetString("cbB_GroupCall.Items14"),
            resources.GetString("cbB_GroupCall.Items15")
        });
        resources.ApplyResources(cbB_GroupCall, "cbB_GroupCall");
        cbB_GroupCall.Name = "cbB_GroupCall";
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(cbB_GroupCall);
        Controls.Add(label_GroupCall);
        Controls.Add(btn_Close);
        Controls.Add(btn_Default);
        Controls.Add(label19);
        Controls.Add(label18);
        Controls.Add(cbB_StopTime);
        Controls.Add(cbB_LastTime);
        Controls.Add(label_StopTime);
        Controls.Add(label_lastTime);
        Controls.Add(gB_IDOfHost);
        Controls.Add(gB_GroupDTMF);
        KeyPreview = true;
        Name = "FormDTMF";
        FormClosing += FormDTMF_FormClosing;
        KeyPress += FormDTMF_KeyPress;
        gB_GroupDTMF.ResumeLayout(false);
        gB_GroupDTMF.PerformLayout();
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        panel11.ResumeLayout(false);
        panel11.PerformLayout();
        panel12.ResumeLayout(false);
        panel12.PerformLayout();
        panel13.ResumeLayout(false);
        panel13.PerformLayout();
        panel14.ResumeLayout(false);
        panel14.PerformLayout();
        panel15.ResumeLayout(false);
        panel15.PerformLayout();
        panel6.ResumeLayout(false);
        panel6.PerformLayout();
        panel7.ResumeLayout(false);
        panel7.PerformLayout();
        panel8.ResumeLayout(false);
        panel8.PerformLayout();
        panel9.ResumeLayout(false);
        panel9.PerformLayout();
        panel10.ResumeLayout(false);
        panel10.PerformLayout();
        panel5.ResumeLayout(false);
        panel5.PerformLayout();
        panel4.ResumeLayout(false);
        panel4.PerformLayout();
        panel3.ResumeLayout(false);
        panel3.PerformLayout();
        panel2.ResumeLayout(false);
        panel2.PerformLayout();
        gB_IDOfHost.ResumeLayout(false);
        panel16.ResumeLayout(false);
        panel16.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }
}