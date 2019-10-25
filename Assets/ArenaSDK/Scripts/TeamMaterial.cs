using UnityEngine;

namespace Arena {
    public class TeamMaterial : ArenaBase
    {
        public bool MakeTransparent = false;

        void
        Start()
        {
            Initialize();
        }

        // public override void
        // Initialize()
        // {
        //     base.Initialize();
        //
        //     if (GetComponentInParent<ArenaTeam>() != null) {
        //         getGlobalManager().ApplyTeamMaterial(
        //             GetComponentInParent<ArenaTeam>().getTeamID(),
        //             gameObject
        //         );
        //     } else {
        //         Debug.LogError("The object need to be a child of an ArenaTeam to be able to get team color.");
        //     }
        //
        //     if (MakeTransparent) {
        //         Utils.TransparentObject(gameObject);
        //     }
        // }
    }
}
