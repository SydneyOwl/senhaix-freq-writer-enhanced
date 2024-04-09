using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.Controls;
using SHX_GT12_CPS.Properties;

namespace SHX_GT12_CPS.View;

public class FormChannelList : Form
{
    private static FormChannelList instance;

    private readonly IContainer components = null;

    private readonly string LANG = "Chinese";

    private readonly int maxFreq = 520;

    private readonly int minFreq = 100;

    private readonly string[] strAreaCN = new string[30]
    {
        "区域一", "区域二", "区域三", "区域四", "区域五", "区域六", "区域七", "区域八", "区域九", "区域十",
        "区域十一", "区域十二", "区域十三", "区域十四", "区域十五", "区域十六", "区域十七", "区域十八", "区域十九", "区域二十",
        "区域二十一", "区域二十二", "区域二十三", "区域二十四", "区域二十五", "区域二十六", "区域二十七", "区域二十八", "区域二十九", "区域三十"
    };

    private readonly string[] strAreaEN = new string[30]
    {
        "ZONE 1", "ZONE 2", "ZONE 3", "ZONE 4", "ZONEE 5", "ZONE 6", "ZONE 7", "ZONE 8", "ZONE 9", "ZONE 10",
        "ZONE 11", "ZONE 12", "ZONE 13", "ZONE 14", "ZONEE 15", "ZONE 16", "ZONE 17", "ZONE 18", "ZONE 19", "ZONE 20",
        "ZONE 21", "ZONE 22", "ZONE 23", "ZONE 24", "ZONEE 25", "ZONE 26", "ZONE 27", "ZONE 28", "ZONE 29", "ZONE 30"
    };

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

    private AppData appData;

    private Button btn_AtFirstPage;

    private Button btn_AtLastPage;

    private Button btn_LastPage;

    private Button btn_NextPage;

    private Channel[][] channelList = new Channel[30][];

    private DataGridViewComboBoxColumn Column_Bandwide;

    private DataGridViewTextBoxColumn Column_ID;

    private DataGridViewTextBoxColumn Column_Name;

    private DataGridViewComboBoxColumn Column_PTTID;

    private DataGridViewComboBoxExColumn Column_RxCtsDcs;

    private DataGridViewTextBoxColumn Column_RxFreq;

    private DataGridViewComboBoxColumn Column_ScanAdd;

    private DataGridViewComboBoxColumn Column_SignalGroup;

    private DataGridViewComboBoxColumn Column_SignalSystem;

    private DataGridViewComboBoxColumn Column_SQMode;

    private DataGridViewComboBoxExColumn Column_TxCtsDcs;

    private DataGridViewTextBoxColumn Column_TxFreq;

    private DataGridViewComboBoxColumn Column_TxPower;
    private ContextMenuStrip contextMenuStrip;

    private int curPage;

    private Channel currentCache;

    public DataGridViewX dGV_ChannelList;

    private Rectangle dragBoxFromMouseDown;

    private Label label_BankName;

    private Label label_CurPage;
    private ToolStripItem RCBatchClearEmptyChannel;

    // Right-click
    private ToolStripItem RCclearThisChannel;
    private ToolStripItem RCcopyChannel;
    private ToolStripItem RCcutChannel;
    private ToolStripItem RCdelThisChannel;
    private ToolStripItem RCinsertAfterChannel;
    private ToolStripItem RCpasteChannel;
    private int rowIndexFromMouseDown;
    private int rowIndexOfItemUnderMouseToDrop;

    private TextBox tB_BankName;

    public FormChannelList(Form parent)
    {
        InitializeComponent();
        Column_RxCtsDcs.Items.AddRange(tblCTSDCS);
        Column_TxCtsDcs.Items.AddRange(tblCTSDCS);
        MdiParent = parent;
        for (var i = 0; i < 32; i++)
        {
            var dataGridViewRow = new DataGridViewRow();
            dataGridViewRow.CreateCells(dGV_ChannelList);
            dataGridViewRow.SetValues(i);
            dataGridViewRow.ReadOnly = true;
            dataGridViewRow.Cells[1].ReadOnly = false;
            dGV_ChannelList.Rows.Add(dataGridViewRow);
        }

        LANG = Settings.Default.LANG;
    }

    public static FormChannelList getInstance()
    {
        return instance;
    }

    public static FormChannelList getNewInstance(Form parent)
    {
        instance = new FormChannelList(parent);
        return instance;
    }

    public void LoadData(AppData appData)
    {
        this.appData = appData;
        channelList = appData.ChannelList;
        for (var i = 0; i < 30; i++)
            if (this.appData.BankName[i] == "")
            {
                if (LANG == "Chinese")
                    this.appData.BankName[i] = strAreaCN[i];
                else
                    this.appData.BankName[i] = strAreaEN[i];
            }

        curPage = 0;
        UpdateDataGridView(curPage);
    }

    // ---------------
    private void updateCurrentPageChannelIndex(int page)
    {
        for (var i = 0; i < 32; i++) channelList[page][0].Id = i + 1;
    }

    private bool isEmpty(int page, int channelRowIndex)
    {
        return string.IsNullOrEmpty(channelList[page][channelRowIndex].RxFreq);
    }

    public int findLastEmpty()
    {
        var lastEmp = -1;
        for (var i = 31; i >= 0; i--)
            if (isEmpty(curPage, i))
                lastEmp = i;
            else
                break;
        return lastEmp;
    }

    public void insertChannelAfter(string[] channel, int index)
    {
        // channelList[curPage][i].TxPower %= Column_TxPower.Items.Count;
        // channelList[curPage][i].Bandwide %= Column_Bandwide.Items.Count;
        // channelList[curPage][i].ScanAdd %= Column_ScanAdd.Items.Count;
        // channelList[curPage][i].SignalSystem %= Column_SignalSystem.Items.Count;
        // channelList[curPage][i].SqMode %= Column_SQMode.Items.Count;
        // channelList[curPage][i].Pttid %= Column_PTTID.Items.Count;
        // channelList[curPage][i].SignalGroup %= Column_SignalGroup.Items.Count;
        var chan = new Channel(int.Parse(channel[0]), channel[1],
            channel[2], channel[3], channel[4], 0, 0, 0, 0, 0, 0, channel[12]);
        channelList[curPage][index] = chan;
        UpdateDataGridView(curPage);
    }

    private void ctx_copy_click(object sender, EventArgs e)
    {
        var current_row = dGV_ChannelList.CurrentCell.RowIndex;
        currentCache = channelList[curPage][current_row];
    }

    private void ctx_cut_click(object sender, EventArgs e)
    {
        var current_row = dGV_ChannelList.CurrentCell.RowIndex;
        dGV_ChannelList.CurrentCell.ReadOnly = true;
        currentCache = channelList[curPage][current_row];
        channelList[curPage][current_row] = new Channel();
        updateCurrentPageChannelIndex(curPage);
        UpdateDataGridView(curPage);
    }

    private void ctx_paste_click(object sender, EventArgs e)
    {
        if (currentCache == null)
        {
            MessageBox.Show("还没有进行复制或剪切操作");
            return;
        }

        var current_row = dGV_ChannelList.CurrentCell.RowIndex;
        channelList[curPage][current_row] = (Channel)currentCache.Clone();
        updateCurrentPageChannelIndex(curPage);
        UpdateDataGridView(curPage);
    }


    private void ctx_removeCurrent_Click(object sender, EventArgs e)
    {
        var current_row = dGV_ChannelList.CurrentCell.RowIndex;
        dGV_ChannelList.CurrentCell.ReadOnly = true;
        for (var i = current_row; i < 31; i++)
            channelList[curPage][i] = (Channel)channelList[curPage][i + 1].Clone();
        channelList[curPage][31] = new Channel();
        updateCurrentPageChannelIndex(curPage);
        UpdateDataGridView(curPage);
    }

    private void ctx_clearChan_Click(object sender, EventArgs e)
    {
        var clrRow = dGV_ChannelList.CurrentCell.RowIndex;
        dGV_ChannelList.CurrentCell.ReadOnly = true;
        channelList[curPage][clrRow] = new Channel();
        UpdateDataGridView(curPage);
    }

    private void ctx_insertEmptyChannel_Click(object sender, EventArgs e)
    {
        var current_row = dGV_ChannelList.CurrentCell.RowIndex;
        if (!isEmpty(curPage, 31))
        {
            MessageBox.Show("信道32不为空，无法插入");
            return;
        }

        // find last empty index
        var lastEmp = 0;
        for (var i = 0; i < 31; i++)
            if (!isEmpty(curPage, i))
                lastEmp = i;

        for (var i = lastEmp; i > current_row; i--) channelList[curPage][i + 1] = channelList[curPage][i];

        channelList[curPage][current_row + 1] = new Channel();
        updateCurrentPageChannelIndex(curPage);
        UpdateDataGridView(curPage);
    }


    private void ctx_batch_clear_channel(object sender, EventArgs e)
    {
        var cached_channel = new Channel[32];
        var channel_cursor = 0;
        for (var i = 0; i < 32; i++)
            //check it via "TxAllow"
            if (!isEmpty(curPage, i))
            {
                cached_channel[channel_cursor] = new Channel();
                cached_channel[channel_cursor++] = (Channel)channelList[curPage][i].Clone();
            }

        for (var i = channel_cursor; i < 32; i++) cached_channel[i] = new Channel();

        cached_channel.CopyTo(channelList[curPage], 0);
        updateCurrentPageChannelIndex(curPage);
        UpdateDataGridView(curPage);
    }

    // -------------EOF
    // -------------DRG
    private void dataGridView_MouseMove(object sender, MouseEventArgs e)
    {
        if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            // If the mouse moves outside the rectangle, start the drag.
            if (dragBoxFromMouseDown != Rectangle.Empty &&
                !dragBoxFromMouseDown.Contains(e.X, e.Y))
            {
                // Proceed with the drag and drop, passing in the list item.                    
                var dropEffect = dGV_ChannelList.DoDragDrop(
                    dGV_ChannelList.Rows[rowIndexFromMouseDown],
                    DragDropEffects.Move);
            }
    }

    private void dataGridView_MouseDown(object sender, MouseEventArgs e)
    {
        // Get the index of the item the mouse is below.
        rowIndexFromMouseDown = dGV_ChannelList.HitTest(e.X, e.Y).RowIndex;
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
        var clientPoint = dGV_ChannelList.PointToClient(new Point(e.X, e.Y));

        // Get the row index of the item the mouse is below. 
        rowIndexOfItemUnderMouseToDrop =
            dGV_ChannelList.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

        // If the drag operation was a move then remove and insert the row.
        if (e.Effect == DragDropEffects.Move)
        {
            // DataGridViewRow rowToMove = e.Data.GetData(
            //     typeof(DataGridViewRow)) as DataGridViewRow;
            // dGV.Rows.RemoveAt(rowIndexFromMouseDown);
            // dGV.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);
            // Console.WriteLine(rowIndexFromMouseDown);
            // Console.WriteLine(rowIndexOfItemUnderMouseToDrop);
            dGV_ChannelList.CurrentRow.ReadOnly = true;
            swapChannelData(rowIndexFromMouseDown, rowIndexOfItemUnderMouseToDrop);
            updateCurrentPageChannelIndex(curPage);
            UpdateDataGridView(curPage);
        }
    }

    private void swapChannelData(int rowA, int rowB)
    {
        var a = channelList[curPage][rowA].Clone();
        var b = channelList[curPage][rowB].Clone();
        channelList[curPage][rowA] = (Channel)b;
        channelList[curPage][rowB] = (Channel)a;
    }

    // ----------EOD
    private void UpdateDataGridView(int page)
    {
        string[] array = null;
        dGV_ChannelList.CurrentCell = null;
        label_CurPage.Text = curPage + 1 + "/30";
        array = !(LANG == "Chinese") ? strAreaEN : strAreaCN;
        if (appData.BankName[curPage] != "")
        {
            tB_BankName.Text = appData.BankName[curPage];
        }
        else
        {
            tB_BankName.Text = array[curPage];
            appData.BankName[curPage] = array[curPage];
        }

        if (LANG == "Chinese")
            Text = "信道信息 - " + appData.BankName[curPage];
        else
            Text = "Channel Informations - " + appData.BankName[curPage];

        for (var i = 0; i < 32; i++)
        {
            dGV_ChannelList.Rows[i].Cells[0].Value = (i + 1).ToString();
            if (channelList[page][i].RxFreq == "")
            {
                for (var j = 1; j < dGV_ChannelList.ColumnCount; j++) dGV_ChannelList.Rows[i].Cells[j].Value = "";

                dGV_ChannelList.Rows[i].ReadOnly = true;
                dGV_ChannelList.Rows[i].Cells[1].ReadOnly = false;
                continue;
            }

            channelList[curPage][i].TxPower %= Column_TxPower.Items.Count;
            channelList[curPage][i].Bandwide %= Column_Bandwide.Items.Count;
            channelList[curPage][i].ScanAdd %= Column_ScanAdd.Items.Count;
            channelList[curPage][i].SignalSystem %= Column_SignalSystem.Items.Count;
            channelList[curPage][i].SqMode %= Column_SQMode.Items.Count;
            channelList[curPage][i].Pttid %= Column_PTTID.Items.Count;
            channelList[curPage][i].SignalGroup %= Column_SignalGroup.Items.Count;
            dGV_ChannelList.Rows[i].Cells[1].Value = channelList[curPage][i].RxFreq;
            dGV_ChannelList.Rows[i].Cells[2].Value = channelList[curPage][i].StrRxCtsDcs;
            dGV_ChannelList.Rows[i].Cells[3].Value = channelList[curPage][i].TxFreq;
            dGV_ChannelList.Rows[i].Cells[4].Value = channelList[curPage][i].StrTxCtsDcs;
            dGV_ChannelList.Rows[i].Cells[5].Value = Column_TxPower.Items[channelList[curPage][i].TxPower];
            dGV_ChannelList.Rows[i].Cells[6].Value = Column_Bandwide.Items[channelList[curPage][i].Bandwide];
            dGV_ChannelList.Rows[i].Cells[7].Value = Column_ScanAdd.Items[channelList[curPage][i].ScanAdd];
            dGV_ChannelList.Rows[i].Cells[8].Value = Column_SignalSystem.Items[channelList[curPage][i].SignalSystem];
            dGV_ChannelList.Rows[i].Cells[9].Value = Column_SQMode.Items[channelList[curPage][i].SqMode];
            dGV_ChannelList.Rows[i].Cells[10].Value = Column_PTTID.Items[channelList[curPage][i].Pttid];
            dGV_ChannelList.Rows[i].Cells[11].Value = Column_SignalGroup.Items[channelList[curPage][i].SignalGroup];
            dGV_ChannelList.Rows[i].Cells[12].Value = channelList[curPage][i].Name;
            dGV_ChannelList.Rows[i].ReadOnly = false;
            dGV_ChannelList.Rows[i].Cells[0].ReadOnly = true;
        }
    }

    private string CalNameSize(string name)
    {
        var num = 0;
        var num2 = 0;
        var text = name;
        var bytes = Encoding.GetEncoding("gb2312").GetBytes(text);
        if (bytes.Length > 12)
        {
            var num3 = 0;
            while (num3 < 12)
                if (bytes[num3] >= 47 && bytes[num3] < 127)
                {
                    num++;
                    num3++;
                }
                else
                {
                    num2++;
                    num3 += 2;
                }

            text = num % 2 == 0
                ? Encoding.GetEncoding("gb2312").GetString(bytes, 0, 12)
                : Encoding.GetEncoding("gb2312").GetString(bytes, 0, 11);
        }

        return text;
    }

    private void tB_BankName_Leave(object sender, EventArgs e)
    {
        var text = CalNameSize(tB_BankName.Text);
        if (text == "") text = !(LANG == "Chinese") ? strAreaEN[curPage] : strAreaCN[curPage];

        tB_BankName.Text = text;
        appData.BankName[curPage] = tB_BankName.Text;
        if (LANG == "Chinese")
            Text = "信道信息 - " + appData.BankName[curPage];
        else
            Text = "Channel Informations - " + appData.BankName[curPage];
    }

    private void tB_BankName_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == '\r')
        {
            var text = CalNameSize(tB_BankName.Text);
            if (text == "") text = !(LANG == "Chinese") ? strAreaEN[curPage] : strAreaCN[curPage];

            tB_BankName.Text = text;
            appData.BankName[curPage] = tB_BankName.Text;
            if (LANG == "Chinese")
                Text = "信道信息 - " + appData.BankName[curPage];
            else
                Text = "Channel Informations - " + appData.BankName[curPage];
        }
    }

    private void btn_LastPage_Click(object sender, EventArgs e)
    {
        if (curPage > 0)
        {
            curPage--;
            UpdateDataGridView(curPage);
        }
    }

    private void btn_NextPage_Click(object sender, EventArgs e)
    {
        if (curPage < 29)
        {
            curPage++;
            UpdateDataGridView(curPage);
        }
    }

    private void btn_AtFirstPage_Click(object sender, EventArgs e)
    {
        curPage = 0;
        UpdateDataGridView(curPage);
    }

    private void btn_AtLastPage_Click(object sender, EventArgs e)
    {
        curPage = 29;
        UpdateDataGridView(curPage);
    }

    private void ClearChannel(int indexRow)
    {
        channelList[curPage][indexRow] = new Channel();
        for (var i = 1; i < dGV_ChannelList.ColumnCount; i++) dGV_ChannelList.Rows[indexRow].Cells[i].Value = "";

        dGV_ChannelList.Rows[indexRow].ReadOnly = true;
        dGV_ChannelList.Rows[indexRow].Cells[1].ReadOnly = false;
    }

    private void AddChannel(int indexRow, int freq)
    {
        var text = freq.ToString().Insert(3, ".");
        channelList[curPage][indexRow].RxFreq = text;
        channelList[curPage][indexRow].TxFreq = text;
        dGV_ChannelList.Rows[indexRow].Cells[1].Value = text;
        dGV_ChannelList.Rows[indexRow].Cells[2].Value = channelList[curPage][indexRow].StrRxCtsDcs;
        dGV_ChannelList.Rows[indexRow].Cells[3].Value = text;
        dGV_ChannelList.Rows[indexRow].Cells[4].Value = channelList[curPage][indexRow].StrTxCtsDcs;
        dGV_ChannelList.Rows[indexRow].Cells[5].Value = Column_TxPower.Items[channelList[curPage][indexRow].TxPower];
        dGV_ChannelList.Rows[indexRow].Cells[6].Value = Column_Bandwide.Items[channelList[curPage][indexRow].Bandwide];
        dGV_ChannelList.Rows[indexRow].Cells[7].Value = Column_ScanAdd.Items[channelList[curPage][indexRow].ScanAdd];
        dGV_ChannelList.Rows[indexRow].Cells[8].Value =
            Column_SignalSystem.Items[channelList[curPage][indexRow].SignalSystem];
        dGV_ChannelList.Rows[indexRow].Cells[9].Value = Column_SQMode.Items[channelList[curPage][indexRow].SqMode];
        dGV_ChannelList.Rows[indexRow].Cells[10].Value = Column_PTTID.Items[channelList[curPage][indexRow].Pttid];
        dGV_ChannelList.Rows[indexRow].Cells[11].Value =
            Column_SignalGroup.Items[channelList[curPage][indexRow].SignalGroup];
        dGV_ChannelList.Rows[indexRow].ReadOnly = false;
        dGV_ChannelList.Rows[indexRow].Cells[0].ReadOnly = true;
    }

    private void ResumeCtsDcs(int indexColumn, int indexRow)
    {
        if (indexColumn == 2)
            dGV_ChannelList.CurrentCell.Value = channelList[curPage][indexRow].StrRxCtsDcs;
        else
            dGV_ChannelList.CurrentCell.Value = channelList[curPage][indexRow].StrTxCtsDcs;
    }

    // bind rightclick..
    private void dGView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right && e.ColumnIndex > -1 && e.RowIndex > -1)
        {
            dGV_ChannelList.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
            contextMenuStrip.Show(MousePosition.X, MousePosition.Y);
        }
    }

    private void dGV_ChannelList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        var num = 0;
        var flag = false;
        if (dGV_ChannelList.CurrentCell == null) return;

        var columnIndex = dGV_ChannelList.CurrentCell.ColumnIndex;
        var rowIndex = dGV_ChannelList.CurrentCell.RowIndex;
        switch (columnIndex)
        {
            case 1:
            case 3:
            {
                var text = Convert.ToString(dGV_ChannelList.CurrentCell.Value);
                if (text == "" && dGV_ChannelList.Rows[rowIndex].Cells[2].ReadOnly) break;

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
                            MessageBox.Show("Frequence's format is error!", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Hand);

                        if (columnIndex == 1)
                            dGV_ChannelList.CurrentCell.Value = channelList[curPage][rowIndex].RxFreq;
                        else
                            dGV_ChannelList.CurrentCell.Value = channelList[curPage][rowIndex].TxFreq;

                        break;
                    }
                }

                if (text != "")
                {
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

                        if (columnIndex == 1)
                            dGV_ChannelList.CurrentCell.Value = channelList[curPage][rowIndex].RxFreq;
                        else
                            dGV_ChannelList.CurrentCell.Value = channelList[curPage][rowIndex].TxFreq;

                        break;
                    }

                    num = list[0] * 100000;
                    if (list.Count > 1)
                    {
                        var num5 = 5 - array[1].Length;
                        if (num5 > 0)
                            for (var k = 0; k < num5; k++)
                                list[1] *= 10;

                        num += list[1];
                    }
                }

                if (text != "")
                {
                    if (num % 125 != 0)
                    {
                        num /= 125;
                        num *= 125;
                    }

                    if (dGV_ChannelList.Rows[rowIndex].Cells[2].ReadOnly)
                    {
                        AddChannel(rowIndex, num);
                        break;
                    }

                    dGV_ChannelList.CurrentCell.Value = num.ToString().Insert(3, ".");
                    if (columnIndex == 1)
                        channelList[curPage][rowIndex].RxFreq = dGV_ChannelList.CurrentCell.Value.ToString();
                    else
                        channelList[curPage][rowIndex].TxFreq = dGV_ChannelList.CurrentCell.Value.ToString();
                }
                else if (columnIndex == 1)
                {
                    ClearChannel(rowIndex);
                }
                else
                {
                    channelList[curPage][rowIndex].TxFreq = "";
                }

                break;
            }
            case 2:
            case 4:
            {
                var text = Convert.ToString(dGV_ChannelList.CurrentCell.Value);
                if (text == "")
                {
                    ResumeCtsDcs(columnIndex, rowIndex);
                }
                else
                {
                    if (!(text != "")) break;

                    var num2 = -1;
                    num2 = Column_RxCtsDcs.Items.IndexOf(text);
                    if (num2 == -1)
                    {
                        if (text[0] != 'D')
                            try
                            {
                                var num3 = double.Parse(dGV_ChannelList.CurrentCell.Value.ToString());
                                if (num3 >= 60.0 && num3 <= 260.0)
                                {
                                    var text2 = num3.ToString();
                                    var num4 = text2.IndexOf('.');
                                    if (num4 == -1)
                                        text2 += ".0";
                                    else if (num4 == text.Length - 1)
                                        text2 += "0";
                                    else if (num4 != text.Length - 2)
                                        text2 = text2.Remove(num4 + 2, text2.Length - 1 - (num4 + 1));

                                    dGV_ChannelList.CurrentCell.Value = text2;
                                    if (columnIndex == 2)
                                    {
                                        channelList[curPage][rowIndex].StrRxCtsDcs = text2;
                                        channelList[curPage][rowIndex].StrTxCtsDcs = text2;
                                        dGV_ChannelList.Rows[rowIndex].Cells[4].Value = text2;
                                    }
                                    else
                                    {
                                        channelList[curPage][rowIndex].StrTxCtsDcs = text2;
                                    }
                                }
                                else
                                {
                                    ResumeCtsDcs(columnIndex, rowIndex);
                                }

                                break;
                            }
                            catch
                            {
                                ResumeCtsDcs(columnIndex, rowIndex);
                                break;
                            }

                        ResumeCtsDcs(columnIndex, rowIndex);
                    }
                    else if (columnIndex == 2)
                    {
                        channelList[curPage][rowIndex].StrRxCtsDcs = dGV_ChannelList.CurrentCell.Value.ToString();
                        channelList[curPage][rowIndex].StrTxCtsDcs = dGV_ChannelList.CurrentCell.Value.ToString();
                        dGV_ChannelList.Rows[rowIndex].Cells[4].Value = dGV_ChannelList.CurrentCell.Value;
                    }
                    else
                    {
                        channelList[curPage][rowIndex].StrTxCtsDcs = dGV_ChannelList.CurrentCell.Value.ToString();
                    }
                }

                break;
            }
            case 5:
                channelList[curPage][rowIndex].TxPower =
                    Column_TxPower.Items.IndexOf(dGV_ChannelList.CurrentCell.Value);
                break;
            case 6:
                channelList[curPage][rowIndex].Bandwide =
                    Column_Bandwide.Items.IndexOf(dGV_ChannelList.CurrentCell.Value);
                break;
            case 7:
                channelList[curPage][rowIndex].ScanAdd =
                    Column_ScanAdd.Items.IndexOf(dGV_ChannelList.CurrentCell.Value);
                break;
            case 8:
                channelList[curPage][rowIndex].SignalSystem =
                    Column_SignalSystem.Items.IndexOf(dGV_ChannelList.CurrentCell.Value);
                break;
            case 9:
                channelList[curPage][rowIndex].SqMode = Column_SQMode.Items.IndexOf(dGV_ChannelList.CurrentCell.Value);
                break;
            case 10:
                channelList[curPage][rowIndex].Pttid = Column_PTTID.Items.IndexOf(dGV_ChannelList.CurrentCell.Value);
                break;
            case 11:
                channelList[curPage][rowIndex].SignalGroup =
                    Column_SignalGroup.Items.IndexOf(dGV_ChannelList.CurrentCell.Value);
                break;
            case 12:
                channelList[curPage][rowIndex].Name = dGV_ChannelList.CurrentCell.Value.ToString();
                break;
        }
    }

    private void FormChannelList_FormClosing(object sender, FormClosingEventArgs e)
    {
        Hide();
        e.Cancel = true;
    }

    private void FormChannelList_Load(object sender, EventArgs e)
    {
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
        var dataGridViewCellStyle4 =
            new DataGridViewCellStyle();
        var dataGridViewCellStyle5 =
            new DataGridViewCellStyle();
        var resources =
            new ComponentResourceManager(typeof(FormChannelList));
        dGV_ChannelList = new DataGridViewX();
        btn_LastPage = new Button();
        btn_NextPage = new Button();
        label_CurPage = new Label();
        btn_AtFirstPage = new Button();
        btn_AtLastPage = new Button();
        tB_BankName = new TextBox();
        label_BankName = new Label();
        Column_ID = new DataGridViewTextBoxColumn();
        Column_RxFreq = new DataGridViewTextBoxColumn();
        Column_RxCtsDcs = new DataGridViewComboBoxExColumn();
        Column_TxFreq = new DataGridViewTextBoxColumn();
        Column_TxCtsDcs = new DataGridViewComboBoxExColumn();
        Column_TxPower = new DataGridViewComboBoxColumn();
        Column_Bandwide = new DataGridViewComboBoxColumn();
        Column_ScanAdd = new DataGridViewComboBoxColumn();
        Column_SignalSystem = new DataGridViewComboBoxColumn();
        Column_SQMode = new DataGridViewComboBoxColumn();
        Column_PTTID = new DataGridViewComboBoxColumn();
        Column_SignalGroup = new DataGridViewComboBoxColumn();
        Column_Name = new DataGridViewTextBoxColumn();
        ((ISupportInitialize)dGV_ChannelList).BeginInit();
        SuspendLayout();
        dGV_ChannelList.AllowUserToAddRows = false;
        dGV_ChannelList.AllowUserToDeleteRows = false;
        dGV_ChannelList.AllowUserToResizeColumns = false;
        dGV_ChannelList.AllowUserToResizeRows = false;
        dataGridViewCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle.BackColor = Color.FromArgb(249, 249, 249);
        dataGridViewCellStyle.SelectionBackColor = SystemColors.MenuHighlight;
        dGV_ChannelList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle;
        dGV_ChannelList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                                 AnchorStyles.Left | AnchorStyles.Right;
        dGV_ChannelList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle2.BackColor = Color.Moccasin;
        dataGridViewCellStyle2.Font = new Font("宋体", 9f, FontStyle.Regular,
            GraphicsUnit.Point, 134);
        dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
        dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
        dGV_ChannelList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
        dGV_ChannelList.ColumnHeadersHeight = 32;
        dGV_ChannelList.ColumnHeadersHeightSizeMode =
            DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dGV_ChannelList.Columns.AddRange(Column_ID, Column_RxFreq, Column_RxCtsDcs,
            Column_TxFreq, Column_TxCtsDcs, Column_TxPower, Column_Bandwide, Column_ScanAdd,
            Column_SignalSystem, Column_SQMode, Column_PTTID, Column_SignalGroup, Column_Name);
        dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dataGridViewCellStyle3.BackColor = Color.FromArgb(230, 239, 238);
        dataGridViewCellStyle3.Font = new Font("宋体", 9f, FontStyle.Regular,
            GraphicsUnit.Point, 134);
        dataGridViewCellStyle3.ForeColor = SystemColors.ControlText;
        dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
        dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
        dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
        dGV_ChannelList.DefaultCellStyle = dataGridViewCellStyle3;
        dGV_ChannelList.EditMode = DataGridViewEditMode.EditOnEnter;
        dGV_ChannelList.EnableHeadersVisualStyles = false;
        dGV_ChannelList.GridColor = Color.FromArgb(208, 215, 229);
        dGV_ChannelList.Location = new Point(0, 0);
        dGV_ChannelList.Margin = new Padding(3, 2, 3, 2);
        dGV_ChannelList.MultiSelect = false;
        dGV_ChannelList.Name = "dGV_ChannelList";
        dGV_ChannelList.RowHeadersVisible = false;
        dGV_ChannelList.RowHeadersWidth = 51;
        dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
        dGV_ChannelList.RowsDefaultCellStyle = dataGridViewCellStyle4;
        dGV_ChannelList.RowTemplate.Height = 27;
        dGV_ChannelList.Size = new Size(1205, 648);
        dGV_ChannelList.TabIndex = 0;
        dGV_ChannelList.CellEndEdit +=
            dGV_ChannelList_CellEndEdit;
        btn_LastPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btn_LastPage.Location = new Point(771, 659);
        btn_LastPage.Margin = new Padding(3, 2, 3, 2);
        btn_LastPage.Name = "btn_LastPage";
        btn_LastPage.Size = new Size(104, 30);
        btn_LastPage.TabIndex = 1;
        btn_LastPage.Text = "上一区域";
        btn_LastPage.UseVisualStyleBackColor = true;
        btn_LastPage.Click += btn_LastPage_Click;
        btn_NextPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btn_NextPage.Location = new Point(955, 660);
        btn_NextPage.Margin = new Padding(3, 2, 3, 2);
        btn_NextPage.Name = "btn_NextPage";
        btn_NextPage.Size = new Size(104, 30);
        btn_NextPage.TabIndex = 2;
        btn_NextPage.Text = "下一区域";
        btn_NextPage.UseVisualStyleBackColor = true;
        btn_NextPage.Click += btn_NextPage_Click;
        label_CurPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        label_CurPage.AutoSize = true;
        label_CurPage.Location = new Point(899, 668);
        label_CurPage.Name = "label_CurPage";
        label_CurPage.Size = new Size(39, 15);
        label_CurPage.TabIndex = 3;
        label_CurPage.Text = "1/30";
        btn_AtFirstPage.Anchor =
            AnchorStyles.Bottom | AnchorStyles.Right;
        btn_AtFirstPage.Location = new Point(660, 660);
        btn_AtFirstPage.Margin = new Padding(3, 2, 3, 2);
        btn_AtFirstPage.Name = "btn_AtFirstPage";
        btn_AtFirstPage.Size = new Size(104, 30);
        btn_AtFirstPage.TabIndex = 4;
        btn_AtFirstPage.Text = "区域一";
        btn_AtFirstPage.UseVisualStyleBackColor = true;
        btn_AtFirstPage.Click += btn_AtFirstPage_Click;
        btn_AtLastPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btn_AtLastPage.Location = new Point(1064, 661);
        btn_AtLastPage.Margin = new Padding(3, 2, 3, 2);
        btn_AtLastPage.Name = "btn_AtLastPage";
        btn_AtLastPage.Size = new Size(104, 30);
        btn_AtLastPage.TabIndex = 5;
        btn_AtLastPage.Text = "区域三十";
        btn_AtLastPage.UseVisualStyleBackColor = true;
        btn_AtLastPage.Click += btn_AtLastPage_Click;
        tB_BankName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        tB_BankName.Location = new Point(140, 663);
        tB_BankName.Margin = new Padding(3, 2, 3, 2);
        tB_BankName.MaxLength = 12;
        tB_BankName.Name = "tB_BankName";
        tB_BankName.Size = new Size(148, 25);
        tB_BankName.TabIndex = 7;
        tB_BankName.TextAlign = HorizontalAlignment.Center;
        tB_BankName.KeyPress += tB_BankName_KeyPress;
        tB_BankName.Leave += tB_BankName_Leave;
        label_BankName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        label_BankName.Location = new Point(8, 660);
        label_BankName.Name = "label_BankName";
        label_BankName.Size = new Size(127, 31);
        label_BankName.TabIndex = 6;
        label_BankName.Text = "区域名称";
        label_BankName.TextAlign = ContentAlignment.MiddleRight;
        Column_ID.FillWeight = 55f;
        Column_ID.HeaderText = "信道";
        Column_ID.MinimumWidth = 6;
        Column_ID.Name = "Column_ID";
        Column_ID.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_RxFreq.HeaderText = "接收频率";
        Column_RxFreq.MinimumWidth = 6;
        Column_RxFreq.Name = "Column_RxFreq";
        Column_RxFreq.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_RxCtsDcs.DropDownHeight = 106;
        Column_RxCtsDcs.DropDownWidth = 121;
        Column_RxCtsDcs.FlatStyle = FlatStyle.Flat;
        Column_RxCtsDcs.HeaderText = "亚音解码";
        Column_RxCtsDcs.ImeMode = ImeMode.NoControl;
        Column_RxCtsDcs.IntegralHeight = false;
        Column_RxCtsDcs.ItemHeight = 20;
        Column_RxCtsDcs.MinimumWidth = 6;
        Column_RxCtsDcs.Name = "Column_RxCtsDcs";
        Column_RxCtsDcs.Resizable = DataGridViewTriState.True;
        Column_RxCtsDcs.RightToLeft = RightToLeft.No;
        Column_RxCtsDcs.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_TxFreq.HeaderText = "发射频率";
        Column_TxFreq.MinimumWidth = 6;
        Column_TxFreq.Name = "Column_TxFreq";
        Column_TxFreq.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_TxCtsDcs.DropDownHeight = 106;
        Column_TxCtsDcs.DropDownWidth = 121;
        Column_TxCtsDcs.FlatStyle = FlatStyle.Flat;
        Column_TxCtsDcs.HeaderText = "亚音编码";
        Column_TxCtsDcs.ImeMode = ImeMode.NoControl;
        Column_TxCtsDcs.IntegralHeight = false;
        Column_TxCtsDcs.ItemHeight = 20;
        Column_TxCtsDcs.MinimumWidth = 6;
        Column_TxCtsDcs.Name = "Column_TxCtsDcs";
        Column_TxCtsDcs.Resizable = DataGridViewTriState.True;
        Column_TxCtsDcs.RightToLeft = RightToLeft.No;
        Column_TxCtsDcs.SortMode = DataGridViewColumnSortMode.NotSortable;
        Column_TxPower.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        Column_TxPower.FillWeight = 60f;
        Column_TxPower.HeaderText = "功率";
        Column_TxPower.Items.AddRange("高", "中", "低");
        Column_TxPower.MinimumWidth = 6;
        Column_TxPower.Name = "Column_TxPower";
        Column_TxPower.Resizable = DataGridViewTriState.True;
        Column_Bandwide.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        Column_Bandwide.FillWeight = 60f;
        Column_Bandwide.HeaderText = "带宽";
        Column_Bandwide.Items.AddRange("宽", "窄");
        Column_Bandwide.MinimumWidth = 6;
        Column_Bandwide.Name = "Column_Bandwide";
        Column_Bandwide.Resizable = DataGridViewTriState.True;
        Column_ScanAdd.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        Column_ScanAdd.FillWeight = 80f;
        Column_ScanAdd.HeaderText = "扫描添加";
        Column_ScanAdd.Items.AddRange("删除", "添加");
        Column_ScanAdd.MinimumWidth = 6;
        Column_ScanAdd.Name = "Column_ScanAdd";
        Column_ScanAdd.Resizable = DataGridViewTriState.True;
        Column_SignalSystem.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        Column_SignalSystem.FillWeight = 80f;
        Column_SignalSystem.HeaderText = "信令";
        Column_SignalSystem.Items.AddRange("OFF", "SDC", "DTMF");
        Column_SignalSystem.Name = "Column_SignalSystem";
        Column_SQMode.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        Column_SQMode.HeaderText = "静音方式";
        Column_SQMode.Items.AddRange("QT/DQT", "QT/DQT*DTMF", "QT/DQT+DTMF");
        Column_SQMode.MinimumWidth = 6;
        Column_SQMode.Name = "Column_SQMode";
        Column_SQMode.Resizable = DataGridViewTriState.True;
        Column_PTTID.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        Column_PTTID.HeaderText = "PTT-ID";
        Column_PTTID.Items.AddRange("无", "按下PTT", "松开PTT", "按下和松开PTT");
        Column_PTTID.MinimumWidth = 6;
        Column_PTTID.Name = "Column_PTTID";
        Column_PTTID.Resizable = DataGridViewTriState.True;
        Column_SignalGroup.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
        Column_SignalGroup.FillWeight = 80f;
        Column_SignalGroup.HeaderText = "信令码";
        Column_SignalGroup.Items.AddRange("1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13",
            "14", "15", "16", "17", "18", "19", "20");
        Column_SignalGroup.MinimumWidth = 6;
        Column_SignalGroup.Name = "Column_SignalGroup";
        Column_SignalGroup.Resizable = DataGridViewTriState.True;
        dataGridViewCellStyle5.NullValue = "\"\"";
        Column_Name.DefaultCellStyle = dataGridViewCellStyle5;
        Column_Name.HeaderText = "信道名称";
        Column_Name.MaxInputLength = 12;
        Column_Name.MinimumWidth = 6;
        Column_Name.Name = "Column_Name";
        Column_Name.SortMode = DataGridViewColumnSortMode.NotSortable;
        AutoScaleDimensions = new SizeF(8f, 15f);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1205, 701);
        Controls.Add(tB_BankName);
        Controls.Add(label_BankName);
        Controls.Add(btn_AtLastPage);
        Controls.Add(btn_AtFirstPage);
        Controls.Add(label_CurPage);
        Controls.Add(btn_NextPage);
        Controls.Add(btn_LastPage);
        Controls.Add(dGV_ChannelList);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Margin = new Padding(3, 2, 3, 2);
        Name = "FormChannelList";
        StartPosition = FormStartPosition.CenterParent;
        Text = "信道列表";

        //
        dGV_ChannelList.AllowDrop = true;
        dGV_ChannelList.MouseDown += dataGridView_MouseDown;
        dGV_ChannelList.MouseMove += dataGridView_MouseMove;
        dGV_ChannelList.DragOver += dataGridView_DragOver;
        dGV_ChannelList.DragDrop += dataGridView_DragDrop;
        // right click
        RCclearThisChannel = new ToolStripMenuItem();
        RCclearThisChannel.Size = new Size(148, 22);
        RCclearThisChannel.Text = "清空该信道";
        RCclearThisChannel.Click += ctx_clearChan_Click;

        RCdelThisChannel = new ToolStripMenuItem();
        RCdelThisChannel.Size = new Size(148, 22);
        RCdelThisChannel.Text = "删除该信道";
        RCdelThisChannel.Click += ctx_removeCurrent_Click;

        RCinsertAfterChannel = new ToolStripMenuItem();
        RCinsertAfterChannel.Size = new Size(148, 22);
        RCinsertAfterChannel.Text = "插入空信道";
        RCinsertAfterChannel.Click += ctx_insertEmptyChannel_Click;

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

        RCBatchClearEmptyChannel = new ToolStripMenuItem();
        RCBatchClearEmptyChannel.Size = new Size(148, 22);
        RCBatchClearEmptyChannel.Text = "批量清除空信道";
        RCBatchClearEmptyChannel.Click += ctx_batch_clear_channel;

        contextMenuStrip = new ContextMenuStrip();
        contextMenuStrip.Items.AddRange(new[]
        {
            RCclearThisChannel,
            RCdelThisChannel,
            RCinsertAfterChannel,
            RCcopyChannel,
            RCcutChannel,
            RCpasteChannel,
            RCBatchClearEmptyChannel
        });
        // RCinsertAfterChannel});
        contextMenuStrip.Size = new Size(149, 54);

        dGV_ChannelList.CellMouseDown += dGView_CellMouseDown;
        FormClosing += FormChannelList_FormClosing;
        Load += FormChannelList_Load;
        ((ISupportInitialize)dGV_ChannelList).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}