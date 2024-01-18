#if NET461
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQ5R.View;

// This form cons shx8800 bluetooth
// bt.5
public partial class FormConnBluetooth : Form
{
    public BleCore bleCore;

    private FormConnStat formConnStat;

    public FormConnBluetooth()
    {
        if (!isBtSupported())
        {
            MessageBox.Show("您的设备没有蓝牙功能或未打开");
            var process = new Process { StartInfo = { FileName = "control", Arguments = "bthprops.cpl" } };
            process.Start();
        }

        InitializeComponent();
    }

    private bool isBtSupported()
    {
        foreach (var allNetworkInterface in NetworkInterface.GetAllNetworkInterfaces())
            if (allNetworkInterface.Description.Contains("Bluetooth") ||
                allNetworkInterface.Description.Contains("bluetooth")) //)
                return true;

        return false;
    }

    private void dataGridViewX1_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (!scanButton.Enabled) return;

        var curRow = dataGridViewX1.CurrentRow;
        if (curRow == null)
        {
            MessageBox.Show("未选择！");
            return;
        }

        var target = curRow.Cells[3].Value.ToString();
        // var target = dataGridViewX1.Rows[e.RowIndex].Cells[3].Value.ToString();
        if (string.IsNullOrEmpty(target))
        {
            MessageBox.Show("无效的mac地址！");
            return;
        }

        if (formConnStat != null)
        {
            formConnStat.Close();
            formConnStat.Dispose();
        }

        formConnStat = new FormConnStat();
        formConnStat.StartPosition = FormStartPosition.CenterScreen;
        formConnStat.TopMost = true;
        Close();
        formConnStat.Show();
        Task.Run(() =>
        {
            //connect...
            var targetDev = bleCore.DeviceList.Find(device => bleCore.calMac(device.BluetoothAddress) == target);
            if (targetDev == null)
            {
                MessageBox.Show("出错啦！");
                formConnStat.Close();
                return;
            }

            bleCore.ConnectDevice(targetDev);
            // 连接中... 成功！ 失败！
            while (bleCore.connStep == BTConsts.STATUS_READY) Thread.Sleep(10);

            if (bleCore.connStep == BTConsts.STATUS_CONN_FAILED)
            {
                formConnStat.labelStatus.Text = "连接失败";
                formConnStat.TopMost = false;
                MessageBox.Show("请检查设备状态或您使用的不是森海克斯8800");
                formConnStat.Close();
            }
            else
            {
                formConnStat.labelStatus.Text = "连接成功！";
                Thread.Sleep(800);
                var gattCharacteristic =
                    bleCore.characteristics.Find(x => x.Uuid.ToString().Contains(BTConsts.RW_CHARACTERISTIC_UUID));
                if (gattCharacteristic == null)
                {
                    formConnStat.labelStatus.Text = "无写特征";
                    formConnStat.TopMost = false;
                    MessageBox.Show("没有找到写特征，请确认您使用的是森海克斯8800！");
                    formConnStat.Close();
                    return;
                }

                bleCore.SetOpteron(gattCharacteristic);
                formConnStat.labelStatus.Text = "已找到写特征,请进行读写操作";
                Thread.Sleep(3000);
                //gogogo!
                formConnStat.Close();
            }
        });
    }

    private void scanButton_Click(object sender, EventArgs e)
    {
        if (!isBtSupported())
        {
            MessageBox.Show("蓝牙未打开或不支持");
            var process = new Process { StartInfo = { FileName = "control", Arguments = "bthprops.cpl" } };
            process.Start();
            return;
        }

        dataGridViewX1.Rows.Clear();
        ButtonConnectDevice.Enabled = false;
        bleCore = BleCore.BleInstance();
        bleCore.Dispose();
        bleCore.setView(dataGridViewX1);
        bleCore.setCheckbox(checkBoxDisableSSIDFilter, checkboxDisableWeakSignal);
        checkBoxDisableSSIDFilter.Enabled = false;
        scanButton.Enabled = false;
        checkboxDisableWeakSignal.Enabled = false;
        scanButton.Text = "扫描中，稍等!";
        bleCore.DeviceWatcherChanged += bleCore.TriggerDeviceWatcherChanged;
        bleCore.CharacteristicAdded += bleCore.TriggerCharacteristicAdded;
        bleCore.CharacteristicFinish += bleCore.TriggerCharacteristicFinish;
        bleCore.Recdate += bleCore.TriggerRecdata;
        Task.Factory.StartNew(() =>
        {
            // sleep 10s
            Thread.Sleep(10000);
            bleCore.StopBleDeviceWatcher();
            scanButton.Enabled = true;
            checkBoxDisableSSIDFilter.Enabled = true;
            scanButton.Text = "扫描结束，按下再次扫描";
            ButtonConnectDevice.Enabled = true;
            checkboxDisableWeakSignal.Enabled = true;
        });
        bleCore.StartBleDeviceWatcher();
    }

    private void ButtonConnectDeviceClick(object sender, EventArgs e)
    {
        dataGridViewX1_CellContentClick(sender, null);
    }
}
#endif