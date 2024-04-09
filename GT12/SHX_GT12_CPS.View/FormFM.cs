using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SHX_GT12_CPS.View;

public class FormFM : Form
{
    private readonly IContainer components = null;
    private DataGridViewTextBoxColumn Column_Freq_1;

    private DataGridViewTextBoxColumn Column_Freq_2;

    private DataGridViewTextBoxColumn Column_ID_1;

    private DataGridViewTextBoxColumn Column_ID_2;

    private DataGridView dGV_FM;
    private FMChannel fms;

    private string oldValue = "";

    public FormFM(Form parent)
    {
        InitializeComponent();
        MdiParent = parent;
    }

    private void FormFM_Load(object sender, EventArgs e)
    {
        DataGridViewRow dataGridViewRow;
        for (var i = 0; i < 8; i++)
        {
            var values = new string[4]
            {
                (i * 2 + 1).ToString(),
                "",
                (i * 2 + 2).ToString(),
                ""
            };
            dataGridViewRow = new DataGridViewRow();
            dataGridViewRow.CreateCells(dGV_FM);
            dataGridViewRow.SetValues(values);
            dataGridViewRow.Cells[0].ReadOnly = true;
            dataGridViewRow.Cells[2].ReadOnly = true;
            dGV_FM.Rows.Add(dataGridViewRow);
        }

        dGV_FM.Rows[7].Cells[2].Value = "";
        dGV_FM.Rows[7].Cells[2].ReadOnly = true;
        dGV_FM.Rows[7].Cells[3].ReadOnly = true;
        dataGridViewRow = new DataGridViewRow();
        dataGridViewRow.CreateCells(dGV_FM);
        dataGridViewRow.SetValues("当前频率", "90.4", "", "");
        dataGridViewRow.Cells[0].ReadOnly = true;
        dataGridViewRow.Cells[2].ReadOnly = true;
        dataGridViewRow.Cells[3].ReadOnly = true;
        dGV_FM.Rows.Add(dataGridViewRow);
    }

    public void LoadData(FMChannel fms)
    {
        var num = 0;
        var num2 = 0;
        var text = "";
        this.fms = fms;
        for (var i = 0; i < 15; i++)
        {
            num = i / 2;
            num2 = i % 2;
            if (fms.Channels[i] == 0)
            {
                dGV_FM.Rows[num].Cells[num2 * 2 + 1].Value = "";
                continue;
            }

            text = fms.Channels[i].ToString();
            text = text.Insert(text.Length - 1, ".");
            dGV_FM.Rows[num].Cells[num2 * 2 + 1].Value = text;
        }

        if (fms.CurFreq == 0)
        {
            dGV_FM.Rows[8].Cells[1].Value = "";
            return;
        }

        text = fms.CurFreq.ToString();
        text = text.Insert(text.Length - 1, ".");
        dGV_FM.Rows[8].Cells[1].Value = text;
    }

    private void dGV_FM_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        var columnIndex = dGV_FM.CurrentCell.ColumnIndex;
        var rowIndex = dGV_FM.CurrentCell.RowIndex;
        if (columnIndex == 0 || columnIndex == 2)
        {
            dGV_FM.CurrentCell = dGV_FM.Rows[rowIndex].Cells[columnIndex + 1];
            dGV_FM.BeginEdit(false);
        }

        oldValue = dGV_FM.CurrentCell.Value.ToString();
    }

    private void dGV_FM_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        var num = 0;
        var num2 = 0;
        var num3 = 0;
        if (dGV_FM.CurrentCell == null) return;

        num = dGV_FM.CurrentCell.RowIndex;
        num2 = dGV_FM.CurrentCell.ColumnIndex;
        var text = dGV_FM.CurrentCell.Value.ToString();
        if (text != "")
        {
            var array = text.Split('.');
            var list = new List<int>();
            list.Add(int.Parse(array[0]));
            if (array.Length > 1)
            {
                list.Add(int.Parse(array[1]));
                num3 = list[0] * 10 + list[1];
            }
            else
            {
                num3 = list[0] * 10;
            }

            if (num3 < 650 || num3 >= 1080)
            {
                MessageBox.Show("频率范围: 65 - 108MHz", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                dGV_FM.CurrentCell.Value = oldValue;
                return;
            }

            text = num3.ToString();
            dGV_FM.CurrentCell.Value = text.Insert(text.Length - 1, ".");
        }
        else
        {
            num3 = 0;
        }

        switch (num2)
        {
            case 1:
                if (num != 8)
                    fms.Channels[num * 2] = num3;
                else
                    fms.CurFreq = num3;

                break;
            case 3:
                fms.Channels[num * 2 + 1] = num3;
                break;
        }
    }

    private void FormFM_FormClosing(object sender, FormClosingEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    private void dGV_FM_KeyPress(object sender, KeyPressEventArgs e)
    {
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if ((keyData < Keys.NumPad0 || keyData > Keys.NumPad9) && keyData != Keys.Back && keyData != Keys.Decimal &&
            keyData != Keys.Return)
            return true;

        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var dataGridViewCellStyle =
            new DataGridViewCellStyle();
        var dataGridViewCellStyle2 =
            new DataGridViewCellStyle();
        var dataGridViewCellStyle3 =
            new DataGridViewCellStyle();
        var resources =
            new ComponentResourceManager(typeof(FormFM));
        dGV_FM = new DataGridView();
        Column_ID_1 = new DataGridViewTextBoxColumn();
        Column_Freq_1 = new DataGridViewTextBoxColumn();
        Column_ID_2 = new DataGridViewTextBoxColumn();
        Column_Freq_2 = new DataGridViewTextBoxColumn();
        ((ISupportInitialize)dGV_FM).BeginInit();
        SuspendLayout();
        dGV_FM.AllowUserToAddRows = false;
        dGV_FM.AllowUserToDeleteRows = false;
        dGV_FM.AllowUserToResizeColumns = false;
        dGV_FM.AllowUserToResizeRows = false;
        dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle.BackColor = Color.FromArgb(249, 249, 249);
        dGV_FM.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle;
        dGV_FM.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle2.BackColor = SystemColors.Control;
        dataGridViewCellStyle2.Font = new Font("宋体", 9f, FontStyle.Regular,
            GraphicsUnit.Point, 134);
        dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
        dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
        dGV_FM.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
        dGV_FM.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dGV_FM.Columns.AddRange(Column_ID_1, Column_Freq_1, Column_ID_2, Column_Freq_2);
        dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle3.BackColor = SystemColors.Window;
        dataGridViewCellStyle3.Font = new Font("宋体", 9f, FontStyle.Regular,
            GraphicsUnit.Point, 134);
        dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
        dGV_FM.DefaultCellStyle = dataGridViewCellStyle3;
        dGV_FM.EditMode = DataGridViewEditMode.EditOnEnter;
        dGV_FM.GridColor = Color.FromArgb(208, 215, 229);
        dGV_FM.Location = new Point(12, 12);
        dGV_FM.MultiSelect = false;
        dGV_FM.Name = "dGV_FM";
        dGV_FM.RowHeadersVisible = false;
        dGV_FM.RowHeadersWidth = 51;
        dGV_FM.RowTemplate.Height = 27;
        dGV_FM.Size = new Size(511, 532);
        dGV_FM.TabIndex = 0;
        dGV_FM.CellClick += dGV_FM_CellClick;
        dGV_FM.CellEndEdit += dGV_FM_CellEndEdit;
        Column_ID_1.HeaderText = "序号";
        Column_ID_1.MinimumWidth = 6;
        Column_ID_1.Name = "Column_ID_1";
        Column_ID_1.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_Freq_1.HeaderText = "频点";
        Column_Freq_1.MaxInputLength = 6;
        Column_Freq_1.MinimumWidth = 6;
        Column_Freq_1.Name = "Column_Freq_1";
        Column_Freq_1.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_ID_2.HeaderText = "序号";
        Column_ID_2.MinimumWidth = 6;
        Column_ID_2.Name = "Column_ID_2";
        Column_ID_2.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_Freq_2.HeaderText = "频点";
        Column_Freq_2.MaxInputLength = 6;
        Column_Freq_2.MinimumWidth = 6;
        Column_Freq_2.Name = "Column_Freq_2";
        Column_Freq_2.SortMode = DataGridViewColumnSortMode.NotSortable;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(535, 556);
        Controls.Add(dGV_FM);
        Icon = (Icon)resources.GetObject("$this.Icon");
        KeyPreview = true;
        Name = "FormFM";
        StartPosition = FormStartPosition.CenterParent;
        Text = "收音机";
        FormClosing += FormFM_FormClosing;
        Load += FormFM_Load;
        ((ISupportInitialize)dGV_FM).EndInit();
        ResumeLayout(false);
    }
}