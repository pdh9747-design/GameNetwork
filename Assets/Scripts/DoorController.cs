
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    public Transform door;
    public float openDistance = 2f;
    public float openSpeed = 3f;
    public float interactDistance = 3f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpen = false;
    private PlayerInventory playerInventory;
    private Transform playerTransform;

    void Start()
    {
        if (door == null)
            door = transform;

        closedPosition = door.position;
        openPosition = closedPosition + Vector3.right * openDistance;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
            playerInventory = player.GetComponent<PlayerInventory>();
        }
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame && !isOpen)
        {
            if (playerTransform == null)
                return;

            float distance = Vector3.Distance(
                playerTransform.position,
                door.position
            );

            if (distance > interactDistance)
            {
                Debug.Log("문 가까이에서 E 키를 눌러 주세요.");
                return;
            }

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
