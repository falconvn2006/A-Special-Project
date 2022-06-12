using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text fpsText;

    private int lastFrameIndex;

    private float[] frameDeltaTimeArr;

    void Awake(){
        frameDeltaTimeArr = new float[50];
    }

    // Update is called once per frame
    void Update()
    {
        frameDeltaTimeArr[lastFrameIndex] = Time.deltaTime;
        lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArr.Length;

        fpsText.text = "FPS : " + Mathf.RoundToInt(CalculateFPS());
    }

    float CalculateFPS(){
        float total = 0f;
        foreach(float deltaTime in frameDeltaTimeArr){
            total += deltaTime;
        }
        return frameDeltaTimeArr.Length / total;
    }
}
