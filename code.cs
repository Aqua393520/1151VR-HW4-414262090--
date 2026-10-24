using UnityEngine;

public class FlightController : MonoBehaviour
{
    public float moveSpeed = 10f;   // 基礎飛行速度
    public float boostSpeed = 22f;  // 【新增】按住加速時的速度
    public float steerSpeed = 5f;   // 轉向靈敏度

    private Rigidbody rb;

    [Header("音效設定")]
    public AudioSource engineAudio;   // 飛行持續音效
    public AudioSource hitAudio;      // 撞到障礙物音效
    public AudioSource passAudio;     // 通過與終點音效

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // 避免飛機因重力掉下去
        }

        // 播放飛行持續音效並設定為循環
        if (engineAudio != null)
        {
            engineAudio.loop = true;
            engineAudio.Play();
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // 取得左右與上下鍵輸入
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // 【新增】預設使用普通速度，如果按住 Left Shift 鍵就切換成加速速度
            float currentSpeed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = boostSpeed;
            }

            // 計算移動方向
            Vector3 moveDir = new Vector3(horizontalInput, verticalInput, 1f).normalized;

            // 讓飛機透過當前速度前進
            rb.linearVelocity = transform.TransformDirection(moveDir) * currentSpeed;
        }
    }

    // 實體碰撞偵測：撞到障礙物
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("obstacle"))
        {
            Debug.Log("撞到障礙物！");
            if (hitAudio != null) hitAudio.Play();
        }
    }

    // 觸發器偵測：順利通過障礙物或抵達終點
    private void OnTriggerEnter(Collider other)
    {
        // 通過一般障礙物感應區
        if (other.CompareTag("PassTrigger"))
        {
            Debug.Log("順利通過障礙物！");
            if (passAudio != null) passAudio.Play();
        }
        // 抵達終點線
        else if (other.CompareTag("Finish"))
        {
            Debug.Log("抵達終點！遊戲結束！");
            if (passAudio != null) passAudio.Play();

            // 停下飛機速度
            moveSpeed = 0;
            boostSpeed = 0;
        }
    }
}