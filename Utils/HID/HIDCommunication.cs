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

public class HidCommunication
{
    private readonly DataHelper _helper;

    private readonly OpType _opType;

    private readonly string[] _tblCtsdcs = new string[210]
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

    // private bool flagReceiveData;

    private bool _flagRetry;

    private bool _flagTransmitting;

    private readonly HidTools _hid = HidTools.GetInstance();

    private string _progressCont = "";

    private int _progressVal;

    // private byte[] rxBuffer = new byte[64];

    private Step _step;

    private Timer _timer;

    private byte _timesOfRetry = 5;

    public AppData AppData = AppData.GetInstance();

    public ConcurrentQueue<ProgressBarValue> StatusQueue = new();

    public HidCommunication(OpType opType)
    {
        _opType = opType;
        _helper = new DataHelper();
        TimerInit();
    }

    public void UpdateProgressBar(int value, string content)
    {
        StatusQueue.Enqueue(new ProgressBarValue(value, content));
    }

    private void TimerInit()
    {
        _timer = new Timer();
        _timer.Interval = 1000.0;
        _timer.Elapsed += Timer_Elapsed;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _flagRetry = true;
    }

    public void DataReceived(object sender, byte[] e)
    {
        _hid.RxBuffer = e;
        _hid.FlagReceiveData = true;
    }

    private void ResetRetryCount()
    {
        _timesOfRetry = 5;
        _flagRetry = false;
    }

    public bool DoIt(CancellationToken token)
    {
// #if !WINDOWS
//         hid.requestReconnect = true;
// #endif
        _flagTransmitting = true;
        ResetRetryCount();
        _step = Step.StepHandshake1;
        _hid.FlagReceiveData = false;
        if (HandShake(token))
        {
            if (_opType == OpType.Write)
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
        while (_flagTransmitting && !token.IsCancellationRequested)
            if (!_flagRetry)
            {
                switch (_step)
                {
                    case Step.StepHandshake1:
                        // DebugWindow.GetInstance().updateDebugContent("strp1");
                        byData = Encoding.ASCII.GetBytes("PROGRAMGT12");
                        byData = _helper.LoadPackage(1, 0, byData, (byte)byData.Length);
                        _hid.Send(byData);
                        _progressVal = 0;
                        _progressCont = "【如果卡住请插拔写频线或重启设备后重试！】握手...";
                        UpdateProgressBar(_progressVal, _progressCont);
                        _timer.Start();
                        ResetRetryCount();
                        _step = Step.StepHandshake2;
                        break;
                    case Step.StepHandshake2:
                        // DebugWindow.GetInstance().updateDebugContent("strp2");
                        if (_hid.FlagReceiveData)
                        {
                            _hid.FlagReceiveData = false;
                            _helper.AnalyzePackage(_hid.RxBuffer);
                            if (_helper.ErrorCode == HidErrors.ErNone)
                            {
                                byData = _helper.LoadPackage(2, 0, null, 1);
                                _hid.Send(byData);
                                ResetRetryCount();
                                _step = Step.StepHandshake3;
                            }
                        }

                        break;
                    case Step.StepHandshake3:
                        // DebugWindow.GetInstance().updateDebugContent("strp3");
                        if (_hid.FlagReceiveData)
                        {
                            _hid.FlagReceiveData = false;
                            _helper.AnalyzePackage(_hid.RxBuffer);
                            if (_helper.ErrorCode == HidErrors.ErNone)
                            {
                                byData = _helper.LoadPackage(70, 0, null, 1);
                                _hid.Send(byData);
                                ResetRetryCount();
                                _step = Step.StepHandshake4;
                            }
                        }

                        break;
                    case Step.StepHandshake4:
                        // DebugWindow.GetInstance().updateDebugContent("strp4");
                        if (!_hid.FlagReceiveData) break;

                        _hid.FlagReceiveData = false;
                        _helper.AnalyzePackage(_hid.RxBuffer);
                        if (_helper.ErrorCode == HidErrors.ErNone)
                        {
                            _timer.Stop();
                            ResetRetryCount();
                            _progressVal = 0;
                            _progressCont = "进度..." + _progressVal + "%";
                            UpdateProgressBar(_progressVal, _progressCont);
                            if (_opType == OpType.Read)
                            {
                                _step = Step.StepRead1;
                                return true;
                            }

                            _step = Step.StepWrite1;
                            return true;
                        }

                        break;
                }
            }
            else
            {
                if (_timesOfRetry <= 0)
                {
                    _timer.Stop();
                    _flagTransmitting = false;
                    return false;
                }

                _timesOfRetry--;
                _flagRetry = false;
                _hid.Send(byData);
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
        
        // 中转模式误触发保护功能：强制覆盖VFO设定，设定为航空频段防止误触发中转发射
        // deprecated!!
        // AppData.Vfos.VfoAFreq = "120.00000";
        // AppData.Vfos.VfoBFreq = "120.00000";
        // AppData.FunCfgs.ChAWorkmode = 0;
        // AppData.FunCfgs.ChBWorkmode = 0;

        // DebugWindow.GetInstance().updateDebugContent("we're in writing");
        while (_flagTransmitting && !token.IsCancellationRequested)
            // DebugWindow.GetInstance().updateDebugContent("wE;RE WRITEING");
            if (!_flagRetry)
            {
                switch (_step)
                {
                    case Step.StepWrite1:
                        if (num < 30720)
                        {
                            var channelInfos = GetChannelInfos(num2++);
                            Array.Copy(channelInfos, 0, array, 0, 32);
                        }
                        else if (num >= 32000 && num < 32480)
                        {
                            var num3 = (num - 32000) / 16;
                            GetbankName(array, 0, AppData.BankName[num3]);
                            GetbankName(array, 16, AppData.BankName[num3 + 1]);
                        }
                        else if (num == 32768)
                        {
                            var vFoaInfos = GetVfoaInfos();
                            for (var j = 0; j < 32; j++) array[j] = vFoaInfos[j];
                        }
                        else if (num == 32800)
                        {
                            var vFobInfos = GetVfobInfos();
                            for (var k = 0; k < 32; k++) array[k] = vFobInfos[k];
                        }
                        else if (num == 36864)
                        {
                            array[0] = (byte)AppData.FunCfgs.Sql;
                            array[1] = (byte)AppData.FunCfgs.SaveMode;
                            array[2] = (byte)AppData.FunCfgs.Vox;
                            array[3] = (byte)AppData.FunCfgs.VoxDlyTime;
                            array[4] = (byte)AppData.FunCfgs.DualStandby;
                            array[5] = (byte)AppData.FunCfgs.Tot;
                            array[6] = (byte)AppData.FunCfgs.Beep;
                            array[7] = (byte)AppData.FunCfgs.SideTone;
                            array[8] = (byte)AppData.FunCfgs.ScanMode;
                            array[9] = (byte)AppData.Vfos.Pttid;
                            array[10] = (byte)AppData.FunCfgs.PttDly;
                            array[11] = (byte)AppData.FunCfgs.ChADisType;
                            array[12] = (byte)AppData.FunCfgs.ChBDisType;
                            array[13] = (byte)AppData.FunCfgs.CbBMisscall;
                            array[14] = (byte)AppData.FunCfgs.AutoLock;
                            array[15] = (byte)AppData.FunCfgs.MicGain;
                            array[16] = (byte)AppData.FunCfgs.AlarmMode;
                            array[17] = (byte)AppData.FunCfgs.TailClear;
                            array[18] = (byte)AppData.FunCfgs.RptTailClear;
                            array[19] = (byte)AppData.FunCfgs.RptTailDet;
                            array[20] = (byte)AppData.FunCfgs.Roger;
                            array[21] = 0;
                            array[22] = (byte)AppData.FunCfgs.FmEnable;
                            array[23] = 0;
                            array[23] |= (byte)AppData.FunCfgs.ChAWorkmode;
                            array[23] |= (byte)(AppData.FunCfgs.ChBWorkmode << 4);
                            array[24] = (byte)AppData.FunCfgs.KeyLock;
                            array[25] = (byte)AppData.FunCfgs.AutoPowerOff;
                            array[26] = (byte)AppData.FunCfgs.PowerOnDisType;
                            array[27] = 0;
                            array[28] = (byte)AppData.FunCfgs.Tone;
                            array[29] = (byte)AppData.FunCfgs.CurBank;
                            array[30] = (byte)AppData.FunCfgs.Backlight;
                            array[31] = (byte)AppData.FunCfgs.MenuQuitTime;
                        }
                        else if (num == 36896)
                        {
                            array[0] = (byte)AppData.FunCfgs.Key1Short;
                            array[1] = (byte)AppData.FunCfgs.Key1Long;
                            array[2] = (byte)AppData.FunCfgs.Key2Short;
                            array[3] = (byte)AppData.FunCfgs.Key2Long;
                            array[4] = (byte)AppData.FunCfgs.Bright;
                            array[5] = 0;
                            array[6] = 0;
                            array[7] = (byte)AppData.FunCfgs.VoxSw;
                            array[8] = (byte)AppData.FunCfgs.PowerUpDisTime;
                            array[9] = (byte)AppData.FunCfgs.BluetoothAudioGain;
                            // 中转模式
                            // 36913 ->　sbtn_traanser_speaker
                            // 36912 -> sbtn_transfer_mode
                            array[16] = (byte)AppData.FunCfgs.RelaySw;
                            array[17] = (byte)AppData.FunCfgs.RelaySpeakerSw;
                            if (AppData.FunCfgs.CallSign != null && AppData.FunCfgs.CallSign != "")
                            {
                                var bytes = Encoding.GetEncoding("gb2312").GetBytes(AppData.FunCfgs.CallSign);
                                var array2 = bytes;
                                Array.Copy(bytes, 0, array, 10, bytes.Length);
                            }
                        }
                        else if (num == 40960)
                        {
                            GetFmFreq(array, 0, AppData.Fms.CurFreq);
                            GetFmFreq(array, 2, AppData.Fms.Channels[0]);
                            GetFmFreq(array, 4, AppData.Fms.Channels[1]);
                            GetFmFreq(array, 6, AppData.Fms.Channels[2]);
                            GetFmFreq(array, 8, AppData.Fms.Channels[3]);
                            GetFmFreq(array, 10, AppData.Fms.Channels[4]);
                            GetFmFreq(array, 12, AppData.Fms.Channels[5]);
                            GetFmFreq(array, 14, AppData.Fms.Channels[6]);
                            GetFmFreq(array, 16, AppData.Fms.Channels[7]);
                            GetFmFreq(array, 18, AppData.Fms.Channels[8]);
                            GetFmFreq(array, 20, AppData.Fms.Channels[9]);
                            GetFmFreq(array, 22, AppData.Fms.Channels[10]);
                            GetFmFreq(array, 24, AppData.Fms.Channels[11]);
                            GetFmFreq(array, 26, AppData.Fms.Channels[12]);
                            GetFmFreq(array, 28, AppData.Fms.Channels[13]);
                            GetFmFreq(array, 30, AppData.Fms.Channels[14]);
                        }
                        else if (num >= 45056 && num < 45744)
                        {
                            switch (num)
                            {
                                case 45056:
                                    GetDtmfWord(array, 0, AppData.Dtmfs.LocalId);
                                    GetDtmfWord(array, 16, AppData.Dtmfs.Group[0]);
                                    break;
                                case 45088:
                                    GetDtmfWord(array, 0, AppData.Dtmfs.Group[1]);
                                    GetDtmfWord(array, 16, AppData.Dtmfs.Group[2]);
                                    break;
                                case 45120:
                                    GetDtmfWord(array, 0, AppData.Dtmfs.Group[3]);
                                    GetDtmfWord(array, 16, AppData.Dtmfs.Group[4]);
                                    break;
                                case 45152:
                                    GetDtmfWord(array, 0, AppData.Dtmfs.Group[5]);
                                    GetDtmfWord(array, 16, AppData.Dtmfs.Group[6]);
                                    break;
                                case 45184:
                                    GetDtmfWord(array, 0, AppData.Dtmfs.Group[7]);
                                    GetDtmfWord(array, 16, AppData.Dtmfs.Group[8]);
                                    break;
                                case 45216:
                                    GetDtmfWord(array, 0, AppData.Dtmfs.Group[9]);
                                    GetDtmfWord(array, 16, AppData.Dtmfs.Group[10]);
                                    break;
                                case 45248:
                                    GetDtmfWord(array, 0, AppData.Dtmfs.Group[11]);
                                    GetDtmfWord(array, 16, AppData.Dtmfs.Group[12]);
                                    break;
                                case 45280:
                                    GetDtmfWord(array, 0, AppData.Dtmfs.Group[13]);
                                    GetDtmfWord(array, 16, AppData.Dtmfs.Group[14]);
                                    break;
                                case 45312:
                                    GetDtmfWord(array, 0, AppData.Dtmfs.Group[15]);
                                    GetDtmfWord(array, 16, AppData.Dtmfs.Group[16]);
                                    break;
                                case 45344:
                                    GetDtmfWord(array, 0, AppData.Dtmfs.Group[17]);
                                    GetDtmfWord(array, 16, AppData.Dtmfs.Group[18]);
                                    break;
                                case 45376:
                                    GetDtmfWord(array, 0, AppData.Dtmfs.Group[19]);
                                    GetDtmfName(array, 16, AppData.Dtmfs.GroupName[0]);
                                    break;
                                case 45408:
                                    GetDtmfName(array, 0, AppData.Dtmfs.GroupName[1]);
                                    GetDtmfName(array, 16, AppData.Dtmfs.GroupName[2]);
                                    break;
                                case 45440:
                                    GetDtmfName(array, 0, AppData.Dtmfs.GroupName[3]);
                                    GetDtmfName(array, 16, AppData.Dtmfs.GroupName[4]);
                                    break;
                                case 45472:
                                    GetDtmfName(array, 0, AppData.Dtmfs.GroupName[5]);
                                    GetDtmfName(array, 16, AppData.Dtmfs.GroupName[6]);
                                    break;
                                case 45504:
                                    GetDtmfName(array, 0, AppData.Dtmfs.GroupName[7]);
                                    GetDtmfName(array, 16, AppData.Dtmfs.GroupName[8]);
                                    break;
                                case 45536:
                                    GetDtmfName(array, 0, AppData.Dtmfs.GroupName[9]);
                                    GetDtmfName(array, 16, AppData.Dtmfs.GroupName[10]);
                                    break;
                                case 45568:
                                    GetDtmfName(array, 0, AppData.Dtmfs.GroupName[11]);
                                    GetDtmfName(array, 16, AppData.Dtmfs.GroupName[12]);
                                    break;
                                case 45600:
                                    GetDtmfName(array, 0, AppData.Dtmfs.GroupName[13]);
                                    GetDtmfName(array, 16, AppData.Dtmfs.GroupName[14]);
                                    break;
                                case 45632:
                                    GetDtmfName(array, 0, AppData.Dtmfs.GroupName[15]);
                                    GetDtmfName(array, 16, AppData.Dtmfs.GroupName[16]);
                                    break;
                                case 45664:
                                    GetDtmfName(array, 0, AppData.Dtmfs.GroupName[17]);
                                    GetDtmfName(array, 16, AppData.Dtmfs.GroupName[18]);
                                    break;
                                case 45696:
                                    GetDtmfName(array, 0, AppData.Dtmfs.GroupName[19]);
                                    array[16] = (byte)AppData.Dtmfs.WordTime;
                                    array[17] = (byte)AppData.Dtmfs.IdleTime;
                                    break;
                                case 45728:
                                    GetCallIdWord(array, 0, AppData.Mdcs.CallId);
                                    GetMDC1200_IDWord(array, 16, AppData.Mdcs.Id);
                                    break;
                            }
                        }

                        byData = _helper.LoadPackage(87, num, array, (byte)array.Length);
                        _hid.Send(byData);
                        _timer.Start();
                        _progressVal = num * 100 / 45728;
                        if (_progressVal > 100) _progressVal = 100;

                        _progressCont = "进度..." + _progressVal + "%";
                        UpdateProgressBar(_progressVal, _progressCont);
                        _step = Step.StepWrite2;
                        break;
                    case Step.StepWrite2:
                        if (!_hid.FlagReceiveData) break;

                        _hid.FlagReceiveData = false;
                        _helper.AnalyzePackage(_hid.RxBuffer);
                        if (_helper.ErrorCode == HidErrors.ErNone)
                        {
                            _timer.Stop();
                            ResetRetryCount();
                            for (var i = 0; i < 32; i++) array[i] = byte.MaxValue;

                            if (num >= 45728)
                            {
                                _progressVal = 100;
                                _progressCont = "完成";
                                UpdateProgressBar(_progressVal, _progressCont);
                                _flagTransmitting = false;
                                return true;
                            }

                            num += 32;
                            _step = Step.StepWrite1;
                        }

                        break;
                }
            }
            else
            {
                if (_timesOfRetry <= 0)
                {
                    _timer.Stop();
                    _flagTransmitting = false;
                    return false;
                }

                _timesOfRetry--;
                _flagRetry = false;
                _hid.Send(byData);
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
                if (strData == _tblCtsdcs[i])
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

    private byte[] GetChannelInfos(int chNum)
    {
        var num = chNum / 32;
        var num2 = chNum % 32;
        var array = new byte[32]
        {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255
        };
        if (AppData.ChannelList[num][num2].RxFreq != "")
        {
            var sourceArray = CaculateFreq_StrToHex(AppData.ChannelList[num][num2].RxFreq);
            Array.Copy(sourceArray, 0, array, 0, 4);
            if (AppData.ChannelList[num][num2].TxFreq != "")
            {
                var sourceArray2 = CaculateFreq_StrToHex(AppData.ChannelList[num][num2].TxFreq);
                Array.Copy(sourceArray2, 0, array, 4, 4);
            }

            var sourceArray3 = CaculateCtsDcs(AppData.ChannelList[num][num2].StrRxCtsDcs);
            Array.Copy(sourceArray3, 0, array, 8, 2);
            sourceArray3 = CaculateCtsDcs(AppData.ChannelList[num][num2].StrTxCtsDcs);
            Array.Copy(sourceArray3, 0, array, 10, 2);
            array[12] = (byte)AppData.ChannelList[num][num2].SignalGroup;
            array[13] = (byte)AppData.ChannelList[num][num2].Pttid;
            array[14] = (byte)AppData.ChannelList[num][num2].TxPower;
            array[15] = 0;
            array[15] |= (byte)(AppData.ChannelList[num][num2].Bandwide << 6);
            array[15] |= (byte)(AppData.ChannelList[num][num2].SqMode << 4);
            array[15] |= (byte)(AppData.ChannelList[num][num2].ScanAdd << 2);
            array[15] |= (byte)(AppData.ChannelList[num][num2].SignalSystem & 3);
            if (AppData.ChannelList[num][num2].Name != "")
            {
                var num3 = 0;
                var bytes = Encoding.GetEncoding("gb2312").GetBytes(AppData.ChannelList[num][num2].Name);
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

    private byte[] GetVfoaInfos()
    {
        var array = new byte[32]
        {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 0, 0, 0,
            0, 0, 0, 255, 255, 255, 255, 255, 255, 255,
            255, 255
        };
        var s = AppData.Vfos.VfoAFreq.Remove(3, 1);
        var num = int.Parse(s);
        array[0] = (byte)((uint)num & 0xFFu);
        array[1] = (byte)((uint)(num >> 8) & 0xFFu);
        array[2] = (byte)((uint)(num >> 16) & 0xFFu);
        array[3] = (byte)((uint)(num >> 24) & 0xFFu);
        var sourceArray = CaculateCtsDcs(AppData.Vfos.StrVfoaRxCtsDcs);
        Array.Copy(sourceArray, 0, array, 4, 2);
        sourceArray = CaculateCtsDcs(AppData.Vfos.StrVfoaTxCtsDcs);
        Array.Copy(sourceArray, 0, array, 6, 2);
        array[8] = (byte)AppData.Vfos.VfoABusyLock;
        array[9] = (byte)AppData.Vfos.VfoASignalGroup;
        array[10] = (byte)AppData.Vfos.VfoATxPower;
        array[11] = (byte)AppData.Vfos.VfoABandwide;
        array[12] = (byte)AppData.Vfos.VfoAScram;
        array[13] = (byte)AppData.Vfos.VfoAStep;
        array[14] = (byte)AppData.Vfos.VfoADir;
        var s2 = AppData.Vfos.VfoAOffset.Remove(2, 1);
        var num2 = int.Parse(s2) * 10;
        array[15] = (byte)((uint)num2 & 0xFFu);
        array[16] = (byte)((uint)(num2 >> 8) & 0xFFu);
        array[17] = (byte)((uint)(num2 >> 16) & 0xFFu);
        array[18] = (byte)((uint)(num2 >> 24) & 0xFFu);
        array[19] = (byte)AppData.Vfos.VfoAsqMode;
        array[26] = (byte)AppData.Vfos.VfoASignalSystem;
        return array;
    }

    private byte[] GetVfobInfos()
    {
        var array = new byte[32]
        {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 0, 0, 0,
            0, 0, 0, 255, 255, 255, 255, 255, 255, 255,
            255, 255
        };
        var s = AppData.Vfos.VfoBFreq.Remove(3, 1);
        var num = int.Parse(s);
        array[0] = (byte)((uint)num & 0xFFu);
        array[1] = (byte)((uint)(num >> 8) & 0xFFu);
        array[2] = (byte)((uint)(num >> 16) & 0xFFu);
        array[3] = (byte)((uint)(num >> 24) & 0xFFu);
        var sourceArray = CaculateCtsDcs(AppData.Vfos.StrVfobRxCtsDcs);
        Array.Copy(sourceArray, 0, array, 4, 2);
        sourceArray = CaculateCtsDcs(AppData.Vfos.StrVfobTxCtsDcs);
        Array.Copy(sourceArray, 0, array, 6, 2);
        array[8] = (byte)AppData.Vfos.VfoBBusyLock;
        array[9] = (byte)AppData.Vfos.VfoBSignalGroup;
        array[10] = (byte)AppData.Vfos.VfoBTxPower;
        array[11] = (byte)AppData.Vfos.VfoBBandwide;
        array[12] = (byte)AppData.Vfos.VfoBScram;
        array[13] = (byte)AppData.Vfos.VfoBStep;
        array[14] = (byte)AppData.Vfos.VfoBDir;
        var s2 = AppData.Vfos.VfoBOffset.Remove(2, 1);
        var num2 = int.Parse(s2) * 10;
        array[15] = (byte)((uint)num2 & 0xFFu);
        array[16] = (byte)((uint)(num2 >> 8) & 0xFFu);
        array[17] = (byte)((uint)(num2 >> 16) & 0xFFu);
        array[18] = (byte)((uint)(num2 >> 24) & 0xFFu);
        array[19] = (byte)AppData.Vfos.VfoBsqMode;
        array[26] = (byte)AppData.Vfos.VfoBSignalSystem;
        return array;
    }

    private void GetFmFreq(byte[] payload, int offset, int freq)
    {
        if (freq != 0)
        {
            payload[offset] = (byte)freq;
            payload[offset + 1] = (byte)(freq >> 8);
        }
    }

    private void GetDtmfWord(byte[] payload, int offset, string word)
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

    private void GetDtmfName(byte[] payload, int offset, string word)
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

    private void GetCallIdWord(byte[] payload, int offset, string callId)
    {
        if (callId != "")
        {
            var bytes = Encoding.ASCII.GetBytes(callId);
            for (var i = 0; i < callId.Length; i++) payload[i + offset] = bytes[i];
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
        while (_flagTransmitting && !token.IsCancellationRequested)
            if (!_flagRetry)
            {
                switch (_step)
                {
                    case Step.StepRead1:
                        byData = _helper.LoadPackage(82, num, null, 1);
                        _hid.Send(byData);
                        _progressVal = num * 100 / 45728;
                        if (_progressVal > 100) _progressVal = 100;

                        _progressCont = "进度..." + _progressVal + "%";
                        UpdateProgressBar(_progressVal, _progressCont);
                        ResetRetryCount();
                        _timer.Start();
                        _step = Step.StepRead2;
                        break;
                    case Step.StepRead2:
                        if (!_hid.FlagReceiveData) break;

                        _hid.FlagReceiveData = false;
                        _timer.Stop();
                        ResetRetryCount();
                        _helper.AnalyzePackage(_hid.RxBuffer);
                        if (_helper.ErrorCode != HidErrors.ErNone) break;

                        if (num < 30720)
                        {
                            SetChannelInfos(num2++, _helper.Payload);
                        }
                        else if (num >= 32000 && num < 32480)
                        {
                            var num3 = (num - 32000) / 16;
                            AppData.BankName[num3] = SetbankName(_helper.Payload, 0);
                            AppData.BankName[num3 + 1] = SetbankName(_helper.Payload, 16);
                        }
                        else if (num == 32768)
                        {
                            SetVfoaInfos(_helper.Payload);
                        }
                        else if (num == 32800)
                        {
                            SetVfobInfos(_helper.Payload);
                        }
                        else if (num == 36864)
                        {
                            AppData.FunCfgs.Sql = _helper.Payload[0] % 10;
                            AppData.FunCfgs.SaveMode = _helper.Payload[1] % 2;
                            AppData.FunCfgs.Vox = _helper.Payload[2] % 3;
                            AppData.FunCfgs.VoxDlyTime = _helper.Payload[3] % 16;
                            AppData.FunCfgs.DualStandby = _helper.Payload[4] % 2;
                            AppData.FunCfgs.Tot = _helper.Payload[5] % 9;
                            AppData.FunCfgs.Beep = _helper.Payload[6] % 4;
                            AppData.FunCfgs.SideTone = _helper.Payload[7] % 2;
                            AppData.FunCfgs.ScanMode = _helper.Payload[8] % 3;
                            AppData.Vfos.Pttid = _helper.Payload[9] % 4;
                            AppData.FunCfgs.PttDly = _helper.Payload[10] % 16;
                            AppData.FunCfgs.ChADisType = _helper.Payload[11] % 3;
                            AppData.FunCfgs.ChBDisType = _helper.Payload[12] % 3;
                            AppData.FunCfgs.CbBMisscall = _helper.Payload[13] % 2;
                            AppData.FunCfgs.AutoLock = _helper.Payload[14] % 7;
                            AppData.FunCfgs.MicGain = _helper.Payload[15] % 3;
                            AppData.FunCfgs.AlarmMode = _helper.Payload[16] % 3;
                            AppData.FunCfgs.TailClear = _helper.Payload[17] % 2;
                            AppData.FunCfgs.RptTailClear = _helper.Payload[18] % 11;
                            AppData.FunCfgs.RptTailDet = _helper.Payload[19] % 11;
                            AppData.FunCfgs.Roger = _helper.Payload[20] % 2;
                            AppData.FunCfgs.FmEnable = _helper.Payload[22] % 2;
                            AppData.FunCfgs.ChAWorkmode = (_helper.Payload[23] & 0xF) % 2;
                            AppData.FunCfgs.ChBWorkmode = ((_helper.Payload[23] & 0xF0) >> 4) % 2;
                            AppData.FunCfgs.KeyLock = _helper.Payload[24] % 2;
                            AppData.FunCfgs.AutoPowerOff = _helper.Payload[25] % 5;
                            AppData.FunCfgs.PowerOnDisType = _helper.Payload[26] % 3;
                            AppData.FunCfgs.Tone = _helper.Payload[28] % 4;
                            AppData.FunCfgs.CurBank = _helper.Payload[29] % 30;
                            AppData.FunCfgs.Backlight = _helper.Payload[30] % 9;
                            AppData.FunCfgs.MenuQuitTime = _helper.Payload[31] % 11;
                        }
                        else if (num == 36896)
                        {
                            AppData.FunCfgs.Key1Short = _helper.Payload[0] % 10;
                            AppData.FunCfgs.Key1Long = _helper.Payload[1] % 10;
                            AppData.FunCfgs.Key2Short = _helper.Payload[2] % 10;
                            AppData.FunCfgs.Key2Long = _helper.Payload[3] % 10;
                            AppData.FunCfgs.Bright = _helper.Payload[4] % 2;
                            AppData.FunCfgs.VoxSw = _helper.Payload[7] % 2;
                            AppData.FunCfgs.PowerUpDisTime = _helper.Payload[8] % 15;
                            AppData.FunCfgs.BluetoothAudioGain = _helper.Payload[9] % 5;
                            // 中转模式
                            AppData.FunCfgs.RelaySw = _helper.Payload[16] % 2;
                            AppData.FunCfgs.RelaySpeakerSw = _helper.Payload[17] % 2;
                            var num4 = 0;
                            for (var i = 0; i < 6 && _helper.Payload[10 + i] != byte.MaxValue; i++)
                            {
                                num4++;
                                Debug.WriteLine("Byte:{0}", _helper.Payload[10 + i]);
                            }

                            AppData.FunCfgs.CallSign =
                                Encoding.GetEncoding("gb2312").GetString(_helper.Payload, 10, num4);
                        }
                        else if (num == 40960)
                        {
                            AppData.Fms.CurFreq = SetFmFreq(_helper.Payload, 0);
                            AppData.Fms.Channels[0] = SetFmFreq(_helper.Payload, 2);
                            AppData.Fms.Channels[1] = SetFmFreq(_helper.Payload, 4);
                            AppData.Fms.Channels[2] = SetFmFreq(_helper.Payload, 6);
                            AppData.Fms.Channels[3] = SetFmFreq(_helper.Payload, 8);
                            AppData.Fms.Channels[4] = SetFmFreq(_helper.Payload, 10);
                            AppData.Fms.Channels[5] = SetFmFreq(_helper.Payload, 12);
                            AppData.Fms.Channels[6] = SetFmFreq(_helper.Payload, 14);
                            AppData.Fms.Channels[7] = SetFmFreq(_helper.Payload, 16);
                            AppData.Fms.Channels[8] = SetFmFreq(_helper.Payload, 18);
                            AppData.Fms.Channels[9] = SetFmFreq(_helper.Payload, 20);
                            AppData.Fms.Channels[10] = SetFmFreq(_helper.Payload, 22);
                            AppData.Fms.Channels[11] = SetFmFreq(_helper.Payload, 24);
                            AppData.Fms.Channels[12] = SetFmFreq(_helper.Payload, 26);
                            AppData.Fms.Channels[13] = SetFmFreq(_helper.Payload, 28);
                            AppData.Fms.Channels[14] = SetFmFreq(_helper.Payload, 30);
                        }
                        else if (num >= 45056 && num < 45744)
                        {
                            switch (num)
                            {
                                case 45056:
                                    AppData.Dtmfs.LocalId = SetDtmfWord(_helper.Payload, 0);
                                    AppData.Dtmfs.Group[0] = SetDtmfWord(_helper.Payload, 16);
                                    break;
                                case 45088:
                                    AppData.Dtmfs.Group[1] = SetDtmfWord(_helper.Payload, 0);
                                    AppData.Dtmfs.Group[2] = SetDtmfWord(_helper.Payload, 16);
                                    break;
                                case 45120:
                                    AppData.Dtmfs.Group[3] = SetDtmfWord(_helper.Payload, 0);
                                    AppData.Dtmfs.Group[4] = SetDtmfWord(_helper.Payload, 16);
                                    break;
                                case 45152:
                                    AppData.Dtmfs.Group[5] = SetDtmfWord(_helper.Payload, 0);
                                    AppData.Dtmfs.Group[6] = SetDtmfWord(_helper.Payload, 16);
                                    break;
                                case 45184:
                                    AppData.Dtmfs.Group[7] = SetDtmfWord(_helper.Payload, 0);
                                    AppData.Dtmfs.Group[8] = SetDtmfWord(_helper.Payload, 16);
                                    break;
                                case 45216:
                                    AppData.Dtmfs.Group[9] = SetDtmfWord(_helper.Payload, 0);
                                    AppData.Dtmfs.Group[10] = SetDtmfWord(_helper.Payload, 16);
                                    break;
                                case 45248:
                                    AppData.Dtmfs.Group[11] = SetDtmfWord(_helper.Payload, 0);
                                    AppData.Dtmfs.Group[12] = SetDtmfWord(_helper.Payload, 16);
                                    break;
                                case 45280:
                                    AppData.Dtmfs.Group[13] = SetDtmfWord(_helper.Payload, 0);
                                    AppData.Dtmfs.Group[14] = SetDtmfWord(_helper.Payload, 16);
                                    break;
                                case 45312:
                                    AppData.Dtmfs.Group[15] = SetDtmfWord(_helper.Payload, 0);
                                    AppData.Dtmfs.Group[16] = SetDtmfWord(_helper.Payload, 16);
                                    break;
                                case 45344:
                                    AppData.Dtmfs.Group[17] = SetDtmfWord(_helper.Payload, 0);
                                    AppData.Dtmfs.Group[18] = SetDtmfWord(_helper.Payload, 16);
                                    break;
                                case 45376:
                                    AppData.Dtmfs.Group[19] = SetDtmfWord(_helper.Payload, 0);
                                    AppData.Dtmfs.GroupName[0] = SetDtmfGroupName(_helper.Payload, 16);
                                    break;
                                case 45408:
                                    AppData.Dtmfs.GroupName[1] = SetDtmfGroupName(_helper.Payload, 0);
                                    AppData.Dtmfs.GroupName[2] = SetDtmfGroupName(_helper.Payload, 16);
                                    break;
                                case 45440:
                                    AppData.Dtmfs.GroupName[3] = SetDtmfGroupName(_helper.Payload, 0);
                                    AppData.Dtmfs.GroupName[4] = SetDtmfGroupName(_helper.Payload, 16);
                                    break;
                                case 45472:
                                    AppData.Dtmfs.GroupName[5] = SetDtmfGroupName(_helper.Payload, 0);
                                    AppData.Dtmfs.GroupName[6] = SetDtmfGroupName(_helper.Payload, 16);
                                    break;
                                case 45504:
                                    AppData.Dtmfs.GroupName[7] = SetDtmfGroupName(_helper.Payload, 0);
                                    AppData.Dtmfs.GroupName[8] = SetDtmfGroupName(_helper.Payload, 16);
                                    break;
                                case 45536:
                                    AppData.Dtmfs.GroupName[9] = SetDtmfGroupName(_helper.Payload, 0);
                                    AppData.Dtmfs.GroupName[10] = SetDtmfGroupName(_helper.Payload, 16);
                                    break;
                                case 45568:
                                    AppData.Dtmfs.GroupName[11] = SetDtmfGroupName(_helper.Payload, 0);
                                    AppData.Dtmfs.GroupName[12] = SetDtmfGroupName(_helper.Payload, 16);
                                    break;
                                case 45600:
                                    AppData.Dtmfs.GroupName[13] = SetDtmfGroupName(_helper.Payload, 0);
                                    AppData.Dtmfs.GroupName[14] = SetDtmfGroupName(_helper.Payload, 16);
                                    break;
                                case 45632:
                                    AppData.Dtmfs.GroupName[15] = SetDtmfGroupName(_helper.Payload, 0);
                                    AppData.Dtmfs.GroupName[16] = SetDtmfGroupName(_helper.Payload, 16);
                                    break;
                                case 45664:
                                    AppData.Dtmfs.GroupName[17] = SetDtmfGroupName(_helper.Payload, 0);
                                    AppData.Dtmfs.GroupName[18] = SetDtmfGroupName(_helper.Payload, 16);
                                    break;
                                case 45696:
                                    AppData.Dtmfs.GroupName[19] = SetDtmfGroupName(_helper.Payload, 0);
                                    AppData.Dtmfs.WordTime = _helper.Payload[16];
                                    AppData.Dtmfs.IdleTime = _helper.Payload[17];
                                    break;
                                case 45728:
                                    AppData.Mdcs.CallId = SetCallId(_helper.Payload, 0);
                                    AppData.Mdcs.Id = SetMdc1200Id(_helper.Payload, 16);
                                    break;
                            }
                        }

                        if (num < 45728)
                        {
                            num += 32;
                            _timer.Start();
                            _step = Step.StepRead1;
                            break;
                        }

                        _progressVal = 100;
                        _progressCont = "完成";
                        UpdateProgressBar(_progressVal, _progressCont);
                        byData = _helper.LoadPackage(69, 0, null, 1);
                        _hid.Send(byData);
                        _flagTransmitting = false;
                        return true;
                }
            }
            else
            {
                if (_timesOfRetry <= 0)
                {
                    _timer.Stop();
                    _flagTransmitting = false;
                    return false;
                }

                _timesOfRetry--;
                _flagRetry = false;
                _hid.Send(byData);
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
                if (dat[offset] != 0 && dat[offset] <= 210) return _tblCtsdcs[dat[offset] - 1];

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

    private void SetChannelInfos(int chNum, byte[] dat)
    {
        var num = chNum / 32;
        var num2 = chNum % 32;
        if (dat[0] == byte.MaxValue || dat[1] == byte.MaxValue || dat[3] == 0) return;

        AppData.ChannelList[num][num2].RxFreq = CaculateFreq_HexToStr(dat, 0);
        if (!string.IsNullOrEmpty(AppData.ChannelList[num][num2].RxFreq))
            AppData.ChannelList[num][num2].IsVisable = true;
        if (dat[4] != byte.MaxValue && dat[5] != byte.MaxValue)
            AppData.ChannelList[num][num2].TxFreq = CaculateFreq_HexToStr(dat, 4);

        AppData.ChannelList[num][num2].StrRxCtsDcs = CaculateCtsDcs(dat, 8);
        AppData.ChannelList[num][num2].StrTxCtsDcs = CaculateCtsDcs(dat, 10);
        AppData.ChannelList[num][num2].SignalGroup = dat[12] % 20;
        AppData.ChannelList[num][num2].Pttid = dat[13] % 4;
        AppData.ChannelList[num][num2].TxPower = dat[14] % 2;
        AppData.ChannelList[num][num2].Bandwide = (dat[15] >> 6) & 1;
        AppData.ChannelList[num][num2].SqMode = ((dat[15] >> 4) & 3) % 3;
        AppData.ChannelList[num][num2].ScanAdd = (dat[15] >> 2) & 1;
        AppData.ChannelList[num][num2].SignalSystem = (dat[15] & 3) % 3;
        if (dat[20] != byte.MaxValue)
        {
            var num3 = 0;
            for (var i = 0; i < 12 && dat[20 + i] != byte.MaxValue; i++) num3++;

            AppData.ChannelList[num][num2].Name = Encoding.GetEncoding("gb2312").GetString(dat, 20, num3);
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

    private void SetVfoaInfos(byte[] dat)
    {
        var text = CaculateFreq(dat);
        if (text != "") AppData.Vfos.VfoAFreq = text;

        AppData.Vfos.StrVfoaRxCtsDcs = CaculateCtsDcs(dat, 4);
        AppData.Vfos.StrVfoaTxCtsDcs = CaculateCtsDcs(dat, 6);
        AppData.Vfos.VfoABusyLock = dat[8] % 2;
        AppData.Vfos.VfoASignalGroup = dat[9] % 20;
        AppData.Vfos.VfoATxPower = dat[10] % 3;
        AppData.Vfos.VfoABandwide = dat[11] % 2;
        AppData.Vfos.VfoAScram = dat[12] % 9;
        AppData.Vfos.VfoAStep = dat[13] % 8;
        AppData.Vfos.VfoADir = dat[14] % 3;
        AppData.Vfos.VfoAOffset = CaculateOffset(dat, 15);
        AppData.Vfos.VfoAsqMode = dat[19] % 3;
        AppData.Vfos.VfoASignalSystem = dat[26] % 3;
    }

    private void SetVfobInfos(byte[] dat)
    {
        var text = CaculateFreq(dat);
        if (text != "") AppData.Vfos.VfoBFreq = text;

        AppData.Vfos.StrVfobRxCtsDcs = CaculateCtsDcs(dat, 4);
        AppData.Vfos.StrVfobTxCtsDcs = CaculateCtsDcs(dat, 6);
        AppData.Vfos.VfoBBusyLock = dat[8] % 2;
        AppData.Vfos.VfoBSignalGroup = dat[9] % 20;
        AppData.Vfos.VfoBTxPower = dat[10] % 3;
        AppData.Vfos.VfoBBandwide = dat[11] % 2;
        AppData.Vfos.VfoBScram = dat[12] % 9;
        AppData.Vfos.VfoBStep = dat[13] % 8;
        AppData.Vfos.VfoBDir = dat[14] % 3;
        AppData.Vfos.VfoBOffset = CaculateOffset(dat, 15);
        AppData.Vfos.VfoBsqMode = dat[19] % 3;
        AppData.Vfos.VfoBSignalSystem = dat[26] % 3;
    }

    private int SetFmFreq(byte[] payload, int offset)
    {
        var num = 0;
        if (payload[offset] != byte.MaxValue && payload[offset + 1] != byte.MaxValue)
            num = payload[offset] + (payload[offset + 1] << 8);

        if (num < 650 || num > 1080) num = 0;

        return num;
    }

    private string SetDtmfWord(byte[] payload, int offset)
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

    private string SetDtmfGroupName(byte[] payload, int offset)
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

    private string SetCallId(byte[] payload, int offset)
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

    private string SetMdc1200Id(byte[] payload, int offset)
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