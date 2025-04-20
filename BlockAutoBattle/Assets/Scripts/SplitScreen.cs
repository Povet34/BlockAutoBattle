using UnityEngine;

[ExecuteInEditMode]
public class SplitScreen : MonoBehaviour
{
    public Camera makeViewCam;
    public Camera fightViewCam;

    void Start()
    {
        // 왼쪽 절반
        makeViewCam.rect = new Rect(0f, 0f, 0.5f, 1f);

        // 오른쪽 절반
        fightViewCam.rect = new Rect(0.5f, 0f, 0.5f, 1f);
    }
}