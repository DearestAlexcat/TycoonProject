using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleTycoon
{
    public class GMVisualProgress : MonoBehaviour
    {
        [SerializeField] Image back;
        [SerializeField] Image waiting;
        [SerializeField] List<Image> visualProgress;

        [Space]
        [SerializeField] PopUpText popUpTextPrefab;

        Dictionary<Guid, Image> hookedVP;

        public void VisualizeCostToPlay(int costToPlay)
        {
            Instantiate(popUpTextPrefab, transform).textUP.text = costToPlay.ToString();
            Service<MoneyController>.Get().AddMoney(costToPlay);
        }

        public void Initialize()
        {
            hookedVP = new Dictionary<Guid, Image>();

            DisableVisualProgress();
        }

        public void DisableVisualProgress()
        {
            back.gameObject.SetActive(false);
            waiting.gameObject.SetActive(false);

            foreach (Image item in visualProgress)
                item.gameObject.SetActive(false);
        }

        public void DisplayWaiting()
        {
            waiting.fillAmount = 1f;
            waiting.gameObject.SetActive(true);
        }

        public void DisplayHolder()
        {
            back.gameObject.SetActive(true);
            waiting.gameObject.SetActive(false);
        }

        public void SetWaitingFillAmount(float t) => waiting.fillAmount = t;

        public float GetWaitingFillAmount => waiting.fillAmount;

        public void SetFillAmount(Guid key, float t)
        {
            hookedVP[key].fillAmount = t;
        }

        public float GetFillAmount(Guid key)
        {
            return hookedVP[key].fillAmount;
        }

        public void RegisterVisualProgress(Guid key)
        {
            if (visualProgress.Count == 0)
            {
                Debug.LogError("All visualProgress are busy.");
                return;
            }

            hookedVP.Add(key, visualProgress[0]);
            visualProgress[0].gameObject.SetActive(true);
            visualProgress[0].fillAmount = 1f;
            visualProgress.RemoveAt(0);
        }

        public void UnRegisterVisualProgress(Guid key)
        {
            visualProgress.Add(hookedVP[key]);
            hookedVP[key].gameObject.SetActive(false);
            hookedVP.Remove(key);
        }
    }
}