using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DashingCooldownUI : MonoBehaviour
{
    [SerializeField] private float spacing;
    private GameObject _pointUIPrefab;
    private List<GameObject> _pointObjects;

    private void Awake()
    {
        _pointUIPrefab = Resources.Load("Prefabs/Dashing Point") as GameObject;
        _pointObjects = new List<GameObject>();
    }

    private void Start()
    {
        var dashPoints = PlayerController.Instance.DashMaxPoints;
        var startX = -(dashPoints - 1) / 2f * spacing;

        for (var i = 0; i < PlayerController.Instance.DashMaxPoints; i++)
        {
            var offset = startX + (i * spacing);
            var newPosition = transform.position + new Vector3(offset, 0, 0);

            var dashPoint = Instantiate(_pointUIPrefab, transform);
            dashPoint.GetComponent<RectTransform>().position = newPosition;
            _pointObjects.Add(dashPoint);
        }

        PlayerController.DashCooldownEvent += UpdateDashPoint;
    }
    private void UpdateDashPoint(int amount)
    {
        // TODO: Refine this
        _pointObjects.ForEach(p => p.GetComponent<Toggle>().isOn = false);
        _pointObjects.Take(amount).ToList().ForEach(p => p.GetComponent<Toggle>().isOn = true);
    }
}