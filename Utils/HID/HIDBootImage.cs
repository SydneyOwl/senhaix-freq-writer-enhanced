using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Timers;
using SenhaixFreqWriter.Constants.Gt12;
using SenhaixFreqWriter.DataModels.Gt12;
using SkiaSharp;

namespace SenhaixFreqWriter.Utils.HID;

public class HIDBootImage
{
	private SKBitmap img = null;

	private Timer overTimer;

	private HID_BOOTIMAGE_STATUS comStep = HID_BOOTIMAGE_STATUS.Step_HandShake;

	private byte[] bufForData = new byte[2048];

	private int packageID = 0;

	private Stream s_out = null;

	private int address = 0;

	private int blockOfErase = 0;

	private int totalPackages = 0;

	private int cntPackages = 0;

	private int packageLength = 0;

	private string curFilePath = "";

	private bool flagOverTime = false;

	private int countOverTime = 0;

	private int progressValue = 0;

	private string progressText = "";

	private string progressTextHead = "";

	// private ComInfoIssue progressUpdate = new ComInfoIssue();

	private byte[] bufferBmpData = new byte[1048576];

	private uint byteOfData = 0u;

	private HidTools hid = HidTools.GetInstance();

	private DataHelper helper;
	
	public ConcurrentQueue<int> currentProg = new ConcurrentQueue<int>();


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

	public HIDBootImage(SKBitmap img)
	{
		this.img = img;
	}

	private void TimerInit()
	{
		overTimer = new Timer();
		overTimer.Elapsed += OverTimer_Elapsed;
		overTimer.Interval = 5000.0;
		overTimer.AutoReset = false;
		overTimer.Enabled = true;
	}

	private void OverTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		flagOverTime = true;
	}

	private void OverTimer_Start()
	{
		overTimer.Stop();
		overTimer.Start();
		flagOverTime = false;
		countOverTime = 5;
	}

	public bool WriteImg()
	{
		TimerInit();
		comStep = HID_BOOTIMAGE_STATUS.Step_HandShake_Jump1;
		helper = new DataHelper();
		if (HandShake() && Communication())
		{
			return true;
		}
		return false;
	}

	public bool HandShake()
	{
		byte[] byData = new byte[10];
		while (true)
		{
			if (!flagOverTime)
			{
				switch (comStep)
				{
				case HID_BOOTIMAGE_STATUS.Step_HandShake_Jump1:
					byData = Encoding.ASCII.GetBytes("PROGRAMGT12");
					byData = helper.LoadPackage(1, 0, byData, (byte)byData.Length);
					hid.Send(byData);
					comStep = HID_BOOTIMAGE_STATUS.Step_HandShake_Jump2;
					OverTimer_Start();
					break;
				case HID_BOOTIMAGE_STATUS.Step_HandShake_Jump2:
					if (hid.FlagReceiveData)
					{
						hid.FlagReceiveData = false;
						helper.AnalyzePackage(hid.RxBuffer);
						if (helper.ErrorCode == HidErrors.ErNone)
						{
							byData = helper.LoadPackage(2, 0, null, 1);
							hid.Send(byData);
							OverTimer_Start();
							comStep = HID_BOOTIMAGE_STATUS.Step_HandShake_Jump3;
							// Console.Write("Handshake2!");
						}
					}
					break;
				case HID_BOOTIMAGE_STATUS.Step_HandShake_Jump3:
					if (hid.FlagReceiveData)
					{
						hid.FlagReceiveData = false;
						helper.AnalyzePackage(hid.RxBuffer);
						if (helper.ErrorCode == HidErrors.ErNone)
						{
							byData = helper.LoadPackage(16, 0, null, 1);
							hid.Send(byData);
							OverTimer_Start();
							comStep = HID_BOOTIMAGE_STATUS.Step_HandShake_Jump4;
							// Console.Write("Handshake3!");
						}
					}
					break;
				case HID_BOOTIMAGE_STATUS.Step_HandShake_Jump4:
					if (hid.FlagReceiveData)
					{
						hid.FlagReceiveData = false;
						helper.AnalyzePackage(hid.RxBuffer);
						if (helper.ErrorCode == HidErrors.ErNone)
						{
							overTimer.Stop();
							comStep = HID_BOOTIMAGE_STATUS.Step_HandShake;
							// Console.Write("Handshake4!");
							return true;
						}
					}
					break;
				}
			}
			else
			{
				if (countOverTime <= 0)
				{
					break;
				}
				overTimer.Stop();
				overTimer.Start();
				countOverTime--;
				flagOverTime = false;
				hid.Send(byData);
			}
		}
		overTimer.Stop();
		return false;
	}

	public bool Communication()
	{
		int num = 0;
		byte[] array = new byte[10];
		byte[] array2 = new byte[32];
		int num2 = 0;
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
		byteOfData = 0u;
		for (int k = 0; k < bufferBmpData.Length; k++)
		{
			bufferBmpData[k] = byte.MaxValue;
		}
		// if (bitmap2.PixelFormat == PixelFormat.Format24bppRgb)
		// {
			for (int l = 0; l < img.Height; l++)
			{
				for (int m = 0; m < img.Width; m++)
				{
					SKColor pixel = img.GetPixel(m, l);
					int num3 = pixel.Red >> 3;
					int num4 = pixel.Green >> 2;
					int num5 = pixel.Blue >> 3;
					int num6 = (num3 << 11) | (num4 << 5) | num5;
					bufferBmpData[byteOfData++] = (byte)num6;
					bufferBmpData[byteOfData++] = (byte)(num6 >> 8);
				}
			}
			while (true)
			{
				if (!flagOverTime)
				{
					switch (comStep)
					{
					case HID_BOOTIMAGE_STATUS.Step_HandShake:
						// Console.Write("Step_HandShake");
						address = 851968;
						blockOfErase = 3;
						progressValue = 0;
						currentProg.Enqueue(progressValue);
						// progressUpdate.IssueProgressValue(progressValue);
						packageID = 0;
						cntPackages = (int)(byteOfData / 1024);
						if (byteOfData % 1024 != 0)
						{
							cntPackages++;
						}
						totalPackages = cntPackages;
						comStep = HID_BOOTIMAGE_STATUS.Step_Erase;
						break;
					case HID_BOOTIMAGE_STATUS.Step_SetAddress:
					{
						// Console.Write("Step_SetAddress");
						bufForData = new byte[1080];
						packageLength = 0;
						for (int n = 0; n < 1024; n++)
						{
							if (num + packageLength >= byteOfData)
							{
								break;
							}
							bufForData[n] = bufferBmpData[num + n];
							packageLength++;
						}
						num += packageLength;
						if (packageLength > 0)
						{
							num2 = DataHelper.CrcValidation(bufForData, 0, 1024);
							array2 = new byte[6];
							array2[3] = (byte)((uint)(address >> 24) & 0xFFu);
							array2[2] = (byte)((uint)(address >> 16) & 0xFFu);
							array2[1] = (byte)((uint)(address >> 8) & 0xFFu);
							array2[0] = (byte)((uint)address & 0xFFu);
							array2[4] = 4;
							array2[5] = 0;
							array = helper.LoadPackage(18, 0, array2, 6);
							hid.Send(array);
							OverTimer_Start();
							comStep = HID_BOOTIMAGE_STATUS.Step_Receive_1;
							break;
						}
						array = helper.LoadPackage(69, 0, null, 0);
						hid.Send(array);
						overTimer.Stop();
						return true;
					}
					case HID_BOOTIMAGE_STATUS.Step_Erase:
					{
						// Console.Write("Step_Erase");
						ushort args = 17668;
						array2 = new byte[6];
						array2[3] = (byte)((uint)(address >> 24) & 0xFFu);
						array2[2] = (byte)((uint)(address >> 16) & 0xFFu);
						array2[1] = (byte)((uint)(address >> 8) & 0xFFu);
						array2[0] = (byte)((uint)address & 0xFFu);
						array2[4] = (byte)((uint)(blockOfErase >> 8) & 0xFFu);
						array2[5] = (byte)((uint)blockOfErase & 0xFFu);
						array = helper.LoadPackage(19, args, array2, 6);
						hid.Send(array);
						OverTimer_Start();
						comStep = HID_BOOTIMAGE_STATUS.Step_Receive_1;
						break;
					}
					case HID_BOOTIMAGE_STATUS.Step_Data1:
					{
						// Console.Write("Step_Data1");
						// Console.Write(BitConverter.ToString(bufForData));
						array = new byte[59];
						for (int num7 = 0; num7 < 18; num7++)
						{
							for (int num8 = 0; num8 < 59; num8++)
							{
								array[num8] = bufForData[num7 * 59 + num8];
							}
							array = helper.LoadImgDataPackage(5, (ushort)num7, array, 59);
							hid.Send(array);
						}
						OverTimer_Start();
						comStep = HID_BOOTIMAGE_STATUS.Step_Data2;
						break;
					}
					case HID_BOOTIMAGE_STATUS.Step_Data2:
						// Console.Write("Step_Data2");
						array = new byte[2]
						{
							(byte)((uint)num2 & 0xFFu),
							(byte)((uint)(num2 >> 8) & 0xFFu)
						};
						array = helper.LoadPackage(6, 0, array, 2);
						hid.Send(array);
						OverTimer_Start();
						comStep = HID_BOOTIMAGE_STATUS.Step_Receive_1;
						break;
					case HID_BOOTIMAGE_STATUS.Step_Receive_1:
						// Console.Write("Step_Receive_1");
						if (!hid.FlagReceiveData)
						{
							break;
						}
						hid.FlagReceiveData = false;
						helper.AnalyzePackage(hid.RxBuffer);
						if (helper.ErrorCode == HidErrors.ErNone)
						{
							switch (helper.Command)
							{
							case 19:
								comStep = HID_BOOTIMAGE_STATUS.Step_SetAddress;
								break;
							case 18:
								comStep = HID_BOOTIMAGE_STATUS.Step_Data1;
								break;
							case 6:
								address += 1024;
								packageID++;
								progressValue = packageID * 100 / totalPackages;
								currentProg.Enqueue(progressValue);
								// progressUpdate.IssueProgressValue(progressValue);
								comStep = HID_BOOTIMAGE_STATUS.Step_SetAddress;
								break;
							}
							OverTimer_Start();
						}
						break;
					}
				}
				else
				{
					if (countOverTime <= 0)
					{
						break;
					}
					overTimer.Stop();
					overTimer.Start();
					countOverTime--;
					flagOverTime = false;
					hid.Send(array);
				}
			}
			overTimer.Stop();
			return false;
		}
		// MessageBox.Show("格式错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		// return false;
	// }

	public void DataReceived(object sender, byte[] e)
	{
		hid.RxBuffer = e;
		hid.FlagReceiveData = true;
	}
}
