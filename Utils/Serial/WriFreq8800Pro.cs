using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Timers;
using SenhaixFreqWriter.Constants.Shx8800Pro;
using SenhaixFreqWriter.DataModels.Shx8800Pro;
using SenhaixFreqWriter.Views.Common;
using Timer = System.Timers.Timer;

namespace SenhaixFreqWriter.Utils.Serial;

public class WriFreq8800Pro
{
	private Timer timer;

	private OpType opType;

	private MySerialPort port;

	public AppData appData;

	private bool flagTransmitting = false;

	private bool flagRetry = false;

	private byte timesOfRetry = 5;

	private Step step;

	private bool flagReceiveData = false;

	private byte[] rxBuffer = new byte[128];

	private DataHelper helper;

	private int progressVal = 0;

	private string progressCont = "";

	private string[] tblCTSDCS = new string[210]
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


	public ConcurrentQueue<ProgressBarValue> statusQueue = new();

	public WriFreq8800Pro(MySerialPort port, OpType opType)
	{
		this.port = port;
		this.opType = opType;
		this.appData = AppData.GetInstance();
		helper = new DataHelper();
		statusQueue.Clear();
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

	private void Timer_Elapsed(object sender, ElapsedEventArgs e)
	{
		flagRetry = true;
	}

	public void DataReceived(object sender, byte[] e)
	{
		rxBuffer = e;
		flagReceiveData = true;
	}

	private void resetRetryCount()
	{
		timesOfRetry = 5;
		flagRetry = false;
	}

	public bool DoIt(CancellationToken cancellationToken)
	{
		flagTransmitting = true;
		resetRetryCount();
		step = Step.StepHandshake1;
		if (HandShake(cancellationToken))
		{
			if (opType == OpType.Write)
			{
				if (Write(cancellationToken))
				{
					return true;
				}
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
		byte[] array = new byte[1];
		while (flagTransmitting && !cancellationToken.IsCancellationRequested)
		{
			if (!flagRetry)
			{
				switch (step)
				{
					case Step.StepHandshake1:
						DebugWindow.GetInstance().updateDebugContent("Now: Step.StepHandshake1");
						array = Encoding.ASCII.GetBytes("PROGRAMSHXPU");
						port.WriteByte(array, 0, array.Length);
						progressVal = 0;
						progressCont = "握手...";
						statusQueue.Enqueue(new ProgressBarValue(progressVal,progressCont));
						timer.Start();
						resetRetryCount();
						step = Step.StepHandshake2;
						break;
					case Step.StepHandshake2:
						DebugWindow.GetInstance().updateDebugContent("Now: Step.StepHandshake2");
						if (port.BytesToReadFromCache >= 1)
						{
							port.ReadByte(rxBuffer, 0, 1);
							if (rxBuffer[0] == 6)
							{
								array = new byte[1] { 70 };
								port.WriteByte(array, 0, array.Length);
								resetRetryCount();
								step = Step.StepHandshake3;
							}
						}

						break;
					case Step.StepHandshake3:
						DebugWindow.GetInstance().updateDebugContent("Now: Step.StepHandshake3");
						if (port.BytesToReadFromCache >= 16)
						{
							port.ReadByte(rxBuffer, 0, 16);
							timer.Stop();
							resetRetryCount();
							progressVal = 0;
							progressCont = "进度..." + progressVal + "%";
							statusQueue.Enqueue(new ProgressBarValue(progressVal,progressCont));
							if (opType == OpType.Read)
							{
								step = Step.StepRead1;
								return true;
							}

							step = Step.StepWrite1;
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
				port.WriteByte(array, 0, array.Length);
			}
		}

		return false;
	}

	private bool Write(CancellationToken cancellationToken)
	{
		byte[] array = new byte[1];
		byte[] array2 = new byte[64];
		ushort num = 0;
		int num2 = 0;
		while (flagTransmitting && !cancellationToken.IsCancellationRequested)
		{
			if (!flagRetry)
			{
				switch (step)
				{
					case Step.StepWrite1:
						DebugWindow.GetInstance().updateDebugContent("Now: Step.StepWrite1");
						if (num < 16384)
						{
							byte[] channelInfos = GetChannelInfos(num2++);
							byte[] channelInfos2 = GetChannelInfos(num2++);
							Array.Copy(channelInfos, 0, array2, 0, 32);
							Array.Copy(channelInfos2, 0, array2, 32, 32);
						}
						else if (num == 32768)
						{
							byte[] vFOAInfos = GetVFOAInfos();
							byte[] vFOBInfos = GetVFOBInfos();
							for (int i = 0; i < 32; i++)
							{
								array2[i] = vFOAInfos[i];
								array2[i + 32] = vFOBInfos[i];
							}
						}
						else if (num == 36864)
						{
							array2[0] = (byte)appData.FunCfgs.Sql;
							array2[1] = (byte)appData.FunCfgs.SaveMode;
							array2[2] = (byte)appData.FunCfgs.Vox;
							array2[3] = (byte)appData.FunCfgs.Backlight;
							array2[4] = (byte)appData.FunCfgs.DualStandby;
							array2[5] = (byte)appData.FunCfgs.Tot;
							array2[6] = (byte)appData.FunCfgs.Beep;
							array2[7] = (byte)appData.FunCfgs.VoiceSw;
							array2[8] = 0;
							array2[9] = (byte)appData.FunCfgs.SideTone;
							array2[10] = (byte)appData.FunCfgs.ScanMode;
							array2[11] = (byte)appData.Vfos.Pttid;
							array2[12] = (byte)appData.FunCfgs.PttDly;
							array2[13] = (byte)appData.FunCfgs.ChADisType;
							array2[14] = (byte)appData.FunCfgs.ChBDisType;
							array2[16] = (byte)appData.FunCfgs.AutoLock;
							array2[17] = (byte)appData.FunCfgs.AlarmMode;
							array2[18] = (byte)appData.FunCfgs.LocalSosTone;
							array2[19] = 0;
							array2[20] = (byte)appData.FunCfgs.TailClear;
							array2[21] = (byte)appData.FunCfgs.RptTailClear;
							array2[22] = (byte)appData.FunCfgs.RptTailDet;
							array2[23] = (byte)appData.FunCfgs.Roger;
							array2[24] = 0;
							array2[25] = (byte)appData.FunCfgs.FmEnable;
							array2[26] = 0;
							array2[26] |= (byte)appData.FunCfgs.ChAWorkmode;
							array2[26] |= (byte)(appData.FunCfgs.ChBWorkmode << 4);
							array2[27] = (byte)appData.FunCfgs.KeyLock;
							array2[28] = (byte)appData.FunCfgs.PowerOnDisType;
							array2[29] = 0;
							array2[30] = (byte)appData.FunCfgs.Tone;
							array2[32] = (byte)appData.FunCfgs.VoxDlyTime;
							array2[33] = (byte)appData.FunCfgs.MenuQuitTime;
							array2[34] = (byte)appData.FunCfgs.MicGain;
							array2[36] = (byte)appData.FunCfgs.PwrOnDlyTime;
							array2[37] = (byte)appData.FunCfgs.VoxSw;
							array2[42] = (byte)appData.FunCfgs.Key2Short;
							array2[43] = (byte)appData.FunCfgs.Key2Long;
							array2[46] = (byte)appData.FunCfgs.CurBankA;
							array2[47] = (byte)appData.FunCfgs.CurBankB;
							array2[49] = (byte)appData.FunCfgs.BtMicGain;
							array2[50] = (byte)appData.FunCfgs.BluetoothAudioGain;
							if (appData.FunCfgs.CallSign != null && appData.FunCfgs.CallSign != "")
							{
								byte[] bytes = Encoding.GetEncoding("gb2312").GetBytes(appData.FunCfgs.CallSign);
								Array.Copy(bytes, 0, array2, 52, bytes.Length);
							}
						}
						else if (num >= 40960 && num <= 41216)
						{
							switch (num)
							{
								case 40960:
									GetDTMFWord(array2, 0, appData.Dtmfs.LocalId);
									array2[5] = byte.MaxValue;
									array2[6] = (byte)appData.Dtmfs.Pttid;
									array2[7] = (byte)appData.Dtmfs.WordTime;
									array2[8] = (byte)appData.Dtmfs.IdleTime;
									GetDTMFWord(array2, 32, appData.Dtmfs.Group[0]);
									GetDTMFWord(array2, 48, appData.Dtmfs.Group[1]);
									break;
								case 41024:
									GetDTMFWord(array2, 0, appData.Dtmfs.Group[2]);
									GetDTMFWord(array2, 16, appData.Dtmfs.Group[3]);
									GetDTMFWord(array2, 32, appData.Dtmfs.Group[4]);
									GetDTMFWord(array2, 48, appData.Dtmfs.Group[5]);
									break;
								case 41088:
									GetDTMFWord(array2, 0, appData.Dtmfs.Group[6]);
									GetDTMFWord(array2, 16, appData.Dtmfs.Group[7]);
									GetDTMFWord(array2, 32, appData.Dtmfs.Group[8]);
									GetDTMFWord(array2, 48, appData.Dtmfs.Group[9]);
									break;
								case 41152:
									GetDTMFWord(array2, 0, appData.Dtmfs.Group[10]);
									GetDTMFWord(array2, 16, appData.Dtmfs.Group[11]);
									GetDTMFWord(array2, 32, appData.Dtmfs.Group[12]);
									GetDTMFWord(array2, 48, appData.Dtmfs.Group[13]);
									break;
								case 41216:
									GetDTMFWord(array2, 0, appData.Dtmfs.Group[14]);
									break;
							}
						}
						else
						{
							switch (num)
							{
								case 41472:
									GetBankName(array2, 0, appData.BankName[0]);
									GetBankName(array2, 16, appData.BankName[1]);
									GetBankName(array2, 32, appData.BankName[2]);
									GetBankName(array2, 48, appData.BankName[3]);
									break;
								case 41536:
									GetBankName(array2, 0, appData.BankName[4]);
									GetBankName(array2, 16, appData.BankName[5]);
									GetBankName(array2, 32, appData.BankName[6]);
									GetBankName(array2, 48, appData.BankName[7]);
									break;
								case 45056:
								{
									GetFMFreq(array2, 0, appData.Fms.CurFreq);
									for (int j = 0; j < 30; j++)
									{
										GetFMFreq(array2, 2 + j * 2, appData.Fms.Channels[j]);
									}

									array2[62] = 0;
									array2[63] = 0;
									break;
								}
							}
						}

						array = helper.LoadPackage(87, num, array2, (byte)array2.Length);
						port.WriteByte(array, 0, array.Length);
						timer.Start();
						progressVal = num * 100 / 45056;
						if (progressVal > 100)
						{
							progressVal = 100;
						}
						progressCont = "进度..." + progressVal + "%";
						statusQueue.Enqueue(new ProgressBarValue(progressVal,progressCont));
						step = Step.StepWrite2;
						break;
					case Step.StepWrite2:
						DebugWindow.GetInstance().updateDebugContent("Now: Step.StepWrite2");
						if (port.BytesToReadFromCache < 1)
						{
							break;
						}

						port.ReadByte(rxBuffer, 0, 1);
						if (rxBuffer[0] == 6)
						{
							timer.Stop();
							resetRetryCount();
							if (num >= 45056)
							{
								progressVal = 100;
								progressCont = "完成";
								statusQueue.Enqueue(new ProgressBarValue(progressVal,progressCont));
								flagTransmitting = false;
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

							step = Step.StepWrite1;
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
				port.WriteByte(array, 0, array.Length);
			}
		}

		return false;
	}

	private byte[] CaculateCtsDcs(string strData)
	{
		byte[] array = new byte[2] { 255, 255 };
		if ('D' == strData[0])
		{
			int i;
			for (i = 0; i < 210; i++)
			{
				if (strData == tblCTSDCS[i])
				{
					i++;
					break;
				}
			}

			array[0] = (byte)i;
			array[1] = 0;
		}
		else if (strData != "OFF")
		{
			int startIndex = strData.Length - 2;
			strData = strData.Remove(startIndex, 1);
			ushort num = ushort.Parse(strData);
			byte b = (byte)num;
			byte b2 = (byte)((num & 0xFF00) >> 8);
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
		byte[] array = new byte[4] { 255, 255, 255, 255 };
		int num = int.Parse(strDat.Remove(3, 1));
		for (int i = 0; i < 4; i++)
		{
			int num2 = num % 100;
			num /= 100;
			array[i] = (byte)((num2 / 10 << 4) | (num2 % 10));
		}

		return array;
	}

	private byte[] GetChannelInfos(int CH_Num)
	{
		int num = CH_Num / 64;
		int num2 = CH_Num % 64;
		byte[] array = new byte[32]
		{
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255
		};
		if (appData.ChannelList[num][num2].RxFreq != "")
		{
			byte[] sourceArray = CaculateFreq_StrToHex(appData.ChannelList[num][num2].RxFreq);
			Array.Copy(sourceArray, 0, array, 0, 4);
			if (appData.ChannelList[num][num2].TxFreq != "")
			{
				byte[] sourceArray2 = CaculateFreq_StrToHex(appData.ChannelList[num][num2].TxFreq);
				Array.Copy(sourceArray2, 0, array, 4, 4);
			}

			byte[] sourceArray3 = CaculateCtsDcs(appData.ChannelList[num][num2].StrRxCtsDcs);
			Array.Copy(sourceArray3, 0, array, 8, 2);
			sourceArray3 = CaculateCtsDcs(appData.ChannelList[num][num2].StrTxCtsDcs);
			Array.Copy(sourceArray3, 0, array, 10, 2);
			array[12] = (byte)appData.ChannelList[num][num2].SignalGroup;
			array[13] = (byte)appData.ChannelList[num][num2].Pttid;
			array[14] = (byte)appData.ChannelList[num][num2].TxPower;
			array[15] = 0;
			array[15] |= (byte)(appData.ChannelList[num][num2].Bandwide << 6);
			array[15] |= (byte)(appData.ChannelList[num][num2].ScanAdd << 2);
			if (appData.ChannelList[num][num2].Name != "")
			{
				int num3 = 0;
				byte[] bytes = Encoding.GetEncoding("gb2312").GetBytes(appData.ChannelList[num][num2].Name);
				num3 = ((bytes.Length > 12) ? 12 : bytes.Length);
				Array.Copy(bytes, 0, array, 20, num3);
			}
		}

		return array;
	}

	private void GetBankName(byte[] payload, int offset, string name)
	{
		int num = 0;
		if (name != "")
		{
			byte[] bytes = Encoding.GetEncoding("gb2312").GetBytes(name);
			num = bytes.Length;
			if (num > 12)
			{
				num = 12;
			}

			for (int i = 0; i < num; i++)
			{
				payload[i + offset] = bytes[i];
			}
		}
	}

	private byte[] GetVFOAInfos()
	{
		byte[] array = new byte[32]
		{
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 0, 0, 0,
			0, 0, 0, 255, 255, 255, 255, 255, 255, 255,
			255, 255
		};
		string s = appData.Vfos.VfoAFreq.Remove(3, 1);
		int num = int.Parse(s);
		for (int num2 = 7; num2 >= 0; num2--)
		{
			array[num2] = (byte)(num % 10);
			num /= 10;
		}

		byte[] sourceArray = CaculateCtsDcs(appData.Vfos.StrVfoaRxCtsDcs);
		Array.Copy(sourceArray, 0, array, 8, 2);
		sourceArray = CaculateCtsDcs(appData.Vfos.StrVfoaTxCtsDcs);
		Array.Copy(sourceArray, 0, array, 10, 2);
		array[13] = (byte)appData.Vfos.VfoABusyLock;
		array[14] = (byte)((appData.Vfos.VfoADir << 4) | appData.Vfos.VfoASignalGroup);
		array[16] = (byte)appData.Vfos.VfoATxPower;
		array[17] = (byte)(appData.Vfos.VfoABandwide << 6);
		array[19] = (byte)appData.Vfos.VfoAStep;
		string[] array2 = appData.Vfos.VfoAOffset.Split('.');
		int num3 = int.Parse(array2[0]) * 100000 + int.Parse(array2[1]) * 10;
		for (int num4 = 6; num4 >= 0; num4--)
		{
			array[20 + num4] = (byte)(num3 % 10);
			num3 /= 10;
		}

		return array;
	}

	private byte[] GetVFOBInfos()
	{
		byte[] array = new byte[32]
		{
			255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
			255, 255, 255, 255, 255, 255, 255, 0, 0, 0,
			0, 0, 0, 255, 255, 255, 255, 255, 255, 255,
			255, 255
		};
		string s = appData.Vfos.VfoBFreq.Remove(3, 1);
		int num = int.Parse(s);
		for (int num2 = 7; num2 >= 0; num2--)
		{
			array[num2] = (byte)(num % 10);
			num /= 10;
		}

		byte[] sourceArray = CaculateCtsDcs(appData.Vfos.StrVfobRxCtsDcs);
		Array.Copy(sourceArray, 0, array, 8, 2);
		sourceArray = CaculateCtsDcs(appData.Vfos.StrVfobTxCtsDcs);
		Array.Copy(sourceArray, 0, array, 10, 2);
		array[13] = (byte)appData.Vfos.VfoBBusyLock;
		array[14] = (byte)((appData.Vfos.VfoBDir << 4) | appData.Vfos.VfoBSignalGroup);
		array[16] = (byte)appData.Vfos.VfoBTxPower;
		array[17] = (byte)(appData.Vfos.VfoBBandwide << 6);
		array[19] = (byte)appData.Vfos.VfoBStep;
		string[] array2 = appData.Vfos.VfoBOffset.Split('.');
		int num3 = int.Parse(array2[0]) * 100000 + int.Parse(array2[1]) * 10;
		for (int num4 = 6; num4 >= 0; num4--)
		{
			array[20 + num4] = (byte)(num3 % 10);
			num3 /= 10;
		}

		return array;
	}

	private void GetFMFreq(byte[] payload, int offset, int freq)
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

	private void GetDTMFWord(byte[] payload, int offset, string word)
	{
		string text = "0123456789ABCD*#";
		int num = 0;
		if (!(word != ""))
		{
			return;
		}

		for (int i = 0; i < word.Length; i++)
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

	private void GetDTMFName(byte[] payload, int offset, string word)
	{
		int num = 0;
		if (word != "")
		{
			byte[] bytes = Encoding.GetEncoding("gb2312").GetBytes(word);
			num = bytes.Length;
			if (num > 12)
			{
				num = 12;
			}

			for (int i = 0; i < num; i++)
			{
				payload[i + offset] = bytes[i];
			}
		}
	}

	private void GetCallIDWord(byte[] payload, int offset, string callID)
	{
		if (callID != "")
		{
			byte[] bytes = Encoding.ASCII.GetBytes(callID);
			for (int i = 0; i < callID.Length; i++)
			{
				payload[i + offset] = bytes[i];
			}
		}
	}

	private void GetMDC1200_IDWord(byte[] payload, int offset, string id)
	{
		string text = "0123456789ABCDEF";
		if (id != "")
		{
			for (int i = 0; i < id.Length; i++)
			{
				payload[i + offset] = (byte)text.IndexOf(id[i]);
			}
		}
	}

	private bool Read(CancellationToken cancellationToken)
	{
		byte[][] array = null;
		byte[] array2 = new byte[10];
		ushort num = 0;
		int num2 = 0;
		while (flagTransmitting && !cancellationToken.IsCancellationRequested)
		{
			if (!flagRetry)
			{
				switch (step)
				{
					case Step.StepRead1:
						DebugWindow.GetInstance().updateDebugContent("Now: Step.StepRead1");
						array2 = new byte[4]
						{
							82,
							(byte)(num >> 8),
							(byte)num,
							64
						};
						port.WriteByte(array2, 0, array2.Length);
						progressVal = num * 100 / 45056;
						if (progressVal > 100)
						{
							progressVal = 100;
						}

						progressCont = "进度..." + progressVal + "%";
						statusQueue.Enqueue(new ProgressBarValue(progressVal,progressCont));
						resetRetryCount();
						timer.Start();
						step = Step.StepRead2;
						break;
					case Step.StepRead2:
					{
						DebugWindow.GetInstance().updateDebugContent("Now: Step.StepRead2");
						if (port.BytesToReadFromCache < 68)
						{
							break;
						}

						timer.Stop();
						resetRetryCount();
						port.ReadByte(rxBuffer, 0, port.BytesToReadFromCache);
						byte b = (byte)(num >> 8);
						byte b2 = (byte)num;
						if (rxBuffer[1] != b || rxBuffer[2] != b2)
						{
							break;
						}

						array = new byte[2][]
						{
							new byte[32],
							new byte[32]
						};
						for (int i = 0; i < 32; i++)
						{
							array[0][i] = rxBuffer[i + 4];
							array[1][i] = rxBuffer[i + 4 + 32];
						}

						if (num < 16384)
						{
							SetChannelInfos(num2++, array[0]);
							SetChannelInfos(num2++, array[1]);
						}
						else if (num == 32768)
						{
							SetVFOAInfos(array[0]);
							SetVFOBInfos(array[1]);
						}
						else if (num == 36864)
						{
							appData.FunCfgs.Sql = rxBuffer[4] % 10;
							appData.FunCfgs.SaveMode = rxBuffer[5] % 4;
							appData.FunCfgs.Vox = rxBuffer[6] % 10;
							appData.FunCfgs.Backlight = rxBuffer[7] % 9;
							appData.FunCfgs.DualStandby = rxBuffer[8] % 2;
							appData.FunCfgs.Tot = rxBuffer[9] % 9;
							appData.FunCfgs.Beep = rxBuffer[10] % 2;
							appData.FunCfgs.VoiceSw = rxBuffer[11] % 2;
							appData.FunCfgs.SideTone = rxBuffer[13] % 4;
							appData.FunCfgs.ScanMode = rxBuffer[14] % 3;
							appData.Vfos.Pttid = rxBuffer[15] % 4;
							appData.FunCfgs.PttDly = rxBuffer[16] % 16;
							appData.FunCfgs.ChADisType = rxBuffer[17] % 3;
							appData.FunCfgs.ChBDisType = rxBuffer[18] % 3;
							appData.FunCfgs.AutoLock = rxBuffer[20] % 7;
							appData.FunCfgs.AlarmMode = rxBuffer[21] % 3;
							appData.FunCfgs.LocalSosTone = rxBuffer[22] % 2;
							appData.FunCfgs.TailClear = rxBuffer[24] % 2;
							appData.FunCfgs.RptTailClear = rxBuffer[25] % 11;
							appData.FunCfgs.RptTailDet = rxBuffer[26] % 11;
							appData.FunCfgs.Roger = rxBuffer[27] % 2;
							appData.FunCfgs.FmEnable = rxBuffer[29] % 2;
							appData.FunCfgs.ChAWorkmode = (rxBuffer[30] & 0xF) % 2;
							appData.FunCfgs.ChBWorkmode = ((rxBuffer[30] & 0xF0) >> 4) % 2;
							appData.FunCfgs.KeyLock = rxBuffer[31] % 2;
							appData.FunCfgs.PowerOnDisType = rxBuffer[32] % 22;
							appData.FunCfgs.Tone = rxBuffer[34] % 4;
							appData.FunCfgs.VoxDlyTime = rxBuffer[36] % 16;
							appData.FunCfgs.MenuQuitTime = rxBuffer[37] % 11;
							appData.FunCfgs.MicGain = rxBuffer[38] % 3;
							appData.FunCfgs.PwrOnDlyTime = rxBuffer[40] % 15;
							appData.FunCfgs.VoxSw = rxBuffer[41] % 2;
							appData.FunCfgs.Key2Short = rxBuffer[46] % 5;
							appData.FunCfgs.Key2Long = rxBuffer[47] % 5;
							appData.FunCfgs.CurBankA = rxBuffer[50] % 8;
							appData.FunCfgs.CurBankB = rxBuffer[51] % 8;
							appData.FunCfgs.BluetoothAudioGain = rxBuffer[53] % 5;
							appData.FunCfgs.BtMicGain = rxBuffer[54] % 5;
							int num3 = 0;
							for (int j = 0; j < 6 && rxBuffer[56 + j] != byte.MaxValue; j++)
							{
								num3++;
							}

							appData.FunCfgs.CallSign = Encoding.GetEncoding("gb2312").GetString(rxBuffer, 56, num3);
						}
						else if (num >= 40960 && num <= 41216)
						{
							switch (num)
							{
								case 40960:
									appData.Dtmfs.LocalId = SetDTMFWord(rxBuffer, 4);
									appData.Dtmfs.Pttid = rxBuffer[10];
									appData.Dtmfs.WordTime = rxBuffer[11];
									appData.Dtmfs.IdleTime = rxBuffer[12];
									appData.Dtmfs.Group[0] = SetDTMFWord(rxBuffer, 36);
									appData.Dtmfs.Group[1] = SetDTMFWord(rxBuffer, 52);
									break;
								case 41024:
									appData.Dtmfs.Group[2] = SetDTMFWord(rxBuffer, 4);
									appData.Dtmfs.Group[3] = SetDTMFWord(rxBuffer, 20);
									appData.Dtmfs.Group[4] = SetDTMFWord(rxBuffer, 36);
									appData.Dtmfs.Group[5] = SetDTMFWord(rxBuffer, 52);
									break;
								case 41088:
									appData.Dtmfs.Group[6] = SetDTMFWord(rxBuffer, 4);
									appData.Dtmfs.Group[7] = SetDTMFWord(rxBuffer, 20);
									appData.Dtmfs.Group[8] = SetDTMFWord(rxBuffer, 36);
									appData.Dtmfs.Group[9] = SetDTMFWord(rxBuffer, 52);
									break;
								case 41152:
									appData.Dtmfs.Group[10] = SetDTMFWord(rxBuffer, 4);
									appData.Dtmfs.Group[11] = SetDTMFWord(rxBuffer, 20);
									appData.Dtmfs.Group[12] = SetDTMFWord(rxBuffer, 36);
									appData.Dtmfs.Group[13] = SetDTMFWord(rxBuffer, 52);
									break;
								case 41216:
									appData.Dtmfs.Group[14] = SetDTMFWord(rxBuffer, 4);
									break;
							}
						}
						else
						{
							switch (num)
							{
								case 41472:
									appData.BankName[0] = SetBankName(rxBuffer, 4);
									appData.BankName[1] = SetBankName(rxBuffer, 20);
									appData.BankName[2] = SetBankName(rxBuffer, 36);
									appData.BankName[3] = SetBankName(rxBuffer, 52);
									break;
								case 41536:
									appData.BankName[4] = SetBankName(rxBuffer, 4);
									appData.BankName[5] = SetBankName(rxBuffer, 20);
									appData.BankName[6] = SetBankName(rxBuffer, 36);
									appData.BankName[7] = SetBankName(rxBuffer, 52);
									break;
								case 45056:
								{
									appData.Fms.CurFreq = SetFMFreq(rxBuffer, 4);
									for (int k = 0; k < 30; k++)
									{
										appData.Fms.Channels[k] = SetFMFreq(rxBuffer, 6 + k * 2);
									}

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

							timer.Start();
							step = Step.StepRead1;
							break;
						}

						progressVal = 100;
						progressCont = "完成";
						statusQueue.Enqueue(new ProgressBarValue(progressVal,progressCont));
						array2 = new byte[1] { 69 };
						port.WriteByte(array2, 0, array2.Length);
						flagTransmitting = false;
						return true;
					}
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
				port.WriteByte(array2, 0, array2.Length);
			}
		}

		return false;
	}

	private string CaculateFreq_HexToStr(byte[] dat, int offset)
	{
		uint num = 0u;
		for (int i = 0; i < 4; i++)
		{
			dat[i + offset] = (byte)(((dat[i + offset] >> 4) & 0xF) * 10 + (dat[i + offset] & 0xF));
		}

		for (int num2 = 3; num2 >= 0; num2--)
		{
			num = num * 100 + dat[num2 + offset];
		}

		return num.ToString().Insert(3, ".");
	}

	private string CaculateCtsDcs(byte[] dat, int offset)
	{
		string text = "";
		try
		{
			if (dat[1 + offset] == 0)
			{
				if (dat[offset] != 0 && dat[offset] <= 210)
				{
					return tblCTSDCS[dat[offset] - 1];
				}

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

	private void SetChannelInfos(int CH_Num, byte[] dat)
	{
		int num = CH_Num / 64;
		int num2 = CH_Num % 64;
		if (dat[0] == byte.MaxValue || dat[1] == byte.MaxValue || dat[3] == 0)
		{
			return;
		}

		appData.ChannelList[num][num2].RxFreq = CaculateFreq_HexToStr(dat, 0);
		if (!string.IsNullOrEmpty(appData.ChannelList[num][num2].RxFreq))
			appData.ChannelList[num][num2].IsVisable = true;
		if (dat[4] != byte.MaxValue && dat[5] != byte.MaxValue)
		{
			appData.ChannelList[num][num2].TxFreq = CaculateFreq_HexToStr(dat, 4);
		}

		appData.ChannelList[num][num2].StrRxCtsDcs = CaculateCtsDcs(dat, 8);
		appData.ChannelList[num][num2].StrTxCtsDcs = CaculateCtsDcs(dat, 10);
		appData.ChannelList[num][num2].SignalGroup = dat[12] % 20;
		appData.ChannelList[num][num2].Pttid = dat[13] % 4;
		appData.ChannelList[num][num2].TxPower = dat[14] % 2;
		appData.ChannelList[num][num2].Bandwide = (dat[15] >> 6) & 1;
		appData.ChannelList[num][num2].BusyLock = (dat[15] >> 3) & 1;
		appData.ChannelList[num][num2].ScanAdd = (dat[15] >> 2) & 1;
		if (dat[20] != byte.MaxValue)
		{
			int num3 = 0;
			for (int i = 0; i < 12 && dat[20 + i] != byte.MaxValue; i++)
			{
				num3++;
			}

			appData.ChannelList[num][num2].Name = Encoding.GetEncoding("gb2312").GetString(dat, 20, num3);
		}
	}

	private string SetBankName(byte[] dat, int offset)
	{
		string result = "";
		int num = 0;
		if (dat[offset] != byte.MaxValue)
		{
			for (int i = 0; i < 12 && dat[i + offset] != byte.MaxValue; i++)
			{
				num++;
			}

			result = Encoding.GetEncoding("gb2312").GetString(dat, offset, num);
		}

		return result;
	}

	private string CaculateFreq(byte[] dat)
	{
		string text = "";
		int num = 0;
		try
		{
			for (int i = 0; i < 8; i++)
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
		string text = "00.0000";
		int num = 0;
		try
		{
			for (int i = 0; i < 7; i++)
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

	private void SetVFOAInfos(byte[] dat)
	{
		string text = CaculateFreq(dat);
		if (text != "")
		{
			appData.Vfos.VfoAFreq = text;
		}

		appData.Vfos.StrVfoaRxCtsDcs = CaculateCtsDcs(dat, 8);
		appData.Vfos.StrVfoaTxCtsDcs = CaculateCtsDcs(dat, 10);
		appData.Vfos.VfoABusyLock = dat[13] % 2;
		appData.Vfos.VfoASignalGroup = (dat[14] & 0xF) % 16;
		appData.Vfos.VfoADir = ((dat[14] >> 4) & 3) % 3;
		appData.Vfos.VfoATxPower = (dat[16] & 0xF) % 2;
		appData.Vfos.VfoAScram = ((dat[16] >> 4) & 0xF) % 9;
		appData.Vfos.VfoABandwide = (dat[17] >> 6) & 1;
		appData.Vfos.VfoAStep = dat[19] % 8;
		appData.Vfos.VfoAOffset = CaculateOffset(dat, 20);
	}

	private void SetVFOBInfos(byte[] dat)
	{
		string text = CaculateFreq(dat);
		if (text != "")
		{
			appData.Vfos.VfoBFreq = text;
		}

		appData.Vfos.StrVfobRxCtsDcs = CaculateCtsDcs(dat, 8);
		appData.Vfos.StrVfobTxCtsDcs = CaculateCtsDcs(dat, 10);
		appData.Vfos.VfoBBusyLock = dat[13] % 2;
		appData.Vfos.VfoBSignalGroup = (dat[14] & 0xF) % 16;
		appData.Vfos.VfoBDir = ((dat[14] >> 4) & 3) % 3;
		appData.Vfos.VfoBTxPower = (dat[16] & 0xF) % 2;
		appData.Vfos.VfoBScram = ((dat[16] >> 4) & 0xF) % 9;
		appData.Vfos.VfoBBandwide = (dat[17] >> 6) & 1;
		appData.Vfos.VfoBStep = dat[19] % 8;
		appData.Vfos.VfoBOffset = CaculateOffset(dat, 20);
	}

	private int SetFMFreq(byte[] payload, int offset)
	{
		int num = 0;
		if (payload[offset] != byte.MaxValue && payload[offset + 1] != byte.MaxValue)
		{
			num = payload[offset] + (payload[offset + 1] << 8);
		}

		if (num < 650 || num > 1080)
		{
			num = 0;
		}

		return num;
	}

	private string SetDTMFWord(byte[] payload, int offset)
	{
		string text = "0123456789ABCD*#";
		string text2 = "";
		int num = 0;
		if (payload[offset] != byte.MaxValue)
		{
			for (int i = 0; i < 6 && payload[offset + i] != byte.MaxValue; i++)
			{
				num++;
			}

			for (int j = 0; j < num; j++)
			{
				text2 += text[payload[offset + j] % 16];
			}
		}

		return text2;
	}

	private string SetDTMFGroupName(byte[] payload, int offset)
	{
		string result = "";
		int num = 0;
		if (payload[offset] != byte.MaxValue)
		{
			for (int i = 0; i < 12 && payload[offset + i] != byte.MaxValue; i++)
			{
				num++;
			}

			result = Encoding.GetEncoding("gb2312").GetString(payload, offset, num);
		}

		return result;
	}

	private string SetCallID(byte[] payload, int offset)
	{
		string result = "";
		int num = 0;
		if (payload[offset] != byte.MaxValue)
		{
			for (int i = 0; i < 6 && payload[offset + i] != byte.MaxValue; i++)
			{
				num++;
			}

			result = Encoding.ASCII.GetString(payload, offset, num);
		}

		return result;
	}

	private string SetMDC1200ID(byte[] payload, int offset)
	{
		string text = "0123456789ABCDEF";
		string text2 = "";
		int num = 0;
		if (payload[offset] != byte.MaxValue)
		{
			for (int i = 0; i < 4 && payload[offset + i] != byte.MaxValue; i++)
			{
				num++;
			}

			text2 = "";
			try
			{
				for (int j = 0; j < 4; j++)
				{
					text2 += text[payload[offset + j]];
				}
			}
			catch
			{
				text2 = "1111";
			}
		}

		return text2;
	}
}