using JunkMage.Stats;
using UnityEngine;
using UnityEngine.Serialization;

public class DashCooldownUI : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float cooldownBarMaxWidth = 1.7f;
    
    private PlayerMovement playerMovement;
    private PlayerStats playerStats;
    
    private GameObject uiContainer;
    
    private RectTransform rt;
    [SerializeField] private RectTransform backgroundRt;
    [SerializeField] private RectTransform slider;

    private float totalDashCooldown => playerStats.GetVal(Stat.DashCooldown);
    
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerStats = player.GetComponent<PlayerStats>();
        
        uiContainer = transform.Find("Container").gameObject;
        uiContainer.SetActive(false);
        
        backgroundRt.sizeDelta = new Vector2(cooldownBarMaxWidth, backgroundRt.sizeDelta.y);
    }

    void Update()
    {
        bool shouldShow = playerMovement.DashCooldownRemaining > 0f;
        if (uiContainer.activeSelf != shouldShow)
            uiContainer.SetActive(shouldShow);

        if (shouldShow)
        {
            float t = 1f - (playerMovement.DashCooldownRemaining / totalDashCooldown);
            slider.sizeDelta = new Vector2(t * cooldownBarMaxWidth, backgroundRt.sizeDelta.y);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        rt.position = player.transform.position + Vector3.up * 1.2f;
    }
}
