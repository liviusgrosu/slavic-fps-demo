using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Image _currentBar;

    public void UpdateBar(int max, int current)
    {
        _currentBar.fillAmount = (float)current / (float)max;
    }
}
