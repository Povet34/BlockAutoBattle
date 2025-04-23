using UnityEngine;

[CreateAssetMenu(fileName = "ConstructCardData", menuName = "Scriptable Objects/ConstructCardData")]
public class ConstructCardData : ScriptableObject
{
    public TetrisBlockData tetrisBlockData; // ��Ʈ���� ��� ������
    public int cost; // ī�� ��� ���
    public Sprite cardMainImage;

    public string description; // ī�� ����
}
