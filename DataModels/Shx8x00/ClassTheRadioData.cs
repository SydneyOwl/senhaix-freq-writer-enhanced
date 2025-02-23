using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using MsBox.Avalonia;
using Newtonsoft.Json;
using SenhaixFreqWriter.DataModels.Interfaces;
using SenhaixFreqWriter.Utils.Other;

namespace SenhaixFreqWriter.DataModels.Shx8x00;

[Serializable]
public class ClassTheRadioData : IBackupable
{
    [JsonIgnore] public static ClassTheRadioData Instance;

    //TODO 无法直接反序列化到chanData, 只能这样一下
    public List<ChannelData> ChanneldataList = new();

    public DtmfData DtmfData = new();

    public FunCfgData FunCfgData = new();

    [JsonIgnore] public ObservableCollection<ChannelData> ObsChanData = new();
    
    [JsonIgnore] private UndoRedoStack<List<ChannelData>> _undoRedoStack = new();

    public OtherImfData OtherImfData = new();

    public ClassTheRadioData()
    {
        for (var i = 0; i < 128; i++)
        {
            var data = new ChannelData();
            data.ChanNum = i.ToString();
            ObsChanData.Add(data);
        }
        ObsChanData.CollectionChanged += CollectionChangedHandler;
    }

    public void SaveToFile(Stream s)
    {
        var serializer = new JsonSerializer();
        serializer.Formatting = Formatting.Indented;
        using (var streamWriter = new StreamWriter(s, Encoding.UTF8))
        {
            Instance.ChanneldataList = Instance.ObsChanData.ToList();
            serializer.Serialize(streamWriter, Instance);
        }
    }


    public void CreatObjFromFile(Stream s)
    {
        using (var streamReader = new StreamReader(s, Encoding.UTF8))
        {
            var res = streamReader.ReadToEnd();
            ClassTheRadioData tmp;
            
            Instance.ObsChanData.CollectionChanged -= CollectionChangedHandler;
            try
            {
                var jsonSerializer = new JsonSerializer();
                var stringReader = new JsonTextReader(new StringReader(res));
                tmp = jsonSerializer.Deserialize<ClassTheRadioData>(stringReader);
                Instance.FunCfgData = tmp.FunCfgData;
                Instance.DtmfData = tmp.DtmfData;
                Instance.OtherImfData = tmp.OtherImfData;
                Instance.ObsChanData.Clear();
                foreach (var cd in tmp.ChanneldataList)
                {
                    if (!cd.AllEmpty()) cd.IsVisable = true;
                    Instance.ObsChanData.Add(cd);
                }
            }
            catch
            {
                MessageBoxManager.GetMessageBoxStandard("注意", "无效的文件").ShowAsync();
            }
            
            Instance.ObsChanData.CollectionChanged += CollectionChangedHandler;
        }
    }

    public static ClassTheRadioData GetInstance()
    {
        if (Instance != null) return Instance;

        Instance = new ClassTheRadioData();
        return Instance;
    }

    public void ForceNewChannel()
    {
        Instance.ObsChanData.CollectionChanged -= CollectionChangedHandler;
        Instance.ObsChanData.Clear();
        for (var i = 0; i < 128; i++)
        {
            var chan = new ChannelData();
            chan.ChanNum = i.ToString();
            chan.IsVisable = false;
            Instance.ObsChanData.Add(chan);
        }
        Instance.ObsChanData.CollectionChanged += CollectionChangedHandler;
    }
    
    public void ForceNewChannel(List<ChannelData> chanData)
    {
        Instance.ObsChanData.CollectionChanged -= CollectionChangedHandler;
        Instance.ObsChanData.Clear();
        for (var i = 0; i < 128; i++)
        {
            Instance.ObsChanData.Add(chanData[i]);
        }
        Instance.ObsChanData.CollectionChanged += CollectionChangedHandler;
    }
    
    private void CollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Console.WriteLine("Collection changed");
        SaveChanges();
    }

    public void Undo()
    {
        try
        {
            var lastOpData = _undoRedoStack.Undo();
            Instance.ForceNewChannel(lastOpData);
        }
        catch (Exception)
        {
            //ignored;stack is Empty 
        }
    }
    
    public void Redo()
    {
        try
        { 
            var lastOpData = _undoRedoStack.Redo();
           Instance.ForceNewChannel(lastOpData);
        }
        catch (Exception)
        {
            //ignored;stack is Empty 
        }
    }

    public void SaveChanges()
    {
        // Sometimes the dataset is null...
        if(ObsChanData.Count == 0)return;
        Instance._undoRedoStack.Execute(ObsChanData.Select(item => item.DeepCopy()).ToList());
    }

    public void ClearStack()
    {
        _undoRedoStack.ClearStack();
    }
}