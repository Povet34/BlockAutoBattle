using UnityEngine;

[CreateAssetMenu(fileName = "ConstructCardData", menuName = "Scriptable Objects/ConstructCardData")]
public class ConstructCardData : ScriptableObject
{
    public TetrisBlockData tetrisBlockData; // 테트리스 블록 데이터
    public int cost; // 카드 사용 비용

    public string description; // 카드 설명
}
