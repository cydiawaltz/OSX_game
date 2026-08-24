using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowDrawer : MonoBehaviour
{
    [SerializeField] Window parent;
    WindowManager manager;
    public DecalProjector decalProjector;
    public Texture[] textures;//0:有効 1:裏
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<WindowManager>();
        manager.changeIndexState += StateChange;
    }
    
    void StateChange()
    {
        Debug.Log("ShadowDrawer:StateChange");
        //if(manager.windows_statestore[0] == parent)
        if(parent.isTopMost)
        {
            decalProjector.material.mainTexture = textures[0];
            Debug.Log("ShadowDrawer:StateChange:表");
        }
        else
        {
            decalProjector.material.mainTexture = textures[1];
            Debug.Log("ShadowDrawer:StateChange:裏");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
