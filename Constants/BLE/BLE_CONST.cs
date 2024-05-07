namespace SenhaixFreqWriter.Constants.BLE;

public class BleConst
{
    public static byte[] FinalDataStarter = { 87, 28, 00 };
    public static int BtConnectTimeout = 5;
    public static string BtnameShx8800 = "walkie-talkie";

    public static int BtScanTimeout = 10;

    public static string ManufacturerCharacteristicUuid = "2a29";
    public static string FirmwareRevisionCharacteristicUuid = "2a26";
    public static string ModelNumberCharacteristicUuid = "2a24";
    public static string CheckCharacteristicUuid = "ff31";
    public static string RwServiceUuid = "ffe0";
    public static string RwCharacteristicUuid = "ffe1";

    public static int StatusError = -1;
    public static int StatusReady = 0;
    public static int StatusConnSuccess = 1;
    public static int StatusConnFailed = 2;
    public static int StatusFindingCharacteristic = 3;
    public static int StatusDone = 4;
}