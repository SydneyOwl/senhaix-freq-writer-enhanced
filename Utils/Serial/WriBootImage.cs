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
    private readonly byte[] _bufferBmpData = new byte[1048576];

    private readonly byte[] _bufForData = new byte[2048];

    private readonly SKBitmap _image;
    private readonly MySerialPort _sp = MySerialPort.GetInstance();

    private int _address;

    private int _blockOfErase;

    private uint _byteOfData;

    private int _byteOfPackage;

    private int _cntPackages;

    private int _cntRetry;

    private State _comStep = State.HandShakeStep1;

    private int _countOverTime;

    private readonly ShxDevice _device;

    private bool _flagOverTime;

    private bool _flagReceivePackageOver;

    private NImgStep _nComStep = NImgStep.StepHandShake;

    private Timer _overTimer;

    private int _packageId;

    private int _packageLength;

    private int _progressValue;

    private Timer _rxOverTimer;

    private readonly string _strHandshake = "PROGRAM";

    private int _totalPackages;

    private CancellationTokenSource _wriImgTokenSource;

    public ConcurrentQueue<int> CurrentProg = new();

    public WriBootImage(ShxDevice device, SKBitmap img)
    {
        _device = device;
        _image = img;
        _comStep = State.HandShakeStep1;
        _nComStep = NImgStep.StepHandShake;
        if (device is ShxDevice.Shx8600Pro)
        {
            TimerInitPro();
            _sp.OpenSerial8600Pro();
        }
        else if (device is ShxDevice.Shx8800Pro)
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
        _overTimer = new Timer();
        _overTimer.Elapsed += OverTimer_Elapsed;
        _overTimer.Interval = 200.0;
        _overTimer.AutoReset = true;
        _overTimer.Enabled = true;
        _rxOverTimer = new Timer();
        _rxOverTimer.Elapsed += RxOverTimer_Elapsed;
        _rxOverTimer.Interval = 30.0;
        _rxOverTimer.AutoReset = false;
        _rxOverTimer.Enabled = true;
    }

    private void TimerInitPro()
    {
        _overTimer = new Timer();
        _overTimer.Elapsed += OverTimer_Elapsed;
        _overTimer.Interval = 1000.0;
        _overTimer.AutoReset = false;
        _overTimer.Enabled = true;
        _rxOverTimer = new Timer();
        _rxOverTimer.Elapsed += RxOverTimer_Elapsed;
        _rxOverTimer.Interval = 30.0;
        _rxOverTimer.AutoReset = false;
        _rxOverTimer.Enabled = true;
    }

    private void RxOverTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _flagReceivePackageOver = true;
    }

    private void OverTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (_device == ShxDevice.Shx8600Pro)
        {
            _flagOverTime = true;
        }
        else
        {
            if (_cntRetry > 0)
                _cntRetry--;
            else
                _flagOverTime = true;
        }
    }

    private void OverTimer_Start()
    {
        _overTimer.Start();
        _flagReceivePackageOver = false;
        _countOverTime = 5;
        _cntRetry = 3;
    }

    public void CancelWriteImg()
    {
        _wriImgTokenSource.Cancel();
    }

    public bool WriteImg()
    {
        _wriImgTokenSource = new CancellationTokenSource();
        _comStep = State.HandShakeStep1;
        _nComStep = NImgStep.StepHandShakeJump1;
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
        _byteOfData = 0u;
        for (var k = 0; k < _bufferBmpData.Length; k++) _bufferBmpData[k] = byte.MaxValue;
        // if (bitmap2.PixelFormat == PixelFormat.Format24bppRgb)
        // {
        for (var l = 0; l < _image.Height; l++)
        for (var m = 0; m < _image.Width; m++)
        {
            var pixel = _image.GetPixel(m, l);
            var num = pixel.Red >> 3;
            var num2 = pixel.Green >> 2;
            var num3 = pixel.Blue >> 3;
            var num4 = (num << 11) | (num2 << 5) | num3;
            _bufferBmpData[_byteOfData++] = (byte)num4;
            _bufferBmpData[_byteOfData++] = (byte)(num4 >> 8);
        }

        try
        {
            if (_device == ShxDevice.Shx8600 || _device == ShxDevice.Shx8800)
            {
                DebugWindow.GetInstance().UpdateDebugContent("使用普通8x00");
                return HandShake() && Communication();
            }
            else
            {
                DebugWindow.GetInstance().UpdateDebugContent("使用新版8600");
                return NHandShake() && NCommunication();
            }
        }
        catch (Exception ae)
        {
            DebugWindow.GetInstance().UpdateDebugContent($"写图片出错：{ae.Message}");
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
        while (!_wriImgTokenSource.Token.IsCancellationRequested)
            if (!_flagOverTime)
            {
                switch (_comStep)
                {
                    case State.HandShakeStep1:
                        array = Encoding.ASCII.GetBytes("PROGROMSHXU");
                        _sp.WriteByte(array, 0, array.Length);
                        OverTimer_Start();
                        _comStep = State.HandShakeStep2;
                        break;
                    case State.HandShakeStep2:
                        if (_sp.BytesToReadFromCache >= 1)
                        {
                            _sp.ReadByte(_bufForData, 0, 1);
                            if (_bufForData[0] == 6)
                            {
                                array = new byte[1] { 70 };
                                _sp.WriteByte(array, 0, 1);
                                _comStep = State.HandShakeStep3;
                            }
                        }

                        break;
                    case State.HandShakeStep3:
                        if (_sp.BytesToReadFromCache >= 8)
                        {
                            _sp.ReadByte(_bufForData, 0, 8);
                            _comStep = State.WriteStep1;
                            return true;
                        }

                        break;
                }
            }
            else
            {
                if (_countOverTime <= 0) break;
                _countOverTime--;
                _cntRetry = 3;
                _flagOverTime = false;
                if (_comStep == State.HandShakeStep2)
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
        while (!_wriImgTokenSource.Token.IsCancellationRequested)
            if (!_flagOverTime)
            {
                switch (_comStep)
                {
                    case State.WriteStep1:
                    {
                        var array2 = new byte[1024];
                        if (num < _byteOfData)
                        {
                            if (flag)
                            {
                                Array.Copy(_bufferBmpData, num, array2, 16, 48);
                                array2[0] = 23;
                                array2[1] = 9;
                                array2[2] = 34;
                                array2[3] = 32;
                                num += 48;
                                flag = false;
                            }
                            else
                            {
                                Array.Copy(_bufferBmpData, num, array2, 0, 64);
                                num += 64;
                            }

                            for (var i = 0; i < 64; i++) array[i + 4] = array2[i];
                            array[1] = (byte)(num2 >> 8);
                            array[2] = (byte)num2;
                            _sp.WriteByte(array, 0, 68);
                            CurrentProg.Enqueue((int)(num * 100 / _byteOfData));
                            _comStep = State.WriteStep2;
                            break;
                        }

                        CurrentProg.Enqueue(100);
                        return true;
                    }
                    case State.WriteStep2:
                        if (_sp.BytesToReadFromCache >= 1)
                        {
                            _sp.ReadByte(_bufForData, 0, 1);
                            if (_bufForData[0] == 6)
                            {
                                num2 += 64;
                                _rxOverTimer.Stop();
                                OverTimer_Start();
                                _comStep = State.WriteStep1;
                            }
                        }

                        break;
                }
            }
            else
            {
                if (_countOverTime <= 0) break;
                _countOverTime--;
                _cntRetry = 3;
                _flagOverTime = false;
                _sp.WriteByte(array, 0, 68);
            }

        return false;
    }

    public bool NHandShake()
    {
        var array = new byte[1];
        while (!_wriImgTokenSource.Token.IsCancellationRequested)
            if (!_flagOverTime)
            {
                switch (_nComStep)
                {
                    case NImgStep.StepHandShakeJump1:
                        array = Encoding.ASCII.GetBytes("PROGROMSHXU");
                        _sp.WriteByte(array, 0, array.Length);
                        _nComStep = NImgStep.StepHandShakeJump2;
                        OverTimer_Start();
                        break;
                    case NImgStep.StepHandShakeJump2:
                        if (_sp.BytesToReadFromCache >= 1)
                        {
                            _sp.ReadByte(_bufForData, 0, 1);
                            if (_bufForData[0] == 6)
                            {
                                _bufForData[0] = 68;
                                _sp.WriteByte(_bufForData, 0, 1);
                                _nComStep = NImgStep.StepHandShake;
                                _overTimer.Stop();
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
                if (_countOverTime <= 0) break;
                _countOverTime--;
                _flagOverTime = false;
                var nImgStep = _nComStep;
                if (nImgStep == NImgStep.StepHandShakeJump2) _sp.WriteByte(array, 0, array.Length);
                _overTimer.Start();
                _flagReceivePackageOver = false;
            }

        _sp.CloseSerial();
        return false;
    }

    public bool NCommunication()
    {
        var num = 0;
        var readFailedTimes = 0;
        while (!_wriImgTokenSource.Token.IsCancellationRequested)
        {
            if (!_flagOverTime)
            {
                switch (_nComStep)
                {
                    case NImgStep.StepHandShake:
                    {
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.Step_HandShake");
                        var array = Encoding.ASCII.GetBytes(_strHandshake);
                        _packageLength = LoadPackage(NCommandType.CmdHandshake, 0, array.Length, array);
                        _sp.WriteByte(_bufForData, 0, _packageLength);
                        OverTimer_Start();
                        _nComStep = NImgStep.StepReceive1;
                        CurrentProg.Enqueue(3);
                        break;
                    }
                    case NImgStep.StepOver:
                    {
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.Step_Over");
                        var array = Encoding.ASCII.GetBytes("Over");
                        _packageLength = LoadPackage(NCommandType.CmdOver, 0, array.Length, array);
                        _sp.WriteByte(_bufForData, 0, _packageLength);
                        CurrentProg.Enqueue(100);
                        Thread.Sleep(100);
                        _sp.CloseSerial();
                        return true;
                    }
                    case NImgStep.StepSetFontAddress:
                    case NImgStep.StepSetImageAddress:
                    case NImgStep.StepSetVoiceAddress:
                    {
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.3set");
                        var array = new byte[4];
                        array[3] = (byte)((uint)(_address >> 24) & 0xFFu);
                        array[2] = (byte)((uint)(_address >> 16) & 0xFFu);
                        array[1] = (byte)((uint)(_address >> 8) & 0xFFu);
                        array[0] = (byte)((uint)_address & 0xFFu);
                        _packageLength = LoadPackage(NCommandType.CmdSetaddress, 0, array.Length, array);
                        _sp.WriteByte(_bufForData, 0, _packageLength);
                        OverTimer_Start();
                        _nComStep = NImgStep.StepReceive1;
                        break;
                    }
                    case NImgStep.StepErase:
                    {
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.erase");
                        var num2 = 17668;
                        var array = new byte[6];
                        array[3] = (byte)((uint)(_address >> 24) & 0xFFu);
                        array[2] = (byte)((uint)(_address >> 16) & 0xFFu);
                        array[1] = (byte)((uint)(_address >> 8) & 0xFFu);
                        array[0] = (byte)((uint)_address & 0xFFu);
                        array[4] = (byte)((uint)(_blockOfErase >> 8) & 0xFFu);
                        array[5] = (byte)((uint)_blockOfErase & 0xFFu);
                        _packageLength = LoadPackage(NCommandType.CmdErase, num2, 6, array);
                        _sp.WriteByte(_bufForData, 0, _packageLength);
                        OverTimer_Start();
                        _nComStep = NImgStep.StepReceive1;
                        break;
                    }
                    case NImgStep.StepData:
                    {
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.data");
                        var array = new byte[1024];
                        Array.Copy(_bufferBmpData, num, array, 0, 1024);
                        if (num <= _byteOfData)
                        {
                            _packageLength = LoadPackage(NCommandType.CmdWrite, _packageId, 1024, array);
                            _sp.WriteByte(_bufForData, 0, _packageLength);
                            OverTimer_Start();
                            _nComStep = NImgStep.StepReceive1;
                            num += 1024;
                        }
                        else
                        {
                            _nComStep = NImgStep.StepOver;
                        }

                        break;
                    }
                    case NImgStep.StepReceive1:
                        // DebugWindow.GetInstance().updateDebugContent($"NImgStep.recv1:{readFailedTimes}");
                        if (_sp.BytesToReadFromCache >= 1)
                        {
                            readFailedTimes = 0;
                            // DebugWindow.GetInstance().updateDebugContent("Gonna read something out...");
                            _sp.ReadByte(_bufForData, 0, 1);
                            if (_bufForData[0] == 165)
                            {
                                _nComStep = NImgStep.StepReceive2;
                                _flagReceivePackageOver = false;
                                _byteOfPackage = _sp.BytesToReadFromCache;
                                _rxOverTimer.Start();
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
                    case NImgStep.StepReceive2:
                        // DebugWindow.GetInstance().updateDebugContent("NImgStep.recv2");
                        if (_flagReceivePackageOver)
                        {
                            _flagReceivePackageOver = false;
                            _sp.ReadByte(_bufForData, 1, _sp.BytesToReadFromCache);
                            var array = AnalysisPackage(_bufForData);
                            _overTimer.Stop();
                            if (array == null) break;
                            if (array[0] == 89 && array.Length == 1)
                                switch (_bufForData[1])
                                {
                                    case 2:
                                        _address = 65536;
                                        _blockOfErase = 1;
                                        CurrentProg.Enqueue(_progressValue);
                                        _packageId = 0;
                                        _cntPackages = (int)(_byteOfData / 1024);
                                        if (_byteOfData % 1024 != 0) _cntPackages++;
                                        _totalPackages = _cntPackages;
                                        _nComStep = NImgStep.StepErase;
                                        break;
                                    case 3:
                                        _nComStep = NImgStep.StepData;
                                        break;
                                    case 87:
                                        _cntPackages--;
                                        if (_cntPackages == 0)
                                        {
                                            _nComStep = NImgStep.StepOver;
                                            break;
                                        }

                                        _packageId++;
                                        _progressValue = _packageId * 100 / _totalPackages;
                                        CurrentProg.Enqueue(_progressValue);
                                        _nComStep = NImgStep.StepData;
                                        break;
                                    case 4:
                                        _nComStep = NImgStep.StepSetImageAddress;
                                        break;
                                }
                            else
                                _nComStep = NImgStep.StepOver;
                        }
                        else if (_byteOfPackage != _sp.BytesToReadFromCache)
                        {
                            _byteOfPackage = _sp.BytesToReadFromCache;
                            _rxOverTimer.Stop();
                            _rxOverTimer.Start();
                        }

                        break;
                }

                continue;
            }

            if (_countOverTime <= 0) break;
            _countOverTime--;
            _flagOverTime = false;
            if (_nComStep == NImgStep.StepReceive1 || _nComStep == NImgStep.StepReceive2)
                switch (_bufForData[1])
                {
                    case 2:
                        _nComStep = NImgStep.StepHandShake;
                        break;
                    case 3:
                        _nComStep = NImgStep.StepSetFontAddress;
                        break;
                    case 4:
                        _nComStep = NImgStep.StepErase;
                        break;
                    case 87:
                        _sp.WriteByte(_bufForData, 0, _packageLength);
                        _overTimer.Start();
                        _flagReceivePackageOver = false;
                        _nComStep = NImgStep.StepReceive1;
                        break;
                }
        }

        _sp.CloseSerial();
        return false;
    }

    private int LoadPackage(NCommandType cmd, int packageId, int len, byte[] dat)
    {
        _bufForData[0] = 165;
        _bufForData[1] = (byte)cmd;
        _bufForData[2] = (byte)((uint)(packageId >> 8) & 0xFFu);
        _bufForData[3] = (byte)((uint)packageId & 0xFFu);
        _bufForData[4] = (byte)((uint)(len >> 8) & 0xFFu);
        _bufForData[5] = (byte)((uint)len & 0xFFu);
        for (var i = 0; i < len; i++) _bufForData[6 + i] = dat[i];
        var num = CrcValidation(_bufForData, 1, 5 + len);
        _bufForData[6 + len] = (byte)((uint)(num >> 8) & 0xFFu);
        _bufForData[6 + len + 1] = (byte)((uint)num & 0xFFu);
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