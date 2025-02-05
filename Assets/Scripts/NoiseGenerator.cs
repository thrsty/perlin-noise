using UnityEngine;
using System;
using System.Collections;

using Vec2 = UnityEngine.Vector2;
using Vec3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;

public class NoiseGenerator : MonoBehaviour {

    public int mapWidth;
    public int mapHeight;
    public int squareCount;
    public float scale;
    public float changeRate;
    public float waitTime;

    float[,] noiseMap;

    // grid is squareCount x squareCount squares
    Vec2 [,] gradVecGrid;

    float xCof;
    float yCof;
    bool init = false;

    float fade(float x) {
        return ((6 * x - 15) * x + 10) * x * x * x;
    }

    private void OnDrawGizmosSelected() {

        Gizmos.color = Color.white;

        for (int i = 0; i < squareCount + 1; i++) {
            for (int j = 0; j < squareCount + 1; j++) {
                Vec2 pos = new Vec2 (-20 + i * 40 / squareCount, -20 + j * 40 / squareCount);
                Gizmos.DrawLine(new Vec3(pos.x, 0, pos.y), new Vec3(pos.x + gradVecGrid[i, j].x, 0, pos.y + gradVecGrid[i, j].y));
            }
        }
    }

    float noiseValue (float x, float y) {

        float sampleX = x * xCof;
        float sampleY = y * yCof;
        int xFloor = (int)Math.Round(Math.Floor(sampleX) / (double)scale, 0);
        int yFloor = (int)Math.Round(Math.Floor(sampleY) / (double)scale, 0);
        
        //Debug.Log("grid candidate point is within : " + xFloor + " " + (xFloor + 1) + "\n " + yFloor + " " + (yFloor + 1));

        //Debug.Log("xFloor : " + xFloor + ", yFloor : " + yFloor);
        //Debug.Log("x : " + x + ", y : " + y);

        // dot product of top left, bottom left, top right, bottom right random vectors
        Vec2 bottomLeft = gradVecGrid[xFloor, yFloor];
        Vec2 topLeft = gradVecGrid[xFloor, yFloor + 1];
        Vec2 bottomRight = gradVecGrid[xFloor + 1, yFloor];
        Vec2 topRight = gradVecGrid[xFloor + 1, yFloor + 1];

        float dotBottomLeft = Vec2.Dot(new Vec2(sampleX % 1, sampleY % 1), bottomLeft);
        float dotTopLeft = Vec2.Dot(new Vec2(sampleX % 1, 1 - sampleY % 1), topLeft);
        float dotBottomRight = Vec2.Dot(new Vec2(1 - sampleX % 1, sampleY % 1), bottomRight);
        float dotTopRight = Vec2.Dot(new Vec2(1 - sampleX % 1, 1 - sampleY % 1), topRight);

        float u = fade(sampleX % 1);
        float v = fade(sampleY % 1);

        if (u < 0 || v < 0 || u > 1 || v > 1) {
            Debug.Log("u : " + u + ", v : " + v);
        }

        float value = Mathf.Lerp(Mathf.Lerp(dotBottomLeft, dotTopLeft, v), Mathf.Lerp(dotBottomRight, dotTopRight, v), u);

        if (value > 1 || value < -1) {
            Debug.Log("value : " + value);
            value = MathF.Round(value, 0);
        }

        return value;
    }

    void initCanvas() {

        // map mapWidth and mapHeight to a value in the grids squareCount x squareCount dimensions
        xCof = scale * (float)squareCount / (float)mapWidth;
        yCof = scale * (float)squareCount / (float)mapHeight;

        // xCof = mapWidth / squareCount;
        // yCof = mapHeight / squareCount;

        //Debug.Log("xCof : " + xCof + ", yCof : " + yCof);
        
        noiseMap = new float [mapWidth, mapHeight];
        gradVecGrid = new Vec2 [squareCount + 1, squareCount + 1];
        
        // scale check
        if (scale <= 0) {
            scale = 0.0001f;
        }

        // make random gradient vectors
        for (int i = 0; i < squareCount + 1; i++) {
            for (int j = 0; j < squareCount + 1; j++) {
                gradVecGrid[i, j] = Random.insideUnitCircle.normalized;
            }
        }
    }

    float[,] generateNoiseMap () {

        if (!init) {
            initCanvas();
            init = true;
        }

        // fill in map
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                noiseMap[x, y] = noiseValue(x, y);
                //Debug.Log("x : " + x + ", y : " + y + ", value : " + noiseMap[x, y]);
            }
        }

        StartCoroutine(changeAngle());

        return noiseMap;
    }

    IEnumerator changeAngle() {
        // change angle barely
        for (int i = 0; i < squareCount + 1; i++) {
            for (int j = 0; j < squareCount + 1; j++) {
                Vec3 rot = new Vec3 (gradVecGrid[i, j].x, 0, gradVecGrid[i, j].y);
                float ran = Random.Range(0.01f, 1f);
                Debug.Log(ran);
                rot = Quaternion.AngleAxis(changeRate, Vec3.up) * rot;
                rot.Normalize();
                gradVecGrid[i,j] = new Vec2 (rot.x, rot.z);
            }
        }

        yield return new WaitForSeconds(waitTime);
    }

    public void generate() {
        float[,] noiseMap = generateNoiseMap();

        MapDisplay display = FindFirstObjectByType<MapDisplay>();
        display.drawNoiseMap(noiseMap);
    }

    void Update() {
        generate();
    }
}
