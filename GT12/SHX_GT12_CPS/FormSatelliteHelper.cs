using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace SHX_GT12_CPS;

public partial class FormSatelliteHelper : Form
{
    private List<string> namelist = new();
    private JArray satelliteJson = new();

    private string[] currentChannel = new string[12];

    public FormSatelliteHelper()
    {
        InitializeComponent();
        button2.Hide();
        // Read json data
        loadJSON();
    }


    private string matchTone(string mode)
    {
        string pattern = @"(?i)\b(?:tone|ctcss)\s+(\d+(?:\.\d+)?)?(?=hz\b)";

        Regex regex = new Regex(pattern);

        Match match = regex.Match(mode);

        if (match.Success)
        {
            // 返回匹配的浮点数部分
            return match.Groups[1].Value;
        }
        else
        {
            return "";
        }
    }

    private void loadLB()
    {
        //update listbox here
        listBox1.Items.Clear();
        listBox2.Items.Clear();
        richTextBox1.Clear();
        richTextBox2.Clear();
        listBox1.Items.AddRange(namelist.ToArray());
    }

    private void loadJSON()
    {
        if (!File.Exists("./amsat-all-frequencies.json"))
        {
            MessageBox.Show("无卫星数据，您可以点击 更新卫星数据 进行获取！", "注意");
        }
        else
        {
            var satelliteData = File.ReadAllText("./amsat-all-frequencies.json");
            if (satelliteData == "")
            {
                MessageBox.Show("卫星数据无效，您可以点击 更新卫星数据 重新获取！", "注意");
                return;
            }

            try
            {
                satelliteJson = JArray.Parse(satelliteData);
                foreach (var b in satelliteJson) namelist.Add((string)b["name"]);
                namelist = namelist.Distinct().ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show("卫星数据无效，您可以点击 更新卫星数据 重新获取！", "注意");
                return;
            }
        }

        loadLB();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        // Update Satellite 
        // Not elegent... will do refactor.
        var url =
            "https://raw.githubusercontent.com/palewire/amateur-satellite-database/main/data/amsat-all-frequencies.json";
        var proxyPrefix = "https://mirror.ghproxy.com/";
        try
        {
            button1.Text = "更新中...";
            button1.Enabled = false;
            using (var client = new WebClient())
            {
                client.DownloadFile(url, "./amsat-all-frequencies.json");
            }

            loadJSON();
            MessageBox.Show("更新完成");
        }
        catch (Exception w)
        {
            Console.WriteLine(w);
            button1.Text = "失败重试中";
            url = proxyPrefix + url;
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, "./amsat-all-frequencies.json");
                }

                loadJSON();
                MessageBox.Show("更新完成");
            }
            catch
            {
                MessageBox.Show("更新失败，您可能需要magic...", "失败");
            }
        }
        finally
        {
            button1.Text = "卫星数据更新";
            button1.Enabled = true;
        }
    }

    private void groupBox1_Enter(object sender, EventArgs e)
    {
    }

    private void FromSatelliteHelper_Load(object sender, EventArgs e)
    {
        // throw new System.NotImplementedException();
    }

    private void label1_Click(object sender, EventArgs e)
    {
        // throw new System.NotImplementedException();
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (listBox1.SelectedItem == null) return;
        // listBox1.SelectedItem
        // throw new System.NotImplementedException();
        var current = listBox1.SelectedItem.ToString();
        var query = from item in satelliteJson where (string)item["name"] == current select item;
        listBox2.Items.Clear();
        richTextBox1.Text = current;
        foreach (var a in query)
            if (a["mode"].Value<string>() != null)
                listBox2.Items.Add(a["mode"].ToString());
    }

    private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    // Not elegent... will do refactor.
    private void richTextBox2_TextChanged(object sender, EventArgs e)
    {
        var current = richTextBox2.Text;
        var query = from item in namelist where item.ToLower().Contains(current.ToLower()) select item;
        listBox1.Items.Clear();
        foreach (var a in query) listBox1.Items.Add(a);
    }

    // Not elegent... will do refactor.
    private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (listBox2.SelectedItem == null) return;
        var currentSat = listBox1.SelectedItem.ToString();
        var currentMode = listBox2.SelectedItem.ToString();
        var query = from item in satelliteJson
            where (string)item["name"] == currentSat && (string)item["mode"] == currentMode
            select item;
        var sat = query.First()["name"];
        var uplink = query.First()["uplink"];
        var downlink = query.First()["downlink"];
        var callsign = query.First()["callsign"];
        var status = query.First()["status"];
        var satnogId = query.First()["satnogs_id"];
        var tone = matchTone(checker(query.First()["mode"]));
        richTextBox1.Text = string.Format("上行频率:{0}\n下行频率：{1}\n亚音：{2}\n呼号：{3}\n状态：{4}\nid:{5}", checker(uplink),
            checker(downlink),tone,checker(callsign),checker(status), checker(satnogId));
        currentChannel = new string[13]
        {
            "", checker(downlink), "OFF", checker(uplink), tone, "高", "宽", "删除", "OFF",
            "QT/DQT", "无","1", checker(sat)
        };
    }

    private string checker(IEnumerable<JToken> res)
    {
        return res.Value<string>() == null ? "" : res.ToString();
    }

    private void richTextBox1_TextChanged(object sender, EventArgs e)
    {
        // throw new System.NotImplementedException();
    }

    private void label2_Click(object sender, EventArgs e)
    {
        // throw new System.NotImplementedException();
    }

    private void button3_Click(object sender, EventArgs e)
    {
        listBox2_SelectedIndexChanged(null, null);
        // throw new System.NotImplementedException();
        // et laess. channel
        int lastEmpty = FormChannelList.getInstance().findLastEmpty();
        if (lastEmpty == -1)
        {
            MessageBox.Show("当前区域已满或最后一条信道不为空，无法插入！");
            return;
        }
        // "", checker(downlink), "OFF", checker(uplink), tone, "高", "宽", "删除", "OFF",
        // "QT/DQT", "无","1", checker(sat)
        if (currentChannel[1] == "")
        {
            if (checkIsFloatOrNumber(currentChannel[3]))
            {
                currentChannel[1] = currentChannel[3];
            }
        }
        if (currentChannel[3] == "")
        {
            if (checkIsFloatOrNumber(currentChannel[1]))
            {
                currentChannel[3] = currentChannel[1];
            }
        }

        if (currentChannel[4] == "")
        {
            currentChannel[4] = "OFF";
        }
            
        if (int.TryParse(currentChannel[4], out int number))
        {
            currentChannel[4] = number + ".0";
        }
        
        
        if (!(checkIsFloatOrNumber(currentChannel[1]) &&
              checkIsFloatOrNumber(currentChannel[3]) && CheckTones(currentChannel[4])))
        {
            MessageBox.Show("不支持插入当前卫星");
            return;
        } 
        
        
        // NOT ELEGANT!
        double kk;
        Double.TryParse(currentChannel[1], out kk);
        currentChannel[1]= kk.ToString("0.00000");
        
        Double.TryParse(currentChannel[3], out kk);
        currentChannel[3]= kk.ToString("0.00000");
        

        if (!checkBox1.Checked)
        {
            // "", checker(downlink), "OFF", checker(uplink), tone, "高", "宽", "删除", "OFF",
            // "QT/DQT", "无","1", checker(sat)
            currentChannel[0] = (lastEmpty+1).ToString();
            FormChannelList.getInstance().insertChannelAfter(currentChannel,lastEmpty);
            MessageBox.Show("成功！");
        }
        else
        {
            if (lastEmpty + 4 > 31)
            {
                MessageBox.Show("信道已满，无法加入！");
                return;
            }

            string uChg = richTextBox3.Text;
            string vChg = richTextBox4.Text;
            int parsedVnumber, parsedUnumber;
            bool a = Int32.TryParse(vChg, out parsedVnumber);
            bool b = Int32.TryParse(uChg, out parsedUnumber);
            if (!(a && b))
            {
                MessageBox.Show("多普勒步进有误！");
                return;
            }

            var tmp = (string[])currentChannel.Clone();
            // Dont want to use loopssssss.......................
            currentChannel[0] = (lastEmpty + 1).ToString();
            currentChannel[1] = calcDop(double.Parse(currentChannel[1]), parsedUnumber, parsedVnumber, -2,1).ToString("0.00000");
            currentChannel[3] = calcDop(double.Parse(currentChannel[3]), parsedUnumber, parsedVnumber, -2,0).ToString("0.00000");
            currentChannel[12] += "-A1";
            FormChannelList.getInstance().insertChannelAfter(currentChannel,lastEmpty);
            currentChannel = (string[])tmp.Clone();
            currentChannel[0] = (lastEmpty + 2).ToString();
            currentChannel[1] = calcDop(double.Parse(currentChannel[1]), parsedUnumber, parsedVnumber, -1,1).ToString("0.00000");
            currentChannel[3] = calcDop(double.Parse(currentChannel[3]), parsedUnumber, parsedVnumber, -1,0).ToString("0.00000");
            currentChannel[12] += "-A2";
            FormChannelList.getInstance().insertChannelAfter(currentChannel,lastEmpty + 1);
            currentChannel = (string[])tmp.Clone();
            currentChannel[0] = (lastEmpty + 3).ToString();
            currentChannel[1] = calcDop(double.Parse(currentChannel[1]), parsedUnumber, parsedVnumber, 0,1).ToString("0.00000");
            currentChannel[3] = calcDop(double.Parse(currentChannel[3]), parsedUnumber, parsedVnumber, 0,0).ToString("0.00000");
            FormChannelList.getInstance().insertChannelAfter(currentChannel,lastEmpty + 2);
            currentChannel = (string[])tmp.Clone();
            currentChannel[0] = (lastEmpty + 4).ToString();
            currentChannel[1] = calcDop(double.Parse(currentChannel[1]), parsedUnumber, parsedVnumber, 1,1).ToString("0.00000");
            currentChannel[3] = calcDop(double.Parse(currentChannel[3]), parsedUnumber, parsedVnumber, 1,0).ToString("0.00000");
            currentChannel[12] += "-L1";
            FormChannelList.getInstance().insertChannelAfter(currentChannel,lastEmpty + 3);
            currentChannel = (string[])tmp.Clone();
            currentChannel[0] = (lastEmpty + 5).ToString();
            currentChannel[1] = calcDop(double.Parse(currentChannel[1]), parsedUnumber, parsedVnumber, 2,1).ToString("0.00000");
            currentChannel[3] = calcDop(double.Parse(currentChannel[3]), parsedUnumber, parsedVnumber, 2,0).ToString("0.00000");
            currentChannel[12] += "-L2";
            FormChannelList.getInstance().insertChannelAfter(currentChannel,lastEmpty + 4);
            MessageBox.Show("成功！");
        }
    }

    //计算多普勒
    private double calcDop(double band, int U_step, int V_step, int level, int direction)
        //direction==0: uplink
    {
        // level should be -2~~2
        if (band < 300)
        {
            // V band
            if (direction == 1)
            {
                return band - 0.0005 * V_step*level;
            }
            else
            {
                return band + 0.0005 * V_step*level;
            }
        }
        else
        {
            if (direction == 1)
            {
                return band - 0.0005 * U_step*level;
            }
            else
            {
                return band + 0.0005 * U_step*level;
            }
        }

    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
        if (checkBox1.Checked)
        {
            richTextBox4.Enabled = true;
            richTextBox3.Enabled = true;
        }
        else
        {
            richTextBox4.Enabled = false;
            richTextBox3.Enabled = false;
        }
    }

    private bool checkIsFloatOrNumber(string value)
    {
        int tempInt;
        if (Int32.TryParse(value, out tempInt))
        {
            return true;
        }

        // 浮点数验证
        double tempDouble;
        if (Double.TryParse(value, out tempDouble))
        {
            return true;
        }

        // 如果既不是整数也不是浮点数，则返回false
        return false;
    }

    private bool CheckTones(string value)
    {
        var ctcss = new string[]
        {
            "OFF", "67.0", "69.3", "71.9", "74.4", "77.0", "79.7", "82.5", "85.4", "88.5",
            "91.5", "94.8", "97.4", "100.0", "103.5", "107.2", "110.9", "114.8", "118.8", "123.0",
            "127.3", "131.8", "136.5", "141.3", "146.2", "151.4", "156.7", "159.8", "162.2", "165.5",
            "167.9", "171.3", "173.8", "177.3", "179.9", "183.5", "186.2", "189.9", "192.8", "196.6",
            "199.5", "203.5", "206.5", "210.7", "218.1", "225.7", "229.1", "233.6", "241.8", "250.3",
            "254.1"
        };
        return ctcss.Contains(value);
    }

    private void label4_Click(object sender, EventArgs e)
    {
        // throw new System.NotImplementedException();
    }

    private void richTextBox3_TextChanged(object sender, EventArgs e)
    {
        // throw new System.NotImplementedException();
    }

    private void label7_Click(object sender, EventArgs e)
    {
        // throw new System.NotImplementedException();
    }
}