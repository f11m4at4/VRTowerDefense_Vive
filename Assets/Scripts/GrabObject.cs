﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 폭탄을 잡고 싶다.
public class GrabObject : MonoBehaviour
{
    //필요속성 : 물체잡고 있는지 여부, 잡고 있는 물체, 잡을 물체 종류, 잡을 거리
    //물체잡고 있는지 여부
    bool isGrabbing = false;
    //잡고 있는 물체
    GameObject grabbedObject;
    //잡을 물체 종류
    public LayerMask grabbedLayer;
    //잡을 거리
    public float grabRange = 0.2f;
    // 이전위치
    Vector3 prevPos;
    // 던질 힘
    public float throwPower = 10;

    // 이전회전
    Quaternion prevRot;
    // 회전력
    public float rotPower = 5;
    void Update()
    {
        // 물체를 잡고 싶다.
        // 1. 물체를 안잡고 있을 경우
        if (isGrabbing == false)
        {
            // 잡기 시도
            TryGrab();
        }
        else
        {
            // 물체 놓기
            TryUngrab();
        }
    }
    private void TryGrab()
    {
        // Grab 버튼을 누르면 일정영역안에 있는 폭탄을 잡는다.
        // 1. grab 버튼을 눌렀다면
        if (ARAVRInput.GetDown(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            int closest = 0;

            // 2. 일정영역 안에 폭탄이 있으니까
            // - 영역안에 있는 모든 폭탄 검출
            Collider[] hitObjects = Physics.OverlapSphere(ARAVRInput.RHandPosition, grabRange, grabbedLayer);
            // - 손과 가장 가까운 물체 선택
            for (int i = 1; i < hitObjects.Length; i++)
            {
                // 손과 가장 가까운 물체와의 거리
                Vector3 closestPos = hitObjects[closest].transform.position;
                float closestDistance = Vector3.Distance(closestPos, ARAVRInput.RHandPosition);
                // 다음 물체와 손과의 거리
                Vector3 nextPos = hitObjects[i].transform.position;
                float nextDistance = Vector3.Distance(nextPos, ARAVRInput.RHandPosition);
                // 다음 물체와의 거리가 더 가깝다면 
                if (nextDistance < closestDistance)
                {
                    // 가장가까운 물체 인덱스 교체
                    closest = i;
                }
            }
            // 3. 폭탄을 잡는다.
            // - 검출된 물체가 있을 경우
            if (hitObjects.Length > 0)
            {
                // 잡은 상태로 전환
                isGrabbing = true;
                // 잡은 물체기억
                grabbedObject = hitObjects[closest].gameObject;
                // 잡은 물체를 손의 자식으로 등록
                grabbedObject.transform.parent = ARAVRInput.RHand;

                // 물리기능 정지
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                // 초기 위치값 지정
                prevPos = ARAVRInput.RHand.position;
                // 초기 회전 값 지정
                prevRot = ARAVRInput.RHand.rotation;
            }
        }
    }

    private void TryUngrab()
    {
        // 던질 방향
        Vector3 throwDirection = (ARAVRInput.RHand.position - prevPos);
        // 위치 기억
        prevPos = ARAVRInput.RHand.position;

        // 쿼터니온 공식
        // angle1 = Q1, angle2 = Q2
        // angle1 + angle2 = Q1 * Q2
        // -angle2 = Quaternion.Inverse(Q2)
        // angle2 - angle1 = Quaternion.FromToRotation(Q1, Q2) = Q2 * Quaternion.Inverse(Q1)
        // 회전 방향 = current - previous 의 차 로 구함 - previous 는 Inverse 로 구함
        Quaternion deltaRotation = ARAVRInput.RHand.rotation * Quaternion.Inverse(prevRot);
        // 이전 회전 저장
        prevRot = ARAVRInput.RHand.rotation;

        // 버튼을 놓았다면
        if (ARAVRInput.GetUp(ARAVRInput.Button.HandTrigger, ARAVRInput.Controller.RTouch))
        {
            // 잡지 않은 상태로 전환
            isGrabbing = false;
            // 물리기능 활성화
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            // 손에서 폭탄 떼어내기
            grabbedObject.transform.parent = null;
            // 던지기
            grabbedObject.GetComponent<Rigidbody>().velocity = throwDirection * throwPower;
            // 각속도 = (1/dt) * dθ(특정축 기준 변위각도)
            float angle;
            Vector3 axis;
            deltaRotation.ToAngleAxis(out angle, out axis);
            Vector3 angularVelocity = (1.0f / Time.deltaTime) * angle * axis;
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = angularVelocity;

            // 잡은 물체 없도록 설정
            grabbedObject = null;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ARAVRInput.RHandPosition, grabRange);

        //if (isThrowed)
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawLine(lastPosition, lastPosition + throwDirection * throwPower);
        //    print("lastPosition : " + lastPosition + ", " + throwDirection);
        //}
    }
}
