using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int score;

    public float walkSpeed;
    public float sprintSpeed;
    public int maxSprint;
    public int sprintCount ;

    public GameObject phone;
    public bool holdingPhone;
    public float phoneGrabRange;
    public float phoneOffsetX;
    public float phoneOffsetY;

    private Vector3 mousePosition;
    private Vector3 objectPosition;
    private float angle;

    private Vector3 playerInput;
    private Vector3 maxPlayerInput;

    void OnCollisionEnter2D(Collision2D collision) {
        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "Coin") {
            Debug.Log("Collected coin");
            Destroy(collision.gameObject);
            score++;
        }
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        //sprint
        if (Input.GetKey(KeyCode.LeftShift) && sprintCount > 0){
            transform.position += playerInput * sprintSpeed * Time.deltaTime;
            sprintCount -= 1;
        }
        else if (sprintCount < maxSprint && !Input.GetKey(KeyCode.LeftShift)){ 
            sprintCount += 2;
            transform.position += playerInput * walkSpeed * Time.deltaTime;
        }
        else {
            transform.position += playerInput * walkSpeed * Time.deltaTime;
        }

        if (Input.GetKeyDown("f")) {
            if (phone.transform.parent != null) {
                phone.transform.parent = null;
                holdingPhone = false;
            } else {
                if (Vector3.Distance(phone.transform.position, transform.position) < phoneGrabRange) {
                    phone.transform.parent = transform;
                    holdingPhone = true;
                }
            }
        }

        if (holdingPhone) {
            phone.transform.localPosition = Vector3.Lerp(phone.transform.localPosition, new Vector3 (phoneOffsetX, phoneOffsetY, 0), 0.2f);
        }

        //rotation
        mousePosition = Input.mousePosition;
        objectPosition = Camera.main.WorldToScreenPoint(transform.position);

        mousePosition.x = mousePosition.x - objectPosition.x;
        mousePosition.y = mousePosition.y - objectPosition.y;
 
        angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //GetComponent<SpriteRenderer>().color = new Color(transform.position.x, transform.position.y, 50);
    }
}
