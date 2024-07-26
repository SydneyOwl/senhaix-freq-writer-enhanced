namespace SenhaixFreqWriter.Constants.Shx8x00;

public enum State
{
    HandShakeStep1,
    HandShakeStep2,
    HandShakeStep3,
    HandShakeStep4,
    ReadStep1,
    ReadStep2,
    ReadStep3,
    WriteStep1,
    WriteStep2
}

// 新版8600写开机画面
public enum NImgStep
{
    Step_HandShake_Jump1,
    Step_HandShake_Jump2,
    Step_HandShake_Jump3,
    Step_HandShake_Jump4,
    Step_HandShake,
    Step_SetFontAddress,
    Step_SetImageAddress,
    Step_SetVoiceAddress,
    Step_Erase,
    Step_Data,
    Step_Receive_1,
    Step_Receive_2,
    Step_Over
}