using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using System.Net;
using System;
using Firebase;
using Firebase.Database;

public class Track : MonoBehaviour
{

    //This is a public serialized field of type Text named valores.It will be exposed in the Unity Inspector for assigning a Text component to it.

    [SerializeField]
    public Text valores;

    //This is a private variable of type XRNode named lxRNode that is initialized with the value XRNode.LeftHand.
    
    private XRNode lxRNode = XRNode.LeftHand;

    //These are private variables. devices and heads are lists of InputDevice objects. device and c_head are InputDevice objects.
    
    private List<InputDevice> devices = new List<InputDevice>(), heads = new List<InputDevice>();
    private InputDevice device;
    private InputDevice c_head;

    //These are private variables. devicePositionChosen is a boolean flag. NowSearching is also a boolean flag initialized as false. devicePositionValue and prevdevicePositionValue are Vector3 objects initialized with default values.
    
    private bool devicePositionChosen, NowSearching = false;
    private Vector3 devicePositionValue = Vector3.zero;
    private Vector3 prevdevicePositionValue;

    //This is a method named Start() which is called once when the script starts executing. It retrieves information about connected webcams, sets up a WebCamTexture and assigns it to the main texture of a renderer component.
    
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log("Number of web cams connected: " + devices.Length);
        Renderer rend = this.GetComponentInChildren<Renderer>();

        WebCamTexture mycam = new WebCamTexture();
        string camName = devices[3].name;
        Debug.Log("The webcam name is " + camName);
        mycam.deviceName = camName;
        rend.material.mainTexture = mycam;

        mycam.Play();
    }

    //This is a method named GetDevice() which gets the input devices associated with the lxRNode (left hand) and assigns them to the devices list.
    
    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(lxRNode, devices);
        //device =- devices[2];

    }

    //This is a method named GetHead() which gets the input devices associated with the XRNode.Head (head) and assigns them to the heads list. It also assigns the first element of heads to the c_head variable.
    
    void GetHead()
    {
        InputDevices.GetDevicesAtXRNode(XRNode.Head, heads);
        c_head = heads[0];
    }

    //This method is called when the script or GameObject is enabled. It checks if the device variable is not valid (not initialized) and, if so, calls the GetDevice() method to retrieve the input device.
    
    void OnEnable()
    {
        if (!device.isValid)
        {
            GetDevice();
        }
    }

    /*This Update() method is called every frame. It performs the following actions:
    Retrieves the current text value from the valores Text component and logs it using Debug.Log().
    Checks if the device is not valid, and if so, retrieves the input device and head device using the GetDevice() and GetHead() methods.
    Sets up the devicePositionsUsage to represent the common usage of device position.
    Checks if the device position has changed by comparing devicePositionValue and prevdevicePositionValue. If it has changed, sets devicePositionChosen to false.
    If the head device successfully retrieves the feature value for device position, and the device position is not zero, and devicePositionChosen is false, it performs the following steps:
    Updates the text value of valores to the device position value as a string with three decimal places.
    Updates prevdevicePositionValue to the current devicePositionValue.
    Sets devicePositionChosen to true.
    Converts the device position value to a string and logs it using Debug.Log().
    Calls the SendTextData() method, passing the device position value as a string.
    If the device position is zero and devicePositionChosen is true, it performs the following steps:
    Updates the text value of valores to the device position value as a string with three decimal places.
    Updates prevdevicePositionValue to the current devicePositionValue.
    Sets devicePositionChosen to false.
    */
    void Update()
    {

        string str1 = valores.text;
        Debug.Log(str1);

        if (!device.isValid)
        {
            GetDevice();
            GetHead();
        }

        // capturing position changes
        InputFeatureUsage<Vector3> devicePositionsUsage = CommonUsages.devicePosition;
        // make sure the value is not zero and that it has changed
        if (devicePositionValue != prevdevicePositionValue)
        {
            devicePositionChosen = false;
        }

        if (c_head.TryGetFeatureValue(devicePositionsUsage, out devicePositionValue) && devicePositionValue != Vector3.zero && !devicePositionChosen)
        {
            valores.text = devicePositionValue.ToString("F3");
            prevdevicePositionValue = devicePositionValue;
            devicePositionChosen = true;
            string str = valores.text;
            Debug.Log(str);
            SendTextData(str);


                
        }
        else if (devicePositionValue == Vector3.zero && devicePositionChosen)
        {
            valores.text = devicePositionValue.ToString("F3");
            prevdevicePositionValue = devicePositionValue;
            devicePositionChosen = false;
        }
    }

    /*This SendTextData() method is a public method that takes a string str as a parameter. It performs the following actions:
      Retrieves a reference to the root node of the Firebase database using FirebaseDatabase.DefaultInstance.RootReference.
      Creates a child node reference named "textData" by calling Child("Data") on the root reference.
      Sets the value of the "textData" node to the provided str using SetValueAsync(str).
      Logs the message "Hello" using Debug.Log().
    */
    public void SendTextData(string str)
    {
        // Get a reference to the root node of the database
        DatabaseReference databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Get a reference to the "textData" node
        DatabaseReference textDataRef = databaseReference.Child("Data");

        // Send the text data to the "textData" node
        textDataRef.SetValueAsync(str);

        Debug.Log("Hello");
    }
}