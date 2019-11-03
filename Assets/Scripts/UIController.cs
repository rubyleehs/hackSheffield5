using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text score;
    public Image sprint;

    public PlayerController player;
    // Update is called once per frame
    void Update() {
        score.text = "Score: " + player.score;

        var sprintRectTransform = sprint.transform as RectTransform;
          sprintRectTransform.sizeDelta = new Vector2 (player.sprintCount, sprintRectTransform.sizeDelta.y);
    }
}