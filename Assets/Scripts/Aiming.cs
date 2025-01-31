using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Aiming : NetworkBehaviour
{

    public Transform playerTransform;
    public float gunDistance = 1.5f;
    public bool lockedIn;

    [SerializeField] private SpriteRenderer gunSprite;
    private readonly NetworkVariable<bool> isFacingRightNetwork = new(true);
    private NetworkVariable<Quaternion> gunRotation = new (default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> gunPositionAngle = new (default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    private void Start()
    {
        isFacingRightNetwork.OnValueChanged += OnFacingDirectionChanged;
    }

    public override void OnDestroy()
    {
        isFacingRightNetwork.OnValueChanged -= OnFacingDirectionChanged;
    }

    public void Update()
    {
        if (lockedIn)
            return;

        if (IsOwner)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePos - playerTransform.position;

            Quaternion newRotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            gunRotation.Value = newRotation;
            gunPositionAngle.Value = angle;

            GunFlipController(mousePos);
        }

        transform.rotation = gunRotation.Value;
        transform.position = playerTransform.position + Quaternion.Euler(0, 0, gunPositionAngle.Value) * new Vector3(gunDistance, 0, 0);
    }

    private void OnFacingDirectionChanged(bool oldValue, bool newValue)
    {
        gunSprite.flipY = !newValue;
    }

    private void GunFlipController(Vector3 mousePos)
    {
        bool isFacingRight = mousePos.x > transform.position.x;

        if (isFacingRight != isFacingRightNetwork.Value)
        {
            if (IsServer)
            {
                isFacingRightNetwork.Value = isFacingRight;
            }
            else
            {
                UpdateFacingDirectionServerRpc(isFacingRight);
            }
        }
    }

    [ServerRpc]
    private void UpdateFacingDirectionServerRpc(bool newFacingRight)
    {
        isFacingRightNetwork.Value = newFacingRight;
    }
}
