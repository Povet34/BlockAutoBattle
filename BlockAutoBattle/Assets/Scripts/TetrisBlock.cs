using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;

    private Renderer[] renderers; // 블럭의 렌더러 배열
    private Collider[] colliders; // 블럭의 콜라이더 배열
    private Vector3[] cubePositions; // 블럭의 큐브 위치 배열

    public void Initialize(TetrisBlockData blockData, Material material)
    {
        cubePositions = blockData.cubePositions;
        renderers = new Renderer[cubePositions.Length];
        colliders = new Collider[cubePositions.Length];

        // 큐브 생성
        for (int i = 0; i < cubePositions.Length; i++)
        {
            GameObject cube = Instantiate(cubePrefab, transform);
            cube.transform.localPosition = cubePositions[i];

            // 렌더러 설정
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = material;
            }
            renderers[i] = renderer;

            // 콜라이더 설정
            Collider collider = cube.GetComponent<Collider>();
            if (collider != null)
            {
                colliders[i] = collider;
            }
        }

        // 블럭 색상 설정 (blockData의 색상 사용)
        SetColor(blockData.blockColor);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetColor(Color color)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    public void EnableColliders(bool enable)
    {
        foreach (Collider collider in colliders)
        {
            if (collider != null)
            {
                collider.enabled = enable;
            }
        }
    }

    public void Rotate(Vector3 axis)
    {
        // 90도씩 회전
        transform.Rotate(axis * 90f, Space.World);
    }
}
