using UnityEngine;
using System;
using UnityEngine.Events;

public class UIButton : MonoBehaviour
{
    [SerializeField] UnityEvent OnClick;

    [SerializeField] Camera overViewCamera;
    [SerializeField] bool turnUnActiveOnEnd;
     Material target;
    [SerializeField] Renderer targetrenderer;
    [SerializeField] Texture[] textures;//0:有効 1:無効 2:裏有効
    //[SerializeField] Color DownColor;
    public Window parent;

    private RectAngleSet rect;
    bool isActive = true;

    void Start()
    {
        if (overViewCamera == null)
            overViewCamera = WindowManager.overCam;
        target = targetrenderer.material;
        parent.OnChangeWindowState += ChangeActiveState;
        parent.OnDragEnd += () => { rect = FunctionSet.GetRectAngle(gameObject, overViewCamera); };
    }

    void Update()
    {
        //rect = FunctionSet.GetRectAngle(gameObject, overViewCamera);
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Input.mousePosition;
            if(isActive && mouse.x >= rect.minX &&
                mouse.x <= rect.maxX &&
                mouse.y >= rect.minY &&
                mouse.y <= rect.maxY)
            {
                target.mainTexture = textures[0];
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mouse = Input.mousePosition;

            if (mouse.x >= rect.minX &&
                mouse.x <= rect.maxX &&
                mouse.y >= rect.minY &&
                mouse.y <= rect.maxY)
            {
                OnClick?.Invoke();
            }
            target.mainTexture = isActive ? textures[0] : textures[1];
            if(turnUnActiveOnEnd) isActive = false;
        }
    }
    void ChangeActiveState()
    {
        if(parent.isTopMost && !parent.oldTopMost)
        {
            target.mainTexture = textures[0];
        }
        else if(!parent.isTopMost && parent.oldTopMost)
        {
            target.mainTexture = textures[2];
        }
    }
}
