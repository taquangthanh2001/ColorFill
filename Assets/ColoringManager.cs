using UnityEngine;
using UnityEngine.UI;

public class ColoringManager : MonoBehaviour
{
    [Header("UI References")]
    public RawImage drawingImage;
    public Texture2D sourceTexture;

    [Header("Color")]
    public Color currentColor = Color.red;

    private Texture2D editableTexture;

    void Start()
    {
        // Tạo bản sao texture để vẽ
        editableTexture = Instantiate(sourceTexture);
        editableTexture.Apply();

        drawingImage.texture = editableTexture;
    }

    public void SetCurrentColor(Color color)
    {
        currentColor = color;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 localPos;
            RectTransform rectTransform = drawingImage.rectTransform;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                Input.mousePosition,
                null,
                out localPos))
            {
                // Chuyển từ localPos sang UV (0–1)
                float pivotX = rectTransform.pivot.x;
                float pivotY = rectTransform.pivot.y;

                float width = rectTransform.rect.width;
                float height = rectTransform.rect.height;

                float xNormalized = (localPos.x / width) + pivotX;
                float yNormalized = (localPos.y / height) + pivotY;

                int texX = Mathf.FloorToInt(xNormalized * editableTexture.width);
                int texY = Mathf.FloorToInt(yNormalized * editableTexture.height);

                // Kiểm tra trong biên
                if (texX >= 0 && texX < editableTexture.width && texY >= 0 && texY < editableTexture.height)
                {
                    Color targetColor = editableTexture.GetPixel(texX, texY);
                    FloodFill(texX, texY, targetColor, currentColor);
                    editableTexture.Apply();
                }
            }
        }
    }

    void FloodFill(int x, int y, Color targetColor, Color newColor)
    {
        if (!IsSimilarColor(targetColor, Color.white)) return;
        if (IsSimilarColor(targetColor, newColor)) return;

        FloodFillRecursive(x, y, targetColor, newColor);
    }

    void FloodFillRecursive(int x, int y, Color targetColor, Color newColor)
    {
        if (x < 0 || x >= editableTexture.width || y < 0 || y >= editableTexture.height) return;
        if (!IsSimilarColor(editableTexture.GetPixel(x, y), targetColor)) return;

        editableTexture.SetPixel(x, y, newColor);

        FloodFillRecursive(x + 1, y, targetColor, newColor);
        FloodFillRecursive(x - 1, y, targetColor, newColor);
        FloodFillRecursive(x, y + 1, targetColor, newColor);
        FloodFillRecursive(x, y - 1, targetColor, newColor);
    }

    bool IsSimilarColor(Color a, Color b, float tolerance = 0.1f)
    {
        return Vector4.Distance(a, b) < tolerance;
    }
}
