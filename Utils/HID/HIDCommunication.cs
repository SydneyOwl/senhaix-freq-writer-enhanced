using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Timers;
using SenhaixFreqWriter.Constants.Gt12;
using SenhaixFreqWriter.DataModels.Gt12;
using Timer = System.Timers.Timer;

namespace SenhaixFreqWriter.Utils.HID;

public class HIDCommunication
{
    private readonly DataHelper helper;

    private readonly HIDTools hid = HIDTools.getInstance();

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

    public AppData appData = AppData.getInstance();

    // private bool flagReceiveData;

    private bool flagRetry;

    private bool flagTransmitting;

    private string progressCont = "";

    private int progressVal;

    // private byte[] rxBuffer = new byte[64];

    private STEP step;

    private Timer timer;

    private byte timesOfRetry = 5;

    public ConcurrentQueue<ProgressBarValue> statusQueue = new();

    public HIDCommunication(OP_TYPE opType)
    {
        this.opType = opType;
        helper = new DataHelper();
        TimerInit();
    }

    public void UpdateProgressBar(int value, string content)
    {
        statusQueue.Enqueue(new ProgressBarValue(value, content));
    }

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
        HIDTools.getInstance().rxBuffer = e;
        HIDTools.getInstance().flagReceiveData = true;
    }

    private void resetRetryCount()
    {
        timesOfRetry = 5;
        flagRetry = false;
    }

    public bool DoIt(CancellationToken token)
    {
        flagTransmitting = true;
        resetRetryCount();
        step = STEP.STEP_HANDSHAKE_1;
        HIDTools.getInstance().flagReceiveData = false;
        if (HandShake(token))
        {
            if (opType == OP_TYPE.WRITE)
            {
                if (Write(token)) return true;
            }
            else if (Read(token))
            {
                return true;
            }
        }

        return false;
    }

    private bool HandShake(CancellationToken token)
    {
        var byData = new byte[1];
        while (flagTransmitting && !token.IsCancellationRequested)
            if (!flagRetry)
            { ;
                switch (step)
                {
                    case STEP.STEP_HANDSHAKE_1:
                        // Console.WriteLine("strp1");
                        byData = Encoding.ASCII.GetBytes("PROGRAMGT12");
                        byData = helper.LoadPackage(1, 0, byData, (byte)byData.Length);
                        hid.Send(byData);
                        progressVal = 0;
                        progressCont = "握手...";
                        UpdateProgressBar(progressVal, progressCont);
                        timer.Start();
                        resetRetryCount();
                        step = STEP.STEP_HANDSHAKE_2;
                        break;
                    case STEP.STEP_HANDSHAKE_2:
                        // Console.WriteLine("strp2");
                        if (HIDTools.getInstance().flagReceiveData)
                        {
                            HIDTools.getInstance().flagReceiveData = false;
                            helper.AnalyzePackage(HIDTools.getInstance().rxBuffer);
                            if (helper.errorCode == HID_ERRORS.ER_NONE)
                            {
                                byData = helper.LoadPackage(2, 0, null, 1);
                                hid.Send(byData);
                                resetRetryCount();
                                step = STEP.STEP_HANDSHAKE_3;
                            }
                        }

                        break;
                    case STEP.STEP_HANDSHAKE_3:
                        // Console.WriteLine("strp3");
                        if (HIDTools.getInstance().flagReceiveData)
                        {
                            HIDTools.getInstance().flagReceiveData = false;
                            helper.AnalyzePackage(HIDTools.getInstance().rxBuffer);
                            if (helper.errorCode == HID_ERRORS.ER_NONE)
                            {
                                byData = helper.LoadPackage(70, 0, null, 1);
                                hid.Send(byData);
                                resetRetryCount();
                                step = STEP.STEP_HANDSHAKE_4;
                            }
                        }

                        break;
                    case STEP.STEP_HANDSHAKE_4:
                        // Console.WriteLine("strp4");
                        if (!HIDTools.getInstance().flagReceiveData) break;

                        HIDTools.getInstance().flagReceiveData = false;
                        helper.AnalyzePackage(HIDTools.getInstance().rxBuffer);
                        if (helper.errorCode == HID_ERRORS.ER_NONE)
                        {
                            timer.Stop();
                            resetRetryCount();
                            progressVal = 0;
                            progressCont = "进度..." + progressVal + "%";
                            UpdateProgressBar(progressVal, progressCont);
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

    private bool Write(CancellationToken token)
    {
        var byData = new byte[10];
        var array = new byte[48];
        ushort num = 0;
        byte b = 0;
        var num2 = 0;
        
        // Console.WriteLine("we're in writing");
        while (flagTransmitting && !token.IsCancellationRequested)
        {
            // Console.WriteLine("wE;RE WRITEING");
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
                            GetbankName(array, 0, appData.bankName[num3]);
                            GetbankName(array, 16, appData.bankName[num3 + 1]);
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
                            array[0] = (byte)appData.funCfgs.Sql;
                            array[1] = (byte)appData.funCfgs.SaveMode;
                            array[2] = (byte)appData.funCfgs.Vox;
                            array[3] = (byte)appData.funCfgs.VoxDlyTime;
                            array[4] = (byte)appData.funCfgs.DualStandby;
                            array[5] = (byte)appData.funCfgs.Tot;
                            array[6] = (byte)appData.funCfgs.Beep;
                            array[7] = (byte)appData.funCfgs.SideTone;
                            array[8] = (byte)appData.funCfgs.ScanMode;
                            array[9] = (byte)appData.vfos.Pttid;
                            array[10] = (byte)appData.funCfgs.PttDly;
                            array[11] = (byte)appData.funCfgs.ChADisType;
                            array[12] = (byte)appData.funCfgs.ChBDisType;
                            array[13] = (byte)appData.funCfgs.CbBMisscall;
                            array[14] = (byte)appData.funCfgs.AutoLock;
                            array[15] = (byte)appData.funCfgs.MicGain;
                            array[16] = (byte)appData.funCfgs.AlarmMode;
                            array[17] = (byte)appData.funCfgs.TailClear;
                            array[18] = (byte)appData.funCfgs.RptTailClear;
                            array[19] = (byte)appData.funCfgs.RptTailDet;
                            array[20] = (byte)appData.funCfgs.Roger;
                            array[21] = 0;
                            array[22] = (byte)appData.funCfgs.FmEnable;
                            array[23] = 0;
                            array[23] |= (byte)appData.funCfgs.ChAWorkmode;
                            array[23] |= (byte)(appData.funCfgs.ChBWorkmode << 4);
                            array[24] = (byte)appData.funCfgs.KeyLock;
                            array[25] = (byte)appData.funCfgs.AutoPowerOff;
                            array[26] = (byte)appData.funCfgs.PowerOnDisType;
                            array[27] = 0;
                            array[28] = (byte)appData.funCfgs.Tone;
                            array[29] = (byte)appData.funCfgs.CurBank;
                            array[30] = (byte)appData.funCfgs.Backlight;
                            array[31] = (byte)appData.funCfgs.MenuQuitTime;
                        }
                        else if (num == 36896)
                        {
                            array[0] = (byte)appData.funCfgs.Key1Short;
                            array[1] = (byte)appData.funCfgs.Key1Long;
                            array[2] = (byte)appData.funCfgs.Key2Short;
                            array[3] = (byte)appData.funCfgs.Key2Long;
                            array[4] = (byte)appData.funCfgs.Bright;
                            array[5] = 0;
                            array[6] = 0;
                            array[7] = (byte)appData.funCfgs.VoxSw;
                            array[8] = (byte)appData.funCfgs.PowerUpDisTime;
                            array[9] = (byte)appData.funCfgs.BluetoothAudioGain;
                            if (appData.funCfgs.CallSign != null && appData.funCfgs.CallSign != "")
                            {
                                var bytes = Encoding.GetEncoding("gb2312").GetBytes(appData.funCfgs.CallSign);
                                Debug.WriteLine($"Callsign: {appData.funCfgs.CallSign}");
                                var array2 = bytes;
                                foreach (var b2 in array2) Debug.WriteLine($"Byte: {b2}");

                                Array.Copy(bytes, 0, array, 10, bytes.Length);
                            }
                        }
                        else if (num == 40960)
                        {
                            GetFMFreq(array, 0, appData.fms.CurFreq);
                            GetFMFreq(array, 2, appData.fms.Channels[0]);
                            GetFMFreq(array, 4, appData.fms.Channels[1]);
                            GetFMFreq(array, 6, appData.fms.Channels[2]);
                            GetFMFreq(array, 8, appData.fms.Channels[3]);
                            GetFMFreq(array, 10, appData.fms.Channels[4]);
                            GetFMFreq(array, 12, appData.fms.Channels[5]);
                            GetFMFreq(array, 14, appData.fms.Channels[6]);
                            GetFMFreq(array, 16, appData.fms.Channels[7]);
                            GetFMFreq(array, 18, appData.fms.Channels[8]);
                            GetFMFreq(array, 20, appData.fms.Channels[9]);
                            GetFMFreq(array, 22, appData.fms.Channels[10]);
                            GetFMFreq(array, 24, appData.fms.Channels[11]);
                            GetFMFreq(array, 26, appData.fms.Channels[12]);
                            GetFMFreq(array, 28, appData.fms.Channels[13]);
                            GetFMFreq(array, 30, appData.fms.Channels[14]);
                        }
                        else if (num >= 45056 && num < 45744)
                        {
                            switch (num)
                            {
                                case 45056:
                                    GetDTMFWord(array, 0, appData.dtmfs.LocalID);
                                    GetDTMFWord(array, 16, appData.dtmfs.Group[0]);
                                    break;
                                case 45088:
                                    GetDTMFWord(array, 0, appData.dtmfs.Group[1]);
                                    GetDTMFWord(array, 16, appData.dtmfs.Group[2]);
                                    break;
                                case 45120:
                                    GetDTMFWord(array, 0, appData.dtmfs.Group[3]);
                                    GetDTMFWord(array, 16, appData.dtmfs.Group[4]);
                                    break;
                                case 45152:
                                    GetDTMFWord(array, 0, appData.dtmfs.Group[5]);
                                    GetDTMFWord(array, 16, appData.dtmfs.Group[6]);
                                    break;
                                case 45184:
                                    GetDTMFWord(array, 0, appData.dtmfs.Group[7]);
                                    GetDTMFWord(array, 16, appData.dtmfs.Group[8]);
                                    break;
                                case 45216:
                                    GetDTMFWord(array, 0, appData.dtmfs.Group[9]);
                                    GetDTMFWord(array, 16, appData.dtmfs.Group[10]);
                                    break;
                                case 45248:
                                    GetDTMFWord(array, 0, appData.dtmfs.Group[11]);
                                    GetDTMFWord(array, 16, appData.dtmfs.Group[12]);
                                    break;
                                case 45280:
                                    GetDTMFWord(array, 0, appData.dtmfs.Group[13]);
                                    GetDTMFWord(array, 16, appData.dtmfs.Group[14]);
                                    break;
                                case 45312:
                                    GetDTMFWord(array, 0, appData.dtmfs.Group[15]);
                                    GetDTMFWord(array, 16, appData.dtmfs.Group[16]);
                                    break;
                                case 45344:
                                    GetDTMFWord(array, 0, appData.dtmfs.Group[17]);
                                    GetDTMFWord(array, 16, appData.dtmfs.Group[18]);
                                    break;
                                case 45376:
                                    GetDTMFWord(array, 0, appData.dtmfs.Group[19]);
                                    GetDTMFName(array, 16, appData.dtmfs.GroupName[0]);
                                    break;
                                case 45408:
                                    GetDTMFName(array, 0, appData.dtmfs.GroupName[1]);
                                    GetDTMFName(array, 16, appData.dtmfs.GroupName[2]);
                                    break;
                                case 45440:
                                    GetDTMFName(array, 0, appData.dtmfs.GroupName[3]);
                                    GetDTMFName(array, 16, appData.dtmfs.GroupName[4]);
                                    break;
                                case 45472:
                                    GetDTMFName(array, 0, appData.dtmfs.GroupName[5]);
                                    GetDTMFName(array, 16, appData.dtmfs.GroupName[6]);
                                    break;
                                case 45504:
                                    GetDTMFName(array, 0, appData.dtmfs.GroupName[7]);
                                    GetDTMFName(array, 16, appData.dtmfs.GroupName[8]);
                                    break;
                                case 45536:
                                    GetDTMFName(array, 0, appData.dtmfs.GroupName[9]);
                                    GetDTMFName(array, 16, appData.dtmfs.GroupName[10]);
                                    break;
                                case 45568:
                                    GetDTMFName(array, 0, appData.dtmfs.GroupName[11]);
                                    GetDTMFName(array, 16, appData.dtmfs.GroupName[12]);
                                    break;
                                case 45600:
                                    GetDTMFName(array, 0, appData.dtmfs.GroupName[13]);
                                    GetDTMFName(array, 16, appData.dtmfs.GroupName[14]);
                                    break;
                                case 45632:
                                    GetDTMFName(array, 0, appData.dtmfs.GroupName[15]);
                                    GetDTMFName(array, 16, appData.dtmfs.GroupName[16]);
                                    break;
                                case 45664:
                                    GetDTMFName(array, 0, appData.dtmfs.GroupName[17]);
                                    GetDTMFName(array, 16, appData.dtmfs.GroupName[18]);
                                    break;
                                case 45696:
                                    GetDTMFName(array, 0, appData.dtmfs.GroupName[19]);
                                    array[16] = (byte)appData.dtmfs.WordTime;
                                    array[17] = (byte)appData.dtmfs.IdleTime;
                                    break;
                                case 45728:
                                    GetCallIDWord(array, 0, appData.mdcs.CallID);
                                    GetMDC1200_IDWord(array, 16, appData.mdcs.Id);
                                    break;
                            }
                        }

                        byData = helper.LoadPackage(87, num, array, (byte)array.Length);
                        hid.Send(byData);
                        timer.Start();
                        progressVal = num * 100 / 45728;
                        if (progressVal > 100) progressVal = 100;

                        progressCont = "进度..." + progressVal + "%";
                        UpdateProgressBar(progressVal, progressCont);
                        step = STEP.STEP_WRITE2;
                        break;
                    case STEP.STEP_WRITE2:
                        if (!HIDTools.getInstance().flagReceiveData) break;

                        HIDTools.getInstance().flagReceiveData = false;
                        helper.AnalyzePackage(HIDTools.getInstance().rxBuffer);
                        if (helper.errorCode == HID_ERRORS.ER_NONE)
                        {
                            timer.Stop();
                            resetRetryCount();
                            for (var i = 0; i < 32; i++) array[i] = byte.MaxValue;

                            if (num >= 45728)
                            {
                                progressVal = 100;
                                progressCont = "完成";
                                UpdateProgressBar(progressVal, progressCont);
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
        if (appData.channelList[num][num2].RxFreq != "")
        {
            var sourceArray = CaculateFreq_StrToHex(appData.channelList[num][num2].RxFreq);
            Array.Copy(sourceArray, 0, array, 0, 4);
            if (appData.channelList[num][num2].TxFreq != "")
            {
                var sourceArray2 = CaculateFreq_StrToHex(appData.channelList[num][num2].TxFreq);
                Array.Copy(sourceArray2, 0, array, 4, 4);
            }

            var sourceArray3 = CaculateCtsDcs(appData.channelList[num][num2].StrRxCtsDcs);
            Array.Copy(sourceArray3, 0, array, 8, 2);
            sourceArray3 = CaculateCtsDcs(appData.channelList[num][num2].StrTxCtsDcs);
            Array.Copy(sourceArray3, 0, array, 10, 2);
            array[12] = (byte)appData.channelList[num][num2].SignalGroup;
            array[13] = (byte)appData.channelList[num][num2].Pttid;
            array[14] = (byte)appData.channelList[num][num2].TxPower;
            array[15] = 0;
            array[15] |= (byte)(appData.channelList[num][num2].Bandwide << 6);
            array[15] |= (byte)(appData.channelList[num][num2].SqMode << 4);
            array[15] |= (byte)(appData.channelList[num][num2].ScanAdd << 2);
            array[15] |= (byte)(appData.channelList[num][num2].SignalSystem & 3);
            if (appData.channelList[num][num2].Name != "")
            {
                var num3 = 0;
                var bytes = Encoding.GetEncoding("gb2312").GetBytes(appData.channelList[num][num2].Name);
                num3 = bytes.Length > 12 ? 12 : bytes.Length;
                Array.Copy(bytes, 0, array, 20, num3);
            }
        }

        return array;
    }

    private void GetbankName(byte[] payload, int offset, string name)
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
        var s = appData.vfos.VfoAFreq.Remove(3, 1);
        var num = int.Parse(s);
        array[0] = (byte)((uint)num & 0xFFu);
        array[1] = (byte)((uint)(num >> 8) & 0xFFu);
        array[2] = (byte)((uint)(num >> 16) & 0xFFu);
        array[3] = (byte)((uint)(num >> 24) & 0xFFu);
        var sourceArray = CaculateCtsDcs(appData.vfos.StrVFOARxCtsDcs);
        Array.Copy(sourceArray, 0, array, 4, 2);
        sourceArray = CaculateCtsDcs(appData.vfos.StrVFOATxCtsDcs);
        Array.Copy(sourceArray, 0, array, 6, 2);
        array[8] = (byte)appData.vfos.VfoABusyLock;
        array[9] = (byte)appData.vfos.VfoASignalGroup;
        array[10] = (byte)appData.vfos.VfoATxPower;
        array[11] = (byte)appData.vfos.VfoABandwide;
        array[12] = (byte)appData.vfos.VfoAScram;
        array[13] = (byte)appData.vfos.VfoAStep;
        array[14] = (byte)appData.vfos.VfoADir;
        var s2 = appData.vfos.VfoAOffset.Remove(2, 1);
        var num2 = int.Parse(s2) * 10;
        array[15] = (byte)((uint)num2 & 0xFFu);
        array[16] = (byte)((uint)(num2 >> 8) & 0xFFu);
        array[17] = (byte)((uint)(num2 >> 16) & 0xFFu);
        array[18] = (byte)((uint)(num2 >> 24) & 0xFFu);
        array[19] = (byte)appData.vfos.VfoASQMode;
        array[26] = (byte)appData.vfos.VfoASignalSystem;
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
        var s = appData.vfos.VfoBFreq.Remove(3, 1);
        var num = int.Parse(s);
        array[0] = (byte)((uint)num & 0xFFu);
        array[1] = (byte)((uint)(num >> 8) & 0xFFu);
        array[2] = (byte)((uint)(num >> 16) & 0xFFu);
        array[3] = (byte)((uint)(num >> 24) & 0xFFu);
        var sourceArray = CaculateCtsDcs(appData.vfos.StrVFOBRxCtsDcs);
        Array.Copy(sourceArray, 0, array, 4, 2);
        sourceArray = CaculateCtsDcs(appData.vfos.StrVFOBTxCtsDcs);
        Array.Copy(sourceArray, 0, array, 6, 2);
        array[8] = (byte)appData.vfos.VfoBBusyLock;
        array[9] = (byte)appData.vfos.VfoBSignalGroup;
        array[10] = (byte)appData.vfos.VfoBTxPower;
        array[11] = (byte)appData.vfos.VfoBBandwide;
        array[12] = (byte)appData.vfos.VfoBScram;
        array[13] = (byte)appData.vfos.VfoBStep;
        array[14] = (byte)appData.vfos.VfoBDir;
        var s2 = appData.vfos.VfoBOffset.Remove(2, 1);
        var num2 = int.Parse(s2) * 10;
        array[15] = (byte)((uint)num2 & 0xFFu);
        array[16] = (byte)((uint)(num2 >> 8) & 0xFFu);
        array[17] = (byte)((uint)(num2 >> 16) & 0xFFu);
        array[18] = (byte)((uint)(num2 >> 24) & 0xFFu);
        array[19] = (byte)appData.vfos.VfoBSQMode;
        array[26] = (byte)appData.vfos.VfoBSignalSystem;
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

    private bool Read(CancellationToken token)
    {
        var byData = new byte[10];
        ushort num = 0;
        var num2 = 0;
        while (flagTransmitting && !token.IsCancellationRequested)
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
                        UpdateProgressBar(progressVal, progressCont);
                        resetRetryCount();
                        timer.Start();
                        step = STEP.STEP_READ2;
                        break;
                    case STEP.STEP_READ2:
                        if (!HIDTools.getInstance().flagReceiveData) break;

                        HIDTools.getInstance().flagReceiveData = false;
                        timer.Stop();
                        resetRetryCount();
                        helper.AnalyzePackage(HIDTools.getInstance().rxBuffer);
                        if (helper.errorCode != HID_ERRORS.ER_NONE) break;

                        if (num < 30720)
                        {
                            SetChannelInfos(num2++, helper.payload);
                        }
                        else if (num >= 32000 && num < 32480)
                        {
                            var num3 = (num - 32000) / 16;
                            appData.bankName[num3] = SetbankName(helper.payload, 0);
                            appData.bankName[num3 + 1] = SetbankName(helper.payload, 16);
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
                            appData.funCfgs.Sql = helper.payload[0] % 10;
                            appData.funCfgs.SaveMode = helper.payload[1] % 2;
                            appData.funCfgs.Vox = helper.payload[2] % 3;
                            appData.funCfgs.VoxDlyTime = helper.payload[3] % 16;
                            appData.funCfgs.DualStandby = helper.payload[4] % 2;
                            appData.funCfgs.Tot = helper.payload[5] % 9;
                            appData.funCfgs.Beep = helper.payload[6] % 4;
                            appData.funCfgs.SideTone = helper.payload[7] % 2;
                            appData.funCfgs.ScanMode = helper.payload[8] % 3;
                            appData.vfos.Pttid = helper.payload[9] % 4;
                            appData.funCfgs.PttDly = helper.payload[10] % 16;
                            appData.funCfgs.ChADisType = helper.payload[11] % 3;
                            appData.funCfgs.ChBDisType = helper.payload[12] % 3;
                            appData.funCfgs.CbBMisscall = helper.payload[13] % 2;
                            appData.funCfgs.AutoLock = helper.payload[14] % 7;
                            appData.funCfgs.MicGain = helper.payload[15] % 3;
                            appData.funCfgs.AlarmMode = helper.payload[16] % 3;
                            appData.funCfgs.TailClear = helper.payload[17] % 2;
                            appData.funCfgs.RptTailClear = helper.payload[18] % 11;
                            appData.funCfgs.RptTailDet = helper.payload[19] % 11;
                            appData.funCfgs.Roger = helper.payload[20] % 2;
                            appData.funCfgs.FmEnable = helper.payload[22] % 2;
                            appData.funCfgs.ChAWorkmode = (helper.payload[23] & 0xF) % 2;
                            appData.funCfgs.ChBWorkmode = ((helper.payload[23] & 0xF0) >> 4) % 2;
                            appData.funCfgs.KeyLock = helper.payload[24] % 2;
                            appData.funCfgs.AutoPowerOff = helper.payload[25] % 5;
                            appData.funCfgs.PowerOnDisType = helper.payload[26] % 3;
                            appData.funCfgs.Tone = helper.payload[28] % 4;
                            appData.funCfgs.CurBank = helper.payload[29] % 30;
                            appData.funCfgs.Backlight = helper.payload[30] % 9;
                            appData.funCfgs.MenuQuitTime = helper.payload[31] % 11;
                        }
                        else if (num == 36896)
                        {
                            appData.funCfgs.Key1Short = helper.payload[0] % 10;
                            appData.funCfgs.Key1Long = helper.payload[1] % 10;
                            appData.funCfgs.Key2Short = helper.payload[2] % 10;
                            appData.funCfgs.Key2Long = helper.payload[3] % 10;
                            appData.funCfgs.Bright = helper.payload[4] % 2;
                            appData.funCfgs.VoxSw = helper.payload[7] % 2;
                            appData.funCfgs.PowerUpDisTime = helper.payload[8] % 15;
                            appData.funCfgs.BluetoothAudioGain = helper.payload[9] % 5;
                            var num4 = 0;
                            for (var i = 0; i < 6 && helper.payload[10 + i] != byte.MaxValue; i++)
                            {
                                num4++;
                                Debug.WriteLine("Byte:{0}", helper.payload[10 + i]);
                            }

                            appData.funCfgs.CallSign =
                                Encoding.GetEncoding("gb2312").GetString(helper.payload, 10, num4);
                        }
                        else if (num == 40960)
                        {
                            appData.fms.CurFreq = SetFMFreq(helper.payload, 0);
                            appData.fms.Channels[0] = SetFMFreq(helper.payload, 2);
                            appData.fms.Channels[1] = SetFMFreq(helper.payload, 4);
                            appData.fms.Channels[2] = SetFMFreq(helper.payload, 6);
                            appData.fms.Channels[3] = SetFMFreq(helper.payload, 8);
                            appData.fms.Channels[4] = SetFMFreq(helper.payload, 10);
                            appData.fms.Channels[5] = SetFMFreq(helper.payload, 12);
                            appData.fms.Channels[6] = SetFMFreq(helper.payload, 14);
                            appData.fms.Channels[7] = SetFMFreq(helper.payload, 16);
                            appData.fms.Channels[8] = SetFMFreq(helper.payload, 18);
                            appData.fms.Channels[9] = SetFMFreq(helper.payload, 20);
                            appData.fms.Channels[10] = SetFMFreq(helper.payload, 22);
                            appData.fms.Channels[11] = SetFMFreq(helper.payload, 24);
                            appData.fms.Channels[12] = SetFMFreq(helper.payload, 26);
                            appData.fms.Channels[13] = SetFMFreq(helper.payload, 28);
                            appData.fms.Channels[14] = SetFMFreq(helper.payload, 30);
                        }
                        else if (num >= 45056 && num < 45744)
                        {
                            switch (num)
                            {
                                case 45056:
                                    appData.dtmfs.LocalID = SetDTMFWord(helper.payload, 0);
                                    appData.dtmfs.Group[0] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45088:
                                    appData.dtmfs.Group[1] = SetDTMFWord(helper.payload, 0);
                                    appData.dtmfs.Group[2] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45120:
                                    appData.dtmfs.Group[3] = SetDTMFWord(helper.payload, 0);
                                    appData.dtmfs.Group[4] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45152:
                                    appData.dtmfs.Group[5] = SetDTMFWord(helper.payload, 0);
                                    appData.dtmfs.Group[6] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45184:
                                    appData.dtmfs.Group[7] = SetDTMFWord(helper.payload, 0);
                                    appData.dtmfs.Group[8] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45216:
                                    appData.dtmfs.Group[9] = SetDTMFWord(helper.payload, 0);
                                    appData.dtmfs.Group[10] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45248:
                                    appData.dtmfs.Group[11] = SetDTMFWord(helper.payload, 0);
                                    appData.dtmfs.Group[12] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45280:
                                    appData.dtmfs.Group[13] = SetDTMFWord(helper.payload, 0);
                                    appData.dtmfs.Group[14] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45312:
                                    appData.dtmfs.Group[15] = SetDTMFWord(helper.payload, 0);
                                    appData.dtmfs.Group[16] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45344:
                                    appData.dtmfs.Group[17] = SetDTMFWord(helper.payload, 0);
                                    appData.dtmfs.Group[18] = SetDTMFWord(helper.payload, 16);
                                    break;
                                case 45376:
                                    appData.dtmfs.Group[19] = SetDTMFWord(helper.payload, 0);
                                    appData.dtmfs.GroupName[0] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45408:
                                    appData.dtmfs.GroupName[1] = SetDTMFGroupName(helper.payload, 0);
                                    appData.dtmfs.GroupName[2] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45440:
                                    appData.dtmfs.GroupName[3] = SetDTMFGroupName(helper.payload, 0);
                                    appData.dtmfs.GroupName[4] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45472:
                                    appData.dtmfs.GroupName[5] = SetDTMFGroupName(helper.payload, 0);
                                    appData.dtmfs.GroupName[6] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45504:
                                    appData.dtmfs.GroupName[7] = SetDTMFGroupName(helper.payload, 0);
                                    appData.dtmfs.GroupName[8] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45536:
                                    appData.dtmfs.GroupName[9] = SetDTMFGroupName(helper.payload, 0);
                                    appData.dtmfs.GroupName[10] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45568:
                                    appData.dtmfs.GroupName[11] = SetDTMFGroupName(helper.payload, 0);
                                    appData.dtmfs.GroupName[12] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45600:
                                    appData.dtmfs.GroupName[13] = SetDTMFGroupName(helper.payload, 0);
                                    appData.dtmfs.GroupName[14] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45632:
                                    appData.dtmfs.GroupName[15] = SetDTMFGroupName(helper.payload, 0);
                                    appData.dtmfs.GroupName[16] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45664:
                                    appData.dtmfs.GroupName[17] = SetDTMFGroupName(helper.payload, 0);
                                    appData.dtmfs.GroupName[18] = SetDTMFGroupName(helper.payload, 16);
                                    break;
                                case 45696:
                                    appData.dtmfs.GroupName[19] = SetDTMFGroupName(helper.payload, 0);
                                    appData.dtmfs.WordTime = helper.payload[16];
                                    appData.dtmfs.IdleTime = helper.payload[17];
                                    break;
                                case 45728:
                                    appData.mdcs.CallID = SetCallID(helper.payload, 0);
                                    appData.mdcs.Id = SetMDC1200ID(helper.payload, 16);
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
                        UpdateProgressBar(progressVal, progressCont);
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

        appData.channelList[num][num2].RxFreq = CaculateFreq_HexToStr(dat, 0);
        if (!string.IsNullOrEmpty(appData.channelList[num][num2].RxFreq))
        {
            appData.channelList[num][num2].IsVisable = true;
        }
        if (dat[4] != byte.MaxValue && dat[5] != byte.MaxValue)
            appData.channelList[num][num2].TxFreq = CaculateFreq_HexToStr(dat, 4);

        appData.channelList[num][num2].StrRxCtsDcs = CaculateCtsDcs(dat, 8);
        appData.channelList[num][num2].StrTxCtsDcs = CaculateCtsDcs(dat, 10);
        appData.channelList[num][num2].SignalGroup = dat[12] % 20;
        appData.channelList[num][num2].Pttid = dat[13] % 4;
        appData.channelList[num][num2].TxPower = dat[14] % 2;
        appData.channelList[num][num2].Bandwide = (dat[15] >> 6) & 1;
        appData.channelList[num][num2].SqMode = ((dat[15] >> 4) & 3) % 3;
        appData.channelList[num][num2].ScanAdd = (dat[15] >> 2) & 1;
        appData.channelList[num][num2].SignalSystem = (dat[15] & 3) % 3;
        if (dat[20] != byte.MaxValue)
        {
            var num3 = 0;
            for (var i = 0; i < 12 && dat[20 + i] != byte.MaxValue; i++) num3++;

            appData.channelList[num][num2].Name = Encoding.GetEncoding("gb2312").GetString(dat, 20, num3);
        }
    }

    private string SetbankName(byte[] dat, int offset)
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
        if (text != "") appData.vfos.VfoAFreq = text;

        appData.vfos.StrVFOARxCtsDcs = CaculateCtsDcs(dat, 4);
        appData.vfos.StrVFOATxCtsDcs = CaculateCtsDcs(dat, 6);
        appData.vfos.VfoABusyLock = dat[8] % 2;
        appData.vfos.VfoASignalGroup = dat[9] % 20;
        appData.vfos.VfoATxPower = dat[10] % 3;
        appData.vfos.VfoABandwide = dat[11] % 2;
        appData.vfos.VfoAScram = dat[12] % 9;
        appData.vfos.VfoAStep = dat[13] % 8;
        appData.vfos.VfoADir = dat[14] % 3;
        appData.vfos.VfoAOffset = CaculateOffset(dat, 15);
        appData.vfos.VfoASQMode = dat[19] % 3;
        appData.vfos.VfoASignalSystem = dat[26] % 3;
    }

    private void SetVFOBInfos(byte[] dat)
    {
        var text = CaculateFreq(dat);
        if (text != "") appData.vfos.VfoBFreq = text;

        appData.vfos.StrVFOBRxCtsDcs = CaculateCtsDcs(dat, 4);
        appData.vfos.StrVFOBTxCtsDcs = CaculateCtsDcs(dat, 6);
        appData.vfos.VfoBBusyLock = dat[8] % 2;
        appData.vfos.VfoBSignalGroup = dat[9] % 20;
        appData.vfos.VfoBTxPower = dat[10] % 3;
        appData.vfos.VfoBBandwide = dat[11] % 2;
        appData.vfos.VfoBScram = dat[12] % 9;
        appData.vfos.VfoBStep = dat[13] % 8;
        appData.vfos.VfoBDir = dat[14] % 3;
        appData.vfos.VfoBOffset = CaculateOffset(dat, 15);
        appData.vfos.VfoBSQMode = dat[19] % 3;
        appData.vfos.VfoBSignalSystem = dat[26] % 3;
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