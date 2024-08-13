using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Timers;
using SenhaixFreqWriter.Constants.Common;
using SenhaixFreqWriter.Constants.Shx8x00;
using SenhaixFreqWriter.Views.Common;
using SkiaSharp;
using Timer = System.Timers.Timer;

namespace SenhaixFreqWriter.Utils.Serial;

public class WriBootImage
{
    private readonly MySerialPort _sp = MySerialPort.GetInstance();

    private readonly byte[] bufferBmpData = new byte[1048576];

    private readonly byte[] bufForData = new byte[2048];

    private uint byteOfData;

    private int cntRetry;

    private State comStep = State.HandShakeStep1;

    private NImgStep NComStep = NImgStep.Step_HandShake;

    private int countOverTime;

    public ConcurrentQueue<int> currentProg = new();

    private bool flagOverTime;

    private bool flagReceivePackageOver = false;

    private readonly SKBitmap image;

    private Timer overTimer;

    private Timer rxOverTimer;

    private string STR_HANDSHAKE = "PROGRAM";

    private int progressValue = 0;

    private int packageLength = 0;

    private int address = 0;

    private int blockOfErase = 0;

    private int byteOfPackage = 0;

    private int totalPackages = 0;

    private int cntPackages = 0;

    private int packageID = 0;

    private SHX_DEVICE _device;

    private CancellationTokenSource wriImgTokenSource;

    public WriBootImage(SHX_DEVICE device, SKBitmap img)
    {
        _device = device;
        image = img;
        comStep = State.HandShakeStep1;
        NComStep = NImgStep.Step_HandShake;
        if (device is SHX_DEVICE.SHX8600PRO)
        {
            TimerInitPro();
            _sp.OpenSerial8600Pro();
        }else if (device is SHX_DEVICE.SHX8800PRO)
        {
            TimerInitPro();
            _sp.OpenSerial8800Pro();
        }
        else
        {
            TimerInit();
            _sp.OpenSerial();
        }
    }


    private void TimerInit()
    {
        overTimer = new Timer();
        overTimer.Elapsed += OverTimer_Elapsed;
        overTimer.Interval = 200.0;
        overTimer.AutoReset = true;
        overTimer.Enabled = true;
        rxOverTimer = new Timer();
        rxOverTimer.Elapsed += RxOverTimer_Elapsed;
        rxOverTimer.Interval = 30.0;
        rxOverTimer.AutoReset = false;
        rxOverTimer.Enabled = true;
    }

    private void TimerInitPro()
    {
        overTimer = new Timer();
        overTimer.Elapsed += OverTimer_Elapsed;
        overTimer.Interval = 1000.0;
        overTimer.AutoReset = false;
        overTimer.Enabled = true;
        rxOverTimer = new Timer();
        rxOverTimer.Elapsed += RxOverTimer_Elapsed;
        rxOverTimer.Interval = 30.0;
        rxOverTimer.AutoReset = false;
        rxOverTimer.Enabled = true;
    }

    private void RxOverTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        flagReceivePackageOver = true;
    }

    private void OverTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (_device == SHX_DEVICE.SHX8600PRO)
        {
            flagOverTime = true;
        }
        else
        {
            if (cntRetry > 0)
                cntRetry--;
            else
                flagOverTime = true;
        }
    }

    private void OverTimer_Start()
    {
        overTimer.Start();
        flagReceivePackageOver = false;
        countOverTime = 5;
        cntRetry = 3;
    }

    public void CancelWriteImg()
    {
        wriImgTokenSource.Cancel();
    }

    public bool WriteImg()
    {
        wriImgTokenSource = new CancellationTokenSource();
        comStep = State.HandShakeStep1;
        NComStep = NImgStep.Step_HandShake_Jump1;
        // Bitmap bitmap2 = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
        // for (int i = 0; i < image.Width; i++)
        // {
        // 	for (int j = 0; j < image.Height; j++)
        // 	{
        // 		Color pixel = bitmap.GetPixel(i, j);
        // 		bitmap2.SetPixel(i, j, pixel);
        // 	}
        // }
        // int width = bitmap2.Width;
        // int height = bitmap2.Height;
        byteOfData = 0u;
        for (var k = 0; k < bufferBmpData.Length; k++) bufferBmpData[k] = byte.MaxValue;
        // if (bitmap2.PixelFormat == PixelFormat.Format24bppRgb)
        // {
        for (var l = 0; l < image.Height; l++)
        for (var m = 0; m < image.Width; m++)
        {
            var pixel = image.GetPixel(m, l);
            var num = pixel.Red >> 3;
            var num2 = pixel.Green >> 2;
            var num3 = pixel.Blue >> 3;
            var num4 = (num << 11) | (num2 << 5) | num3;
            bufferBmpData[byteOfData++] = (byte)num4;
            bufferBmpData[byteOfData++] = (byte)(num4 >> 8);
        }

        try
        {
            if (_device == SHX_DEVICE.SHX8600 || _device == SHX_DEVICE.SHX8800)
            {
                DebugWindow.GetInstance().updateDebugContent("使用普通8x00");
                return HandShake() && Communication();
            }
            else
            {
                DebugWindow.GetInstance().updateDebugContent("使用新版8600");
                return NHandShake() && NCommunication();
            }
        }
        catch (Exception ae)
        {
            DebugWindow.GetInstance().updateDebugContent($"写图片出错：{ae.Message}");
            return false;
        }
        finally
        {
            _sp.CloseSerial();
        }
        // }
        // MessageBox.Show("格式错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        // return false;
    }

    public bool HandShake()
    {
        var array = new byte[1];
        while (!wriImgTokenSource.Token.IsCancellationRequested)
            if (!flagOverTime)
            {
                switch (comStep)
                {
                    case State.HandShakeStep1:
                        array = Encoding.ASCII.GetBytes("PROGROMSHXU");
                        _sp.WriteByte(array, 0, array.Length);
                        OverTimer_Start();
                        comStep = State.HandShakeStep2;
                        break;
                    case State.HandShakeStep2:
                        if (_sp.BytesToReadFromCache >= 1)
                        {
                            _sp.ReadByte(bufForData, 0, 1);
                            if (bufForData[0] == 6)
                            {
                                array = new byte[1] { 70 };
                                _sp.WriteByte(array, 0, 1);
                                comStep = State.HandShakeStep3;
                            }
                        }

                        break;
                    case State.HandShakeStep3:
                        if (_sp.BytesToReadFromCache >= 8)
                        {
                            _sp.ReadByte(bufForData, 0, 8);
                            comStep = State.WriteStep1;
                            return true;
                        }

                        break;
                }
            }
            else
            {
                if (countOverTime <= 0) break;
                countOverTime--;
                cntRetry = 3;
                flagOverTime = false;
                if (comStep == State.HandShakeStep2)
                {
                    array = Encoding.ASCII.GetBytes("PROGROMSHXU");
                    _sp.WriteByte(array, 0, array.Length);
                }
                else
                {
                    _sp.WriteByte(array, 0, array.Length);
                }
            }

        return false;
    }

    public bool Communication()
    {
        var array = new byte[68]
        {
            73, 0, 0, 64, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255
        };
        var flag = true;
        var num = 0;
        var num2 = 0;
        while (!wriImgTokenSource.Token.IsCancellationRequested)
            if (!flagOverTime)
            {
                switch (comStep)
                {
                    case State.WriteStep1:
                    {
                        var array2 = new byte[1024];
                        if (num < byteOfData)
                        {
                            if (flag)
                            {
                                Array.Copy(bufferBmpData, num, array2, 16, 48);
                                array2[0] = 23;
                                array2[1] = 9;
                                array2[2] = 34;
                                array2[3] = 32;
                                num += 48;
                                flag = false;
                            }
                            else
                            {
                                Array.Copy(bufferBmpData, num, array2, 0, 64);
                                num += 64;
                            }

                            for (var i = 0; i < 64; i++) array[i + 4] = array2[i];
                            array[1] = (byte)(num2 >> 8);
                            array[2] = (byte)num2;
                            _sp.WriteByte(array, 0, 68);
                            currentProg.Enqueue((int)(num * 100 / byteOfData));
                            comStep = State.WriteStep2;
                            break;
                        }

                        currentProg.Enqueue(100);
                        return true;
                    }
                    case State.WriteStep2:
                        if (_sp.BytesToReadFromCache >= 1)
                        {
                            _sp.ReadByte(bufForData, 0, 1);
                            if (bufForData[0] == 6)
                            {
                                num2 += 64;
                                rxOverTimer.Stop();
                                OverTimer_Start();
                                comStep = State.WriteStep1;
                            }
                        }

                        break;
                }
            }
            else
            {
                if (countOverTime <= 0) break;
                countOverTime--;
                cntRetry = 3;
                flagOverTime = false;
                _sp.WriteByte(array, 0, 68);
            }

        return false;
    }

    public bool NHandShake()
    {
        var array = new byte[1];
        while (!wriImgTokenSource.Token.IsCancellationRequested)
            if (!flagOverTime)
            {
                switch (NComStep)
                {
                    case NImgStep.Step_HandShake_Jump1:
                        array = Encoding.ASCII.GetBytes("PROGROMSHXU");
                        _sp.WriteByte(array, 0, array.Length);
                        NComStep = NImgStep.Step_HandShake_Jump2;
                        OverTimer_Start();
                        break;
                    case NImgStep.Step_HandShake_Jump2:
                        if (_sp.BytesToReadFromCache >= 1)
                        {
                            _sp.ReadByte(bufForData, 0, 1);
                            if (bufForData[0] == 6)
                            {
                                bufForData[0] = 68;
                                _sp.WriteByte(bufForData, 0, 1);
                                NComStep = NImgStep.Step_HandShake;
                                overTimer.Stop();
                                // _sp.CloseSerial();
                                // Thread.Sleep(100);
                                // _sp.OpenSerialProWithHigherBaudrate();
                                // OVERRIDE SETTINGS
                                _sp.Close();
                                _sp.BaudRate = 115200;
                                _sp.Open();
                                Thread.Sleep(100);
                                // DebugWindow.GetInstance().updateDebugContent("Successfully handshake!");
                                return true;
                            }
                        }

                        break;
                }
            }
            else
            {
                if (countOverTime <= 0) break;
                countOverTime--;
                flagOverTime = false;
                var NImgStep = NComStep;
                if (NImgStep == NImgStep.Step_HandShake_Jump2) _sp.WriteByte(array, 0, array.Length);
                overTimer.Start();
                flagReceivePackageOver = false;
            }

        _sp.CloseSerial();
        return false;
    }

    public bool NCommunication()
    {
        var num = 0;
        var readFailedTimes = 0;
        while (!wriImgTokenSource.Token.IsCancellationRequested)
        {
            if (!flagOverTime)
            {
                switch (NComStep)
                {
                    case NImgStep.Step_HandShake:
                    {
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.Step_HandShake");
                        var array = Encoding.ASCII.GetBytes(STR_HANDSHAKE);
                        packageLength = LoadPackage(NCommandType.CMD_HANDSHAKE, 0, array.Length, array);
                        _sp.WriteByte(bufForData, 0, packageLength);
                        OverTimer_Start();
                        NComStep = NImgStep.Step_Receive_1;
                        currentProg.Enqueue(3);
                        break;
                    }
                    case NImgStep.Step_Over:
                    {
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.Step_Over");
                        var array = Encoding.ASCII.GetBytes("Over");
                        packageLength = LoadPackage(NCommandType.CMD_OVER, 0, array.Length, array);
                        _sp.WriteByte(bufForData, 0, packageLength);
                        currentProg.Enqueue(100);
                        Thread.Sleep(100);
                        _sp.CloseSerial();
                        return true;
                    }
                    case NImgStep.Step_SetFontAddress:
                    case NImgStep.Step_SetImageAddress:
                    case NImgStep.Step_SetVoiceAddress:
                    {
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.3set");
                        var array = new byte[4];
                        array[3] = (byte)((uint)(address >> 24) & 0xFFu);
                        array[2] = (byte)((uint)(address >> 16) & 0xFFu);
                        array[1] = (byte)((uint)(address >> 8) & 0xFFu);
                        array[0] = (byte)((uint)address & 0xFFu);
                        packageLength = LoadPackage(NCommandType.CMD_SETADDRESS, 0, array.Length, array);
                        _sp.WriteByte(bufForData, 0, packageLength);
                        OverTimer_Start();
                        NComStep = NImgStep.Step_Receive_1;
                        break;
                    }
                    case NImgStep.Step_Erase:
                    {
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.erase");
                        var num2 = 17668;
                        var array = new byte[6];
                        array[3] = (byte)((uint)(address >> 24) & 0xFFu);
                        array[2] = (byte)((uint)(address >> 16) & 0xFFu);
                        array[1] = (byte)((uint)(address >> 8) & 0xFFu);
                        array[0] = (byte)((uint)address & 0xFFu);
                        array[4] = (byte)((uint)(blockOfErase >> 8) & 0xFFu);
                        array[5] = (byte)((uint)blockOfErase & 0xFFu);
                        packageLength = LoadPackage(NCommandType.CMD_ERASE, num2, 6, array);
                        _sp.WriteByte(bufForData, 0, packageLength);
                        OverTimer_Start();
                        NComStep = NImgStep.Step_Receive_1;
                        break;
                    }
                    case NImgStep.Step_Data:
                    {
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.data");
                        var array = new byte[1024];
                        Array.Copy(bufferBmpData, num, array, 0, 1024);
                        if (num <= byteOfData)
                        {
                            packageLength = LoadPackage(NCommandType.CMD_WRITE, packageID, 1024, array);
                            _sp.WriteByte(bufForData, 0, packageLength);
                            OverTimer_Start();
                            NComStep = NImgStep.Step_Receive_1;
                            num += 1024;
                        }
                        else
                        {
                            NComStep = NImgStep.Step_Over;
                        }

                        break;
                    }
                    case NImgStep.Step_Receive_1:
                        // DebugWindow.GetInstance().updateDebugContent($"NImgStep.recv1:{readFailedTimes}");
                        if (_sp.BytesToReadFromCache >= 1)
                        {
                            readFailedTimes = 0;
                            // DebugWindow.GetInstance().updateDebugContent("Gonna read something out...");
                            _sp.ReadByte(bufForData, 0, 1);
                            if (bufForData[0] == 165)
                            {
                                NComStep = NImgStep.Step_Receive_2;
                                flagReceivePackageOver = false;
                                byteOfPackage = _sp.BytesToReadFromCache;
                                rxOverTimer.Start();
                            }
                        }
                        else
                        {
                            readFailedTimes++;
                            // or more..
                            Thread.Sleep(5);
                            if (readFailedTimes > 200)
                            {
                                _sp.CloseSerial();
                                return false;
                            }
                        }

                        break;
                    case NImgStep.Step_Receive_2:
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.recv2");
                        if (flagReceivePackageOver)
                        {
                            flagReceivePackageOver = false;
                            _sp.ReadByte(bufForData, 1, _sp.BytesToReadFromCache);
                            var array = AnalysisPackage(bufForData);
                            overTimer.Stop();
                            if (array == null) break;
                            if (array[0] == 89 && array.Length == 1)
                                switch (bufForData[1])
                                {
                                    case 2:
                                        address = 65536;
                                        blockOfErase = 1;
                                        currentProg.Enqueue(progressValue);
                                        packageID = 0;
                                        cntPackages = (int)(byteOfData / 1024);
                                        if (byteOfData % 1024 != 0) cntPackages++;
                                        totalPackages = cntPackages;
                                        NComStep = NImgStep.Step_Erase;
                                        break;
                                    case 3:
                                        NComStep = NImgStep.Step_Data;
                                        break;
                                    case 87:
                                        cntPackages--;
                                        if (cntPackages == 0)
                                        {
                                            NComStep = NImgStep.Step_Over;
                                            break;
                                        }

                                        packageID++;
                                        progressValue = packageID * 100 / totalPackages;
                                        currentProg.Enqueue(progressValue);
                                        NComStep = NImgStep.Step_Data;
                                        break;
                                    case 4:
                                        NComStep = NImgStep.Step_SetImageAddress;
                                        break;
                                }
                            else
                                NComStep = NImgStep.Step_Over;
                        }
                        else if (byteOfPackage != _sp.BytesToReadFromCache)
                        {
                            byteOfPackage = _sp.BytesToReadFromCache;
                            rxOverTimer.Stop();
                            rxOverTimer.Start();
                        }

                        break;
                }

                continue;
            }

            if (countOverTime <= 0) break;
            countOverTime--;
            flagOverTime = false;
            if (NComStep == NImgStep.Step_Receive_1 || NComStep == NImgStep.Step_Receive_2)
                switch (bufForData[1])
                {
                    case 2:
                        NComStep = NImgStep.Step_HandShake;
                        break;
                    case 3:
                        NComStep = NImgStep.Step_SetFontAddress;
                        break;
                    case 4:
                        NComStep = NImgStep.Step_Erase;
                        break;
                    case 87:
                        _sp.WriteByte(bufForData, 0, packageLength);
                        overTimer.Start();
                        flagReceivePackageOver = false;
                        NComStep = NImgStep.Step_Receive_1;
                        break;
                }
        }

        _sp.CloseSerial();
        return false;
    }

    private int LoadPackage(NCommandType cmd, int packageID, int len, byte[] dat)
    {
        bufForData[0] = 165;
        bufForData[1] = (byte)cmd;
        bufForData[2] = (byte)((uint)(packageID >> 8) & 0xFFu);
        bufForData[3] = (byte)((uint)packageID & 0xFFu);
        bufForData[4] = (byte)((uint)(len >> 8) & 0xFFu);
        bufForData[5] = (byte)((uint)len & 0xFFu);
        for (var i = 0; i < len; i++) bufForData[6 + i] = dat[i];
        var num = CrcValidation(bufForData, 1, 5 + len);
        bufForData[6 + len] = (byte)((uint)(num >> 8) & 0xFFu);
        bufForData[6 + len + 1] = (byte)((uint)num & 0xFFu);
        return 6 + len + 2;
    }

    private byte[] AnalysisPackage(byte[] buffer)
    {
        var num = 0;
        num = buffer[4];
        num <<= 8;
        num |= buffer[5];
        byte[] array;
        if (num != 0)
        {
            array = new byte[num];
            for (var i = 0; i < num; i++) array[i] = buffer[6 + i];
            int num2 = buffer[6 + num];
            num2 <<= 8;
            num2 |= buffer[6 + num + 1];
            var num3 = CrcValidation(buffer, 1, num + 5) & 0xFFFF;
            if (num2 != num3) array = null;
        }
        else
        {
            array = null;
        }

        return array;
    }

    private int CrcValidation(byte[] dat, int offset, int count)
    {
        var num = 0;
        for (var i = 0; i < count; i++)
        {
            int num2 = dat[i + offset];
            num ^= num2 << 8;
            for (var j = 0; j < 8; j++) num = (num & 0x8000) != 32768 ? num << 1 : (num << 1) ^ 0x1021;
        }

        return num;
    }
}