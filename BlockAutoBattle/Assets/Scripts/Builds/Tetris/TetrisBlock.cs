using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;

    private Renderer[] renderers; // 블럭의 렌더러 배열
    private Collider[] colliders; // 블럭의 콜라이더 배열
    private Vector3[] cubePositions; // 블럭의 큐브 위치 배열
    private TetrisBlockData blockData; // 인스턴스화된 TetrisBlockData

    public void Initialize(TetrisBlockData originalBlockData, Material material)
    {
        // TetrisBlockData 인스턴스화 (Instantiate를 사용하여 복제)
        blockData = Instantiate(originalBlockData);

        // 블록 데이터의 큐브 위치와 중심점 가져오기
        Vector3 center = blockData.center;
        cubePositions = new Vector3[blockData.cubePositions.Length];

        // 큐브 위치를 center를 기준으로 재정렬
        for (int i = 0; i < blockData.cubePositions.Length; i++)
        {
            cubePositions[i] = blockData.cubePositions[i];
        }

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

    public void SetCubePositions(Vector3[] positions)
    {
        if (positions.Length == cubePositions.Length)
        {
            for (int i = 0; i < cubePositions.Length; i++)
            {
                cubePositions[i] = positions[i];
                transform.GetChild(i).localPosition = positions[i];
            }
        }
        else
        {
            Debug.LogError("Positions array length does not match cubePositions length.");
        }
    }

    public void Rotate(Vector3 axis)
    {
        // 블록 회전
        transform.Rotate(axis, 90f, Space.World);

        // 큐브의 offset 데이터 업데이트
        Quaternion rotation = Quaternion.Euler(axis * 90f);
        for (int i = 0; i < cubePositions.Length; i++)
        {
            cubePositions[i] = rotation * cubePositions[i];
        }
    }

    public Vector3[] GetCubePositions()
    {
        return cubePositions;
    }

    public TetrisBlockData GetBlockData()
    {
        return blockData;
    }
}
