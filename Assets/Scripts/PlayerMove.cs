using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자의 입력에 따라 앞뒤좌우로 이동하고 싶다.
// 필요속성 : 이동속도, CharacterController 컴포넌트
//목표: 중력 적용시키고 싶다.
//필요속성 : 중력 가속도 크기, 수직속도
//목표: 점프를 하고 싶다.
//필요속성 : 점프크기
public class PlayerMove : MonoBehaviour
{
    // 이동속도
    public float speed = 5;
    // CharacterController 컴포넌트
    CharacterController cc;

    // 중력 가속도 크기
    public float gravity = -20;
    // 수직속도
    float yVelocity = 0;

    // 점프크기
    public float jumpPower = 5;
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    
    void Update()
    {
        // 사용자의 입력에 따라 앞뒤좌우로 이동하고 싶다.
        // 1. 사용자의 입력을 받는다.
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");
        float h = ARAVRInput.GetAxis("Horizontal");
        float v = ARAVRInput.GetAxis("Vertical");

        // 2. 방향을 만든다.
        Vector3 dir = new Vector3(h, 0, v);

        // 2.0 사용자가 바라보는 방향으로 입력 값을 변화 시키기
        dir = Camera.main.transform.TransformDirection(dir);

        // 2.1 중력 적용한 수직 방향 추가 v=v0+at
        yVelocity += gravity * Time.deltaTime;

        // 2.2 바닥에 있을 경우 수직항력처리를 위해 속도를 0으로 한다.
        if(cc.isGrounded)
        {
            yVelocity = 0;
        }

        // 2.3 사용자가 점프버튼을 누르면 속도에 점프크기를 할당한다.
        if(ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch))
        {
            yVelocity = jumpPower;
        }

        dir.y = yVelocity;

        // 3. 이동한다.
        cc.Move(dir * speed * Time.deltaTime);
    }
}
