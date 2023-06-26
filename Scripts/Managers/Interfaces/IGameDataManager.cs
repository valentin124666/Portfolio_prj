using System.Collections;
using System.Collections.Generic;
using Data.Characteristics;
using UnityEngine;

public interface IGameDataManager
{
    T GetDataScriptable<T>() where T : ScriptableObject;
    T Registration<T>(IDataClient client) where T : IData;
    void SaveDataClients();
}
