using UnityEngine;

[CreateAssetMenu(fileName = "NewTetrisBlock", menuName = "Tetris Block", order = 1)]
public class TetrisBlockData : ScriptableObject
{
    public string blockName; // ��� �̸�
    public Color blockColor = Color.white; // ��� ����
    public Vector3[] cubePositions; // ť���� ����� ��ġ �迭
    public int centerIndex; // �߽������� ����� ť���� �ε���
    public Vector3 center; // �߽��� (ĳ�̵� ��)

    // �߽��� ��ȯ
    public Vector3 GetCenter()
    {
        if (cubePositions != null && centerIndex >= 0 && centerIndex < cubePositions.Length)
        {
            center = cubePositions[centerIndex];
            return center;
        }
        Debug.LogWarning($"Invalid centerIndex for {blockName}. Defaulting to Vector3.zero.");
        return Vector3.zero; // �⺻��
    }
}
