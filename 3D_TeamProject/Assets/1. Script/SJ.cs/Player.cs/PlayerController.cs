using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    #region | public 변수 | ======================================

    [Header("플레이어 이동")]
    public float moveSpeed = 5f;
    public float jumpPower = 8f;
    public float gravity = -40f;

    [Header("마우스")]
    public float mouseSensitivity = 0.2f;

    [Header("카메라")]
    public Transform cameraPivot;
    public Transform cameraTransform;

    [Header("카메라 줌")]
    public float cameraDistance = -3f;      // 사용자가 설정한 카메라 거리
    public float minDistance = 0f;          // 1인칭
    public float maxDistance = -10f;         // 3인칭 최대  거리
    public float zoomSpeed = 1f;            // 휠 한 번당 이동 거리
    public float zoomSmooth = 10f;          // 원래 거리로 복귀하는 속도

    [Header("카메라 충돌")]
    public LayerMask cameraCollisionLayer;

    // 이 거리보다 벽이 가까우면 강제로 1인칭
    public float firstPersonWallDistance = 0.5f;

    // 카메라와 벽 사이에 확보할 거리
    public float cameraWallOffset = 0.2f;

    // Ray 시작점이 벽 안에 들어갔는지 검사하는 크기
    public float wallCheckRadius = 0.1f;

    [Header("플레이어 그래픽")]
    public GameObject playerMesh;
    #endregion



    #region | private 변수 | ======================================

    private float pitch = 20f;
    private float verticalVelocity;

    private bool isRunning;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private CharacterController controller;

    private float zoomInput;

    #endregion



    #region InputSystem

    public void OnMove(InputValue value)        // 움직임
    {
        moveInput = value.Get<Vector2>();
    }


    public void OnJump(InputValue value)        // 점프
    {
        if (value.isPressed && controller.isGrounded)
        {
            verticalVelocity = jumpPower;
        }
    }

    public void OnSprint(InputValue value)      // 달리기 유무
    {
        isRunning = value.isPressed;
    }


    public void OnLook(InputValue value)        // 카메라 시아
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnZoom(InputValue value)        // 카메라 줌
    {
        zoomInput = value.Get<float>();
    }

    #endregion



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        PlayerMovement();

        CameraRotation();

        CameraZoom();

        CameraCollision();
    }



    #region 플레이어 이동

    private void PlayerMovement()
    {
        // 땅에 있을 때 중력 초기화
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;

        float speed = moveSpeed;

        // 달리기
        if (isRunning)
        {
            speed = moveSpeed * 2f;
        }

        move *= speed;

        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    #endregion



    #region 카메라 회전

    private void CameraRotation()
    {
        // 좌우 회전
        transform.Rotate(0f, lookInput.x * mouseSensitivity, 0f);

        // 상하 회전
        pitch -= lookInput.y * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -20f, 60f);

        cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }

    #endregion



    #region 카메라 줌

    private void CameraZoom()
    {
        if (zoomInput != 0f)
        {
            cameraDistance += Mathf.Sign(zoomInput) * zoomSpeed;

            cameraDistance = Mathf.Clamp(cameraDistance, maxDistance, minDistance);
        }

        // 줌 + 달리기 효과가 적용된 카메라 목표 거리
        float targetZ = GetCameraRunEffect();

        Vector3 camPos = cameraTransform.localPosition;

        // 장애물이 없으면 목표 거리까지 부드럽게 이동
        camPos.z = Mathf.Lerp(camPos.z, targetZ, zoomSmooth * Time.deltaTime);

        cameraTransform.localPosition = camPos;

        //1인칭 효과.
        FirstPersonEffect(camPos);
    }

    #endregion



    #region Addition. 카메라 시아 가림 방지.

    private void CameraCollision()
    {
        float targetZ = GetCameraRunEffect();

        float desiredDistance = Mathf.Abs(targetZ);

        Vector3 start = cameraPivot.position;
        Vector3 direction = -cameraPivot.forward;

        // 기본값 = 원래 가고 싶은 카메라 거리
        float finalZ = targetZ;

        // Pivot이 벽과 겹쳤는지 검사
        bool insideWall = Physics.CheckSphere(start, wallCheckRadius, cameraCollisionLayer, QueryTriggerInteraction.Ignore);

        if (insideWall)
        {
            finalZ = 0f;        // 카메라를 둘 공간이 없으므로 1인칭
        }
        else
        {
            // 카메라 뒤쪽 장애물 검사
            if (Physics.Raycast(start, direction, out RaycastHit hit, desiredDistance, cameraCollisionLayer, QueryTriggerInteraction.Ignore))        
            {
                float possibleDistance = hit.distance - cameraWallOffset;       // 벽까지 확보 가능한 거리

                if (possibleDistance <= firstPersonWallDistance)
                {
                    finalZ = 0f;        // 벽이 너무 가까움? --> 1인칭.
                }
                else
                {
                    finalZ = -possibleDistance;         // 공간이 있음? --> 벽 앞까지.
                }
            }
        }

        if (finalZ != targetZ)
        {
            Vector3 camPos = cameraTransform.localPosition;

            // 벽 관통 방지를 위해 즉시 이동
            camPos.z = finalZ;

            cameraTransform.localPosition = camPos;

            //1인칭 효과.
            FirstPersonEffect(camPos);
        }

        // Scene 디버깅용 DrawRay
        Debug.DrawRay(start, direction * desiredDistance, finalZ != targetZ ? Color.red : Color.green);
    }

    #endregion



    #region Addition. 카메라 달리기 효과

    private float GetCameraRunEffect()
    {
        float targetZ = cameraDistance;

        // 달리는 중이고 3인칭이라면 카메라를 조금 뒤로
        if (isRunning && cameraDistance < 0f)
        {
            targetZ -= 2f;
        }

        return targetZ;
    }

    #endregion



    #region Addition. 카메라 1인칭 효과

    private void FirstPersonEffect(Vector3 camPos)
    {
        // 1인칭이면 Mesh를 모습을 비활성화 해서 1인칭 시점으로 보이게 끔.
        bool isFirstPerson = Mathf.Abs(camPos.z) <= 0.1f;

        if (playerMesh != null)
        {
            playerMesh.SetActive(!isFirstPerson);
        }
    }

    #endregion
}