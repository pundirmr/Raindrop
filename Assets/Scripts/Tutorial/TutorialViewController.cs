using System.Collections.Generic;
using UnityEngine;
using LumosLabs.Shared.Pillar;
using System.Linq;

namespace LumosLabs.Raindrops
{
    public class TutorialViewController : Controller
    {
        [SerializeField] private TutorialView tutorialView;
        [SerializeField] private string dropHintKey = "dropHint";
        [SerializeField] private string sunHintKey = "sunHint";
        [SerializeField] private string tryAgainKey = "tryAgain";

        private IMessengerService messenger;
        private IAudioService _audio;
        private ILocalizationService _localization;

        private TrialsModel trialsModel;

        protected override void Initialize()
        {
            messenger = Services.Get<IMessengerService>();
            _audio = Services.Get<IAudioService>();
            _localization = Services.Get<ILocalizationService>();
            trialsModel = new TrialsModel(Services.Get<IMessengerService>());
            if (tutorialView.dropPrefab != null)
                trialsModel.dropletsPool = new ObjectPool(tutorialView.dropPrefab, tutorialView.DropsAndSunParent, 5);

            if (tutorialView.sunPrefab != null)
                trialsModel.sunsPool = new ObjectPool(tutorialView.sunPrefab, tutorialView.DropsAndSunParent, 1);
        }

        protected override void RegisterCommands(IMessageRegistrar commands)
        {
            
        }

        protected override void RegisterNotifications(IMessageRegistrar notifications)
        {
            notifications.AddListener(TutorialNote.Started, HandleStartCommand);
            notifications.AddListener(TutorialNote.Skipped, ReleasePools);

            notifications.AddListener(TutorialViewNote.SpawnDroplet, HandleDropSpawn);
            notifications.AddListener(TutorialViewNote.SpawnSun, HandleSunSpawn);
            notifications.AddListener(GameplayNote.OnKeypadClicked, OnNumberTyped);
        }

        private void ReleasePools()
        {
            if(trialsModel == null)
            {
                return;
            }
            for (int i = 0; i < trialsModel.gameState.ActiveDrops.Count; i++)
            {
                if (trialsModel.gameState.ActiveDrops[i].GetType() == typeof(Droplet))
                {
                    trialsModel.dropletsPool.Release(trialsModel.gameState.ActiveDrops[i].gameObject);
                }
                else
                {
                    trialsModel.sunsPool.Release(trialsModel.gameState.ActiveDrops[i].gameObject);
                }
                trialsModel.gameState.ActiveDrops[i].gameObject.gameObject.SetActive(false);
            }
            trialsModel.gameState.ActiveDrops.Clear();
        }

        private void HandleStartCommand()
        {
            tutorialView.water.Initialize();
            tutorialView.water.SetWaterLevel(3);
            SetSpawnRange(tutorialView.spawnAreaRect);
        }

        private void HandleDropSpawn()
        {
            trialsModel.spawnModel.waterPixelPosition = Screen.height - tutorialView.water.GetComponent<RectTransform>().anchoredPosition.y;
            // Get random position
            var position = trialsModel.spawnModel.GetSpawnPosition();

            // Get the next random equation based on current difficulty for this droplet
            var equation = trialsModel.equationsManager.Next();

            // Calculate speed
            var speed = trialsModel.spawnModel.GetSpawnSpeed();

            var droplet = trialsModel.dropletsPool.Get(true).GetComponent<Droplet>();
            droplet.Initialize(equation, position, speed);
            droplet.onWaterCollision += droplet_onWaterCollision;
            droplet.onDeath += droplet_onPop;

            trialsModel.gameState.ActiveDrops.Add(droplet);
            tutorialView.SetHintText(_localization.Localize(dropHintKey));
        }

        private void HandleSunSpawn()
        {
            tutorialView.SunAppear();
            // Get random position
            var position = trialsModel.spawnModel.GetSpawnPosition();

            // Get the next random equation based on current difficulty for this droplet
            var equation = trialsModel.equationsManager.Next();

            // Calculate speed
            var speed = trialsModel.spawnModel.GetSpawnSpeed();

            _audio.PlayClip(AudioClip.Sun);

            var sun = trialsModel.sunsPool.Get(true).GetComponent<Sun>();
            sun.Initialize(equation, position, speed);
            sun.onWaterCollision += sun_onWaterCollision;
            sun.onDeath += sun_onDeath;

            trialsModel.gameState.ActiveDrops.Add(sun);
            tutorialView.SetHintText(_localization.Localize(sunHintKey));
        }

        private void SetSpawnRange(RectTransform spawnAreaRect)
        {
            var store = Services.Get<IModelStoreService>();
            var raindropsSettings = store.GetModel<SettingsModel>(SettingsModel.Key);

            // Initialize spawn manager 
            GameObject pooledDrop = trialsModel.dropletsPool.Get(false);
            var dropletCollider = pooledDrop.GetComponent<BoxCollider2D>();

            trialsModel.spawnModel = new SpawnModel(Services.Get<IMessengerService>(),
                trialsModel.gameState, raindropsSettings.Asset.baseSpawnPeriod, raindropsSettings.Asset.baseDropletSpeed,
                dropletCollider.offset, dropletCollider.size, raindropsSettings.Asset.sunOccuranceFrequency);

            Vector2 spawnRangeX = DetermineSpawnRange_X(spawnAreaRect);
            Vector2 spawnRangeY = DetermineSpawnRange_Y(spawnAreaRect);
            trialsModel.spawnModel.SetSpawnRange(spawnRangeX, spawnRangeY);
            trialsModel.dropletsPool.Release(pooledDrop);
        }

        private void droplet_onPop(object sender, System.EventArgs e)
        {
            var droplet = sender as Droplet;
            droplet.onWaterCollision -= droplet_onWaterCollision;
            droplet.onDeath -= droplet_onPop;
            trialsModel.dropletsPool.Release(droplet.gameObject);
            droplet.gameObject.SetActive(false);
        }

        private void droplet_onWaterCollision(object sender, System.EventArgs e)
        {
            //Debug.Log("[RaindropsGameplayProxy]droplet_onWaterCollision()");
            //var droplet = sender as Droplet;
            //droplet.Splash();
            tutorialView.SetHintText(_localization.Localize(tryAgainKey));
            SplashAllDrops();
            Services.Get<IMessengerService>().Send(TutorialViewNote.Restart);
        }

        void sun_onDeath(object sender, System.EventArgs e)
        {
            tutorialView.SunDisappear();
            // Remove tint effect
            //gameAnimator.SunDisappear();

            // Kill the sun
            var sun = sender as Sun;
            sun.onWaterCollision -= sun_onWaterCollision;
            sun.onDeath -= sun_onDeath;

            // Release to the pool
            trialsModel.sunsPool.Release(sun.gameObject);
        }

        void sun_onWaterCollision(object sender, System.EventArgs e)
        {
            tutorialView.SetHintText(_localization.Localize(tryAgainKey));
            var sun = sender as Sun;
            // Touching the water with sun is not punishing for the player
            trialsModel.gameState.ActiveDrops.Remove(sun);
            sun.Explode();
            Services.Get<IMessengerService>().Send(TutorialViewNote.Restart);
        }

        public Vector2 DetermineSpawnRange_X(RectTransform rectT)
        {
            Vector2 toReturn = Vector2.zero;
            Vector3[] v = new Vector3[4];
            rectT.GetWorldCorners(v);

            toReturn.x = v[0].x;        //Bottom-left conrner.
            toReturn.y = v[2].x;        //Top-right conrner.

            return toReturn;
        }

        public Vector2 DetermineSpawnRange_Y(RectTransform rectT)
        {
            Vector2 toReturn = Vector2.zero;
            Vector3[] v = new Vector3[4];
            rectT.GetWorldCorners(v);

            toReturn.x = v[0].y;        //Bottom-left conrner.
            toReturn.y = v[2].y;        //Top-right conrner.

            return toReturn;
        }

        public void OnNumberTyped(int number, bool isTutorial)
        {
            if (!isTutorial)
            {
                return;
            }

            if (number >= 0 && number <= 9)
            {
                trialsModel.currentTypedNumberStr += number.ToString();
            }
            else
            {
                if (number == -1)    //If Clear button was pressed,
                {
                    trialsModel.currentTypedNumberStr = "";
                }
                else                //If 'Enter' button was pressed,
                {
                    //TODO: 
                    int numberTyped = -1;
                    int.TryParse(trialsModel.currentTypedNumberStr, out numberTyped);
                    trialsModel.currentTypedNumberStr = "";
                    CheckAnswer(numberTyped);
                }
            }

            tutorialView.SetAnswerText(trialsModel.currentTypedNumberStr);
        }

        void CheckAnswer(int guessedAnswer)
        {
            Debug.Log(string.Format("Checking answer: {0}", guessedAnswer));

            var poppedDroplets = new List<DropBase>();
            var dropCount = trialsModel.gameState.ActiveDrops.Count;

            var settings = Services.Get<IModelStoreService>().GetModel<SettingsModel>(SettingsModel.Key);

            // First check if we hit any suns
            bool hitSun = false;
            var suns = trialsModel.gameState.ActiveDrops.Where(x => x.DropType == DropletType.SUN);
            if (suns.Count() > 0)
            {
                foreach (var sunDrop in suns)
                {
                    if (sunDrop.transform.position.y >= TrialsModel.DropletVisibilityTreshold && sunDrop.Equation.CheckAnswer(guessedAnswer))
                    {
                        poppedDroplets.Add(sunDrop);
                        hitSun = true;

                        _audio.PlayClip(AudioClip.SunBurst);
                        _audio.PlayClip(AudioClip.AnswerCorrect);

                        // Pop effect
                        var sun = sunDrop as Sun;
                        sun.Explode(true);
                        //DestroyAllOtherDrops(sun, DropsDestroyType.EVAPORATION);
                        trialsModel.gameState.ActiveDrops.Remove(sun);
                        EvaporateAllDrops();

                        messenger.Send(TutorialViewNote.NextStep);
                        break;
                    }
                }
            }
            if (!hitSun)
            {
                bool pointsAdded = false;
                foreach (var drop in trialsModel.gameState.ActiveDrops)
                {
                    if (drop.transform.position.y >= TrialsModel.DropletVisibilityTreshold && drop.Equation.CheckAnswer(guessedAnswer))
                    {
                        poppedDroplets.Add(drop);

                        _audio.PlayClip(AudioClip.DropletBurst);
                        _audio.PlayClip(AudioClip.AnswerCorrect);

                        // Has to be droplet - pop effect
                        var droplet = drop as Droplet;
                        droplet.Explode(!pointsAdded);

                        // Get rid of popped droplets in list
                        trialsModel.gameState.ActiveDrops = trialsModel.gameState.ActiveDrops.Except(poppedDroplets).ToList();
                        messenger.Send(TutorialViewNote.NextStep);
                    }
                }
            }
            else
            {
                trialsModel.gameState.ActiveDrops.Clear();
            }
        }

        private void SplashAllDrops()
        {
            foreach (var obj in trialsModel.gameState.ActiveDrops)
            {
                if (obj is Droplet droplet)
                {
                    droplet.Splash();
                }
            }

            trialsModel.gameState.ActiveDrops.Clear();
        }

        private void EvaporateAllDrops()
        {
            foreach (Droplet droplet in trialsModel.gameState.ActiveDrops)
            {
                droplet.Evaporate();
            }
        }
    }
}
