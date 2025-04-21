using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;

    private Renderer[] renderers; // ���� ������ �迭
    private Collider[] colliders; // ���� �ݶ��̴� �迭
    private Vector3[] cubePositions; // ���� ť�� ��ġ �迭

    public void Initialize(TetrisBlockData blockData, Material material)
    {
        cubePositions = blockData.cubePositions;
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

    public void Rotate(Vector3 axis)
    {
        // 90���� ȸ��
        transform.Rotate(axis * 90f, Space.World);
    }
}
