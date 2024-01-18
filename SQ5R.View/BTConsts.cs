namespace SQ5R.View;

// migrated from shx8800-ble-connector
public class BTConsts
{
    public static byte[] FINAL_DATA_STARTER = { 87, 28, 00 };
    public static int BT_CONNECT_TIMEOUT = 5;
    public static string BTNAME_SHX8800 = "walkie-talkie";

    public static int BT_SCAN_TIMEOUT = 10;

    public static string MANUFACTURER_CHARACTERISTIC_UUID = "2a29";
    public static string FIRMWARE_REVISION_CHARACTERISTIC_UUID = "2a26";
    public static string MODEL_NUMBER_CHARACTERISTIC_UUID = "2a24";
    public static string CHECK_CHARACTERISTIC_UUID = "ff31";
    public static string RW_CHARACTERISTIC_UUID = "ffe1";

    public static int STATUS_ERROR = -1;
    public static int STATUS_READY = 0;
    public static int STATUS_CONN_SUCCESS = 1;
    public static int STATUS_CONN_FAILED = 2;
    public static int STATUS_FINDING_CHARACTERISTIC = 3;
    public static int STATUS_DONE = 4;
}