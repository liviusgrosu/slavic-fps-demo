using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Image _currentBar;
    private void Start()
    {
        //_currentBar = transform.Find("Current").GetComponent<Image>();
    }

    public void UpdateBar(int max, int current)
    {
        _currentBar.fillAmount = (float)current / (float)max;
    }
}
