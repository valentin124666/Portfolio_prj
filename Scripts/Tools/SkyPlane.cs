using Core;
using Managers.Controller;
using Managers.Interfaces;
using UnityEngine;

public class SkyPlane : MonoBehaviour
{
    private CameraController _cameraController;

    private Vector3 _localPos;
    public void Init()
    {
        _cameraController = GameClient.Get<IGameplayManager>().GetController<CameraController>();
        _localPos = transform.localPosition;
    }

    public void SetSizeByCamera()
    {
        _cameraController.SetChild(transform);

        transform.localPosition = _localPos;

        var height = (2.0f * Mathf.Tan(0.5f * _cameraController.CameraMain.fieldOfView * Mathf.Deg2Rad) * transform.localPosition.z);
        var width = height * Screen.width / Screen.height;

        transform.localScale = new Vector3(width +(width*0.6f), height+(height*0.6f), 1f);
        
        transform.SetParent(null);
    }
}