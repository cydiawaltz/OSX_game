using UnityEngine;
using System;

public class SignalButton : MonoBehaviour
{
    [SerializeField] Renderer[] targetRender;
    [SerializeField] Transform center;
    [SerializeField] float radius;

    public Action OnClick;

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