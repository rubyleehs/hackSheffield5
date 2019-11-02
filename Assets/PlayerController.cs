using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public Transform squareTransform;
    
    private Vector3 mousePosition;
    private Vector3 objectPosition;
    private float angle;
    private int sprintCount;
    private Vector3 playerInput;
    private Vector3 maxPlayerInput;
    private new Transform transform;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
        sprintCount = 100;
        transform = this.GetComponent<Transform>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        //sprint
        if (Input.GetKey(KeyCode.LeftShift) && (sprintCount <= 100 && sprintCount > 0)){
            moveSpeed = 10;
            transform.position += playerInput * moveSpeed * Time.deltaTime;
            sprintCount -= 1;
        }
        else if (sprintCount < 100 && !Input.GetKey(KeyCode.LeftShift)){ 
            sprintCount += 1;
            moveSpeed = 5;
            transform.position += playerInput * moveSpeed * Time.deltaTime;
        }
        else{
            moveSpeed = 5;
            transform.position += playerInput * moveSpeed * Time.deltaTime;
            }

        //rotation
        mousePosition = Input.mousePosition;
        objectPosition = Camera.main.WorldToScreenPoint(transform.position);

        mousePosition.x = mousePosition.x - objectPosition.x;
        mousePosition.y = mousePosition.y - objectPosition.y;
 
        angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));


        transform.position += playerInput * moveSpeed * Time.deltaTime;
        spriteRenderer.color = new Color(transform.position.x, transform.position.y, 50);
        squareTransform.GetComponent<SpriteRenderer>().color = new Color(transform.position.x, transform.position.y, 50);
        

    }
}
