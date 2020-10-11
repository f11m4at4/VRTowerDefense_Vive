using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

// 사용자가 발사 버튼을 누르면 총을 쏘고 싶다.
// 필요속성 : 총알 파편, 총알파편 효과, 총알 발사 사운드
public class Gun : MonoBehaviour
{
    public Transform bulletImpact; 	// 총알 파편 효과
    ParticleSystem bulletEffect;	// 총알 파편 ParticleSystem
    AudioSource bulletAudio;    // 총알 발사 사운드

    Transform explosion;	// drone 폭발 효과
    ParticleSystem explosionEffect;	// 폭발

    public Transform crosshair;	// crosshair 를 위한 속성

    void Start()
    {
        // 총알 효과 파티클시스템 컴포넌트 가져오기
        bulletEffect = bulletImpact.GetComponent<ParticleSystem>();
        // 총알 효과 오디오소스 컴포넌트 가져오기
        bulletAudio = bulletImpact.GetComponent<AudioSource>();

        // 폭발 효과 있을 때 파티클시스템 컴포넌트 가져오기
        if (explosion)
        {
            explosionEffect = explosion.GetComponent<ParticleSystem>();
        }
    }
    void Update()
    {
        // 크로스헤어 표시
        ARAVRInput.DrawCrosshair(crosshair);

        // 사용자가 IndexTrigger 버튼을 누르면
        if (ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger))
        {
            // 컨트롤러의 진동 재생
            ARAVRInput.PlayVibration(ARAVRInput.Controller.RTouch);

            // 총알 오디오 재생
            bulletAudio.Stop();
            bulletAudio.Play();

            // Ray 를 카메라의 위치로 부터 나가도록 만든다.
            Ray ray = new Ray(ARAVRInput.RHandPosition, ARAVRInput.RHandDirection);
            // Ray 의 충돌정보를 저장하기 위한 변수 지정
            RaycastHit hitInfo;
            // 플레이어 레이어 얻어오기
            int playerLayer = 1 << LayerMask.NameToLayer("Player");
            // 타워 레이어 얻어오기
            int towerLayer = 1 << LayerMask.NameToLayer("Tower");
            int layerMask = playerLayer | towerLayer;
            // Ray 를 쏜다. ray 가 부딪힌 정보는 hitInfo 에 담긴다.
            if (Physics.Raycast(ray, out hitInfo, 200, ~layerMask))
            {
                // 총알파편효과 처리
                // 총알 이펙트 진행되고 있으면 멈추고 재생
                bulletEffect.Stop();
                bulletEffect.Play();
                // 부딪힌 지점의 방향으로 총알 이펙트 방향을 설정
                bulletImpact.up = hitInfo.normal;
                // 부딪힌 지점 바로 위에서 이펙트가 보여지도록 설정
                bulletImpact.position = hitInfo.point;

                // ray 와 부딪힌 객체가 drone 이라면 폭발효과 처리
                if (hitInfo.transform.name.Contains("Drone"))
                {
                    // 폭발 효과가 있다면
                    if (explosion)
                    {
                        // 드론위치에 폭발효과 놓기
                        explosion.position = hitInfo.transform.position;
                        // 폭발 효과 재생
                        explosionEffect.Stop();
                        explosionEffect.Play();
                        // 폭발 오디오 재생
                        explosion.GetComponent<AudioSource>().Stop();
                        explosion.GetComponent<AudioSource>().Play();

                    }
                    // 드론 제거
                    Destroy(hitInfo.transform.gameObject);
                }

            }

        }

    }

}
