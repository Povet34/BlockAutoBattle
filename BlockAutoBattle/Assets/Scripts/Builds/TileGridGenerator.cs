using UnityEngine;

[ExecuteInEditMode]
public class TileGridGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public Color lineColor = Color.gray;

    public GameObject viewTarget; // 그리드 중심으로 이동할 오브젝트
    private Material lineMaterial;

    // 2차원 배열: 각 위치의 층 높이를 저장
    public int[,] gridHeights;

    public float yOffset = -0.5f;
    public float xOffset = 0.5f; // X축 오프셋
    public float zOffset = 0.5f; // Z축 오프셋

    private void Update()
    {
        UpdateViewTargetPosition();
    }

    void Awake()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        gridHeights = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridHeights[x, z] = 0; // 초기화: 모든 위치의 층 높이는 0
            }
        }
    }

    void UpdateViewTargetPosition()
    {
        if (viewTarget != null)
        {
            // 그리드 중심 계산
            float centerX = (width * cellSize) / 2 + xOffset;
            float centerZ = (height * cellSize) / 2 + zOffset;
            float centerY = yOffset;

            // viewTarget 위치 설정
            viewTarget.transform.position = new Vector3(centerX, centerY, centerZ);
        }
    }

    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    void OnRenderObject()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        // Draw grid lines
        for (int x = 0; x <= width; x++)
        {
            float xPos = x * cellSize + xOffset; // X축 오프셋 적용
            GL.Vertex3(xPos, yOffset, zOffset);
            GL.Vertex3(xPos, yOffset, height * cellSize + zOffset);
        }

        for (int z = 0; z <= height; z++)
        {
            float zPos = z * cellSize + zOffset; // Z축 오프셋 적용
            GL.Vertex3(xOffset, yOffset, zPos);
            GL.Vertex3(width * cellSize + xOffset, yOffset, zPos);
        }

        GL.End();
        GL.PopMatrix();
    }

    // 에디터에서 값이 변경될 때 viewTarget 위치를 업데이트
    void OnValidate()
    {
        UpdateViewTargetPosition();
    }
}
