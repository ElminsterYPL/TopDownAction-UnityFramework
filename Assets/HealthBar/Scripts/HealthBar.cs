using UnityEngine;

/// <summary>
/// 血条。
/// </summary>
public class HealthBar : MonoBehaviour {

    private MaterialPropertyBlock _matBlock;
    private MeshRenderer _meshRenderer;
    private Camera _mainCamera;
    
    private ChaHandler _chaHandler;

    private void Awake() 
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _matBlock = new MaterialPropertyBlock();
        
        _chaHandler = GetComponentInParent<ChaHandler>();
    }

    private void Start() 
    {
        _mainCamera = Camera.main;
    }

    private void Update() 
    {
        //只有血量不满时才显示
        if (_chaHandler.resource.Hp < _chaHandler.property.MaxHp) 
        {
            _meshRenderer.enabled = true;
            //AlignCamera();
            UpdateParams();
        } 
        else 
        {
            _meshRenderer.enabled = false;
        }

    }

    private void UpdateParams() 
    {
        _meshRenderer.GetPropertyBlock(_matBlock);
        _matBlock.SetFloat("_Fill", _chaHandler.resource.Hp / _chaHandler.property.MaxHp);
        _meshRenderer.SetPropertyBlock(_matBlock);
    }

    private void AlignCamera() 
    {
        if (_mainCamera) 
        {
            var camTrans = _mainCamera.transform;
            var forward = transform.position - camTrans.position;
            forward.Normalize();
            var up = Vector3.Cross(forward, camTrans.right);
            transform.rotation = Quaternion.LookRotation(forward, up);
        }
    }

}