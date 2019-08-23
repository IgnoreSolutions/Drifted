/*
by Faye
*/

using Drifted.Interactivity;
using Drifted.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drifted.Environment
{
    public class PlantStageHandler : DriftedSceneInteractable
    {
        public GameObject[] Stages;
        public GameObject CutDownStage;

        public Transform parent;
        public float growthTime;
        public int currentStage = 0;
        
        public PlayerMovement playerMovement;
        public CameraFollow cameraFollow;

        [SerializeField]
        ItemContainer itemDrop;

        protected override void Awake()
        {
            base.Awake();
            BeginStage(currentStage);
            Invoke("nextStage", growthTime);

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerMovement = player.GetComponent<PlayerMovement>();

            if(cameraFollow == null) cameraFollow = Camera.main.GetComponent<CameraFollow>();
        }

        void ClearChildren()
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        void BeginStage(int stage)
        {
            stage = Mathf.Clamp(stage, 0, Stages.Length - 1);

            ClearChildren();
            GameObject toCreate = Stages[stage];

            if (toCreate != null)
            {
                GameObject newIn = Instantiate(toCreate, parent.transform.position, toCreate.transform.rotation);
                newIn.transform.SetParent(parent);
            }
        }

        void SetCut()
        {
            ClearChildren();

            if (CutDownStage != null)
            {
                GameObject newIn = Instantiate(CutDownStage, parent.transform.position, CutDownStage.transform.rotation);
                newIn.transform.SetParent(parent);
            }

            currentStage = 5;

            Invoke("nextStage", growthTime);
        }

        public void nextStage()
        {
            if (currentStage == 5) // Cut down state
            {
                currentStage = 2;
                BeginStage(currentStage);
                Invoke("nextStage", growthTime);
            }

            if (currentStage < Stages.Length - 1)
            {
                currentStage++;
                BeginStage(currentStage);
                Invoke("nextStage", growthTime);
            }
        }

        bool Harvest()
        {
            if (currentStage == Stages.Length - 1)
            {
                playerMovement.EnqueueAction(playerMovement.MovePlayerTo(transform));
                playerMovement.EnqueueAction(() => playerMovement.PlayChoppingAnimation(true));
                playerMovement.EnqueueWait(4.0f);
                playerMovement.EnqueueAction(() => playerMovement.PlayChoppingAnimation(false));
                playerMovement.EnqueueAction(() =>
                {
                    if (playerInventory != null && itemDrop != null)
                    {
                        playerInventory.AddItem(itemDrop);
                    }

                    if (notificationManager != null)
                    {
                        notificationManager.PushNotification(gameObject, new DriftedNotification($"You placed {itemDrop.Quantity}x {itemDrop.GetItem().ItemName} in your inventory.",
                                                                            itemDrop.GetItem().Icon));
                    }
                    SetCut();
                });
            }

            return false;
        }

        public override void Interact(MonoBehaviour sender)
        {
            // TODO: allow this to be generic and extensible.
            List<AbstractMenuItem> MenuItems = new List<AbstractMenuItem>();
            MenuItems.Add(PopUpMenu.MakeMenuItem("Harvest", Harvest));
            //
            menuManager.MakePopUpMenu(this.gameObject, MenuItems.ToArray());
            cameraFollow.SetFollowTarget(this.gameObject);
        }
    }

}
