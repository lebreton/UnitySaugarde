using UnityEngine;

public abstract class WindowUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup Group;

    public abstract void Ini();

    public void Show()
    {
        Group.alpha = 1;
        Group.interactable = true;
        Group.blocksRaycasts = true;

        Ini();
    }

    public void Close()
    {
        Group.alpha = 0;
        Group.interactable = false;
        Group.blocksRaycasts = false;
    }
}