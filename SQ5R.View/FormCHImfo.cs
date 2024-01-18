using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using DevComponents.DotNetBar.Controls;

namespace SQ5R.View;

public class FormCHImfo : Form
{
    private readonly string[][] clearChData = new string[128][];

    private readonly IContainer components = null;

    // (row, col, data)
    private readonly Stack<string[][]> dataHistory = new();

    private readonly string[][] defaultChData = new string[128][];
    private readonly int lastCindex = -1;
    private readonly string lastData = "";

    private readonly int lastRindex = -1;
    public Button btn_Clear;

    public Button btn_clr_channel;


    private Button btn_default;

    public Button btn_ins_channel;

    public Button btn_recall_operation;

    public Button btn_refresh;

    public Button btn_remove_current_channel;

    public Button btn_rmEmpty;
    private string[][] channelData;

    private DataGridViewComboBoxColumn col_addonscan;

    private DataGridViewComboBoxColumn col_bandw;

    private DataGridViewTextBoxColumn col_chnName;

    private DataGridViewTextBoxColumn col_chnNum;

    private DataGridViewComboBoxColumn col_enableTx;

    private DataGridViewComboBoxColumn col_encrypt;

    private DataGridViewComboBoxColumn col_lockonbusy;

    private DataGridViewComboBoxColumn col_pttid;

    private DataGridViewComboBoxExColumn col_QTDQT;

    private DataGridViewComboBoxExColumn col_QTDTQ;

    private DataGridViewTextBoxColumn col_rxFreq;

    private DataGridViewComboBoxColumn col_signcode;

    private DataGridViewTextBoxColumn col_txFreq;

    private DataGridViewComboBoxColumn col_txPwr;
    private ContextMenuStrip contextMenuStrip;

    private int countsOfChannel_New = 128;

    private int countsOfChannel_Old = 128;
    private string[] currentCache;

    public DataGridViewX dGV;


    private Rectangle dragBoxFromMouseDown;

    private string language = "中文";
    private ToolStripItem RCclearThisChannel;
    private ToolStripItem RCcopyChannel;
    private ToolStripItem RCcutChannel;
    private ToolStripItem RCdelThisChannel;
    private ToolStripItem RCinsertAfterChannel;
    private ToolStripItem RCpasteChannel;
    private int rowIndexFromMouseDown;
    private int rowIndexOfItemUnderMouseToDrop;

    private short theMaxFreqForUHF = 520;

    private short theMaxFreqForVHF = 180;

    private short theMinFreqForUHF = 100;

    private short theMinFreqForVHF = 100;

    // DSEOF

    public FormCHImfo(string[][] data)
    {
        channelData = data;
        InitializeComponent();
        StartPosition = FormStartPosition.Manual;
        for (var i = 0; i < 128; i++)
        {
            var dataGridViewRow = new DataGridViewRow();
            dataGridViewRow.CreateCells(dGV);
            dataGridViewRow.SetValues(i);
            dataGridViewRow.ReadOnly = true;
            dataGridViewRow.Cells[2].ReadOnly = false;
            dGV.Rows.Add(dataGridViewRow);
            defaultChData[i] = new string[14];
            defaultChData[i][0] = i.ToString();
            channelData[i] = new string[14];
            channelData[i][0] = i.ToString();
            clearChData[i] = new string[14];
            clearChData[i][0] = i.ToString();
        }

        defaultChData[1] = new string[14]
        {
            "1", "Yes", "136.10000", "OFF", "136.10000", "OFF", "H", "W", "OFF", "OFF",
            "ON", "1", "感谢使用", "OFF"
        };
        defaultChData[2] = new string[14]
        {
            "2", "Yes", "173.95000", "OFF", "173.95000", "OFF", "H", "W", "OFF", "OFF",
            "ON", "1", "TU", "OFF"
        };
        defaultChData[3] = new string[14]
        {
            "3", "Yes", "400.02500", "OFF", "400.02500", "OFF", "H", "W", "OFF", "OFF",
            "ON", "1", "73", "OFF"
        };
        defaultChData[4] = new string[14]
        {
            "4", "Yes", "479.95000", "OFF", "479.95000", "OFF", "L", "W", "OFF", "OFF",
            "ON", "1", "E", "OFF"
        };
        defaultChData[5] = new string[14]
        {
            "5", "Yes", "436.50000", "250.3", "436.50000", "250.3", "L", "W", "OFF", "OFF",
            "ON", "1", "E", "OFF"
        };
        defaultChData[6] = new string[14]
        {
            "6", "Yes", "155.50000", "D754I", "155.50000", "D754I", "L", "W", "OFF", "OFF",
            "ON", "1", "", "OFF"
        };
        // recordStep();
    }

    // now support drag
    private void dataGridView_MouseMove(object sender, MouseEventArgs e)
    {
        if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            // If the mouse moves outside the rectangle, start the drag.
            if (dragBoxFromMouseDown != Rectangle.Empty &&
                !dragBoxFromMouseDown.Contains(e.X, e.Y))
            {
                // Proceed with the drag and drop, passing in the list item.                    
                var dropEffect = dGV.DoDragDrop(
                    dGV.Rows[rowIndexFromMouseDown],
                    DragDropEffects.Move);
            }
    }

    private void dataGridView_MouseDown(object sender, MouseEventArgs e)
    {
        // Get the index of the item the mouse is below.
        rowIndexFromMouseDown = dGV.HitTest(e.X, e.Y).RowIndex;
        if (rowIndexFromMouseDown != -1)
        {
            // Remember the point where the mouse down occurred. 
            // The DragSize indicates the size that the mouse can move 
            // before a drag event should be started.                
            var dragSize = SystemInformation.DragSize;

            // Create a rectangle using the DragSize, with the mouse position being
            // at the center of the rectangle.
            dragBoxFromMouseDown = new Rectangle(new Point(e.X - dragSize.Width / 2,
                    e.Y - dragSize.Height / 2),
                dragSize);
        }
        else
            // Reset the rectangle if the mouse is not over an item in the ListBox.
        {
            dragBoxFromMouseDown = Rectangle.Empty;
        }
    }

    private void dataGridView_DragOver(object sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.Move;
    }

    private void dataGridView_DragDrop(object sender, DragEventArgs e)
    {
        // The mouse locations are relative to the screen, so they must be 
        // converted to client coordinates.
        var clientPoint = dGV.PointToClient(new Point(e.X, e.Y));

        // Get the row index of the item the mouse is below. 
        rowIndexOfItemUnderMouseToDrop =
            dGV.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

        // If the drag operation was a move then remove and insert the row.
        if (e.Effect == DragDropEffects.Move)
        {
            // DataGridViewRow rowToMove = e.Data.GetData(
            //     typeof(DataGridViewRow)) as DataGridViewRow;
            // dGV.Rows.RemoveAt(rowIndexFromMouseDown);
            // dGV.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);
            // Console.WriteLine(rowIndexFromMouseDown);
            // Console.WriteLine(rowIndexOfItemUnderMouseToDrop);
            dGV.CurrentRow.ReadOnly = true;
            swapChannelData(rowIndexFromMouseDown, rowIndexOfItemUnderMouseToDrop);
            updateChannelIndex();
            updateFormByChanData();
            dGV.CurrentRow.ReadOnly = false;
            recordStep();
        }
    }

    private void swapChannelData(int rowA, int rowB)
    {
        var a = channelData[rowA];
        var b = channelData[rowB];
        channelData[rowA] = b;
        channelData[rowB] = a;
    }

    public static FormCHImfo getInstance(Form father, string[][] data)
    {
        var formCHImfo = new FormCHImfo(data);
        formCHImfo.MdiParent = father;
        return formCHImfo;
    }

    private void FormCHImfo_Load(object sender, EventArgs e)
    {
    }

    private void FormCHImfo_FormClosing(object sender, FormClosingEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    // Some new functions...

    // bind rightclick..
    private void dGView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right && e.ColumnIndex > -1 && e.RowIndex > -1)
        {
            dGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            contextMenuStrip.Show(MousePosition.X, MousePosition.Y);
        }
    }

    private bool isEmpty(int channelRowIndex)
    {
        // Deprecated
        // for (int i = 0; i < 14; i++)
        // {
        //     var tmp = channelData[channelRowIndex][i];
        //     if (!string.IsNullOrEmpty(tmp))
        //     {
        //         return false;
        //     }
        // }


        return string.IsNullOrEmpty(channelData[channelRowIndex][1]);
    }

    // Context
    private void ctx_copy_click(object sender, EventArgs e)
    {
        var current_row = dGV.CurrentCell.RowIndex;
        currentCache = channelData[current_row];
    }

    private void ctx_cut_click(object sender, EventArgs e)
    {
        var current_row = dGV.CurrentCell.RowIndex;
        dGV.CurrentCell.ReadOnly = true;
        currentCache = channelData[current_row];
        channelData[current_row] = new string[14];
        updateChannelIndex();
        updateFormByChanData();
        dGV.CurrentCell.ReadOnly = false;
        recordStep();
    }


    private void ctx_paste_click(object sender, EventArgs e)
    {
        if (currentCache == null)
        {
            MessageBox.Show("还没有进行复制或剪切操作");
            return;
        }

        var current_row = dGV.CurrentCell.RowIndex;
        currentCache.CopyTo(channelData[current_row], 0);
        updateChannelIndex();
        updateFormByChanData();
        recordStep();
    }

    // Button...

    private void btn_switch_order_Click(object sender, EventArgs e)
    {
    }

    private void btn_refresh_click(object sender, EventArgs e)
    {
        updateFormByChanData();
    }

    private void btn_removeCurrent_Click(object sender, EventArgs e)
    {
        var current_row = dGV.CurrentCell.RowIndex;
        for (var i = current_row; i < 127; i++) channelData[i] = channelData[i + 1];

        channelData[127] = new string[14];
        updateChannelIndex();

        updateFormByChanData();
        recordStep();
    }

    private void btn_insertEmptyChannel_Click(object sender, EventArgs e)
    {
        var current_row = dGV.CurrentCell.RowIndex;
        if (!isEmpty(127))
        {
            MessageBox.Show("信道127不为空，无法插入");
            return;
        }

        // find last empty index
        var lastEmp = 0;
        for (var i = 0; i < 127; i++)
            if (!isEmpty(i))
                lastEmp = i;

        for (var i = lastEmp; i > current_row; i--) channelData[i + 1] = channelData[i];

        channelData[current_row + 1] = new string[14];
        updateChannelIndex();

        updateFormByChanData();
        recordStep();
    }

    private void btn_rmEmpty_Click(object sender, EventArgs e)
    {
        var cached_channel = new string[128][];
        var channel_cursor = 0;
        for (var i = 0; i < 128; i++)
            //check it via "TxAllow"
            if (!isEmpty(i))
            {
                cached_channel[channel_cursor] = new string[14];
                channelData[i].CopyTo(cached_channel[channel_cursor++], 0);
            }

        for (var i = 0; i < channel_cursor; i++)
        {
            cached_channel[i].CopyTo(channelData[i], 0);
            channelData[i][0] = i.ToString();
            dGV.Rows[i].SetValues(channelData[i]);
            dGV.Rows[i].ReadOnly = false;
        }

        for (var i = channel_cursor; i < 128; i++)
        {
            channelData[i] = new string[14];
            channelData[i][0] = i.ToString();
            dGV.Rows[i].SetValues(channelData[i]);
            dGV.Rows[i].ReadOnly = true;
            dGV.Rows[i].Cells[2].ReadOnly = false;
        }

        recordStep();
    }

    private void btn_recall_Click(object sender, EventArgs e)
    {
        // Console.WriteLine(dataHistory.Count);
        if (dataHistory.Count == 0)
        {
            MessageBox.Show("已经到最后一步了");
            return;
        }

        var lastRecall = dataHistory.Pop();
        dGV.Rows.Clear();
        for (var i = 0; i < 128; i++) dGV.Rows.Insert(i, lastRecall[i]);

        for (var i = 0; i < 128; i++)
        for (var j = 0; j < 14; j++)
            channelData[i][j] = lastRecall[i][j];
        // var lastR = lastRecall[0];
        // var lastC = lastRecall[1];
        // var lastD = lastRecall[2];
        // var tmpChannel = (string[])channelData[int.Parse(lastR)].Clone();
        // tmpChannel[int.Parse(lastC)] = lastD;
        // dGV.Rows.RemoveAt(int.Parse(lastR));
        // dGV.Rows.Insert(int.Parse(lastR), tmpChannel);
        // channelData[int.Parse(lastR)][int.Parse(lastC)] = lastD;
        // Console.WriteLine("Recalling:");
        // Console.Write(int.Parse(lastR));
        // Console.Write(int.Parse(lastC));
        // Console.Write(lastD);
    }

    private void btn_clearChan_Click(object sender, EventArgs e)
    {
        var clrRow = dGV.CurrentCell.RowIndex;
        dGV.CurrentCell.ReadOnly = true;
        channelData[clrRow] = new string[14];
        updateChannelIndex();
        updateFormByChanData();
        dGV.CurrentCell.ReadOnly = false;
        recordStep();
    }

    // binds...
    private void col_change(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex == lastCindex && e.RowIndex == lastRindex &&
            dGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == lastData)
            // Do nothing if nothing changes...
            return;

        // lastCindex = e.ColumnIndex;
        // lastRindex = e.RowIndex;
        // lastData = dGV.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
        // dataHistory.Push(new[] { lastRindex.ToString(), lastCindex.ToString(), lastData });
        recordStep();
    }

    private void recordStep()
    {
        var owlcache = new string[128][];
        for (var i = 0; i < 128; i++)
        {
            owlcache[i] = new string[14];
            for (var j = 0; j < 14; j++) owlcache[i][j] = channelData[i][j];
        }

        dataHistory.Push(owlcache);
    }

    private void updateFormByChanData()
    {
        dGV.Rows.Clear();
        for (var i = 0; i < 128; i++)
        {
            dGV.Rows.Insert(i, channelData[i]);
            if (isEmpty(i))
            {
                dGV.Rows[i].ReadOnly = true;
                dGV.Rows[i].Cells[2].ReadOnly = false;
            }
        }
    }

    private void updateChannelIndex()
    {
        for (var i = 0; i < 128; i++) channelData[i][0] = i.ToString();
    }

    // EOF
    private void btn_default_Click(object sender, EventArgs e)
    {
        dGV.CurrentCell = null;
        for (var i = 0; i < 128; i++)
        for (var j = 0; j < 14; j++)
            channelData[i][j] = defaultChData[i][j];

        for (var k = 0; k < countsOfChannel_New; k++)
        {
            defaultChData[k][0] = k.ToString();
            dGV.Rows[k].SetValues(channelData[k]);
        }

        if (countsOfChannel_New > 7)
            for (var l = 1; l < 7; l++)
            {
                dGV.Rows[l].ReadOnly = false;
                dGV.Rows[l].Cells[0].ReadOnly = true;
            }
        else if (countsOfChannel_New > 0)
            for (var m = 1; m < countsOfChannel_New; m++)
            {
                dGV.Rows[m].ReadOnly = false;
                dGV.Rows[m].Cells[0].ReadOnly = true;
            }

        recordStep();
    }

    private void btn_Clear_Click(object sender, EventArgs e)
    {
        dGV.CurrentCell = null;
        channelData = new string[128][];
        for (var i = 0; i < 128; i++) channelData[i] = new string[14];

        for (var j = 0; j < countsOfChannel_New; j++)
        {
            dGV.Rows[j].SetValues(channelData[j]);
            dGV.Rows[j].ReadOnly = true;
            dGV.Rows[j].Cells[2].ReadOnly = false;
        }

        recordStep();
    }

    private void dGV_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        try
        {
            if (dGV.CurrentCell == null) return;

            var rowIndex = dGV.CurrentCell.RowIndex;
            var columnIndex = dGV.CurrentCell.ColumnIndex;
            var text = Convert.ToString(dGV.CurrentCell.Value);
            switch (columnIndex)
            {
                case 2:
                case 4:
                {
                    var flag = false;
                    var flag2 = false;
                    if (text != "" && text != null)
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

                            if (language == "中文")
                                MessageBox.Show("请输入正确的频率!");
                            else
                                MessageBox.Show("Frequence's format is error!");

                            dGV.CurrentCell.Value = channelData[rowIndex][columnIndex];
                            flag2 = false;
                            break;
                        }

                        flag2 = true;
                    }

                    if (flag2)
                    {
                        var num4 = double.Parse(text);
                        if (num4 < theMinFreqForUHF || num4 > theMaxFreqForUHF)
                        {
                            if (language == "中文")
                                MessageBox.Show("频率错误!\n频率范围:" + theMinFreqForUHF + "--" + theMaxFreqForUHF);
                            else
                                MessageBox.Show("Frequence is error!\nFreq Range:" + theMinFreqForUHF + "--" +
                                                theMaxFreqForUHF);

                            dGV.CurrentCell.Value = channelData[rowIndex][columnIndex];
                            break;
                        }

                        if (text.Length < 9)
                        {
                            for (var j = text.Length; j < 9; j++)
                                text = j != 3 ? text.Insert(j, "0") : text.Insert(j, ".");

                            dGV.CurrentCell.Value = text;
                        }

                        var s = text.Replace(".", "");
                        var num5 = uint.Parse(s);
                        if (num5 % 125 != 0)
                        {
                            var num6 = (ushort)(num5 % 125);
                            ushort num7 = 0;
                            num7 = 125;
                            var num8 = num5 / num7;
                            text = (num8 * num7).ToString();
                        }

                        channelData[rowIndex][columnIndex] = text;
                        dGV.CurrentCell.Value = text;
                    }

                    if (dGV.Rows[rowIndex].Cells[1].ReadOnly)
                    {
                        if (dGV.CurrentCell.Value != null && dGV.CurrentCell.Value.ToString() != "")
                        {
                            var text4 = dGV.CurrentCell.Value.ToString();
                            var array = new string[14]
                            {
                                rowIndex.ToString(),
                                "Yes",
                                text4,
                                "OFF",
                                text4,
                                "OFF",
                                "H",
                                "W",
                                "OFF",
                                "OFF",
                                "ON",
                                "1",
                                "",
                                "OFF"
                            };
                            for (var k = 0; k < 14; k++) channelData[rowIndex][k] = array[k];

                            dGV.Rows[rowIndex].SetValues(channelData[rowIndex]);
                            dGV.Rows[rowIndex].ReadOnly = false;
                            dGV.Rows[rowIndex].Cells[0].ReadOnly = true;
                        }
                    }
                    else
                    {
                        if (!(text == "") && text != null) break;

                        if (columnIndex == 2)
                        {
                            for (var l = 1; l < 14; l++) channelData[rowIndex][l] = "";

                            dGV.Rows[rowIndex].SetValues(channelData[rowIndex]);
                            dGV.Rows[rowIndex].Cells[0].ReadOnly = true;
                            dGV.Rows[rowIndex].Cells[1].ReadOnly = true;
                            for (var m = 3; m < 14; m++) dGV.Rows[rowIndex].Cells[m].ReadOnly = true;
                        }
                        else
                        {
                            channelData[rowIndex][columnIndex] = "";
                            dGV.CurrentCell.Value = "";
                        }
                    }

                    break;
                }
                case 3:
                case 5:
                {
                    var num = -1;
                    if (dGV.CurrentCell.Value != null)
                    {
                        num = col_QTDTQ.Items.IndexOf(dGV.CurrentCell.Value);
                        if (num == -1)
                        {
                            if (text[0] != 'D')
                                try
                                {
                                    var num2 = double.Parse(dGV.CurrentCell.Value.ToString());
                                    if (num2 >= 60.0 && num2 <= 260.0)
                                    {
                                        var text2 = num2.ToString();
                                        var num3 = text2.IndexOf('.');
                                        if (num3 == -1)
                                            text2 += ".0";
                                        else if (num3 == text.Length - 1)
                                            text2 += "0";
                                        else if (num3 != text.Length - 2)
                                            text2 = text2.Remove(num3 + 2, text2.Length - 1 - (num3 + 1));

                                        if (columnIndex == 3)
                                        {
                                            channelData[rowIndex][3] = text2;
                                            channelData[rowIndex][5] = channelData[rowIndex][3];
                                            dGV.Rows[rowIndex].Cells[5].Value = channelData[rowIndex][5];
                                            dGV.Rows[rowIndex].Cells[3].Value = channelData[rowIndex][5];
                                        }
                                        else
                                        {
                                            channelData[rowIndex][5] = text2;
                                            dGV.Rows[rowIndex].Cells[5].Value = channelData[rowIndex][5];
                                        }
                                    }
                                    else
                                    {
                                        dGV.CurrentCell.Value = channelData[rowIndex][columnIndex];
                                    }

                                    break;
                                }
                                catch
                                {
                                    dGV.CurrentCell.Value = channelData[rowIndex][columnIndex];
                                    break;
                                }

                            dGV.CurrentCell.Value = channelData[rowIndex][columnIndex];
                        }
                        else if (columnIndex == 3)
                        {
                            channelData[rowIndex][3] = dGV.CurrentCell.Value.ToString();
                            channelData[rowIndex][5] = channelData[rowIndex][3];
                            dGV.Rows[rowIndex].Cells[5].Value = channelData[rowIndex][5];
                        }
                        else
                        {
                            channelData[rowIndex][5] = dGV.CurrentCell.Value.ToString();
                        }
                    }
                    else
                    {
                        dGV.CurrentCell.Value = channelData[rowIndex][columnIndex];
                    }

                    break;
                }
                case 12:
                    channelData[rowIndex][columnIndex] = Convert.ToString(dGV.CurrentCell.Value);
                    break;
                default:
                    channelData[rowIndex][columnIndex] = dGV.CurrentCell.Value.ToString();
                    break;
            }
        }
        catch (Exception ex)
        {
            // buggy function.. do nothing
            // MessageBox.Show("出了点小问题...请继续操作吧");
        }
    }

    private void FormCHImfo_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (dGV.CurrentCell == null) return;

        switch (dGV.CurrentCell.ColumnIndex)
        {
            case 2:
            case 4:
                if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.' && e.KeyChar != '\r')
                    e.Handled = true;

                break;
            case 3:
            case 5:
                if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.' &&
                    e.KeyChar != '\r' && e.KeyChar != 'D' && e.KeyChar != 'I' && e.KeyChar != 'N')
                    e.Handled = true;

                break;
        }
    }

    public void upDataChData(string[][] dat)
    {
        channelData = dat;
        for (var i = 0; i < countsOfChannel_New; i++)
        {
            dGV.Rows[i].SetValues(channelData[i]);
            if (channelData[i][2] != null)
            {
                dGV.Rows[i].ReadOnly = false;
                dGV.Rows[i].Cells[0].ReadOnly = true;
            }
            else
            {
                dGV.Rows[i].ReadOnly = true;
                dGV.Rows[i].Cells[2].ReadOnly = false;
            }
        }

        if (dGV.CurrentCell != null) dGV.CurrentCell = null;

        recordStep();
    }

    public string[][] getChData()
    {
        return channelData;
    }

    public void upDataFreqRange(string theMinFreqForVHF, string theMaxFreqForVHF, string theMinFreqForUHF,
        string theMaxFreqForUHF)
    {
        this.theMinFreqForVHF = short.Parse(theMinFreqForVHF);
        this.theMaxFreqForVHF = short.Parse(theMaxFreqForVHF);
        this.theMinFreqForUHF = short.Parse(theMinFreqForUHF);
        this.theMaxFreqForUHF = short.Parse(theMaxFreqForUHF);
    }

    public void changeTheCountsOfCH(int counts)
    {
        countsOfChannel_Old = countsOfChannel_New;
        countsOfChannel_New = counts;
        if (countsOfChannel_New > countsOfChannel_Old)
            for (var i = countsOfChannel_Old; i < countsOfChannel_New; i++)
            {
                var dataGridViewRow = new DataGridViewRow();
                dataGridViewRow.CreateCells(dGV);
                dataGridViewRow.SetValues(channelData[i]);
                if (channelData[i][2] != null)
                {
                    dataGridViewRow.ReadOnly = false;
                    dataGridViewRow.Cells[0].ReadOnly = true;
                }
                else
                {
                    dataGridViewRow.ReadOnly = true;
                    dataGridViewRow.Cells[2].ReadOnly = false;
                }

                dGV.Rows.Add(dataGridViewRow);
            }
        else
            for (var j = countsOfChannel_New; j < countsOfChannel_Old; j++)
                dGV.Rows.RemoveAt(countsOfChannel_New);
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

        var componentResourceManager = new ComponentResourceManager(typeof(FormCHImfo));
        componentResourceManager.ApplyResources(this, "$this");
        foreach (DataGridViewColumn column in dGV.Columns) componentResourceManager.ApplyResources(column, column.Name);

        foreach (Control control in Controls) componentResourceManager.ApplyResources(control, control.Name);

        ResumeLayout(false);
        Visible = visible;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Return:
                SendKeys.Send("{TAB}");
                if (dGV.CurrentCell != null)
                {
                    var rowIndex = dGV.CurrentCell.RowIndex;
                    var columnIndex = dGV.CurrentCell.ColumnIndex;
                    if (columnIndex == 13) SendKeys.Send("{TAB}");
                }

                return true;
            case Keys.Escape:
                if (dGV.CurrentCell != null) dGV.CurrentCell = null;

                break;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null) components.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var resources =
            new ComponentResourceManager(typeof(FormCHImfo));
        var dataGridViewCellStyle =
            new DataGridViewCellStyle();
        var dataGridViewCellStyle2 =
            new DataGridViewCellStyle();
        var dataGridViewCellStyle3 =
            new DataGridViewCellStyle();
        var dataGridViewCellStyle4 =
            new DataGridViewCellStyle();
        btn_default = new Button();
        btn_Clear = new Button();


        // Here we add somethng...
        btn_rmEmpty = new Button();
        btn_rmEmpty.Location = new Point(20, 650);
        btn_rmEmpty.Text = "一键删除空信道";
        btn_rmEmpty.Width = 200;
        btn_rmEmpty.Height = 50;
        btn_rmEmpty.UseVisualStyleBackColor = true;
        btn_rmEmpty.Click += btn_rmEmpty_Click;
        Controls.Add(btn_rmEmpty);

        btn_default = new Button();
        btn_default.Location = new Point(1000, 650);
        btn_default.Text = "改为出厂参数";
        btn_default.Width = 200;
        btn_default.Height = 30;
        btn_default.UseVisualStyleBackColor = true;
        btn_default.Click += btn_default_Click;
        Controls.Add(btn_default);

        btn_Clear = new Button();
        btn_Clear.Location = new Point(1000, 690);
        btn_Clear.Text = "! 清空所有信道 !";
        btn_Clear.Width = 200;
        btn_Clear.Height = 30;
        btn_Clear.UseVisualStyleBackColor = true;
        btn_Clear.Click += btn_Clear_Click;
        Controls.Add(btn_Clear);

        btn_recall_operation = new Button();
        btn_recall_operation.Location = new Point(240, 650);
        btn_recall_operation.Text = "撤回";
        btn_recall_operation.Width = 120;
        btn_recall_operation.Height = 50;
        btn_recall_operation.UseVisualStyleBackColor = true;
        btn_recall_operation.Click += btn_recall_Click;
        Controls.Add(btn_recall_operation);

        // btn_remove_current_channel = new Button();
        // btn_remove_current_channel.Location = new Point(800, 650);
        // btn_remove_current_channel.Text = "删除该条信道";
        // btn_remove_current_channel.Width = 200;
        // btn_remove_current_channel.Height = 30;
        // btn_remove_current_channel.UseVisualStyleBackColor = true;
        // btn_remove_current_channel.Click += btn_removeCurrent_Click;
        // Controls.Add(btn_remove_current_channel);
        //
        // btn_ins_channel = new Button();
        // btn_ins_channel.Location = new Point(800, 690);
        // btn_ins_channel.Text = "在下方插入空信道";
        // btn_ins_channel.Width = 200;
        // btn_ins_channel.Height = 30;
        // btn_ins_channel.UseVisualStyleBackColor = true;
        // btn_ins_channel.Click += btn_insertEmptyChannel_Click;
        // Controls.Add(btn_ins_channel);
        //
        // btn_clr_channel = new Button();
        // btn_clr_channel.Location = new Point(600, 650);
        // btn_clr_channel.Text = "清空该条信道";
        // btn_clr_channel.Width = 200;
        // btn_clr_channel.Height = 30;
        // btn_clr_channel.UseVisualStyleBackColor = true;
        // btn_clr_channel.Click += btn_clearChan_Click;
        // Controls.Add(btn_clr_channel);

        btn_refresh = new Button();
        btn_refresh.Location = new Point(380, 650);
        btn_refresh.Text = "刷新视图";
        btn_refresh.Width = 120;
        btn_refresh.Height = 50;
        btn_refresh.UseVisualStyleBackColor = true;
        btn_refresh.Click += btn_refresh_click;
        Controls.Add(btn_refresh);
        // EOA


        dGV = new DataGridViewX();
        col_chnNum = new DataGridViewTextBoxColumn();
        col_enableTx = new DataGridViewComboBoxColumn();
        col_rxFreq = new DataGridViewTextBoxColumn();
        col_QTDTQ = new DataGridViewComboBoxExColumn();
        col_txFreq = new DataGridViewTextBoxColumn();
        col_QTDQT = new DataGridViewComboBoxExColumn();
        col_txPwr = new DataGridViewComboBoxColumn();
        col_bandw = new DataGridViewComboBoxColumn();
        col_pttid = new DataGridViewComboBoxColumn();
        col_lockonbusy = new DataGridViewComboBoxColumn();
        col_addonscan = new DataGridViewComboBoxColumn();
        col_signcode = new DataGridViewComboBoxColumn();
        col_chnName = new DataGridViewTextBoxColumn();
        col_encrypt = new DataGridViewComboBoxColumn();
        ((ISupportInitialize)dGV).BeginInit();
        SuspendLayout();

        // Here we load configs from xml... so bad! deprecated!
        //
        // resources.ApplyResources(this.btn_default, "btn_default");
        // this.btn_default.Name = "btn_default";
        // this.btn_default.UseVisualStyleBackColor = true;
        // this.btn_default.Click += new System.EventHandler(btn_default_Click);
        // resources.ApplyResources(this.btn_Clear, "btn_Clear");
        // this.btn_Clear.Name = "btn_Clear";
        // this.btn_Clear.UseVisualStyleBackColor = true;
        // this.btn_Clear.Click += new System.EventHandler(btn_Clear_Click);


        dGV.AllowUserToAddRows = false;
        dGV.AllowUserToDeleteRows = false;
        dGV.AllowUserToResizeColumns = false;
        dGV.AllowUserToResizeRows = false;
        dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle.BackColor = Color.FromArgb(249, 249, 249);
        dataGridViewCellStyle.SelectionBackColor = SystemColors.MenuHighlight;
        dGV.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle;
        dGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle2.BackColor = SystemColors.Control;
        dataGridViewCellStyle2.Font = new Font("宋体", 9f, FontStyle.Regular,
            GraphicsUnit.Point, 134);
        dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
        dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
        dGV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
        resources.ApplyResources(dGV, "dGV");
        dGV.ColumnHeadersHeightSizeMode =
            DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dGV.Columns.AddRange(col_chnNum, col_enableTx, col_rxFreq, col_QTDTQ,
            col_txFreq, col_QTDQT, col_txPwr, col_bandw, col_pttid, col_lockonbusy,
            col_addonscan, col_signcode, col_chnName, col_encrypt);
        dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle3.BackColor = Color.FromArgb(230, 239, 238);
        dataGridViewCellStyle3.Font = new Font("宋体", 9f, FontStyle.Regular,
            GraphicsUnit.Point, 134);
        dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle3.SelectionForeColor = Color.White;
        dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
        dGV.DefaultCellStyle = dataGridViewCellStyle3;
        dGV.EditMode = DataGridViewEditMode.EditOnEnter;
        dGV.GridColor = SystemColors.AppWorkspace;
        dGV.MultiSelect = false;
        dGV.Name = "dGV";
        dGV.RowHeadersVisible = false;
        dGV.RowTemplate.Height = 27;
        dGV.CellEndEdit += dGV_CellEndEdit;
        dGV.CellEndEdit += col_change;
        col_chnNum.FillWeight = 30f;
        resources.ApplyResources(col_chnNum, "col_chnNum");
        col_chnNum.Name = "col_chnNum";
        col_chnNum.SortMode = DataGridViewColumnSortMode.NotSortable;
        col_enableTx.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        col_enableTx.FillWeight = 38f;
        resources.ApplyResources(col_enableTx, "col_enableTx");
        col_enableTx.Items.AddRange("Yes", "No");
        col_enableTx.Name = "col_enableTx";
        col_enableTx.Resizable = DataGridViewTriState.True;
        col_rxFreq.FillWeight = 42f;
        resources.ApplyResources(col_rxFreq, "col_rxFreq");
        col_rxFreq.MaxInputLength = 9;
        col_rxFreq.Name = "col_rxFreq";
        col_rxFreq.SortMode = DataGridViewColumnSortMode.NotSortable;
        col_QTDTQ.DisplayMember = "Text";
        col_QTDTQ.DropDownHeight = 106;
        col_QTDTQ.DropDownWidth = 121;
        col_QTDTQ.FillWeight = 50f;
        col_QTDTQ.FlatStyle = FlatStyle.Flat;
        resources.ApplyResources(col_QTDTQ, "col_QTDTQ");
        col_QTDTQ.ImeMode = ImeMode.NoControl;
        col_QTDTQ.IntegralHeight = false;
        col_QTDTQ.ItemHeight = 20;
        col_QTDTQ.Items.AddRange(new object[252]
        {
            "OFF", "67.0", "69.3", "71.9", "74.4", "77.0", "79.7", "82.5", "85.4", "88.5",
            "91.5", "94.8", "97.4", "100.0", "103.5", "107.2", "110.9", "114.8", "118.8", "123.0",
            "123.0", "127.3", "131.8", "136.5", "141.3", "146.2", "151.4", "156.7", "162.2", "167.9",
            "173.8", "179.9", "186.2", "192.8", "203.5", "210.7", "218.1", "225.7", "229.1", "233.6",
            "241.8", "250.3", "D023N", "D025N", "D026N", "D031N", "D032N", "D036N", "D043N", "D047N",
            "D051N", "D053N", "D054N", "D065N", "D071N", "D072N", "D073N", "D074N", "D114N", "D115N",
            "D116N", "D122N", "D125N", "D131N", "D132N", "D134N", "D143N", "D145N", "D152N", "D155N",
            "D156N", "D162N", "D165N", "D172N", "D174N", "D205N", "D212N", "D223N", "D225N", "D226N",
            "D243N", "D244N", "D245N", "D246N", "D251N", "D252N", "D255N", "D261N", "D263N", "D265N",
            "D266N", "D271N", "D274N", "D306N", "D311N", "D315N", "D325N", "D331N", "D332N", "D343N",
            "D346N", "D351N", "D356N", "D364N", "D365N", "D371N", "D411N", "D412N", "D413N", "D423N",
            "D431N", "D432N", "D445N", "D446N", "D452N", "D454N", "D455N", "D462N", "D464N", "D465N",
            "D466N", "D503N", "D506N", "D516N", "D523N", "D526N", "D532N", "D546N", "D565N", "D606N",
            "D612N", "D624N", "D627N", "D631N", "D632N", "D645N", "D654N", "D662N", "D664N", "D703N",
            "D712N", "D723N", "D731N", "D732N", "D734N", "D743N", "D754N", "D023I", "D025I", "D026I",
            "D031I", "D032I", "D036I", "D043I", "D047I", "D051I", "D053I", "D054I", "D065I", "D071I",
            "D072I", "D073I", "D074I", "D114I", "D115I", "D116I", "D122I", "D125I", "D131I", "D132I",
            "D134I", "D143I", "D145I", "D152I", "D155I", "D156I", "D162I", "D165I", "D172I", "D174I",
            "D205I", "D212I", "D223I", "D225I", "D226I", "D243I", "D244I", "D245I", "D246I", "D251I",
            "D252I", "D255I", "D261I", "D263I", "D265I", "D266I", "D271I", "D274I", "D306I", "D311I",
            "D315I", "D325I", "D331I", "D332I", "D343I", "D346I", "D351I", "D356I", "D364I", "D365I",
            "D371I", "D411I", "D412I", "D413I", "D423I", "D431I", "D432I", "D445I", "D446I", "D452I",
            "D454I", "D455I", "D462I", "D464I", "D465I", "D466I", "D503I", "D506I", "D516I", "D523I",
            "D526I", "D532I", "D546I", "D565I", "D606I", "D612I", "D624I", "D627I", "D631I", "D632I",
            "D645I", "D654I", "D662I", "D664I", "D703I", "D712I", "D723I", "D731I", "D732I", "D734I",
            "D743I", "D754I"
        });
        col_QTDTQ.Name = "col_QTDTQ";
        col_QTDTQ.RightToLeft = RightToLeft.No;
        col_QTDTQ.SortMode = DataGridViewColumnSortMode.NotSortable;
        dataGridViewCellStyle4.NullValue = null;
        col_txFreq.DefaultCellStyle = dataGridViewCellStyle4;
        col_txFreq.FillWeight = 42f;
        resources.ApplyResources(col_txFreq, "col_txFreq");
        col_txFreq.MaxInputLength = 9;
        col_txFreq.Name = "col_txFreq";
        col_txFreq.SortMode = DataGridViewColumnSortMode.NotSortable;
        col_QTDQT.DisplayMember = "Text";
        col_QTDQT.DropDownHeight = 106;
        col_QTDQT.DropDownWidth = 121;
        col_QTDQT.FillWeight = 50f;
        col_QTDQT.FlatStyle = FlatStyle.Flat;
        resources.ApplyResources(col_QTDQT, "col_QTDQT");
        col_QTDQT.ImeMode = ImeMode.NoControl;
        col_QTDQT.IntegralHeight = false;
        col_QTDQT.ItemHeight = 20;
        col_QTDQT.Items.AddRange(new object[252]
        {
            "OFF", "67.0", "69.3", "71.9", "74.4", "77.0", "79.7", "82.5", "85.4", "88.5",
            "91.5", "94.8", "97.4", "100.0", "103.5", "107.2", "110.9", "114.8", "118.8", "123.0",
            "123.0", "127.3", "131.8", "136.5", "141.3", "146.2", "151.4", "156.7", "162.2", "167.9",
            "173.8", "179.9", "186.2", "192.8", "203.5", "210.7", "218.1", "225.7", "229.1", "233.6",
            "241.8", "250.3", "D023N", "D025N", "D026N", "D031N", "D032N", "D036N", "D043N", "D047N",
            "D051N", "D053N", "D054N", "D065N", "D071N", "D072N", "D073N", "D074N", "D114N", "D115N",
            "D116N", "D122N", "D125N", "D131N", "D132N", "D134N", "D143N", "D145N", "D152N", "D155N",
            "D156N", "D162N", "D165N", "D172N", "D174N", "D205N", "D212N", "D223N", "D225N", "D226N",
            "D243N", "D244N", "D245N", "D246N", "D251N", "D252N", "D255N", "D261N", "D263N", "D265N",
            "D266N", "D271N", "D274N", "D306N", "D311N", "D315N", "D325N", "D331N", "D332N", "D343N",
            "D346N", "D351N", "D356N", "D364N", "D365N", "D371N", "D411N", "D412N", "D413N", "D423N",
            "D431N", "D432N", "D445N", "D446N", "D452N", "D454N", "D455N", "D462N", "D464N", "D465N",
            "D466N", "D503N", "D506N", "D516N", "D523N", "D526N", "D532N", "D546N", "D565N", "D606N",
            "D612N", "D624N", "D627N", "D631N", "D632N", "D645N", "D654N", "D662N", "D664N", "D703N",
            "D712N", "D723N", "D731N", "D732N", "D734N", "D743N", "D754N", "D023I", "D025I", "D026I",
            "D031I", "D032I", "D036I", "D043I", "D047I", "D051I", "D053I", "D054I", "D065I", "D071I",
            "D072I", "D073I", "D074I", "D114I", "D115I", "D116I", "D122I", "D125I", "D131I", "D132I",
            "D134I", "D143I", "D145I", "D152I", "D155I", "D156I", "D162I", "D165I", "D172I", "D174I",
            "D205I", "D212I", "D223I", "D225I", "D226I", "D243I", "D244I", "D245I", "D246I", "D251I",
            "D252I", "D255I", "D261I", "D263I", "D265I", "D266I", "D271I", "D274I", "D306I", "D311I",
            "D315I", "D325I", "D331I", "D332I", "D343I", "D346I", "D351I", "D356I", "D364I", "D365I",
            "D371I", "D411I", "D412I", "D413I", "D423I", "D431I", "D432I", "D445I", "D446I", "D452I",
            "D454I", "D455I", "D462I", "D464I", "D465I", "D466I", "D503I", "D506I", "D516I", "D523I",
            "D526I", "D532I", "D546I", "D565I", "D606I", "D612I", "D624I", "D627I", "D631I", "D632I",
            "D645I", "D654I", "D662I", "D664I", "D703I", "D712I", "D723I", "D731I", "D732I", "D734I",
            "D743I", "D754I"
        });
        col_QTDQT.Name = "col_QTDQT";
        col_QTDQT.RightToLeft = RightToLeft.No;
        col_QTDQT.SortMode = DataGridViewColumnSortMode.NotSortable;
        col_txPwr.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        col_txPwr.FillWeight = 40f;
        resources.ApplyResources(col_txPwr, "col_txPwr");
        col_txPwr.Items.AddRange("H", "L");
        col_txPwr.Name = "col_txPwr";
        col_bandw.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        col_bandw.FillWeight = 30f;
        resources.ApplyResources(col_bandw, "col_bandw");
        col_bandw.Items.AddRange("W", "N");
        col_bandw.Name = "col_bandw";
        col_pttid.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        col_pttid.FillWeight = 38f;
        resources.ApplyResources(col_pttid, "col_pttid");
        col_pttid.Items.AddRange("OFF", "BOT", "EOT", "BOTH");
        col_pttid.Name = "col_pttid";
        col_lockonbusy.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        col_lockonbusy.FillWeight = 45f;
        resources.ApplyResources(col_lockonbusy, "col_lockonbusy");
        col_lockonbusy.Items.AddRange("OFF", "ON");
        col_lockonbusy.Name = "col_lockonbusy";
        col_addonscan.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        col_addonscan.FillWeight = 40f;
        resources.ApplyResources(col_addonscan, "col_addonscan");
        col_addonscan.Items.AddRange("OFF", "ON");
        col_addonscan.Name = "col_addonscan";
        col_signcode.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        col_signcode.FillWeight = 35f;
        resources.ApplyResources(col_signcode, "col_signcode");
        col_signcode.Items.AddRange("1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14",
            "15");
        col_signcode.Name = "col_signcode";
        col_signcode.Resizable = DataGridViewTriState.True;
        col_chnName.FillWeight = 40f;
        resources.ApplyResources(col_chnName, "col_chnName");
        col_chnName.MaxInputLength = 10;
        col_chnName.Name = "col_chnName";
        col_chnName.SortMode = DataGridViewColumnSortMode.NotSortable;
        col_encrypt.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        col_encrypt.FillWeight = 30f;
        resources.ApplyResources(col_encrypt, "col_encrypt");
        col_encrypt.Items.AddRange("OFF", "ON");
        col_encrypt.Name = "col_encrypt";
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;

        // dragdrop
        dGV.AllowDrop = true;
        dGV.MouseDown += dataGridView_MouseDown;
        dGV.MouseMove += dataGridView_MouseMove;
        dGV.DragOver += dataGridView_DragOver;
        dGV.DragDrop += dataGridView_DragDrop;


        Controls.Add(dGV);
        Controls.Add(btn_Clear);
        Controls.Add(btn_default);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        KeyPreview = true;
        MaximizeBox = false;
        Name = "FormCHImfo";
        FormClosing += FormCHImfo_FormClosing;
        Load += FormCHImfo_Load;
        KeyPress += FormCHImfo_KeyPress;


        // right click
        RCclearThisChannel = new ToolStripMenuItem();
        RCclearThisChannel.Size = new Size(148, 22);
        RCclearThisChannel.Text = "清空该信道";
        RCclearThisChannel.Click += btn_clearChan_Click;

        RCdelThisChannel = new ToolStripMenuItem();
        RCdelThisChannel.Size = new Size(148, 22);
        RCdelThisChannel.Text = "删除该信道";
        RCdelThisChannel.Click += btn_removeCurrent_Click;

        RCinsertAfterChannel = new ToolStripMenuItem();
        RCinsertAfterChannel.Size = new Size(148, 22);
        RCinsertAfterChannel.Text = "插入空信道";
        RCinsertAfterChannel.Click += btn_insertEmptyChannel_Click;

        RCcopyChannel = new ToolStripMenuItem();
        RCcopyChannel.Size = new Size(148, 22);
        RCcopyChannel.Text = "复制信道";
        RCcopyChannel.Click += ctx_copy_click;

        RCcutChannel = new ToolStripMenuItem();
        RCcutChannel.Size = new Size(148, 22);
        RCcutChannel.Text = "剪切信道";
        RCcutChannel.Click += ctx_cut_click;

        RCpasteChannel = new ToolStripMenuItem();
        RCpasteChannel.Size = new Size(148, 22);
        RCpasteChannel.Text = "粘贴信道（！如存在则覆盖！）";
        RCpasteChannel.Click += ctx_paste_click;

        contextMenuStrip = new ContextMenuStrip();
        contextMenuStrip.Items.AddRange(new[]
        {
            RCclearThisChannel,
            RCdelThisChannel,
            RCinsertAfterChannel,
            RCcopyChannel,
            RCcutChannel,
            RCpasteChannel
        });
        // RCinsertAfterChannel});
        contextMenuStrip.Size = new Size(149, 54);
        dGV.CellMouseDown += dGView_CellMouseDown;

        ((ISupportInitialize)dGV).EndInit();
        ResumeLayout(false);
    }
}