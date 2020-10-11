﻿#define PC
//#define Oculus
//#define Vive
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class ARAVRInput
{
#if PC
    public static bool isPC = true;
#else
    public static bool isPC = false;
    static Transform rootTransform;

#endif

#if PC
    public enum ButtonPC
    {
        Fire1,
        Fire2,
        Fire3,
    }
#endif

    public enum Button
    {
#if PC
        One = ButtonPC.Fire1,
        Two = ButtonPC.Fire2,
        Thumbstick = ButtonPC.Fire1,
        IndexTrigger = ButtonPC.Fire3,
        HandTrigger = ButtonPC.Fire2
#elif Oculus
        One = OVRInput.Button.One,
        Two = OVRInput.Button.Two,
        Thumbstick = OVRInput.Button.PrimaryThumbstick,
        IndexTrigger = OVRInput.Button.PrimaryIndexTrigger,
        HandTrigger = OVRInput.Button.PrimaryHandTrigger
#endif
    }

    public enum Controller
    {
#if PC
        LTouch = 0,
        RTouch = 1
#elif Oculus
        LTouch = OVRInput.Controller.LTouch,
        RTouch = OVRInput.Controller.RTouch
#endif
    }

#if Oculus
    static Transform GetTransform()
    {
        if (rootTransform == null)
        {
            rootTransform = GameObject.Find("TrackingSpace").transform;
        }

        return rootTransform;
    }
#endif

    public static Vector3 RHandPosition
    {
        get
        {
#if PC
            /*
            Plane plane = new Plane(Vector3.up, 0);
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(r, out distance))
            {
                return r.GetPoint(distance);

            }
            */
            Vector3 pos = Input.mousePosition;
            pos.z = Camera.main.nearClipPlane + 0.01f;
            pos = Camera.main.ScreenToWorldPoint(pos);

            

            return pos;
#elif Oculus
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            pos = GetTransform().TransformPoint(pos);
            return pos;                
#endif
        }
    }

    public static Vector3 RHandDirection
    {
        get
        {
#if PC
            Vector3 direction = RHandPosition - Camera.main.transform.position;
            return direction;
#elif Oculus
            Vector3 direction = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
            direction = GetTransform().TransformDirection(direction);
            return direction;
#endif
        }
    }

    public static Vector3 LHandPosition
    {
        get
        {
#if PC
            /*
            Plane plane = new Plane(Vector3.up, 0);
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(r, out distance))
            {
                return r.GetPoint(distance);

            }
            */
            Vector3 pos = Input.mousePosition;
            pos.z = Camera.main.nearClipPlane + 0.01f;
            pos = Camera.main.ScreenToWorldPoint(pos);
            return pos;
#elif Oculus
            Vector3 pos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            pos = GetTransform().TransformPoint(pos);
            return pos;                
#endif
        }
    }

    public static Vector3 LHandDirection
    {
        get
        {
#if PC
            Vector3 direction = LHandPosition - Camera.main.transform.position;
            return direction;
#elif Oculus
            Vector3 direction = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch) * Vector3.forward;
            direction = GetTransform().TransformDirection(direction);
            return direction;
#endif
        }
    }

    static Transform lHand;
    static Transform rHand;
    public static Transform LHand
    {

#if PC
        get
        {
            if(lHand == null)
            {
                GameObject handObj = new GameObject("LHand");
                lHand = handObj.transform;
                lHand.position = LHandPosition;
                lHand.forward = LHandDirection;
                lHand.parent = Camera.main.transform;
            }
            return lHand;
        }
#elif Oculus
        get
        {
            if (lHand == null)
            {
                lHand = GameObject.Find("LeftControllerAnchor").transform;
            }
            return lHand;
        }
#endif

    }

    public static Transform RHand
    {

#if PC
        get
        {
            if (rHand == null)
            {
                GameObject handObj = new GameObject("RHand");
                rHand = handObj.transform;
                rHand.position = RHandPosition;
                rHand.forward = RHandDirection;
                rHand.parent = Camera.main.transform;
            }
            return rHand;
        }
#elif Oculus
        get
        {
            if (rHand == null)
            {
                rHand = GameObject.Find("RightControllerAnchor").transform;
            }
            return rHand;
        }
#endif

    }

    public static bool Get(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        return Input.GetButton(((ButtonPC)virtualMask).ToString());
#elif Oculus
        return OVRInput.Get((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#endif
    }

    public static bool GetDown(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        //Debug.Log("button : " + ((ButtonPC)virtualMask).ToString());
        return Input.GetButtonDown(((ButtonPC)virtualMask).ToString());
#elif Oculus
        return OVRInput.GetDown((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#endif
    }

    public static bool GetUp(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        return Input.GetButtonUp(((ButtonPC)virtualMask).ToString());
#elif Oculus
        return OVRInput.GetUp((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#endif
    }


#if PC
    static Vector3 originScale = Vector3.one * 0.02f;
#else
    static Vector3 originScale = Vector3.one * 0.005f;
#endif
    public static void DrawCrosshair(Transform crosshair, bool isHand = true, Controller hand = Controller.RTouch)
    {
        if(crosshair == null)
        {
            return;
        }

        Ray ray;
        // 컨트롤러의 위치와 방향을 이용하여 Ray 제작
        if (isHand)
        {
#if PC
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#else
            if(hand == Controller.RTouch)
            {
                ray = new Ray(RHandPosition, RHandDirection);
            }
            else
            {
                ray = new Ray(LHandPosition, LHandDirection);
            }
#endif
        }
        else
        {
            ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        }
        // 눈에 안보이는 Plane 을 만든다.
        Plane plane = new Plane(Vector3.up, 0);
        float distance = 0;
        // plane 을 이용하여 ray 를 쏜다.
        if (plane.Raycast(ray, out distance))
        {
            // ray 의 GetPoint 함수를 이용하여 충돌 지점의 위치를 가져온다.
            crosshair.position = ray.GetPoint(distance);
            crosshair.forward = -Camera.main.transform.forward;
            // 크로스헤어의 크기를 최소 기본 크기에서 거리에 따라 더 커지도록 한다.
            crosshair.localScale = originScale * Mathf.Max(1, distance);
        }
        else
        {
            crosshair.position = ray.origin + ray.direction * 100;
            crosshair.forward = -Camera.main.transform.forward;
            distance = (crosshair.position - ray.origin).magnitude;
            crosshair.localScale = originScale * Mathf.Max(1, distance);
        }
    }

    // 진동 호출 하기
    static AudioClip vibrationClip = null;
    public static void PlayVibration(Controller hand)
    {
#if PC

#elif Oculus

#elif Vive

#endif
    }

    // 진동호출하기
    // duration : 반복횟수, frequency : 지속시간, amplify : 진동크기, hand : 왼쪽 혹은 오른쪽 컨트롤러
    public static void PlayVibration(int duration, int frequency, int amplify, Controller hand)
    {
#if PC

#elif Oculus

#elif Vive

#endif
    }
}
