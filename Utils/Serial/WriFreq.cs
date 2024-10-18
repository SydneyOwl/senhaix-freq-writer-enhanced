using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Timers;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.DataModels.Shx8x00;
using Timer = System.Timers.Timer;


namespace SenhaixFreqWriter.Utils.Serial;

//别想着尝试重构这个...因为我试过

internal class WriFreq
{
    private static readonly ushort ModeltypeWlt = 0;

    private static readonly ushort ModeltypeShx = 1;

    private static readonly ushort ModeltypeBfHx = 2;

    private readonly byte[] _bufForData = new byte[128];

    private readonly OtherImfData _cfgData;

    private readonly byte[] _hSTable1 = new byte[7] { 80, 82, 79, 71, 82, 79, 77 };

    private readonly byte[][] _hStable2ModelType = new byte[3][]
    {
        new byte[3] { 87, 76, 84 },
        new byte[3] { 83, 72, 88 },
        new byte[3] { 66, 70, 72 }
    };

    private readonly ushort _modeltype = ModeltypeShx;

    private readonly OperationType _op;

    private readonly MySerialPort _sP;

    private readonly string[] _tableQt = new string[210]
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

    private readonly ClassTheRadioData _theRadioData;

    private bool _flagRetry;

    private int _numOfChannel = 0;

    private State _state = State.HandShakeStep1;

    private Timer _timer;

    private byte _timesOfRetry = 5;

    public ConcurrentQueue<State> CurrentProgress = new();

    public ushort EepAddr;

    public bool FlagTransmitting;

    public WriFreq(MySerialPort sP, ClassTheRadioData theRadioData, OperationType op)
    {
        _op = op;
        _sP = sP;
        _theRadioData = theRadioData;
        TimerInit();
    }

    public WriFreq(MySerialPort sP, OtherImfData data, OperationType op)
    {
        _op = op;
        _sP = sP;
        _cfgData = data;
        TimerInit();
    }

    public State State
    {
        get => _state;
        set
        {
            _state = value;
            CurrentProgress.Enqueue(value);
        }
    }

    private void TimerInit()
    {
        _timer = new Timer();
        _timer.Interval = 1000.0;
        _timer.Elapsed += Timer_Elapsed;
        _timer.AutoReset = true;
        _timer.Enabled = true;
    }

    private void ResetRetryCount()
    {
        _timesOfRetry = 5;
        _flagRetry = false;
    }

    public bool DoIt(CancellationToken cancellationToken)
    {
        FlagTransmitting = true;
        State = State.HandShakeStep1;
        if (HandShake(cancellationToken))
        {
            if (_op == OperationType.Read)
            {
                if (ReadChData(cancellationToken))
                {
                    _sP.CloseSerial();
                    return true;
                }

                _sP.CloseSerial();
                return false;
            }

            if (OperationType.Write == _op)
            {
                if (WriteChData(cancellationToken))
                {
                    _sP.CloseSerial();
                    MySerialPort.GetInstance().WriteBle = null;
                    return true;
                }

                MySerialPort.GetInstance().WriteBle = null;
                _sP.CloseSerial();
                return false;
            }

            if (OperationType.ReadConfig == _op)
            {
                if (ReadConfigData(cancellationToken))
                {
                    _sP.CloseSerial();
                    return true;
                }

                _sP.CloseSerial();
                return false;
            }

            if (OperationType.WriteConfig == _op)
            {
                if (WriteConfigData(cancellationToken))
                {
                    _sP.CloseSerial();
                    return true;
                }

                _sP.CloseSerial();
                return false;
            }

            _sP.CloseSerial();
            return false;
        }

        _sP.CloseSerial();
        return false;
    }

    private bool HandShake(CancellationToken cancellationToken)
    {
        while (FlagTransmitting && !cancellationToken.IsCancellationRequested)
            if (!_flagRetry)
            {
                switch (State)
                {
                    case State.HandShakeStep1:
                        _sP.WriteByte(_hSTable1, 0, _hSTable1.Length);
                        _sP.WriteByte(_hStable2ModelType[_modeltype], 0, _hStable2ModelType[_modeltype].Length);
                        _sP.WriteByte(85);
                        _timer.Start();
                        ResetRetryCount();
                        State = State.HandShakeStep2;
                        break;
                    case State.HandShakeStep2:
                        if (_sP.BytesToReadFromCache >= 1)
                        {
                            _sP.ReadByte(_bufForData, 0, 1);
                            if (_bufForData[0] == 6)
                            {
                                ResetRetryCount();
                                _sP.WriteByte(70);
                                State = State.HandShakeStep3;
                            }
                        }

                        break;
                    case State.HandShakeStep3:
                        if (_sP.BytesToReadFromCache >= 8)
                        {
                            _sP.ReadByte(_bufForData, 0, 8);
                            _timer.Stop();
                            ResetRetryCount();
                            if (_op == OperationType.Read || _op == OperationType.ReadConfig)
                                State = State.ReadStep1;
                            else if (OperationType.Write == _op || _op == OperationType.WriteConfig)
                                State = State.WriteStep1;
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
                    FlagTransmitting = false;
                    return false;
                }

                _timesOfRetry--;
                _flagRetry = false;
                switch (State)
                {
                    case State.HandShakeStep2:
                        State = State.HandShakeStep1;
                        break;
                    case State.HandShakeStep3:
                        _sP.WriteByte(70);
                        break;
                }
            }

        return false;
    }


    private bool ReadChData(CancellationToken cancellationToken)
    {
        var array = new byte[4] { 82, 0, 0, 64 };
        var num = 0;
        while (FlagTransmitting && !cancellationToken.IsCancellationRequested)
            if (!_flagRetry)
            {
                switch (State)
                {
                    case State.ReadStep1:
                        _sP.WriteByte(array, 0, 4);
                        State = State.ReadStep2;
                        _timer.Start();
                        break;
                    case State.ReadStep2:
                    {
                        if (_sP.BytesToReadFromCache < array[3] + 4) break;
                        _timer.Stop();
                        ResetRetryCount();
                        _sP.ReadByte(_bufForData, 0, array[3] + 4);
                        if (_bufForData[1] != array[1] || _bufForData[2] != array[2]) break;
                        var array2 = new byte[4][]
                        {
                            new byte[16],
                            new byte[16],
                            new byte[16],
                            new byte[16]
                        };
                        for (var i = 0; i < 16; i++)
                        {
                            array2[0][i] = _bufForData[i + 4];
                            array2[1][i] = _bufForData[i + 4 + 16];
                            array2[2][i] = _bufForData[i + 4 + 32];
                            array2[3][i] = _bufForData[i + 4 + 48];
                        }

                        if (EepAddr >= 0 && EepAddr < 2048)
                        {
                            GetCHImfo_HexToStr(_theRadioData, num++, array2[0]);
                            GetCHImfo_HexToStr(_theRadioData, num++, array2[1]);
                            GetCHImfo_HexToStr(_theRadioData, num++, array2[2]);
                            GetCHImfo_HexToStr(_theRadioData, num++, array2[3]);
                        }
                        else if (EepAddr == 2048)
                        {
                            num = 0;
                        }
                        else if (EepAddr >= 3072 && EepAddr < 5120)
                        {
                            _theRadioData.ChanneldataList[num++].ChangeByNum(12, GetTheNameOfCh(array2[0]));
                            _theRadioData.ChanneldataList[num++].ChangeByNum(12, GetTheNameOfCh(array2[1]));
                            _theRadioData.ChanneldataList[num++].ChangeByNum(12, GetTheNameOfCh(array2[2]));
                            _theRadioData.ChanneldataList[num++].ChangeByNum(12, GetTheNameOfCh(array2[3]));
                        }
                        else if (EepAddr == 6656)
                        {
                            GetFunCfgImf_Part1(_theRadioData, array2[0]);
                            GetFunCfgImf_Part2(_theRadioData, array2[1]);
                            if (array2[2][0] == byte.MaxValue) array2[2][0] = 5;
                            _theRadioData.FunCfgData.CbBVoxDelay = array2[2][0];
                            if (array2[2][1] == byte.MaxValue) array2[2][1] = 1;
                            _theRadioData.FunCfgData.CbBTimerMenuQuit = array2[2][1];
                            if (array2[2][2] == byte.MaxValue) array2[2][2] = 1;
                            _theRadioData.FunCfgData.CbBMicGain = array2[2][2];
                        }
                        else if (EepAddr == 6720)
                        {
                            GetVFO_A_Parameter(_theRadioData, array2[0], array2[1]);
                            GetVFO_B_Parameter(_theRadioData, array2[2], array2[3]);
                        }
                        else if (EepAddr == 6784)
                        {
                            var array3 = new byte[5] { 7, 10, 5, 28, 29 };
                            var num2 = -1;
                            for (var j = 0; j < 5; j++)
                                if (array2[0][0] == array3[j])
                                {
                                    num2 = j;
                                    break;
                                }

                            if (num2 != -1) _theRadioData.FunCfgData.CbBKeySide = num2;
                            num2 = -1;
                            for (var k = 0; k < 5; k++)
                                if (array2[0][1] == array3[k])
                                {
                                    num2 = k;
                                    break;
                                }

                            if (num2 != -1) _theRadioData.FunCfgData.CbBKeySideL = num2;
                        }
                        else if (EepAddr >= 6912 && EepAddr < 7136)
                        {
                            switch (EepAddr)
                            {
                                case 6912:
                                    _theRadioData.DtmfData.GroupOfDtmf1 = ConvertHex2Str_DTMF(array2[0]);
                                    _theRadioData.DtmfData.GroupOfDtmf2 = ConvertHex2Str_DTMF(array2[1]);
                                    _theRadioData.DtmfData.GroupOfDtmf3 = ConvertHex2Str_DTMF(array2[2]);
                                    _theRadioData.DtmfData.GroupOfDtmf4 = ConvertHex2Str_DTMF(array2[3]);
                                    break;
                                case 6976:
                                    _theRadioData.DtmfData.GroupOfDtmf5 = ConvertHex2Str_DTMF(array2[0]);
                                    _theRadioData.DtmfData.GroupOfDtmf6 = ConvertHex2Str_DTMF(array2[1]);
                                    _theRadioData.DtmfData.GroupOfDtmf7 = ConvertHex2Str_DTMF(array2[2]);
                                    _theRadioData.DtmfData.GroupOfDtmf8 = ConvertHex2Str_DTMF(array2[3]);
                                    break;
                                case 7040:
                                    _theRadioData.DtmfData.GroupOfDtmf9 = ConvertHex2Str_DTMF(array2[0]);
                                    _theRadioData.DtmfData.GroupOfDtmfA = ConvertHex2Str_DTMF(array2[1]);
                                    _theRadioData.DtmfData.GroupOfDtmfB = ConvertHex2Str_DTMF(array2[2]);
                                    _theRadioData.DtmfData.GroupOfDtmfC = ConvertHex2Str_DTMF(array2[3]);
                                    break;
                                case 7104:
                                    _theRadioData.DtmfData.GroupOfDtmfD = ConvertHex2Str_DTMF(array2[0]);
                                    _theRadioData.DtmfData.GroupOfDtmfE = ConvertHex2Str_DTMF(array2[1]);
                                    _theRadioData.DtmfData.GroupOfDtmfF = ConvertHex2Str_DTMF(array2[2]);
                                    _theRadioData.DtmfData.TheIdOfLocalHost = GetTheLocalId(array2[3]);
                                    _theRadioData.DtmfData.GroupCall = array2[3][5];
                                    _theRadioData.DtmfData.SendOnPttPressed = GetTheBoolOfSendIDOnPTTPress(array2[3]);
                                    _theRadioData.DtmfData.SendOnPttReleased =
                                        GetTheBoolOfSendIDOnPTTRelease(array2[3]);
                                    _theRadioData.DtmfData.LastTimeSend = array2[3][7];
                                    _theRadioData.DtmfData.LastTimeStop = array2[3][8];
                                    break;
                            }
                        }

                        if (EepAddr < 7168)
                        {
                            EepAddr += array[3];
                            if (EepAddr == 2112)
                                EepAddr = 3072;
                            else if (EepAddr == 5120) EepAddr = 6656;
                            array[1] = (byte)(EepAddr >> 8);
                            array[2] = (byte)EepAddr;
                            _timer.Start();
                            State = State.ReadStep1;
                            break;
                        }

                        _sP.WriteByte(69);
                        FlagTransmitting = false;
                        return true;
                    }
                }
            }
            else
            {
                if (_timesOfRetry <= 0)
                {
                    FlagTransmitting = false;
                    return false;
                }

                _timesOfRetry--;
                _flagRetry = false;
            }

        return false;
    }

    private bool WriteChData(CancellationToken cancellationToken)
    {
        var array = new byte[36]
        {
            87, 0, 0, 32, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255
        };
        var array2 = new byte[36]
        {
            87, 0, 0, 32, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255
        };
        var num = 0;
        while (FlagTransmitting && !cancellationToken.IsCancellationRequested)
            if (!_flagRetry)
            {
                switch (State)
                {
                    case State.WriteStep1:
                        if (EepAddr < 2048)
                        {
                            var cHImfStrToHex = GetCHImf_StrToHex(_theRadioData.ChanneldataList[num++].TransList());
                            var cHImfStrToHex2 = GetCHImf_StrToHex(_theRadioData.ChanneldataList[num++].TransList());
                            for (var j = 0; j < 16; j++)
                            {
                                array[j + 4] = cHImfStrToHex[j];
                                array[j + 20] = cHImfStrToHex2[j];
                            }
                        }
                        else if (EepAddr == 2048)
                        {
                            num = 0;
                        }
                        else if (EepAddr >= 3072 && EepAddr < 5120)
                        {
                            var array3 = SetChNameToHex(_theRadioData.ChanneldataList[num++].TransList());
                            var array4 = SetChNameToHex(_theRadioData.ChanneldataList[num++].TransList());
                            for (var k = 0; k < 16; k++)
                            {
                                array[k + 4] = array3[k];
                                array[k + 20] = array4[k];
                            }
                        }
                        else if (EepAddr == 6656)
                        {
                            array[4] = (byte)_theRadioData.FunCfgData.CbBSql;
                            array[5] = (byte)_theRadioData.FunCfgData.CbBSaveMode;
                            array[6] = (byte)_theRadioData.FunCfgData.CbBVox;
                            array[7] = (byte)_theRadioData.FunCfgData.CbBAutoBackLight;
                            if (_theRadioData.FunCfgData.CBTdr)
                                array[8] = 1;
                            else
                                array[8] = 0;
                            array[9] = (byte)_theRadioData.FunCfgData.CbBTot;
                            if (_theRadioData.FunCfgData.CBSoundOfBi)
                                array[10] = 1;
                            else
                                array[10] = 0;
                            array[11] = (byte)_theRadioData.FunCfgData.CbBVoicSwitch;
                            array[12] = (byte)_theRadioData.FunCfgData.CbBLanguage;
                            array[13] = (byte)_theRadioData.FunCfgData.CbBDtmf;
                            array[14] = (byte)_theRadioData.FunCfgData.CbBScan;
                            array[15] = (byte)_theRadioData.FunCfgData.CbBPttid;
                            array[16] = (byte)_theRadioData.FunCfgData.CbBSendIdDelay;
                            array[17] = (byte)_theRadioData.FunCfgData.CbBChADisplayMode;
                            array[18] = (byte)_theRadioData.FunCfgData.CbBChBDisplayMode;
                            if (_theRadioData.FunCfgData.CBStopSendOnBusy)
                                array[19] = 1;
                            else
                                array[19] = 0;
                            if (_theRadioData.FunCfgData.CBAutoLock)
                                array[20] = 1;
                            else
                                array[20] = 0;
                            array[21] = (byte)_theRadioData.FunCfgData.CbBAlarmMode;
                            if (_theRadioData.FunCfgData.CBAlarmSound)
                                array[22] = 1;
                            else
                                array[22] = 0;
                            array[23] = (byte)_theRadioData.FunCfgData.CbBTxUnderTdrStart;
                            array[24] = (byte)_theRadioData.FunCfgData.CbBTailNoiseClear;
                            array[25] = (byte)_theRadioData.FunCfgData.CbBPassRptNoiseClear;
                            array[26] = (byte)_theRadioData.FunCfgData.CbBPassRptNoiseDetect;
                            array[27] = (byte)_theRadioData.FunCfgData.CbBSoundOfTxEnd;
                            array[28] = 0;
                            if (_theRadioData.FunCfgData.CBFmRadioEnable)
                                array[29] = 0;
                            else
                                array[29] = 1;
                            array[30] = (byte)_theRadioData.FunCfgData.CbBWorkModeB;
                            array[30] <<= 4;
                            array[30] = (byte)(array[30] | _theRadioData.FunCfgData.CbBWorkModeA);
                            if (_theRadioData.FunCfgData.CBLockKeyBoard)
                                array[31] = 1;
                            else
                                array[31] = 0;
                            array[32] = (byte)_theRadioData.FunCfgData.CbBPowerOnMsg;
                            array[33] = 0;
                            // 8800和pro此项始终为0
                            array[34] = (byte)_theRadioData.FunCfgData.CbB1750Hz;
                            array[35] = 128;
                        }
                        else if (EepAddr == 6688)
                        {
                            array[4] = (byte)_theRadioData.FunCfgData.CbBVoxDelay;
                            array[5] = (byte)_theRadioData.FunCfgData.CbBTimerMenuQuit;
                            array[6] = (byte)_theRadioData.FunCfgData.CbBMicGain;
                        }
                        else if (EepAddr == 6720)
                        {
                            var array5 = SetDataVfoFreq(_theRadioData.FunCfgData.TBaCurFreq);
                            var array6 = SetDataVfoqt(_theRadioData.FunCfgData.CbBaRxQt,
                                _theRadioData.FunCfgData.CbBaTxQt);
                            var array7 = SetDataVfoCfgA(_theRadioData);
                            var array8 = SetDataVfoRemainFreq(_theRadioData.FunCfgData.TBaRemainFreq);
                            for (var l = 0; l < 4; l++)
                            {
                                array[l + 4] = array5[l];
                                array[l + 8] = array5[l + 4];
                                array[l + 12] = array6[l];
                            }

                            for (var m = 0; m < 6; m++) array[m + 18] = array7[m];
                            for (var n = 0; n < 7; n++) array[n + 24] = array8[n];
                        }
                        else if (EepAddr == 6752)
                        {
                            var array9 = SetDataVfoFreq(_theRadioData.FunCfgData.TBbCurFreq);
                            var array10 = SetDataVfoqt(_theRadioData.FunCfgData.CbBbRxQt,
                                _theRadioData.FunCfgData.CbBbTxQt);
                            var array11 = SetDataVfoCfgB(_theRadioData);
                            var array12 = SetDataVfoRemainFreq(_theRadioData.FunCfgData.TBbRemainFreq);
                            for (var num2 = 0; num2 < 4; num2++)
                            {
                                array[num2 + 4] = array9[num2];
                                array[num2 + 8] = array9[num2 + 4];
                                array[num2 + 12] = array10[num2];
                            }

                            for (var num3 = 0; num3 < 6; num3++) array[num3 + 18] = array11[num3];
                            for (var num4 = 0; num4 < 7; num4++) array[num4 + 24] = array12[num4];
                        }
                        else if (EepAddr == 6784)
                        {
                            var array13 = new byte[5] { 7, 10, 5, 28, 29 };
                            array[4] = array13[_theRadioData.FunCfgData.CbBKeySide];
                            array[5] = array13[_theRadioData.FunCfgData.CbBKeySideL];
                        }
                        else if (EepAddr >= 6912 && EepAddr <= 7136)
                        {
                            switch (EepAddr)
                            {
                                case 6912:
                                {
                                    var array14 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmf1);
                                    var array15 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmf2);
                                    for (var num8 = 0; num8 < 16; num8++)
                                    {
                                        array[num8 + 4] = array14[num8];
                                        array[num8 + 20] = array15[num8];
                                    }

                                    break;
                                }
                                case 6944:
                                {
                                    var array14 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmf3);
                                    var array15 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmf4);
                                    for (var num9 = 0; num9 < 16; num9++)
                                    {
                                        array[num9 + 4] = array14[num9];
                                        array[num9 + 20] = array15[num9];
                                    }

                                    break;
                                }
                                case 6976:
                                {
                                    var array14 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmf5);
                                    var array15 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmf6);
                                    for (var num13 = 0; num13 < 16; num13++)
                                    {
                                        array[num13 + 4] = array14[num13];
                                        array[num13 + 20] = array15[num13];
                                    }

                                    break;
                                }
                                case 7008:
                                {
                                    var array14 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmf7);
                                    var array15 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmf8);
                                    for (var num7 = 0; num7 < 16; num7++)
                                    {
                                        array[num7 + 4] = array14[num7];
                                        array[num7 + 20] = array15[num7];
                                    }

                                    break;
                                }
                                case 7040:
                                {
                                    var array14 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmf9);
                                    var array15 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmfA);
                                    for (var num12 = 0; num12 < 16; num12++)
                                    {
                                        array[num12 + 4] = array14[num12];
                                        array[num12 + 20] = array15[num12];
                                    }

                                    break;
                                }
                                case 7072:
                                {
                                    var array14 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmfB);
                                    var array15 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmfC);
                                    for (var num10 = 0; num10 < 16; num10++)
                                    {
                                        array[num10 + 4] = array14[num10];
                                        array[num10 + 20] = array15[num10];
                                    }

                                    break;
                                }
                                case 7104:
                                {
                                    var array14 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmfD);
                                    var array15 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmfE);
                                    for (var num11 = 0; num11 < 16; num11++)
                                    {
                                        array[num11 + 4] = array14[num11];
                                        array[num11 + 20] = array15[num11];
                                    }

                                    break;
                                }
                                case 7136:
                                {
                                    var array14 = SetDtmftoHex(_theRadioData.DtmfData.GroupOfDtmfF);
                                    for (var num5 = 0; num5 < 16; num5++) array[num5 + 4] = array14[num5];
                                    if (_theRadioData.DtmfData.TheIdOfLocalHost != "")
                                    {
                                        var theIdOfLocalHost = _theRadioData.DtmfData.TheIdOfLocalHost;
                                        for (var num6 = 0; num6 < theIdOfLocalHost.Length; num6++)
                                            array[num6 + 20] = byte.Parse(theIdOfLocalHost[num6].ToString() ?? "");
                                    }

                                    array[25] = (byte)_theRadioData.DtmfData.GroupCall;
                                    array[26] = 0;
                                    if (_theRadioData.DtmfData.SendOnPttPressed) array[26] |= 1;
                                    if (_theRadioData.DtmfData.SendOnPttReleased) array[26] |= 2;
                                    array[27] = (byte)_theRadioData.DtmfData.LastTimeSend;
                                    array[28] = (byte)_theRadioData.DtmfData.LastTimeStop;
                                    break;
                                }
                            }
                        }

                        _sP.WriteByte(array, 0, array[3] + 4);
                        _timer.Start();
                        State = State.WriteStep2;
                        break;
                    case State.WriteStep2:

                        if (_sP.BytesToReadFromCache < 1) break;
                        _sP.ReadByte(_bufForData, 0, _sP.BytesToReadFromCache);
                        if (_bufForData[0] == 6)
                        {
                            _timer.Stop();
                            ResetRetryCount();
                            if (EepAddr >= 7168)
                            {
                                FlagTransmitting = false;
                                return true;
                            }

                            EepAddr += array[3];
                            if (EepAddr == 2080)
                                EepAddr = 3072;
                            else if (EepAddr == 5120) EepAddr = 6656;
                            for (var i = 0; i < 36; i++) array[i] = array2[i];
                            array[1] = (byte)(EepAddr >> 8);
                            array[2] = (byte)EepAddr;
                            State = State.WriteStep1;
                        }

                        break;
                }
            }
            else
            {
                // DebugWindow.GetInstance().updateDebugContent("Write kk5455");
                if (_timesOfRetry <= 0)
                {
                    FlagTransmitting = false;
                    return false;
                }

                _timesOfRetry--;
                num = 0;
                _flagRetry = false;
            }

        return false;
    }

    private bool ReadConfigData(CancellationToken cancellationToken)
    {
        var array = new byte[4] { 83, 31, 192, 64 };
        EepAddr = 8128;
        while (FlagTransmitting && !cancellationToken.IsCancellationRequested)
            if (!_flagRetry)
            {
                switch (State)
                {
                    case State.ReadStep1:
                        _sP.WriteByte(array, 0, 4);
                        State = State.ReadStep2;
                        _timer.Start();
                        break;
                    case State.ReadStep2:

                        if (_sP.BytesToReadFromCache < array[3] + 4) break;
                        _timer.Stop();
                        ResetRetryCount();
                        _sP.ReadByte(_bufForData, 0, _sP.BytesToReadFromCache);
                        if (_bufForData[1] != array[1] || _bufForData[2] != array[2]) break;
                        if (EepAddr == 8128)
                        {
                            var array2 = new byte[4][];
                            for (var i = 0; i < 4; i++)
                            {
                                array2[i] = new byte[16];
                                for (var j = 0; j < 16; j++) array2[i][j] = _bufForData[4 + i * 16 + j];
                            }

                            _cfgData.TheMinFreqOfVhf =
                                (HexToInt(array2[0][1]) * 100 + HexToInt(array2[0][2])).ToString();
                            _cfgData.TheMaxFreqOfVhf =
                                (HexToInt(array2[0][3]) * 100 + HexToInt(array2[0][4])).ToString();
                            _cfgData.TheMinFreqOfUhf =
                                (HexToInt(array2[0][6]) * 100 + HexToInt(array2[0][7])).ToString();
                            _cfgData.TheMaxFreqOfUhf =
                                (HexToInt(array2[0][8]) * 100 + HexToInt(array2[0][9])).ToString();
                            if (_cfgData.TheMinFreqOfUhf == "433")
                                _cfgData.TheRangeOfUhf = 1;
                            else
                                _cfgData.TheRangeOfUhf = 0;
                            if (_cfgData.TheMaxFreqOfVhf == "174")
                                _cfgData.TheRangeOfVhf = 0;
                            else if (_cfgData.TheMaxFreqOfVhf == "149")
                                _cfgData.TheRangeOfVhf = 1;
                            else if (_cfgData.TheMaxFreqOfVhf == "260")
                                _cfgData.TheRangeOfVhf = 2;
                            else
                                _cfgData.TheRangeOfVhf = 3;
                            if ((array2[0][10] & 1) == 1)
                                _cfgData.EnableTxOver480M = true;
                            else
                                _cfgData.EnableTxOver480M = false;
                        }
                        else if (EepAddr == 7872)
                        {
                            var array3 = new byte[4][];
                            for (var k = 0; k < 4; k++)
                            {
                                array3[k] = new byte[16];
                                for (var l = 0; l < 16; l++) array3[k][l] = _bufForData[4 + k * 16 + l];
                            }

                            var text = "";
                            var aSciiEncoding = new ASCIIEncoding();
                            for (var m = 0; m < 7 && array3[2][m] != byte.MaxValue; m++)
                                text += aSciiEncoding.GetString(array3[2], m, 1);
                            _cfgData.PowerUpChar1 = text;
                            text = "";
                            for (var n = 7; n < 14 && array3[2][n] != byte.MaxValue; n++)
                                text += aSciiEncoding.GetString(array3[2], n, 1);
                            _cfgData.PowerUpChar2 = text;
                        }

                        _timer.Start();
                        _sP.WriteByte(6);
                        if (EepAddr == 8128)
                        {
                            EepAddr = 7872;
                            array[1] = (byte)(EepAddr >> 8);
                            array[2] = (byte)EepAddr;
                            _sP.WriteByte(array, 0, 4);
                            State = State.ReadStep3;
                            break;
                        }

                        FlagTransmitting = false;
                        return true;
                    case State.ReadStep3:

                        if (_sP.BytesToReadFromCache >= 1)
                        {
                            _timer.Stop();
                            ResetRetryCount();
                            _sP.ReadByte(_bufForData, 0, 1);
                            if (_bufForData[0] == 6) State = State.ReadStep2;
                        }

                        break;
                }
            }
            else
            {
                if (_timesOfRetry <= 0)
                {
                    FlagTransmitting = false;
                    return false;
                }

                _timesOfRetry--;
                _flagRetry = false;
            }

        return false;
    }

    private bool WriteConfigData(CancellationToken cancellationToken)
    {
        var array = new byte[20]
        {
            88, 31, 192, 16, 1, 1, 54, 1, 116, 1,
            4, 0, 4, 121, 0, 14, 15, 16, 17, 21
        };
        var array2 = new byte[20]
        {
            88, 30, 224, 16, 32, 32, 32, 32, 32, 32,
            32, 32, 32, 32, 32, 32, 32, 32, 0, 0
        };
        byte[] array3 = null;
        EepAddr = 8128;
        while (FlagTransmitting && !cancellationToken.IsCancellationRequested)
            if (!_flagRetry)
            {
                switch (State)
                {
                    case State.WriteStep1:
                        if (EepAddr == 8128)
                        {
                            ushort num = 0;
                            num = ushort.Parse(_cfgData.TheMinFreqOfVhf);
                            array[5] = (byte)(num / 100);
                            array[6] = (byte)(num % 100 / 10 * 16 + num % 10);
                            num = ushort.Parse(_cfgData.TheMaxFreqOfVhf);
                            array[7] = (byte)(num / 100);
                            array[8] = (byte)(num % 100 / 10 * 16 + num % 10);
                            num = ushort.Parse(_cfgData.TheMinFreqOfUhf);
                            array[10] = (byte)(num / 100);
                            array[11] = (byte)(num % 100 / 10 * 16 + num % 10);
                            num = ushort.Parse(_cfgData.TheMaxFreqOfUhf);
                            array[12] = (byte)(num / 100);
                            array[13] = (byte)(num % 100 / 10 * 16 + num % 10);
                            if (_cfgData.EnableTxOver480M)
                                array[14] = 1;
                            else
                                array[14] = 0;
                            array3 = array;
                        }
                        else if (EepAddr == 7904)
                        {
                            var powerUpChar = _cfgData.PowerUpChar1;
                            var powerUpChar2 = _cfgData.PowerUpChar2;
                            if (powerUpChar == null || powerUpChar == "")
                            {
                                for (var i = 4; i < 11; i++) array2[i] = 32;
                            }
                            else
                            {
                                var num2 = 7 - _cfgData.PowerUpChar1.Length;
                                var num3 = num2 / 2;
                                var bytes = Encoding.ASCII.GetBytes(_cfgData.PowerUpChar1);
                                for (var j = 4 + num3; j < 4 + num3 + _cfgData.PowerUpChar1.Length; j++)
                                    array2[j] = bytes[j - 4 - num3];
                                num2 = 7 - _cfgData.PowerUpChar2.Length;
                                num3 = num2 / 2;
                                bytes = Encoding.ASCII.GetBytes(_cfgData.PowerUpChar2);
                                for (var k = 11 + num3; k < 11 + num3 + _cfgData.PowerUpChar2.Length; k++)
                                    array2[k] = bytes[k - 11 - num3];
                                array3 = array2;
                            }
                        }
                        else
                        {
                            ushort num4 = 0;
                            num4 = ushort.Parse(_cfgData.TheMinFreqOfVhf);
                            if (num4 == 136)
                                array3[4] = 0;
                            else
                                array3[4] = 85;
                            array3[3] = 1;
                        }

                        _sP.WriteByte(array3, 0, array3[3] + 4);
                        _sP.WriteByte(array3, 0, array3[3] + 4);
                        _timer.Start();
                        State = State.WriteStep2;
                        break;
                    case State.WriteStep2:
                        if (_sP.BytesToReadFromCache < 1) break;
                        _sP.ReadByte(_bufForData, 0, _sP.BytesToReadFromCache);
                        if (_bufForData[0] != 6) break;
                        _timer.Stop();
                        ResetRetryCount();
                        if (EepAddr == 8128)
                        {
                            EepAddr = 7904;
                            array2[1] = (byte)(EepAddr >> 8);
                            array2[2] = (byte)EepAddr;
                            State = State.WriteStep1;
                            break;
                        }

                        if (EepAddr == 7904)
                        {
                            EepAddr = 7872;
                            array2[1] = (byte)(EepAddr >> 8);
                            array2[2] = (byte)EepAddr;
                            State = State.WriteStep1;
                            break;
                        }

                        FlagTransmitting = false;
                        return true;
                }
            }
            else
            {
                // DebugWindow.GetInstance().updateDebugContent("Retry overrr");
                if (_timesOfRetry <= 0)
                {
                    FlagTransmitting = false;
                    return false;
                }

                _timesOfRetry--;
                _flagRetry = false;
            }

        return false;
    }

    private byte[] GetCHImf_StrToHex(string[] str)
    {
        var array = new byte[16]
        {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255
        };
        if (str[2] != null && str[2] != "")
        {
            var array2 = SetFreqImf_StrToHex(str[2], str[4]);
            var array3 = SetChannelCFG_StrToHex(str);
            for (var i = 0; i < 8; i++)
            {
                array[i] = array2[i];
                array[i + 8] = array3[i];
            }
        }

        return array;
    }

    private byte[] SetFreqImf_StrToHex(string strRxFreqDat, string strTxFreqDat)
    {
        var array = new byte[8] { 255, 255, 255, 255, 255, 255, 255, 255 };
        if (strRxFreqDat != null && strRxFreqDat != "")
        {
            var array2 = new byte[4];
            var array3 = new byte[4];
            array2 = CaculateFreq_StrToHex(strRxFreqDat);
            array3 = CaculateFreq_StrToHex(strTxFreqDat);
            for (var i = 0; i < 4; i++)
            {
                array[i] = array2[i];
                array[i + 4] = array3[i];
            }
        }
        else
        {
            for (var j = 0; j < 8; j++) array[j] = byte.MaxValue;
        }

        return array;
    }

    private byte[] SetChannelCFG_StrToHex(string[] strDat)
    {
        var array = new byte[8];
        var array2 = new byte[2];
        var array3 = new byte[2];
        array2 = CaculateSubaudio_StrToHex(strDat[3]);
        array3 = CaculateSubaudio_StrToHex(strDat[5]);
        var array4 = CaculateChannelCFG_StrToHex(strDat);
        array[0] = array2[0];
        array[1] = array2[1];
        array[2] = array3[0];
        array[3] = array3[1];
        array[4] = GetChSignalCode(strDat[11]);
        array[5] = array4[0];
        array[6] = array4[1];
        array[7] = array4[2];
        return array;
    }

    private byte[] CaculateFreq_StrToHex(string strDat)
    {
        var array = new byte[4] { 255, 255, 255, 255 };
        if (strDat != "" && strDat != null)
        {
            var num = int.Parse(strDat.Remove(3, 1));
            for (var i = 0; i < 4; i++)
            {
                var num2 = num % 100;
                num /= 100;
                array[i] = (byte)(((num2 / 10) << 4) | (num2 % 10));
            }
        }

        return array;
    }

    private byte[] CaculateSubaudio_StrToHex(string strDat)
    {
        var array = new byte[2];
        if ('D' == strDat[0])
        {
            int i;
            for (i = 0; i < 210; i++)
                if (strDat == _tableQt[i])
                {
                    i++;
                    break;
                }

            array[0] = (byte)i;
            array[1] = 0;
        }
        else if (strDat != "OFF")
        {
            var startIndex = strDat.Length - 2;
            strDat = strDat.Remove(startIndex, 1);
            var num = ushort.Parse(strDat);
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

    private byte GetChSignalCode(string strDat)
    {
        return (byte)(byte.Parse(strDat) - 1);
    }

    private byte GetChTxPower(string strDat)
    {
        if (strDat == "H") return 0;
        return 1;
    }

    private byte[] CaculateChannelCFG_StrToHex(string[] strDatas)
    {
        var text = strDatas[7];
        var text2 = strDatas[8];
        var text3 = strDatas[9];
        var text4 = strDatas[10];
        var text5 = strDatas[6];
        var text6 = strDatas[1];
        var text7 = "";
        try
        {
            text7 = strDatas[13];
        }
        catch
        {
            text7 = "OFF";
        }

        var array = new byte[3];
        switch (text2)
        {
            case "OFF":
                array[0] = 0;
                break;
            case "BOT":
                array[0] = 1;
                break;
            case "EOT":
                array[0] = 2;
                break;
            default:
                array[0] = 3;
                break;
        }

        // 在8600新版上还有M选项
        if (ChanChoice.TxPwr.Count == 2)
        {
            if (text5 == "H")
                array[1] = 0;
            else
                array[1] = 1;
        }
        else
        {
            if (text5 == "H")
                array[1] = 0;
            else if (text5 == "M")
                array[1] = 1;
            else
                array[1] = 2;
        }

        array[2] = 0;
        if (text == "N") array[2] |= 64;
        if (text7 == "ON") array[2] |= 1;
        if (text3 == "ON") array[2] |= 8;
        if (text4 == "ON") array[2] |= 4;
        if (text6 == "Yes") array[2] |= 2;
        return array;
    }

    private byte[] SetDataVfoFreq(string vfoFreq)
    {
        var array = new byte[8];
        array[0] = byte.Parse(vfoFreq[0].ToString() ?? "");
        array[1] = byte.Parse(vfoFreq[1].ToString() ?? "");
        array[2] = byte.Parse(vfoFreq[2].ToString() ?? "");
        array[3] = byte.Parse(vfoFreq[4].ToString() ?? "");
        array[4] = byte.Parse(vfoFreq[5].ToString() ?? "");
        array[5] = byte.Parse(vfoFreq[6].ToString() ?? "");
        array[6] = byte.Parse(vfoFreq[7].ToString() ?? "");
        array[7] = byte.Parse(vfoFreq[8].ToString() ?? "");
        return array;
    }

    private byte[] SetDataVfoRemainFreq(string strRemainFreq)
    {
        var array = new byte[7];
        string text;
        try
        {
            text = strRemainFreq;
            if (text == null) text = "00.0000";
        }
        catch
        {
            text = "00.0000";
        }

        array[0] = 0;
        array[1] = byte.Parse(text[0].ToString() ?? "");
        array[2] = byte.Parse(text[1].ToString() ?? "");
        array[3] = byte.Parse(text[3].ToString() ?? "");
        array[4] = byte.Parse(text[4].ToString() ?? "");
        array[5] = byte.Parse(text[5].ToString() ?? "");
        array[6] = byte.Parse(text[6].ToString() ?? "");
        return array;
    }

    private byte[] SetDataVfoqt(string rxQt, string txQt)
    {
        var array = new byte[4];
        var array2 = new byte[2];
        array2 = CaculateSubaudio_StrToHex(rxQt);
        array[0] = array2[0];
        array[1] = array2[1];
        array2 = CaculateSubaudio_StrToHex(txQt);
        array[2] = array2[0];
        array[3] = array2[1];
        return array;
    }

    private byte[] SetDataVfoCfgA(ClassTheRadioData theRadioData)
    {
        var array = new byte[6] { 0, 0, 0, 0, 0, 0 };
        if (theRadioData.FunCfgData.CbBaRemainDir != 0)
        {
            if (theRadioData.FunCfgData.CbBaRemainDir == 1)
                array[0] = 16;
            else
                array[0] = 32;
        }

        array[0] &= 240;
        array[0] += (byte)theRadioData.FunCfgData.CbBaSignalingEnCoder;
        array[1] = 0;
        array[2] = (byte)theRadioData.FunCfgData.CbBaPower;
        array[3] = 0;
        if (theRadioData.FunCfgData.CbBaChBand == 1) array[3] |= 64;
        if (theRadioData.FunCfgData.CbBaFhss == 1) array[3] |= 1;
        array[4] = (byte)theRadioData.FunCfgData.CbBaBand;
        array[5] = (byte)theRadioData.FunCfgData.CbBaFreqStep;
        return array;
    }

    private byte[] SetDataVfoCfgB(ClassTheRadioData theRadioData)
    {
        var array = new byte[6] { 0, 0, 0, 0, 0, 0 };
        if (theRadioData.FunCfgData.CbBbRemainDir != 0)
        {
            if (theRadioData.FunCfgData.CbBbRemainDir == 1)
                array[0] = 16;
            else
                array[0] = 32;
        }

        array[0] &= 240;
        array[0] += (byte)theRadioData.FunCfgData.CbBbSignalingEnCoder;
        array[1] = 0;
        array[2] = (byte)theRadioData.FunCfgData.CbBbPower;
        array[3] = 0;
        if (theRadioData.FunCfgData.CbBbChBand == 1) array[3] |= 64;
        if (theRadioData.FunCfgData.CbBbFhss == 1) array[3] |= 1;
        array[4] = (byte)theRadioData.FunCfgData.CbBbBand;
        array[5] = (byte)theRadioData.FunCfgData.CbBbFreqStep;
        return array;
    }

    private byte[] SetChNameToHex(string[] channelData)
    {
        var array = new byte[16]
        {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255
        };
        if (channelData[2] != null && !(channelData[12] == "") && channelData[12] != null)
        {
            var encoding = Encoding.GetEncoding("gb2312");
            var bytes = encoding.GetBytes(channelData[12]);
            var num = 0;
            // 解除长度限制
            num = bytes.Length > 16 ? 16 : bytes.Length;
            for (var i = 0; i < num; i++) array[i] = bytes[i];
        }

        return array;
    }

    private byte[] SetDtmftoHex(string dtmfCode)
    {
        var array = new byte[16]
        {
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255
        };
        if (dtmfCode != "")
            for (var i = 0; i < dtmfCode.Length; i++)
                array[i] = (byte)"0123456789ABCD*#".IndexOf(dtmfCode[i]);
        return array;
    }

    private void GetCHImfo_HexToStr(ClassTheRadioData theRadioData, int noCh, byte[] dat)
    {
        if (dat[0] == byte.MaxValue)
        {
            for (var i = 0; i < 13; i++) theRadioData.ChanneldataList[noCh].ChangeByNum(i, null);
            // theRadioData.ChannelData[NO_CH][i] = null;
            // theRadioData.ChannelData[NO_CH][0] = NO_CH.ToString();
            theRadioData.ChanneldataList[noCh].ChangeByNum(0, noCh.ToString());
        }
        else
        {
            GetFreqImf_HexToStr(theRadioData, noCh, dat);
            GetQTImf_HexToStr(theRadioData, noCh, dat);
            GetChOtherImf(theRadioData, noCh, dat);
        }
    }

    private void GetFreqImf_HexToStr(ClassTheRadioData theRadioData, int noCh, byte[] dat)
    {
        var dat2 = new byte[4]
        {
            dat[0],
            dat[1],
            dat[2],
            dat[3]
        };
        var dat3 = new byte[4]
        {
            dat[4],
            dat[5],
            dat[6],
            dat[7]
        };
        theRadioData.ChanneldataList[noCh].ChangeByNum(2, CaculateFreq_HexToStr(dat2));
        theRadioData.ChanneldataList[noCh].ChangeByNum(4, CaculateFreq_HexToStr(dat3));
        // channelDat[2] = CaculateFreq_HexToStr(dat2);
        // channelDat[4] = CaculateFreq_HexToStr(dat3);
    }

    private string CaculateFreq_HexToStr(byte[] dat)
    {
        var num = 0;
        if (dat[3] != byte.MaxValue && dat[3] != 0)
        {
            for (var i = 0; i < 4; i++) dat[i] = (byte)(((dat[i] >> 4) & 0xF) * 10 + (dat[i] & 0xF));
            for (var num2 = 3; num2 >= 0; num2--) num = num * 100 + dat[num2];
            return num.ToString().Insert(3, ".");
        }

        return null;
    }

    private void GetQTImf_HexToStr(ClassTheRadioData theRadioData, int noCh, byte[] dat)
    {
        var array = new byte[2]
        {
            dat[8],
            dat[9]
        };
        var array2 = new byte[2]
        {
            dat[10],
            dat[11]
        };
        if (GetTypeOfSubaudio_HexToStr(array[1]) == SubaudioType.Ctcss)
            theRadioData.ChanneldataList[noCh].ChangeByNum(3, CaculateCTCSS_HexToStr(array));
        // channelDat[3] = CaculateCTCSS_HexToStr(array);
        else
            theRadioData.ChanneldataList[noCh].ChangeByNum(3, CaculateCDCSS_HexToStr(array));
        // channelDat[3] = CaculateCDCSS_HexToStr(array);
        if (GetTypeOfSubaudio_HexToStr(array2[1]) == SubaudioType.Ctcss)
            theRadioData.ChanneldataList[noCh].ChangeByNum(5, CaculateCTCSS_HexToStr(array2));
        // channelDat[5] = CaculateCTCSS_HexToStr(array2);
        else
            theRadioData.ChanneldataList[noCh].ChangeByNum(5, CaculateCDCSS_HexToStr(array2));
        // channelDat[5] = CaculateCDCSS_HexToStr(array2);
    }

    private string CaculateCTCSS_HexToStr(byte[] dat)
    {
        ushort num = 0;
        if (dat[0] != 0 && dat[0] != byte.MaxValue)
        {
            ushort num2 = dat[1];
            num2 <<= 8;
            var text = ((ushort)(num2 + dat[0])).ToString();
            return text.Insert(text.Length - 1, ".");
        }

        return "OFF";
    }

    private string CaculateCDCSS_HexToStr(byte[] dat)
    {
        if (dat[0] != 0 && dat[0] <= 210) return _tableQt[dat[0] - 1];
        return "OFF";
    }

    private SubaudioType GetTypeOfSubaudio_HexToStr(byte dat)
    {
        if (dat == 0) return SubaudioType.Cdcss;
        return SubaudioType.Ctcss;
    }

    private void GetChOtherImf(ClassTheRadioData theRadioData, int noCh, byte[] dat)
    {
        if (dat[12] >= 15) dat[12] = 0;
        theRadioData.ChanneldataList[noCh].ChangeByNum(11, (dat[12] + 1).ToString());
        // channelDat[11] = (dat[12] + 1).ToString();
        var array = new string[4] { "OFF", "BOT", "EOT", "BOTH" };
        if (dat[13] > 3) dat[13] = 0;
        theRadioData.ChanneldataList[noCh].ChangeByNum(8, array[dat[13]]);
        // channelDat[8] = array[dat[13]];
        var array2 = new string[2] { "H", "L" };
        if (dat[14] > 1) dat[14] = 0;
        theRadioData.ChanneldataList[noCh].ChangeByNum(6, array2[dat[14]]);

        // channelDat[6] = array2[dat[14]];

        if ((dat[15] & 0x40) == 64)
            theRadioData.ChanneldataList[noCh].ChangeByNum(7, "N");
        // channelDat[7] = "N";
        else
            theRadioData.ChanneldataList[noCh].ChangeByNum(7, "W");
        // channelDat[7] = "W";
        if ((dat[15] & 8) == 8)
            theRadioData.ChanneldataList[noCh].ChangeByNum(9, "ON");
        // channelDat[9] = "ON";
        else
            theRadioData.ChanneldataList[noCh].ChangeByNum(9, "OFF");
        // channelDat[9] = "OFF";
        if ((dat[15] & 4) == 4)
            theRadioData.ChanneldataList[noCh].ChangeByNum(10, "ON");
        // channelDat[10] = "ON";
        else
            theRadioData.ChanneldataList[noCh].ChangeByNum(10, "OFF");
        // channelDat[10] = "OFF";
        if ((dat[15] & 2) == 2)
            theRadioData.ChanneldataList[noCh].ChangeByNum(1, "Yes");
        // channelDat[1] = "Yes";
        else
            theRadioData.ChanneldataList[noCh].ChangeByNum(1, "No");
        // channelDat[1] = "No";
        if ((dat[15] & 1) == 1)
            theRadioData.ChanneldataList[noCh].ChangeByNum(13, "ON");
        // channelDat[13] = "ON";
        else
            theRadioData.ChanneldataList[noCh].ChangeByNum(13, "OFF");
        // channelDat[13] = "OFF";
    }

    private string GetTheNameOfCh(byte[] dat)
    {
        var text = "";
        var aSciiEncoding = new ASCIIEncoding();
        var encoding = Encoding.GetEncoding("gb2312");
        var num = 0;
        // 解除长度限制
        while (num < 16 && dat[num] != byte.MaxValue)
            if (dat[num] >= 161)
            {
                text += encoding.GetString(dat, num, 2);
                num += 2;
            }
            else
            {
                text += aSciiEncoding.GetString(dat, num, 1);
                num++;
            }

        return text;
        // channelDat[12] = text;
    }

    private string ConvertHex2Str_DTMF(byte[] dat)
    {
        var text = "";
        for (var i = 0; i < 5 && dat[i] != byte.MaxValue; i++) text += "0123456789ABCD*#"[dat[i]];
        return text;
    }

    private string GetTheLocalId(byte[] dat)
    {
        var text = "";
        for (var i = 0; i < 5 && dat[i] != byte.MaxValue; i++) text += dat[i];
        return text;
    }

    private bool GetTheBoolOfSendIDOnPTTPress(byte[] dat)
    {
        if ((dat[6] & 1) == 1) return true;
        return false;
    }

    private bool GetTheBoolOfSendIDOnPTTRelease(byte[] dat)
    {
        if ((dat[6] & 2) == 2) return true;
        return false;
    }

    private void GetFunCfgImf_Part1(ClassTheRadioData theRadioData, byte[] dat)
    {
        theRadioData.FunCfgData.CbBSql = dat[0];
        if (dat[1] > 2) dat[1] = 1;
        theRadioData.FunCfgData.CbBSaveMode = dat[1];
        theRadioData.FunCfgData.CbBVox = dat[2];
        theRadioData.FunCfgData.CbBAutoBackLight = dat[3];
        if (dat[4] > 0)
            theRadioData.FunCfgData.CBTdr = true;
        else
            theRadioData.FunCfgData.CBTdr = false;
        theRadioData.FunCfgData.CbBTot = dat[5];
        if (dat[6] > 0)
            theRadioData.FunCfgData.CBSoundOfBi = true;
        else
            theRadioData.FunCfgData.CBSoundOfBi = false;
        theRadioData.FunCfgData.CbBVoicSwitch = dat[7];
        if (dat[8] >= 2) dat[8] = 1;
        theRadioData.FunCfgData.CbBLanguage = dat[8];
        theRadioData.FunCfgData.CbBDtmf = dat[9];
        theRadioData.FunCfgData.CbBScan = dat[10];
        if (dat[11] > 3) dat[11] = 0;
        theRadioData.FunCfgData.CbBPttid = dat[11];
        theRadioData.FunCfgData.CbBSendIdDelay = dat[12];
        theRadioData.FunCfgData.CbBChADisplayMode = dat[13];
        theRadioData.FunCfgData.CbBChBDisplayMode = dat[14];
        if (dat[15] > 0)
            theRadioData.FunCfgData.CBStopSendOnBusy = true;
        else
            theRadioData.FunCfgData.CBStopSendOnBusy = false;
    }

    private void GetFunCfgImf_Part2(ClassTheRadioData theRadioData, byte[] dat)
    {
        if (dat[0] > 0)
            theRadioData.FunCfgData.CBAutoLock = true;
        else
            theRadioData.FunCfgData.CBAutoLock = false;
        theRadioData.FunCfgData.CbBAlarmMode = dat[1];
        if (dat[2] > 0)
            theRadioData.FunCfgData.CBAlarmSound = true;
        else
            theRadioData.FunCfgData.CBAlarmSound = false;
        theRadioData.FunCfgData.CbBTxUnderTdrStart = dat[3];
        theRadioData.FunCfgData.CbBTailNoiseClear = dat[4];
        theRadioData.FunCfgData.CbBPassRptNoiseClear = dat[5];
        theRadioData.FunCfgData.CbBPassRptNoiseDetect = dat[6];
        theRadioData.FunCfgData.CbBSoundOfTxEnd = dat[7];
        if (dat[9] > 0)
            theRadioData.FunCfgData.CBFmRadioEnable = false;
        else
            theRadioData.FunCfgData.CBFmRadioEnable = true;
        theRadioData.FunCfgData.CbBWorkModeA = dat[10] & 0xF;
        theRadioData.FunCfgData.CbBWorkModeB = (dat[10] >> 4) & 0xF;
        if (dat[11] > 0)
            theRadioData.FunCfgData.CBLockKeyBoard = true;
        else
            theRadioData.FunCfgData.CBLockKeyBoard = false;
        if (dat[12] == byte.MaxValue) dat[12] = 0;
        theRadioData.FunCfgData.CbBPowerOnMsg = dat[12];
        if (dat[14] > 3) dat[14] = 2;
        theRadioData.FunCfgData.CbB1750Hz = dat[14];
    }

    private string GetVFO_CurFreq(byte[] dat)
    {
        var text = "";
        for (var i = 0; i < 8; i++) text += dat[i];
        return text.Insert(3, ".");
    }

    private string GetVFO_RemainFreq(byte[] dat)
    {
        var text = "";
        for (var i = 1; i < 7; i++) text += dat[i];
        return text.Insert(2, ".");
    }

    private string[] GetVF0_QTImf(byte[] dat)
    {
        var array = new byte[2]
        {
            dat[0],
            dat[1]
        };
        var array2 = new byte[2]
        {
            dat[2],
            dat[3]
        };
        var array3 = new string[2];
        if (GetTypeOfSubaudio_HexToStr(array[1]) == SubaudioType.Ctcss)
            array3[0] = CaculateCTCSS_HexToStr(array);
        else
            array3[0] = CaculateCDCSS_HexToStr(array);
        if (GetTypeOfSubaudio_HexToStr(array2[1]) == SubaudioType.Ctcss)
            array3[1] = CaculateCTCSS_HexToStr(array2);
        else
            array3[1] = CaculateCDCSS_HexToStr(array2);
        return array3;
    }

    private int GetVFO_DirOfRemainFreq(byte dat)
    {
        if ((dat & 0x10) == 16) return 1;
        if ((dat & 0x20) == 32) return 2;
        return 0;
    }

    private int GetVFO_SignalingCode(byte dat)
    {
        return dat & 0xF;
    }

    private int GetVFO_TxPower(byte dat)
    {
        var b = dat;
        if ((b & 0x80) == 128) return 1;
        return 0;
    }

    private int GetVFO_BandWide(byte dat)
    {
        var b = dat;
        if ((b & 0x40) == 64) return 1;
        return 0;
    }

    private int GetVFO_Fhss(byte dat)
    {
        if ((dat & 1) == 1) return 1;
        return 0;
    }

    private void GetVFO_A_Parameter(ClassTheRadioData theRadioData, byte[] dat, byte[] dat2)
    {
        var dat3 = new byte[8]
        {
            dat[0],
            dat[1],
            dat[2],
            dat[3],
            dat[4],
            dat[5],
            dat[6],
            dat[7]
        };
        var dat4 = new byte[4]
        {
            dat[8],
            dat[9],
            dat[10],
            dat[11]
        };
        var dat5 = new byte[7]
        {
            dat2[4],
            dat2[5],
            dat2[6],
            dat2[7],
            dat2[8],
            dat2[9],
            dat2[10]
        };
        var array = new string[2];
        theRadioData.FunCfgData.TBaCurFreq = GetVFO_CurFreq(dat3);
        array = GetVF0_QTImf(dat4);
        theRadioData.FunCfgData.CbBaRxQt = array[0];
        theRadioData.FunCfgData.CbBaTxQt = array[1];
        theRadioData.FunCfgData.CbBaRemainDir = GetVFO_DirOfRemainFreq(dat[14]);
        theRadioData.FunCfgData.CbBaSignalingEnCoder = GetVFO_SignalingCode(dat[14]);
        theRadioData.FunCfgData.CbBaPower = dat2[0];
        theRadioData.FunCfgData.CbBaChBand = GetVFO_BandWide(dat2[1]);
        theRadioData.FunCfgData.CbBaFhss = GetVFO_Fhss(dat2[1]);
        theRadioData.FunCfgData.CbBaBand = dat2[2];
        theRadioData.FunCfgData.CbBaFreqStep = dat2[3] % 8;
        theRadioData.FunCfgData.TBaRemainFreq = GetVFO_RemainFreq(dat5);
    }

    private void GetVFO_B_Parameter(ClassTheRadioData theRadioData, byte[] dat, byte[] dat2)
    {
        var dat3 = new byte[8]
        {
            dat[0],
            dat[1],
            dat[2],
            dat[3],
            dat[4],
            dat[5],
            dat[6],
            dat[7]
        };
        var dat4 = new byte[4]
        {
            dat[8],
            dat[9],
            dat[10],
            dat[11]
        };
        var dat5 = new byte[7]
        {
            dat2[4],
            dat2[5],
            dat2[6],
            dat2[7],
            dat2[8],
            dat2[9],
            dat2[10]
        };
        var array = new string[2];
        theRadioData.FunCfgData.TBbCurFreq = GetVFO_CurFreq(dat3);
        array = GetVF0_QTImf(dat4);
        theRadioData.FunCfgData.CbBbRxQt = array[0];
        theRadioData.FunCfgData.CbBbTxQt = array[1];
        theRadioData.FunCfgData.CbBbRemainDir = GetVFO_DirOfRemainFreq(dat[14]);
        theRadioData.FunCfgData.CbBbSignalingEnCoder = GetVFO_SignalingCode(dat[14]);
        theRadioData.FunCfgData.CbBbPower = dat2[0];
        theRadioData.FunCfgData.CbBbChBand = GetVFO_BandWide(dat2[1]);
        theRadioData.FunCfgData.CbBbFhss = GetVFO_Fhss(dat2[1]);
        theRadioData.FunCfgData.CbBbBand = dat2[2];
        theRadioData.FunCfgData.CbBbFreqStep = dat2[3] % 8;
        theRadioData.FunCfgData.TBbRemainFreq = GetVFO_RemainFreq(dat5);
    }

    private short HexToInt(byte data)
    {
        short num = 0;
        return (short)(((data & 0xF0) >> 4) * 10 + (data & 0xF));
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _flagRetry = true;
    }
}