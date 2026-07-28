using UnityEngine;
using System;

public class ScrollBar : MonoBehaviour
{
    public GameObject target;
    public Window parent;
    public Action ScrollBarDown;
    public Action ScrollBarUp;
    public float currentPosition;
    [SerializeField] float max, min;
    //public bool isXaxis;
    public bool isActive;
    public RectAngleSet rect;
    public Vector3 local;
    public Vector3 oldLocal;
    private Vector3 dragStartMouseLocal;
    private Vector3 dragStartBarLocal;
    private float screenZ;
    public enum ScrollAxis
    {
        X,
        Y,
        Z
    }

    [SerializeField] private ScrollAxis axis;

    void Start()
    {
        rect = FunctionSet.GetRectAngle(target, WindowManager.overCam);
        var mousePos = Input.mousePosition;
        Vector3 world = WindowManager.overCam.ScreenToWorldPoint(mousePos);
        oldLocal = target.transform.InverseTransformPoint(world);
    }

    // Update is called once per frame
    void Update()
    {
        var mousePos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0) && parent.isTopMost)
        {
            if (mousePos.x >= rect.minX && mousePos.x <= rect.maxX && mousePos.y >= rect.minY && mousePos.y <= rect.maxY)
            {
                isActive = true;
                ScrollBarDown?.Invoke();
                screenZ = WindowManager.overCam.WorldToScreenPoint(target.transform.position).z;
                mousePos.z = screenZ;
                Vector3 world = WindowManager.overCam.ScreenToWorldPoint(mousePos);
                dragStartMouseLocal = target.transform.parent.InverseTransformPoint(world);
                dragStartBarLocal = target.transform.localPosition;
            }
        }
        else if (Input.GetMouseButtonUp(0) && parent.isTopMost)
        {
            isActive = false;
            ScrollBarUp?.Invoke();
            rect = FunctionSet.GetRectAngle(target, WindowManager.overCam);
        }
        if (isActive && WindowManager.overCam.gameObject.activeSelf)
        {
            mousePos.z = screenZ;
            Vector3 world = WindowManager.overCam.ScreenToWorldPoint(mousePos);
            Vector3 currentMouseLocal = target.transform.parent.InverseTransformPoint(world);

            Vector3 pos = dragStartBarLocal;

            switch (axis)
            {
                case ScrollAxis.X:
                    pos.x += currentMouseLocal.x - dragStartMouseLocal.x;
                    pos.x = Mathf.Clamp(pos.x, min, max);
                    currentPosition = Mathf.InverseLerp(min, max, pos.x);
                    break;

                case ScrollAxis.Y:
                    pos.y += currentMouseLocal.y - dragStartMouseLocal.y;
                    pos.y = Mathf.Clamp(pos.y, min, max);
                    currentPosition = Mathf.InverseLerp(min, max, pos.y);
                    break;

                case ScrollAxis.Z:
                    pos.z += currentMouseLocal.z - dragStartMouseLocal.z;
                    pos.z = Mathf.Clamp(pos.z, min, max);
                    currentPosition = Mathf.InverseLerp(min, max, pos.z);
                    break;
            }

            target.transform.localPosition = pos;
        }
    }
}
