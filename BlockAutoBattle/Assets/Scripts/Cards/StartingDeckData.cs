using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StartingDeckData", menuName = "Scriptable Objects/StartingDeckData")]
public class StartingDeckData : ScriptableObject
{
    public List<ConstructCardData> constructCardDatas;
}
