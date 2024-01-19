using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using SQ5R.View;
using WF_FRAM_KDH;
using Timer = System.Timers.Timer;

namespace BF_H802_Import_Picture_tools;

public class ImportBmpHelper
{
    private const int ADDR_FONT = 65536;

    private const int ADDR_IMAGE = 786432;

    private const int ADDR_IMAGE_UV21 = 1048576;

    private const int ADDR_VOICE = 851968;

    private static readonly uint MODELTYPE_JJCC8629 = 0u;

    private readonly byte[] bufferBmpData = new byte[1048576];

    private readonly byte[] bufForData = new byte[2048];
    private readonly Image image;

    private readonly uint MODELTYPE = MODELTYPE_JJCC8629;

    private readonly Stream s_out = null;

    private uint byteOfData;

    private int cntRetry;

    private IMPORT_STEP comStep = IMPORT_STEP.HandShakeStep1;

    private int countOverTime;

    private bool flagOverTime;

    private byte[] hSTable1 = new byte[7] { 80, 82, 79, 71, 82, 79, 77 };

    private byte[][] hStable2_ModelType = new byte[1][] { new byte[3] { 83, 72, 88 } };

    private int indexModelType;

    private Timer overTimer;

    private MySerialPort port;

    private Timer rxOverTimer;

    private string STR_HANDSHAKE = "PROGRAM";

    public ImportBmpHelper(Image img, int indexModelType)
    {
        image = img;
        this.indexModelType = indexModelType;
    }

    public ComInfoIssue ProgressUpdate { get; set; } = new();

    public int LinkDevice(string portName)
    {
        port = new MySerialPort();
#if NET461
        if (BleCore.BleInstance().CurrentDevice == null)
        {
#endif
            port.PortName = portName;
            port.BaudRate = 9600;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Parity = Parity.None;
            port.ReadBufferSize = 10240;
            port.WriteBufferSize = 10240;
            port.DtrEnable = true;
            port.RtsEnable = true;
#if NET461
        }
#endif
        try
        {
            port.OpenSerial();
            TimerInit();
            comStep = IMPORT_STEP.HandShakeStep1;
            return 1;
        }
        catch
        {
            return -1;
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

    private void RxOverTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
    }

    private void OverTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        if (cntRetry > 0)
            cntRetry--;
        else
            flagOverTime = true;
    }

    private void OverTimer_Start()
    {
        overTimer.Start();
        countOverTime = 5;
        cntRetry = 3;
    }

    public bool Doit()
    {
        comStep = IMPORT_STEP.HandShakeStep1;
        var bitmap = new Bitmap(image);
        var bitmap2 = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
        for (var i = 0; i < image.Width; i++)
        for (var j = 0; j < image.Height; j++)
        {
            var pixel = bitmap.GetPixel(i, j);
            bitmap2.SetPixel(i, j, pixel);
        }

        var width = bitmap2.Width;
        var height = bitmap2.Height;
        byteOfData = 0u;
        for (var k = 0; k < bufferBmpData.Length; k++) bufferBmpData[k] = byte.MaxValue;

        if (bitmap2.PixelFormat == PixelFormat.Format24bppRgb)
        {
            for (var l = 0; l < height; l++)
            for (var m = 0; m < width; m++)
            {
                var pixel = bitmap2.GetPixel(m, l);
                var num = pixel.R >> 3;
                var num2 = pixel.G >> 2;
                var num3 = pixel.B >> 3;
                var num4 = (num << 11) | (num2 << 5) | num3;
                bufferBmpData[byteOfData++] = (byte)num4;
                bufferBmpData[byteOfData++] = (byte)(num4 >> 8);
            }

            if (HandShake() && Communication()) return true;

            return false;
        }

        MessageBox.Show("格式错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        return false;
    }

    public bool HandShake()
    {
        var array = new byte[1];
        while (true)
            if (!flagOverTime)
            {
                switch (comStep)
                {
                    case IMPORT_STEP.HandShakeStep1:
                        array = Encoding.ASCII.GetBytes("PROGROMSHXU");
                        port.WriteByte(array, 0, array.Length);
                        OverTimer_Start();
                        comStep = IMPORT_STEP.HandShakeStep2;
                        break;
                    case IMPORT_STEP.HandShakeStep2:
                        if (port.BytesToReadFromCache >= 1)
                        {
                            port.ReadByte(bufForData, 0, 1);
                            if (bufForData[0] == 6)
                            {
                                array = new byte[1] { 70 };
                                port.WriteByte(array, 0, 1);
                                comStep = IMPORT_STEP.HandShakeStep3;
                            }
                        }

                        break;
                    case IMPORT_STEP.HandShakeStep3:
                        if (port.BytesToReadFromCache >= 8)
                        {
                            port.ReadByte(bufForData, 0, 8);
                            comStep = IMPORT_STEP.WriteStep1;
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
                if (comStep == IMPORT_STEP.HandShakeStep2)
                {
                    array = Encoding.ASCII.GetBytes("PROGROMSHXU");
                    port.WriteByte(array, 0, array.Length);
                }
                else
                {
                    port.WriteByte(array, 0, array.Length);
                }
            }

        port.CloseSerial();
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
        while (true)
            if (!flagOverTime)
            {
                switch (comStep)
                {
                    case IMPORT_STEP.WriteStep1:
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
                            port.WriteByte(array, 0, 68);
                            ProgressUpdate.IssueProgressValue((int)(num * 100 / byteOfData));
                            comStep = IMPORT_STEP.WriteStep2;
                            break;
                        }

                        if (s_out != null) s_out.Close();

                        port.CloseSerial();
                        ProgressUpdate.IssueProgressValue(100);
                        return true;
                    }
                    case IMPORT_STEP.WriteStep2:
                        if (port.BytesToReadFromCache >= 1)
                        {
                            port.ReadByte(bufForData, 0, 1);
                            if (bufForData[0] == 6)
                            {
                                num2 += 64;
                                rxOverTimer.Stop();
                                OverTimer_Start();
                                comStep = IMPORT_STEP.WriteStep1;
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
                port.WriteByte(array, 0, 68);
            }

        if (s_out != null) s_out.Close();

        port.CloseSerial();
        return false;
    }
}