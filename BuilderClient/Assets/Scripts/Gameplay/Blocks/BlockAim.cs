using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAim : MonoBehaviour
{
    [SerializeField] private GameObject _aim;
    [SerializeField] private Transform _quadCanvas;
    [SerializeField] private LayerMask _mask;
    private Camera _camera;
    private Vector3 _globalAimPostion;

    public Vector3Int Position { get; private set; } = Vector3Int.zero;
    public Quaternion Rotation { get; private set; } = Quaternion.identity;

    private void Start()
    {
        _camera = Camera.main;
    }

    public void Lock()
    {
        
    }

    public void Unlock()
    {

    }

    public void SetQuadLocation(in Vector3 mousePosition, in Vector3 playerPosition)
    {
        Ray ray = _camera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, _mask))
        {
            _globalAimPostion = hit.point;
            Vector3 normal = hit.normal;

            Vector3 position = _globalAimPostion + (normal * 0.5f);

            Vector3Int roundPosition = Vector3Int.zero;
            roundPosition.x = (int)Mathf.Round(position.x);
            roundPosition.y = (int)Mathf.Round(position.y);
            roundPosition.z = (int)Mathf.Round(position.z);
            Position = roundPosition;
            _quadCanvas.position = Position;

            Vector3 playerForward = RelativeForward(playerPosition,_globalAimPostion,roundPosition,normal);

#if UNITY_EDITOR
            Debug.DrawRay(_globalAimPostion, normal, Color.green);
            Debug.DrawRay(_globalAimPostion, playerForward, Color.blue);
#endif
            Rotation = Quaternion.LookRotation(playerForward, normal);
            _quadCanvas.rotation = Rotation;
        }
    }

    private Vector3 RelativeForward(in Vector3 from, in Vector3 realTo, in Vector3 roundTo, in Vector3 up)
    {
        if (Mathf.Abs(up.y) < .1f)
            return (realTo.y - roundTo.y) > 0 ? Vector3.up : Vector3.down;
        else
        {
            float xOffset = Mathf.Abs(realTo.x - from.x);
            float zOffset = Mathf.Abs(realTo.z - from.z);

            if (xOffset > zOffset)
                return (realTo.x - from.x) > 0 ? Vector3.right : Vector3.left;
            else
                return (realTo.z - from.z) > 0 ? Vector3.forward : Vector3.back;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_globalAimPostion, .1f);
    }
#endif
}
