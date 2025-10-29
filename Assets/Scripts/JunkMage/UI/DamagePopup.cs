using System;
using TMPro;
using UnityEngine;

namespace JunkMage.UI
{
    public class DamagePopup : MonoBehaviour
    {
        private TMP_Text dmgText;
        [SerializeField] private float lifetime = 0.8f;
        [SerializeField] private Vector3 moveSpeed = new Vector3(0, 4f, 0);

        private float timer = 0;
        private float endTime = Mathf.Infinity;

        private void Awake()
        {
            dmgText = GetComponent<TMP_Text>();
        }
        
        public void SetDmg(float dmg, bool isCrit)
        {
            dmgText.text = dmg.ToString();
            
            if (isCrit) dmgText.color = new Color(1, 61/255, 61/255, 1);
            
            endTime = Time.deltaTime + lifetime;
        }

        void Update()
        {
            transform.position += moveSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            if (timer >= endTime)
                Destroy(gameObject);
        }
    }
}
