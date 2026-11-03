using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private float verticalVelocity = 0f;

    private bool loggedOwnerStatus = false;
    private bool loggedGameScoreMissing = false;
    private bool loggedGameNotStarted = false;
    private bool loggedKeyboardMissing = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void OnNetworkSpawn()
    {
        controller.enabled = IsOwner;

        Debug.Log(
            "Player Spawn됨 / Owner: " + IsOwner +
            " / ClientId: " + OwnerClientId +
            " / Spawned: " + IsSpawned
        );
    }

    void Update()
    {
        if (!IsOwner || !IsSpawned)
        {
            if (!loggedOwnerStatus)
            {
                Debug.Log(
                    "이동 중단 / Owner: " + IsOwner +
                    " / Spawned: " + IsSpawned
                );

                loggedOwnerStatus = true;
            }

            return;
        }

        if (GameScore.Instance == null)
        {
            if (!loggedGameScoreMissing)
            {
                Debug.Log("이동 중단 / GameScore.Instance가 없음");
                loggedGameScoreMissing = true;
            }

            return;
        }

        if (!GameScore.Instance.GameStarted.Value)
        {
            if (!loggedGameNotStarted)
            {
                Debug.Log("이동 중단 / GameStarted가 false");
                loggedGameNotStarted = true;
            }

            return;
        }

        if (Keyboard.current == null)
        {
            if (!loggedKeyboardMissing)
            {
                Debug.Log("이동 중단 / Keyboard.current가 없음");
                loggedKeyboardMissing = true;
            }

            return;
        }

        float x = 0f;
        float z = 0f;

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed)
        {
            x -= 1f;
        }

        if (Keyboard.current.dKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            x += 1f;
        }

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed)
        {
            z += 1f;
        }

        if (Keyboard.current.sKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed)
        {
            z -= 1f;
        }

        Vector3 move = new Vector3(x, 0f, z).normalized;

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}