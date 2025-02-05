using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor
{
    bool vecTog;
    bool drawVecs;

    public override void OnInspectorGUI() {
        NoiseGenerator noiseGen = (NoiseGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate!")) {
            noiseGen.generate();
        }
    }
}
