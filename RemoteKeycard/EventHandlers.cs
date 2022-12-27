using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using MapGeneration.Distributors;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using UnityEngine;

namespace RemoteKeycard
{
    public class EventHandlers
    {
        
        [PluginEvent(ServerEventType.PlayerInteractDoor)]
        public void OnDoorInteract(Player player, DoorVariant door, bool canOpen)
        {
            SetupBlacklistedDoors();
            if (!RemoteKeycard.Instance.Config.AffectDoors) return;
            if (DoorsUtils.GetBlacklistedDoors().Any(blacklistedDoor => door.name.StartsWith(blacklistedDoor))) return;
            if (player.ReferenceHub.inventory.CurInstance is KeycardItem) return;
            try
            {
                foreach (var keycardItem in player.ReferenceHub.inventory.UserInventory.Items.Values)
                {
                    if (player.ReferenceHub.inventory.UserInventory.Items.ContainsValue(keycardItem) &&
                        door.RequiredPermissions.CheckPermissions(keycardItem, player.ReferenceHub))
                    {
                        door.NetworkTargetState = !door.TargetState;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug($"{nameof(OnDoorInteract)}: {e.Message}\n{e.StackTrace}");
            }
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
        public void OnLockerInteract(Player player, Locker locker, byte colliderId, bool canOpen)
        {
            SetupBlacklistedLockers();
            if (!RemoteKeycard.Instance.Config.AffectScpLockers) return;
            if (player.ReferenceHub.inventory.CurInstance is KeycardItem) return;
            if (locker is PedestalScpLocker) return;
            if (LockerUtils.GetBlacklistedLockers().Any(blacklistedLocker => locker.name.Contains(blacklistedLocker))) return;
            try
            {
                foreach (var item in player.ReferenceHub.inventory.UserInventory.Items.Values)
                {
                    if (!(item is KeycardItem keycardItem)) continue;
                    if (!player.ReferenceHub.inventory.UserInventory.Items.ContainsValue(keycardItem) ||keycardItem.Permissions.HasFlagFast(locker.Chambers[colliderId].RequiredPermissions)) continue;
                    locker.Chambers[colliderId].SetDoor(!locker.Chambers[colliderId].IsOpen,
                        locker.GetFieldValue<AudioClip>("_grantedBeep"));
                    var refreshOpenedSyncVarMethod = locker.GetType().GetMethod("RefreshOpenedSyncvar",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    refreshOpenedSyncVarMethod?.Invoke(locker, null);
                }
            }
            catch (Exception e)
            {
                Log.Debug($"{nameof(OnLockerInteract)}: {e.Message}\n{e.StackTrace}");
            }
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

    public static class ReflectionExtensions {
        public static T GetFieldValue<T>(this object obj, string name) {
            // Set the flags so that private and public fields from instances will be found
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            return (T)field?.GetValue(obj);
        }
    }
}