using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;



public class SetNavTarget : MonoBehaviour
{

    [SerializeField]
    private TMP_Dropdown navigationTargetDropDown;
    [SerializeField]
    private List<Target> navigationTargetObjects = new List<Target>();
    [SerializeField]
    private Slider navigationYOffset;

    private NavMeshPath path; // current calculated path
    private LineRenderer line; // linerenderer to display path
    private Vector3 targetPosition = Vector3.zero; // current target position

    private int currentFloor = 1;

    private bool lineToggle = false;

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        line.enabled = lineToggle;
    }

    private void Update()
    {
        if (lineToggle && targetPosition != Vector3.zero)
        {
            NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);
            line.positionCount = path.corners.Length;
            Vector3[] calculatedPathAndOffset = AddLineOffset();
            line.SetPositions(calculatedPathAndOffset);
        }
    }

    public void SetCurrentNavigationTarget(int selectedValue)
    {
        targetPosition = Vector3.zero;
        string selectedText = navigationTargetDropDown.options[selectedValue].text;
        Target currentTarget = navigationTargetObjects.Find(x => x.Name.ToLower().Equals(selectedText.ToLower()));
        if (currentTarget != null)
        {

            if (!line.enabled)
            {
                ToggleVisibility();
            }
            targetPosition = currentTarget.PositionObject.transform.position;
        }
    }

    public void ToggleVisibility()
    {
        lineToggle = !lineToggle;
        line.enabled = lineToggle;
    }

    public void ChangeActiveFloor(int floorNumber)
    {
        currentFloor = floorNumber;
        SetNavigationTargetDropDownOptions(currentFloor);
    }

    private Vector3[] AddLineOffset()
    {
        if (navigationYOffset.value == 0)
        {
            return path.corners;
        }

        Vector3[] calculatedLine = new Vector3[path.corners.Length];
        for (int i = 0; i < path.corners.Length; i++)
        {
            calculatedLine[i] = path.corners[i] + new Vector3(0, navigationYOffset.value, 0);
        }
        return calculatedLine;
    }

    private void SetNavigationTargetDropDownOptions(int floorNumber)
    {
        navigationTargetDropDown.ClearOptions();
        navigationTargetDropDown.value = 0;

        if (line.enabled)
        {
            ToggleVisibility();
        }

        // make sure to clear any existing entries first, if needed:
        navigationTargetDropDown.options.Clear();

        if (floorNumber == 1)
        {
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("PrinterArea1"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("PrinterArea2"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Men'sToilets"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Women'sToilets"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("AccessibleToilets"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Ipad1"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Ipad2"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Lift1"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("Lift2"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("FireEscape1"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("FireEscape2"));
        }
        else if (floorNumber == 2)
        {
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("SecondPrinterArea1"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("SecondPrinterArea2"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("SecondMen'sToilets"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("SecondWomen'sToilets"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("SecondAccessibleToilets"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("SecondIpad1"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("SecondIpad2"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("SecondLift1"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("SecondLift2"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("SecondFireEscape1"));
            navigationTargetDropDown.options.Add(new TMP_Dropdown.OptionData("SecondFireEscape2"));
        }

    }
}
