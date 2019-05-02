﻿#undef DEBUG_LOG  // 磁力線を引く処理時間を計測するため
using UnityEngine;

/// <summary>
/// UpdateでApplicationParamsのstateを監視
/// </summary>
public class BarMagnetMagneticForceLinesSimultaneouslyDrawer : MonoBehaviour {

    public int Mode = 0;  // 0: 2D, 1: 3D  Todo: Listを使う
    private GameObject magneticForceLine;
    private bool hasLogged;

    static Material lineMaterial;

    private bool isDrawingCurrent = false;
    private bool isDrawingOld = false;

    /*
    private void Start()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);
    }
    */
    private void Start()
    {
        magneticForceLine = BarMagnetModel.Instance.MagneticForceLineReference;
    }

    public void Update()
    {
        isDrawingCurrent = BarMagnetModel.Instance.IsDrawing;

        if (isDrawingCurrent)
        {
            Draw();
        }

        if (isDrawingCurrent != isDrawingOld)
        {
            if (!isDrawingCurrent)
            {
                DeleteLines();
            }
        }
        isDrawingOld = isDrawingCurrent;
    }

    public void DeleteLines()
    {
        GameObject[] lines = GameObject.FindGameObjectsWithTag("CloneLine");

        foreach (GameObject line in lines)
        {
            Destroy(line);
        }
    }

    //static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    /*
    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);

        GL.Color(new Color(1, 0, 0, 1));
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0.1F, 0, 0);

        GL.Color(new Color(0, 1, 0, 1));
        GL.Vertex3(0.1F, 0.1F, 0);
        GL.Vertex3(0, 0.1F, 0);

        GL.End();
    }
    */

    public void Draw()
    {
        GameObject myMagnet = gameObject;

        magneticForceLine.SetActive(true);
        this.DeleteLines();

        Vector3 myBarMagnetNorthPoleWorldPosition = BarMagnetModel.Instance.NorthPoleReference.transform.position;
        Vector3 myBarMagnetSouthPoleWorldPosition = BarMagnetModel.Instance.SouthPoleReference.transform.position;

        // デバッグ用ログ出力
        MyHelper.DebugLogEvery10Seconds(
            "DrawMagnetForceLines3D.Update() is fired.\n" +
            "BarMagnet: " + gameObject.transform.position.ToString() + "\n" +
            "NorthPole: " + myBarMagnetNorthPoleWorldPosition.ToString() + "\n" +
            "SouthPole: " + myBarMagnetSouthPoleWorldPosition.ToString(), ref hasLogged);

        Vector3 barMagnetDirection = transform.rotation.eulerAngles;

        //for (int i = -1; i <= 1; i += 2)  // j=1のときN極側の磁力線を描く
        for (int i = -1; i <= 1; i += 2)  // j=1のときN極側の磁力線を描く
        {
            int numStartZ = -2;  // 磁力線描画開始地点を 奥行き 方向にいくつとるか
            int numEndZ   = -numStartZ;
            int numShiftZ = 2;   // 磁力線描画開始地点を 奥行き 方向にいくつとるか
            int numStartY = -2;  // 磁力線描画開始地点を 垂直   方向にいくつとるか
            int numEndY   = -numStartZ;
            int numShiftY = 1;   // 磁力線描画開始地点を 垂直   方向にいくつとるか

            for (int j = numStartZ; j <= numEndZ; j += numShiftZ) // z
            {
                for (int k = numStartY; k <= numEndY; k += numShiftY) // y
                {
                    GameObject magneticForceLine =
                        Instantiate(this.magneticForceLine, transform.position, Quaternion.identity);

                    // 作成したオブジェクトを子として登録
                    magneticForceLine.tag = "CloneLine";
                    magneticForceLine.transform.parent = transform;

                    bool lineIsFromNorthPole = true;
                    Vector3 myBarMagnetPoleWorldPosition;

                    // N極
                    if (i == 1)
                    {
                        lineIsFromNorthPole = true;
                        myBarMagnetPoleWorldPosition = myBarMagnetNorthPoleWorldPosition;
                    }
                    // S極
                    else
                    {
                        lineIsFromNorthPole = false;
                        myBarMagnetPoleWorldPosition = myBarMagnetSouthPoleWorldPosition;
                    }

                    Vector3 shiftPositionFromMyPole = new Vector3(
                        0.001f * j,  // y
                        0.001f * i,  // x
                        0.001f * k  // z
                        );

                    shiftPositionFromMyPole =
                        myMagnet.transform.rotation * shiftPositionFromMyPole;
                    Vector3 startPosition = myBarMagnetPoleWorldPosition + shiftPositionFromMyPole;

                    /*
                    // 処理時間の計測
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                    */

                    BarMagnetMagneticForceLineDrawer.Instance.Draw(
                        magneticForceLine, lineIsFromNorthPole, startPosition, 0.003f);

                    /*
                                        // 処理時間の計測
                    sw.Stop();
            #if DEBUG_LOG
                    Debug.Log("DrawMagnetForceLines3D takes " + sw.ElapsedMilliseconds + "ms");
            #endif
                    */
                }
            }
        }
        magneticForceLine.SetActive(false);
    }
}
