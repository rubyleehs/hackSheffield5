using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text score;
    public Text sprint;

    public PlayerController player;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        score.text = "Score: " + player.score;
        sprint.text = "Sprint: " + player.sprintCount;
    }
}
