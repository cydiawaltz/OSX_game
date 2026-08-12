using UnityEngine;
using System;

public class SignalButton : MonoBehaviour
{
    [SerializeField] Renderer[] targetRender;
    [SerializeField] Transform center;
    [SerializeField] float radius;
    public Color pushcolor;

    public Action OnClick;

    void Start()
    {
        foreach (Renderer target in targetRender)
        {
            target.material.color = new Color(1, 1, 1, 1);
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

        }
    }
    void Precheck(Vector3 worldPoint)
    {
        float distance = Vector3.Distance(worldPoint, center.position);

        if (distance <= radius)
        {
            foreach (Renderer target in targetRender)
            {
                target.material.color = pushcolor;
            }
        }

    }
    public bool Check(Vector3 worldPoint)
    {
        float distance = Vector3.Distance(worldPoint, center.position);

        if (distance <= radius)
        {
            OnClick?.Invoke();

            return true;
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (center == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center.position, radius);
    }
#endif
}