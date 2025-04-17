using UnityEngine;

[CreateAssetMenu(fileName = "NewTetrisBlock", menuName = "Tetris Block", order = 1)]
public class TetrisBlock : ScriptableObject
{
    public string blockName; // 블록 이름
    public Color blockColor = Color.white; // 블록 색상
    public Vector3[] cubePositions; // 큐브의 상대적 위치 배열
}