﻿using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class BarMagnetModel : MonoBehaviour
{
    [HideInInspector] public GameObject NorthPoleReference;
    [HideInInspector] public GameObject SouthPoleReference;
    public GameObject handReference;
    public GameObject MagneticForceLinePrefab;
    //public bool IsDrawing;

    // Use this for initialization
    void Start()
    {
        gameObject.GetComponent<BarMagnetModel>().NorthPoleReference =
            transform.Find("North Body/North Pole").gameObject;
        gameObject.GetComponent<BarMagnetModel>().SouthPoleReference =
            transform.Find("South Body/South Pole").gameObject;

        //準備出来たらGlobalListenerに追加
        if (gameObject.GetComponent<SetGlobalListener>() == null)
            gameObject.AddComponent<SetGlobalListener>();

        if (handReference != null)
            handReference.SetActive(false);

    }

}
