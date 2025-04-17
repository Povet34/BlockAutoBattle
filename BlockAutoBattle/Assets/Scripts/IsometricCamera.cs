using UnityEngine;
using Unity.Cinemachine;

[ExecuteInEditMode]
public class IsometricCamera : MonoBehaviour
{
    [Header("Isometric View Settings")]
    public Transform target; // 카메라가 따라갈 타겟
    public float cameraHeight = 10f; // 카메라 높이
    public float cameraDistance = 10f; // 카메라 거리
    public float cameraAngle = 30f; // 카메라 각도

    private CinemachineCamera cinemachineCamera;
    private CinemachineOrbitalFollow orbitalFollow;

    void Start()
    {
        // Cinemachine Camera가 없으면 추가
        cinemachineCamera = GetComponent<CinemachineCamera>();
        if (cinemachineCamera == null)
        {
            cinemachineCamera = gameObject.AddComponent<CinemachineCamera>();
        }

        // CinemachineOrbitalFollow 설정
        orbitalFollow = cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
        if (orbitalFollow == null)
        {
            orbitalFollow = gameObject.AddComponent<CinemachineOrbitalFollow>();
        }

        // 타겟 설정
        if (target != null)
        {
            cinemachineCamera.Follow = target;
        }

        // 카메라 높이, 거리, 각도 설정
        orbitalFollow.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;
        orbitalFollow.Radius = cameraDistance;
        orbitalFollow.VerticalAxis.Value = cameraAngle; // 카메라 각도
    }

    void Update()
    {
        // 카메라 설정 업데이트
        if (orbitalFollow != null)
        {
            orbitalFollow.Radius = cameraDistance;
            orbitalFollow.VerticalAxis.Value = cameraAngle;
            orbitalFollow.TargetOffset = new Vector3(0, cameraHeight, 0);
        }
    }
}