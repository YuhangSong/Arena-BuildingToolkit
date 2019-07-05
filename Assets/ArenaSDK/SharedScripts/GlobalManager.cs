using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace Arena
{
    /// <summary>
    /// GlobalManager should be the parent of everything.
    /// </summary>
    public class GlobalManager : Academy
    {
        [Header("Global Environment Settings")][Space(10)]

        /// <summary>
        /// If debugging, arena will log more information.
        /// If not debugging, arena will log only necessary information. Also, arena will remove all objects with tag of Debug
        /// </summary>
        public bool Debugging = true;

        public enum AgentCameraDisplayModes {
            Single,
            All
        }

        /// <summary>
        /// If Single, only one agent camera is displayed, use key c to switch to others.
        /// If All, all agents' camera will be displayed
        /// </summary>
        public AgentCameraDisplayModes AgentCameraDisplayMode = AgentCameraDisplayModes.Single;

        /// <summary>
        /// Get Debugging
        /// </summary>
        public bool
        isDebugging()
        {
            return Debugging;
        }

        /// <summary>
        /// Set LightIntensityRandom to positive number so that all light in
        /// the game will be randomlized at each episode.
        /// </summary>
        public float LightIntensityRandom = 0f;

        /// <summary>
        /// A lsit of LightReinitializor to reinitialize all lights in the game.
        /// </summary>
        private List<LightReinitializor> LightReinitializors = new List<LightReinitializor>();

        /// <summary>
        /// Initialize LightReinitializors.
        /// </summary>
        private void
        InitLightReinitializors()
        {
            if (LightIntensityRandom > 0f) {
                foreach (Light light_ in GetComponentsInChildren<Light>()) {
                    LightReinitializors.Add(new LightReinitializor(light_, LightIntensityRandom));
                }
            }
        }

        /// <summary>
        /// Reinitialize LightReinitializors.
        /// </summary>
        private void
        ReinitilizeLightReinitializors()
        {
            foreach (LightReinitializor LightReinitializor_ in LightReinitializors) {
                LightReinitializor_.Reinitialize();
            }
        }

        /// <summary>
        /// Set objects you want to respawn at the reset of every
        /// episode, identified by tag.
        /// </summary>
        public List<string> RespawnTags = new List<string>();
        private List<TransformReinitializor> RespawnReinitializors = new List<TransformReinitializor>();

        /// <summary>
        /// Set objects you want to destroy at the reset of every
        /// episode, identified by tag.
        /// </summary>
        public List<string> DestroyTags = new List<string>();

        /// <summary>
        /// Reference to the GameObjects to be respawned at the reset of each episode.
        /// </summary>
        private List<GameObject> GameObjectToBeRespawned = new List<GameObject>();

        /// <summary>
        /// Destroy all GameObjects in DestroyTags.
        /// </summary>
        private void
        DestroyObjectsInDestroyTags()
        {
            foreach (string each_tag in DestroyTags) {
                foreach (GameObject each_gameobject in GameObject.FindGameObjectsWithTag(each_tag)) {
                    Destroy(each_gameobject.gameObject);
                }
            }
        }

        /// <summary>
        /// Respawn all GameObjects in GameObjectToBeRespawned.
        /// </summary>
        private void
        RespawnObjectsInTags()
        {
            foreach (GameObject each in GameObjectToBeRespawned) {
                each.SetActive(true);
                foreach (Rigidbody each_ in each.GetComponentsInChildren<Rigidbody>()) {
                    if (each_ != null) {
                        each_.velocity        = Vector3.zero;
                        each_.angularVelocity = Vector3.zero;
                    }
                }
            }
            foreach (TransformReinitializor each in RespawnReinitializors) {
                each.Reinitialize();
            }
        }

        /// <summary>
        /// Initialize GameObjectToBeRespawned by RespawnTags.
        /// </summary>
        private void
        InitGameObjectToBeRespawned()
        {
            foreach (string each_tag in RespawnTags) {
                foreach (GameObject each_gameobject in GameObject.FindGameObjectsWithTag(each_tag)) {
                    GameObjectToBeRespawned.Add(each_gameobject);
                    TransformReinitializor PlayerReinitializor = new TransformReinitializor(
                        each_gameobject,
                        Vector3.zero, Vector3.zero,
                        Vector3.zero, Vector3.zero,
                        Vector3.zero, Vector3.zero);
                    RespawnReinitializors.Add(PlayerReinitializor);
                }
            }
        }

        /// <summary>
        /// Set different materials in inspector for differnt materials used to identify differnt teams.
        /// </summary>
        public List<Material> TeamMaterials = new List<Material>();

        /// <summary>
        /// Set two color for different text color when agent is living and dead.
        /// The first color is the one used when agent is living.
        /// The second color is the one used when agent is dead.
        /// Depreciated
        /// </summary>
        public List<Color> StateTextColor = new List<Color>();

        /// <summary>
        /// Get the StateTextColor.
        /// </summary>
        /// <param name="Living_">
        /// <c>true</c>, if living, <c>false</c> otherwise.
        /// </param>
        public Color
        getStateTextColor(bool Living_)
        {
            if (Living_) {
                return StateTextColor[0];
            } else {
                return StateTextColor[1];
            }
        }

        [Header("Living Condition")][Space(10)]

        /// <summary>
        /// Condition at which the team is considerred to be living.
        /// </summary>
        public LivingConditions LivingCondition = LivingConditions.AtLeastOneLiving;

        /// <summary>
        /// Number of AtLeastSpecificNumberLiving in LivingCondition.
        /// </summary>
        public int AtLeastSpecificNumberLiving = 1;

        /// <summary>
        /// Portion of AtLeastSpecificPortionLiving in LivingCondition.
        /// </summary>
        public float AtLeastSpecificPortion = 0.5f;

        [Header("Global Agent Settings")][Space(10)]

        /// <summary>
        /// All ArenaAgents will by default use NumberOfActionsBetweenDecisions set in the GlobalManager.
        /// </summary>
        public int NumberOfActionsBetweenDecisions = 5;

        /// <summary>
        /// Get action space type.
        /// </summary>
        /// <returns>The the action space type.</returns>
        public SpaceType
        GetActionSpaceType()
        {
            return getAgentBrain().brainParameters.vectorActionSpaceType;
        }

        [Header("Reward Scheme")][Space(10)]

        /// <summary>
        /// Scale of RewardScheme at this level
        /// </summary>
        public float RewardSchemeScale = 100.0f;

        [Header("Reward Functions (Competitive)")][Space(10)]

        public bool IsRewardRanking = true;

        public RankingWinTypes RankingWinType;

        [Header("Reward Functions (Collaborative)")][Space(10)]

        public bool IsRewardTime = false;

        public TimeWinTypes TimeWinType = TimeWinTypes.Looger;

        [Header("Reward Function Properties")][Space(10)]

        /// <summary>
        /// Properrties of reward function.
        /// Coeffient for integrating different reward functions
        /// </summary>
        public float RewardRankingCoefficient = 1.0f;
        public float RewardTimeCoefficient    = 0.001f;

        [Header("Turn-based Game")][Space(10)]

        /// <summary>
        /// Set TurnDuration to -1.0f when the game is not turn based.
        /// Otherwise, set it as the duration for each turn (in seconds).
        /// </summary>
        public float TurnDuration = -1.0f;

        /// <summary>
        /// Get TurnDuration.
        /// </summary>
        /// <returns>TurnDuration.</returns>
        public float
        getTurnDuration()
        {
            return TurnDuration;
        }

        /// <summary>
        /// Check if this is a turn-based game.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if this is a turn-based game, <c>false</c> otherwise.
        /// </returns>
        public bool
        isTurnBasedGame()
        {
            return (TurnDuration > 0.0f);
        }

        /// <summary>
        /// For turn-based games
        /// TurnType:
        ///  SwitchTeamsFirst: first switching between teams, then switching between agents.
        ///  SwitchAgentsFirst: first switching between agents in a team, then switching between teams.
        /// </summary>
        public enum TurnTypes {
            SwitchTeamsFirst,
            SwitchAgentsFirst
        }

        /// <summary>
        /// See TurnTypes.
        /// </summary>
        public TurnTypes TurnType;

        /// <summary>
        /// Get TurnType.
        /// </summary>
        /// <returns>TurnType.</returns>
        public TurnTypes
        getTurnType()
        {
            return TurnType;
        }

        /// <summary>
        /// Kill a particular agent by TeamID and AgentID.
        /// </summary>
        /// <param name="TeamID_">TeamID of which the agent is killed.</param>
        /// <param name="AgentID_">AgentID of which the agent is killed.</param>
        public void
        KillAgent(int TeamID_, int AgentID_)
        {
            getTeam(TeamID_).getAgent(AgentID_).ReceiveSignal(AgentNextStates.Die, 0f);
        }

        /// <summary>
        /// Kill a particular team (killing all agents in the team) by TeamID.
        /// </summary>
        /// <param name="TeamID_">TeamID of which the team is killed.</param>
        public void
        KillTeam(int TeamID_)
        {
            getTeam(TeamID_).KillAllAgents();
        }

        /// <summary>
        /// reward all teams.
        /// </summary>
        /// <param name="NextReward_">The reward to be sent.</param>
        public void
        RewardAllTeams(float NextReward_)
        {
            for (int i = 0; i < getNumTeams(); i++) {
                getTeam(i).RewardAllAgents(NextReward_);
            }
        }

        /// <summary>
        /// Kill all teams except one team (specified by TeamID).
        /// </summary>
        /// <param name="TeamID_">TeamID of which the team survives.</param>
        public void
        KillAllTeamsExcept(int TeamID_)
        {
            for (int i = 0; i < getNumTeams(); i++) {
                if (i != TeamID_) {
                    getTeam(i).KillAllAgents();
                }
            }
        }

        /// <summary>
        /// Kill all teams.
        /// </summary>
        public void
        KillAllTeams()
        {
            for (int i = 0; i < getNumTeams(); i++) {
                getTeam(i).KillAllAgents();
            }
        }

        /// <summary>
        /// Kill all agents except one agent (specified by TeamID and AgentID).
        /// </summary>
        /// <param name="TeamID_">TeamID of which the agent survives.</param>
        /// <param name="AgentID_">AgentID of which the agent survives.</param>
        public void
        KillAllAgentsExcept(int TeamID_, int AgentID_)
        {
            for (int i = 0; i < getNumTeams(); i++) {
                if (i != TeamID_) {
                    getTeam(i).KillAllAgents();
                } else {
                    getTeam(i).KillAllAgentsExcept(AgentID_);
                }
            }
        }

        /// <summary>
        /// Initialize all teams.
        /// </summary>
        public void
        InitializeAllTeams()
        {
            for (int i = 0; i < getNumTeams(); i++) {
                getTeam(i).Initialize();
            }
        }

        /// <summary>
        /// Reset all teams.
        /// </summary>
        public void
        ResetAllTeams()
        {
            for (int i = 0; i < getNumTeams(); i++) {
                getTeam(i).Reset();
            }
        }

        /// <summary>
        /// Step all teams.
        /// </summary>
        public void
        StepAllTeams()
        {
            for (int i = 0; i < getNumTeams(); i++) {
                getTeam(i).Step();
            }

            float StepReward_ = 0f;

            if (IsRewardTime) {
                if (TimeWinType == TimeWinTypes.Looger) {
                    StepReward_ += RewardTimeCoefficient;
                } else if (TimeWinType == TimeWinTypes.Shorter) {
                    StepReward_ -= RewardTimeCoefficient;
                } else {
                    Debug.LogError("TimeWinType is invalid.");
                }
            }

            RewardAllTeams(StepReward_);
        }

        /// <summary>
        /// Get the TeamMaterial by TeamID.
        /// </summary>
        /// <param name="TeamID_">TeamID of which the TeamMaterial you want to get.</param>
        public Material
        getTeamMaterial(int TeamID_)
        {
            if (TeamID_ < TeamMaterials.Count) {
                return TeamMaterials[TeamID_];
            } else {
                Debug.LogError(
                    "Not enough TeamMaterials are assigned, requiring " + (TeamID_ + 1) + ", but got "
                    + TeamMaterials.Count);
                return null;
            }
        }

        /// <summary>
        /// Apply TeamMaterial of a team to a GameObject.
        /// Example: apply team material to a GameObject that is the child of an ArenaTeam
        //    GetComponentInParent<GlobalManager>().ApplyTeamMaterial(
        //      GetComponentInParent<ArenaTeam>().getTeamID(),
        //      GameObject_,
        //    )
        /// </summary>
        /// <param name="TeamID_">TeamID of which the TeamMaterial will be applied.</param>
        /// <param name="GameObject_">GameObject to be applied with TeamMaterial.</param>
        public void
        ApplyTeamMaterial(int TeamID_, GameObject GameObject_)
        {
            if (GameObject_.GetComponent<MeshRenderer>() != null) {
                // There is a MeshRenderer attached to the GameObject
                // only apply to this MeshRenderer
                GameObject_.GetComponent<MeshRenderer>().material = getTeamMaterial(TeamID_);
            } else if (GameObject_.GetComponent<SkinnedMeshRenderer>() != null) {
                GameObject_.GetComponent<SkinnedMeshRenderer>().material = getTeamMaterial(TeamID_);
            } else {
                Debug.LogWarning("There is no MeshRenderer attached to the GameObject");
            }
        }

        /// <summary>
        /// Get the NumberOfActionsBetweenDecisions.
        /// All ArenaAgents and Academy will by default use maxSteps set in the GlobalManager.
        /// </summary>
        /// <returns>NumberOfActionsBetweenDecisions.</returns>
        public int
        getNumberOfActionsBetweenDecisions()
        {
            return NumberOfActionsBetweenDecisions;
        }

        /// <summary>
        /// Get the Brain.
        /// All ArenaAgents will by default use Brain set in the GlobalManager.
        /// </summary>
        /// <returns>Brain.</returns>
        public Brain
        getAgentBrain()
        {
            if (broadcastHub.broadcastingBrains.Count != 1) {
                Debug.LogWarning(
                    "Please set one and only one brain in the inspector of GlobalManager/BroadcastHub, currently you set " + broadcastHub.broadcastingBrains.Count
                    + " brains.");
                return null;
            }
            return broadcastHub.broadcastingBrains[0];
        }

        /// <summary>
        /// Get TurnState.
        /// </summary>
        /// <returns>TurnState.</returns>
        public TurnStates
        getTurnState(int TeamID_, int AgentID_)
        {
            RunTurn();
            if (isSwitchingTurn()) {
                TurnStartTime = Time.time;
                return TurnStates.Switching;
            } else {
                if (isTurn(TeamID_, AgentID_)) {
                    return TurnStates.Turn;
                } else {
                    return TurnStates.NotTurn;
                }
            }
        }

        /// <summary>
        /// Message globalManager to keep
        /// current turn for another turn.
        /// </summary>
        public void
        KeepATurn()
        {
            WillKeepATurn = true;
        }

        /// <summary>
        /// Message globalManager to end this
        /// turn as soon as possible.
        /// </summary>
        public void
        EndATurn()
        {
            WillEndATurn = true;
        }

        /// <summary>
        /// Get asix where teams are aligned in the window.
        /// </summary>
        /// <returns>The asix where teams are aligned in the window.</returns>
        public ViewAxis
        getTeamViewAsix()
        {
            if (getNumTeams() >= getMaxNumAgentsPerTeam()) {
                return ViewAxis.X;
            } else {
                return ViewAxis.Y;
            }
        }

        /// <summary>
        /// Get size of the view port where teams are aligned in the window.
        /// </summary>
        /// <returns>The size of the view port where teams are aligned in the window.</returns>
        public float
        getTeamViewPortSize()
        {
            return (1f / (float) getNumTeams());
        }

        /// <summary>
        /// Get size of the view port where agents are aligned in the window.
        /// </summary>
        /// <returns>The size of the view port where agents are aligned in the window.</returns>
        public float
        getAgentViewPortSize()
        {
            return (1f / (float) getMaxNumAgentsPerTeam());
        }

        protected Dictionary<string, UIPercentageBar> UIPercentageBars = new Dictionary<string, UIPercentageBar>();

        protected List<Camera> Cameras = new List<Camera>();

        protected int DownerCameraDepth = 2;
        protected int UpperCameraDepth  = 3;

        /// <summary>
        /// Customize GlobalManager should override InitializeAcademy() and call base.InitializeAcademy() before adding
        /// customized code.
        /// </summary>
        public override void
        InitializeAcademy()
        {
            base.InitializeAcademy();

            tag = "GlobalManager";
            Debug.Log(getLogTag() + " Initialize");

            InitializeReferenceToTeams();
            InitGameObjectToBeRespawned();
            InitLightReinitializors();
            InitKilledRanking();
            InitializeRewardFunction();
            if (isTurnBasedGame()) {
                InitTurnBasedGame();
            }
            InitRamObservation();
            InitializeAllTeams();

            // initialize reference to UIPercentageBars
            foreach (UIPercentageBar UIPercentageBar_ in GameObject.FindGameObjectWithTag("TopDownCamera").
              GetComponentsInChildren<UIPercentageBar>())
            {
                UIPercentageBars.Add(UIPercentageBar_.ID, UIPercentageBar_);
            }

            UIPercentageBars["EL"].Enable();

            // initialize reference to Camera
            foreach (Camera Camera_ in GetComponentsInChildren<Camera>()) {
                string ID_;
                int InitialCameraDepth = 0;

                if (Camera_.CompareTag("TopDownCamera")) {
                    ID_ = "TopDownCamera";
                    InitialCameraDepth = UpperCameraDepth;

                    if (AgentCameraDisplayMode == AgentCameraDisplayModes.All) {
                        Debug.LogWarning(
                            "AgentCameraDisplayMode = AgentCameraDisplayModes.All is not recommended, as it results in camera feild that is too small and UI will be resized.");
                        Camera_.rect = new Rect(0.5f, 0f, 0.5f, 1f);
                    } else if (AgentCameraDisplayMode == AgentCameraDisplayModes.Single) {
                        Camera_.rect = new Rect(0f, 0f, 1f, 1f);
                    } else {
                        Debug.LogError("Invalid AgentCameraDisplayMode");
                    }
                } else if (Camera_.CompareTag("AgentCamera")) {
                    ID_ = Camera_.GetComponentInParent<ArenaAgent>().getLogTag();
                    InitialCameraDepth = DownerCameraDepth;
                } else {
                    Debug.LogError(
                        "A camera in Arena should be either TopDownCamera or AgentCamera, use corresponding prefab provided in ArenaSDK/SharedPrefabs");
                    ID_ = "None";
                }

                Cameras.Add(Camera_);
                Cameras[Cameras.Count - 1].depth = InitialCameraDepth;
            }

            if (!isDebugging()) {
                foreach (GameObject each_gameobject in GameObject.FindGameObjectsWithTag("Debug")) {
                    each_gameobject.SetActive(false);
                }
            }
        } // InitializeAcademy

        void
        OnGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown) {
                if (e.keyCode == KeyCode.F1) {
                    Debug.Log("Switch to next camera");
                    SwitchCamera(true);
                } else if (e.keyCode == KeyCode.F2) {
                    Debug.Log("Switch to previous camera");
                    SwitchCamera(false);
                }
            }
        }

        /// <summary>
        /// DisplayNextCamera.
        /// </summary>
        /// <param name="IsNext_">If switch to next camera or previous one.</param>
        protected void
        SwitchCamera(bool IsNext_)
        {
            int i = 0;
            int CurrentUpperCameraID = 0;

            foreach (Camera Camera_ in Cameras) {
                if (Camera_.depth == UpperCameraDepth) {
                    CurrentUpperCameraID = i;
                    Camera_.depth        = DownerCameraDepth;
                    break;
                }
                i++;
            }

            int NextUpperCameraID = 0;

            if (IsNext_) {
                if (CurrentUpperCameraID < (Cameras.Count - 1)) {
                    NextUpperCameraID = CurrentUpperCameraID + 1;
                } else if (CurrentUpperCameraID == (Cameras.Count - 1)) {
                    NextUpperCameraID = 0;
                } else {
                    Debug.LogError("Error");
                }
            } else {
                if (CurrentUpperCameraID > 0) {
                    NextUpperCameraID = CurrentUpperCameraID - 1;
                } else if (CurrentUpperCameraID == 0) {
                    NextUpperCameraID = (Cameras.Count - 1);
                } else {
                    Debug.LogError("Error");
                }
            }

            Cameras[NextUpperCameraID].depth = UpperCameraDepth;
        } // SwitchCamera

        /// <summary>
        /// Specifies the academy behavior at every step of the environment.
        /// </summary>
        public override void
        AcademyStep()
        {
            StepAllTeams();
            UIPercentageBars["EL"].UpdateValue(GetStepCount());
        }

        /// <summary>
        /// Called at an team die.
        /// </summary>
        public virtual void
        AtAnTeamDie()
        {
            CheckPopulationRanking();
        }

        /// <summary>
        /// Called at an team respawn.
        /// </summary>
        public virtual void
        AtAnTeamRespawn(){ }

        /// <summary>
        /// Customize GlobalManager should override AcademyReset() and call base.AcademyReset() before adding
        /// customized code.
        /// </summary>
        public override void
        AcademyReset()
        {
            base.AcademyReset();

            Debug.Log(getLogTag() + " Reset");

            UIPercentageBars["EL"].UpdateValue(GetStepCount());

            // respawn and destroy
            RespawnObjectsInTags();
            DestroyObjectsInDestroyTags();

            // reinitialize lights
            ReinitilizeLightReinitializors();

            // reset internal variables
            ResetKilledRanking();

            // reset turn-based game
            if (isTurnBasedGame()) {
                ResetTurnBasedGame();
            }

            // reset teams
            ResetAllTeams();
        }

        /// <summary>
        /// Implement this method for the condition under which the turn is switching.
        /// For example, check if all Rigidbody is sleep.
        /// </summary>
        virtual protected bool
        isSwitchingTurn()
        {
            return false;
        }

        /// <summary>
        /// Referece to all teams in this GlobalManager.
        /// </summary>
        private ArenaTeam[] Teams;

        /// <summary>
        /// Initialize referece to all teams in this GlobalManager.
        /// </summary>
        private void
        InitializeReferenceToTeams()
        {
            Teams = new ArenaTeam[getNumTeams()];
            foreach (ArenaTeam each in GetComponentsInChildren<ArenaTeam>()) {
                Teams[each.getTeamID()] = each;
            }
        }

        /// <summary>
        /// Get the reference to an team in this team by TeamID.
        /// </summary>
        /// <param name="TeamID_">The TeamID of the team you want to get.</param>
        /// <returns>The reference to the team in this GlobalManager.</returns>
        public ArenaTeam
        getTeam(int TeamID_)
        {
            if (Teams == null) {
                InitializeReferenceToTeams();
            }
            if (CheckValidTeamID(TeamID_)) {
                return Teams[TeamID_];
            } else {
                Debug.LogWarning("No TeamID " + TeamID_);
                return null;
            }
        }

        /// <summary>
        /// Get the TeamID by random.
        /// </summary>
        /// <returns>The random TeamID.</returns>
        private int
        getRandomTeamID()
        {
            return Random.Range(0, getNumTeams());
        }

        /// <summary>
        /// Check if an TeamID is valid or not.
        /// </summary>
        /// <param name="TeamID_">The TeamID you want to check.</param>
        /// <returns>
        /// <c>true</c>, if the TeamID_ is valid, <c>false</c> otherwise.
        /// </returns>
        private bool
        CheckValidTeamID(int TeamID_)
        {
            if ((TeamID_ >= 0) && (TeamID_ < getNumTeams())) {
                return true;
            } else {
                Debug.LogWarning("No TeamID " + TeamID_);
                return false;
            }
        }

        /// <summary>
        /// Get how many teams are still living.
        /// </summary>
        /// <returns>The number of teams that are still living.</returns>
        private int
        getNumLivingTeams()
        {
            int NumLivingTeam = 0;

            for (int Team_i = 0; Team_i < getNumTeams(); Team_i++) {
                if (getTeam(Team_i).isLiving()) {
                    NumLivingTeam += 1;
                }
            }
            return NumLivingTeam;
        }

        /// <summary>
        /// Get the total number of teams.
        /// </summary>
        /// <returns>The total number of teams.</returns>
        public int
        getNumTeams()
        {
            if (Teams == null) {
                int NumTeams = 0;
                foreach (ArenaTeam each in GetComponentsInChildren<ArenaTeam>()) {
                    NumTeams += 1;
                }
                return NumTeams;
            } else {
                return Teams.Length;
            }
        }

        /// <summary>
        /// Get the maximal number of agents per team.
        /// </summary>
        /// <returns>The maximal number of agents per team.</returns>
        public int
        getMaxNumAgentsPerTeam()
        {
            while (MaxNumAgentsPerTeam < 0) {
                ComputeMaxNumAgentsPerTeam();
            }
            return MaxNumAgentsPerTeam;
        }

        /// <summary>
        /// Get the view port size of an axis.
        /// </summary>
        /// <returns>The view port size of the axis.</returns>
        public float
        getViewPortSize(ViewAxis ViewAxis_)
        {
            if (getTeamViewAsix() == ViewAxis.X) {
                if (ViewAxis_ == ViewAxis.X) {
                    return getTeamViewPortSize() * 0.5f;
                } else {
                    return getAgentViewPortSize();
                }
            } else {
                if (ViewAxis_ == ViewAxis.X) {
                    return getAgentViewPortSize() * 0.5f;
                } else {
                    return getTeamViewPortSize();
                }
            }
        }

        /// <summary>
        /// Get the view port Rect of the agent axis.
        /// </summary>
        /// <returns>The view port Rect of the agent axis.</returns>
        public Rect
        getAgentViewPortRect(int TeamID_, int AgentID_)
        {
            if (AgentCameraDisplayMode == AgentCameraDisplayModes.All) {
                if (getTeamViewAsix() == ViewAxis.X) {
                    return new Rect(
                        TeamID_ * getViewPortSize(ViewAxis.X),
                        AgentID_ * getViewPortSize(ViewAxis.Y),
                        getViewPortSize(ViewAxis.X),
                        getViewPortSize(ViewAxis.Y)
                    );
                } else {
                    return new Rect(
                        AgentID_ * getViewPortSize(ViewAxis.X),
                        TeamID_ * getViewPortSize(ViewAxis.Y),
                        getViewPortSize(ViewAxis.X),
                        getViewPortSize(ViewAxis.Y)
                    );
                }
            } else if (AgentCameraDisplayMode == AgentCameraDisplayModes.Single) {
                return new Rect(0f, 0f, 1f, 1f);
            } else {
                Debug.LogError("Invalid AgentCameraDisplayMode");
                return new Rect(0f, 0f, 1f, 1f);
            }
        }

        /// <summary>
        /// Storage the current ranking of the killing (How many teams have been killed till now.).
        /// </summary>
        private int KilledRanking;

        /// <summary>
        /// Storage the ranking of each team being killed.
        /// Zero represents the agent is alive.
        /// Positive numbers represents the ranking of the agent being killed.
        /// </summary>
        private int[] KilledRankings;

        /// <summary>
        /// Initialize KilledRankings and KilledRanking.
        /// </summary>
        private void
        InitKilledRanking()
        {
            KilledRankings = new int[getNumTeams()];
            KilledRanking  = 0;
        }

        /// <summary>
        /// Reset KilledRankings and KilledRanking.
        /// </summary>
        private void
        ResetKilledRanking()
        {
            KilledRanking = 0;
            for (int Team_i = 0; Team_i < getNumTeams(); Team_i++) {
                KilledRankings[Team_i] = 0;
            }
        }

        /// <summary>
        /// Check if there is any agent that is living in the record KilledRanking, but dead when calling the method.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if there is any agent that is living in the record KilledRanking, but dead when calling the method, <c>false</c> otherwise.
        /// </returns>
        private bool
        isAnyTeamTransferToDead()
        {
            for (int Team_i = 0; Team_i < getNumTeams(); Team_i++) {
                if (KilledRankings[Team_i] == 0) {
                    // team still living in record
                    if (!getTeam(Team_i).isLiving()) {
                        // but now dead
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Record the KilledRanking of all the teams that is living in the record but dead when calling this method.
        /// </summary>
        /// <param name="KilledRanking_">The KilledRanking to be recorded.</param>
        private void
        RecordDeadTeamRanking(int KilledRanking_)
        {
            // check the living teams, if any team dead, set the in the record
            for (int Team_i = 0; Team_i < getNumTeams(); Team_i++) {
                if (KilledRankings[Team_i] == 0) {
                    // team still living in record
                    if (!getTeam(Team_i).isLiving()) {
                        // but now dead
                        KilledRankings[Team_i] = KilledRanking_;
                    }
                }
            }
        }

        /// <summary>
        /// The method will:
        ///  1, check if any team died since last time this method being called.
        ///  2, if there is, update KilledRanking, and update record KilledRankings.
        ///  2, check if there is only one or no agent living, and reset episode.
        /// </summary>
        private void
        CheckPopulationRanking()
        {
            // check any team dead in the living record, and update KilledRanking
            if (isAnyTeamTransferToDead()) {
                KilledRanking += 1;
                RecordDeadTeamRanking(KilledRanking);
            }

            if (!Utils.isLiving(LivingCondition, getNumLivingTeams(), getNumTeams(),
              AtLeastSpecificNumberLiving, AtLeastSpecificPortion))
            {
                KillAllTeams();
            }

            if (getNumLivingTeams() == 0) {
                RewardAndDone();
            }
        }

        /// <summary>
        /// The method will:
        ///  1, compute reward for each team (all agents).
        ///  2, set the computed reward and done signal to all agents.
        /// </summary>
        private void
        RewardAndDone()
        {
            if (IsRewardRanking) {
                for (int Team_i = 0; Team_i < getNumTeams(); Team_i++) {
                    float StepReward_ = 0f;

                    if (RankingWinType == RankingWinTypes.Survive) {
                        // Survive: dead ranking at 1 means reward 0, the higher the dead ranking, the better
                        StepReward_ += ((float) KilledRankings[Team_i] - 1f) * RewardRankingCoefficient;
                    } else if (RankingWinType == RankingWinTypes.Depart) {
                        // Depart: dead last (ranking getNumTeams()) means reward 0, the lower the dead ranking, the better
                        StepReward_ += ((float) getNumTeams() - (float) KilledRankings[Team_i])
                          * RewardRankingCoefficient;
                    } else {
                        Debug.LogError("RankingWinType is invalid.");
                    }

                    getTeam(Team_i).RewardAllAgents(StepReward_ * RewardSchemeScale);
                }
            }

            Done();
        }

        /// <summary>
        /// Get the log tag of the team.
        /// </summary>
        /// <returns>LogTag.</returns>
        private string
        getLogTag()
        {
            return tag;
        }

        /// <summary>
        /// Initialize turn-based game.
        /// </summary>
        private void
        InitTurnBasedGame()
        { }

        /// <summary>
        /// Reset turn-based game.
        /// </summary>
        private void
        ResetTurnBasedGame()
        {
            RandomCurrentTurn();
            TurnStartTime = Time.time;
        }

        /// <summary>
        /// TeamID of the team that is currently at turn.
        /// </summary>
        private int CurrentTurnTeamID;

        /// <summary>
        /// Set CurrentTurnTeamID.
        /// </summary>
        /// <param name="TeamID_">The CurrentTurnTeamID to be set.</param>
        private void
        setCurrentTurnTeamID(int TeamID_)
        {
            CurrentTurnTeamID = TeamID_;
        }

        /// <summary>
        /// Get CurrentTurnTeamID.
        /// </summary>
        /// <returns>The CurrentTurnTeamID.</returns>
        private int
        getCurrentTurnTeamID()
        {
            return CurrentTurnTeamID;
        }

        /// <summary>
        /// Check if a TeamID is CurrentTurnTeamID.
        /// </summary>
        /// <param name="TeamID_">The TeamID to be checked.</param>
        /// <returns>
        /// <c>true</c>, if the TeamID_ is CurrentTurnTeamID, <c>false</c> otherwise.
        /// </returns>
        private bool
        isCurrentTurnTeamID(int TeamID_)
        {
            return (getCurrentTurnTeamID() == TeamID_);
        }

        /// <summary>
        /// Randomlize current turn.
        /// </summary>
        private void
        RandomCurrentTurn()
        {
            setCurrentTurnTeamID(getRandomTeamID());
            getTeam(getCurrentTurnTeamID()).RandomCurrentAgentTurn();
        }

        /// <summary>
        /// Switch turn to next team by the order of TeamID.
        /// </summary>
        private void
        PushCurrentTurnTeamIDForward()
        {
            if (CurrentTurnTeamID < (getNumTeams() - 1)) {
                CurrentTurnTeamID += 1;
            } else {
                CurrentTurnTeamID = 0;
            }
        }

        /// <summary>
        /// Signal flag of if the next turn will be kept.
        /// </summary>
        private bool WillKeepATurn = false;

        /// <summary>
        /// Get WillKeepATurn.
        /// </summary>
        /// <returns>The WillKeepATurn.</returns>
        private bool
        isWillKeepATurn()
        {
            return WillKeepATurn;
        }

        /// <summary>
        /// Reset signal flag of WillKeepATurn.
        /// </summary>
        private void
        ResetWillKeepATurn()
        {
            WillKeepATurn = false;
        }

        /// <summary>
        /// Signal flag of if this turn will be end.
        /// </summary>
        private bool WillEndATurn = false;

        /// <summary>
        /// Get WillEndATurn.
        /// </summary>
        /// <returns>The WillEndATurn.</returns>
        private bool
        isWillEndATurn()
        {
            return WillEndATurn;
        }

        /// <summary>
        /// Reset signal flag of WillEndATurn.
        /// </summary>
        private void
        ResetWillEndATurn()
        {
            WillEndATurn = false;
        }

        /// <summary>
        /// Switch to next turn.
        /// </summary>
        private void
        SwitchTurn()
        {
            if (getTurnType() == TurnTypes.SwitchTeamsFirst) {
                PushCurrentTurnTeamIDForward();
                getTeam(getCurrentTurnTeamID()).NextTeamTurn(false);
            } else if (getTurnType() == TurnTypes.SwitchAgentsFirst) {
                if (getTeam(getCurrentTurnTeamID()).NextTeamTurn(true)) {
                    PushCurrentTurnTeamIDForward();
                }
            }
        }

        /// <summary>
        /// Get how far this turn has gone (by percentage).
        /// </summary>
        /// <returns>The TurnPercentage.</returns>
        public float
        getTurnPercentage()
        {
            float TurnPercentage = (Time.time - TurnStartTime) / getTurnDuration();

            if (TurnPercentage > 1f) {
                TurnPercentage = 1f;
            }
            return TurnPercentage;
        }

        /// <summary>
        /// The record of the time when current turn starts.
        /// </summary>
        private float TurnStartTime;

        /// <summary>
        /// Check if current turn has reached the duration limit.
        /// </summary>
        private bool
        isTurnDurationReached()
        {
            return (Time.time - TurnStartTime) > getTurnDuration();
        }

        /// <summary>
        /// Check if an agen is currently at turn by TeamID and AgentID.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the TeamID_ is CurrentTurnTeamID, <c>false</c> otherwise.
        /// </returns>
        private bool
        isTurn(int TeamID_, int AgentID_)
        {
            return (isCurrentTurnTeamID(TeamID_) && (getTeam(TeamID_).isCurrentTurnAgentID(AgentID_)));
        }

        /// <summary>
        /// This method is called repeatly by the agents to check if the turn stage has changed.
        /// </summary>
        private void
        RunTurn()
        {
            // time to SwitchTurn or receive signal to switch turn
            if (isTurnDurationReached() || isWillEndATurn()) {
                ResetWillEndATurn();
                if (isWillKeepATurn()) {
                    ResetWillKeepATurn();
                } else {
                    SwitchTurn();
                }
            }
        }

        /// <summary>
        /// The maximal number of agents per team.
        /// </summary>
        private int MaxNumAgentsPerTeam = -1;

        /// <summary>
        /// Compute the maximal number of agents per team.
        /// </summary>
        private void
        ComputeMaxNumAgentsPerTeam()
        {
            MaxNumAgentsPerTeam = 0;
            for (int i = 0; i < getNumTeams(); i++) {
                int NumAgents_ = getTeam(i).getNumTeams();
                if (NumAgents_ > MaxNumAgentsPerTeam) {
                    MaxNumAgentsPerTeam = NumAgents_;
                }
            }
        }

        /// <summary>
        /// Size of the ram observation.
        /// </summary>
        private int RamSize;

        /// <summary>
        /// Get RamSize.
        /// </summary>
        /// <returns>RamSize.</returns>
        public int
        getRamSize()
        {
            return RamSize;
        }

        /// <summary>
        /// Get GlobalRamSize, which is (RamSize - 2), since two places are left for TeamID and AgentID.
        /// </summary>
        /// <returns>RamSize.</returns>
        public int
        getGlobalRamSize()
        {
            return RamSize - 2;
        }

        /// <summary>
        /// Add information to Ram.
        /// </summary>
        virtual protected void
        AddRam()
        {
            // AddAllRigidBodiesToRam();
            // AddPopulationStateToRam();
        }

        /// <summary>
        /// Fill the rest of the ram with zeros.
        /// </summary>
        private void
        FinishRam()
        {
            if ((getGlobalRamSize() - Ram.Count) < 0) {
                Debug.LogWarning(getLogTag() + " Ram size overflow");
            } else if ((getGlobalRamSize() - Ram.Count) == 0) {
                // ram size fit
                return;
            } else if ((getGlobalRamSize() - Ram.Count) > 0) {
                // ram size underflow
                Ram.AddRange(new float[getGlobalRamSize() - Ram.Count]);
            }
        }

        /// <summary>
        /// The ram observation.
        /// </summary>
        protected List<float> Ram;

        /// <summary>
        /// Get the the ram observation.
        /// </summary>
        /// <returns>Ram observation.</returns>
        public List<float>
        getRam()
        {
            Ram.Clear();
            AddRam();
            FinishRam();
            return Ram;
        }

        /// <summary>
        /// Initialize ram observation.
        /// </summary>
        private void
        InitRamObservation()
        {
            if (getAgentBrain() == null) {
                RamSize = 1;
            } else {
                RamSize = getAgentBrain().brainParameters.vectorObservationSize;
            }
            Ram = new List<float>(getGlobalRamSize());
        }

        /// <summary>
        /// Add all Rigidbody information to Ram.
        /// </summary>
        protected void
        AddAllRigidBodiesToRam()
        {
            foreach (Rigidbody Rigidbody_ in GetComponentsInChildren<Rigidbody>()) {
                // public Vector3 position;
                Ram.Add(Rigidbody_.position.x);
                Ram.Add(Rigidbody_.position.y);
                Ram.Add(Rigidbody_.position.z);
                // public Quaternion rotation;
                Ram.Add(Rigidbody_.rotation.x);
                Ram.Add(Rigidbody_.rotation.y);
                Ram.Add(Rigidbody_.rotation.z);
                Ram.Add(Rigidbody_.rotation.w);
                // public Vector3 velocity;
                Ram.Add(Rigidbody_.velocity.x);
                Ram.Add(Rigidbody_.velocity.y);
                Ram.Add(Rigidbody_.velocity.z);
                // public Vector3 angularVelocity;
                Ram.Add(Rigidbody_.angularVelocity.x);
                Ram.Add(Rigidbody_.angularVelocity.y);
                Ram.Add(Rigidbody_.angularVelocity.z);
            }
        }

        /// <summary>
        /// Add population state to the Ram observation.
        /// </summary>
        protected void
        AddPopulationStateToRam()
        {
            for (int Team_i = 0; Team_i < getNumTeams(); Team_i++) {
                Ram.Add(Utils.bool2float(getTeam(Team_i).isLiving()));
                for (int Agent_i = 0; Agent_i < getTeam(Team_i).getNumTeams(); Agent_i++) {
                    Ram.Add(Utils.bool2float(getTeam(Team_i).getAgent(Agent_i).isLiving()));
                }
            }
        }

        /// <summary>
        /// Initlize reward function.
        /// 1, check if RewardFunction is valid;
        /// 2, initalize according to RewardFunction;
        /// Customize InitializeRewardFunction should override InitializeRewardFunction() and call base.InitializeRewardFunction() before adding customized code.
        /// </summary>
        protected virtual void
        InitializeRewardFunction()
        { }
    }
}
