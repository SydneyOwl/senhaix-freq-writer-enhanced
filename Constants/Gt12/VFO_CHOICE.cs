using System.Collections.ObjectModel;

namespace SenhaixFreqWriter.Constants.Gt12;

public class VFO_CHOICE
{
    public static ObservableCollection<string> qtdqt = new()
    {
        "OFF", "67.0", "69.3", "71.9", "74.4", "77.0", "79.7", "82.5", "85.4", "88.5",
        "91.5", "94.8", "97.4", "100.0", "103.5", "107.2", "110.9", "114.8", "118.8", "123.0",
        "127.3", "131.8", "136.5", "141.3", "146.2", "151.4", "156.7", "159.8", "162.2", "165.5",
        "167.9", "171.3", "173.8", "177.3", "179.9", "183.5", "186.2", "189.9", "192.8", "196.6",
        "199.5", "203.5", "206.5", "210.7", "218.1", "225.7", "229.1", "233.6", "241.8", "250.3",
        "254.1", "D023N", "D025N", "D026N", "D031N", "D032N", "D036N", "D043N", "D047N", "D051N",
        "D053N", "D054N", "D065N", "D071N", "D072N", "D073N", "D074N", "D114N", "D115N", "D116N",
        "D122N", "D125N", "D131N", "D132N", "D134N", "D143N", "D145N", "D152N", "D155N", "D156N",
        "D162N", "D165N", "D172N", "D174N", "D205N", "D212N", "D223N", "D225N", "D226N", "D243N",
        "D244N", "D245N", "D246N", "D251N", "D252N", "D255N", "D261N", "D263N", "D265N", "D266N",
        "D271N", "D274N", "D306N", "D311N", "D315N", "D325N", "D331N", "D332N", "D343N", "D346N",
        "D351N", "D356N", "D364N", "D365N", "D371N", "D411N", "D412N", "D413N", "D423N", "D431N",
        "D432N", "D445N", "D446N", "D452N", "D454N", "D455N", "D462N", "D464N", "D465N", "D466N",
        "D503N", "D506N", "D516N", "D523N", "D526N", "D532N", "D546N", "D565N", "D606N", "D612N",
        "D624N", "D627N", "D631N", "D632N", "D645N", "D654N", "D662N", "D664N", "D703N", "D712N",
        "D723N", "D731N", "D732N", "D734N", "D743N", "D754N", "D023I", "D025I", "D026I", "D031I",
        "D032I", "D036I", "D043I", "D047I", "D051I", "D053I", "D054I", "D065I", "D071I", "D072I",
        "D073I", "D074I", "D114I", "D115I", "D116I", "D122I", "D125I", "D131I", "D132I", "D134I",
        "D143I", "D145I", "D152I", "D155I", "D156I", "D162I", "D165I", "D172I", "D174I", "D205I",
        "D212I", "D223I", "D225I", "D226I", "D243I", "D244I", "D245I", "D246I", "D251I", "D252I",
        "D255I", "D261I", "D263I", "D265I", "D266I", "D271I", "D274I", "D306I", "D311I", "D315I",
        "D325I", "D331I", "D332I", "D343I", "D346I", "D351I", "D356I", "D364I", "D365I", "D371I",
        "D411I", "D412I", "D413I", "D423I", "D431I", "D432I", "D445I", "D446I", "D452I", "D454I",
        "D455I", "D462I", "D464I", "D465I", "D466I", "D503I", "D506I", "D516I", "D523I", "D526I",
        "D532I", "D546I", "D565I", "D606I", "D612I", "D624I", "D627I", "D631I", "D632I", "D645I",
        "D654I", "D662I", "D664I", "D703I", "D712I", "D723I", "D731I", "D732I", "D734I", "D743I",
        "D754I"
    };

    public static ObservableCollection<string> busyLock = new ObservableCollection<string>()
    {
        "关", "开"
    };

    public static ObservableCollection<string> sigSys = new()
    {
        "无", "SDC", "DTMF"
    };

    public static ObservableCollection<string> direction = new()
    {
        "OFF", "+", "-"
    };

    public static ObservableCollection<string> sigGroup = new()
    {
        "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
        "11", "12", "13", "14", "15", "16", "17", "18", "19", "20"
    };
    public static ObservableCollection<string> step = new()
    {
        "2.5  KHz", "5.0  KHz", "6.25 KHz", "10.0 KHz", "12.5 KHz", "20.0 KHz", "25.0 KHz", "50.0 KHz" 
    };

    public static ObservableCollection<string> bandwidth = new()
    {
        "宽", "窄"
    };

    public static ObservableCollection<string> power = new()
    {
        "高", "中", "低"
    };
    public static ObservableCollection<string> sqmode = new()
    {
        "QT/DQT", "QT/DQT*DTMF", "QT/DQT+DTMF"
    };

    public static ObservableCollection<string> pttid = new()
    {
        "无", "发射开始", "发射结束", "两者"
    };
}