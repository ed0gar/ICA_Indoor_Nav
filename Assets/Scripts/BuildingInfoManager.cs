using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;


[Serializable]
public class Building
{
    public string Id;
    public string Buildingname;
    public string description;
    public string image;  
}

[Serializable]
public class BuildingList
{
    public List<Building> buildings;
}

public class BuildingInfoManager : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private GameObject qrCanvas;            
    [SerializeField] private GameObject buildingInfoCanvas;  

    [Header("Info UI References")]
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private Image buildingImage;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Reset Buttons")]
    [SerializeField] private Button resetButton;   
    [SerializeField] private Button resetButton2;  

    private Dictionary<string, Building> _lookup;

    void Awake()
    {

        var json = Resources.Load<TextAsset>("BuildingInfo");
        if (json == null)
        {
            Debug.LogError("Missing BuildingInfo.json in Resources!");
            return;
        }

        var list = JsonUtility.FromJson<BuildingList>(json.text);
        _lookup = new Dictionary<string, Building>();
        foreach (var b in list.buildings)
            _lookup[b.Id] = b;

      
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetToScan);
        if (resetButton2 != null)
            resetButton2.onClick.AddListener(ResetToScan);


        qrCanvas.SetActive(true);
        buildingInfoCanvas.SetActive(false);
    }


    public void OnQRCodeScanned(string scannedId)
    {
        if (_lookup.TryGetValue(scannedId, out var b))
            ShowBuilding(b);
        else
            Debug.LogWarning($"No building info for ID: {scannedId}");
    }

    private void ShowBuilding(Building b)
    {
  
        qrCanvas.SetActive(false);
        buildingInfoCanvas.SetActive(true);


        buildingNameText.text = b.Buildingname;
        descriptionText.text = b.description;


        string path = $"Images/{b.image}";
        Debug.Log($"Attempting to load sprite at Resources/{path}.png");
        var spr = Resources.Load<Sprite>(path);
        if (spr != null)
            buildingImage.sprite = spr;
        else
            Debug.LogWarning($"Sprite not found at Resources/{path}.png");
    }

    private void ResetToScan()
    {
        buildingInfoCanvas.SetActive(false);
        qrCanvas.SetActive(true);


        buildingNameText.text = "";
        descriptionText.text = "";
        buildingImage.sprite = null;

        var scanner = FindObjectOfType<QRCodeScanner>();
        if (scanner != null)
            scanner.RestartScanning();
    }
}