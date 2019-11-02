using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public Transform squareTrasform;

    private Vector3 playerInput;
    private new Transform transform;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        transform = this.GetComponent<Transform>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        transform.position += playerInput * moveSpeed * Time.deltaTime;
        spriteRenderer.color = new Color(transform.position.x, transform.position.y, 50);
        squareTrasform.GetComponent<SpriteRenderer>().color = new Color(transform.position.x, transform.position.y, 50);
    }
}
