using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    public void drawNoiseMap(float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        //Debug.Log(width);
        //Debug.Log(height);

        Texture2D texture = new Texture2D(width, height);
        Color[] colourMap = new Color[width * height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                colourMap[y * width + x] = new Color(noiseMap[x, y], noiseMap[x, y], noiseMap[x, y]);//Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        texture.SetPixels(colourMap);


        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                //Debug.Log(" x : " + x + ", y : " + y + ", value : " + texture.GetPixel(x, y)); 
            }
        }
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;
        textureRenderer.sharedMaterial.mainTexture = texture;
        //textureRenderer.transform.localScale = new Vector3(width, 1, height);

    }
}
