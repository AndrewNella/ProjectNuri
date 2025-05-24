using UnityEngine;
using UnityEngine.EventSystems;

public interface UIEventSelection
{
    public void SetCurrentlySelectedObject(GameObject _incomingGameObject);

    public void ClearCurrentlySelectedObject();
}
