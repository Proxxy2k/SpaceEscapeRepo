using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject TutorialMenu;
    public GameObject FinishMenu;

    //Debug slider References

    [Header("Player Attributes")]
    public Slider ForwardSpeedSlider;
    public Slider SideSpeedSlider;
    public Slider JumpSpeedSlider;
    public Slider ShootingRateSlider;
    public Toggle InvincibiltyToggle;

    [Header("Enemy Attributes")]
    public Slider EnemyHealth;
    public Slider EnemyShootingRateSlider;

    [Header("Camera Attributes")]
    public Slider XRotationSlider;
    public Slider DistanceSlider;

    [Header("Audio")]
    public AudioSource MainAudioSource;
    public AudioClip PlayerShoot;
    public AudioClip TurretShoot;
    public AudioClip TurretExplosion;
    public AudioClip PlayerKill;

    void Start()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //showing tutorial first time
        if(PlayerPrefs.GetInt("tutorial")==0)
        {
            Time.timeScale = 0;
            TutorialMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            TutorialMenu.SetActive(false);
        }
    }

    public void JumpButton()
    {
        PlayerScript.instance.Jump();
    }

    public void PauseButton()
    {
        Time.timeScale = 0;
    }

    public void PlaySfx(AudioClip clip)
    {
        MainAudioSource.PlayOneShot(clip,0.6f);
    }

    public void ResumeButton()
    {
        Time.timeScale = 1;
        PlayerScript.instance.ChangeValues();
    }

    public void RestartButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ShowFinishMenu()
    {
        Time.timeScale = 0;
        FinishMenu.SetActive(true);
    }

    public void TutorialResumeButton()
    {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("tutorial", 1);
    }

    public IEnumerator RestartScene(float RestartDelay)
    {
        yield return new WaitForSeconds(RestartDelay);
        SceneManager.LoadScene(0);
    }

    public void ResetParametersButton()
    {
        Time.timeScale = 1;
        //Player Attributes
        ForwardSpeedSlider.value = 2.5f;
        SideSpeedSlider.value = 2f;
        JumpSpeedSlider.value = 6000;
        ShootingRateSlider.value = 0.8f;
        InvincibiltyToggle.isOn = false;

        //Enemy Attributes
        EnemyHealth.value = 3;
        EnemyShootingRateSlider.value = 0.6f;

        //Camera Attributes
        XRotationSlider.value = 12;
        DistanceSlider.value = -0.74f;

        PlayerScript.instance.ChangeValues();
    }
}
