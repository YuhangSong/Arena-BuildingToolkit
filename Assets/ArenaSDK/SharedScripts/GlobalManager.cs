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
        /// The maximal number of child ArenaNode for each parent ArenaNode
        /// </summary>
        public int MaxNumChildNodePerParentNode = 4;

        /// <summary>
        /// Getter of MaxNumChildNodePerParentNode.
        /// </summary>
        public int
        GetMaxNumChildNodePerParentNode()
        {
            return MaxNumChildNodePerParentNode;
        }

        /// <summary>
        /// The maximal depth of social tree
        /// </summary>
        public int MaxDepthSocialTree = 4;

        /// <summary>
        /// Getter of MaxDepthSocialTree.
        /// </summary>
        public int
        GetMaxDepthSocialTree()
        {
            return MaxDepthSocialTree;
        }

        /// <summary>
        /// Get maximal number of agents.
        /// </summary>
        public int
        GetMaxNumAgents()
        {
            return (int) Mathf.Pow(GetMaxNumChildNodePerParentNode(), GetMaxDepthSocialTree());
        }

        /// <summary>
        /// If debugging, arena will log more information.
        /// If not debugging, arena will log only necessary information. Also, arena will remove all objects with tag of Debug
        /// </summary>
        public bool Debugging = true;

        /// <summary>
        /// Get Debugging
        /// </summary>
        public bool
        isDebugging()
        {
            return Debugging;
        }

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
            if (RandomizeLightIntensity > 0f) {
                foreach (Light light_ in GetComponentsInChildren<Light>()) {
                    LightReinitializors.Add(new LightReinitializor(light_, RandomizeLightIntensity));
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
        /// Reinitialize MaterialReinitializors.
        /// </summary>
        private void
        ReinitilizeMaterialReinitializors()
        {
            foreach (MaterialReinitializor MaterialReinitializor_ in GetComponentsInChildren<MaterialReinitializor>()) {
                MaterialReinitializor_.Reinitialize();
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

        [System.Serializable]
        public class TagPair
        {
            public string TagA;
            public string TagB;
        }

        public List<TagPair> IgnoreCollisionTagPairs = new List<TagPair>();


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

        [Header("Global Randomization Settings")][Space(10)]

        /// <summary>
        /// Set RandomizeLightIntensity to positive number so that all light in
        /// the game will be randomlized at each episode.
        /// </summary>
        public float RandomizeLightIntensity = 0f;

        /// <summary>
        /// Set these values so that the corresponding physical properties will be randomized
        /// at the reset of every episode around the default value.
        /// </summary>
        [SerializeField]
        public MyDictionary.StringStringDictionary RandomizePhysicalProperties =
          new MyDictionary.StringStringDictionary(){
            { "mass", "0" },
            { "gravity", "0" },
            { "bounceThreshold", "0" },
            { "drag", "0" },
            { "angularDrag", "0" },
        };

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

        [Header("Reward Function Properties")][Space(10)]

        /// <summary>
        /// Properrties of reward function.
        /// Coeffient for integrating different reward functions
        /// Applies to all reward functions at all levels
        /// </summary>
        public float RewardRankingCoefficient      = 1.0f;
        public float RewardDistanceCoefficient     = 1.0f;
        public float RewardVelocityCoefficient     = 0.03f;
        public float RewardDirectionCoefficient    = 0.01f;
        public float RewardTimeCoefficient         = 0.001f;
        public float RewardShapeOfGroupCoefficient = 1.0f;

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
        /// Get the NumberOfActionsBetweenDecisions.
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

        protected Dictionary<string, UIPercentageBar> UIPercentageBarsTopDownCamera = new Dictionary<string,
            UIPercentageBar>();
        protected Dictionary<string, UIPercentageBar> UIPercentageBarsVisualizationCamera = new Dictionary<string,
            UIPercentageBar>();
        protected Dictionary<string, UIText> UITextsTopDownCamera       = new Dictionary<string, UIText>();
        protected Dictionary<string, UIText> UITextsVisualizationCamera = new Dictionary<string, UIText>();

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
            // Debug.Log(GetLogTag() + " Initialize");

            InitGameObjectToBeRespawned();
            InitLightReinitializors();

            foreach (TagPair TagPair_ in IgnoreCollisionTagPairs) {
                Utils.IgnoreCollision(TagPair_.TagA, TagPair_.TagB);
            }

            // InitRamObservation();

            // if (isTurnBasedGame()) {
            //     InitTurnBasedGame();
            // }

            // initialize reference of TopDownCamera
            if (GameObject.FindGameObjectWithTag("TopDownCamera") == null) {
                Debug.LogError("Add prefab ArenaSDK/SharedPrefabs/TopDownCamera in your game sence");
            }

            // initialize reference to UIPercentageBarsTopDownCamera
            foreach (UIPercentageBar UIPercentageBar_ in GameObject.FindGameObjectWithTag("TopDownCamera").
              GetComponentsInChildren<UIPercentageBar>())
            {
                UIPercentageBarsTopDownCamera.Add(UIPercentageBar_.ID, UIPercentageBar_);
            }
            UIPercentageBarsTopDownCamera["Episode Length"].Enable();

            // initialize reference to UITextsTopDownCamera
            foreach (UIText UIText_ in GameObject.FindGameObjectWithTag("TopDownCamera").
              GetComponentsInChildren<UIText>())
            {
                UITextsTopDownCamera.Add(UIText_.ID, UIText_);
            }
            UITextsTopDownCamera["Status"].setText("Top Down Camera");

            // initialize reference to VisualizationCamera
            if (GameObject.FindGameObjectWithTag("VisualizationCamera") == null) {
                Debug.LogError("Add prefab ArenaSDK/SharedPrefabs/VisualizationCamera in your game sence");
            }

            // initialize reference to UIPercentageBarsVisualizationCamera
            foreach (UIPercentageBar UIPercentageBar_ in GameObject.FindGameObjectWithTag("VisualizationCamera").
              GetComponentsInChildren<UIPercentageBar>())
            {
                UIPercentageBarsVisualizationCamera.Add(UIPercentageBar_.ID, UIPercentageBar_);
            }
            UIPercentageBarsVisualizationCamera["Episode Length"].Enable();

            // initialize reference to UITextsVisualizationCamera
            foreach (UIText UIText_ in GameObject.FindGameObjectWithTag("VisualizationCamera").
              GetComponentsInChildren<UIText>())
            {
                UITextsVisualizationCamera.Add(UIText_.ID, UIText_);
            }
            UITextsVisualizationCamera["Status"].setText("Visualization Camera");

            // initialize reference to Camera
            foreach (Camera Camera_ in GetComponentsInChildren<Camera>()) {
                string ID_;
                int InitialCameraDepth = 0;

                if (Camera_.CompareTag("TopDownCamera")) {
                    ID_ = "TopDownCamera";
                    InitialCameraDepth = UpperCameraDepth;
                    Camera_.rect       = new Rect(0f, 0f, 1f, 1f);
                } else if (Camera_.CompareTag("VisualizationCamera")) {
                    ID_ = "VisualizationCamera";
                    InitialCameraDepth = DownerCameraDepth;
                    Camera_.rect       = new Rect(0f, 0f, 1f, 1f);
                } else if (Camera_.CompareTag("AgentCamera")) {
                    ID_ = Camera_.GetComponentInParent<ArenaAgent>().GetLogTag();
                    InitialCameraDepth = DownerCameraDepth;
                } else {
                    Debug.LogError(
                        "A camera in Arena should be either TopDownCamera or VisualizationCamera or AgentCamera, use corresponding prefab provided in ArenaSDK/SharedPrefabs");
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
                    // Debug.Log("Switch to next camera");
                    SwitchCamera(true);
                } else if (e.keyCode == KeyCode.F2) {
                    // Debug.Log("Switch to previous camera");
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
            gameObject.GetComponent<ArenaNode>().Step();
            UIPercentageBarsTopDownCamera["Episode Length"].UpdateValue(GetComponent<ArenaNode>().GetNumLivingSteps());
            UIPercentageBarsVisualizationCamera["Episode Length"].UpdateValue(
                GetComponent<ArenaNode>().GetNumLivingSteps());
        }

        /// <summary>
        /// Customize GlobalManager should override AcademyReset() and call base.AcademyReset() before adding
        /// customized code.
        /// </summary>
        public override void
        AcademyReset()
        {
            base.AcademyReset();

            // Debug.Log(GetLogTag() + " Reset");

            UIPercentageBarsTopDownCamera["Episode Length"].UpdateValue(GetComponent<ArenaNode>().GetNumLivingSteps());
            UIPercentageBarsVisualizationCamera["Episode Length"].UpdateValue(
                GetComponent<ArenaNode>().GetNumLivingSteps());

            // respawn and destroy
            RespawnObjectsInTags();
            DestroyObjectsInDestroyTags();

            // reinitialize lights, materials
            ReinitilizeLightReinitializors();
            ReinitilizeMaterialReinitializors();

            gameObject.GetComponent<ArenaNode>().Reset();

            // reset all EventGate
            foreach (EventGate EventGate_ in gameObject.GetComponentsInChildren<EventGate>()) {
                EventGate_.Reset();
            }

            // reset all MazeSpawner
            foreach (MazeSpawner MazeSpawner_ in gameObject.GetComponentsInChildren<MazeSpawner>()) {
                MazeSpawner_.Reset();
            }

            // // reset turn-based game
            // if (isTurnBasedGame()) {
            //     ResetTurnBasedGame();
            // }
        } // AcademyReset

        /// <summary>
        /// Get the view port Rect of the agent axis.
        /// </summary>
        /// <returns>The view port Rect of the agent axis.</returns>
        public Rect
        getAgentViewPortRect(int TeamID_, int AgentID_)
        {
            return new Rect(0f, 0f, 1f, 1f);
        }

        /// <summary>
        /// Get the log tag of the team.
        /// </summary>
        /// <returns>LogTag.</returns>
        private string
        GetLogTag()
        {
            return tag;
        }

        // /// <summary>
        // /// Size of the ram observation.
        // /// </summary>
        // private int RamSize;
        //
        // /// <summary>
        // /// Get RamSize.
        // /// </summary>
        // /// <returns>RamSize.</returns>
        // public int
        // getRamSize()
        // {
        //     return RamSize;
        // }
        //
        // /// <summary>
        // /// Get GlobalRamSize, which is (RamSize - 2), since two places are left for TeamID and AgentID.
        // /// </summary>
        // /// <returns>RamSize.</returns>
        // public int
        // getGlobalRamSize()
        // {
        //     return RamSize - 2;
        // }
        //
        // /// <summary>
        // /// Add information to Ram.
        // /// </summary>
        // virtual protected void
        // AddRam()
        // {
        //     // AddAllRigidBodiesToRam();
        //     // AddPopulationStateToRam();
        // }
        //
        // /// <summary>
        // /// Fill the rest of the ram with zeros.
        // /// </summary>
        // private void
        // FinishRam()
        // {
        //     if ((getGlobalRamSize() - Ram.Count) < 0) {
        //         Debug.LogWarning(GetLogTag() + " Ram size overflow");
        //     } else if ((getGlobalRamSize() - Ram.Count) == 0) {
        //         // ram size fit
        //         return;
        //     } else if ((getGlobalRamSize() - Ram.Count) > 0) {
        //         // ram size underflow
        //         Ram.AddRange(new float[getGlobalRamSize() - Ram.Count]);
        //     }
        // }
        //
        // /// <summary>
        // /// The ram observation.
        // /// </summary>
        // protected List<float> Ram;
        //
        // /// <summary>
        // /// Get the the ram observation.
        // /// </summary>
        // /// <returns>Ram observation.</returns>
        // public List<float>
        // getRam()
        // {
        //     Ram.Clear();
        //     AddRam();
        //     FinishRam();
        //     return Ram;
        // }
        //
        // /// <summary>
        // /// Initialize ram observation.
        // /// </summary>
        // private void
        // InitRamObservation()
        // {
        //     if (getAgentBrain() == null) {
        //         RamSize = 1;
        //     } else {
        //         RamSize = getAgentBrain().brainParameters.vectorObservationSize;
        //     }
        //     Ram = new List<float>(getGlobalRamSize());
        // }
        //
        // /// <summary>
        // /// Add all Rigidbody information to Ram.
        // /// </summary>
        // protected void
        // AddAllRigidBodiesToRam()
        // {
        //     foreach (Rigidbody Rigidbody_ in GetComponentsInChildren<Rigidbody>()) {
        //         // public Vector3 position;
        //         Ram.Add(Rigidbody_.position.x);
        //         Ram.Add(Rigidbody_.position.y);
        //         Ram.Add(Rigidbody_.position.z);
        //         // public Quaternion rotation;
        //         Ram.Add(Rigidbody_.rotation.x);
        //         Ram.Add(Rigidbody_.rotation.y);
        //         Ram.Add(Rigidbody_.rotation.z);
        //         Ram.Add(Rigidbody_.rotation.w);
        //         // public Vector3 velocity;
        //         Ram.Add(Rigidbody_.velocity.x);
        //         Ram.Add(Rigidbody_.velocity.y);
        //         Ram.Add(Rigidbody_.velocity.z);
        //         // public Vector3 angularVelocity;
        //         Ram.Add(Rigidbody_.angularVelocity.x);
        //         Ram.Add(Rigidbody_.angularVelocity.y);
        //         Ram.Add(Rigidbody_.angularVelocity.z);
        //     }
        // }
        //
        // /// <summary>
        // /// Add population state to the Ram observation.
        // /// </summary>
        // protected void
        // AddPopulationStateToRam()
        // {
        //     for (int Team_i = 0; Team_i < getNumTeams(); Team_i++) {
        //         Ram.Add(Utils.bool2float(getTeam(Team_i).isLiving()));
        //         for (int Agent_i = 0; Agent_i < getTeam(Team_i).getNumTeams(); Agent_i++) {
        //             Ram.Add(Utils.bool2float(getTeam(Team_i).getTeam(Agent_i).isLiving()));
        //         }
        //     }
        // }

        // [Header("Turn-based Game")][Space(10)]
        //
        // /// <summary>
        // /// Set TurnDuration to -1.0f when the game is not turn based.
        // /// Otherwise, set it as the duration for each turn (in seconds).
        // /// </summary>
        // public float TurnDuration = -1.0f;
        //
        // /// <summary>
        // /// Get TurnDuration.
        // /// </summary>
        // /// <returns>TurnDuration.</returns>
        // public float
        // getTurnDuration()
        // {
        //     return TurnDuration;
        // }
        //
        // /// <summary>
        // /// Check if this is a turn-based game.
        // /// </summary>
        // /// <returns>
        // /// <c>true</c>, if this is a turn-based game, <c>false</c> otherwise.
        // /// </returns>
        // public bool
        // isTurnBasedGame()
        // {
        //     return (TurnDuration > 0.0f);
        // }
        //
        // /// <summary>
        // /// For turn-based games
        // /// TurnType:
        // ///  SwitchTeamsFirst: first switching between teams, then switching between agents.
        // ///  SwitchAgentsFirst: first switching between agents in a team, then switching between teams.
        // /// </summary>
        // public enum TurnTypes {
        //     SwitchTeamsFirst,
        //     SwitchAgentsFirst
        // }
        //
        // /// <summary>
        // /// See TurnTypes.
        // /// </summary>
        // public TurnTypes TurnType;
        //
        // /// <summary>
        // /// Get TurnType.
        // /// </summary>
        // /// <returns>TurnType.</returns>
        // public TurnTypes
        // getTurnType()
        // {
        //     return TurnType;
        // }
        //
        // /// <summary>
        // /// Message globalManager to keep
        // /// current turn for another turn.
        // /// </summary>
        // public void
        // KeepATurn()
        // {
        //     WillKeepATurn = true;
        // }
        //
        // /// <summary>
        // /// Message globalManager to end this
        // /// turn as soon as possible.
        // /// </summary>
        // public void
        // EndATurn()
        // {
        //     WillEndATurn = true;
        // }
        //
        // /// <summary>
        // /// Implement this method for the condition under which the turn is switching.
        // /// For example, check if all Rigidbody is sleep.
        // /// </summary>
        // virtual protected bool
        // isSwitchingTurn()
        // {
        //     return false;
        // }
        //
        // /// <summary>
        // /// Initialize turn-based game.
        // /// </summary>
        // private void
        // InitTurnBasedGame()
        // { }
        //
        // /// <summary>
        // /// Reset turn-based game.
        // /// </summary>
        // private void
        // ResetTurnBasedGame()
        // {
        //     RandomCurrentTurn();
        //     TurnStartTime = Time.time;
        // }
        //
        // /// <summary>
        // /// TeamID of the team that is currently at turn.
        // /// </summary>
        // private int CurrentTurnTeamID;
        //
        // /// <summary>
        // /// Set CurrentTurnTeamID.
        // /// </summary>
        // /// <param name="TeamID_">The CurrentTurnTeamID to be set.</param>
        // private void
        // setCurrentTurnTeamID(int TeamID_)
        // {
        //     CurrentTurnTeamID = TeamID_;
        // }
        //
        // /// <summary>
        // /// Get CurrentTurnTeamID.
        // /// </summary>
        // /// <returns>The CurrentTurnTeamID.</returns>
        // private int
        // getCurrentTurnTeamID()
        // {
        //     return CurrentTurnTeamID;
        // }
        //
        // /// <summary>
        // /// Check if a TeamID is CurrentTurnTeamID.
        // /// </summary>
        // /// <param name="TeamID_">The TeamID to be checked.</param>
        // /// <returns>
        // /// <c>true</c>, if the TeamID_ is CurrentTurnTeamID, <c>false</c> otherwise.
        // /// </returns>
        // private bool
        // isCurrentTurnTeamID(int TeamID_)
        // {
        //     return (getCurrentTurnTeamID() == TeamID_);
        // }
        //
        // /// <summary>
        // /// Randomlize current turn.
        // /// </summary>
        // private void
        // RandomCurrentTurn()
        // {
        //     setCurrentTurnTeamID(getRandomTeamID());
        //     getTeam(getCurrentTurnTeamID()).RandomCurrentTeamTurn();
        // }
        //
        // /// <summary>
        // /// Switch turn to next team by the order of TeamID.
        // /// </summary>
        // private void
        // PushCurrentTurnTeamIDForward()
        // {
        //     if (CurrentTurnTeamID < (getNumTeams() - 1)) {
        //         CurrentTurnTeamID += 1;
        //     } else {
        //         CurrentTurnTeamID = 0;
        //     }
        // }
        //
        // /// <summary>
        // /// Signal flag of if the next turn will be kept.
        // /// </summary>
        // private bool WillKeepATurn = false;
        //
        // /// <summary>
        // /// Get WillKeepATurn.
        // /// </summary>
        // /// <returns>The WillKeepATurn.</returns>
        // private bool
        // isWillKeepATurn()
        // {
        //     return WillKeepATurn;
        // }
        //
        // /// <summary>
        // /// Reset signal flag of WillKeepATurn.
        // /// </summary>
        // private void
        // ResetWillKeepATurn()
        // {
        //     WillKeepATurn = false;
        // }
        //
        // /// <summary>
        // /// Signal flag of if this turn will be end.
        // /// </summary>
        // private bool WillEndATurn = false;
        //
        // /// <summary>
        // /// Get WillEndATurn.
        // /// </summary>
        // /// <returns>The WillEndATurn.</returns>
        // private bool
        // isWillEndATurn()
        // {
        //     return WillEndATurn;
        // }
        //
        // /// <summary>
        // /// Reset signal flag of WillEndATurn.
        // /// </summary>
        // private void
        // ResetWillEndATurn()
        // {
        //     WillEndATurn = false;
        // }
        //
        // /// <summary>
        // /// Switch to next turn.
        // /// </summary>
        // private void
        // SwitchTurn()
        // {
        //     if (getTurnType() == TurnTypes.SwitchTeamsFirst) {
        //         PushCurrentTurnTeamIDForward();
        //         getTeam(getCurrentTurnTeamID()).NextTeamTurn(false);
        //     } else if (getTurnType() == TurnTypes.SwitchAgentsFirst) {
        //         if (getTeam(getCurrentTurnTeamID()).NextTeamTurn(true)) {
        //             PushCurrentTurnTeamIDForward();
        //         }
        //     }
        // }
        //
        // /// <summary>
        // /// Get how far this turn has gone (by percentage).
        // /// </summary>
        // /// <returns>The TurnPercentage.</returns>
        // public float
        // getTurnPercentage()
        // {
        //     float TurnPercentage = (Time.time - TurnStartTime) / getTurnDuration();
        //
        //     if (TurnPercentage > 1f) {
        //         TurnPercentage = 1f;
        //     }
        //     return TurnPercentage;
        // }
        //
        // /// <summary>
        // /// The record of the time when current turn starts.
        // /// </summary>
        // private float TurnStartTime;
        //
        // /// <summary>
        // /// Check if current turn has reached the duration limit.
        // /// </summary>
        // private bool
        // isTurnDurationReached()
        // {
        //     return (Time.time - TurnStartTime) > getTurnDuration();
        // }
        //
        // /// <summary>
        // /// This method is called repeatly by the agents to check if the turn stage has changed.
        // /// </summary>
        // private void
        // RunTurn()
        // {
        //     // time to SwitchTurn or receive signal to switch turn
        //     if (isTurnDurationReached() || isWillEndATurn()) {
        //         ResetWillEndATurn();
        //         if (isWillKeepATurn()) {
        //             ResetWillKeepATurn();
        //         } else {
        //             SwitchTurn();
        //         }
        //     }
        // }
        //
        // /// <summary>
        // /// The maximal number of agents per team.
        // /// </summary>
        // private int MaxNumAgentsPerTeam = -1;
        //
        // /// <summary>
        // /// Compute the maximal number of agents per team.
        // /// </summary>
        // private void
        // ComputeMaxNumAgentsPerTeam()
        // {
        //     MaxNumAgentsPerTeam = 0;
        //     for (int i = 0; i < getNumTeams(); i++) {
        //         int NumAgents_ = getTeam(i).getNumTeams();
        //         if (NumAgents_ > MaxNumAgentsPerTeam) {
        //             MaxNumAgentsPerTeam = NumAgents_;
        //         }
        //     }
        // }
    }
}
