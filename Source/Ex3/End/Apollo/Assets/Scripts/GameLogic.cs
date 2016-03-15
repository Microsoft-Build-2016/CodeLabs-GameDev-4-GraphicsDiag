using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour {
    public Text missionTimeText;
    public Text pathIndicatorText;
    public Text missionReportText;
    public GameObject planetEarth;
    public GameObject earthEnvironment;
    public GameObject player;
    public GameObject moon;
    public RectTransform miniMapPosition;
    public List<Texture> starsImages;
    public List<Texture> starsImagesResources;
    public RawImage starsImageToDisplay;

    TimeSpan timeSpan;
    float timeInSpace = 0.0f;
    bool hasLaunched = false;
    float pathIndicator = 0.0f;
    RocketController playerHandler;
    Vector3 originalPlayerDotPosition;
    
    // Use this for initialization
    void Start () {
        timeSpan = TimeSpan.FromSeconds(-10.0);
        playerHandler = player.GetComponent<RocketController>();

        originalPlayerDotPosition = miniMapPosition.position;
    }
	

	// Update is called once per frame
	void Update () {
        timeSpan += TimeSpan.FromSeconds(Time.deltaTime);
        
        if (hasLaunched)
        {
            timeInSpace += Time.deltaTime;
        }

        OffsetMoon();
        SwitchEnvironment();
        IsGameOver();
        UpdateUI();
    }

    public void Launch()
    {
        hasLaunched = true;
    }

    void OffsetMoon()
    {

        Vector3 moonPos = moon.transform.position;
        moonPos.y = player.transform.position.y + 400.0f;
        moon.transform.position = moonPos;
    }
    void SwitchEnvironment()
    {

        if (timeInSpace > 20.0f)
        {
            if (earthEnvironment != null)
                Destroy(earthEnvironment);

            planetEarth.SetActive(true);
        }

    }

    void IsGameOver()
    {

        string status = "";

        if (playerHandler.isGameOver)
        {
            starsImageToDisplay.gameObject.SetActive(true);
            float points = 1000.0f - Mathf.Abs(pathIndicator) * 100.0f;

            if (points >= 980)
            {
                status = "PERFECT!";
                starsImageToDisplay.texture = starsImages[5];
            }

            if (points <= 980)
            {
                status = "GREAT!";
                starsImageToDisplay.texture = starsImages[4];
            }

            if (points <= 600)
            {
                status = "GOOD!";
                starsImageToDisplay.texture = starsImages[3];
            }

            if (points <= 500)
            {
                status = "OK!";
                starsImageToDisplay.texture = starsImages[2];
            }

            if (points <= 300)
            {
                status = "MEH!";
                starsImageToDisplay.texture = starsImages[1];
            }

            if (points <= 100)
            {
                status = "BAD!";
                starsImageToDisplay.texture = starsImages[1];
            }

            if (points <= 100)
            {
                status = "CATASTROPHIC!";
                starsImageToDisplay.texture = starsImages[0];
            }

            missionTimeText.text = "";
            missionTimeText.text = "LAUNCH POINTS\n" + points.ToString("0000");
        }

        missionReportText.text = status;
    }
    void UpdateUI()
    {
        if (!playerHandler.isGameOver)
        {
            pathIndicator = 0.0f - player.transform.position.x;
        }

        missionTimeText.text = string.Format("MISSION TIME\nT: {0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        pathIndicatorText.text = "[ " + (-1.0f * pathIndicator).ToString("0.00") + " ]";

        miniMapPosition.position = originalPlayerDotPosition + (Vector3.left * pathIndicator * 5.0f);
    }
}
