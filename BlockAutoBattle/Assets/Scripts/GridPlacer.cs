using UnityEngine;
using UnityEngine.UI;

public class GridPlacer : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject ghostPrefab; // 반투명 큐브 프리팹
    public float gridSize = 1f;
    public Button placeButton; // UI 버튼
    public TileGridGenerator gridDrawer; // GridLinesDrawer 참조

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

    private bool IsOverlapping(Vector3 position)
    {
        // 겹침 여부 확인
        Collider[] colliders = Physics.OverlapBox(
            position,
            ghostInstance.transform.localScale / 2,
            Quaternion.identity
        );

        foreach (Collider collider in colliders)
        {
            // 배치된 큐브의 위치를 그리드 단위로 변환
            Vector3 colliderPosition = collider.transform.position;
            int colliderGridX = Mathf.FloorToInt(colliderPosition.x / gridSize);
            int colliderGridY = Mathf.FloorToInt(colliderPosition.y / gridSize);
            int colliderGridZ = Mathf.FloorToInt(colliderPosition.z / gridSize);

            // 동일한 그리드 위치에 있는지 확인
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
            // Ray가 부딪힌 위치를 기준으로 고스트 큐브 배치  
            Vector3 hitPosition = hit.point;

            // 그리드 스냅 적용  
            int gridX = Mathf.FloorToInt(hitPosition.x / gridSize);
            int gridZ = Mathf.FloorToInt(hitPosition.z / gridSize);
            int gridY = Mathf.FloorToInt(hitPosition.y / gridSize);

            Vector3 snapped = new Vector3(
                gridX * gridSize + gridSize / 2, // X축 스냅  
                (gridY + 1) * gridSize,          // 부딪힌 큐브 바로 위로 배치  
                gridZ * gridSize + gridSize / 2  // Z축 스냅  
            );

            ghostInstance.transform.position = snapped;

            // 겹침 여부 확인 및 색상 변경
            if (IsOverlapping(ghostInstance.transform.position))
            {
                ghostRenderer.material.color = new Color(1, 0, 0, 0.5f); // 빨간색 (반투명)
            }
            else
            {
                ghostRenderer.material.color = new Color(0, 1, 0, 0.5f); // 초록색 (반투명)
            }
        }
        else
        {
            // Ray가 아무것도 부딪히지 않은 경우, 기본 평면 위에 고스트 큐브 배치  
            Plane plane = new Plane(Vector3.up, Vector3.zero); // y=0 평면  
            if (plane.Raycast(ray, out float distance))
            {
                Vector3 point = ray.GetPoint(distance);

                // 그리드 스냅 적용  
                int gridX = Mathf.FloorToInt(point.x / gridSize);
                int gridZ = Mathf.FloorToInt(point.z / gridSize);

                Vector3 snapped = new Vector3(
                    gridX * gridSize + gridSize / 2, // X축 스냅  
                    0,                        // 기본 Y축 간격을 1로 설정  
                    gridZ * gridSize + gridSize / 2  // Z축 스냅  
                );

                ghostInstance.transform.position = snapped;

                // 겹침 여부 확인 및 색상 변경
                if (IsOverlapping(ghostInstance.transform.position))
                {
                    ghostRenderer.material.color = new Color(1, 0, 0, 0.5f); // 빨간색 (반투명)
                }
                else
                {
                    ghostRenderer.material.color = new Color(0, 1, 0, 0.5f); // 초록색 (반투명)
                }
            }
        }
    }

    void PlaceCube()
    {
        Vector3 position = ghostInstance.transform.position;

        // 그리드 스냅 적용
        int gridX = Mathf.FloorToInt(position.x / gridSize);
        int gridY = Mathf.FloorToInt(position.y / gridSize);
        int gridZ = Mathf.FloorToInt(position.z / gridSize);

        // 그리드 범위 내인지 확인
        if (gridX >= 0 && gridX < gridDrawer.width && gridZ >= 0 && gridZ < gridDrawer.height)
        {
            if (IsOverlapping(position))
            {
                // 겹치는 경우 배치하지 않음
                Debug.Log("Cannot place cube: position is already occupied.");
                return;
            }

            // 큐브 배치
            Instantiate(cellPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.Log("Cannot place cube: position is out of grid bounds.");
        }
    }
}
