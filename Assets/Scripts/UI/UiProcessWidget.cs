using UnityEngine;
using UnityEngine.UI;

public class UiProcessWidget : MonoBehaviour
{
    [SerializeField]
    Text _name;
    [SerializeField]
    Slider _progressBar;

    public int id { get; set; }

    public void SetName(string name)
    {
        _name.text = name;
    }

    public void SetProgress(float value)
    {
        _progressBar.value = value;
    }
}
