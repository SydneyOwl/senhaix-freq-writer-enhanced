#define DEBUG
using System;
using System.Diagnostics;
using System.Text;
using System.Timers;
using HID;
using SHX_GT12_CPS.Properties;

namespace SHX_GT12_CPS;

public class Communication
{
    private readonly DataHelper helper;

    private readonly HIDInterface hid;

    private readonly OP_TYPE opType;

    private readonly string[] tblCTSDCS = new string[210]
    {
        "D023N", "D025N", "D026N", "D031N", "D032N", "D036N", "D043N", "D047N", "D051N", "D053N",
        "D054N", "D065N", "D071N", "D072N", "D073N", "D074N", "D114N", "D115N", "D116N", "D122N",
        "D125N", "D131N", "D132N", "D134N", "D143N", "D145N", "D152N", "D155N", "D156N", "D162N",
        "D165N", "D172N", "D174N", "D205N", "D212N", "D223N", "D225N", "D226N", "D243N", "D244N",
        "D245N", "D246N", "D251N", "D252N", "D255N", "D261N", "D263N", "D265N", "D266N", "D271N",
        "D274N", "D306N", "D311N", "D315N", "D325N", "D331N", "D332N", "D343N", "D346N", "D351N",
        "D356N", "D364N", "D365N", "D371N", "D411N", "D412N", "D413N", "D423N", "D431N", "D432N",
        "D445N", "D446N", "D452N", "D454N", "D455N", "D462N", "D464N", "D465N", "D466N", "D503N",
        "D506N", "D516N", "D523N", "D526N", "D532N", "D546N", "D565N", "D606N", "D612N", "D624N",
        "D627N", "D631N", "D632N", "D645N", "D654N", "D662N", "D664N", "D703N", "D712N", "D723N",
        "D731N", "D732N", "D734N", "D743N", "D754N", "D023I", "D025I", "D026I", "D031I", "D032I",
        "D036I", "D043I", "D047I", "D051I", "D053I", "D054I", "D065I", "D071I", "D072I", "D073I",
        "D074I", "D114I", "D115I", "D116I", "D122I", "D125I", "D131I", "D132I", "D134I", "D143I",
        "D145I", "D152I", "D155I", "D156I", "D162I", "D165I", "D172I", "D174I", "D205I", "D212I",
        "D223I", "D225I", "D226I", "D243I", "D244I", "D245I", "D246I", "D251I", "D252I", "D255I",
        "D261I", "D263I", "D265I", "D266I", "D271I", "D274I", "D306I", "D311I", "D315I", "D325I",
        "D331I", "D332I", "D343I", "D346I", "D351I", "D356I", "D364I", "D365I", "D371I", "D411I",
        "D412I", "D413I", "D423I", "D431I", "D432I", "D445I", "D446I", "D452I", "D454I", "D455I",
        "D462I", "D464I", "D465I", "D466I", "D503I", "D506I", "D516I", "D523I", "D526I", "D532I",
        "D546I", "D565I", "D606I", "D612I", "D624I", "D627I", "D631I", "D632I", "D645I", "D654I",
        "D662I", "D664I", "D703I", "D712I", "D723I", "D731I", "D732I", "D734I", "D743I", "D754I"
    };

    public AppData appData;

    private bool flagReceiveData;

    private bool flagRetry;

    private bool flagTransmitting;

    private string progressCont = "";

    private int progressVal;

    private byte[] rxBuffer = new byte[64];

    private STEP step;

    private Timer timer;

    private byte timesOfRetry = 5;

    public Communication(HIDInterface hid, AppData appData, OP_TYPE opType)
    {
        this.hid = hid;
        this.opType = opType;
        if (this.opType == OP_TYPE.READ)
        {
            var lANG = Settings.Default.LANG;
            this.appData = new AppData(lANG);
        }
        else
        {
            this.appData = appData;
        }

        helper = new DataHelper();
        UpdateProgressBar = new ProgressBarIssue();
        TimerInit();
    }

    public ProgressBarIssue UpdateProgressBar { get; set; }

    private void TimerInit()
    {
        timer = new Timer();
        timer.Interval = 1000.0;
        timer.Elapsed += Timer_Elapsed;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        flagRetry = true;
    }

    public void DataReceived(object sender, byte[] e)
    {
        rxBuffer = e;
        flagReceiveData = true;
    }

    private void resetRetryCount()
    {
        timesOfRetry = 5;
        flagRetry = false;
    }

    public bool DoIt()
    {
        flagTransmitting = true;
        resetRetryCount();
        step = STEP.STEP_HANDSHAKE_1;
        if (HandShake())
        {
            if (opType == OP_TYPE.WRITE)
            {
                if (Write()) return true;
            }
            else if (Read())
            {
                return true;
            }
        }

        return false;
    }

    private bool HandShake()
    {
        var byData = new byte[1];
        while (flagTransmitting)
            if (!flagRetry)
            {
                switch (step)
                {
                    case STEP.STEP_HANDSHAKE_1:
                        byData = Encoding.ASCII.GetBytes("PROGRAMGT12");
                        byData = helper.LoadPackage(1, 0, byData, (byte)byData.Length);
                        hid.Send(byData);
                        progressVal = 0;
                        progressCont = "握手...";
                        UpdateProgressBar.IssueProgressValue(progressVal, progressCont);
                        timer.Start();
                        resetRetryCount();
                        step = STEP.STEP_HANDSHAKE_2;
                        break;
                    case STEP.STEP_HANDSHAKE_2:
                        if (flagReceiveData)
                        {
                            flagReceiveData = false;
                            helper.AnalyzePackage(rxBuffer);
                            if (helper.errorCode == ENUM_ERRORS.ER_NONE)
                            {
                                byData = helper.LoadPackage(2, 0, null, 1);
                                hid.Send(byData);
                                resetRetryCount();
                                step = STEP.STEP_HANDSHAKE_3;
                            }
                        }

                        break;
                    case STEP.STEP_HANDSHAKE_3:
                        if (flagReceiveData)
                        {
                            flagReceiveData = false;
                            helper.AnalyzePackage(rxBuffer);
                            if (helper.errorCode == ENUM_ERRORS.ER_NONE)
                            {
                                byData = helper.LoadPackage(70, 0, null, 1);
                                hid.Send(byData);
                                resetRetryCount();
                                step = STEP.STEP_HANDSHAKE_4;
                            }
                        }

                        break;
                    case STEP.STEP_HANDSHAKE_4:
                        if (!flagReceiveData) break;

                        flagReceiveData = false;
                        helper.AnalyzePackage(rxBuffer);
                        if (helper.errorCode == ENUM_ERRORS.ER_NONE)
                        {
                            timer.Stop();
                            resetRetryCount();
                            progressVal = 0;
                            progressCont = "进度..." + progressVal + "%";
                            UpdateProgressBar.IssueProgressValue(progressVal, progressCont);
                            if (opType == OP_TYPE.READ)
                            {
                                step = STEP.STEP_READ1;
                                return true;
                            }

                            step = STEP.STEP_WRITE1;
                            return true;
                        }

                        break;
                }
            }
            else
            {
                if (timesOfRetry <= 0)
                {
                    timer.Stop();
                    flagTransmitting = false;
                    return false;
                }

                timesOfRetry--;
                flagRetry = false;
                hid.Send(byData);
            }

        return false;
    }

    private bool Write()
    {
        var byData = new byte[10];
        var array = new byte[48];
        ushort num = 0;
        byte b = 0;
        var num2 = 0;
        while (flagTransmitting)
            if (!flagRetry)
            {
                switch (step)
                {
                    case STEP.STEP_WRITE1:
                        if (num < 30720)
                        {
                            var channelInfos = GetChannelInfos(num2++);
                            Array.Copy(channelInfos, 0, array, 0, 32);
                        }
                        else if (num >= 32000 && num < 32480)
                        {
                            var num3 = (num - 32000) / 16;
                            GetBankName(array, 0, appData.BankName[num3]);
                            GetBankName(array, 16, appData.BankName[num3 + 1]);
                        }
                        else if (num == 32768)
                        {
                            var vFOAInfos = GetVFOAInfos();
                            for (var j = 0; j < 32; j++) array[j] = vFOAInfos[j];
                        }
                        else if (num == 32800)
                        {
                            var vFOBInfos = GetVFOBInfos();
                            for (var k = 0; k < 32; k++) array[k] = vFOBInfos[k];
                        }
                        else if (num == 36864)
                        {
                            array[0] = (byte)appData.FunCfgs.Sql;
                            array[1] = (byte)appData.FunCfgs.SaveMode;
                            array[2] = (byte)appData.FunCfgs.Vox;
                            array[3] = (byte)appData.FunCfgs.VoxDlyTime;
                            array[4] = (byte)appData.FunCfgs.DualStandby;
                            array[5] = (byte)appData.FunCfgs.Tot;
                            array[6] = (byte)appData.FunCfgs.Beep;
                            array[7] = (byte)appData.FunCfgs.SideTone;
                            array[8] = (byte)appData.FunCfgs.ScanMode;
                            array[9] = (byte)appData.Vfos.Pttid;
                            array[10] = (byte)appData.FunCfgs.PttDly;
                            array[11] = (byte)appData.FunCfgs.ChADisType;
                            array[12] = (byte)appData.FunCfgs.ChBDisType;
                            array[13] = (byte)appData.FunCfgs.CbBMisscall;
                            array[14] = (byte)appData.FunCfgs.AutoLock;
                            array[15] = (byte)appData.FunCfgs.MicGain;
                            array[16] = (byte)appData.FunCfgs.AlarmMode;
                            array[17] = (byte)appData.FunCfgs.TailClear;
                            array[18] = (byte)appData.FunCfgs.RptTailClear;
                            array[19] = (byte)appData.FunCfgs.RptTailDet;
                            array[20] = (byte)appData.FunCfgs.Roger;
                            array[21] = 0;
                            array[22] = (byte)appData.FunCfgs.FmEnable;
                            array[23] = 0;
                            array[23] |= (byte)appData.FunCfgs.ChAWorkmode;
                            array[23] |= (byte)(appData.FunCfgs.ChBWorkmode << 4);
                            array[24] = (byte)appData.FunCfgs.KeyLock;
                            array[25] = (byte)appData.FunCfgs.AutoPowerOff;
                            array[26] = (byte)appData.FunCfgs.PowerOnDisType;
                            array[27] = 0;
                            array[28] = (byte)appData.FunCfgs.Tone;
                            array[29] = (byte)appData.FunCfgs.CurBank;
                            array[30] = (byte)appData.FunCfgs.Backlight;
                            array[31] = (byte)appData.FunCfgs.MenuQuitTime;
                        }
                        else if (num == 36896)
                        {
                            array[0] = (byte)appData.FunCfgs.Key1Short;
                            array[1] = (byte)appData.FunCfgs.Key1Long;
                            array[2] = (byte)appData.FunCfgs.Key2Short;
                            array[3] = (byte)appData.FunCfgs.Key2Long;
                            array[4] = (byte)appData.FunCfgs.Bright;
                            array[5] = 0;
                            array[6] = 0;
                            array[7] = (byte)appData.FunCfgs.VoxSw;
                            array[8] = (byte)appData.FunCfgs.PowerUpDisTime;
                            array[9] = (byte)appData.FunCfgs.BluetoothAudioGain;
                            if (appData.FunCfgs.CallSign != null && appData.FunCfgs.CallSign != "")
                            {
                                var bytes = Encoding.GetEncoding("gb2312").GetBytes(appData.FunCfgs.CallSign);
                                Debug.WriteLine($"Callsign: {appData.FunCfgs.CallSign}");
                                var array2 = bytes;
                                foreach (var b2 in array2) Debug.WriteLine($"Byte: {b2}");

                                Array.Copy(bytes, 0, array, 10, bytes.Length);
                            }
                        }
                        else if (num == 40960)
                        {
                            GetFMFreq(array, 0, appData.Fms.CurFreq);
                            GetFMFreq(array, 2, appData.Fms.Channels[0]);
                            GetFMFreq(array, 4, appData.Fms.Channels[1]);
                            GetFMFreq(array, 6, appData.Fms.Channels[2]);
                            GetFMFreq(array, 8, appData.Fms.Channels[3]);
                            GetFMFreq(array, 10, appData.Fms.Channels[4]);
                            GetFMFreq(array, 12, appData.Fms.Channels[5]);
                            GetFMFreq(array, 14, appData.Fms.Channels[6]);
                            GetFMFreq(array, 16, appData.Fms.Channels[7]);
                            GetFMFreq(array, 18, appData.Fms.Channels[8]);
                            GetFMFreq(array, 20, appData.Fms.Channels[9]);
                            GetFMFreq(array, 22, appData.Fms.Channels[10]);
                            GetFMFreq(array, 24, appData.Fms.Channels[11]);
                            GetFMFreq(array, 26, appData.Fms.Channels[12]);
                            GetFMFreq(array, 28, appData.Fms.Channels[13]);
                            GetFMFreq(array, 30, appData.Fms.Channels[14]);
                        }
                        else if (num >= 45056 && num < 45744)
                        {
                            switch (num)
                            {
                                case 45056:
                                    GetDTMFWord(array, 0, appData.Dtmfs.LocalID);
                                    GetDTMFWord(array, 16, appData.Dtmfs.Group[0]);
                                    break;
                                case 45088:
                                    GetDTMFWord(array, 0, appData.Dtmfs.Group[1]);
                                    GetDTMFWord(array, 16, appData.Dtmfs.Group[2]);
                                    break;
                                case 45120:
                                    GetDTMFWord(array, 0, appData.Dtmfs.Group[3]);
                                    GetDTMFWord(array, 16, appData.Dtmfs.Group[4]);
                                    break;
                                case 45152:
                                    GetDTMFWord(array, 0, appData.Dtmfs.Group[5]);
                                    GetDTMFWord(array, 16, appData.Dtmfs.Group[6]);
                                    break;
                                case 45184:
                                    GetDTMFWord(array, 0, appData.Dtmfs.Group[7]);
                                    GetDTMFWord(array, 16, appData.Dtmfs.Group[8]);
                                    break;
                                case 45216:
                                    GetDTMFWord(array, 0, appData.Dtmfs.Group[9]);
                                    GetDTMFWord(array, 16, appData.Dtmfs.Group[10]);
                                    break;
                                case 45248:
                                    GetDTMFWord(array, 0, appData.Dtmfs.Group[11]);
                                    GetDTMFWord(array, 16, appData.Dtmfs.Group[12]);
                                    break;
                                case 45280:
                                    GetDTMFWord(array, 0, appData.Dtmfs.Group[13]);
                                    GetDTMFWord(array, 16, appData.Dtmfs.Group[14]);
                                    break;
                                case 45312:
                                    GetDTMFWord(array, 0, appData.Dtmfs.Group[15]);
                                    GetDTMFWord(array, 16, appData.Dtmfs.Group[16]);
                                    break;
                                case 45344:
                                    GetDTMFWord(array, 0, appData.Dtmfs.Group[17]);
                                    GetDTMFWord(array, 16, appData.Dtmfs.Group[18]);
                                    break;
                                case 45376:
                                    GetDTMFWord(array, 0, appData.Dtmfs.Group[19]);
                                    GetDTMFName(array, 16, appData.Dtmfs.GroupName[0]);
                                    break;
                                case 45408:
                                    GetDTMFName(array, 0, appData.Dtmfs.GroupName[1]);
                                    GetDTMFName(array, 16, appData.Dtmfs.GroupName[2]);
                                    break;
                                case 45440:
                                    GetDTMFName(array, 0, appData.Dtmfs.GroupName[3]);
                                    GetDTMFName(array, 16, appData.Dtmfs.GroupName[4]);
                                    break;
                                case 45472:
                                    GetDTMFName(array, 0, appData.Dtmfs.GroupName[5]);
                                    GetDTMFName(array, 16, appData.Dtmfs.GroupName[6]);
                                    break;
                                case 45504:
                                    GetDTMFName(array, 0, appData.Dtmfs.GroupName[7]);
                                    GetDTMFName(array, 16, appData.Dtmfs.GroupName[8]);
                                    break;
                                case 45536:
                                    GetDTMFName(array, 0, appData.Dtmfs.GroupName[9]);
                                    GetDTMFName(array, 16, appData.Dtmfs.GroupName[10]);
                                    break;
                                case 45568:
                                    GetDTMFName(array, 0, appData.Dtmfs.GroupName[11]);
                                    GetDTMFName(array, 16, appData.Dtmfs.GroupName[12]);
                                    break;
                                case 45600:
                                    GetDTMFName(array, 0, appData.Dtmfs.GroupName[13]);
                                    GetDTMFName(array, 16, appData.Dtmfs.GroupName[14]);
                                    break;
                                case 45632:
                                    GetDTMFName(array, 0, appData.Dtmfs.GroupName[15]);
                                    GetDTMFName(array, 16, appData.Dtmfs.GroupName[16]);
                                    break;
                                case 45664:
                                    GetDTMFName(array, 0, appData.Dtmfs.GroupName[17]);
                                    GetDTMFName(array, 16, appData.Dtmfs.GroupName[18]);
                                    break;
                                case 45696:
                                    GetDTMFName(array, 0, appData.Dtmfs.GroupName[19]);
                                    array[16] = (byte)appData.Dtmfs.WordTime;
                                    array[17] = (byte)appData.Dtmfs.IdleTime;
                                    break;
                                case 45728:
                                    GetCallIDWord(array, 0, appData.Mdcs.CallID1);
                                    GetMDC1200_IDWord(array, 16, appData.Mdcs.Id);
                                    break;
                            }
                        }

                        byData = helper.LoadPackage(87, num, array, (byte)array.Length);
                        hid.Send(byData);
                        timer.Start();
                        progressVal = num * 100 / 45728;
                        if (progressVal > 100) progressVal = 100;

                        progressCont = "进度..." + progressVal + "%";
                        UpdateProgressBar.IssueProgressValue(progressVal, progressCont);
                        step = STEP.STEP_WRITE2;
                        break;
                    case STEP.STEP_WRITE2:
                        if (!flagReceiveData) break;

                        flagReceiveData = false;
                        helper.AnalyzePackage(rxBuffer);
                        if (helper.errorCode == ENUM_ERRORS.ER_NONE)
                        {
                            timer.Stop();
                            resetRetryCount();
                            for (var i = 0; i < 32; i++) array[i] = byte.MaxValue;

                            if (num >= 45728)
                            {
                                progressVal = 100;
                                progressCont = "完成";
                                UpdateProgressBar.IssueProgressValue(progressVal, progressCont);
                                flagTransmitting = false;
                                return true;
                            }

                            num += 32;
                            step = STEP.STEP_WRITE1;
                        }

                        break;
                }
            }
            else
            {
                if (timesOfRetry <= 0)
                {
                    timer.Stop();
                    flagTransmitting = false;
                    return false;
                }

                timesOfRetry--;
                flagRetry = false;
                hid.Send(byData);
            }

        return false;
    }

    private byte[] CaculateCtsDcs(string strData)
    {
        var array = new byte[2] { 255, 255 };
        if ('D' == strData[0])
        {
            int i;
            for (i = 0; i < 210; i++)
                if (strData == tblCTSDCS[i])
                {
                    i++;
                    break;
                }

            array[0] = (byte)i;
            array[1] = 0;
        }
        else if (strData != "OFF")
        {
            var startIndex = strData.Length - 2;
            strData = strData.Remove(startIndex, 1);
            var num = ushort.Parse(strData);
            var b = (byte)num;
            var b2 = (byte)((num & 0xFF00) >> 8);
            array[0] = b;
            array[1] = b2;
        }
        else
        {
            array[0] = 0;
            array[1] = 0;
        }

        return array;
    }

    private byte[] CaculateFreq_StrToHex(string strDat)
    {
        var array = new byte[4] { 255, 255, 255, 255 };
        var num = int.Parse(strDat.Remove(3, 1));
        for (var i = 0; i < 4; i++)
        {
            var num2 = num % 100;
            num /= 100;
            array[i] = (byte)(((num2 / 10) << 4) | (num2 % 10));
        }

        return array;
    }

    private byte[] GetChannelInfos(int CH_Num)
    {
        var num = CH_Num / 32;
        var num2 = CH_Num % 32;
        var array = new byte[32]
        {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255
        };
        if (appData.ChannelList[num][num2].RxFreq != "")
        {
            var sourceArray = CaculateFreq_StrToHex(appData.ChannelList[num][num2].RxFreq);
            Array.Copy(sourceArray, 0, array, 0, 4);
            if (appData.ChannelList[num][num2].TxFreq != "")
            {
                var sourceArray2 = CaculateFreq_StrToHex(appData.ChannelList[num][num2].TxFreq);
                Array.Copy(sourceArray2, 0, array, 4, 4);
            }

            var sourceArray3 = CaculateCtsDcs(appData.ChannelList[num][num2].StrRxCtsDcs);
            Array.Copy(sourceArray3, 0, array, 8, 2);
            sourceArray3 = CaculateCtsDcs(appData.ChannelList[num][num2].StrTxCtsDcs);
            Array.Copy(sourceArray3, 0, array, 10, 2);
            array[12] = (byte)appData.ChannelList[num][num2].SignalGroup;
            array[13] = (byte)appData.ChannelList[num][num2].Pttid;
            array[14] = (byte)appData.ChannelList[num][num2].TxPower;
            array[15] = 0;
            array[15] |= (byte)(appData.ChannelList[num][num2].Bandwide << 6);
            array[15] |= (byte)(appData.ChannelList[num][num2].SqMode << 4);
            array[15] |= (byte)(appData.ChannelList[num][num2].ScanAdd << 2);
            array[15] |= (byte)(appData.ChannelList[num][num2].SignalSystem & 3);
            if (appData.ChannelList[num][num2].Name != "")
            {
                var num3 = 0;
                var bytes = Encoding.GetEncoding("gb2312").GetBytes(appData.ChannelList[num][num2].Name);
                num3 = bytes.Length > 12 ? 12 : bytes.Length;
                Array.Copy(bytes, 0, array, 20, num3);
            }
        }

        return array;
    }

    private void GetBankName(byte[] payload, int offset, string name)
    {
        var num = 0;
        if (name != "")
        {
            var bytes = Encoding.GetEncoding("gb2312").GetBytes(name);
            num = bytes.Length;
            if (num > 12) num = 12;

            for (var i = 0; i < num; i++) payload[i + offset] = bytes[i];
        }
    }

    private byte[] GetVFOAInfos()
    {
        var array = new byte[32]
        {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 0, 0, 0,
            0, 0, 0, 255, 255, 255, 255, 255, 255, 255,
            255, 255
        };
        var s = appData.Vfos.VfoAFreq.Remove(3, 1);
        var num = int.Parse(s);
        array[0] = (byte)((uint)num & 0xFFu);
        array[1] = (byte)((uint)(num >> 8) & 0xFFu);
        array[2] = (byte)((uint)(num >> 16) & 0xFFu);
        array[3] = (byte)((uint)(num >> 24) & 0xFFu);
        var sourceArray = CaculateCtsDcs(appData.Vfos.StrVFOARxCtsDcs);
        Array.Copy(sourceArray, 0, array, 4, 2);
        sourceArray = CaculateCtsDcs(appData.Vfos.StrVFOATxCtsDcs);
        Array.Copy(sourceArray, 0, array, 6, 2);
        array[8] = (byte)appData.Vfos.VfoABusyLock;
        array[9] = (byte)appData.Vfos.VfoASignalGroup;
        array[10] = (byte)appData.Vfos.VfoATxPower;
        array[11] = (byte)appData.Vfos.VfoABandwide;
        array[12] = (byte)appData.Vfos.VfoAScram;
        array[13] = (byte)appData.Vfos.VfoAStep;
        array[14] = (byte)appData.Vfos.VfoADir;
        var s2 = appData.Vfos.VfoAOffset.Remove(2, 1);
        var num2 = int.Parse(s2) * 10;
        array[15] = (byte)((uint)num2 & 0xFFu);
        array[16] = (byte)((uint)(num2 >> 8) & 0xFFu);
        array[17] = (byte)((uint)(num2 >> 16) & 0xFFu);
        array[18] = (byte)((uint)(num2 >> 24) & 0xFFu);
        array[19] = (byte)appData.Vfos.VfoASQMode;
        array[26] = (byte)appData.Vfos.VfoASignalSystem;
        return array;
    }

    private byte[] GetVFOBInfos()
    {
        var array = new byte[32]
        {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 0, 0, 0,
            0, 0, 0, 255, 255, 255, 255, 255, 255, 255,
            255, 255
        };
        var s = appData.Vfos.VfoBFreq.Remove(3, 1);
        var num = int.Parse(s);
        array[0] = (byte)((uint)num & 0xFFu);
        array[1] = (byte)((uint)(num >> 8) & 0xFFu);
        array[2] = (byte)((uint)(num >> 16) & 0xFFu);
        array[3] = (byte)((uint)(num >> 24) & 0xFFu);
        var sourceArray = CaculateCtsDcs(appData.Vfos.StrVFOBRxCtsDcs);
        Array.Copy(sourceArray, 0, array, 4, 2);
        sourceArray = CaculateCtsDcs(appData.Vfos.StrVFOBTxCtsDcs);
        Array.Copy(sourceArray, 0, array, 6, 2);
        array[8] = (byte)appData.Vfos.VfoBBusyLock;
        array[9] = (byte)appData.Vfos.VfoBSignalGroup;
        array[10] = (byte)appData.Vfos.VfoBTxPower;
        array[11] = (byte)appData.Vfos.VfoBBandwide;
        array[12] = (byte)appData.Vfos.VfoBScram;
        array[13] = (byte)appData.Vfos.VfoBStep;
        array[14] = (byte)appData.Vfos.VfoBDir;
        var s2 = appData.Vfos.VfoBOffset.Remove(2, 1);
        var num2 = int.Parse(s2) * 10;
        array[15] = (byte)((uint)num2 & 0xFFu);
        array[16] = (byte)((uint)(num2 >> 8) & 0xFFu);
        array[17] = (byte)((uint)(num2 >> 16) & 0xFFu);
        array[18] = (byte)((uint)(num2 >> 24) & 0xFFu);
        array[19] = (byte)appData.Vfos.VfoBSQMode;
        array[26] = (byte)appData.Vfos.VfoBSignalSystem;
        return array;
    }

    private void GetFMFreq(byte[] payload, int offset, int freq)
    {
        if (freq != 0)
        {
            payload[offset] = (byte)freq;
            payload[offset + 1] = (byte)(freq >> 8);
        }
    }

    private void GetDTMFWord(byte[] payload, int offset, string word)
    {
        var text = "0123456789ABCD*#";
        var num = 0;
        if (!(word != "")) return;

        for (var i = 0; i < word.Length; i++)
        {
            num = text.IndexOf(char.ToUpper(word[i]));
            if (num != -1)
            {
                payload[i + offset] = (byte)num;
                continue;
            }

            break;
        }
    }

    private void GetDTMFName(byte[] payload, int offset, string word)
    {
        var num = 0;
        if (word != "")
        {
            var bytes = Encoding.GetEncoding("gb2312").GetBytes(word);
            num = bytes.Length;
            if (num > 12) num = 12;

            for (var i = 0; i < num; i++) payload[i + offset] = bytes[i];
        }
    }

    private void GetCallIDWord(byte[] payload, int offset, string callID)
    {
        if (callID != "")
        {
            var bytes = Encoding.ASCII.GetBytes(callID);
            for (var i = 0; i < callID.Length; i++) payload[i + offset] = bytes[i];
        }
    }

    private void GetMDC1200_IDWord(byte[] payload, int offset, string id)
    {
        var text = "0123456789ABCDEF";
        if (id != "")
            for (var i = 0; i < id.Length; i++)
                payload[i + offset] = (byte)text.IndexOf(id[i]);
    }

    private bool Read()
    {
        var byData = new byte[10];
        ushort num = 0;
        var num2 = 0;
        while (flagTransmitting)
            if (!flagRetry)
            {
                switch (step)
                {
                    case STEP.STEP_READ1:
                        byData = helper.LoadPackage(82, num, null, 1);
                        hid.Send(byData);
                        progressVal = num * 100 / 45728;
                        if (progressVal > 100) progressVal = 100;

                        progressCont = "进度..." + progressVal + "%";
                        UpdateProgressBar.IssueProgressValue(progressVal, progressCont);
                        resetRetryCount();
                        timer.Start();
                        step = STEP.STEP_READ2;
                        break;
                    case STEP.STEP_READ2:
                        if (!flagReceiveData) break;

                        flagReceiveData = false;
                        timer.Stop();
                        resetRetryCount();
                        helper.AnalyzePackage(rxBuffer);
                        if (helper.errorCode != ENUM_ERRORS.ER_NONE) break;

                        if (num < 30720)
                        {
                            SetChannelInfos(num2++, helper.payload);
                        }
                        else if (num >= 32000 && num < 32480)
                        {
                            var num3 = (num - 32000) / 16;
                            appData.BankName[num3] = SetBankName(helper.payload, 0);
                            appData.BankName[num3 + 1] = SetBankName(helper.payload, 16);
                        }
                        else if (num == 32768)
                        {
                            SetVFOAInfos(helper.payload);
                        }
                        else if (num == 32800)
                        {
                            SetVFOBInfos(helper.payload);
                        }
                        else if (num == 36864)
                        {
                            appData.FunCfgs.Sql = helper.payload[0] % 10;
                            appData.FunCfgs.SaveMode = helper.payload[1] % 2;
                            appData.FunCfgs.Vox = helper.payload[2] % 3;
                            appData.FunCfgs.VoxDlyTime = helper.payload[3] % 16;
                            appData.FunCfgs.DualStandby = helper.payload[4] % 2;
                            appData.FunCfgs.Tot = helper.payload[5] % 9;
                            appData.FunCfgs.Beep = helper.payload[6] % 4;
                            appData.FunCfgs.SideTone = helper.payload[7] % 2;
                            appData.FunCfgs.ScanMode = helper.payload[8] % 3;
                            appData.Vfos.Pttid = helper.payload[9] % 4;
                            appData.FunCfgs.PttDly = helper.payload[10] % 16;
                            appData.FunCfgs.ChADisType = helper.payload[11] % 3;
                            appData.FunCfgs.ChBDisType = helper.payload[12] % 3;
                            appData.FunCfgs.CbBMisscall = helper.payload[13] % 2;
                            appData.FunCfgs.AutoLock = helper.payload[14] % 7;
                            appData.FunCfgs.MicGain = helper.payload[15] % 3;
                            appData.FunCfgs.AlarmMode = helper.payload[16] % 3;
                            appData.FunCfgs.TailClear = helper.payload[17] % 2;
                            appData.FunCfgs.RptTailClear = helper.payload[18] % 11;
                            appData.FunCfgs.RptTailDet = helper.payload[19] % 11;
                            appData.FunCfgs.Roger = helper.payload[20] % 2;
                            appData.FunCfgs.FmEnable = helper.payload[22] % 2;
                            appData.FunCfgs.ChAWorkmode = (helper.payload[23] & 0xF) % 2;
                            appData.FunCfgs.ChBWorkmode = ((helper.payload[23] & 0xF0) >> 4) % 2;
                            appData.FunCfgs.KeyLock = helper.payload[24] % 2;
                            appData.FunCfgs.AutoPowerOff = helper.payload[25] % 5;
                            appData.FunCfgs.PowerOnDisType = helper.payload[26] % 3;
                            appData.FunCfgs.Tone = helper.payload[28] % 4;
                            appData.FunCfgs.CurBank = helper.payload[29] % 30;
                            appData.FunCfgs.Backlight = helper.payload[30] % 9;
                            appData.FunCfgs.MenuQuitTime = helper.payload[31] % 11;
                        }
                        else if (num == 36896)
                        {
                            appData.FunCfgs.Key1Short = helper.payload[0] % 10;
                            appData.FunCfgs.Key1Long = helper.payload[1] % 10;
                            appData.FunCfgs.Key2Short = helper.payload[2] % 10;
                            appData.FunCfgs.Key2Long = helper.payload[3] % 10;
                            appData.FunCfgs.Bright = helper.payload[4] % 2;
                            appData.FunCfgs.VoxSw = helper.payload[7] % 2;
                            appData.FunCfgs.PowerUpDisTime = helper.payload[8] % 15;
                            appData.FunCfgs.BluetoothAudioGain = helper.payload[9] % 5;
                            var num4 = 0;
                            for (var i = 0; i < 6 && helper.payload[10 + i] != byte.MaxValue; i++)
                            {
                                num4++;
                                Debug.WriteLine("Byte:{0}", helper.payload[10 + i]);
                            }

                            appData.FunCfgs.CallSign =
                                Encoding.GetEncoding("gb2312").GetString(helper.payload, 10, num4);
                        }
                        else if (num == 40960)
                        {
                            appData.Fms.CurFreq = SetFMFreq(helper.payload, 0);
                            appData.Fms.Channels[0] = SetFMFreq(helper.payload, 2);
                            appData.Fms.Channels[1] = SetFMFreq(helper.payload, 4);
                            appData.Fms.Channels[2] = SetFMFreq(helper.payload, 6);
                            appData.Fms.Channels[3] = SetFMFreq(helper.payload, 8);
                            appData.Fms.Channels[4] = SetFMFreq(helper.payload, 10);
                            appData.Fms.Channels[5] = SetFMFreq(helper.payload, 12);
                            appData.Fms.Channels[6] = SetFMFreq(helper.payload, 14);
                            appData.Fms.Channels[7] = SetFMFreq(helper.payload, 16);
                            appData.Fms.Channels[8] = SetFMFreq(helper.payload, 18);
                            appData.Fms.Channels[9] = SetFMFreq(helper.payload, 20);
                            appData.Fms.Channels[10] = SetFMFreq(helper.payload, 22);
                            appData.Fms.Channels[11] = SetFMFreq(helper.payload, 24);
                            appData.Fms.Channels[12] = SetFMFreq(helper.payload, 26);
                            appData.Fms.Channels[13] = SetFMFreq(helper.payload, 28);
                            appData.Fms.Channels[14] = SetFMFreq(helper.payload, 30);
                        }
                        else if (num >= 45056 && num < 45744)
                        {
                            switch (num)
                            {
                                case 45056:
                                    appData.Dtmfs.LocalID = SetDTMFWord(helper.payload, 0);
                                    appData.Dtmfs.Group[0] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45088:
                                    appData.Dtmfs.Group[1] = SetDTMFWord(helper.payload, 0);
                                    appData.Dtmfs.Group[2] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45120:
                                    appData.Dtmfs.Group[3] = SetDTMFWord(helper.payload, 0);
                                    appData.Dtmfs.Group[4] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45152:
                                    appData.Dtmfs.Group[5] = SetDTMFWord(helper.payload, 0);
                                    appData.Dtmfs.Group[6] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45184:
                                    appData.Dtmfs.Group[7] = SetDTMFWord(helper.payload, 0);
                                    appData.Dtmfs.Group[8] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45216:
                                    appData.Dtmfs.Group[9] = SetDTMFWord(helper.payload, 0);
                                    appData.Dtmfs.Group[10] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45248:
                                    appData.Dtmfs.Group[11] = SetDTMFWord(helper.payload, 0);
                                    appData.Dtmfs.Group[12] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45280:
                                    appData.Dtmfs.Group[13] = SetDTMFWord(helper.payload, 0);
                                    appData.Dtmfs.Group[14] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45312:
                                    appData.Dtmfs.Group[15] = SetDTMFWord(helper.payload, 0);
                                    appData.Dtmfs.Group[16] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45344:
                                    appData.Dtmfs.Group[17] = SetDTMFWord(helper.payload, 0);
                                    appData.Dtmfs.Group[18] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45376:
                                    appData.Dtmfs.Group[19] = SetDTMFWord(helper.payload, 0);
                                    appData.Dtmfs.GroupName[0] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45408:
                                    appData.Dtmfs.GroupName[1] = SetDTMFGroupName(helper.payload, 0);
                                    appData.Dtmfs.GroupName[2] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45440:
                                    appData.Dtmfs.GroupName[3] = SetDTMFGroupName(helper.payload, 0);
                                    appData.Dtmfs.GroupName[4] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45472:
                                    appData.Dtmfs.GroupName[5] = SetDTMFGroupName(helper.payload, 0);
                                    appData.Dtmfs.GroupName[6] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45504:
                                    appData.Dtmfs.GroupName[7] = SetDTMFGroupName(helper.payload, 0);
                                    appData.Dtmfs.GroupName[8] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45536:
                                    appData.Dtmfs.GroupName[9] = SetDTMFGroupName(helper.payload, 0);
                                    appData.Dtmfs.GroupName[10] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45568:
                                    appData.Dtmfs.GroupName[11] = SetDTMFGroupName(helper.payload, 0);
                                    appData.Dtmfs.GroupName[12] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45600:
                                    appData.Dtmfs.GroupName[13] = SetDTMFGroupName(helper.payload, 0);
                                    appData.Dtmfs.GroupName[14] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45632:
                                    appData.Dtmfs.GroupName[15] = SetDTMFGroupName(helper.payload, 0);
                                    appData.Dtmfs.GroupName[16] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45664:
                                    appData.Dtmfs.GroupName[17] = SetDTMFGroupName(helper.payload, 0);
                                    appData.Dtmfs.GroupName[18] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45696:
                                    appData.Dtmfs.GroupName[19] = SetDTMFGroupName(helper.payload, 0);
                                    appData.Dtmfs.WordTime = helper.payload[16];
                                    appData.Dtmfs.IdleTime = helper.payload[17];
                                    break;
                                case 45728:
                                    appData.Mdcs.CallID1 = SetCallID(helper.payload, 0);
                                    appData.Mdcs.Id = SetMDC1200ID(helper.payload, 16);
                                    break;
                            }
                        }

                        if (num < 45728)
                        {
                            num += 32;
                            timer.Start();
                            step = STEP.STEP_READ1;
                            break;
                        }

                        progressVal = 100;
                        progressCont = "完成";
                        UpdateProgressBar.IssueProgressValue(progressVal, progressCont);
                        byData = helper.LoadPackage(69, 0, null, 1);
                        hid.Send(byData);
                        flagTransmitting = false;
                        return true;
                }
            }
            else
            {
                if (timesOfRetry <= 0)
                {
                    timer.Stop();
                    flagTransmitting = false;
                    return false;
                }

                timesOfRetry--;
                flagRetry = false;
                hid.Send(byData);
            }

        return false;
    }

    private string CaculateFreq_HexToStr(byte[] dat, int offset)
    {
        var num = 0u;
        for (var i = 0; i < 4; i++)
            dat[i + offset] = (byte)(((dat[i + offset] >> 4) & 0xF) * 10 + (dat[i + offset] & 0xF));

        for (var num2 = 3; num2 >= 0; num2--) num = num * 100 + dat[num2 + offset];

        return num.ToString().Insert(3, ".");
    }

    private string CaculateCtsDcs(byte[] dat, int offset)
    {
        var text = "";
        try
        {
            if (dat[1 + offset] == 0)
            {
                if (dat[offset] != 0 && dat[offset] <= 210) return tblCTSDCS[dat[offset] - 1];

                return "OFF";
            }

            if (dat[offset] != 0 && dat[offset] != byte.MaxValue)
            {
                ushort num = dat[1 + offset];
                num <<= 8;
                text = ((ushort)(num + dat[offset])).ToString();
                return text.Insert(text.Length - 1, ".");
            }

            return "OFF";
        }
        catch
        {
            return "OFF";
        }
    }

    private void SetChannelInfos(int CH_Num, byte[] dat)
    {
        var num = CH_Num / 32;
        var num2 = CH_Num % 32;
        if (dat[0] == byte.MaxValue || dat[1] == byte.MaxValue || dat[3] == 0) return;

        appData.ChannelList[num][num2].RxFreq = CaculateFreq_HexToStr(dat, 0);
        if (dat[4] != byte.MaxValue && dat[5] != byte.MaxValue)
            appData.ChannelList[num][num2].TxFreq = CaculateFreq_HexToStr(dat, 4);

        appData.ChannelList[num][num2].StrRxCtsDcs = CaculateCtsDcs(dat, 8);
        appData.ChannelList[num][num2].StrTxCtsDcs = CaculateCtsDcs(dat, 10);
        appData.ChannelList[num][num2].SignalGroup = dat[12] % 20;
        appData.ChannelList[num][num2].Pttid = dat[13] % 4;
        appData.ChannelList[num][num2].TxPower = dat[14] % 2;
        appData.ChannelList[num][num2].Bandwide = (dat[15] >> 6) & 1;
        appData.ChannelList[num][num2].SqMode = ((dat[15] >> 4) & 3) % 3;
        appData.ChannelList[num][num2].ScanAdd = (dat[15] >> 2) & 1;
        appData.ChannelList[num][num2].SignalSystem = (dat[15] & 3) % 3;
        if (dat[20] != byte.MaxValue)
        {
            var num3 = 0;
            for (var i = 0; i < 12 && dat[20 + i] != byte.MaxValue; i++) num3++;

            appData.ChannelList[num][num2].Name = Encoding.GetEncoding("gb2312").GetString(dat, 20, num3);
        }
    }

    private string SetBankName(byte[] dat, int offset)
    {
        var result = "";
        var num = 0;
        if (dat[offset] != byte.MaxValue)
        {
            for (var i = 0; i < 12 && dat[i + offset] != byte.MaxValue; i++) num++;

            result = Encoding.GetEncoding("gb2312").GetString(dat, offset, num);
        }

        return result;
    }

    private string CaculateFreq(byte[] dat)
    {
        var text = "";
        var num = 0;
        try
        {
            if (dat[3] != byte.MaxValue)
            {
                num = dat[3];
                num <<= 8;
                num |= dat[2];
                num <<= 8;
                num |= dat[1];
                num <<= 8;
                return (num | dat[0]).ToString().Insert(3, ".");
            }
        }
        catch
        {
            return "";
        }

        return "";
    }

    private string CaculateOffset(byte[] dat, int offset)
    {
        var result = "00.0000";
        var num = 0;
        try
        {
            if (dat[offset] != byte.MaxValue)
            {
                result = "";
                num = dat[18];
                num <<= 8;
                num |= dat[17];
                num <<= 8;
                num |= dat[16];
                num <<= 8;
                num |= dat[15];
                result = (num / 10).ToString("D6").Insert(2, ".");
            }
        }
        catch
        {
            result = "00.0000";
        }

        return result;
    }

    private void SetVFOAInfos(byte[] dat)
    {
        var text = CaculateFreq(dat);
        if (text != "") appData.Vfos.VfoAFreq = text;

        appData.Vfos.StrVFOARxCtsDcs = CaculateCtsDcs(dat, 4);
        appData.Vfos.StrVFOATxCtsDcs = CaculateCtsDcs(dat, 6);
        appData.Vfos.VfoABusyLock = dat[8] % 2;
        appData.Vfos.VfoASignalGroup = dat[9] % 20;
        appData.Vfos.VfoATxPower = dat[10] % 3;
        appData.Vfos.VfoABandwide = dat[11] % 2;
        appData.Vfos.VfoAScram = dat[12] % 9;
        appData.Vfos.VfoAStep = dat[13] % 8;
        appData.Vfos.VfoADir = dat[14] % 3;
        appData.Vfos.VfoAOffset = CaculateOffset(dat, 15);
        appData.Vfos.VfoASQMode = dat[19] % 3;
        appData.Vfos.VfoASignalSystem = dat[26] % 3;
    }

    private void SetVFOBInfos(byte[] dat)
    {
        var text = CaculateFreq(dat);
        if (text != "") appData.Vfos.VfoBFreq = text;

        appData.Vfos.StrVFOBRxCtsDcs = CaculateCtsDcs(dat, 4);
        appData.Vfos.StrVFOBTxCtsDcs = CaculateCtsDcs(dat, 6);
        appData.Vfos.VfoBBusyLock = dat[8] % 2;
        appData.Vfos.VfoBSignalGroup = dat[9] % 20;
        appData.Vfos.VfoBTxPower = dat[10] % 3;
        appData.Vfos.VfoBBandwide = dat[11] % 2;
        appData.Vfos.VfoBScram = dat[12] % 9;
        appData.Vfos.VfoBStep = dat[13] % 8;
        appData.Vfos.VfoBDir = dat[14] % 3;
        appData.Vfos.VfoBOffset = CaculateOffset(dat, 15);
        appData.Vfos.VfoBSQMode = dat[19] % 3;
        appData.Vfos.VfoBSignalSystem = dat[26] % 3;
    }

    private int SetFMFreq(byte[] payload, int offset)
    {
        var num = 0;
        if (payload[offset] != byte.MaxValue && payload[offset + 1] != byte.MaxValue)
            num = payload[offset] + (payload[offset + 1] << 8);

        if (num < 650 || num > 1080) num = 0;

        return num;
    }

    private string SetDTMFWord(byte[] payload, int offset)
    {
        var text = "0123456789ABCD*#";
        var text2 = "";
        var num = 0;
        if (payload[offset] != byte.MaxValue)
        {
            for (var i = 0; i < 8 && payload[offset + i] != byte.MaxValue; i++) num++;

            for (var j = 0; j < num; j++) text2 += text[payload[offset + j] % 16];
        }

        return text2;
    }

    private string SetDTMFGroupName(byte[] payload, int offset)
    {
        var result = "";
        var num = 0;
        if (payload[offset] != byte.MaxValue)
        {
            for (var i = 0; i < 12 && payload[offset + i] != byte.MaxValue; i++) num++;

            result = Encoding.GetEncoding("gb2312").GetString(payload, offset, num);
        }

        return result;
    }

    private string SetCallID(byte[] payload, int offset)
    {
        var result = "";
        var num = 0;
        if (payload[offset] != byte.MaxValue)
        {
            for (var i = 0; i < 6 && payload[offset + i] != byte.MaxValue; i++) num++;

            result = Encoding.ASCII.GetString(payload, offset, num);
        }

        return result;
    }

    private string SetMDC1200ID(byte[] payload, int offset)
    {
        var text = "0123456789ABCDEF";
        var text2 = "";
        var num = 0;
        if (payload[offset] != byte.MaxValue)
        {
            for (var i = 0; i < 4 && payload[offset + i] != byte.MaxValue; i++) num++;

            text2 = "";
            try
            {
                for (var j = 0; j < 4; j++) text2 += text[payload[offset + j]];
            }
            catch
            {
                text2 = "1111";
            }
        }

        return text2;
    }
}