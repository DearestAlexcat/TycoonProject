using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace IdleTycoon
{
    public class MachineSeat
    {
        int playingPlaceId;
        public BaseMachinePlacement machinePlacement;

        public MachineSeat() { }

        public MachineSeat(int playingPlaceId, BaseMachinePlacement baseMachinePlacement)
        {
            this.playingPlaceId = playingPlaceId;
            this.machinePlacement = baseMachinePlacement;
        }

        public Transform PlayingPlace => machinePlacement.GetPlayingPlace(playingPlaceId);

        public void VacatePlacePoint()
        {
            machinePlacement.VacatePlacePoint(playingPlaceId);
        }
    }

    class MachineTimer
    {
        public float t;
        public Action<bool> callback;

        public Guid UniqueId { get; private set; } = Guid.NewGuid();

        public int GetSubscriberCount()
        {
            return callback?.GetInvocationList().Length ?? 0;
        }

        public IEnumerator InvokeCallbacksWithDelay(bool gameOutcome) // Вызов callback с задержкой. Исп. когда isGroup = true, чтобы юниты уходили более естественно
        {
            foreach (var callback in callback.GetInvocationList())
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.3f)); // Задержка между вызовами
                callback?.DynamicInvoke(gameOutcome);
            }
        }
    }

    public abstract class BaseMachinePlacement : MonoBehaviour
    {
        public int machineLevel;
        public int incomePerCycle;

        public GMVisualProgress visualProgress;

        public List<Transform> playingPlaces;                   // Точки возле автомата
        public BitArray occupiedPlacePoint;                     // Занятость точек

        public bool isGroup = false;                            // Либо все юниты одновременно играют и заканчивают, либо каждый сам по себе

        [HideInInspector] public float machineRunDuration = 2f;  // Cколько длится игра автомата. Время устанавливается комнатой

        bool isAnimating = false;                               // Анимация продолжается пока все юниты не закончат игру
        bool isWaiting = false;

        LinkedList<MachineTimer> timers = new LinkedList<MachineTimer>(); // LinkedList исп. для безопасного удаления

        public BaseMachineObject ObjectInstalledMachine { get; set; }

        public bool SlotIsEmpty { get; set; }

        public int occupiedPlacePointCount = 0;

        public Action<Vector3> OnPlayMoneyFX;

        private void Start()
        {
            visualProgress.DisableVisualProgress();

            occupiedPlacePoint = new BitArray(playingPlaces.Count);
        }

        public float GetCyclesPerMinute()
        {
            return 60f / machineRunDuration;
        }

        public float GetIncomePerMinute()
        {
            return incomePerCycle * GetCyclesPerMinute();
        }

        bool GetRandomGameOutcome => UnityEngine.Random.value < 0.7f ? true : false;

        public void FreeUpSlot()
        {
            UnityEngine.Object.Destroy(ObjectInstalledMachine.gameObject);
            ObjectInstalledMachine = null;
            machineLevel = 0;
        }

        public MachineSeat GetAvailableMachinePoint()
        {
            if (ObjectInstalledMachine != null)
            {
                for (int i = 0; i < playingPlaces.Count; i++)
                {
                    if (!occupiedPlacePoint[i])
                    {
                        occupiedPlacePoint[i] = true;
                        occupiedPlacePointCount++;
                        return new MachineSeat(i, this);
                    }
                }
            }

            return null;
        }

        public Transform GetPlayingPlace(int index)
        {
            if (index < 0 || index >= playingPlaces.Count)
            {
                Debug.LogError("index out of array bounds");
                return null;
            }

            return playingPlaces[index];
        }

        public void VacatePlacePoint(int index)
        {
            if (index < 0 || index >= occupiedPlacePoint.Count)
            {
                Debug.LogError("index out of array bounds");
                return;
            }

            occupiedPlacePoint[index] = false;
            occupiedPlacePointCount--;
        }

        public void StartAnimation(Action<bool> callback)
        {
            if (isGroup)
            {
                isAnimating = true;

                if (timers.Count == 0) // Создается один таймер на всю группу
                {
                    timers.AddLast(new MachineTimer());
                    timers.First.Value.t = 0;

                    visualProgress.Initialize();
                    visualProgress.DisplayWaiting();

                    isWaiting = true;
                }

                // callback каждого юнита подписывается на один callback таймера
                timers.First.Value.callback += callback;

                if (timers.First.Value.GetSubscriberCount() == playingPlaces.Count)
                {
                    isWaiting = false;
                    timers.First.Value.t = 1f;

                    visualProgress.DisplayHolder();
                    visualProgress.RegisterVisualProgress(timers.Last.Value.UniqueId);
                }
            }
            else
            {
                if (timers.Count == 0)
                {
                    visualProgress.Initialize();
                    visualProgress.DisplayHolder();
                }

                // Каждый игрок имеет свой таймер игры
                timers.AddLast(new MachineTimer());
                timers.Last.Value.t = 0;
                timers.Last.Value.callback = callback;
                isAnimating = true;

                // Каждый таймер работает со своим объектом VisualProgress
                visualProgress.RegisterVisualProgress(timers.Last.Value.UniqueId);
            }
        }

        void Update()
        {
            visualProgress.transform.LookAt(visualProgress.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up); // Повернут к камере

            if (isAnimating)
            {
                if (isGroup && isWaiting)
                {
                    if (ObjectInstalledMachine == null)
                    {
                        timers.First.Value.t = machineRunDuration;
                    }
                    else
                    {
                        timers.First.Value.t += Time.deltaTime;
                    }

                    visualProgress.SetWaitingFillAmount(1f - timers.First.Value.t / machineRunDuration);

                    if (visualProgress.GetWaitingFillAmount <= 0f)
                    {
                        visualProgress.DisableVisualProgress();

                        isAnimating = false;
                        isWaiting = false;

                        StartCoroutine(timers.First.Value.InvokeCallbacksWithDelay(false));

                        timers.Clear();
                    }
                }
                else if (isGroup)
                {
                    if (ObjectInstalledMachine == null)
                    {
                        timers.First.Value.t = machineRunDuration;
                    }
                    else
                    {
                        timers.First.Value.t += Time.deltaTime;
                    }

                    visualProgress.SetFillAmount(timers.First.Value.UniqueId, 1f - timers.First.Value.t / machineRunDuration);

                    if (visualProgress.GetFillAmount(timers.First.Value.UniqueId) <= 0f)
                    {
                        visualProgress.DisableVisualProgress();
                        visualProgress.UnRegisterVisualProgress(timers.First.Value.UniqueId);
                        OnPlayMoneyFX?.Invoke(visualProgress.transform.position);

                        isAnimating = false;

                        StartCoroutine(timers.First.Value.InvokeCallbacksWithDelay(GetRandomGameOutcome));

                        int subscriberCount = timers.First.Value.GetSubscriberCount();
                        for (int i = 0; i < subscriberCount; i++)
                        {
                            if (ObjectInstalledMachine != null)
                                visualProgress.VisualizeCostToPlay(incomePerCycle);
                        }

                        timers.Clear();
                    }
                }
                else
                {
                    // Данный подход позволит безопасно добавлять и удалять элементы
                    var node = timers.First;

                    while (node != null)
                    {
                        var next = node.Next; // Сохраняем следующий элемент заранее

                        if (ObjectInstalledMachine == null)
                        {
                            node.Value.t = machineRunDuration;
                        }
                        else
                        {
                            node.Value.t += Time.deltaTime;
                        }

                        visualProgress.SetFillAmount(node.Value.UniqueId, 1f - node.Value.t / machineRunDuration);

                        if (node.Value.t / machineRunDuration >= 1f)
                        {
                            visualProgress.UnRegisterVisualProgress(node.Value.UniqueId);
                            OnPlayMoneyFX?.Invoke(visualProgress.transform.position);

                            if (ObjectInstalledMachine != null)
                                visualProgress.VisualizeCostToPlay(incomePerCycle);

                            timers.Remove(node); // Безопасное удаление

                            StartCoroutine(node.Value.InvokeCallbacksWithDelay(GetRandomGameOutcome)); // Небольшая задержка для естественного ухода

                            if (timers.Count == 0)
                            {
                                visualProgress.DisableVisualProgress();
                                isAnimating = false;
                            }
                        }

                        node = next; // Переходим к следующему
                    }
                }
            }
        }
    }
}