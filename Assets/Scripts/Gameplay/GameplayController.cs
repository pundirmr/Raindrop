using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LumosLabs.Shared.Pillar;
using System.Linq;

namespace LumosLabs.Raindrops
{
    public class GameplayController : Controller, ITrialGenerator
    {
        [SerializeField] private GameplayView gameplayView;
        private ISession _session;
        private ITrial _trial;
        private IMessengerService messenger;
        private IAudioService _audio;

        private TrialsModel trialsModel;

        private int _score = 0;
        private bool _isAutoPlay;

        #region Controller Initialization 
        protected override void Initialize()
        {
            messenger = Services.Get<IMessengerService>();
            _audio = Services.Get<IAudioService>();
            _isAutoPlay = Services.Get<IAppService>().LaunchParameters.IsAutoPlay;
            trialsModel = new TrialsModel(Services.Get<IMessengerService>());
            if (gameplayView.dropPrefab != null)
                trialsModel.dropletsPool = new ObjectPool(gameplayView.dropPrefab, gameplayView.DropsAndSunParent);

            if (gameplayView.sunPrefab != null)
                trialsModel.sunsPool = new ObjectPool(gameplayView.sunPrefab, gameplayView.DropsAndSunParent, 5);
        }

        protected override void RegisterCommands(IMessageRegistrar commands)
        {
            commands.AddListener(GameplayCmd.Start, HandleStartCommand);
            commands.AddListener(GameplayCmd.Reset, HandleResetCommand);
        }

        protected override void RegisterNotifications(IMessageRegistrar notifications)
        {
            notifications.AddListener(MetadataNote.SessionStarted, HandleSessionStarted);
            notifications.AddListener(MetadataNote.SessionEnded, HandleSessionEnded);
            notifications.AddListener(MetadataNote.TrialEnded, HandleTrialEnded);
            notifications.AddListener(GameplayNote.SpawnDroplet, HandleDropSpawning);
            notifications.AddListener(GameplayNote.SpawnSun, HandleSunSpawning);
            notifications.AddListener(GameplayNote.OnKeypadClicked, OnNumberTyped);
            notifications.AddListener(GameplayNote.End, HandleGameEnd);
            notifications.AddListener(AutoPlayNote.Answer, HandleAnswer);
        }
        #endregion

        void ITrialGenerator.GenerateTrial(ITrial trial, ISession session)
        {
            
        }

        #region Command Handlers
        private void HandleStartCommand()
        {
            int waterLevel = 1;
            if (Services.Get<IAppService>().LaunchParameters.IsShortGame)
            {
                waterLevel = 3;
            }

            var music = _audio.PlayClip(AudioClip.AmbientRain,SharedAudioBus.Music);
            music.Loop = true;
            SetSpawnRange(gameplayView.spawnAreaRect);
            gameplayView.Initialize();
            messenger.Send(WaterNote.Initialize);
            messenger.Send(WaterNote.SetWaterLevel, waterLevel);
            gameplayView.OnUpdateEvent += OnViewUpdate;

            if (_isAutoPlay)
            {
                messenger.Send(AutoPlayCmd.Start);
            }
        }

        private void HandleResetCommand()
        {
            gameplayView.OnUpdateEvent -= OnViewUpdate;
            _score = 0;
            messenger.Send(ScoreNote.ScoreChanged, _score);
            messenger.Send(WaterNote.Reset);
            trialsModel.OnGameReset();
            SetSpawnRange(gameplayView.spawnAreaRect);
        }
        #endregion

        #region Notification Handlers
        private void HandleSessionStarted(ISession session)
        {
            _session = session;
        }

        private void HandleSessionEnded(ISession session)
        {
            if (session != _session)
            {
                return;
            }
            if(trialsModel == null || trialsModel.spawnModel==null)
            {
                _session = null;
                return;
            }
            trialsModel.lastDropletSpeed = trialsModel.spawnModel.GetSpawnSpeed();

            var playData = session.PlayData;
            playData.SetField(PlayDataKeys.NumCorrect, trialsModel.gameState.TotalCorrect);
            playData.SetField(PlayDataKeys.NumCorrection, trialsModel.totalCorrections);
            playData.SetField(PlayDataKeys.Speed, trialsModel.lastDropletSpeed);
            playData.SetField(PlayDataKeys.NumProblems, trialsModel.spawnModel.TotalDrops);
            playData.SetField(PlayDataKeys.NumTries, trialsModel.totalTries);
            playData.SetField(PlayDataKeys.NumTriesCorrection, trialsModel.totalTriesCorrections);

            _session = null;
        }

        private void HandleTrialEnded(ITrial trial)
        {
            // return if trial is not complete
            if (trial != _trial || trial.Correct == null)
            {
                return;
            }

            _trial = null;
        }
        #endregion

        public void OnViewUpdate()
        {
            trialsModel.playTimer.Update();
            trialsModel.difficultyManager.Update(trialsModel.playTimer.Time);
            trialsModel.spawnModel.OnViewUpdateListener();
        }

        public void SetSpawnRange(RectTransform spawnAreaRect)
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

        private void HandleDropSpawning()
        {
            trialsModel.spawnModel.waterPixelPosition = Screen.height - gameplayView.water.GetComponent<RectTransform>().anchoredPosition.y;
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
        }

        private void HandleSunSpawning()
        {
            gameplayView.SunAppear();
            // Get random position
            var position = trialsModel.spawnModel.GetSpawnPosition();

            // Get the next random equation based on current difficulty for this droplet
            var equation = trialsModel.equationsManager.Next();

            // Calculate speed
            var speed = trialsModel.spawnModel.GetSpawnSpeed();

            var sun = trialsModel.sunsPool.Get(true).GetComponent<Sun>();
            sun.Initialize(equation, position, speed);
            sun.onWaterCollision += sun_onWaterCollision;
            sun.onDeath += sun_onDeath;

            _audio.PlayClip(AudioClip.Sun);

            trialsModel.gameState.ActiveDrops.Add(sun);
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
            var droplet = sender as Droplet;

            //messenger.Send(MetadataCmd.StartTrial, this, trial => _trial = trial);
           //HandleTrialMetadata(droplet.transform.position, droplet.Equation.GetAnswer().ToString(), "", "X");

            SplashAllDrops();
            _audio.PlayClip(AudioClip.DropletSplash);
            messenger.Send(WaterNote.OnDropHit);

        }

        void sun_onDeath(object sender, System.EventArgs e)
        {
            gameplayView.SunDisappear();

            // Kill the sun
            var sun = sender as Sun;
            sun.onWaterCollision -= sun_onWaterCollision;
            sun.onDeath -= sun_onDeath;

            // Release to the pool
            trialsModel.sunsPool.Release(sun.gameObject);
        }

        void sun_onWaterCollision(object sender, System.EventArgs e)
        {
            var sun = sender as Sun;

            _audio.PlayClip(AudioClip.SunBurst);

            messenger.Send(MetadataCmd.StartTrial, this, trial => _trial = trial);
            HandleTrialMetadata(sun.transform.position, sun.Equation.GetAnswer().ToString(), "", "X");

            // Touching the water with sun is not punishing for the player
            trialsModel.gameState.ActiveDrops.Remove(sun);
            sun.Explode();
        }

        /// <summary>
        /// Determines and returns world X min and max threshold for spawning droplets, using the "SpawnArea_Rect" in view!
        /// Since droplets are now of canvas space, their spawning needs to be calculated to adjust to all aspect ratios.
        /// </summary>
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

        public void OnGameResumed()
        {
            foreach (DropBase drop in trialsModel.gameState.ActiveDrops)
            {
                drop.Unpause();
            }
        }

        public void OnGamePaused()
        {
            foreach (DropBase drop in trialsModel.gameState.ActiveDrops)
            {
                drop.Pause();
            }
        }

        private void HandleAnswer()
        {
            if (trialsModel.gameState.ActiveDrops.Count > 0)
            {
                CheckAnswer(trialsModel.gameState.ActiveDrops[0].Equation.Answer);
            }
        }

        public void OnNumberTyped(int number, bool isTutorial)
        {
            if (isTutorial)
            {
                return;
            }

            bool hasCorrected = false;

            if (number >= 0 && number <= 9)
            {
                trialsModel.currentTypedNumberStr += number.ToString();
            }
            else
            {
                if (number == -1)    //If Clear button was pressed,
                {
                    if (!hasCorrected)
                    {
                        trialsModel.totalTriesCorrections++;
                    }

                    trialsModel.currentTypedNumberStr = "";
                    trialsModel.lastCorrections++;
                    hasCorrected = true;
                }
                else                //If 'Enter' button was pressed,
                {
                    //TODO: 
                    int numberTyped = -1;
                    int.TryParse(trialsModel.currentTypedNumberStr, out numberTyped);
                    CheckAnswer(numberTyped);
                    trialsModel.currentTypedNumberStr = "";
                    trialsModel.lastCorrections = 0;
                }
            }

            gameplayView.SetAnswerText(trialsModel.currentTypedNumberStr);
        }

        void CheckAnswer(int guessedAnswer)
        {
            Debug.Log(string.Format("Checking answer: {0}", guessedAnswer));

            var poppedDroplets = new List<DropBase>();
            var dropCount = trialsModel.gameState.ActiveDrops.Count;

            var settings = Services.Get<IModelStoreService>().GetModel<SettingsModel>(SettingsModel.Key);

            // First check if we hit any suns
            bool hitSun = false;
            bool correct = false;
            var suns = trialsModel.gameState.ActiveDrops.Where(x => x.DropType == DropletType.SUN);
            if (suns.Count() > 0)
            {
                foreach (var sunDrop in suns)
                {
                    if (sunDrop.transform.position.y >= TrialsModel.DropletVisibilityTreshold && sunDrop.Equation.CheckAnswer(guessedAnswer))
                    {
                        poppedDroplets.Add(sunDrop);
                        hitSun = true;
                        correct = true;
                        // Pop effect
                        var sun = sunDrop as Sun;
                        sun.Explode(true);
                        _score += settings.Asset.pointsPerDroplet * trialsModel.gameState.ActiveDrops.Count;
                        messenger.Send(ScoreNote.ScoreChanged, _score);

                        _audio.PlayClip(AudioClip.SunBurst);
                        _audio.PlayClip(AudioClip.AnswerCorrect);

                        //DestroyAllOtherDrops(sun, DropsDestroyType.EVAPORATION);
                        trialsModel.gameState.ActiveDrops.Remove(sun);
                        EvaporateAllDrops();

                        messenger.Send(MetadataCmd.StartTrial, this, trial => _trial = trial);
                        HandleTrialMetadata(sunDrop.transform.position, sunDrop.Equation.GetAnswer().ToString(), "C", "S");

                        AddCorrectAnswer(sunDrop.LifeTime);
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
                        correct = true;
                        // Has to be droplet - pop effect
                        var droplet = drop as Droplet;
                        droplet.Explode(!pointsAdded);

                        _audio.PlayClip(AudioClip.AnswerCorrect);
                        _audio.PlayClip(AudioClip.DropletBurst);

                        // Add points for a single answer though
                        if (!pointsAdded)
                        {
                            AddCorrectAnswer(drop.LifeTime);
                            pointsAdded = true;
                        }

                        messenger.Send(MetadataCmd.StartTrial, this, trial => _trial = trial);
                        HandleTrialMetadata(drop.transform.position, drop.Equation.GetAnswer().ToString(), "T", "P");

                        _score += settings.Asset.pointsPerDroplet;
                        messenger.Send(ScoreNote.ScoreChanged, _score);

                        // Get rid of popped droplets in list
                        trialsModel.gameState.ActiveDrops = trialsModel.gameState.ActiveDrops.Except(poppedDroplets).ToList();
                    }
                }
            }
            else
            {
                trialsModel.gameState.ActiveDrops.Clear();
            }

            if (!correct)
            {
                _audio.PlayClip(AudioClip.AnswerWrong);
                if(_session!=null)
                {
                    messenger.Send(MetadataCmd.StartTrial, this, trial => _trial = trial);
                    HandleTrialMetadata(trialsModel.gameState.ActiveDrops[0].transform.position, trialsModel.gameState.ActiveDrops[0].Equation.GetAnswer().ToString(), "F", "X");
                }
            }

            // Update stats
            trialsModel.totalTries++;
        }

        private void AddCorrectAnswer(float rspTime)
        {
            var store = Services.Get<IModelStoreService>();
            var settings = store.GetModel<SettingsModel>(SettingsModel.Key);

            trialsModel.gameState.TotalCorrect++;
            trialsModel.gameState.Points += settings.Asset.pointsPerDroplet;

            // Check whether we need to up the difficulty
            trialsModel.difficultyManager.CheckForNewDifficulty();

            // Calculate new response time
            trialsModel.avgCorrectResponseTime = ((trialsModel.avgCorrectResponseTime * trialsModel.gameState.TotalCorrect) + rspTime) / (trialsModel.gameState.TotalCorrect + 1);
        }

        public void SplashAllDrops()
        {
            _audio.PlayClip(AudioClip.DropletSplash);

            foreach (var obj in trialsModel.gameState.ActiveDrops)
            {
                if (obj is Droplet droplet)
                {
                    droplet.Splash();
                }
            }

            trialsModel.gameState.ActiveDrops.Clear();
            trialsModel.gameState.TotalHits++;
        }

        public void EvaporateAllDrops()
        {
            _audio.PlayClip(AudioClip.DropletBurst);

            foreach (Droplet droplet in trialsModel.gameState.ActiveDrops)
            {
                droplet.Evaporate();
            }
        }

        private void HandleTrialMetadata(Vector2 position, string problem, string response, string type)
        {
            _trial.SetField(TrialKeys.InputType, "T");
            _trial.SetField(TrialKeys.NumCorrections, trialsModel.lastCorrections);
            _trial.SetField(TrialKeys.NumDrops, trialsModel.gameState.ActiveDrops.Count);
            _trial.SetField(TrialKeys.Problem, problem);
            _trial.SetField(TrialKeys.Response, response);
            _trial.SetField(TrialKeys.Vertical, position.y);
            _trial.SetField(TrialKeys.Horizontal, position.x);
            _trial.SetField(TrialKeys.Type, type);
            _trial.SetField(TrialKeys.Speed, trialsModel.spawnModel.GetSpawnSpeed());
            _trial.SetField(TrialKeys.WaterLevel, gameplayView.water.Level);
            _trial.SetField(TrialKeys.SpawnPeriod, trialsModel.spawnModel.SpawnPeriod);
            _trial.SetField(TrialKeys.RegulateRate, trialsModel.gameState.DifficultyRegulateRate);
            messenger.Send(MetadataCmd.EndTrial, _trial);
        }

        private void HandleGameEnd()
        {
            gameplayView.KeypadDisappear();
            gameplayView.PipDisappear();
            gameplayView.BubblesAppear();
            var scoreModel = Services.Get<IModelStoreService>().GetModel<ScoreModel>(ScoreModel.StoreKey);
            scoreModel.Score = _score;
            gameplayView.OnUpdateEvent -= OnViewUpdate;
            _audio.PlayClip(AudioClip.GameEnd);
        }
    }
}
