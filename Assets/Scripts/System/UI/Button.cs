using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    public GameObject mainCamera;
    public GameObject otherCamera;
    private GameObject ActionBord;
    private GameObject HandsBord;
    //private GameObject RetuneButton;
    //private GameObject ResetButton;
    private GameObject SpeedButton;
    private GameObject ExecutionButton;
    //private float BordPosition_x;
    //private float BordPosition_y;
    //private float BordPosition_z;
    //private float SetPosition_x;
    //private float SetPosition_y;
    //private float SetPosition_z;
    //private bool position_Flag;

    // Use this for initialization
    void Awake ()
    {
        otherCamera.SetActive(false);
        ActionBord = GameObject.Find("ActionBord");
        HandsBord = GameObject.Find("HandsBord");
        //RetuneButton = GameObject.Find("RetuneButton");
        //ResetButton = GameObject.Find("ResetButton");
        SpeedButton = GameObject.Find("SpeedButton");
        ExecutionButton = GameObject.Find("PlayButton");
        //SetPosition_x = 11.0f;
        //SetPosition_y = 2.0f;
        //SetPosition_z = 1.2f;
        //BordPosition_x = 2.0f;
        //BordPosition_y = -2.5f;
        //BordPosition_z = 1.0f;
        //position_Flag = false;
        //if (mainCamera.activeSelf)
        //{
        //    RetuneButton.SetActive(false);
        //    ResetButton.SetActive(false);
        //    SpeedButton.SetActive(false);
        //}

        ActionBord.transform.localPosition = new Vector3(0.05f, 0.05f, -3.0f);

    }
	
	// Update is called once per frame
	void Update ()
    {
        //if (position_Flag == true)
        //{
        //    ActionBord.transform.localPosition = new Vector3(11.0f, 2.0f, 1.2f);
        //}
        //else
        //{
        //    ActionBord.transform.localPosition = new Vector3(2.0f, -2.5f, 1.0f);
        //}
    }

    public void OnClick()
    {

        

        if (mainCamera.activeSelf)
        {
            //position_Flag = true;
            mainCamera.SetActive(false);
            SpeedButton.SetActive(false);
            ExecutionButton.SetActive(false);
            otherCamera.SetActive(true);
            //RetuneButton.SetActive(true);
            //ResetButton.SetActive(true);

            HandsBord.transform.position = new Vector3(1.0f, 1.0f, -10.0f);
            ActionBord.transform.position = new Vector3(1.0f, 4.0f, -10.0f);
            //ActionBord.transform.localPosition = new Vector3(11.0f, 2.0f, 1.2f);
            //ActionBord.transform.localPosition += new Vector3(9.0f, 4.5f, 0.2f);
        }
        else
        {
            //position_Flag = false;
            mainCamera.SetActive(true);
            SpeedButton.SetActive(true);
            ExecutionButton.SetActive(true);
            otherCamera.SetActive(false);
            //RetuneButton.SetActive(false);
            //ResetButton.SetActive(false);

            ActionBord.transform.localPosition = new Vector3(0.05f, 0.05f, -3.0f);
            //ActionBord.transform.localPosition -= new Vector3(9.0f, 4.5f, 0.2f);
        }
    }
}
