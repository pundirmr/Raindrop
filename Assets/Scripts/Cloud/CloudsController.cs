using UnityEngine;
using System.Collections.Generic;
using LumosLabs.Shared.Pillar;

namespace LumosLabs.Raindrops
{
    /// <summary>
    /// Manages random clouds in the background.
    /// </summary>
    public class CloudsController : Controller
    {
        public float[] cloudSpeed;
        public float localPosXThreshold = 680f;
        private List<Vector3> startingPositions;
        private float[] _SpeedCache;
        private int currentCloud;

        protected override void OnEnable()
        {
            currentCloud = 2;
            _SpeedCache = cloudSpeed;
        }

        protected override void RegisterNotifications(IMessageRegistrar notifications)
        {
            notifications.AddListener(WaterNote.OnDropHit, HandleWaterLevelIncrease);
        }

        protected override void Initialize()
        {
            startingPositions = new List<Vector3>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var cloud = transform.GetChild(i);
                cloud.transform.localPosition = new Vector2(cloud.transform.localPosition.x, Random.Range(DetermineSpawnRange_Y().x, DetermineSpawnRange_Y().y));
                startingPositions.Add(cloud.localPosition);
                if (i > 2)
                {
                    cloud.gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            for(int i = 0; i < transform.childCount;i++)
            {
                var cloud = transform.GetChild(i);
                if (!cloud.gameObject.activeSelf)
                {
                    continue;
                }

                cloud.localPosition += new Vector3(cloudSpeed[i] * Time.deltaTime, 0f, 0f);

                // Move back to start once it leaves the screen
                if (cloud.localPosition.x <= localPosXThreshold)
                {
                    cloud.localPosition = startingPositions[i];
                }
            }
        }

        private void HandleWaterLevelIncrease()
        {
            currentCloud++;
            transform.GetChild(currentCloud).gameObject.SetActive(true);
        }

        public void OnGamePaused()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                cloudSpeed[i] = 0f;
            }
        }

        public void OnGameResumed()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                cloudSpeed = _SpeedCache;
            }
        }

        public Vector2 DetermineSpawnRange_Y()
        {
            Vector2 toReturn = Vector2.zero;
            Vector3[] v = new Vector3[4];
            GetComponent<RectTransform>().GetLocalCorners(v);

            toReturn.x = v[0].y;        //Bottom-left conrner.
            toReturn.y = v[2].y;        //Top-right conrner.

            return toReturn;
        }
    }
}