using UnityEngine;
using UnityEngine.UI;

public class GridPlacer : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject ghostPrefab; // ������ ť�� ������
    public float gridSize = 1f;
    public Button placeButton; // UI ��ư
    public TileGridGenerator gridDrawer; // GridLinesDrawer ����

    private GameObject ghostInstance;
    private bool isPlacing = false;
    private Renderer ghostRenderer;

    void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ�� �Լ� ����
        placeButton.onClick.AddListener(StartPlacing);
    }

    void Update()
    {
        if (isPlacing && ghostInstance != null)
        {
            UpdateGhostPosition();

            if (Input.GetMouseButtonDown(0))
            {
                PlaceCube();
            }
        }
    }

    void StartPlacing()
    {
        if (ghostInstance == null)
        {
            ghostInstance = Instantiate(ghostPrefab);
            ghostRenderer = ghostInstance.GetComponent<Renderer>();
        }
        isPlacing = true;
    }

    private bool IsOverlapping(Vector3 position)
    {
        // ��ħ ���� Ȯ��
        Collider[] colliders = Physics.OverlapBox(
            position,
            ghostInstance.transform.localScale / 2,
            Quaternion.identity
        );

        foreach (Collider collider in colliders)
        {
            // ��ġ�� ť���� ��ġ�� �׸��� ������ ��ȯ
            Vector3 colliderPosition = collider.transform.position;
            int colliderGridX = Mathf.FloorToInt(colliderPosition.x / gridSize);
            int colliderGridY = Mathf.FloorToInt(colliderPosition.y / gridSize);
            int colliderGridZ = Mathf.FloorToInt(colliderPosition.z / gridSize);

            // ������ �׸��� ��ġ�� �ִ��� Ȯ��
            int gridX = Mathf.FloorToInt(position.x / gridSize);
            int gridY = Mathf.FloorToInt(position.y / gridSize);
            int gridZ = Mathf.FloorToInt(position.z / gridSize);

            if (gridX == colliderGridX && gridY == colliderGridY && gridZ == colliderGridZ)
            {
                return true;
            }
        }

        return false;
    }

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Ray�� �ε��� ��ġ�� �������� ��Ʈ ť�� ��ġ  
            Vector3 hitPosition = hit.point;

            // �׸��� ���� ����  
            int gridX = Mathf.FloorToInt(hitPosition.x / gridSize);
            int gridZ = Mathf.FloorToInt(hitPosition.z / gridSize);
            int gridY = Mathf.FloorToInt(hitPosition.y / gridSize);

            Vector3 snapped = new Vector3(
                gridX * gridSize + gridSize / 2, // X�� ����  
                (gridY + 1) * gridSize,          // �ε��� ť�� �ٷ� ���� ��ġ  
                gridZ * gridSize + gridSize / 2  // Z�� ����  
            );

            ghostInstance.transform.position = snapped;

            // ��ħ ���� Ȯ�� �� ���� ����
            if (IsOverlapping(ghostInstance.transform.position))
            {
                ghostRenderer.material.color = new Color(1, 0, 0, 0.5f); // ������ (������)
            }
            else
            {
                ghostRenderer.material.color = new Color(0, 1, 0, 0.5f); // �ʷϻ� (������)
            }
        }
        else
        {
            // Ray�� �ƹ��͵� �ε����� ���� ���, �⺻ ��� ���� ��Ʈ ť�� ��ġ  
            Plane plane = new Plane(Vector3.up, Vector3.zero); // y=0 ���  
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);

                // �׸��� ���� ����  
                int gridX = Mathf.FloorToInt(point.x / gridSize);
                int gridZ = Mathf.FloorToInt(point.z / gridSize);

                Vector3 snapped = new Vector3(
                    gridX * gridSize + gridSize / 2, // X�� ����  
                    0,                        // �⺻ Y�� ������ 1�� ����  
                    gridZ * gridSize + gridSize / 2  // Z�� ����  
                );

                ghostInstance.transform.position = snapped;

                // ��ħ ���� Ȯ�� �� ���� ����
                if (IsOverlapping(ghostInstance.transform.position))
                {
                    ghostRenderer.material.color = new Color(1, 0, 0, 0.5f); // ������ (������)
                }
                else
                {
                    ghostRenderer.material.color = new Color(0, 1, 0, 0.5f); // �ʷϻ� (������)
                }
            }
        }
    }

    void PlaceCube()
    {
        Vector3 position = ghostInstance.transform.position;

        // �׸��� ���� ����
        int gridX = Mathf.FloorToInt(position.x / gridSize);
        int gridY = Mathf.FloorToInt(position.y / gridSize);
        int gridZ = Mathf.FloorToInt(position.z / gridSize);

        // �׸��� ���� ������ Ȯ��
        if (gridX >= 0 && gridX < gridDrawer.width && gridZ >= 0 && gridZ < gridDrawer.height)
        {
            if (IsOverlapping(position))
            {
                // ��ġ�� ��� ��ġ���� ����
                Debug.Log("Cannot place cube: position is already occupied.");
                return;
            }

            // ť�� ��ġ
            Instantiate(cellPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.Log("Cannot place cube: position is out of grid bounds.");
        }
    }
}
