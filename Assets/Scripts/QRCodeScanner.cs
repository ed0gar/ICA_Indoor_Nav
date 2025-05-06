using UnityEngine;
using ZXing;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;

public class QRCodeScanner : MonoBehaviour
{
    [SerializeField] ARCameraManager arCameraManager;

    IBarcodeReader _reader;
    bool _scanning = true;

    void Awake()
    {
        // configure ZXing
        _reader = new BarcodeReader
        {
            AutoRotate = true,
            TryInverted = true
        };
    }

    void OnEnable()
    {
        arCameraManager.frameReceived += OnFrameReceived;
    }

    void OnDisable()
    {
        arCameraManager.frameReceived -= OnFrameReceived;
    }

    void OnFrameReceived(ARCameraFrameEventArgs args)
    {
        if (!_scanning)
            return;

        // grab latest camera CPU image
        if (!arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            return;

        // set up conversion to RGBA32
        var config = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width, image.height),
            outputFormat = TextureFormat.RGBA32,
            transformation = XRCpuImage.Transformation.None
        };

        int size = image.GetConvertedDataSize(config);
        var nativeBuffer = new NativeArray<byte>(size, Allocator.Temp);

        // convert into the native array
        image.Convert(config, nativeBuffer);
        image.Dispose();

        // copy into managed array for ZXing
        byte[] managedBuffer = nativeBuffer.ToArray();
        nativeBuffer.Dispose();

        // try decode
        var result = _reader.Decode(
            managedBuffer,
            config.outputDimensions.x,
            config.outputDimensions.y,
            RGBLuminanceSource.BitmapFormat.RGBA32
        );

        if (result != null)
        {
            _scanning = false;
            OnScanned(result.Text);
        }
    }

    void OnScanned(string text)
    {
        Debug.Log($"QR Code: {text}");
        var mgr = FindObjectOfType<BuildingInfoManager>();
        if (mgr != null)
            mgr.OnQRCodeScanned(text);
    }

    /// <summary>
    /// Call this to resume scanning (e.g. from a “Reset” button)
    /// </summary>
    public void RestartScanning()
    {
        _scanning = true;
    }
}
