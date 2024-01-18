#if NET461
using System.ComponentModel;
using DevComponents.DotNetBar.Controls;

namespace SQ5R.View;

partial class FormConnBluetooth
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
        this.label1 = new System.Windows.Forms.Label();
        this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
        this.btNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.ssid = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.signal = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.mac_addr = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.scanButton = new System.Windows.Forms.Button();
        this.checkBoxDisableSSIDFilter = new System.Windows.Forms.CheckBox();
        this.label2 = new System.Windows.Forms.Label();
        this.ButtonConnectDevice = new System.Windows.Forms.Button();
        this.checkboxDisableWeakSignal = new System.Windows.Forms.CheckBox();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
        this.SuspendLayout();
        // 
        // label1
        // 
        this.label1.BackColor = System.Drawing.Color.Lime;
        this.label1.Location = new System.Drawing.Point(12, 361);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(315, 24);
        this.label1.TabIndex = 0;
        this.label1.Text = "扫描结束后 双击目标设备进行连接";
        // 
        // dataGridViewX1
        // 
        this.dataGridViewX1.AllowUserToAddRows = false;
        this.dataGridViewX1.AllowUserToDeleteRows = false;
        this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.btNum, this.ssid, this.signal, this.mac_addr });
        dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
        dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
        dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
        dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
        dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
        this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle1;
        this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
        this.dataGridViewX1.Location = new System.Drawing.Point(12, 91);
        this.dataGridViewX1.Name = "dataGridViewX1";
        this.dataGridViewX1.RowTemplate.Height = 24;
        this.dataGridViewX1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dataGridViewX1.Size = new System.Drawing.Size(626, 267);
        this.dataGridViewX1.TabIndex = 1;
        this.dataGridViewX1.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellContentClick);
        // 
        // btNum
        // 
        this.btNum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
        this.btNum.HeaderText = "序号";
        this.btNum.Name = "btNum";
        this.btNum.ReadOnly = true;
        this.btNum.Width = 60;
        // 
        // ssid
        // 
        this.ssid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
        this.ssid.HeaderText = "名称";
        this.ssid.Name = "ssid";
        this.ssid.ReadOnly = true;
        this.ssid.Width = 150;
        // 
        // signal
        // 
        this.signal.HeaderText = "信号";
        this.signal.Name = "signal";
        this.signal.ReadOnly = true;
        // 
        // mac_addr
        // 
        this.mac_addr.HeaderText = "MAC";
        this.mac_addr.Name = "mac_addr";
        this.mac_addr.ReadOnly = true;
        // 
        // scanButton
        // 
        this.scanButton.Location = new System.Drawing.Point(651, 182);
        this.scanButton.Name = "scanButton";
        this.scanButton.Size = new System.Drawing.Size(134, 56);
        this.scanButton.TabIndex = 2;
        this.scanButton.Text = "开始扫描";
        this.scanButton.UseVisualStyleBackColor = true;
        this.scanButton.Click += new System.EventHandler(this.scanButton_Click);
        // 
        // checkBoxDisableSSIDFilter
        // 
        this.checkBoxDisableSSIDFilter.Location = new System.Drawing.Point(651, 122);
        this.checkBoxDisableSSIDFilter.Name = "checkBoxDisableSSIDFilter";
        this.checkBoxDisableSSIDFilter.Size = new System.Drawing.Size(147, 34);
        this.checkBoxDisableSSIDFilter.TabIndex = 3;
        this.checkBoxDisableSSIDFilter.Text = "禁用ssid过滤";
        this.checkBoxDisableSSIDFilter.UseVisualStyleBackColor = true;
        // 
        // label2
        // 
        this.label2.BackColor = System.Drawing.Color.Yellow;
        this.label2.Location = new System.Drawing.Point(12, 21);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(421, 42);
        this.label2.TabIndex = 4;
        this.label2.Text = "当串口和蓝牙同时连接到设备时，默认写频对象为蓝牙！\r\n请尽量让手台离电脑近一些！";
        // 
        // ButtonConnectDevice
        // 
        this.ButtonConnectDevice.Location = new System.Drawing.Point(651, 273);
        this.ButtonConnectDevice.Name = "ButtonConnectDevice";
        this.ButtonConnectDevice.Size = new System.Drawing.Size(134, 55);
        this.ButtonConnectDevice.TabIndex = 5;
        this.ButtonConnectDevice.Text = "连接设备";
        this.ButtonConnectDevice.UseVisualStyleBackColor = true;
        this.ButtonConnectDevice.Click += new System.EventHandler(this.ButtonConnectDeviceClick);
        // 
        // checkboxDisableWeakSignal
        // 
        this.checkboxDisableWeakSignal.Location = new System.Drawing.Point(651, 76);
        this.checkboxDisableWeakSignal.Name = "checkboxDisableWeakSignal";
        this.checkboxDisableWeakSignal.Size = new System.Drawing.Size(168, 29);
        this.checkboxDisableWeakSignal.TabIndex = 6;
        this.checkboxDisableWeakSignal.Text = "禁用弱信号限制";
        this.checkboxDisableWeakSignal.UseVisualStyleBackColor = true;
        // 
        // FormConnBluetooth
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(810, 394);
        this.Controls.Add(this.checkboxDisableWeakSignal);
        this.Controls.Add(this.ButtonConnectDevice);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.checkBoxDisableSSIDFilter);
        this.Controls.Add(this.scanButton);
        this.Controls.Add(this.dataGridViewX1);
        this.Controls.Add(this.label1);
        this.Name = "FormConnBluetooth";
        this.Text = "蓝牙连接";
        ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
        this.ResumeLayout(false);
    }

    private System.Windows.Forms.CheckBox checkboxDisableWeakSignal;

    private System.Windows.Forms.Button ButtonConnectDevice;

    private System.Windows.Forms.Label label2;

    private System.Windows.Forms.CheckBox checkBoxDisableSSIDFilter;

    private System.Windows.Forms.Button scanButton;

    private System.Windows.Forms.DataGridViewTextBoxColumn btNum;
    private System.Windows.Forms.DataGridViewTextBoxColumn ssid;
    private System.Windows.Forms.DataGridViewTextBoxColumn signal;
    private System.Windows.Forms.DataGridViewTextBoxColumn mac_addr;

    private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;

    private System.Windows.Forms.Label label1;

    #endregion
}
#endif