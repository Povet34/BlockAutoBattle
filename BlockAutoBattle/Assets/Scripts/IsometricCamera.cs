using UnityEngine;
using Unity.Cinemachine;

[ExecuteInEditMode]
public class IsometricCamera : MonoBehaviour
{
    [Header("Isometric View Settings")]
    public Transform target; // ī�޶� ���� Ÿ��
    public float cameraHeight = 10f; // ī�޶� ����
    public float cameraDistance = 10f; // ī�޶� �Ÿ�
    public float cameraAngle = 30f; // ī�޶� ����

    private CinemachineCamera cinemachineCamera;
    private CinemachineOrbitalFollow orbitalFollow;

    void Start()
    {
        // Cinemachine Camera�� ������ �߰�
        cinemachineCamera = GetComponent<CinemachineCamera>();
        if (cinemachineCamera == null)
        {
            cinemachineCamera = gameObject.AddComponent<CinemachineCamera>();
        }

        // CinemachineOrbitalFollow ����
        orbitalFollow = cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
        if (orbitalFollow == null)
        {
            orbitalFollow = gameObject.AddComponent<CinemachineOrbitalFollow>();
        }

        // Ÿ�� ����
        if (target != null)
        {
            cinemachineCamera.Follow = target;
        }

        // ī�޶� ����, �Ÿ�, ���� ����
        orbitalFollow.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;
        orbitalFollow.Radius = cameraDistance;
        orbitalFollow.VerticalAxis.Value = cameraAngle; // ī�޶� ����
    }

    void Update()
    {
        // ī�޶� ���� ������Ʈ
        if (orbitalFollow != null)
        {
            orbitalFollow.Radius = cameraDistance;
            orbitalFollow.VerticalAxis.Value = cameraAngle;
            orbitalFollow.TargetOffset = new Vector3(0, cameraHeight, 0);
        }
    }
}