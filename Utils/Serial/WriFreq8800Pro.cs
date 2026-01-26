using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Timers;
using SenhaixFreqWriter.Constants.Shx8800Pro;
using SenhaixFreqWriter.DataModels.Shx8800Pro;
using Timer = System.Timers.Timer;

namespace SenhaixFreqWriter.Utils.Serial;

public class WriFreq8800Pro
{
    private readonly DataHelper _helper;

    private readonly OpType _opType;

    private readonly MySerialPort _port;

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

    private bool _flagReceiveData;

    private bool _flagRetry;

    private bool _flagTransmitting;

    private string _progressCont = "";

    private int _progressVal;

    private byte[] _rxBuffer = new byte[128];

    private Step _step;

    private Timer _timer;

    private byte _timesOfRetry = 5;

    public AppData AppData;


    public ConcurrentQueue<ProgressBarValue> StatusQueue = new();

    public WriFreq8800Pro(MySerialPort port, OpType opType)
    {
        _port = port;
        _opType = opType;
        AppData = AppData.GetInstance();
        _helper = new DataHelper();
        StatusQueue.Clear();
        TimerInit();
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
        _rxBuffer = e;
        _flagReceiveData = true;
    }

    private void ResetRetryCount()
    {
        _timesOfRetry = 5;
        _flagRetry = false;
    }

    public bool DoIt(CancellationToken cancellationToken)
    {
        _flagTransmitting = true;
        ResetRetryCount();
        _step = Step.StepHandshake1;
        if (HandShake(cancellationToken))
        {
            if (_opType == OpType.Write)
            {
                if (Write(cancellationToken)) return true;
            }
            else if (Read(cancellationToken))
            {
                return true;
            }
        }

        return false;
    }

    private bool HandShake(CancellationToken cancellationToken)
    {
        var array = new byte[1];
        while (_flagTransmitting && !cancellationToken.IsCancellationRequested)
            if (!_flagRetry)
            {
                switch (_step)
                {
                    case Step.StepHandshake1:
                        array = Encoding.ASCII.GetBytes("PROGRAMSHXPU");
                        _port.WriteByte(array, 0, array.Length);
                        _progressVal = 0;
                        _progressCont = "握手...";
                        StatusQueue.Enqueue(new ProgressBarValue(_progressVal, _progressCont));
                        _timer.Start();
                        ResetRetryCount();
                        _step = Step.StepHandshake2;
                        break;
                    case Step.StepHandshake2:
                        if (_port.BytesToReadFromCache >= 1)
                        {
                            _port.ReadByte(_rxBuffer, 0, 1);
                            if (_rxBuffer[0] == 6)
                            {
                                array = new byte[1] { 70 };
                                _port.WriteByte(array, 0, array.Length);
                                ResetRetryCount();
                                _step = Step.StepHandshake3;
                            }
                        }

                        break;
                    case Step.StepHandshake3:
                        if (_port.BytesToReadFromCache >= 16)
                        {
                            _port.ReadByte(_rxBuffer, 0, 16);
                            _timer.Stop();
                            ResetRetryCount();
                            _progressVal = 0;
                            _progressCont = "进度..." + _progressVal + "%";
                            StatusQueue.Enqueue(new ProgressBarValue(_progressVal, _progressCont));
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
                _port.WriteByte(array, 0, array.Length);
            }

        return false;
    }

    private bool Write(CancellationToken cancellationToken)
    {
        var array = new byte[1];
        var array2 = new byte[64];
        ushort num = 0;
        var num2 = 0;
        while (_flagTransmitting && !cancellationToken.IsCancellationRequested)
            if (!_flagRetry)
            {
                switch (_step)
                {
                    case Step.StepWrite1:
                        if (num < 16384)
                        {
                            var channelInfos = GetChannelInfos(num2++);
                            var channelInfos2 = GetChannelInfos(num2++);
                            Array.Copy(channelInfos, 0, array2, 0, 32);
                            Array.Copy(channelInfos2, 0, array2, 32, 32);
                        }
                        else if (num == 32768)
                        {
                            var vFoaInfos = GetVfoaInfos();
                            var vFobInfos = GetVfobInfos();
                            for (var i = 0; i < 32; i++)
                            {
                                array2[i] = vFoaInfos[i];
                                array2[i + 32] = vFobInfos[i];
                            }
                        }
                        else if (num == 36864)
                        {
                            array2[0] = (byte)AppData.FunCfgs.Sql;
                            array2[1] = (byte)AppData.FunCfgs.SaveMode;
                            array2[2] = (byte)AppData.FunCfgs.Vox;
                            array2[3] = (byte)AppData.FunCfgs.Backlight;
                            array2[4] = (byte)AppData.FunCfgs.DualStandby;
                            array2[5] = (byte)AppData.FunCfgs.Tot;
                            array2[6] = (byte)AppData.FunCfgs.Beep;
                            array2[7] = (byte)AppData.FunCfgs.VoiceSw;
                            array2[8] = 0;
                            array2[9] = (byte)AppData.FunCfgs.SideTone;
                            array2[10] = (byte)AppData.FunCfgs.ScanMode;
                            array2[11] = (byte)AppData.Vfos.Pttid;
                            array2[12] = (byte)AppData.FunCfgs.PttDly;
                            array2[13] = (byte)AppData.FunCfgs.ChADisType;
                            array2[14] = (byte)AppData.FunCfgs.ChBDisType;
                            array2[16] = (byte)AppData.FunCfgs.AutoLock;
                            array2[17] = (byte)AppData.FunCfgs.AlarmMode;
                            array2[18] = (byte)AppData.FunCfgs.LocalSosTone;
                            array2[19] = 0;
                            array2[20] = (byte)AppData.FunCfgs.TailClear;
                            array2[21] = (byte)AppData.FunCfgs.RptTailClear;
                            array2[22] = (byte)AppData.FunCfgs.RptTailDet;
                            array2[23] = (byte)AppData.FunCfgs.Roger;
                            array2[24] = 0;
                            array2[25] = (byte)AppData.FunCfgs.FmEnable;
                            array2[26] = 0;
                            array2[26] |= (byte)AppData.FunCfgs.ChAWorkmode;
                            array2[26] |= (byte)(AppData.FunCfgs.ChBWorkmode << 4);
                            array2[27] = (byte)AppData.FunCfgs.KeyLock;
                            array2[28] = (byte)AppData.FunCfgs.PowerOnDisType;
                            array2[29] = 0;
                            array2[30] = (byte)AppData.FunCfgs.Tone;
                            array2[32] = (byte)AppData.FunCfgs.VoxDlyTime;
                            array2[33] = (byte)AppData.FunCfgs.MenuQuitTime;
                            array2[34] = (byte)AppData.FunCfgs.MicGain;
                            array2[36] = (byte)AppData.FunCfgs.PwrOnDlyTime;
                            array2[37] = (byte)AppData.FunCfgs.VoxSw;
                            array2[42] = (byte)AppData.FunCfgs.Key2Short;
                            array2[43] = (byte)AppData.FunCfgs.Key2Long;
                            array2[46] = (byte)AppData.FunCfgs.CurBankA;
                            array2[47] = (byte)AppData.FunCfgs.CurBankB;
                            array2[49] = (byte)AppData.FunCfgs.BtMicGain;
                            array2[50] = (byte)AppData.FunCfgs.BluetoothAudioGain;
                            if (AppData.FunCfgs.CallSign != null && AppData.FunCfgs.CallSign != "")
                            {
                                var bytes = Encoding.GetEncoding("gb2312").GetBytes(AppData.FunCfgs.CallSign);
                                Array.Copy(bytes, 0, array2, 52, bytes.Length);
                            }
                        }
                        else if (num >= 40960 && num <= 41216)
                        {
                            switch (num)
                            {
                                case 40960:
                                    GetDtmfWord(array2, 0, AppData.Dtmfs.LocalId);
                                    array2[5] = byte.MaxValue;
                                    array2[6] = (byte)AppData.Dtmfs.Pttid;
                                    array2[7] = (byte)AppData.Dtmfs.WordTime;
                                    array2[8] = (byte)AppData.Dtmfs.IdleTime;
                                    GetDtmfWord(array2, 32, AppData.Dtmfs.Group[0]);
                                    GetDtmfWord(array2, 48, AppData.Dtmfs.Group[1]);
                                    break;
                                case 41024:
                                    GetDtmfWord(array2, 0, AppData.Dtmfs.Group[2]);
                                    GetDtmfWord(array2, 16, AppData.Dtmfs.Group[3]);
                                    GetDtmfWord(array2, 32, AppData.Dtmfs.Group[4]);
                                    GetDtmfWord(array2, 48, AppData.Dtmfs.Group[5]);
                                    break;
                                case 41088:
                                    GetDtmfWord(array2, 0, AppData.Dtmfs.Group[6]);
                                    GetDtmfWord(array2, 16, AppData.Dtmfs.Group[7]);
                                    GetDtmfWord(array2, 32, AppData.Dtmfs.Group[8]);
                                    GetDtmfWord(array2, 48, AppData.Dtmfs.Group[9]);
                                    break;
                                case 41152:
                                    GetDtmfWord(array2, 0, AppData.Dtmfs.Group[10]);
                                    GetDtmfWord(array2, 16, AppData.Dtmfs.Group[11]);
                                    GetDtmfWord(array2, 32, AppData.Dtmfs.Group[12]);
                                    GetDtmfWord(array2, 48, AppData.Dtmfs.Group[13]);
                                    break;
                                case 41216:
                                    GetDtmfWord(array2, 0, AppData.Dtmfs.Group[14]);
                                    break;
                            }
                        }
                        else
                        {
                            switch (num)
                            {
                                case 41472:
                                    GetBankName(array2, 0, AppData.BankName[0]);
                                    GetBankName(array2, 16, AppData.BankName[1]);
                                    GetBankName(array2, 32, AppData.BankName[2]);
                                    GetBankName(array2, 48, AppData.BankName[3]);
                                    break;
                                case 41536:
                                    GetBankName(array2, 0, AppData.BankName[4]);
                                    GetBankName(array2, 16, AppData.BankName[5]);
                                    GetBankName(array2, 32, AppData.BankName[6]);
                                    GetBankName(array2, 48, AppData.BankName[7]);
                                    break;
                                case 45056:
                                {
                                    GetFmFreq(array2, 0, AppData.Fms.CurFreq);
                                    for (var j = 0; j < 30; j++) GetFmFreq(array2, 2 + j * 2, AppData.Fms.Channels[j]);

                                    array2[62] = 0;
                                    array2[63] = 0;
                                    break;
                                }
                            }
                        }

                        array = _helper.LoadPackage(87, num, array2, (byte)array2.Length);
                        _port.WriteByte(array, 0, array.Length);
                        _timer.Start();
                        _progressVal = num * 100 / 45056;
                        if (_progressVal > 100) _progressVal = 100;
                        _progressCont = "进度..." + _progressVal + "%";
                        StatusQueue.Enqueue(new ProgressBarValue(_progressVal, _progressCont));
                        _step = Step.StepWrite2;
                        break;
                    case Step.StepWrite2:
                        if (_port.BytesToReadFromCache < 1) break;

                        _port.ReadByte(_rxBuffer, 0, 1);
                        if (_rxBuffer[0] == 6)
                        {
                            _timer.Stop();
                            ResetRetryCount();
                            if (num >= 45056)
                            {
                                _progressVal = 100;
                                _progressCont = "完成";
                                StatusQueue.Enqueue(new ProgressBarValue(_progressVal, _progressCont));
                                _flagTransmitting = false;
                                return true;
                            }

                            num += 64;
                            switch (num)
                            {
                                case 16448:
                                    num = 32768;
                                    break;
                                case 32832:
                                    num = 36864;
                                    break;
                                case 36928:
                                    num = 40960;
                                    break;
                            }

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
                _port.WriteByte(array, 0, array.Length);
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
        var num = chNum / 64;
        var num2 = chNum % 64;
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
            array[15] |= (byte)(AppData.ChannelList[num][num2].ScanAdd << 2);
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
        for (var num2 = 7; num2 >= 0; num2--)
        {
            array[num2] = (byte)(num % 10);
            num /= 10;
        }

        var sourceArray = CaculateCtsDcs(AppData.Vfos.StrVfoaRxCtsDcs);
        Array.Copy(sourceArray, 0, array, 8, 2);
        sourceArray = CaculateCtsDcs(AppData.Vfos.StrVfoaTxCtsDcs);
        Array.Copy(sourceArray, 0, array, 10, 2);
        array[13] = (byte)AppData.Vfos.VfoABusyLock;
        array[14] = (byte)((AppData.Vfos.VfoADir << 4) | AppData.Vfos.VfoASignalGroup);
        array[16] = (byte)AppData.Vfos.VfoATxPower;
        array[17] = (byte)(AppData.Vfos.VfoABandwide << 6);
        array[19] = (byte)AppData.Vfos.VfoAStep;
        var array2 = AppData.Vfos.VfoAOffset.Split('.');
        var num3 = int.Parse(array2[0]) * 100000 + int.Parse(array2[1]) * 10;
        for (var num4 = 6; num4 >= 0; num4--)
        {
            array[20 + num4] = (byte)(num3 % 10);
            num3 /= 10;
        }

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
        for (var num2 = 7; num2 >= 0; num2--)
        {
            array[num2] = (byte)(num % 10);
            num /= 10;
        }

        var sourceArray = CaculateCtsDcs(AppData.Vfos.StrVfobRxCtsDcs);
        Array.Copy(sourceArray, 0, array, 8, 2);
        sourceArray = CaculateCtsDcs(AppData.Vfos.StrVfobTxCtsDcs);
        Array.Copy(sourceArray, 0, array, 10, 2);
        array[13] = (byte)AppData.Vfos.VfoBBusyLock;
        array[14] = (byte)((AppData.Vfos.VfoBDir << 4) | AppData.Vfos.VfoBSignalGroup);
        array[16] = (byte)AppData.Vfos.VfoBTxPower;
        array[17] = (byte)(AppData.Vfos.VfoBBandwide << 6);
        array[19] = (byte)AppData.Vfos.VfoBStep;
        var array2 = AppData.Vfos.VfoBOffset.Split('.');
        var num3 = int.Parse(array2[0]) * 100000 + int.Parse(array2[1]) * 10;
        for (var num4 = 6; num4 >= 0; num4--)
        {
            array[20 + num4] = (byte)(num3 % 10);
            num3 /= 10;
        }

        return array;
    }

    private void GetFmFreq(byte[] payload, int offset, int freq)
    {
        if (freq != 0)
        {
            payload[offset] = (byte)freq;
            payload[offset + 1] = (byte)(freq >> 8);
        }
        else
        {
            payload[offset] = 0;
            payload[offset + 1] = 0;
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

    private bool Read(CancellationToken cancellationToken)
    {
        byte[][] array = null;
        var array2 = new byte[10];
        ushort num = 0;
        var num2 = 0;
        while (_flagTransmitting && !cancellationToken.IsCancellationRequested)
            if (!_flagRetry)
            {
                switch (_step)
                {
                    case Step.StepRead1:
                        array2 = new byte[4]
                        {
                            82,
                            (byte)(num >> 8),
                            (byte)num,
                            64
                        };
                        _port.WriteByte(array2, 0, array2.Length);
                        _progressVal = num * 100 / 45056;
                        if (_progressVal > 100) _progressVal = 100;

                        _progressCont = "进度..." + _progressVal + "%";
                        StatusQueue.Enqueue(new ProgressBarValue(_progressVal, _progressCont));
                        ResetRetryCount();
                        _timer.Start();
                        _step = Step.StepRead2;
                        break;
                    case Step.StepRead2:
                    {
                        if (_port.BytesToReadFromCache < 68) break;

                        _timer.Stop();
                        ResetRetryCount();
                        _port.ReadByte(_rxBuffer, 0, _port.BytesToReadFromCache);
                        var b = (byte)(num >> 8);
                        var b2 = (byte)num;
                        if (_rxBuffer[1] != b || _rxBuffer[2] != b2) break;

                        array = new byte[2][]
                        {
                            new byte[32],
                            new byte[32]
                        };
                        for (var i = 0; i < 32; i++)
                        {
                            array[0][i] = _rxBuffer[i + 4];
                            array[1][i] = _rxBuffer[i + 4 + 32];
                        }

                        if (num < 16384)
                        {
                            SetChannelInfos(num2++, array[0]);
                            SetChannelInfos(num2++, array[1]);
                        }
                        else if (num == 32768)
                        {
                            SetVfoaInfos(array[0]);
                            SetVfobInfos(array[1]);
                        }
                        else if (num == 36864)
                        {
                            AppData.FunCfgs.Sql = _rxBuffer[4] % 10;
                            AppData.FunCfgs.SaveMode = _rxBuffer[5] % 4;
                            AppData.FunCfgs.Vox = _rxBuffer[6] % 10;
                            AppData.FunCfgs.Backlight = _rxBuffer[7] % 9;
                            AppData.FunCfgs.DualStandby = _rxBuffer[8] % 2;
                            AppData.FunCfgs.Tot = _rxBuffer[9] % 9;
                            AppData.FunCfgs.Beep = _rxBuffer[10] % 2;
                            AppData.FunCfgs.VoiceSw = _rxBuffer[11] % 2;
                            AppData.FunCfgs.SideTone = _rxBuffer[13] % 4;
                            AppData.FunCfgs.ScanMode = _rxBuffer[14] % 3;
                            AppData.Vfos.Pttid = _rxBuffer[15] % 4;
                            AppData.FunCfgs.PttDly = _rxBuffer[16] % 16;
                            AppData.FunCfgs.ChADisType = _rxBuffer[17] % 3;
                            AppData.FunCfgs.ChBDisType = _rxBuffer[18] % 3;
                            AppData.FunCfgs.AutoLock = _rxBuffer[20] % 7;
                            AppData.FunCfgs.AlarmMode = _rxBuffer[21] % 3;
                            AppData.FunCfgs.LocalSosTone = _rxBuffer[22] % 2;
                            AppData.FunCfgs.TailClear = _rxBuffer[24] % 2;
                            AppData.FunCfgs.RptTailClear = _rxBuffer[25] % 11;
                            AppData.FunCfgs.RptTailDet = _rxBuffer[26] % 11;
                            AppData.FunCfgs.Roger = _rxBuffer[27] % 2;
                            AppData.FunCfgs.FmEnable = _rxBuffer[29] % 2;
                            AppData.FunCfgs.ChAWorkmode = (_rxBuffer[30] & 0xF) % 2;
                            AppData.FunCfgs.ChBWorkmode = ((_rxBuffer[30] & 0xF0) >> 4) % 2;
                            AppData.FunCfgs.KeyLock = _rxBuffer[31] % 2;
                            AppData.FunCfgs.PowerOnDisType = _rxBuffer[32] % 22;
                            AppData.FunCfgs.Tone = _rxBuffer[34] % 4;
                            AppData.FunCfgs.VoxDlyTime = _rxBuffer[36] % 16;
                            AppData.FunCfgs.MenuQuitTime = _rxBuffer[37] % 11;
                            AppData.FunCfgs.MicGain = _rxBuffer[38] % 3;
                            AppData.FunCfgs.PwrOnDlyTime = _rxBuffer[40] % 15;
                            AppData.FunCfgs.VoxSw = _rxBuffer[41] % 2;
                            AppData.FunCfgs.Key2Short = _rxBuffer[46] % 5;
                            AppData.FunCfgs.Key2Long = _rxBuffer[47] % 5;
                            AppData.FunCfgs.CurBankA = _rxBuffer[50] % 8;
                            AppData.FunCfgs.CurBankB = _rxBuffer[51] % 8;
                            AppData.FunCfgs.BluetoothAudioGain = _rxBuffer[53] % 5;
                            AppData.FunCfgs.BtMicGain = _rxBuffer[54] % 5;
                            var num3 = 0;
                            for (var j = 0; j < 6 && _rxBuffer[56 + j] != byte.MaxValue; j++) num3++;

                            AppData.FunCfgs.CallSign = Encoding.GetEncoding("gb2312").GetString(_rxBuffer, 56, num3);
                        }
                        else if (num >= 40960 && num <= 41216)
                        {
                            switch (num)
                            {
                                case 40960:
                                    AppData.Dtmfs.LocalId = SetDtmfWord(_rxBuffer, 4);
                                    AppData.Dtmfs.Pttid = _rxBuffer[10];
                                    AppData.Dtmfs.WordTime = _rxBuffer[11];
                                    AppData.Dtmfs.IdleTime = _rxBuffer[12];
                                    AppData.Dtmfs.Group[0] = SetDtmfWord(_rxBuffer, 36);
                                    AppData.Dtmfs.Group[1] = SetDtmfWord(_rxBuffer, 52);
                                    break;
                                case 41024:
                                    AppData.Dtmfs.Group[2] = SetDtmfWord(_rxBuffer, 4);
                                    AppData.Dtmfs.Group[3] = SetDtmfWord(_rxBuffer, 20);
                                    AppData.Dtmfs.Group[4] = SetDtmfWord(_rxBuffer, 36);
                                    AppData.Dtmfs.Group[5] = SetDtmfWord(_rxBuffer, 52);
                                    break;
                                case 41088:
                                    AppData.Dtmfs.Group[6] = SetDtmfWord(_rxBuffer, 4);
                                    AppData.Dtmfs.Group[7] = SetDtmfWord(_rxBuffer, 20);
                                    AppData.Dtmfs.Group[8] = SetDtmfWord(_rxBuffer, 36);
                                    AppData.Dtmfs.Group[9] = SetDtmfWord(_rxBuffer, 52);
                                    break;
                                case 41152:
                                    AppData.Dtmfs.Group[10] = SetDtmfWord(_rxBuffer, 4);
                                    AppData.Dtmfs.Group[11] = SetDtmfWord(_rxBuffer, 20);
                                    AppData.Dtmfs.Group[12] = SetDtmfWord(_rxBuffer, 36);
                                    AppData.Dtmfs.Group[13] = SetDtmfWord(_rxBuffer, 52);
                                    break;
                                case 41216:
                                    AppData.Dtmfs.Group[14] = SetDtmfWord(_rxBuffer, 4);
                                    break;
                            }
                        }
                        else
                        {
                            switch (num)
                            {
                                case 41472:
                                    AppData.BankName[0] = SetBankName(_rxBuffer, 4);
                                    AppData.BankName[1] = SetBankName(_rxBuffer, 20);
                                    AppData.BankName[2] = SetBankName(_rxBuffer, 36);
                                    AppData.BankName[3] = SetBankName(_rxBuffer, 52);
                                    break;
                                case 41536:
                                    AppData.BankName[4] = SetBankName(_rxBuffer, 4);
                                    AppData.BankName[5] = SetBankName(_rxBuffer, 20);
                                    AppData.BankName[6] = SetBankName(_rxBuffer, 36);
                                    AppData.BankName[7] = SetBankName(_rxBuffer, 52);
                                    break;
                                case 45056:
                                {
                                    AppData.Fms.CurFreq = SetFmFreq(_rxBuffer, 4);
                                    for (var k = 0; k < 30; k++)
                                        AppData.Fms.Channels[k] = SetFmFreq(_rxBuffer, 6 + k * 2);

                                    break;
                                }
                            }
                        }

                        if (num < 45056)
                        {
                            num += 64;
                            switch (num)
                            {
                                case 16448:
                                    num = 32768;
                                    break;
                                case 32832:
                                    num = 36864;
                                    break;
                                case 36928:
                                    num = 40960;
                                    break;
                            }

                            _timer.Start();
                            _step = Step.StepRead1;
                            break;
                        }

                        _progressVal = 100;
                        _progressCont = "完成";
                        StatusQueue.Enqueue(new ProgressBarValue(_progressVal, _progressCont));
                        array2 = new byte[1] { 69 };
                        _port.WriteByte(array2, 0, array2.Length);
                        _flagTransmitting = false;
                        return true;
                    }
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
                _port.WriteByte(array2, 0, array2.Length);
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
        var num = chNum / 64;
        var num2 = chNum % 64;
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
        // Fix #28: power has 3 choices!
        AppData.ChannelList[num][num2].TxPower = dat[14] % 3;
        AppData.ChannelList[num][num2].Bandwide = (dat[15] >> 6) & 1;
        AppData.ChannelList[num][num2].BusyLock = (dat[15] >> 3) & 1;
        AppData.ChannelList[num][num2].ScanAdd = (dat[15] >> 2) & 1;
        if (dat[20] != byte.MaxValue)
        {
            var num3 = 0;
            for (var i = 0; i < 12 && dat[20 + i] != byte.MaxValue; i++) num3++;

            AppData.ChannelList[num][num2].Name = Encoding.GetEncoding("gb2312").GetString(dat, 20, num3);
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
            for (var i = 0; i < 8; i++)
            {
                num *= 10;
                num += dat[i];
            }

            return num.ToString().Insert(3, ".");
        }
        catch
        {
            return "";
        }
    }

    private string CaculateOffset(byte[] dat, int offset)
    {
        var text = "00.0000";
        var num = 0;
        try
        {
            for (var i = 0; i < 7; i++)
            {
                num *= 10;
                num += dat[i + offset];
            }

            return num.ToString("D7").Insert(3, ".");
        }
        catch
        {
            return "000.0000";
        }
    }

    private void SetVfoaInfos(byte[] dat)
    {
        var text = CaculateFreq(dat);
        if (text != "") AppData.Vfos.VfoAFreq = text;

        AppData.Vfos.StrVfoaRxCtsDcs = CaculateCtsDcs(dat, 8);
        AppData.Vfos.StrVfoaTxCtsDcs = CaculateCtsDcs(dat, 10);
        AppData.Vfos.VfoABusyLock = dat[13] % 2;
        AppData.Vfos.VfoASignalGroup = (dat[14] & 0xF) % 16;
        AppData.Vfos.VfoADir = ((dat[14] >> 4) & 3) % 3;
        // Fix #28: power has 3 choices!
        AppData.Vfos.VfoATxPower = (dat[16] & 0xF) % 3;
        AppData.Vfos.VfoAScram = ((dat[16] >> 4) & 0xF) % 9;
        AppData.Vfos.VfoABandwide = (dat[17] >> 6) & 1;
        AppData.Vfos.VfoAStep = dat[19] % 8;
        AppData.Vfos.VfoAOffset = CaculateOffset(dat, 20);
    }

    private void SetVfobInfos(byte[] dat)
    {
        var text = CaculateFreq(dat);
        if (text != "") AppData.Vfos.VfoBFreq = text;

        AppData.Vfos.StrVfobRxCtsDcs = CaculateCtsDcs(dat, 8);
        AppData.Vfos.StrVfobTxCtsDcs = CaculateCtsDcs(dat, 10);
        AppData.Vfos.VfoBBusyLock = dat[13] % 2;
        AppData.Vfos.VfoBSignalGroup = (dat[14] & 0xF) % 16;
        AppData.Vfos.VfoBDir = ((dat[14] >> 4) & 3) % 3;
        // Fix #28: power has 3 choices!
        AppData.Vfos.VfoBTxPower = (dat[16] & 0xF) % 3;
        AppData.Vfos.VfoBScram = ((dat[16] >> 4) & 0xF) % 9;
        AppData.Vfos.VfoBBandwide = (dat[17] >> 6) & 1;
        AppData.Vfos.VfoBStep = dat[19] % 8;
        AppData.Vfos.VfoBOffset = CaculateOffset(dat, 20);
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
            for (var i = 0; i < 6 && payload[offset + i] != byte.MaxValue; i++) num++;

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