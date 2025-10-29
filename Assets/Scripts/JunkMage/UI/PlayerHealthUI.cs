using JunkMage.Entities.Player;
using JunkMage.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JunkMage.UI
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private Slider healthBarSlider;
        [SerializeField] private TextMeshProUGUI healthBarText;
        
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private PlayerHealth playerHealth;
        private float MaxHealth => playerStats.GetVal(Stat.MaxHealth);
        private float CurrentHealth => playerHealth.CurrentHealth;
        
        void Awake()
        {
            playerHealth.OnSetCurrentHealth += SetCurrentHealthUI;
            playerStats.OnSetMaxHealth += SetMaxHealthUI;
        }

        void Start()
        {
            SetCurrentHealthUI();
            SetMaxHealthUI();
        }

        private void SetCurrentHealthUI()
        {
            healthBarText.text = CurrentHealth + "/" +  MaxHealth;
            healthBarSlider.value = CurrentHealth;
        }

        private void SetMaxHealthUI()
        {
            healthBarText.text = CurrentHealth + "/" +  MaxHealth;
            healthBarSlider.maxValue = MaxHealth;
        }
    }
}
