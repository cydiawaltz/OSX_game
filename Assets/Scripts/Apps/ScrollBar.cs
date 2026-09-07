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
    public bool isActiveByWheel;
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
    [Header("マウスホイールスクロール範囲")]
    [SerializeField] Renderer scrollArea;

    [SerializeField] float wheelScrollSpeed = 0.1f;

    void Start()
    {
        rect = FunctionSet.GetRectAngle(target, WindowManager.overCam);
        var mousePos = Input.mousePosition;
        Vector3 world = WindowManager.overCam.ScreenToWorldPoint(mousePos);
        oldLocal = target.transform.InverseTransformPoint(world);
        parent.OnDragEnd += () =>
        {
            rect = FunctionSet.GetRectAngle(target, WindowManager.overCam);
        };

    }

    // Update is called once per frame
    void Update()
    {
        var mousePos = Input.mousePosition;
        if (Input.GetMouseButtonDown(0) && parent.isTopMost)
        {
            Debug.Log("Scrollbar activate");
            if (mousePos.x >= rect.minX && mousePos.x <= rect.maxX && mousePos.y >= rect.minY && mousePos.y <= rect.maxY)
            {
                isActive = true;
                Debug.Log("Scrollbar on the area");
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
                // マウスホイールスクロール
        if (parent.isTopMost &&
            !isActive &&
            scrollArea != null &&
            WindowManager.overCam.gameObject.activeSelf)
        {
            bool isOverArea = IsMouseOverScrollArea(mousePos);

            // ホイールスクロール開始
            if (isOverArea && !isActiveByWheel)
            {
                isActiveByWheel = true;
                //isActive = true;
                ScrollBarDown?.Invoke();
            }
            // ホイールスクロール終了
            else if (!isOverArea && isActiveByWheel)
            {
                isActiveByWheel = false;
                //isActive = false;
                ScrollBarUp?.Invoke();
            }

            if (isOverArea)
            {
                float wheel = Input.mouseScrollDelta.y;

                if (wheel != 0)
                {
                    currentPosition += wheel * wheelScrollSpeed;
                    currentPosition = Mathf.Clamp01(currentPosition);

                    SetScrollPosition(currentPosition);
                }
            }
        }
        else if (isActiveByWheel)
        {
            // ウインドウが最前面でなくなった等の場合も終了扱い
            isActiveByWheel = false;
            ScrollBarUp?.Invoke();
        }
    }
    bool IsMouseOverScrollArea(Vector3 mousePos)
    {
        Rect areaRect = GetRendererScreenRect(scrollArea);

        return areaRect.Contains(
            new Vector2(mousePos.x, mousePos.y)
        );
    }
    Rect GetRendererScreenRect(Renderer renderer)
    {
        Bounds bounds = renderer.bounds;

        Vector3[] corners = new Vector3[8];

        corners[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
        corners[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        corners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        corners[3] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        corners[4] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        corners[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        corners[6] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        corners[7] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);

        Vector3 screenMin =
            WindowManager.overCam.WorldToScreenPoint(corners[0]);

        Vector3 screenMax = screenMin;

        for (int i = 1; i < corners.Length; i++)
        {
            Vector3 screen =
                WindowManager.overCam.WorldToScreenPoint(corners[i]);

            screenMin = Vector3.Min(screenMin, screen);
            screenMax = Vector3.Max(screenMax, screen);
        }

        return Rect.MinMaxRect(
            screenMin.x,
            screenMin.y,
            screenMax.x,
            screenMax.y
        );
    }
    void SetScrollPosition(float position)
    {
        Vector3 pos = target.transform.localPosition;

        switch (axis)
        {
            case ScrollAxis.X:
                pos.x = Mathf.Lerp(min, max, position);
                break;

            case ScrollAxis.Y:
                pos.y = Mathf.Lerp(min, max, position);
                break;

            case ScrollAxis.Z:
                pos.z = Mathf.Lerp(min, max, position);
                break;
        }

        target.transform.localPosition = pos;
        
    }
}
