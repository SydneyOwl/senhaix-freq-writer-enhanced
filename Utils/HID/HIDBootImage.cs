using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using SenhaixFreqWriter.Constants.Gt12;
using SkiaSharp;
using Timer = System.Timers.Timer;

namespace SenhaixFreqWriter.Utils.HID;

public class HidBootImage
{
    // private ComInfoIssue progressUpdate = new ComInfoIssue();

    private readonly byte[] _bufferBmpData = new byte[1048576];

    private readonly HidTools _hid = HidTools.GetInstance();
    private readonly SKBitmap _img;
    private int _address;

    private int _blockOfErase;

    private byte[] _bufForData = new byte[2048];

    private uint _byteOfData;

    private int _cntPackages;

    private HidBootimageStatus _comStep = HidBootimageStatus.StepHandShake;

    private int _countOverTime;

    private string _curFilePath = "";

    private bool _flagOverTime;

    private DataHelper _helper;

    private Timer _overTimer;

    private int _packageId;

    private int _packageLength;

    private string _progressText = "";

    private string _progressTextHead = "";

    private int _progressValue;

    private Stream _sOut = null;

    private int _totalPackages;

    private CancellationTokenSource _wriImgTokenSource;

    public ConcurrentQueue<int> CurrentProg = new();


    // public ComInfoIssue ProgressUpdate
    // {
    // 	get
    // 	{
    // 		return progressUpdate;
    // 	}
    // 	set
    // 	{
    // 		progressUpdate = value;
    // 	}
    // }

    public HidBootImage(SKBitmap img)
    {
        _img = img;
    }

    private void TimerInit()
    {
        _overTimer = new Timer();
        _overTimer.Elapsed += OverTimer_Elapsed;
        _overTimer.Interval = 5000.0;
        _overTimer.AutoReset = false;
        _overTimer.Enabled = true;
    }

    private void OverTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _flagOverTime = true;
    }

    private void OverTimer_Start()
    {
        _overTimer.Stop();
        _overTimer.Start();
        _flagOverTime = false;
        _countOverTime = 5;
    }

    public bool WriteImg()
    {
        _wriImgTokenSource = new CancellationTokenSource();
        TimerInit();
        _comStep = HidBootimageStatus.StepHandShakeJump1;
        _helper = new DataHelper();
        if (HandShake() && Communication()) return true;
        return false;
    }

    public void CancelWriteImg()
    {
        _wriImgTokenSource.Cancel();
    }

    public bool HandShake()
    {
        var byData = new byte[10];
        while (!_wriImgTokenSource.Token.IsCancellationRequested)
            if (!_flagOverTime)
            {
                switch (_comStep)
                {
                    case HidBootimageStatus.StepHandShakeJump1:
                        byData = Encoding.ASCII.GetBytes("PROGRAMGT12");
                        byData = _helper.LoadPackage(1, 0, byData, (byte)byData.Length);
                        _hid.Send(byData);
                        _comStep = HidBootimageStatus.StepHandShakeJump2;
                        OverTimer_Start();
                        break;
                    case HidBootimageStatus.StepHandShakeJump2:
                        if (_hid.FlagReceiveData)
                        {
                            _hid.FlagReceiveData = false;
                            _helper.AnalyzePackage(_hid.RxBuffer);
                            if (_helper.ErrorCode == HidErrors.ErNone)
                            {
                                byData = _helper.LoadPackage(2, 0, null, 1);
                                _hid.Send(byData);
                                OverTimer_Start();
                                _comStep = HidBootimageStatus.StepHandShakeJump3;
                                // Console.Write("Handshake2!");
                            }
                        }

                        break;
                    case HidBootimageStatus.StepHandShakeJump3:
                        if (_hid.FlagReceiveData)
                        {
                            _hid.FlagReceiveData = false;
                            _helper.AnalyzePackage(_hid.RxBuffer);
                            if (_helper.ErrorCode == HidErrors.ErNone)
                            {
                                byData = _helper.LoadPackage(16, 0, null, 1);
                                _hid.Send(byData);
                                OverTimer_Start();
                                _comStep = HidBootimageStatus.StepHandShakeJump4;
                                // Console.Write("Handshake3!");
                            }
                        }

                        break;
                    case HidBootimageStatus.StepHandShakeJump4:
                        if (_hid.FlagReceiveData)
                        {
                            _hid.FlagReceiveData = false;
                            _helper.AnalyzePackage(_hid.RxBuffer);
                            if (_helper.ErrorCode == HidErrors.ErNone)
                            {
                                _overTimer.Stop();
                                _comStep = HidBootimageStatus.StepHandShake;
                                // Console.Write("Handshake4!");
                                return true;
                            }
                        }

                        break;
                }
            }
            else
            {
                if (_countOverTime <= 0) break;
                _overTimer.Stop();
                _overTimer.Start();
                _countOverTime--;
                _flagOverTime = false;
                _hid.Send(byData);
            }

        _overTimer.Stop();
        return false;
    }

    public bool Communication()
    {
        var num = 0;
        var array = new byte[10];
        var array2 = new byte[32];
        var num2 = 0;
        // Bitmap bitmap = new Bitmap(img);
        // Bitmap bitmap2 = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
        // for (int i = 0; i < img.Width; i++)
        // {
        // 	for (int j = 0; j < img.Height; j++)
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
        for (var l = 0; l < _img.Height; l++)
        for (var m = 0; m < _img.Width; m++)
        {
            var pixel = _img.GetPixel(m, l);
            var num3 = pixel.Red >> 3;
            var num4 = pixel.Green >> 2;
            var num5 = pixel.Blue >> 3;
            var num6 = (num3 << 11) | (num4 << 5) | num5;
            _bufferBmpData[_byteOfData++] = (byte)num6;
            _bufferBmpData[_byteOfData++] = (byte)(num6 >> 8);
        }

        while (!_wriImgTokenSource.Token.IsCancellationRequested)
            if (!_flagOverTime)
            {
                switch (_comStep)
                {
                    case HidBootimageStatus.StepHandShake:
                        // Console.Write("Step_HandShake");
                        _address = 851968;
                        _blockOfErase = 3;
                        _progressValue = 0;
                        CurrentProg.Enqueue(_progressValue);
                        // progressUpdate.IssueProgressValue(progressValue);
                        _packageId = 0;
                        _cntPackages = (int)(_byteOfData / 1024);
                        if (_byteOfData % 1024 != 0) _cntPackages++;
                        _totalPackages = _cntPackages;
                        _comStep = HidBootimageStatus.StepErase;
                        break;
                    case HidBootimageStatus.StepSetAddress:
                    {
                        // Console.Write("Step_SetAddress");
                        _bufForData = new byte[1080];
                        _packageLength = 0;
                        for (var n = 0; n < 1024; n++)
                        {
                            if (num + _packageLength >= _byteOfData) break;
                            _bufForData[n] = _bufferBmpData[num + n];
                            _packageLength++;
                        }

                        num += _packageLength;
                        if (_packageLength > 0)
                        {
                            num2 = DataHelper.CrcValidation(_bufForData, 0, 1024);
                            array2 = new byte[6];
                            array2[3] = (byte)((uint)(_address >> 24) & 0xFFu);
                            array2[2] = (byte)((uint)(_address >> 16) & 0xFFu);
                            array2[1] = (byte)((uint)(_address >> 8) & 0xFFu);
                            array2[0] = (byte)((uint)_address & 0xFFu);
                            array2[4] = 4;
                            array2[5] = 0;
                            array = _helper.LoadPackage(18, 0, array2, 6);
                            _hid.Send(array);
                            OverTimer_Start();
                            _comStep = HidBootimageStatus.StepReceive1;
                            break;
                        }

                        array = _helper.LoadPackage(69, 0, null, 0);
                        _hid.Send(array);
                        _overTimer.Stop();
                        return true;
                    }
                    case HidBootimageStatus.StepErase:
                    {
                        // Console.Write("Step_Erase");
                        ushort args = 17668;
                        array2 = new byte[6];
                        array2[3] = (byte)((uint)(_address >> 24) & 0xFFu);
                        array2[2] = (byte)((uint)(_address >> 16) & 0xFFu);
                        array2[1] = (byte)((uint)(_address >> 8) & 0xFFu);
                        array2[0] = (byte)((uint)_address & 0xFFu);
                        array2[4] = (byte)((uint)(_blockOfErase >> 8) & 0xFFu);
                        array2[5] = (byte)((uint)_blockOfErase & 0xFFu);
                        array = _helper.LoadPackage(19, args, array2, 6);
                        _hid.Send(array);
                        OverTimer_Start();
                        _comStep = HidBootimageStatus.StepReceive1;
                        break;
                    }
                    case HidBootimageStatus.StepData1:
                    {
                        // Console.Write("Step_Data1");
                        // Console.Write(BitConverter.ToString(bufForData));
                        array = new byte[59];
                        for (var num7 = 0; num7 < 18; num7++)
                        {
                            for (var num8 = 0; num8 < 59; num8++) array[num8] = _bufForData[num7 * 59 + num8];
                            array = _helper.LoadImgDataPackage(5, (ushort)num7, array, 59);
                            _hid.Send(array);
                        }

                        OverTimer_Start();
                        _comStep = HidBootimageStatus.StepData2;
                        break;
                    }
                    case HidBootimageStatus.StepData2:
                        // Console.Write("Step_Data2");
                        array = new byte[2]
                        {
                            (byte)((uint)num2 & 0xFFu),
                            (byte)((uint)(num2 >> 8) & 0xFFu)
                        };
                        array = _helper.LoadPackage(6, 0, array, 2);
                        _hid.Send(array);
                        OverTimer_Start();
                        _comStep = HidBootimageStatus.StepReceive1;
                        break;
                    case HidBootimageStatus.StepReceive1:
                        // Console.Write("Step_Receive_1");
                        if (!_hid.FlagReceiveData) break;
                        _hid.FlagReceiveData = false;
                        _helper.AnalyzePackage(_hid.RxBuffer);
                        if (_helper.ErrorCode == HidErrors.ErNone)
                        {
                            switch (_helper.Command)
                            {
                                case 19:
                                    _comStep = HidBootimageStatus.StepSetAddress;
                                    break;
                                case 18:
                                    _comStep = HidBootimageStatus.StepData1;
                                    break;
                                case 6:
                                    _address += 1024;
                                    _packageId++;
                                    _progressValue = _packageId * 100 / _totalPackages;
                                    CurrentProg.Enqueue(_progressValue);
                                    // progressUpdate.IssueProgressValue(progressValue);
                                    _comStep = HidBootimageStatus.StepSetAddress;
                                    break;
                            }

                            OverTimer_Start();
                        }

                        break;
                }
            }
            else
            {
                if (_countOverTime <= 0) break;
                _overTimer.Stop();
                _overTimer.Start();
                _countOverTime--;
                _flagOverTime = false;
                _hid.Send(array);
            }

        _overTimer.Stop();
        return false;
    }
    // MessageBox.Show("格式错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
    // return false;
    // }

    public void DataReceived(object sender, byte[] e)
    {
        _hid.RxBuffer = e;
        _hid.FlagReceiveData = true;
    }
}