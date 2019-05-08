﻿using System.Collections.Generic;
using UnityEngine;

public class CompassesManagedlySimultaneouslyUpdater : MonoBehaviour
{

    BarMagnetModel magnet;
    private void Start()
    {
        magnet = GameObject.FindObjectOfType<BarMagnetModel>();

    }
    // Update is called once per frame
    void Update()
    {
        // Observer
        var compasses = CompassesModel.Instance.CompassesReferenceForManagedlyUpdate;

        // Presenter
        if (compasses.Count == 0)
            return;

        //コンパスが複数の場合は、磁石の移動に合わせて位置をupdateする
        if (compasses.Count > 1)
            UpdateCompassParentPosition();

        //コンパスの向きのupdate
        ManagedlyUpdate(compasses);
    }

    void UpdateCompassParentPosition()
    {
        //コンパスの間隔何個分ずれたら、移動を発生させるかの閾値
        int check = 4;

        //磁石とコンパスの位置差分を取得
        var offset = magnet.transform.position - CompassesModel.Instance.ParentTransform.position;
        //コンパスの間隔を取得
        var pitch = CompassesModel.Instance.pitch;

        //間隔何個分ずれているかを取得
        var vecToMove = offset / pitch;


        //x/y/zの優先順位で適用する
        var p = Vector3.zero;

        if (Mathf.Abs(vecToMove.x) >= check)
        {
            p.x += (int)vecToMove.x;
        }
        else if (Mathf.Abs(vecToMove.y) >= check)
        {
            p.y += (int)vecToMove.y;
        }
        else if (Mathf.Abs(vecToMove.z) >= check)
        {
            p.z += (int)vecToMove.z;
        }

        if (p != Vector3.zero)
            CompassesModel.Instance.ParentTransform.position += p * pitch;
    }

    void ManagedlyUpdate(List<CompassManagedlyUpdater> compasses)
    {
        foreach (CompassManagedlyUpdater compass in compasses)
        {
            compass.ManagedlyUpdate();
        }
    }
}
