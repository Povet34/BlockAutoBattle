using UnityEngine;

[CreateAssetMenu(fileName = "NewTetrisBlock", menuName = "Scriptable Objects/TetrisBlockData", order = 1)]
public class TetrisBlockData : ScriptableObject
{
    public string blockName; // ��� �̸�
    public Color blockColor = Color.white; // ��� ����
    public Vector3[] cubePositions; // ť���� ����� ��ġ �迭
    public int centerIndex; // �߽������� ����� ť���� �ε���
    public Vector3 center; // �߽��� (ĳ�̵� ��)
}
