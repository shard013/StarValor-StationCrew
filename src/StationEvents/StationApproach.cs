using HarmonyLib;
using StationCrew.AutoStation;
using UnityEngine;

namespace StationCrew.StationEvents
{
    [HarmonyPatch(typeof(DockingControl), "OnTriggerEnter")]
    public class StationApproach
    {
        public static void Postfix(Collider other, DockingUI ___dockingUI, SpaceShip ___playerSS)
        {
            if (!IsPlayer(other))
            {
                return;
            }

            if (!___playerSS.GetComponent<PlayerControl>().inStationRange)
            {
                return;
            }

            StationUtils.PlayerStationProximity(___dockingUI, ___playerSS);
        }

        public static bool IsPlayer(Collider other)
        {
            var transform = other?.transform;

            if (transform == null)
            {
                return false;
            }

            if (transform.CompareTag("Collider"))
            {
                transform = transform.GetComponent<ColliderControl>().ownerEntity.transform;
            }

            if (!transform.CompareTag("Player"))
            {
                return false;
            }

            return true;
        }
    }

}
