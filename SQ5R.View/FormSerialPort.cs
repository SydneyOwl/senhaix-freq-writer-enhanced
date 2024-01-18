using System;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;

namespace SQ5R.View;

public class FormSerialPort : Form
{
    private readonly IContainer components = null;
    private Button btn_Cancel;

    private Button btn_Sure;

    private ComboBoxEx cbB_Port;
    public string portName;

    public FormSerialPort(string language)
    {
        InitializeComponent();
        if (language == "中文")
        {
            Text = "串口号";
            btn_Sure.Text = "确定(&O)";
            btn_Cancel.Text = "取消(&C)";
        }
        else
        {
            Text = "Port";
            btn_Sure.Text = "OK(&O)";
            btn_Cancel.Text = "Cancel(&C)";
        }

        StartPosition = FormStartPosition.CenterScreen;
    }

    private void FormSerialPort_Load(object sender, EventArgs e)
    {
        var portNames = SerialPort.GetPortNames();
        if (portNames.Length != 0)
        {
            cbB_Port.Items.AddRange(portNames);
            if (portName != null)
                cbB_Port.Text = portName;
            else
                cbB_Port.SelectedIndex = 0;
        }
    }

    private void btn_Sure_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btn_Cancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private void cbB_Port_SelectedIndexChanged(object sender, EventArgs e)
    {
        portName = cbB_Port.SelectedItem.ToString();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var resources =
            new ComponentResourceManager(typeof(FormSerialPort));
        cbB_Port = new ComboBoxEx();
        btn_Sure = new Button();
        btn_Cancel = new Button();
        SuspendLayout();
        cbB_Port.DisplayMember = "Text";
        cbB_Port.DrawMode = DrawMode.OwnerDrawFixed;
        cbB_Port.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_Port.FormattingEnabled = true;
        cbB_Port.ItemHeight = 20;
        cbB_Port.Location = new Point(26, 21);
        cbB_Port.Name = "cbB_Port";
        cbB_Port.Size = new Size(204, 26);
        cbB_Port.Style = eDotNetBarStyle.StyleManagerControlled;
        cbB_Port.TabIndex = 0;
        cbB_Port.SelectedIndexChanged += cbB_Port_SelectedIndexChanged;
        btn_Sure.Location = new Point(16, 65);
        btn_Sure.Name = "btn_Sure";
        btn_Sure.Size = new Size(105, 32);
        btn_Sure.TabIndex = 1;
        btn_Sure.Text = "确定(&O)";
        btn_Sure.UseVisualStyleBackColor = true;
        btn_Sure.Click += btn_Sure_Click;
        btn_Cancel.Location = new Point(137, 65);
        btn_Cancel.Name = "btn_Cancel";
        btn_Cancel.Size = new Size(105, 32);
        btn_Cancel.TabIndex = 2;
        btn_Cancel.Text = "取消(&C)";
        btn_Cancel.UseVisualStyleBackColor = true;
        btn_Cancel.Click += btn_Cancel_Click;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(267, 116);
        Controls.Add(btn_Cancel);
        Controls.Add(btn_Sure);
        Controls.Add(cbB_Port);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormSerialPort";
        StartPosition = FormStartPosition.CenterParent;
        Text = "串口号";
        Load += FormSerialPort_Load;
        ResumeLayout(false);
    }
}