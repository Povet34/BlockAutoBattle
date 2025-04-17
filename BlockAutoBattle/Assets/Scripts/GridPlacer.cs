using UnityEngine;
using UnityEngine.UI;

public class GridPlacer : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject ghostPrefab; // ������ ť�� ������
    public float gridSize = 1f;
    public Button placeButton; // UI ��ư
    public GridLinesDrawer gridDrawer; // GridLinesDrawer ����

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

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero); // y=0 ���

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            int gridX = Mathf.FloorToInt(point.x / gridSize);
            int gridZ = Mathf.FloorToInt(point.z / gridSize);

            // �׸��� ���� ������ Ȯ��
            if (gridX >= 0 && gridX < gridDrawer.width && gridZ >= 0 && gridZ < gridDrawer.height)
            {
                // ���� ��ġ�� �� ���̸� ������
                int currentHeight = gridDrawer.gridHeights[gridX, gridZ];
                Vector3 snapped = new Vector3(
                    gridX * gridSize + gridSize / 2,
                    currentHeight * gridSize,
                    gridZ * gridSize + gridSize / 2
                );

                ghostInstance.transform.position = snapped;

                // ��Ʈ ť�� ������ �׻� �ʷϻ����� ����
                ghostRenderer.material.color = new Color(0, 1, 0, 0.5f); // �ʷϻ� (������)
            }
        }
    }

    void PlaceCube()
    {
        Vector3 position = ghostInstance.transform.position;
        int gridX = Mathf.FloorToInt(position.x / gridSize);
        int gridZ = Mathf.FloorToInt(position.z / gridSize);

        // �׸��� ���� ������ Ȯ��
        if (gridX >= 0 && gridX < gridDrawer.width && gridZ >= 0 && gridZ < gridDrawer.height)
        {
            // ť�� ��ġ
            Instantiate(cellPrefab, position, Quaternion.identity);

            // �ش� ��ġ�� �� ���� ����
            gridDrawer.gridHeights[gridX, gridZ]++;
        }
    }
}