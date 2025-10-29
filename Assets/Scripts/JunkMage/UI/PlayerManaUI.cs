using JunkMage.Entities.Player;
using JunkMage.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace JunkMage.UI
{
    public class PlayerManaUI : MonoBehaviour
    {
        [FormerlySerializedAs("healthBarSlider")] [SerializeField] private Slider manaBarSlider;
        [FormerlySerializedAs("healthBarText")] [SerializeField] private TextMeshProUGUI manaBarText;
        
        [FormerlySerializedAs("playerStats")] [SerializeField] private PlayerStats stats;
        [FormerlySerializedAs("playerMana")] [SerializeField] private PlayerMana mana;
        private float MaxMana => stats.GetVal(Stat.MaxMana);
        private float CurrentMana => Mathf.Floor(mana.CurrentMana * 10) / 10;
        
        void Awake()
        {
            mana.OnRegenMana += SetCurrentManaUI;
            stats.OnSetMaxMana += SetMaxManaUI;
        }

        void Start()
        {
            SetCurrentManaUI();
            SetMaxManaUI();
        }

        private void SetCurrentManaUI()
        {
            manaBarText.text = CurrentMana + "/" +  MaxMana;
            manaBarSlider.value = CurrentMana;
        }

        private void SetMaxManaUI()
        {
            manaBarText.text = CurrentMana + "/" +  MaxMana;
            manaBarSlider.maxValue = MaxMana;
        }
    }
}
