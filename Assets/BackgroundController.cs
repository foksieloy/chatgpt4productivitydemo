using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private Texture2D gradientTexture;

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        gradientTexture = meshRenderer.material.mainTexture as Texture2D;
    }

    public Color GetColorAtWorldPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
        Vector2 textureCoordinates = new Vector2(localPosition.x / transform.localScale.x, localPosition.y / transform.localScale.y);
        textureCoordinates += Vector2.one / 2; // Normalize coordinates to 0..1 range
        return gradientTexture.GetPixelBilinear(textureCoordinates.x, textureCoordinates.y);
    }
}
