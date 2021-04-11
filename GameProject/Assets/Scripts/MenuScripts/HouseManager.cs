using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HouseManager : MonoBehaviour
{
    List<BuildingBehaviour> _buildings = new List<BuildingBehaviour>();
    int buildingsLeft = 0;

    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private Animator houseWidgetAnimator = null;

    private void Start()
    {
        foreach(BuildingBehaviour building in FindObjectsOfType<BuildingBehaviour>())
        {
            if (building.CountsAsScore)
            {
                _buildings.Add(building);
                building.OnBroken += () => { OnBuildingBreak(building); };
            }
        }

        buildingsLeft = _buildings.Count;
        scoreText.text = buildingsLeft.ToString() + " / " + _buildings.Count;
    }

    private void OnBuildingBreak(BuildingBehaviour building)
    {
        buildingsLeft = Mathf.Clamp(buildingsLeft - 1, 0, _buildings.Count);

        scoreText.text = buildingsLeft.ToString() + " / " + _buildings.Count;
        houseWidgetAnimator.SetTrigger("BuildingBreak");
    }


}
