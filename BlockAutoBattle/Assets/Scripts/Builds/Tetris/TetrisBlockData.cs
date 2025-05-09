using UnityEngine;

[CreateAssetMenu(fileName = "NewTetrisBlock", menuName = "Scriptable Objects/TetrisBlockData", order = 1)]
public class TetrisBlockData : ScriptableObject
{
    public string blockName; // 블록 이름
    public Color blockColor = Color.white; // 블록 색상
    public Vector3[] cubePositions; // 큐브의 상대적 위치 배열
    public int centerIndex; // 중심점으로 사용할 큐브의 인덱스
    public Vector3 center; // 중심점 (캐싱된 값)
}
