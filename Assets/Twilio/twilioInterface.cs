﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class twilioInterface : MonoBehaviour {
    public string status;
    string to;
    string from;

    public bool hangUp;

    public bool getInput;
    public string DTMF;
    public int inputLength = 1;
    public int rippled;

    public RippleEffect ripple;

    WebSocket ws;

    void getDTMF (int digits) {
        ws.Send("get-input," + digits);
    }

    void endCall () {
        ws.Send("end-call");
    }

    // Start is called before the first frame update
    void Start() {
        ws = new WebSocket ("ws://localhost:1337");
        ws.OnMessage += (sender, e) => {
            Debug.Log(e.Data);
            string[] parameters = e.Data.Split(',');
            if (parameters[0] == "ringing" || parameters[0] == "in-progress" || parameters[0] == "failed" || parameters[0] == "completed") {
                status = parameters[0];
                to = parameters[1];
                from = parameters[2];
            } else {
                DTMF = parameters[0];
            }
            };
        ws.Connect ();
        
        rippled = 80;
    }

    // Update is called once per frame
    void Update() {
        if (status == "ringing" || status == "in-progress") {
            GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            if (rippled >= 80) {
                ripple.AddRipple(transform.position, 1f, 0.15f);
                rippled = 0;
            }
            rippled += 1;
        } else {
            GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            rippled = 80;
        }

        if (getInput) {
            getDTMF(inputLength);
            getInput = false;
        }

        if (hangUp) {
            endCall();
            hangUp = false;
        }
    }
}
