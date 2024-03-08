using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SHX_GT12_CPS;

public class FormDTMF : Form
{
    private ComboBox cbB_IdleTime;

    private ComboBox cbB_WordTime;

    private DataGridViewTextBoxColumn Column_ID_1;

    private DataGridViewTextBoxColumn Column_ID_2;

    private DataGridViewTextBoxColumn Column_Name_2;

    private DataGridViewTextBoxColumn Column_Name1;

    private DataGridViewTextBoxColumn Column_Word_1;

    private DataGridViewTextBoxColumn Column_Word_2;

    private readonly IContainer components = null;

    private DataGridView dGV_DTMF;
    private DTMF dtmfs;

    private Label label_WordTime;

    private Label label1;

    private string oldStr;

    public FormDTMF(Form parent)
    {
        InitializeComponent();
        MdiParent = parent;
    }

    private void FormDTMF_Load(object sender, EventArgs e)
    {
        DataGridViewRow dataGridViewRow;
        for (var i = 0; i < 10; i++)
        {
            var values = new string[6]
            {
                (i * 2 + 1).ToString(),
                "",
                "",
                (i * 2 + 2).ToString(),
                "",
                ""
            };
            dataGridViewRow = new DataGridViewRow();
            dataGridViewRow.CreateCells(dGV_DTMF);
            dataGridViewRow.SetValues(values);
            dataGridViewRow.Cells[0].ReadOnly = true;
            dataGridViewRow.Cells[3].ReadOnly = true;
            dGV_DTMF.Rows.Add(dataGridViewRow);
        }

        dataGridViewRow = new DataGridViewRow();
        dataGridViewRow.CreateCells(dGV_DTMF);
        dataGridViewRow.SetValues("本机ID", "100", "", "");
        dataGridViewRow.Cells[0].ReadOnly = true;
        dataGridViewRow.Cells[2].ReadOnly = true;
        dataGridViewRow.Cells[3].ReadOnly = true;
        dataGridViewRow.Cells[4].ReadOnly = true;
        dataGridViewRow.Cells[5].ReadOnly = true;
        dGV_DTMF.Rows.Add(dataGridViewRow);
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
            dtmfs.GetType().GetProperty(dataMember).SetValue(dtmfs, defaultVal, null);
            c.DataBindings.Add(new Binding(propertyName, dataSource, dataMember, false,
                DataSourceUpdateMode.OnPropertyChanged));
        }
    }

    public void LoadData(DTMF dtmfs)
    {
        this.dtmfs = dtmfs;
        for (var i = 0; i < 10; i++)
        {
            dGV_DTMF.Rows[i].Cells[1].Value = dtmfs.Group[i * 2];
            dGV_DTMF.Rows[i].Cells[2].Value = dtmfs.GroupName[i * 2];
            dGV_DTMF.Rows[i].Cells[4].Value = dtmfs.Group[i * 2 + 1];
            dGV_DTMF.Rows[i].Cells[5].Value = dtmfs.GroupName[i * 2 + 1];
        }

        dGV_DTMF.Rows[10].Cells[1].Value = dtmfs.LocalID;
        TryToBingdingControl(cbB_WordTime, "SelectedIndex", dtmfs, "WordTime", 1);
        TryToBingdingControl(cbB_IdleTime, "SelectedIndex", dtmfs, "IdleTime", 1);
    }

    private void dGV_DTMF_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (dGV_DTMF.CurrentCell == null) return;

        var text = dGV_DTMF.CurrentCell.Value.ToString();
        var columnIndex = dGV_DTMF.CurrentCell.ColumnIndex;
        var rowIndex = dGV_DTMF.CurrentCell.RowIndex;
        if (text != "")
        {
            var num = columnIndex;
            if (num == 2 || num == 5)
            {
                var bytes = Encoding.GetEncoding("gb2312").GetBytes(text);
                if (bytes.Length > 12) text = Encoding.GetEncoding("gb2312").GetString(bytes, 0, 12);
            }
        }

        switch (columnIndex)
        {
            case 1:
            case 4:
                if (rowIndex == 10 && columnIndex == 1)
                    dtmfs.LocalID = text;
                else if (columnIndex == 1)
                    dtmfs.Group[rowIndex * 2] = text;
                else
                    dtmfs.Group[rowIndex * 2 + 1] = text;

                break;
            case 2:
                dtmfs.GroupName[rowIndex * 2] = text;
                break;
            case 5:
                dtmfs.GroupName[rowIndex * 2 + 1] = text;
                break;
        }
    }

    private void dGV_DTMF_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (dGV_DTMF.CurrentCell != null)
        {
            if (dGV_DTMF.CurrentCell.Value != null)
                oldStr = dGV_DTMF.CurrentCell.Value.ToString();
            else
                oldStr = "";
        }
    }

    private void FormDTMF_KeyPress(object sender, KeyPressEventArgs e)
    {
        e.KeyChar = char.ToUpper(e.KeyChar);
        if (dGV_DTMF.CurrentCell != null)
        {
            var columnIndex = dGV_DTMF.CurrentCell.ColumnIndex;
            var num = columnIndex;
            if ((num == 1 || num == 4) && (e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' &&
                (e.KeyChar != '\r') & (e.KeyChar < 'A' || e.KeyChar > 'D') && e.KeyChar != '*' && e.KeyChar != '#')
                e.Handled = true;
        }
    }

    private void FormDTMF_FormClosing(object sender, FormClosingEventArgs e)
    {
        Hide();
        e.Cancel = true;
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
            new ComponentResourceManager(typeof(FormDTMF));
        dGV_DTMF = new DataGridView();
        Column_ID_1 = new DataGridViewTextBoxColumn();
        Column_Word_1 = new DataGridViewTextBoxColumn();
        Column_Name1 = new DataGridViewTextBoxColumn();
        Column_ID_2 = new DataGridViewTextBoxColumn();
        Column_Word_2 = new DataGridViewTextBoxColumn();
        Column_Name_2 = new DataGridViewTextBoxColumn();
        label_WordTime = new Label();
        label1 = new Label();
        cbB_WordTime = new ComboBox();
        cbB_IdleTime = new ComboBox();
        ((ISupportInitialize)dGV_DTMF).BeginInit();
        SuspendLayout();
        dGV_DTMF.AllowUserToAddRows = false;
        dGV_DTMF.AllowUserToDeleteRows = false;
        dGV_DTMF.AllowUserToResizeColumns = false;
        dGV_DTMF.AllowUserToResizeRows = false;
        dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle.BackColor = Color.FromArgb(249, 249, 249);
        dGV_DTMF.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle;
        dGV_DTMF.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle2.BackColor = SystemColors.Control;
        dataGridViewCellStyle2.Font = new Font("宋体", 9f, FontStyle.Regular,
            GraphicsUnit.Point, 134);
        dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
        dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
        dGV_DTMF.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
        dGV_DTMF.ColumnHeadersHeightSizeMode =
            DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dGV_DTMF.Columns.AddRange(Column_ID_1, Column_Word_1, Column_Name1, Column_ID_2,
            Column_Word_2, Column_Name_2);
        dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle3.BackColor = SystemColors.Window;
        dataGridViewCellStyle3.Font = new Font("宋体", 9f, FontStyle.Regular,
            GraphicsUnit.Point, 134);
        dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
        dGV_DTMF.DefaultCellStyle = dataGridViewCellStyle3;
        dGV_DTMF.EditMode = DataGridViewEditMode.EditOnEnter;
        dGV_DTMF.GridColor = Color.FromArgb(208, 215, 229);
        dGV_DTMF.Location = new Point(12, 12);
        dGV_DTMF.Name = "dGV_DTMF";
        dGV_DTMF.RowHeadersVisible = false;
        dGV_DTMF.RowHeadersWidth = 51;
        dGV_DTMF.RowTemplate.Height = 27;
        dGV_DTMF.Size = new Size(669, 476);
        dGV_DTMF.TabIndex = 0;
        dGV_DTMF.CellClick += dGV_DTMF_CellClick;
        dGV_DTMF.CellEndEdit += dGV_DTMF_CellEndEdit;
        Column_ID_1.HeaderText = "序号";
        Column_ID_1.MinimumWidth = 6;
        Column_ID_1.Name = "Column_ID_1";
        Column_ID_1.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_Word_1.HeaderText = "码";
        Column_Word_1.MaxInputLength = 8;
        Column_Word_1.MinimumWidth = 6;
        Column_Word_1.Name = "Column_Word_1";
        Column_Word_1.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_Name1.HeaderText = "名称";
        Column_Name1.MaxInputLength = 12;
        Column_Name1.MinimumWidth = 6;
        Column_Name1.Name = "Column_Name1";
        Column_Name1.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_ID_2.HeaderText = "序号";
        Column_ID_2.MinimumWidth = 6;
        Column_ID_2.Name = "Column_ID_2";
        Column_ID_2.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_Word_2.HeaderText = "码";
        Column_Word_2.MaxInputLength = 8;
        Column_Word_2.MinimumWidth = 6;
        Column_Word_2.Name = "Column_Word_2";
        Column_Word_2.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_Name_2.HeaderText = "名称";
        Column_Name_2.MaxInputLength = 12;
        Column_Name_2.MinimumWidth = 6;
        Column_Name_2.Name = "Column_Name_2";
        Column_Name_2.SortMode = DataGridViewColumnSortMode.NotSortable;
        label_WordTime.Location = new Point(32, 505);
        label_WordTime.Name = "label_WordTime";
        label_WordTime.Size = new Size(136, 23);
        label_WordTime.TabIndex = 1;
        label_WordTime.Text = "DTMF码持续时间";
        label_WordTime.TextAlign = ContentAlignment.MiddleRight;
        label1.Location = new Point(32, 534);
        label1.Name = "label1";
        label1.Size = new Size(136, 23);
        label1.TabIndex = 2;
        label1.Text = "DTMF码间断时间";
        label1.TextAlign = ContentAlignment.MiddleRight;
        cbB_WordTime.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_WordTime.FormattingEnabled = true;
        cbB_WordTime.Items.AddRange(new object[5] { "50 ms", "100 ms", "200 ms", "300 ms", "500 ms" });
        cbB_WordTime.Location = new Point(174, 505);
        cbB_WordTime.Name = "cbB_WordTime";
        cbB_WordTime.Size = new Size(121, 23);
        cbB_WordTime.TabIndex = 3;
        cbB_IdleTime.DropDownStyle = ComboBoxStyle.DropDownList;
        cbB_IdleTime.FormattingEnabled = true;
        cbB_IdleTime.Items.AddRange(new object[5] { "50 ms", "100 ms", "200 ms", "300 ms", "500 ms" });
        cbB_IdleTime.Location = new Point(174, 534);
        cbB_IdleTime.Name = "cbB_IdleTime";
        cbB_IdleTime.Size = new Size(121, 23);
        cbB_IdleTime.TabIndex = 4;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(693, 570);
        Controls.Add(cbB_IdleTime);
        Controls.Add(cbB_WordTime);
        Controls.Add(label1);
        Controls.Add(label_WordTime);
        Controls.Add(dGV_DTMF);
        Icon = (Icon)resources.GetObject("$this.Icon");
        KeyPreview = true;
        Name = "FormDTMF";
        StartPosition = FormStartPosition.CenterParent;
        Text = "DTMF";
        FormClosing += FormDTMF_FormClosing;
        Load += FormDTMF_Load;
        KeyPress += FormDTMF_KeyPress;
        ((ISupportInitialize)dGV_DTMF).EndInit();
        ResumeLayout(false);
    }
}