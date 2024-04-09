using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SHX_GT12_CPS.View;

public class FormMDC1200 : Form
{
    private readonly IContainer components = null;
    private BackgroundWorker backgroundWorker1;

    private Label label_LocalID;

    private Label label1;
    private MDC1200 mdc1200;

    private TextBox tB_CallID;

    private TextBox tB_LocalID;

    public FormMDC1200(Form parent)
    {
        InitializeComponent();
        MdiParent = parent;
    }

    private void FormMDC1200_Load(object sender, EventArgs e)
    {
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
            mdc1200.GetType().GetProperty(dataMember).SetValue(mdc1200, defaultVal, null);
            c.DataBindings.Add(new Binding(propertyName, dataSource, dataMember, false,
                DataSourceUpdateMode.OnPropertyChanged));
        }
    }

    public void LoadData(MDC1200 mdc)
    {
        mdc1200 = mdc;
        TryToBingdingControl(tB_LocalID, "Text", mdc1200, "Id", "1111");
        TryToBingdingControl(tB_CallID, "Text", mdc1200, "CallID1", "");
    }

    private void FormMDC1200_FormClosing(object sender, FormClosingEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    private void tB_LocalID_KeyPress(object sender, KeyPressEventArgs e)
    {
        e.KeyChar = char.ToUpper(e.KeyChar);
        if ((e.KeyChar < '0' || e.KeyChar > '9') && (e.KeyChar < 'A' || e.KeyChar > 'F') && e.KeyChar != '\b')
            e.Handled = true;
    }

    private void tB_CallID_KeyPress(object sender, KeyPressEventArgs e)
    {
        e.KeyChar = char.ToUpper(e.KeyChar);
        if (e.KeyChar < ' ' || e.KeyChar > '\u007f') e.Handled = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var resources =
            new ComponentResourceManager(typeof(FormMDC1200));
        label_LocalID = new Label();
        tB_LocalID = new TextBox();
        label1 = new Label();
        tB_CallID = new TextBox();
        backgroundWorker1 = new BackgroundWorker();
        SuspendLayout();
        label_LocalID.AutoSize = true;
        label_LocalID.Location = new Point(43, 80);
        label_LocalID.Name = "label_LocalID";
        label_LocalID.Size = new Size(53, 15);
        label_LocalID.TabIndex = 0;
        label_LocalID.Text = "本机ID";
        tB_LocalID.Location = new Point(102, 74);
        tB_LocalID.MaxLength = 4;
        tB_LocalID.Name = "tB_LocalID";
        tB_LocalID.Size = new Size(125, 25);
        tB_LocalID.TabIndex = 1;
        tB_LocalID.KeyPress += tB_LocalID_KeyPress;
        label1.AutoSize = true;
        label1.Location = new Point(29, 37);
        label1.Name = "label1";
        label1.Size = new Size(67, 15);
        label1.TabIndex = 2;
        label1.Text = "本机呼号";
        tB_CallID.Location = new Point(102, 31);
        tB_CallID.MaxLength = 6;
        tB_CallID.Name = "tB_CallID";
        tB_CallID.Size = new Size(125, 25);
        tB_CallID.TabIndex = 3;
        tB_CallID.KeyPress += tB_CallID_KeyPress;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(681, 411);
        Controls.Add(tB_CallID);
        Controls.Add(label1);
        Controls.Add(tB_LocalID);
        Controls.Add(label_LocalID);
        Icon = (Icon)resources.GetObject("$this.Icon");
        KeyPreview = true;
        Name = "FormMDC1200";
        Text = "MDC1200";
        FormClosing += FormMDC1200_FormClosing;
        Load += FormMDC1200_Load;
        ResumeLayout(false);
        PerformLayout();
    }
}