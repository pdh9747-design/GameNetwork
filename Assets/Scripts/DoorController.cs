using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    public Transform door;
    public float openDistance = 2f;
    public float openSpeed = 3f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private PlayerInventory playerInventory;

    void Start()
    {
        if (door == null)
            door = transform;

        closedPosition = door.position;
        openPosition = closedPosition + Vector3.right * openDistance;
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame && !isOpen)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
                playerInventory = player.GetComponent<PlayerInventory>();

            if (playerInventory != null && playerInventory.hasKey)
            {
                isOpen = true;
                Debug.Log("열쇠를 사용해 문을 열었습니다!");
            }
            else
            {
                Debug.Log("문을 열려면 열쇠가 필요합니다.");
            }
        }

        if (isOpen)
        {
            door.position = Vector3.MoveTowards(
                door.position,
                openPosition,
                openSpeed * Time.deltaTime
            );
        }
    }
}