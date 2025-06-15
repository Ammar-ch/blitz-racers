using UnityEngine;
using System.IO;

public class RenderTextureExporter : MonoBehaviour
{
    public RenderTexture renderTexture;

    // وظيفة لحفظ Render Texture كصورة PNG
    [ContextMenu("Save Render Texture to PNG")]
    public void SaveRenderTextureToPNG()
    {
        if (renderTexture == null)
        {
            Debug.LogError("Render Texture is missing!");
            return;
        }

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        RenderTexture.active = currentRT;

        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath, "RoadMask.png");
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Image saved to: " + filePath);
    }
}
