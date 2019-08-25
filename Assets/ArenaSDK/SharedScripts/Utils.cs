using UnityEngine;
using System.Collections.Generic;

namespace Arena
{
    /// <summary>
    /// AgentNextStates:
    ///  None:
    ///  Done:
    ///  Die:
    /// </summary>
    public enum AgentNextStates {
        None,
        Done,
        Die
    }

    public enum ComparingObjectTypes {
        self,
        other
    }

    /// <summary>
    /// How to switch between child:
    ///   Sequence: according to the sequence of ID
    /// </summary>
    public enum ChildSwitchTypes {
        Sequence,
    }

    /// <summary>
    /// Turn state of an agent.
    /// </summary>
    public enum TurnStates {
        NotTurn,
        Switching,
        Turn
    }

    /// <summary>
    /// Axis of a view port.
    /// </summary>
    public enum ViewAxis {
        X,
        Y
    }

    /// <summary>
    /// RankingWinTypes:
    ///  Survive means the team survive longer gets higher reward.
    ///  Depart means the team dies earlier gets higher reward.
    /// </summary>
    public enum RankingWinTypes {
        Survive,
        Depart
    }

    /// <summary>
    /// TimeWinTypes:
    ///  Looger means the one alive longer gets higher reward.
    ///  Depart means the one alive shorter gets higher reward.
    /// </summary>
    public enum TimeWinTypes {
        Looger,
        Shorter
    }

    /// <summary>
    /// Condition at which the team is considerred to be living.
    ///   AllLiving: This team is living when all ArenaNode are living in this team.
    ///   AtLeastOneLiving: This team is living when there is at least one ArenaNode living in this team.
    /// </summary>
    public enum LivingConditions {
        AllLiving,
        AtLeastOneLiving,
        AtLeastSpecificNumberLiving,
        AtLeastSpecificPortionLiving
    }

    [System.Serializable]
    public class Attribute
    {
        public string Key;
        public float Value;
    }

    public struct LineDrawer {
        private LineRenderer lineRenderer;
        private float        lineSize;

        public LineDrawer(float lineSize = 0.2f)
        {
            GameObject lineObj = new GameObject("LineObj");

            lineRenderer = lineObj.AddComponent<LineRenderer>();
            // Particles/Additive
            lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

            this.lineSize = lineSize;
        }

        private void
        init(float lineSize = 0.1f)
        {
            if (lineRenderer == null) {
                GameObject lineObj = new GameObject("LineObj");
                lineRenderer = lineObj.AddComponent<LineRenderer>();
                // Particles/Additive
                lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

                this.lineSize = lineSize;
            }
        }

        // Draws lines through the provided vertices
        public void
        DrawLineInGameView(Vector3 start, Vector3 end, Color color)
        {
            if (lineRenderer == null) {
                init(0.2f);
            }

            // Set color
            lineRenderer.startColor = color;
            lineRenderer.endColor   = color;

            // Set width
            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth   = lineSize;

            // Set line count which is 2
            lineRenderer.positionCount = 2;

            // Set the postion of both two lines
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        public void
        Destroy()
        {
            if (lineRenderer != null) {
                UnityEngine.Object.Destroy(lineRenderer.gameObject);
            }
        }
    }

    static public class Utils
    {
        public static bool
        IsListEqual(List<int> ListA, List<int> ListB, int Count)
        {
            for (int i = 0; i < Count; i++) {
                if (ListA[i] != ListB[i]) {
                    return false;
                }
            }
            return true;
        }

        public static bool
        GetBit(uint x, int index)
        {
            return (x & (1 << index)) != 0;
        }

        public static void
        ApplyMaterial(Material Material_, GameObject GameObject_)
        {
            if (GameObject_.GetComponent<MeshRenderer>() != null) {
                // There is a MeshRenderer attached to the GameObject
                // only apply to this MeshRenderer
                GameObject_.GetComponent<MeshRenderer>().material = Material_;
            } else if (GameObject_.GetComponent<SkinnedMeshRenderer>() != null) {
                GameObject_.GetComponent<SkinnedMeshRenderer>().material = Material_;
            } else {
                Debug.LogWarning("There is no MeshRenderer attached to the GameObject");
            }
        }

        public static List<ArenaNode>
        GetTopLevelArenaNodesInChildren(GameObject GameObject_)
        {
            List<ArenaNode> ArenaNodes = new List<ArenaNode>();

            for (int ID = 0; ID < GameObject_.transform.childCount; ID++) {
                ArenaNode ArenaNode_ = GameObject_.transform.GetChild(ID).gameObject.GetComponent<ArenaNode>();
                if (ArenaNode_ != null) {
                    ArenaNodes.Add(ArenaNode_);
                }
            }

            return ArenaNodes;
        }

        public static ArenaNode
        GetBottomLevelArenaNodeInGameObject(GameObject GameObject_)
        {
            Transform parent_ = GameObject_.transform.parent;

            if (parent_ != null) {
                ArenaNode ArenaNode_ = parent_.gameObject.GetComponent<ArenaNode>();

                if (ArenaNode_ != null) {
                    return ArenaNode_;
                } else {
                    return GetBottomLevelArenaNodeInGameObject(parent_.gameObject);
                }
            } else {
                return null;
            }
        }

        /// <summary>
        /// Set all TextMesh in child with a text.
        /// </summary>
        /// <param name="GameObject_">The GameObject of which the text will be set.</param>
        /// <param name="Text_">The text to be set.</param>
        public static void
        TextAllTextMeshesInChild(GameObject GameObject_, string Text_)
        {
            foreach (TextMesh TextMesh_ in GameObject_.GetComponentsInChildren<TextMesh>()) {
                TextMesh_.text = Text_;
            }
        }

        public static float[]
        ClampNumberList(float[] NumberList_, float Min_, float Max_)
        {
            for (int i = 0; i < NumberList_.Length; i++) {
                NumberList_[i] = ClampNumber(NumberList_[i], Min_, Max_);
            }
            return NumberList_;
        }

        public static float
        ClampNumber(float Number_, float Min_, float Max_)
        {
            return (Number_ < Min_) ? Min_ : (Number_ > Max_) ? Max_ : Number_;
        }

        /// <summary>
        /// Sum numbers in an Array.
        /// </summary>
        /// <returns>The sum.</returns>
        static public int
        SumArray(int[] Array_)
        {
            int sum = 0;

            foreach (int item in Array_) {
                sum += item;
            }
            return sum;
        }

        /// <summary>
        /// Check if all rigidbodies are sleeping in a tag.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if all rigidbodies are sleeping in the tag, <c>false</c> otherwise.
        /// </returns>
        static public bool
        isAllRigidbodySleepingInTag(string Tag_)
        {
            foreach (GameObject GameObject_ in GameObject.FindGameObjectsWithTag(Tag_)) {
                foreach (Rigidbody Rigidbody_ in GameObject_.GetComponentsInChildren<Rigidbody>()) {
                    if (!Rigidbody_.IsSleeping()) {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Check if all rigidbodies are sleeping in a list of tags.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if all rigidbodies are sleeping in the list of tags, <c>false</c> otherwise.
        /// </returns>
        static public bool
        isAllRigidbodySleepingInTags(string[] Tags_)
        {
            foreach (string Tag_ in Tags_) {
                if (!isAllRigidbodySleepingInTag(Tag_)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Generate a random sign (float).
        /// </summary>
        /// <returns>
        /// Could be 1f or -1f.
        /// </returns>
        public static float
        RandomSign_Float()
        {
            return (Random.Range(0, 2) - 0.5f) * 2f;
        }

        /// <summary>
        /// Make an GameObject transparent.
        /// </summary>
        /// <param name="GameObject_">The GameObject that will be make transparent.</param>
        public static void
        TransparentObject(GameObject GameObject_)
        {
            // make a copy of the material, since we will change the render mode
            Material material_ = new Material(GameObject_.GetComponent<MeshRenderer>().material);

            ChangeRenderMode(material_, BlendMode.Transparent);
            // change color to Transparent
            Color color_ = material_.color;
            color_.a        = 0.6f;
            material_.color = color_;
            // set material back to object
            GameObject_.GetComponent<MeshRenderer>().material = material_;
        }

        /// <summary>
        /// All BlendModes supported by ChangeRenderMode.
        /// </summary>
        public enum BlendMode {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }

        /// <summary>
        /// Ignore collision between two (group of) GameObject(s) by tag.
        /// </summary>
        static public void
        IgnoreCollision(string ATag, string BTag)
        {
            foreach (GameObject A_ in GameObject.FindGameObjectsWithTag(ATag)) {
                IgnoreCollision(A_, BTag);
            }
        }

        /// <summary>
        /// Convert bool to float.
        /// </summary>
        static public float
        bool2float(bool bool_)
        {
            return (bool_ ? 1f : 0f);
        }

        /// <summary>
        /// Ignore collision between a GameObject (by reference) and a (group of) GameObject(s) by tag.
        /// </summary>
        /// <param name="GameObject_">Reference to the GameObject.</param>
        /// <param name="Tag_">The Tag.</param>
        static public void
        IgnoreCollision(GameObject GameObject_, string Tag_)
        {
            foreach (GameObject GameObject_Tag_ in GameObject.FindGameObjectsWithTag(Tag_)) {
                if (GameObject_.GetComponent<Collider>() != null && GameObject_Tag_.GetComponent<Collider>() != null) {
                    Physics.IgnoreCollision(GameObject_.GetComponent<Collider>(),
                      GameObject_Tag_.GetComponent<Collider>());
                }
                foreach (Collider Ac_ in GameObject_.GetComponentsInChildren<Collider>()) {
                    foreach (Collider Bc_ in GameObject_Tag_.GetComponentsInChildren<Collider>()) {
                        Physics.IgnoreCollision(Ac_, Bc_);
                    }
                }
            }
        }

        /// <summary>
        /// Changing render mode at runtime.
        /// This is a ugly hack, but unity does not support change the render mode in a singel line,
        /// so the code here is the only option for now.
        /// </summary>
        /// <param name="standardShaderMaterial">The material of which the BlendMode will be set (The material must be a standardShaderMaterial).</param>
        /// <param name="blendMode">The BlendMode to be changed to.</param>
        public static void
        ChangeRenderMode(Material standardShaderMaterial, BlendMode blendMode)
        {
            switch (blendMode) {
                case BlendMode.Opaque:
                    standardShaderMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    standardShaderMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 2450;
                    break;
                case BlendMode.Fade:
                    standardShaderMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                    standardShaderMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;
                case BlendMode.Transparent:
                    standardShaderMaterial.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;
            }
        } // ChangeRenderMode
    }
}
