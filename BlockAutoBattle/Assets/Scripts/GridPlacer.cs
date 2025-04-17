using UnityEngine;
using UnityEngine.UI;

public class GridPlacer : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject ghostPrefab; // 반투명 큐브 프리팹
    public float gridSize = 1f;
    public Button placeButton; // UI 버튼
    public GridLinesDrawer gridDrawer; // GridLinesDrawer 참조

    private GameObject ghostInstance;
    private bool isPlacing = false;
    private Renderer ghostRenderer;

    void Start()
    {
        // 버튼 클릭 이벤트에 함수 연결
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
        Plane plane = new Plane(Vector3.up, Vector3.zero); // y=0 평면

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            int gridX = Mathf.FloorToInt(point.x / gridSize);
            int gridZ = Mathf.FloorToInt(point.z / gridSize);

            // 그리드 범위 내인지 확인
            if (gridX >= 0 && gridX < gridDrawer.width && gridZ >= 0 && gridZ < gridDrawer.height)
            {
                // 현재 위치의 층 높이를 가져옴
                int currentHeight = gridDrawer.gridHeights[gridX, gridZ];
                Vector3 snapped = new Vector3(
                    gridX * gridSize + gridSize / 2,
                    currentHeight * gridSize,
                    gridZ * gridSize + gridSize / 2
                );

                ghostInstance.transform.position = snapped;

                // 고스트 큐브 색상은 항상 초록색으로 유지
                ghostRenderer.material.color = new Color(0, 1, 0, 0.5f); // 초록색 (반투명)
            }
        }
    }

    void PlaceCube()
    {
        Vector3 position = ghostInstance.transform.position;
        int gridX = Mathf.FloorToInt(position.x / gridSize);
        int gridZ = Mathf.FloorToInt(position.z / gridSize);

        // 그리드 범위 내인지 확인
        if (gridX >= 0 && gridX < gridDrawer.width && gridZ >= 0 && gridZ < gridDrawer.height)
        {
            // 큐브 배치
            Instantiate(cellPrefab, position, Quaternion.identity);

            // 해당 위치의 층 높이 증가
            gridDrawer.gridHeights[gridX, gridZ]++;
        }
    }
}