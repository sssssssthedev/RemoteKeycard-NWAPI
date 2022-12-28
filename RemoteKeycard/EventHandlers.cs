using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using MapGeneration.Distributors;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

namespace RemoteKeycard
{
    public class EventHandlers
    {
        
        [PluginEvent(ServerEventType.PlayerInteractDoor)]
        public bool OnDoorInteract(Player player, DoorVariant door, bool canOpen)
        {
            // Loads the blacklisted doors
            SetupBlacklistedDoors();
            // Returns if the config option for affecting doors is set to false
            if (!RemoteKeycard.Instance.Config.AffectDoors) return true;
            // Returns on any blacklisted doors it finds, this is done to prevent the method from running on doors that are not supposed to be affected
            if (DoorsUtils.GetBlacklistedDoors().Any(blacklistedDoor => door.name.StartsWith(blacklistedDoor))) return true;
            // Returns if the player has a keycard in their hands
            if (player.ReferenceHub.inventory.CurInstance is KeycardItem) return true;
            try
            {
                // Does a for each on the player's inventory to find the keycard
                foreach (var keycardItem in player.ReferenceHub.inventory.UserInventory.Items.Values)
                {
                    // If the keycard exists in the player's inventory and the door permissions match the keycard, open the door
                    if (player.ReferenceHub.inventory.UserInventory.Items.ContainsValue(keycardItem) &&
                        door.RequiredPermissions.CheckPermissions(keycardItem, player.ReferenceHub))
                    {
                        door.NetworkTargetState = !door.TargetState;
                        return false;
                    }
                    return true;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Debug($"{nameof(OnDoorInteract)}: {e.Message}\n{e.StackTrace}");
            }

            return false;
        }
        
        // Doesn't work because there is no event for PlayerInteractGenerator
        //[PluginEvent(ServerEventType.PlayerOpenGenerator)]
        //public void OnGeneratorUnlock(Player player, Scp079Generator generator)
        //{
        //    if (!RemoteKeycard.Instance.Config.AffectGenerators) return;
        //    
        //    try
        //    {
        //        Log.Debug("[DEBUG] Triggered PlayerOpenGeneratorEvent");
        //        // Reflection to get requiredPermissions for generator
        //        Scp079Generator scp079Generator = new Scp079Generator();
        //        KeycardPermissions requiredPermissions = scp079Generator.GetFieldValue<KeycardPermissions>("_requiredPermissions");
        //        
        //        Log.Debug($"Required Permissions: {requiredPermissions}");
        //        foreach (KeycardItem keycardItem in player.ReferenceHub.inventory.UserInventory.Items.Values)
        //        {
        //            Log.Debug($"[DEBUG] Keycards: {keycardItem}");
        //            if (player.ReferenceHub.inventory.UserInventory.Items.ContainsValue(keycardItem) && keycardItem.Permissions.HasFlagFast(requiredPermissions))
        //            {
        //                if (!generator.enabled)
        //                {
        //                    generator.Network_flags = 2;
        //                    Log.Debug("[DEBUG] Generator Network Flag set to 2");
        //                }
        //                else
        //                {
        //                    generator.Network_flags = 1;
        //                    Log.Debug("[DEBUG] Generator Network Flag set to 1");
        //                }
        //            }
        //        }
                

        //    }
        //    catch (Exception e)
        //    {
        //        Log.Debug($"{nameof(OnGeneratorUnlock)}: {e.Message}\n{e.StackTrace}");
        //    }
        //}

        [PluginEvent(ServerEventType.PlayerInteractLocker)]
        public bool OnLockerInteract(Player player, Locker locker, byte colliderId, bool canOpen)
        {
            // Loads the blacklisted lockers
            SetupBlacklistedLockers();
            // Returns if the config option for affecting lockers is set to false
            if (!RemoteKeycard.Instance.Config.AffectScpLockers) return true;
            // Returns if the player has a keycard in their hands
            if (player.ReferenceHub.inventory.CurInstance is KeycardItem) return true;
            // Returns if the locker is a pedestal, still can't find any way of making it work
            if (locker is PedestalScpLocker) return true;
            // Returns on any blacklisted lockers it finds, this is done to prevent the method from running on lockers that are not supposed to be affected
            if (LockerUtils.GetBlacklistedLockers().Any(blacklistedLocker => locker.name.Contains(blacklistedLocker))) return true;
            try
            {
                // Does a for each on the player's inventory to find the keycard
                foreach (var item in player.ReferenceHub.inventory.UserInventory.Items.Values)
                {
                    // Checks if the item matches the keycard, this is done to prevent casting another item to the keycard item, which makes it impossible to open the locker if there are more items in the player's inventory
                    if (item is KeycardItem keycardItem) {
                        // If the player has a keycard in their inventory and the locker permissions match the keycard, open the locker
                        if (player.ReferenceHub.inventory.UserInventory.Items.ContainsValue(keycardItem) &&
                            keycardItem.Permissions.HasFlagFast(locker.Chambers[colliderId].RequiredPermissions))
                        {
                            locker.Chambers[colliderId].SetDoor(!locker.Chambers[colliderId].IsOpen, locker._grantedBeep);
                            locker.RefreshOpenedSyncvar();
                            return false;
                        }
                        return true;
                    }
                    return true;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Debug($"{nameof(OnLockerInteract)}: {e.Message}\n{e.StackTrace}");
            }
            return false;
        }

        private static void SetupBlacklistedDoors()
        {
            DoorsUtils.AddBlacklistedDoor("HCZ");
            DoorsUtils.AddBlacklistedDoor("LCZ");
            DoorsUtils.AddBlacklistedDoor("EZ");
            DoorsUtils.AddBlacklistedDoor("Prison BreakableDoor");
            DoorsUtils.AddBlacklistedDoor("Unsecured Pryable GateDoor");
            DoorsUtils.AddBlacklistedDoor("Pryable 173 GateDoor");
        }

        private static void SetupBlacklistedLockers()
        {
            LockerUtils.AddBlacklistedLocker("MiscLocker");
            LockerUtils.AddBlacklistedLocker("Adrenaline");
            LockerUtils.AddBlacklistedLocker("Medkit");
        }
    }

    public static class DoorsUtils
    {
        private static readonly List<string> BlacklistedDoors = new List<string>();
        
        public static List<string> GetBlacklistedDoors()
        {
            return BlacklistedDoors;
        }
        
        public static void AddBlacklistedDoor(string doorName)
        {
            BlacklistedDoors.Add(doorName);
        }
    }

    public static class LockerUtils
    {
        private static readonly List<string> BlacklistedLockers = new List<string>();
        
        public static List<string> GetBlacklistedLockers()
        {
            return BlacklistedLockers;
        }
        
        public static void AddBlacklistedLocker(string lockerName)
        {
            BlacklistedLockers.Add(lockerName);
        }
    }
}