//#define PC
//#define Oculus
#define Vive
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if Vive
using Valve.VR;
using Valve.VR.Extras;
#endif

public static class ARAVRInput
{
#if PC
    public static bool isPC = true;
#else
    public static bool isPC = false;
    static Transform rootTransform;

#endif

#if PC
    public enum ButtonTarget
    {
        Fire1,
        Fire2,
        Fire3,
    }
#elif Vive
    public enum ButtonTarget
    {
        Teleport,
        InteractUI,
        GrabGrip,
        Jump,
    }
#endif

    public enum Button
    {
#if PC
        One = ButtonTarget.Fire1,
        Two = ButtonTarget.Fire2,
        Thumbstick = ButtonTarget.Fire1,
        IndexTrigger = ButtonTarget.Fire3,
        HandTrigger = ButtonTarget.Fire2
#elif Oculus
        One = OVRInput.Button.One,
        Two = OVRInput.Button.Two,
        Thumbstick = OVRInput.Button.PrimaryThumbstick,
        IndexTrigger = OVRInput.Button.PrimaryIndexTrigger,
        HandTrigger = OVRInput.Button.PrimaryHandTrigger
#elif Vive
        One = ButtonTarget.Teleport,
        Two = ButtonTarget.Jump,
        Thumbstick = ButtonTarget.Teleport,
        IndexTrigger = ButtonTarget.InteractUI,
        HandTrigger = ButtonTarget.GrabGrip,
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
#elif Vive
        LTouch = SteamVR_Input_Sources.LeftHand,
        RTouch = SteamVR_Input_Sources.RightHand,
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
#elif Vive
    static Transform GetTransform()
    {
        if (rootTransform == null)
        {
            rootTransform = GameObject.Find("[CameraRig]").transform;
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
#elif Vive
            Vector3 pos = RHand.position;
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
#elif Vive
            Vector3 direction = RHand.forward;
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
#elif Vive
            Vector3 pos = LHand.position;
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
#elif Vive
            Vector3 direction = LHand.forward;
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
#elif Vive
        get
        {
            if (lHand == null)
            {
                lHand = GameObject.Find("Controller (left)").transform;
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
#elif Vive
        get
        {
            if (rHand == null)
            {
                rHand = GameObject.Find("Controller (right)").transform;
            }
            return rHand;
        }
#endif

    }

    public static SteamVR_Input_Sources GetInputSource(Controller hand)
    {
        if (hand == Controller.LTouch)
        {
            return LHand.GetComponent<SteamVR_Behaviour_Pose>().inputSource;
        }
        else
        {
            return RHand.GetComponent<SteamVR_Behaviour_Pose>().inputSource;
        }
    }

    public static bool Get(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        return Input.GetButton(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.Get((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#elif Vive
        //return SteamVR_Actions._default.Teleport.state;
        return SteamVR_Input.GetState(((ButtonTarget)virtualMask).ToString(), GetInputSource(hand));
#endif
    }

    public static bool GetDown(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        //Debug.Log("button : " + ((ButtonTarget)virtualMask).ToString());
        return Input.GetButtonDown(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.GetDown((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#elif Vive
        //return SteamVR_Actions._default.Teleport.stateDown;
        //return SteamVR_Input.GetStateDown(((ButtonTarget)virtualMask).ToString(), (SteamVR_Input_Sources)hand);
        return SteamVR_Input.GetStateDown(((ButtonTarget)virtualMask).ToString(), GetInputSource(hand));
#endif
    }

    public static bool GetUp(Button virtualMask, Controller hand = Controller.RTouch)
    {
#if PC
        return Input.GetButtonUp(((ButtonTarget)virtualMask).ToString());
#elif Oculus
        return OVRInput.GetUp((OVRInput.Button)virtualMask, (OVRInput.Controller)hand);
#elif Vive
        //return SteamVR_Actions._default.Teleport.stateUp;
        return SteamVR_Input.GetStateUp(((ButtonTarget)virtualMask).ToString(), GetInputSource(hand));
#endif
    }


    public static float GetAxis(string axis)
    {
#if PC
        return Input.GetAxis(axis);
#elif Oculus

#elif Vive
        if (axis == "Horizontal")
        {
            return SteamVR_Input.GetVector2("TouchPad", GetInputSource(Controller.LTouch)).x;
        }
        else
        {
            return SteamVR_Input.GetVector2("TouchPad", GetInputSource(Controller.LTouch)).y;
        }
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
        PlayVibration(1, 150, 75, hand);
#endif
    }

    // 진동호출하기
    // duration : 반복횟수, frequency : 지속시간, amplify : 진동크기, hand : 왼쪽 혹은 오른쪽 컨트롤러
    public static void PlayVibration(int duration, int frequency, int amplitude, Controller hand)
    {
#if PC

#elif Oculus

#elif Vive
        SteamVR_Action_Vibration vib = SteamVR_Actions._default.Haptic;
        vib.Execute(0, duration, frequency, amplitude, (SteamVR_Input_Sources)hand);
#endif
    }
}
