using System.Text;
using System.Timers;
using SQ5R;

namespace WF_FRAM_KDH;

internal class WriFreq
{
    private static readonly ushort MODELTYPE_WLT = 0;

    private static readonly ushort MODELTYPE_SHX = 1;

    private static readonly ushort MODELTYPE_BFHx = 2;

    private readonly byte[] bufForData = new byte[128];

    private readonly OtherImfData cfgData;

    private readonly byte[] hSTable1 = new byte[7] { 80, 82, 79, 71, 82, 79, 77 };

    private readonly byte[][] hStable2_ModelType = new byte[3][]
    {
        new byte[3] { 87, 76, 84 },
        new byte[3] { 83, 72, 88 },
        new byte[3] { 66, 70, 72 }
    };

    private readonly ushort MODELTYPE = MODELTYPE_SHX;

    private readonly OPERATION_TYPE op;

    private readonly MySerialPort sP;

    private readonly string[] Table_QT = new string[210]
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

    private readonly ClassTheRadioData theRadioData;

    public ushort eepAddr;

    private bool flagRetry;

    public bool flagTransmitting;

    private int numOfChannel = 0;

    public STATE state = STATE.HandShakeStep1;

    private Timer timer;

    private byte timesOfRetry = 5;

    public WriFreq(MySerialPort sP, ClassTheRadioData theRadioData, OPERATION_TYPE op)
    {
        this.op = op;
        this.sP = sP;
        this.theRadioData = theRadioData;
        TimerInit();
    }

    public WriFreq(MySerialPort sP, OtherImfData data, OPERATION_TYPE op)
    {
        this.op = op;
        this.sP = sP;
        cfgData = data;
        TimerInit();
    }

    private void TimerInit()
    {
        timer = new Timer();
        timer.Interval = 1000.0;
        timer.Elapsed += Timer_Elapsed;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private void resetRetryCount()
    {
        timesOfRetry = 5;
        flagRetry = false;
    }

    public bool DoIt()
    {
        flagTransmitting = true;
        state = STATE.HandShakeStep1;
        if (HandShake())
        {
            if (op == OPERATION_TYPE.READ)
            {
                if (ReadCHData())
                {
                    sP.CloseSerial();
                    return true;
                }

                sP.CloseSerial();
                return false;
            }

            if (OPERATION_TYPE.WRITE == op)
            {
                if (WriteCHData())
                {
                    sP.CloseSerial();
                    return true;
                }

                sP.CloseSerial();
                return false;
            }

            if (OPERATION_TYPE.READ_CONFIG == op)
            {
                if (ReadConfigData())
                {
                    sP.CloseSerial();
                    return true;
                }

                sP.CloseSerial();
                return false;
            }

            if (OPERATION_TYPE.WRITE_CONFIG == op)
            {
                if (WriteConfigData())
                {
                    sP.CloseSerial();
                    return true;
                }

                sP.CloseSerial();
                return false;
            }

            sP.CloseSerial();
            return false;
        }

        sP.CloseSerial();
        return false;
    }

    private bool HandShake()
    {
        while (flagTransmitting)
            if (!flagRetry)
            {
                switch (state)
                {
                    case STATE.HandShakeStep1:
                        sP.WriteByte(hSTable1, 0, hSTable1.Length);
                        sP.WriteByte(hStable2_ModelType[MODELTYPE], 0, hStable2_ModelType[MODELTYPE].Length);
                        sP.WriteByte(85);
                        timer.Start();
                        resetRetryCount();
                        state = STATE.HandShakeStep2;
                        break;
                    case STATE.HandShakeStep2:
                        if (sP.BytesToReadFromCache >= 1)
                        {
                            sP.ReadByte(bufForData, 0, 1);
                            if (bufForData[0] == 6)
                            {
                                resetRetryCount();
                                sP.WriteByte(70);
                                state = STATE.HandShakeStep3;
                            }
                        }

                        break;
                    case STATE.HandShakeStep3:
                        if (sP.BytesToReadFromCache >= 8)
                        {
                            sP.ReadByte(bufForData, 0, 8);
                            timer.Stop();
                            resetRetryCount();
                            if (op == OPERATION_TYPE.READ || op == OPERATION_TYPE.READ_CONFIG)
                                state = STATE.ReadStep1;
                            else if (OPERATION_TYPE.WRITE == op || op == OPERATION_TYPE.WRITE_CONFIG)
                                state = STATE.WriteStep1;

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
                switch (state)
                {
                    case STATE.HandShakeStep2:
                        state = STATE.HandShakeStep1;
                        break;
                    case STATE.HandShakeStep3:
                        sP.WriteByte(70);
                        break;
                }
            }

        return false;
    }

    private bool ReadCHData()
    {
        var array = new byte[4] { 82, 0, 0, 64 };
        var num = 0;
        while (flagTransmitting)
            if (!flagRetry)
            {
                switch (state)
                {
                    case STATE.ReadStep1:
                        sP.WriteByte(array, 0, 4);
                        state = STATE.ReadStep2;
                        timer.Start();
                        break;
                    case STATE.ReadStep2:
                    {
                        if (sP.BytesToReadFromCache < array[3] + 4) break;

                        timer.Stop();
                        resetRetryCount();
                        sP.ReadByte(bufForData, 0, array[3] + 4);
                        if (bufForData[1] != array[1] || bufForData[2] != array[2]) break;

                        var array2 = new byte[4][]
                        {
                            new byte[16],
                            new byte[16],
                            new byte[16],
                            new byte[16]
                        };
                        for (var i = 0; i < 16; i++)
                        {
                            array2[0][i] = bufForData[i + 4];
                            array2[1][i] = bufForData[i + 4 + 16];
                            array2[2][i] = bufForData[i + 4 + 32];
                            array2[3][i] = bufForData[i + 4 + 48];
                        }

                        if (eepAddr >= 0 && eepAddr < 2048)
                        {
                            GetCHImfo_HexToStr(theRadioData, num++, array2[0]);
                            GetCHImfo_HexToStr(theRadioData, num++, array2[1]);
                            GetCHImfo_HexToStr(theRadioData, num++, array2[2]);
                            GetCHImfo_HexToStr(theRadioData, num++, array2[3]);
                        }
                        else if (eepAddr == 2048)
                        {
                            num = 0;
                        }
                        else if (eepAddr >= 3072 && eepAddr < 5120)
                        {
                            GetTheNameOfCH(theRadioData.ChannelData[num++], array2[0]);
                            GetTheNameOfCH(theRadioData.ChannelData[num++], array2[1]);
                            GetTheNameOfCH(theRadioData.ChannelData[num++], array2[2]);
                            GetTheNameOfCH(theRadioData.ChannelData[num++], array2[3]);
                        }
                        else if (eepAddr == 6656)
                        {
                            GetFunCfgImf_Part1(theRadioData, array2[0]);
                            GetFunCfgImf_Part2(theRadioData, array2[1]);
                            if (array2[2][0] == byte.MaxValue) array2[2][0] = 5;

                            theRadioData.funCfgData.CbB_VoxDelay = array2[2][0];
                            if (array2[2][1] == byte.MaxValue) array2[2][1] = 1;

                            theRadioData.funCfgData.CbB_TimerMenuQuit = array2[2][1];
                            if (array2[2][2] == byte.MaxValue) array2[2][2] = 1;

                            theRadioData.funCfgData.CbB_MicGain = array2[2][2];
                        }
                        else if (eepAddr == 6720)
                        {
                            GetVFO_A_Parameter(theRadioData, array2[0], array2[1]);
                            GetVFO_B_Parameter(theRadioData, array2[2], array2[3]);
                        }
                        else if (eepAddr == 6784)
                        {
                            var array3 = new byte[5] { 7, 10, 5, 28, 29 };
                            var num2 = -1;
                            for (var j = 0; j < 5; j++)
                                if (array2[0][0] == array3[j])
                                {
                                    num2 = j;
                                    break;
                                }

                            if (num2 != -1) theRadioData.funCfgData.CbB_KeySide = num2;

                            num2 = -1;
                            for (var k = 0; k < 5; k++)
                                if (array2[0][1] == array3[k])
                                {
                                    num2 = k;
                                    break;
                                }

                            if (num2 != -1) theRadioData.funCfgData.CbB_KeySideL = num2;
                        }
                        else if (eepAddr >= 6912 && eepAddr < 7136)
                        {
                            switch (eepAddr)
                            {
                                case 6912:
                                    theRadioData.dtmfData.GroupOfDTMF_1 = ConvertHex2Str_DTMF(array2[0]);
                                    theRadioData.dtmfData.GroupOfDTMF_2 = ConvertHex2Str_DTMF(array2[1]);
                                    theRadioData.dtmfData.GroupOfDTMF_3 = ConvertHex2Str_DTMF(array2[2]);
                                    theRadioData.dtmfData.GroupOfDTMF_4 = ConvertHex2Str_DTMF(array2[3]);
                                    break;
                                case 6976:
                                    theRadioData.dtmfData.GroupOfDTMF_5 = ConvertHex2Str_DTMF(array2[0]);
                                    theRadioData.dtmfData.GroupOfDTMF_6 = ConvertHex2Str_DTMF(array2[1]);
                                    theRadioData.dtmfData.GroupOfDTMF_7 = ConvertHex2Str_DTMF(array2[2]);
                                    theRadioData.dtmfData.GroupOfDTMF_8 = ConvertHex2Str_DTMF(array2[3]);
                                    break;
                                case 7040:
                                    theRadioData.dtmfData.GroupOfDTMF_9 = ConvertHex2Str_DTMF(array2[0]);
                                    theRadioData.dtmfData.GroupOfDTMF_A = ConvertHex2Str_DTMF(array2[1]);
                                    theRadioData.dtmfData.GroupOfDTMF_B = ConvertHex2Str_DTMF(array2[2]);
                                    theRadioData.dtmfData.GroupOfDTMF_C = ConvertHex2Str_DTMF(array2[3]);
                                    break;
                                case 7104:
                                    theRadioData.dtmfData.GroupOfDTMF_D = ConvertHex2Str_DTMF(array2[0]);
                                    theRadioData.dtmfData.GroupOfDTMF_E = ConvertHex2Str_DTMF(array2[1]);
                                    theRadioData.dtmfData.GroupOfDTMF_F = ConvertHex2Str_DTMF(array2[2]);
                                    theRadioData.dtmfData.TheIDOfLocalHost = GetTheLocalID(array2[3]);
                                    theRadioData.dtmfData.GroupCall = array2[3][5];
                                    theRadioData.dtmfData.SendOnPTTPressed = GetTheBoolOfSendIDOnPTTPress(array2[3]);
                                    theRadioData.dtmfData.SendOnPTTReleased = GetTheBoolOfSendIDOnPTTRelease(array2[3]);
                                    theRadioData.dtmfData.LastTimeSend = array2[3][7];
                                    theRadioData.dtmfData.LastTimeStop = array2[3][8];
                                    break;
                            }
                        }

                        if (eepAddr < 7168)
                        {
                            eepAddr += array[3];
                            if (eepAddr == 2112)
                                eepAddr = 3072;
                            else if (eepAddr == 5120) eepAddr = 6656;

                            array[1] = (byte)(eepAddr >> 8);
                            array[2] = (byte)eepAddr;
                            timer.Start();
                            state = STATE.ReadStep1;
                            break;
                        }

                        sP.WriteByte(69);
                        flagTransmitting = false;
                        return true;
                    }
                }
            }
            else
            {
                if (timesOfRetry <= 0)
                {
                    flagTransmitting = false;
                    return false;
                }

                timesOfRetry--;
                flagRetry = false;
            }

        return false;
    }

    private bool WriteCHData()
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
        while (flagTransmitting)
            if (!flagRetry)
            {
                switch (state)
                {
                    case STATE.WriteStep1:
                        if (eepAddr < 2048)
                        {
                            var cHImf_StrToHex = GetCHImf_StrToHex(theRadioData.ChannelData[num++]);
                            var cHImf_StrToHex2 = GetCHImf_StrToHex(theRadioData.ChannelData[num++]);
                            for (var j = 0; j < 16; j++)
                            {
                                array[j + 4] = cHImf_StrToHex[j];
                                array[j + 20] = cHImf_StrToHex2[j];
                            }
                        }
                        else if (eepAddr == 2048)
                        {
                            num = 0;
                        }
                        else if (eepAddr >= 3072 && eepAddr < 5120)
                        {
                            var array3 = SetCHNaemToHex(theRadioData.ChannelData[num++]);
                            var array4 = SetCHNaemToHex(theRadioData.ChannelData[num++]);
                            for (var k = 0; k < 16; k++)
                            {
                                array[k + 4] = array3[k];
                                array[k + 20] = array4[k];
                            }
                        }
                        else if (eepAddr == 6656)
                        {
                            array[4] = (byte)theRadioData.funCfgData.CbB_SQL;
                            array[5] = (byte)theRadioData.funCfgData.CbB_SaveMode;
                            array[6] = (byte)theRadioData.funCfgData.CbB_VOX;
                            array[7] = (byte)theRadioData.funCfgData.CbB_AutoBackLight;
                            if (theRadioData.funCfgData.CB_TDR)
                                array[8] = 1;
                            else
                                array[8] = 0;

                            array[9] = (byte)theRadioData.funCfgData.CbB_TOT;
                            if (theRadioData.funCfgData.CB_SoundOfBi)
                                array[10] = 1;
                            else
                                array[10] = 0;

                            array[11] = (byte)theRadioData.funCfgData.CbB_VoicSwitch;
                            array[12] = (byte)theRadioData.funCfgData.CbB_Language;
                            array[13] = (byte)theRadioData.funCfgData.CbB_DTMF;
                            array[14] = (byte)theRadioData.funCfgData.CbB_Scan;
                            array[15] = (byte)theRadioData.funCfgData.CbB_PTTID;
                            array[16] = (byte)theRadioData.funCfgData.CbB_SendIDDelay;
                            array[17] = (byte)theRadioData.funCfgData.CbB_CH_A_DisplayMode;
                            array[18] = (byte)theRadioData.funCfgData.CbB_CH_B_DisplayMode;
                            if (theRadioData.funCfgData.CB_StopSendOnBusy)
                                array[19] = 1;
                            else
                                array[19] = 0;

                            if (theRadioData.funCfgData.CB_AutoLock)
                                array[20] = 1;
                            else
                                array[20] = 0;

                            array[21] = (byte)theRadioData.funCfgData.CbB_AlarmMode;
                            if (theRadioData.funCfgData.CB_AlarmSound)
                                array[22] = 1;
                            else
                                array[22] = 0;

                            array[23] = (byte)theRadioData.funCfgData.CbB_TxUnderTDRStart;
                            array[24] = (byte)theRadioData.funCfgData.CbB_TailNoiseClear;
                            array[25] = (byte)theRadioData.funCfgData.CbB_PassRepetNoiseClear;
                            array[26] = (byte)theRadioData.funCfgData.CbB_PassRepetNoiseDetect;
                            array[27] = (byte)theRadioData.funCfgData.CbB_SoundOfTxEnd;
                            array[28] = 0;
                            if (theRadioData.funCfgData.CB_FMRadioEnable)
                                array[29] = 0;
                            else
                                array[29] = 1;

                            array[30] = (byte)theRadioData.funCfgData.CbB_WorkModeB;
                            array[30] <<= 4;
                            array[30] = (byte)(array[30] | theRadioData.funCfgData.CbB_WorkModeA);
                            if (theRadioData.funCfgData.CB_LockKeyBoard)
                                array[31] = 1;
                            else
                                array[31] = 0;

                            array[32] = (byte)theRadioData.funCfgData.CbB_PowerOnMsg;
                            array[33] = 0;
                            array[34] = (byte)theRadioData.funCfgData.CbB_1750Hz;
                            array[35] = 128;
                        }
                        else if (eepAddr == 6688)
                        {
                            array[4] = (byte)theRadioData.funCfgData.CbB_VoxDelay;
                            array[5] = (byte)theRadioData.funCfgData.CbB_TimerMenuQuit;
                            array[6] = (byte)theRadioData.funCfgData.CbB_MicGain;
                        }
                        else if (eepAddr == 6720)
                        {
                            var array5 = SetDataVFOFreq(theRadioData.funCfgData.TB_A_CurFreq);
                            var array6 = SetDataVFOQT(theRadioData.funCfgData.CbB_A_RxQT,
                                theRadioData.funCfgData.CbB_A_TxQT);
                            var array7 = SetDataVFOCfgA(theRadioData);
                            var array8 = SetDataVFORemainFreq(theRadioData.funCfgData.TB_A_RemainFreq);
                            for (var l = 0; l < 4; l++)
                            {
                                array[l + 4] = array5[l];
                                array[l + 8] = array5[l + 4];
                                array[l + 12] = array6[l];
                            }

                            for (var m = 0; m < 6; m++) array[m + 18] = array7[m];

                            for (var n = 0; n < 7; n++) array[n + 24] = array8[n];
                        }
                        else if (eepAddr == 6752)
                        {
                            var array9 = SetDataVFOFreq(theRadioData.funCfgData.TB_B_CurFreq);
                            var array10 = SetDataVFOQT(theRadioData.funCfgData.CbB_B_RxQT,
                                theRadioData.funCfgData.CbB_B_TxQT);
                            var array11 = SetDataVFOCfgB(theRadioData);
                            var array12 = SetDataVFORemainFreq(theRadioData.funCfgData.TB_B_RemainFreq);
                            for (var num2 = 0; num2 < 4; num2++)
                            {
                                array[num2 + 4] = array9[num2];
                                array[num2 + 8] = array9[num2 + 4];
                                array[num2 + 12] = array10[num2];
                            }

                            for (var num3 = 0; num3 < 6; num3++) array[num3 + 18] = array11[num3];

                            for (var num4 = 0; num4 < 7; num4++) array[num4 + 24] = array12[num4];
                        }
                        else if (eepAddr == 6784)
                        {
                            var array13 = new byte[5] { 7, 10, 5, 28, 29 };
                            array[4] = array13[theRadioData.funCfgData.CbB_KeySide];
                            array[5] = array13[theRadioData.funCfgData.CbB_KeySideL];
                        }
                        else if (eepAddr >= 6912 && eepAddr <= 7136)
                        {
                            switch (eepAddr)
                            {
                                case 6912:
                                {
                                    var array14 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_1);
                                    var array15 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_2);
                                    for (var num8 = 0; num8 < 16; num8++)
                                    {
                                        array[num8 + 4] = array14[num8];
                                        array[num8 + 20] = array15[num8];
                                    }

                                    break;
                                }
                                case 6944:
                                {
                                    var array14 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_3);
                                    var array15 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_4);
                                    for (var num9 = 0; num9 < 16; num9++)
                                    {
                                        array[num9 + 4] = array14[num9];
                                        array[num9 + 20] = array15[num9];
                                    }

                                    break;
                                }
                                case 6976:
                                {
                                    var array14 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_5);
                                    var array15 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_6);
                                    for (var num13 = 0; num13 < 16; num13++)
                                    {
                                        array[num13 + 4] = array14[num13];
                                        array[num13 + 20] = array15[num13];
                                    }

                                    break;
                                }
                                case 7008:
                                {
                                    var array14 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_7);
                                    var array15 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_8);
                                    for (var num7 = 0; num7 < 16; num7++)
                                    {
                                        array[num7 + 4] = array14[num7];
                                        array[num7 + 20] = array15[num7];
                                    }

                                    break;
                                }
                                case 7040:
                                {
                                    var array14 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_9);
                                    var array15 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_A);
                                    for (var num12 = 0; num12 < 16; num12++)
                                    {
                                        array[num12 + 4] = array14[num12];
                                        array[num12 + 20] = array15[num12];
                                    }

                                    break;
                                }
                                case 7072:
                                {
                                    var array14 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_B);
                                    var array15 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_C);
                                    for (var num10 = 0; num10 < 16; num10++)
                                    {
                                        array[num10 + 4] = array14[num10];
                                        array[num10 + 20] = array15[num10];
                                    }

                                    break;
                                }
                                case 7104:
                                {
                                    var array14 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_D);
                                    var array15 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_E);
                                    for (var num11 = 0; num11 < 16; num11++)
                                    {
                                        array[num11 + 4] = array14[num11];
                                        array[num11 + 20] = array15[num11];
                                    }

                                    break;
                                }
                                case 7136:
                                {
                                    var array14 = SetDTMFTOHex(theRadioData.dtmfData.GroupOfDTMF_F);
                                    for (var num5 = 0; num5 < 16; num5++) array[num5 + 4] = array14[num5];

                                    if (theRadioData.dtmfData.TheIDOfLocalHost != "")
                                    {
                                        var theIDOfLocalHost = theRadioData.dtmfData.TheIDOfLocalHost;
                                        for (var num6 = 0; num6 < theIDOfLocalHost.Length; num6++)
                                            array[num6 + 20] = byte.Parse(theIDOfLocalHost[num6].ToString() ?? "");
                                    }

                                    array[25] = (byte)theRadioData.dtmfData.GroupCall;
                                    array[26] = 0;
                                    if (theRadioData.dtmfData.SendOnPTTPressed) array[26] |= 1;

                                    if (theRadioData.dtmfData.SendOnPTTReleased) array[26] |= 2;

                                    array[27] = (byte)theRadioData.dtmfData.LastTimeSend;
                                    array[28] = (byte)theRadioData.dtmfData.LastTimeStop;
                                    break;
                                }
                            }
                        }

                        sP.WriteByte(array, 0, array[3] + 4);
                        timer.Start();
                        state = STATE.WriteStep2;
                        break;
                    case STATE.WriteStep2:
                        if (sP.BytesToReadFromCache < 1) break;

                        sP.ReadByte(bufForData, 0, sP.BytesToReadFromCache);
                        if (bufForData[0] == 6)
                        {
                            timer.Stop();
                            resetRetryCount();
                            if (eepAddr >= 7168)
                            {
                                flagTransmitting = false;
                                return true;
                            }

                            eepAddr += array[3];
                            if (eepAddr == 2080)
                                eepAddr = 3072;
                            else if (eepAddr == 5120) eepAddr = 6656;

                            for (var i = 0; i < 36; i++) array[i] = array2[i];

                            array[1] = (byte)(eepAddr >> 8);
                            array[2] = (byte)eepAddr;
                            state = STATE.WriteStep1;
                        }

                        break;
                }
            }
            else
            {
                if (timesOfRetry <= 0)
                {
                    flagTransmitting = false;
                    return false;
                }

                timesOfRetry--;
                num = 0;
                flagRetry = false;
            }

        return false;
    }

    private bool ReadConfigData()
    {
        var array = new byte[4] { 83, 31, 192, 64 };
        eepAddr = 8128;
        while (flagTransmitting)
            if (!flagRetry)
            {
                switch (state)
                {
                    case STATE.ReadStep1:
                        sP.WriteByte(array, 0, 4);
                        state = STATE.ReadStep2;
                        timer.Start();
                        break;
                    case STATE.ReadStep2:
                        if (sP.BytesToReadFromCache < array[3] + 4) break;

                        timer.Stop();
                        resetRetryCount();
                        sP.ReadByte(bufForData, 0, sP.BytesToReadFromCache);
                        if (bufForData[1] != array[1] || bufForData[2] != array[2]) break;

                        if (eepAddr == 8128)
                        {
                            var array2 = new byte[4][];
                            for (var i = 0; i < 4; i++)
                            {
                                array2[i] = new byte[16];
                                for (var j = 0; j < 16; j++) array2[i][j] = bufForData[4 + i * 16 + j];
                            }

                            cfgData.TheMinFreqOfVHF =
                                (HexToInt(array2[0][1]) * 100 + HexToInt(array2[0][2])).ToString();
                            cfgData.TheMaxFreqOfVHF =
                                (HexToInt(array2[0][3]) * 100 + HexToInt(array2[0][4])).ToString();
                            cfgData.TheMinFreqOfUHF =
                                (HexToInt(array2[0][6]) * 100 + HexToInt(array2[0][7])).ToString();
                            cfgData.TheMaxFreqOfUHF =
                                (HexToInt(array2[0][8]) * 100 + HexToInt(array2[0][9])).ToString();
                            if (cfgData.TheMinFreqOfUHF == "433")
                                cfgData.TheRangeOfUHF = 1;
                            else
                                cfgData.TheRangeOfUHF = 0;

                            if (cfgData.TheMaxFreqOfVHF == "174")
                                cfgData.TheRangeOfVHF = 0;
                            else if (cfgData.TheMaxFreqOfVHF == "149")
                                cfgData.TheRangeOfVHF = 1;
                            else if (cfgData.TheMaxFreqOfVHF == "260")
                                cfgData.TheRangeOfVHF = 2;
                            else
                                cfgData.TheRangeOfVHF = 3;

                            if ((array2[0][10] & 1) == 1)
                                cfgData.EnableTxOver480M = true;
                            else
                                cfgData.EnableTxOver480M = false;
                        }
                        else if (eepAddr == 7872)
                        {
                            var array3 = new byte[4][];
                            for (var k = 0; k < 4; k++)
                            {
                                array3[k] = new byte[16];
                                for (var l = 0; l < 16; l++) array3[k][l] = bufForData[4 + k * 16 + l];
                            }

                            var text = "";
                            var aSCIIEncoding = new ASCIIEncoding();
                            for (var m = 0; m < 7 && array3[2][m] != byte.MaxValue; m++)
                                text += aSCIIEncoding.GetString(array3[2], m, 1);

                            cfgData.PowerUpChar1 = text;
                            text = "";
                            for (var n = 7; n < 14 && array3[2][n] != byte.MaxValue; n++)
                                text += aSCIIEncoding.GetString(array3[2], n, 1);

                            cfgData.PowerUpChar2 = text;
                        }

                        timer.Start();
                        sP.WriteByte(6);
                        if (eepAddr == 8128)
                        {
                            eepAddr = 7872;
                            array[1] = (byte)(eepAddr >> 8);
                            array[2] = (byte)eepAddr;
                            sP.WriteByte(array, 0, 4);
                            state = STATE.ReadStep3;
                            break;
                        }

                        flagTransmitting = false;
                        return true;
                    case STATE.ReadStep3:
                        if (sP.BytesToReadFromCache >= 1)
                        {
                            timer.Stop();
                            resetRetryCount();
                            sP.ReadByte(bufForData, 0, 1);
                            if (bufForData[0] == 6) state = STATE.ReadStep2;
                        }

                        break;
                }
            }
            else
            {
                if (timesOfRetry <= 0)
                {
                    flagTransmitting = false;
                    return false;
                }

                timesOfRetry--;
                flagRetry = false;
            }

        return false;
    }

    private bool WriteConfigData()
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
        eepAddr = 8128;
        while (flagTransmitting)
            if (!flagRetry)
            {
                switch (state)
                {
                    case STATE.WriteStep1:
                        if (eepAddr == 8128)
                        {
                            ushort num = 0;
                            num = ushort.Parse(cfgData.TheMinFreqOfVHF);
                            array[5] = (byte)(num / 100);
                            array[6] = (byte)(num % 100 / 10 * 16 + num % 10);
                            num = ushort.Parse(cfgData.TheMaxFreqOfVHF);
                            array[7] = (byte)(num / 100);
                            array[8] = (byte)(num % 100 / 10 * 16 + num % 10);
                            num = ushort.Parse(cfgData.TheMinFreqOfUHF);
                            array[10] = (byte)(num / 100);
                            array[11] = (byte)(num % 100 / 10 * 16 + num % 10);
                            num = ushort.Parse(cfgData.TheMaxFreqOfUHF);
                            array[12] = (byte)(num / 100);
                            array[13] = (byte)(num % 100 / 10 * 16 + num % 10);
                            if (cfgData.EnableTxOver480M)
                                array[14] = 1;
                            else
                                array[14] = 0;

                            array3 = array;
                        }
                        else if (eepAddr == 7904)
                        {
                            var powerUpChar = cfgData.PowerUpChar1;
                            var powerUpChar2 = cfgData.PowerUpChar2;
                            if (powerUpChar == null || powerUpChar == "")
                            {
                                for (var i = 4; i < 11; i++) array2[i] = 32;
                            }
                            else
                            {
                                var num2 = 7 - cfgData.PowerUpChar1.Length;
                                var num3 = num2 / 2;
                                var bytes = Encoding.ASCII.GetBytes(cfgData.PowerUpChar1);
                                for (var j = 4 + num3; j < 4 + num3 + cfgData.PowerUpChar1.Length; j++)
                                    array2[j] = bytes[j - 4 - num3];

                                num2 = 7 - cfgData.PowerUpChar2.Length;
                                num3 = num2 / 2;
                                bytes = Encoding.ASCII.GetBytes(cfgData.PowerUpChar2);
                                for (var k = 11 + num3; k < 11 + num3 + cfgData.PowerUpChar2.Length; k++)
                                    array2[k] = bytes[k - 11 - num3];

                                array3 = array2;
                            }
                        }
                        else
                        {
                            ushort num4 = 0;
                            num4 = ushort.Parse(cfgData.TheMinFreqOfVHF);
                            if (num4 == 136)
                                array3[4] = 0;
                            else
                                array3[4] = 85;

                            array3[3] = 1;
                        }

                        sP.WriteByte(array3, 0, array3[3] + 4);
                        timer.Start();
                        state = STATE.WriteStep2;
                        break;
                    case STATE.WriteStep2:
                        if (sP.BytesToReadFromCache < 1) break;

                        sP.ReadByte(bufForData, 0, sP.BytesToReadFromCache);
                        if (bufForData[0] != 6) break;

                        timer.Stop();
                        resetRetryCount();
                        if (eepAddr == 8128)
                        {
                            eepAddr = 7904;
                            array2[1] = (byte)(eepAddr >> 8);
                            array2[2] = (byte)eepAddr;
                            state = STATE.WriteStep1;
                            break;
                        }

                        if (eepAddr == 7904)
                        {
                            eepAddr = 7872;
                            array2[1] = (byte)(eepAddr >> 8);
                            array2[2] = (byte)eepAddr;
                            state = STATE.WriteStep1;
                            break;
                        }

                        flagTransmitting = false;
                        return true;
                }
            }
            else
            {
                if (timesOfRetry <= 0)
                {
                    flagTransmitting = false;
                    return false;
                }

                timesOfRetry--;
                flagRetry = false;
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
        array[4] = GetCHSignalCode(strDat[11]);
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
                if (strDat == Table_QT[i])
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

    private byte GetCHSignalCode(string strDat)
    {
        return (byte)(byte.Parse(strDat) - 1);
    }

    private byte GetCHTxPower(string strDat)
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

        if (text5 == "H")
            array[1] = 0;
        else
            array[1] = 1;

        array[2] = 0;
        if (text == "N") array[2] |= 64;

        if (text7 == "ON") array[2] |= 1;

        if (text3 == "ON") array[2] |= 8;

        if (text4 == "ON") array[2] |= 4;

        if (text6 == "Yes") array[2] |= 2;

        return array;
    }

    private byte[] SetDataVFOFreq(string vfoFreq)
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

    private byte[] SetDataVFORemainFreq(string strRemainFreq)
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

    private byte[] SetDataVFOQT(string rxQt, string txQt)
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

    private byte[] SetDataVFOCfgA(ClassTheRadioData theRadioData)
    {
        var array = new byte[6] { 0, 0, 0, 0, 0, 0 };
        if (theRadioData.funCfgData.CbB_A_RemainDir != 0)
        {
            if (theRadioData.funCfgData.CbB_A_RemainDir == 1)
                array[0] = 16;
            else
                array[0] = 32;
        }

        array[0] &= 240;
        array[0] += (byte)theRadioData.funCfgData.CbB_A_SignalingEnCoder;
        array[1] = 0;
        array[2] = (byte)theRadioData.funCfgData.CbB_A_Power;
        array[3] = 0;
        if (theRadioData.funCfgData.CbB_A_CHBand == 1) array[3] |= 64;

        if (theRadioData.funCfgData.CbB_A_Fhss == 1) array[3] |= 1;

        array[4] = (byte)theRadioData.funCfgData.CbB_A_Band;
        array[5] = (byte)theRadioData.funCfgData.CbB_A_FreqStep;
        return array;
    }

    private byte[] SetDataVFOCfgB(ClassTheRadioData theRadioData)
    {
        var array = new byte[6] { 0, 0, 0, 0, 0, 0 };
        if (theRadioData.funCfgData.CbB_B_RemainDir != 0)
        {
            if (theRadioData.funCfgData.CbB_B_RemainDir == 1)
                array[0] = 16;
            else
                array[0] = 32;
        }

        array[0] &= 240;
        array[0] += (byte)theRadioData.funCfgData.CbB_B_SignalingEnCoder;
        array[1] = 0;
        array[2] = (byte)theRadioData.funCfgData.CbB_B_Power;
        array[3] = 0;
        if (theRadioData.funCfgData.CbB_B_CHBand == 1) array[3] |= 64;

        if (theRadioData.funCfgData.CbB_B_Fhss == 1) array[3] |= 1;

        array[4] = (byte)theRadioData.funCfgData.CbB_B_Band;
        array[5] = (byte)theRadioData.funCfgData.CbB_B_FreqStep;
        return array;
    }

    private byte[] SetCHNaemToHex(string[] channelData)
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
            num = bytes.Length > 12 ? 12 : bytes.Length;
            for (var i = 0; i < num; i++) array[i] = bytes[i];
        }

        return array;
    }

    private byte[] SetDTMFTOHex(string dtmfCode)
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

    private void GetCHImfo_HexToStr(ClassTheRadioData theRadioData, int NO_CH, byte[] dat)
    {
        if (dat[0] == byte.MaxValue)
        {
            for (var i = 0; i < 13; i++) theRadioData.ChannelData[NO_CH][i] = null;

            theRadioData.ChannelData[NO_CH][0] = NO_CH.ToString();
        }
        else
        {
            GetFreqImf_HexToStr(theRadioData.ChannelData[NO_CH], dat);
            GetQTImf_HexToStr(theRadioData.ChannelData[NO_CH], dat);
            GetCHOtherImf(theRadioData.ChannelData[NO_CH], dat);
        }
    }

    private void GetFreqImf_HexToStr(string[] channelDat, byte[] dat)
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
        channelDat[2] = CaculateFreq_HexToStr(dat2);
        channelDat[4] = CaculateFreq_HexToStr(dat3);
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

    private void GetQTImf_HexToStr(string[] channelDat, byte[] dat)
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
        if (GetTypeOfSubaudio_HexToStr(array[1]) == SUBAUDIO_TYPE.CTCSS)
            channelDat[3] = CaculateCTCSS_HexToStr(array);
        else
            channelDat[3] = CaculateCDCSS_HexToStr(array);

        if (GetTypeOfSubaudio_HexToStr(array2[1]) == SUBAUDIO_TYPE.CTCSS)
            channelDat[5] = CaculateCTCSS_HexToStr(array2);
        else
            channelDat[5] = CaculateCDCSS_HexToStr(array2);
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
        if (dat[0] != 0 && dat[0] <= 210) return Table_QT[dat[0] - 1];

        return "OFF";
    }

    private SUBAUDIO_TYPE GetTypeOfSubaudio_HexToStr(byte dat)
    {
        if (dat == 0) return SUBAUDIO_TYPE.CDCSS;

        return SUBAUDIO_TYPE.CTCSS;
    }

    private void GetCHOtherImf(string[] channelDat, byte[] dat)
    {
        if (dat[12] >= 15) dat[12] = 0;

        channelDat[11] = (dat[12] + 1).ToString();
        var array = new string[4] { "OFF", "BOT", "EOT", "BOTH" };
        if (dat[13] > 3) dat[13] = 0;

        channelDat[8] = array[dat[13]];
        var array2 = new string[2] { "H", "L" };
        if (dat[14] > 1) dat[14] = 0;

        channelDat[6] = array2[dat[14]];
        if ((dat[15] & 0x40) == 64)
            channelDat[7] = "N";
        else
            channelDat[7] = "W";

        if ((dat[15] & 8) == 8)
            channelDat[9] = "ON";
        else
            channelDat[9] = "OFF";

        if ((dat[15] & 4) == 4)
            channelDat[10] = "ON";
        else
            channelDat[10] = "OFF";

        if ((dat[15] & 2) == 2)
            channelDat[1] = "Yes";
        else
            channelDat[1] = "No";

        if ((dat[15] & 1) == 1)
            channelDat[13] = "ON";
        else
            channelDat[13] = "OFF";
    }

    private void GetTheNameOfCH(string[] channelDat, byte[] dat)
    {
        var text = "";
        var aSCIIEncoding = new ASCIIEncoding();
        var encoding = Encoding.GetEncoding("gb2312");
        var num = 0;
        while (num < 12 && dat[num] != byte.MaxValue)
            if (dat[num] >= 161)
            {
                text += encoding.GetString(dat, num, 2);
                num += 2;
            }
            else
            {
                text += aSCIIEncoding.GetString(dat, num, 1);
                num++;
            }

        channelDat[12] = text;
    }

    private string ConvertHex2Str_DTMF(byte[] dat)
    {
        var text = "";
        for (var i = 0; i < 5 && dat[i] != byte.MaxValue; i++) text += "0123456789ABCD*#"[dat[i]];

        return text;
    }

    private string GetTheLocalID(byte[] dat)
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
        theRadioData.funCfgData.CbB_SQL = dat[0];
        if (dat[1] > 2) dat[1] = 1;

        theRadioData.funCfgData.CbB_SaveMode = dat[1];
        theRadioData.funCfgData.CbB_VOX = dat[2];
        theRadioData.funCfgData.CbB_AutoBackLight = dat[3];
        if (dat[4] > 0)
            theRadioData.funCfgData.CB_TDR = true;
        else
            theRadioData.funCfgData.CB_TDR = false;

        theRadioData.funCfgData.CbB_TOT = dat[5];
        if (dat[6] > 0)
            theRadioData.funCfgData.CB_SoundOfBi = true;
        else
            theRadioData.funCfgData.CB_SoundOfBi = false;

        theRadioData.funCfgData.CbB_VoicSwitch = dat[7];
        if (dat[8] >= 2) dat[8] = 1;

        theRadioData.funCfgData.CbB_Language = dat[8];
        theRadioData.funCfgData.CbB_DTMF = dat[9];
        theRadioData.funCfgData.CbB_Scan = dat[10];
        if (dat[11] > 3) dat[11] = 0;

        theRadioData.funCfgData.CbB_PTTID = dat[11];
        theRadioData.funCfgData.CbB_SendIDDelay = dat[12];
        theRadioData.funCfgData.CbB_CH_A_DisplayMode = dat[13];
        theRadioData.funCfgData.CbB_CH_B_DisplayMode = dat[14];
        if (dat[15] > 0)
            theRadioData.funCfgData.CB_StopSendOnBusy = true;
        else
            theRadioData.funCfgData.CB_StopSendOnBusy = false;
    }

    private void GetFunCfgImf_Part2(ClassTheRadioData theRadioData, byte[] dat)
    {
        if (dat[0] > 0)
            theRadioData.funCfgData.CB_AutoLock = true;
        else
            theRadioData.funCfgData.CB_AutoLock = false;

        theRadioData.funCfgData.CbB_AlarmMode = dat[1];
        if (dat[2] > 0)
            theRadioData.funCfgData.CB_AlarmSound = true;
        else
            theRadioData.funCfgData.CB_AlarmSound = false;

        theRadioData.funCfgData.CbB_TxUnderTDRStart = dat[3];
        theRadioData.funCfgData.CbB_TailNoiseClear = dat[4];
        theRadioData.funCfgData.CbB_PassRepetNoiseClear = dat[5];
        theRadioData.funCfgData.CbB_PassRepetNoiseDetect = dat[6];
        theRadioData.funCfgData.CbB_SoundOfTxEnd = dat[7];
        if (dat[9] > 0)
            theRadioData.funCfgData.CB_FMRadioEnable = false;
        else
            theRadioData.funCfgData.CB_FMRadioEnable = true;

        theRadioData.funCfgData.CbB_WorkModeA = dat[10] & 0xF;
        theRadioData.funCfgData.CbB_WorkModeB = (dat[10] >> 4) & 0xF;
        if (dat[11] > 0)
            theRadioData.funCfgData.CB_LockKeyBoard = true;
        else
            theRadioData.funCfgData.CB_LockKeyBoard = false;

        if (dat[12] == byte.MaxValue) dat[12] = 0;

        theRadioData.funCfgData.CbB_PowerOnMsg = dat[12];
        if (dat[14] > 3) dat[14] = 2;

        theRadioData.funCfgData.CbB_1750Hz = dat[14];
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
        if (GetTypeOfSubaudio_HexToStr(array[1]) == SUBAUDIO_TYPE.CTCSS)
            array3[0] = CaculateCTCSS_HexToStr(array);
        else
            array3[0] = CaculateCDCSS_HexToStr(array);

        if (GetTypeOfSubaudio_HexToStr(array2[1]) == SUBAUDIO_TYPE.CTCSS)
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
        theRadioData.funCfgData.TB_A_CurFreq = GetVFO_CurFreq(dat3);
        array = GetVF0_QTImf(dat4);
        theRadioData.funCfgData.CbB_A_RxQT = array[0];
        theRadioData.funCfgData.CbB_A_TxQT = array[1];
        theRadioData.funCfgData.CbB_A_RemainDir = GetVFO_DirOfRemainFreq(dat[14]);
        theRadioData.funCfgData.CbB_A_SignalingEnCoder = GetVFO_SignalingCode(dat[14]);
        theRadioData.funCfgData.CbB_A_Power = dat2[0];
        theRadioData.funCfgData.CbB_A_CHBand = GetVFO_BandWide(dat2[1]);
        theRadioData.funCfgData.CbB_A_Fhss = GetVFO_Fhss(dat2[1]);
        theRadioData.funCfgData.CbB_A_Band = dat2[2];
        theRadioData.funCfgData.CbB_A_FreqStep = dat2[3] % 8;
        theRadioData.funCfgData.TB_A_RemainFreq = GetVFO_RemainFreq(dat5);
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
        theRadioData.funCfgData.TB_B_CurFreq = GetVFO_CurFreq(dat3);
        array = GetVF0_QTImf(dat4);
        theRadioData.funCfgData.CbB_B_RxQT = array[0];
        theRadioData.funCfgData.CbB_B_TxQT = array[1];
        theRadioData.funCfgData.CbB_B_RemainDir = GetVFO_DirOfRemainFreq(dat[14]);
        theRadioData.funCfgData.CbB_B_SignalingEnCoder = GetVFO_SignalingCode(dat[14]);
        theRadioData.funCfgData.CbB_B_Power = dat2[0];
        theRadioData.funCfgData.CbB_B_CHBand = GetVFO_BandWide(dat2[1]);
        theRadioData.funCfgData.CbB_B_Fhss = GetVFO_Fhss(dat2[1]);
        theRadioData.funCfgData.CbB_B_Band = dat2[2];
        theRadioData.funCfgData.CbB_B_FreqStep = dat2[3] % 8;
        theRadioData.funCfgData.TB_B_RemainFreq = GetVFO_RemainFreq(dat5);
    }

    private short HexToInt(byte data)
    {
        short num = 0;
        return (short)(((data & 0xF0) >> 4) * 10 + (data & 0xF));
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        flagRetry = true;
    }
}