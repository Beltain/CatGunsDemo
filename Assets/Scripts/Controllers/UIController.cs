using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
    //Overlay Panels
    [Space(10)]
    [Header("Overlay Panels")]
    protected GameObject[] uiPanels;
    [SerializeField] protected GameObject mainMenuUIPanel;
    [SerializeField] protected GameObject gameUIPanel;
    [SerializeField] protected GameObject pauseUIPanel;
    [SerializeField] protected GameObject gameOverUIPanel;

    //Inventory UI
    [Space(10)]
    [Header("Inventory UI")]
    [SerializeField] public static UIController uiController;
    //public Transform[] inventoryUI;
    [SerializeField] public Transform itemPanel;

    //Status UI
    [Space(10)]
    [Header("Status UI")]
    [SerializeField] protected List<GameObject> statusUIs = new List<GameObject>();
    [SerializeField] protected GameObject statusUIPrefab;
    [SerializeField] protected Transform statusPanel;

    //Alert UI
    [Space(10)]
    [Header("Alert UI")]
    [SerializeField] protected List<GameObject> alertUIs = new List<GameObject>();
    [SerializeField] protected GameObject alertUIPrefab;
    [SerializeField] protected Transform alertUIPanel;
    public static string parriedMessage = "Parried!";
    public static string boostedMessage = "Boosted!";

    //PowerUp UI
    [Space(10)]
    [Header("Powerup UI")]
    [SerializeField] public PowerUpUI HealthPUButton;
    [SerializeField] public PowerUpUI StrengthPUButton;
    [SerializeField] public PowerUpUI SpeedPUButton;
    [SerializeField] public PowerUpUI StaminaPUButton;

    //Game Over
    [Space(10)]
    [Header("Game Over UI")]
    [SerializeField] protected float waitDurationGeneric = 0.2f;

    [SerializeField] protected GameObject scroll;
    [SerializeField] protected Text scrollTitle;
    [SerializeField] protected string[] winLoseTitles = { "Victory", "Defeat" };
    [SerializeField] protected Transform scrollCentre;
    [SerializeField] protected float scrollHeight = 450f;
    [SerializeField] protected float scrollTravelInDuration = 1f;
    [SerializeField] protected float scrollPauseDuration = 0.5f;
    [SerializeField] protected float scrollOpenDuration = 1f;

    [SerializeField] protected Text bountyTitle;
    [SerializeField] protected Transform bountyStartPos;
    [SerializeField] protected List<GameObject> spawnedBountyIcons = new List<GameObject>();
    [SerializeField] protected float spacingBetweenBountyIcons = 75f;
    [SerializeField] protected float timeBetweenBountyIcons = 0.6f;
    [SerializeField] protected bool bountiesCentered = false;

    [SerializeField] protected GameObject rewardSection;
    [SerializeField] protected Text rewardText;
    [SerializeField] protected string rewardCurrencySymbol = "$";
    [SerializeField] protected float rewardCountTime = 1f;

    [SerializeField] protected GameObject[] buttons;
    IEnumerator GameOverUISequencer;

    //IEnumerator dragSequencer;

    private void Awake()
    {
        uiController = this;
        SetupUIPanels();
    }

    #region Power Up UI

    public void SetCooldownUI(GameController.PowerUp powerUp, float amount)
    {
        switch (powerUp)
        {
            case (GameController.PowerUp.stamina):
                StaminaPUButton.SetCooldown(amount);
                break;
            case (GameController.PowerUp.health):
                HealthPUButton.SetCooldown(amount);
                break;
            case (GameController.PowerUp.speed):
                SpeedPUButton.SetCooldown(amount);
                break;
            case (GameController.PowerUp.strength):
                StrengthPUButton.SetCooldown(amount);
                break;
            default:
                break;
        }
    }

    public void ResetPowerUpUI()
    {
        StrengthPUButton.SetPressable(false);
        StrengthPUButton.SetCounter(0);
        StrengthPUButton.SetCooldown(0);
        HealthPUButton.SetPressable(false);
        HealthPUButton.SetCounter(0);
        HealthPUButton.SetCooldown(0);
        StaminaPUButton.SetPressable(false);
        StaminaPUButton.SetCounter(0);
        StaminaPUButton.SetCooldown(0);
        SpeedPUButton.SetPressable(false);
        SpeedPUButton.SetCounter(0);
        SpeedPUButton.SetCooldown(0);
    }


    public void UpdatePowerUpUI()
    {
        int healthPowerUpCount = 0;
        int staminaPowerUpCount = 0;
        int speedPowerUpCount = 0;
        int strengthPowerUpCount = 0;

        StrengthPUButton.SetPressable(false);
        StrengthPUButton.SetCounter(0);
        HealthPUButton.SetPressable(false);
        HealthPUButton.SetCounter(0);
        StaminaPUButton.SetPressable(false);
        StaminaPUButton.SetCounter(0);
        SpeedPUButton.SetPressable(false);
        SpeedPUButton.SetCounter(0);

        foreach (GameController.PowerUp powerUp in GameController.gameController.allyPowerUps)
        {
            switch (powerUp)
            {
                case (GameController.PowerUp.stamina):
                    staminaPowerUpCount++;
                    StaminaPUButton.SetPressable(true);
                    StaminaPUButton.SetCounter(staminaPowerUpCount);
                    break;
                case (GameController.PowerUp.health):
                    healthPowerUpCount++;
                    HealthPUButton.SetPressable(true);
                    HealthPUButton.SetCounter(healthPowerUpCount);
                    break;
                case (GameController.PowerUp.speed):
                    speedPowerUpCount++;
                    SpeedPUButton.SetPressable(true);
                    SpeedPUButton.SetCounter(speedPowerUpCount);
                    break;
                case (GameController.PowerUp.strength):
                    strengthPowerUpCount++;
                    StrengthPUButton.SetPressable(true);
                    StrengthPUButton.SetCounter(strengthPowerUpCount);
                    break;
                default:
                    break;
            }
        }
    }

    #endregion

    #region UIPanelSelect
    void SetupUIPanels()
    {
        GameObject[] allPanels = { mainMenuUIPanel, gameUIPanel, gameOverUIPanel, pauseUIPanel };
        uiPanels = allPanels;
    }


    public void SetUIPanel(string uiPanelName)
    {
        //Set all UiPanels to innactive
        foreach (GameObject uiPanel in uiPanels)
        {
            if(uiPanel.activeInHierarchy) uiPanel.SetActive(false);
        }

        //Set the one requested to active
        switch (uiPanelName)
        {
            case ("MainMenu"):
                mainMenuUIPanel.SetActive(true);
                break;
            case ("Game"):
                gameUIPanel.SetActive(true);
                break;
            case ("Paused"):
                pauseUIPanel.SetActive(true);
                break;
            case ("GameOver"):
                gameOverUIPanel.SetActive(true);
                break;
            default:
                Debug.Log("UI Panel Name does not exist, cannot switch UI");
                break;
        }
    }


    #endregion

    #region Inventory UI

   /* public void StartMovePowerUp(Transform powerUpToMove)
    {
        if(dragSequencer == null && GameController.gamePlayable)
        {
            dragSequencer = DragSequence(powerUpToMove);
            StartCoroutine(dragSequencer);
        }
    }

    public void StopMovePowerUp()
    {
        StopCoroutine(dragSequencer);
        dragSequencer = null;
    }

    IEnumerator DragSequence(Transform powerUpToMove)
    {
        //Set starting state
        GameController.gameNavicable = false;
        GameController.unitsSelectable = false;
        Vector3 origin = powerUpToMove.position;

        //Do while Dragging
        while (Input.touchCount == 1)
        {
            powerUpToMove.position = Input.mousePosition;
            yield return null;
        }

        //Shoot a ray on the selectable layer and if it finds a selectable object, apply the powerup
        int selectableLayermask = 1 << 8;
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, 1000f, selectableLayermask))
        {
            if(hitInfo.collider.gameObject.GetComponent<Unit>() != null)
            {
                if(hitInfo.collider.gameObject.GetComponent<Unit>().teamIndex == CombatController.combatController.combat.playerTeamIndex)
                {
                    powerUpToMove.gameObject.GetComponent<PowerUp>().PowerUpUnit(hitInfo.collider.gameObject.GetComponent<Unit>());
                }
            }
        }

        //Reset state
        GameController.gameNavicable = true;
        GameController.unitsSelectable = true;
        powerUpToMove.position = origin;
        StopMovePowerUp();
    } */


   /* public void UpdateInventoryUI()
    {
        //Refresh the position of the inventory items on the UI
        for( int i = 0; i < GameController.gameController.allyPowerUps.Count; i++)
        {
            GameController.gameController.allyPowerUps[i].transform.position = inventoryUI[i].position;
        }
    }*/

    #endregion

    #region Status UI

    public void GenerateStatusUI()
    {
        foreach (GameObject unit in GameController.gameController.activeUnits)
        {
            GameObject statusUI = Instantiate(statusUIPrefab, statusPanel);
            statusUI.GetComponent<StatusUI>().unitToTrack = unit.transform;
            statusUIs.Add(statusUI);
        }
    }

    public void RemoveStatusUI(StatusUI statusUIToRemove)
    {
        statusUIs.Remove(statusUIToRemove.gameObject);
        Destroy(statusUIToRemove.gameObject);
    }

    public void ChangeStatusScaleUIToMatchZoom()
    {
        float startZoom = CameraController.cameraController.startZoom;
        float currentZoom = Camera.main.orthographicSize;

        foreach(GameObject statusUI in statusUIs)
        {
            statusUI.transform.localScale = Vector3.one * (startZoom / currentZoom);
        }
    }

    #endregion

    #region Alert UI

    bool justattacked = false;
    public void PopupAlertUI(string alert, Vector3 worldPos)
    {
        GameObject alertUIToUse = AddAlertUI();
        alertUIToUse.transform.position = Camera.main.WorldToScreenPoint(worldPos);
        alertUIToUse.GetComponent<AlertTextUI>().DisplayAlert(alert);
    }

    GameObject AddAlertUI()
    {
        GameObject alertUIToUse = null;
        foreach(GameObject alertUI in alertUIs)
        {
            if (!alertUI.activeInHierarchy) alertUIToUse = alertUI;
        }
        if(alertUIToUse == null)
        {
            alertUIToUse = Instantiate(alertUIPrefab, alertUIPanel);
            alertUIs.Add(alertUIToUse);
        }
        alertUIToUse.SetActive(true);
        return alertUIToUse;
    }

    #endregion

    #region Game Over Handling

    void RefreshGameOverUI()
    {
        //Reset all variables from last time
        foreach(GameObject icon in spawnedBountyIcons) Destroy(icon);
        spawnedBountyIcons = new List<GameObject>();
        scrollTitle.text = "";
        bountyTitle.gameObject.SetActive(false);
        rewardText.text = rewardCurrencySymbol + "0";
        rewardSection.gameObject.SetActive(false);
        //pop up buttons
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }


    }

    public void InitiateGameOverUI(bool winState)
    {
        RefreshGameOverUI();

        //Play the UI coroutine for winning/losing the game
        if (GameOverUISequencer != null) StopCoroutine(GameOverUISequencer);
        GameOverUISequencer = GameOverUISequence(winState);
        StartCoroutine(GameOverUISequencer);
    }

    IEnumerator GameOverUISequence(bool winState)
    {
        //Display win/lose title
        if (winState) scrollTitle.text = winLoseTitles[0];
        else scrollTitle.text = winLoseTitles[1];

        //Move scroll in, pause for a lil and then open it
        yield return PlayScrollIntroSequence();

        //Show bounty text
        yield return new WaitForSeconds(waitDurationGeneric);
        bountyTitle.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeBetweenBountyIcons);

        //Show bounty
        yield return PlayBountySequence();

        yield return new WaitForSeconds(waitDurationGeneric);
        rewardSection.gameObject.SetActive(true);
        yield return new WaitForSeconds(waitDurationGeneric);

        //Play reward sequence
        yield return PlayRewardCount();
        yield return new WaitForSeconds(waitDurationGeneric);

        //pop up buttons
        foreach(GameObject button in buttons)
        {
            button.SetActive(true);
            yield return new WaitForSeconds(waitDurationGeneric);
        }

        yield return null;
    }

    List<Vector3> GetBountyPositions(List<GameObject> bountyIcons, bool isCentered)
    {
        List<Vector3> bountyPositions = new List<Vector3>();
        Vector3 curPos = bountyStartPos.position;
        if (isCentered) curPos -= new Vector3((spacingBetweenBountyIcons * (bountyIcons.Count - 1))/2f, 0f, 0f);
        foreach (GameObject bountyIcon in bountyIcons)
        {
            bountyPositions.Add(curPos);
            curPos += new Vector3(spacingBetweenBountyIcons, 0f, 0f);
        }
        return bountyPositions;
    }

    IEnumerator PlayBountySequence()
    {
        List<GameObject> bountyIcons = GameController.gameController.slainEnemyIcons;
        if (bountyIcons.Count > 0)
        {
            List<Vector3> bountyPositions = GetBountyPositions(bountyIcons, bountiesCentered);
            for (int i = 0; i < bountyIcons.Count; i++)
            {
                //Spawn a bounty icon and add it to the list of active ones/move it into place
                GameObject curBount = Instantiate(bountyIcons[i], bountyStartPos);
                curBount.transform.position = bountyPositions[i];
                spawnedBountyIcons.Add(curBount);
                yield return new WaitForSeconds(timeBetweenBountyIcons);
            }
        }
    }

    IEnumerator PlayScrollIntroSequence()
    {
        //Reset scroll to be closed and just off screen
        scroll.transform.position = scrollCentre.position - new Vector3(0f, scrollHeight * 1.5f, 0f);
        scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(scroll.GetComponent<RectTransform>().sizeDelta.x, 0f);

        //Move the scroll into position in the centre of the screen
        float remainingDuration = scrollTravelInDuration;
        while (remainingDuration != 0f)
        {
            remainingDuration = Mathf.Clamp(remainingDuration - Time.deltaTime, 0f, scrollTravelInDuration);
            scroll.transform.position = Vector3.Lerp(scroll.transform.position, scrollCentre.position, 1f - (remainingDuration / scrollTravelInDuration));
            yield return null;
        }

        //Pause the scroll in place
        float remainingPause = scrollPauseDuration;
        while (remainingPause != 0f)
        {
            remainingPause = Mathf.Clamp(remainingPause - Time.deltaTime, 0f, scrollPauseDuration);
            yield return null;
        }

        //Open scroll over time
        float remainingOpenDuration = scrollOpenDuration;
        while (remainingOpenDuration != 0f)
        {
            remainingOpenDuration = Mathf.Clamp(remainingOpenDuration - Time.deltaTime, 0f, scrollOpenDuration);
            scroll.GetComponent<RectTransform>().sizeDelta = new Vector2(scroll.GetComponent<RectTransform>().sizeDelta.x, (1f - (remainingOpenDuration / scrollOpenDuration)) * scrollHeight);
            yield return null;
        }
    }

    IEnumerator PlayRewardCount()
    {
        float curVal = 0f;
        float endVal = GameController.gameController.reward;
        float progress = 0f;
        float speedUp = 10f;
        while (progress != 1f)
        {
            if(Input.GetMouseButton(0)) progress = Mathf.Clamp(progress + (Time.deltaTime / rewardCountTime) * speedUp, 0f, 1f);
            else progress = Mathf.Clamp(progress + (Time.deltaTime/rewardCountTime), 0f, 1f);
            curVal = endVal * progress;
            rewardText.text = rewardCurrencySymbol + (Mathf.FloorToInt(curVal)).ToString();
            yield return null;
        }
    }

    #endregion

}
