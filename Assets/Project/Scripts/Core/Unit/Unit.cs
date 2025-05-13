using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace IdleTycoon
{
    public class Unit : MonoBehaviour, IPoolObject<Unit>
    {
        [SerializeField] UnitStateAnimation stateAnimation;
        [SerializeField] public NavMeshAgent agent;
        [SerializeField] Transform holderEmoji;
        [SerializeField] EmojiPrefabData emojiData;

        [HideInInspector] public Room entryRoom;               // Вход в здания
        [HideInInspector] public Room exitRoom;                // Выход из здания
        [HideInInspector] public Transform spawnPoint;
        [HideInInspector] public List<Room> rooms;     // Посещаемые комнаты

        [HideInInspector] public Transform decisionPoint;
        [HideInInspector] public int currentRoom = -1;

        public MachineSeat machineSeat;

        bool destinationReached = false;
        float stoppingDistance = 0.2f;
        UnitGoal unitGoal;
        Room choosedRoom;

        public Pooler<Unit> Pooler { get; set; }

        private void Start()
        {
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }

        void OnEnable()
        {
            agent.enabled = false;
            agent.Warp(transform.position); // установить корректно без рассинхрона
            agent.enabled = true;
        }

        private void OnDisable()
        {
            currentRoom = -1;
            destinationReached = true;
        }

        public void CreateEmoji(EmojiType type)
        {
            Object.Destroy(Object.Instantiate(emojiData.GetEmojiPrefab(type), holderEmoji), 2f);
        }

        public void GoToRoom()
        {
            currentRoom++;

            while (currentRoom < rooms.Count)
            {
                choosedRoom = rooms[currentRoom];

                if (choosedRoom == null)
                {
                    currentRoom++;
                    continue;
                }

                SetDestination(choosedRoom.enterPoint, UnitGoal.ReachEnterPoint);

                break;
            }

            if (currentRoom >= rooms.Count)
            {
                SetDestination(exitRoom.exitPoint, UnitGoal.ReachExitRoom);
            }

            /*if(currentRoom < rooms.Count)
            {
                choosedRoom = rooms[currentRoom].GetAvailableRoom();
                SetDestination(choosedRoom.enterPoint, UnitGoal.ReachEnterPoint);
            }
            else
            {
                SetDestination(entryExitRoom.exitPoint, UnitGoal.ReachExitRoom);
            }*/
        }

        void Update()
        {
            if (!destinationReached)
            {
                float distance = Vector3.Distance(agent.transform.position, decisionPoint.position);

                if (distance <= agent.stoppingDistance + stoppingDistance)
                {
                    destinationReached = true;
                    OnDestinationReached(unitGoal);
                }
            }
        }

        void SetDestination(Transform destination, UnitGoal goal)
        {
            decisionPoint = destination;
            agent.destination = destination.position;
            destinationReached = false;
            unitGoal = goal;
            stateAnimation.PlayAnimation(AnimationFlags.Walking);
        }

        void OnDestinationReached(UnitGoal goal)
        {
            transform.rotation = Quaternion.LookRotation(decisionPoint.forward);

            switch (goal)
            {
                case UnitGoal.ReachEnterPoint:
                    ReachedRoomDecisionPoint();
                    break;
                case UnitGoal.ReachExitPoint:
                    GoToRoom();
                    break;
                case UnitGoal.ReachQueuePoint:
                    ReachedRoomQueuePoint();
                    break;
                case UnitGoal.ReachMahinePoint:
                    ReachedRoomMahinePoint();
                    break;
                case UnitGoal.ReachExitRoom:
                    ReachedExitRoomPoint();
                    break;
            }
        }

        public void GoToSelectedRoom(MachineSeat machineSeat)
        {
            this.machineSeat = machineSeat;
            choosedRoom.AddToRoom(this);
            SetDestination(machineSeat.PlayingPlace, UnitGoal.ReachMahinePoint);
        }

        public void GoToNextPointQueue(Transform queuePoint)
        {
            SetDestination(queuePoint, UnitGoal.ReachQueuePoint);
        }

        public void ReachedRoomDecisionPoint()
        {
            machineSeat = choosedRoom.GetAvailableMachinePoint();

            if (machineSeat != null)
            {
                choosedRoom.AddToRoom(this);
                SetDestination(machineSeat.PlayingPlace, UnitGoal.ReachMahinePoint);
            }
            else
            {
                Transform queuePoint = choosedRoom.GetNextQueuePoint();

                if (queuePoint != null)
                {
                    choosedRoom.AddToQueue(this);
                    SetDestination(queuePoint, UnitGoal.ReachQueuePoint);
                }
                else
                {
                    CreateEmoji(EmojiType.TearyEyes);
                    GoToRoom();
                }
            }
        }

        public void EndPlayMachine(bool gameOutcome)
        {
            transform.SetParent(null);
            transform.position = decisionPoint.position;
            agent.enabled = true;

            CreateEmoji(gameOutcome ? EmojiType.Cool : EmojiType.TearyEyes);

            SetDestination(choosedRoom.exitPoint, UnitGoal.ReachExitPoint);
        }

        void ReachedRoomQueuePoint()
        {
            stateAnimation.PlayAnimation(AnimationFlags.BreathingIdle);
        }

        void ReachedRoomMahinePoint()
        {
            agent.enabled = false;
            stateAnimation.PlayAnimation(AnimationFlags.BreathingIdle);
            choosedRoom.PlayMachine(this);
        }

        void ReachedExitRoomPoint()
        {
            stateAnimation.PlayAnimation(AnimationFlags.BreathingIdle);

            this.Pooler.Free(this);

            //Destroy(gameObject);
        }
    }
}