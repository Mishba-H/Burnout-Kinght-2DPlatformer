using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region VARIABLES

    private GameObject player;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float smoothTime;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    [SerializeField] private float idleYOffset;
    [SerializeField] private float xSmoothZone;
    [SerializeField] private float ySmoothZone;
    private Vector3 offset;

    #endregion

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    void FixedUpdate()
    {
        if (player == null) return;

        offset = new Vector3(xOffset * Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical") == 0 ? idleYOffset : yOffset * Input.GetAxisRaw("Vertical") , -10f);
        targetPosition = player.transform.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        
        //Smooth Zone conditions - player will not go out of these bounds
        if (player.transform.position.x > transform.position.x + xSmoothZone)
        {
            transform.position = new Vector3(player.transform.position.x - xSmoothZone, transform.position.y, -10f);
        }
        else if (player.transform.position.x < transform.position.x - xSmoothZone)
        {
            transform.position = new Vector3(player.transform.position.x + xSmoothZone, transform.position.y, -10f);
        }

        if (player.transform.position.y > transform.position.y + ySmoothZone)
        {
            transform.position = new Vector3(transform.position.x, player.transform.position.y - ySmoothZone, -10f);
        }
        else if (player.transform.position.y < transform.position.y - ySmoothZone)
        {
            transform.position = new Vector3(transform.position.x, player.transform.position.y + ySmoothZone, -10f);
        }
    }
}
