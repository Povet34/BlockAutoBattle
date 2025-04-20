using UnityEngine;

[ExecuteInEditMode]
public class SplitScreen : MonoBehaviour
{
    public Camera cameraA;
    public Camera cameraB;

    void Start()
    {
        // 왼쪽 절반
        cameraA.rect = new Rect(0f, 0f, 0.5f, 1f);

        // 오른쪽 절반
        cameraB.rect = new Rect(0.5f, 0f, 0.5f, 1f);
    }
}