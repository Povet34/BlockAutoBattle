using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;

    private Renderer[] renderers; // ���� ������ �迭
    private Collider[] colliders; // ���� �ݶ��̴� �迭
    private Vector3[] cubePositions; // ���� ť�� ��ġ �迭
    private TetrisBlockData blockData; // �ν��Ͻ�ȭ�� TetrisBlockData

    public void Initialize(TetrisBlockData originalBlockData, Material material)
    {
        // TetrisBlockData �ν��Ͻ�ȭ (Instantiate�� ����Ͽ� ����)
        blockData = Instantiate(originalBlockData);

        // ��� �������� ť�� ��ġ�� �߽��� ��������
        Vector3 center = blockData.center;
        cubePositions = new Vector3[blockData.cubePositions.Length];

        // ť�� ��ġ�� center�� �������� ������
        for (int i = 0; i < blockData.cubePositions.Length; i++)
        {
            cubePositions[i] = blockData.cubePositions[i];
        }

        renderers = new Renderer[cubePositions.Length];
        colliders = new Collider[cubePositions.Length];

        // ť�� ����
        for (int i = 0; i < cubePositions.Length; i++)
        {
            GameObject cube = Instantiate(cubePrefab, transform);
            cube.transform.localPosition = cubePositions[i];

            // ������ ����
            Renderer renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = material;
            }
            renderers[i] = renderer;

            // �ݶ��̴� ����
            Collider collider = cube.GetComponent<Collider>();
            if (collider != null)
            {
                colliders[i] = collider;
            }
        }

        // �� ���� ���� (blockData�� ���� ���)
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
        // ��� ȸ��
        transform.Rotate(axis, 90f, Space.World);

        // ť���� offset ������ ������Ʈ
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
