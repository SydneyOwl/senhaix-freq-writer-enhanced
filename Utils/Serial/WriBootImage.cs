using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using SenhaixFreqWriter.Constants.Shx8x00;
using SkiaSharp;

namespace SenhaixFreqWriter.Utils.Serial;

public class WriBootImage
{
    private SKBitmap image;

	private Timer overTimer;

	private Timer rxOverTimer;

	private string STR_HANDSHAKE = "PROGRAM";

	private State comStep = State.HandShakeStep1;

	private byte[] bufForData = new byte[2048];

	private bool flagOverTime = false;

	private int countOverTime = 0;

	private int cntRetry = 0;

	private byte[] bufferBmpData = new byte[1048576];

	private uint byteOfData = 0u;

	private MySerialPort _sp = MySerialPort.GetInstance();

	public ConcurrentQueue<int> currentProg = new ConcurrentQueue<int>();

	public WriBootImage(SKBitmap img)
	{
		image = img;
		TimerInit();
		comStep = State.HandShakeStep1;
		_sp.OpenSerial();
	}
	

	private void TimerInit()
	{
		overTimer = new Timer();
		overTimer.Elapsed += OverTimer_Elapsed;
		overTimer.Interval = 200.0;
		overTimer.AutoReset = true;
		overTimer.Enabled = true;
		rxOverTimer = new System.Timers.Timer();
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
		{
			cntRetry--;
		}
		else
		{
			flagOverTime = true;
		}
	}

	private void OverTimer_Start()
	{
		overTimer.Start();
		countOverTime = 5;
		cntRetry = 3;
	}

	public async Task<bool> WriteImg()
	{
		comStep = State.HandShakeStep1;
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
		for (int k = 0; k < bufferBmpData.Length; k++)
		{
			bufferBmpData[k] = byte.MaxValue;
		}
		// if (bitmap2.PixelFormat == PixelFormat.Format24bppRgb)
		// {
		for (int l = 0; l < image.Height; l++)
		{
			for (int m = 0; m < image.Width; m++)
			{
				SKColor pixel = image.GetPixel(m, l);
				int num = pixel.Red >> 3;
				int num2 = pixel.Green >> 2;
				int num3 = pixel.Blue >> 3;
				int num4 = (num << 11) | (num2 << 5) | num3;
				bufferBmpData[byteOfData++] = (byte)num4;
				bufferBmpData[byteOfData++] = (byte)(num4 >> 8);
			}
		}
		try
		{
			if (await HandShake() && await Communication())
			{
				return true;
			}
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

	public async Task<bool> HandShake()
	{
		byte[] array = new byte[1];
		while (true)
		{
			if (!flagOverTime)
			{
				switch (comStep)
				{
				case State.HandShakeStep1:
					array = Encoding.ASCII.GetBytes("PROGROMSHXU");
					await _sp.WriteByte(array, 0, array.Length);
					OverTimer_Start();
					comStep = State.HandShakeStep2;
					break;
				case State.HandShakeStep2:
					if (_sp.BytesToRead >= 1)
					{
						await _sp.ReadByte(bufForData, 0, 1);
						if (bufForData[0] == 6)
						{
							array = new byte[1] { 70 };
							await _sp.WriteByte(array, 0, 1);
							comStep = State.HandShakeStep3;
						}
					}
					break;
				case State.HandShakeStep3:
					if (_sp.BytesToRead >= 8)
					{
						await _sp.ReadByte(bufForData, 0, 8);
						comStep = State.WriteStep1;
						return true;
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
				countOverTime--;
				cntRetry = 3;
				flagOverTime = false;
				if (comStep == State.HandShakeStep2)
				{
					array = Encoding.ASCII.GetBytes("PROGROMSHXU");
					await _sp.WriteByte(array, 0, array.Length);
				}
				else
				{
					await _sp.WriteByte(array, 0, array.Length);
				}
			}
		}
		return false;
	}

	public async Task<bool> Communication()
	{
		byte[] array = new byte[68]
		{
			73, 0, 0, 64, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255
		};
		bool flag = true;
		int num = 0;
		int num2 = 0;
		while (true)
		{
			if (!flagOverTime)
			{
				switch (comStep)
				{
				case State.WriteStep1:
				{
					byte[] array2 = new byte[1024];
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
						for (int i = 0; i < 64; i++)
						{
							array[i + 4] = array2[i];
						}
						array[1] = (byte)(num2 >> 8);
						array[2] = (byte)num2;
						await _sp.WriteByte(array, 0, 68);
						currentProg.Enqueue((int)(num * 100 / byteOfData));
						comStep = State.WriteStep2;
						break;
					}
					currentProg.Enqueue(100);
					return true;
				}
				case State.WriteStep2:
					if (_sp.BytesToRead >= 1)
					{
						await _sp.ReadByte(bufForData, 0, 1);
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
				if (countOverTime <= 0)
				{
					break;
				}
				countOverTime--;
				cntRetry = 3;
				flagOverTime = false;
				await _sp.WriteByte(array, 0, 68);
			}
		}
		return false;
	}
}