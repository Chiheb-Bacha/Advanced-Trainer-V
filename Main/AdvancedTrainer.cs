using GTA;
using GTA.Math;
using LemonUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Advanced_Trainer
{
    public class AdvancedTrainer : Script
    {
        public static readonly Dictionary<VehicleClass,VehicleHash[]> s_vehicleHashesByClass = new Dictionary<VehicleClass,VehicleHash[]>();
        public static readonly PedHash[] s_pedHashes = Ped.GetAllModels();
        public static readonly WeaponHash[] s_weaponHashes = Weapon.GetAllWeaponHashesForHumanPeds();
        public static AdvancedTrainerAsync asyncHandler;
        public static AdvancedTrainerUI trainerUI;
        public static long counter = 0; // Used to give unique IDs to requests passed to AdvancedTrainerAsync.
        public static List<long> requestedVehicleIDs = new List<long>();
        public static List<long> requestedPedIDs = new List<long>();
        public AdvancedTrainer() {
            this.Tick += onTick;
            this.KeyDown += onKeyDown;
            populateVehicleDict();
            asyncHandler = InstantiateScript<AdvancedTrainerAsync>();
            trainerUI = InstantiateScript<AdvancedTrainerUI>();
            Vehicle.PatchEngineTorqueMultiplierUpdate();
        }

        public static void CreateVehicle(Vector3 position, VehicleHash hash)
        {
            long id = counter++;
            requestedVehicleIDs.Add(id);
            asyncHandler.enqueueVehicleSpawnRequest(id, position, hash);
        }

        public Vehicle GetCreatedVehicle(long id)
        {
            return asyncHandler.getCreatedVehicle(id);
        }

        public static void CreatePed(Vector3 position, PedHash hash)
        {
            long id = counter++;
            requestedPedIDs.Add(id);
            asyncHandler.enqueuePedSpawnRequest(id, position, hash);
        }

        public Ped GetCreatedPed(long id)
        {
            return asyncHandler.getCreatedPed(id);
        }

        private static void populateVehicleDict()
        {
            foreach (VehicleClass vehicleClass in Enum.GetValues(typeof(VehicleClass)))
            {
                s_vehicleHashesByClass.Add(vehicleClass, Vehicle.GetAllModelsOfClass(vehicleClass));
            }
        }
        public static VehicleClass[] GetVehicleClasses()
        {
            return s_vehicleHashesByClass.Keys.ToArray();
        }

        public static VehicleHash[] GetVehicleHashesByClass(VehicleClass vehicleClass)
        {
            VehicleHash[] vehicleHashes;
            s_vehicleHashesByClass.TryGetValue(vehicleClass, out vehicleHashes);
            return vehicleHashes;
        }

        public static void ActivateSpecialFlightMode(Vehicle vehicle)
        {
            asyncHandler.enqueueActivateSpecialFlightModeRequest(vehicle);
        }

        public static void DeactivateSpecialFlightMode(Vehicle vehicle)
        {
            asyncHandler.enqueueDeactivateSpecialFlightModeRequest(vehicle);
        }

        public static void ActivateDriftMode(Vehicle vehicle, DriftCarHash driftType)
        {
            asyncHandler.enqueueActivateDriftModeRequest(vehicle, driftType);
        }

        public static void DeactivateDriftMode(Vehicle vehicle)
        {
            asyncHandler.enqueueDeactivateDriftModeRequest(vehicle);
        }

        public static void ActivateMPSelectionWheels()
        {
            Game.UseMpSelectionWheels(true);
        }

        public static void DeactivateMPSelectionWheels()
        {
            Game.UseMpSelectionWheels(false);
        }

        public static bool IsMPSelecitonWheelsActivated()
        {
            return Game.IsUsingMpSelectionWheels();
        }

        

        private void onKeyDown(object sender, KeyEventArgs e)
        {

        }
        
        private void onTick(object sender, EventArgs e)
        {
            // TODO: Create a Class which allows us to register KeyBind combinations (ENUM, like the game uses)
            // with a function that is called when the combination is pressed.
            // these are registered by the user both at runtime and on init from a config file
            // These will then be managed in a list in AdvancedTrainer.
            // Make it so that combinations included in other combinations only count if the longer combination isn't fulfilled
            // For example if keys 100, 101 and 102 are registered for an action, and keys 100 and 102 are registered for another action
            // Make sure that the action corresponsing to 100,102 doesn't fire if 100,102 and 101 are pressed.
            // This should be checked onTick in AdvancedTrainer
        }

    }
}
