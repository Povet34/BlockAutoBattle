using UnityEngine;

[ExecuteInEditMode]
public class SplitScreen : MonoBehaviour
{
    public Camera cameraA;
    public Camera cameraB;

    void Start()
    {
        // ���� ����
        cameraA.rect = new Rect(0f, 0f, 0.5f, 1f);

        // ������ ����
        cameraB.rect = new Rect(0.5f, 0f, 0.5f, 1f);
    }
}