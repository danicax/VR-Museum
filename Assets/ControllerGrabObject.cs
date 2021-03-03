using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;
using System;

public class ControllerGrabObject : MonoBehaviour {

    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean grabAction;

    private GameObject collidingObject; // 1
    private GameObject objectInHand; // 2

    private Vector3 cameraPos;

    public SteamVR_Action_Vibration hapticAction;

    private void SetCollidingObject(Collider col)
    {
        // 1
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        // 2
       // col.gameObject.GetComponent<AudioSource>().Play();
        collidingObject = col.gameObject;
       

        
    }

    private void Pulse()
    {
        hapticAction.Execute(0, 300, 320, 1, handType);
    }

    // 1
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // 2
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // 3
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void GrabObject()
    {
        // 1
        objectInHand = collidingObject;
        if ( objectInHand != null)
        {
            Pulse();
        }
        //if (checkObject())
        //    return;
        collidingObject = null;
        // 2
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
        Debug.Log("Grab");
        
    }

    private bool checkObject()
    {
        
        if (objectInHand.tag.Equals("Plane")){
            //ReleaseObject();
            collidingObject = null;
            objectInHand = null;
            //objectInHand = null;

            SceneManager.LoadScene("PlaneScene", LoadSceneMode.Additive);
            SceneManager.UnloadScene("MuseumScene");
      
            cameraPos = GameObject.FindGameObjectWithTag("MainCamera").transform.position;

            return true;

        }

        
        if (objectInHand.tag.Equals("Rocket")){
            //ReleaseObject();
            collidingObject = null;
            objectInHand = null;
            //objectInHand = null;
      
            SceneManager.LoadScene("RocketScene", LoadSceneMode.Additive);
            SceneManager.UnloadScene("MuseumScene");

            cameraPos = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
     
            return true;

        }
        

        if (objectInHand.tag.Equals("Trophy"))
        {
            //ReleaseObject();
            collidingObject = null;
            objectInHand = null;
            //objectInHand = null;
            //    SceneManager.LoadScene("planescene");
            SceneManager.LoadScene("MuseumScene", LoadSceneMode.Additive);
            SceneManager.UnloadScene("PlaneScene");
            GameObject.FindGameObjectWithTag("MainCamera").transform.position = new Vector3(-6, 4, 11);
          //  Debug.Log("camera pos 2: " + GameObject.FindGameObjectWithTag("MainCamera").transform.position);
            return true;

        }

        if (objectInHand.tag.Equals("Trophy2"))
        {
            //ReleaseObject();
            collidingObject = null;
            objectInHand = null;
            //objectInHand = null;
            //    SceneManager.LoadScene("planescene");
            SceneManager.LoadScene("MuseumScene", LoadSceneMode.Additive);
            SceneManager.UnloadScene("RocketScene");
            GameObject.FindGameObjectWithTag("MainCamera").transform.position = new Vector3(-6, 4, 11);
         //   Debug.Log("camera pos 2: " + GameObject.FindGameObjectWithTag("MainCamera").transform.position);
            return true;

        }

        return false;
        /*
        switch (col.tag)
        {
            case "Plane":
                //col.gameObject.GetComponent<AudioSource>().Play();
                //collidingObject = col.gameObject;
                SceneManager.LoadScene("planescene");
                break;
        }
        */
    }

    // 3
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        // 1
        if (GetComponent<FixedJoint>())
        {
            // 2
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // 3
            objectInHand.GetComponent<Rigidbody>().velocity = controllerPose.GetVelocity();
            objectInHand.GetComponent<Rigidbody>().angularVelocity = controllerPose.GetAngularVelocity();

        }
        // 4
        if (checkObject())
            return;
        objectInHand = null;
    }

    // Update is called once per frame
    void Update () {
        Debug.Log("potato");
        if (grabAction.GetLastStateDown(handType))
        {
            Console.WriteLine("down = true");
            if (collidingObject)
            {
                Console.WriteLine("collidingObject = true");
                GrabObject();
            }
        }

        // 2
        if (grabAction.GetLastStateUp(handType))
        {
            Console.WriteLine("up = true");
            if (objectInHand)
            {
                Console.WriteLine("objectInHand = true");
                ReleaseObject();
            }
        }
    }
}
