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
    StepHandShakeJump1,
    StepHandShakeJump2,
    StepHandShakeJump3,
    StepHandShakeJump4,
    StepHandShake,
    StepSetFontAddress,
    StepSetImageAddress,
    StepSetVoiceAddress,
    StepErase,
    StepData,
    StepReceive1,
    StepReceive2,
    StepOver
}