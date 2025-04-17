using UnityEngine;

[CreateAssetMenu(fileName = "NewTetrisBlock", menuName = "Tetris Block", order = 1)]
public class TetrisBlock : ScriptableObject
{
    public string blockName; // ��� �̸�
    public Color blockColor = Color.white; // ��� ����
    public Vector3[] cubePositions; // ť���� ����� ��ġ �迭
}