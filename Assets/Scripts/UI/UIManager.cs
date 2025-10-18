using JunkMage.Entities.Enemies;
using JunkMage.Systems;
using UnityEngine;

namespace JunkMage.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject dmgPopupPrefab;

        public void RegisterEnemy(EnemyBase enemy)
        {
            enemy.OnTakeDamage += ShowDmgPopup;
        }

        private void ShowDmgPopup(DamageInfo dmgInfo)
        {
            GameObject popup = Instantiate(dmgPopupPrefab, dmgInfo.Target.transform.position, Quaternion.identity);
            popup.GetComponent<DamagePopup>().SetDmg(dmgInfo.Dmg, dmgInfo.IsCrit);
        }
    }
}
