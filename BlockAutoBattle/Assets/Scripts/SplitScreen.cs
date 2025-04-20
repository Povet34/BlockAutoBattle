using UnityEngine;

[ExecuteInEditMode]
public class SplitScreen : MonoBehaviour
{
    public Camera makeViewCam;
    public Camera fightViewCam;

    void Start()
    {
        // ���� ����
        makeViewCam.rect = new Rect(0f, 0f, 0.5f, 1f);

        // ������ ����
        fightViewCam.rect = new Rect(0.5f, 0f, 0.5f, 1f);
    }
}