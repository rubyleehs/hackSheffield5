using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float deltaTime;
    public float timeScale;

    private void Update()
    {
        deltaTime = Time.deltaTime * timeScale;
    }
}
