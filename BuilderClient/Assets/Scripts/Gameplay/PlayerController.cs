using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _mouseSensetivity = 1;
    [SerializeField] private PlayerCharacter _player;
    private BlockAim _aim;
    private BlockManager _blockManager;
    private BlockMap _blockMap;
    private MultiplayerManager _multiplayerManager;
    private bool _hideCursor;

    private void Start()
    {
        _multiplayerManager = MultiplayerManager.Instance;
        _hideCursor = true;
        Cursor.lockState = CursorLockMode.Locked;

        _aim = FindObjectOfType<BlockAim>();
        _blockManager = FindObjectOfType<BlockManager>();
        _blockMap = FindObjectOfType<BlockMap>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            _hideCursor = !_hideCursor;

            if (_hideCursor )
            {
                Cursor.lockState = CursorLockMode.Locked;
                _aim.Lock();
            } else
            {
                Cursor.lockState = CursorLockMode.None;
                _aim.Unlock();
            }
        }

        int mouseScroll = (int)Input.mouseScrollDelta.y;
        float mouseX = 0;
        float mouseY = 0;
        if (_hideCursor)
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }

        Vector3 mousePosition = Input.mousePosition;
        bool lbm = Input.GetMouseButtonDown(0);
        bool rbm = Input.GetMouseButtonDown(1);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isJump = Input.GetKeyDown(KeyCode.Space);

        if(mouseScroll != 0) _blockManager.AddCurrentIndex(mouseScroll);
        _aim.SetQuadLocation(mousePosition, _player.transform.position);
        if (lbm) _blockMap.SpawnBlock();
        if (rbm) _blockMap.DestroyBlock(in mousePosition);
        _player.SetInput(horizontal, vertical, mouseX * _mouseSensetivity);
        _player.RotateX(-mouseY * _mouseSensetivity);

        if (isJump) _player.Jump();

        SendMove();
    }


    private void SendMove()
    {
        _player.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY);
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "pX", position.x },
            { "pY", position.y },
            { "pZ", position.z },
            { "vX", velocity.x },
            { "vY", velocity.y },
            { "vZ", velocity.z },
            { "rX", rotateX },
            { "rY", rotateY }
        };

        _multiplayerManager.SendMessage("move", data);
    }

}
