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
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            playerHealth.OnSetCurrentHealth += SetCurrentCurrentHealth;
            playerStats.OnSetMaxHealth += UpdateMaxHealth;
        }

        private void SetCurrentCurrentHealth()
        {
            healthBarText.text = CurrentHealth + "/" +  MaxHealth;
            healthBarSlider.value = CurrentHealth;
        }

        private void UpdateMaxHealth()
        {
            healthBarText.text = CurrentHealth + "/" +  MaxHealth;
            healthBarSlider.maxValue = MaxHealth;
        }
    }
}
