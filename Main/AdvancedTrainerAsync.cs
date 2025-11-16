using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

namespace Advanced_Trainer
{
    /// <summary>
    /// This class handles everything that waits, so that our script doesn't freeze whenever we call a function that calls Script.Wait() internally.
    /// </summary>
    [ScriptAttributes(NoDefaultInstance = true)]
    public class AdvancedTrainerAsync : Script
    {
        // TODO: pass a this-reference to the requests so that they can call stuff in a non-static way
        // So that we could have many async handlers for possibly different things 
        // Using polymorphism and the abstract class AsyncRequest, each subclass implements process,
        // And take the process implementation for each thing we have here and use it in the respective class
        // Still manage priority requests seperately from other requests, so that we can call processPrioritizedRequests

        Queue<CreateVehicleAsyncRequest> vehicleSpawnRequestQueue = new Queue<CreateVehicleAsyncRequest>();
        Queue<CreatePedAsyncRequest> pedSpawnRequestQueue = new Queue<CreatePedAsyncRequest>();
        Queue<SpecialFlightModeRequest> specialFlightModeRequestQueue = new Queue<SpecialFlightModeRequest>();
        Queue<DriftModeRequest> driftModeRequestQueue = new Queue<DriftModeRequest>();
        Dictionary<long, Vehicle> createdVehicles = new Dictionary<long, Vehicle>();
        Dictionary<long, Ped> createdPeds = new Dictionary<long, Ped>();
        public AdvancedTrainerAsync() {
            this.Tick += onTick;
        }

        private void onTick(object sender, EventArgs e)
        {
            processPrioritizedRequests();
            processCreateVehicleRequestQueue();
            processCreatePedRequestQueue();
        }

        // This should always have priority over other request types
        private void processPrioritizedRequests()
        {
            processSpecialFlightModeRequestsQueue();
            processDriftModeRequestsQueue();
        }

        private void processSpecialFlightModeRequestsQueue()
        {
            while (specialFlightModeRequestQueue.Count > 0)
            {
                var request = specialFlightModeRequestQueue.Dequeue();
                if (request != null)
                {
                    request.Process();
                }
            }
        }

        private void processDriftModeRequestsQueue()
        {
            while (driftModeRequestQueue.Count > 0)
            {
                var request = driftModeRequestQueue.Dequeue();
                if (request != null)
                {
                    request.Process();
                }
            }
        }

        private void processCreateVehicleRequestQueue()
        {
            while (vehicleSpawnRequestQueue.Count > 0)
            {
                processPrioritizedRequests();
                var request = vehicleSpawnRequestQueue.Dequeue();
                if (request != null)
                {
                    request.Process();
                }
            }
        }

        private void processCreatePedRequestQueue()
        {
            while (pedSpawnRequestQueue.Count > 0)
            {
                processPrioritizedRequests();
                var request = pedSpawnRequestQueue.Dequeue();
                if (request != null)
                {
                    request.Process();
                }
            }
        }

        public void enqueueVehicleSpawnRequest(long id, Vector3 position, VehicleHash hash)
        {
            vehicleSpawnRequestQueue.Enqueue(new CreateVehicleAsyncRequest(this, id, position, hash));
        }

        public void enqueuePedSpawnRequest(long id, Vector3 position, PedHash hash)
        {
            pedSpawnRequestQueue.Enqueue(new CreatePedAsyncRequest(this, id, position, hash));
        }

        public void enqueueActivateSpecialFlightModeRequest(Vehicle vehicle)
        {
            specialFlightModeRequestQueue.Enqueue(new ActivateSpecialFlightModeRequest(vehicle));
        }

        public void enqueueDeactivateSpecialFlightModeRequest(Vehicle vehicle)
        {
            specialFlightModeRequestQueue.Enqueue(new DeactivateSpecialFlightModeRequest(vehicle));
        }

        public void enqueueActivateDriftModeRequest(Vehicle vehicle, DriftCarHash driftType)
        {
            driftModeRequestQueue.Enqueue(new ActivateDriftModeRequest(vehicle, driftType));
        }

        public void enqueueDeactivateDriftModeRequest(Vehicle vehicle)
        {
            driftModeRequestQueue.Enqueue(new DeactivateDriftModeRequest(vehicle));
        }

        public Vehicle getCreatedVehicle(long  id)
        {
            Vehicle vehicle = null;
            var result = createdVehicles.TryGetValue(id, out vehicle);
            if (result)
            {
                createdVehicles.Remove(id);
            }
            return vehicle;
        }

        public Ped getCreatedPed(long id)
        {
            Ped ped = null;
            var result = createdPeds.TryGetValue(id, out ped);
            if (result)
            {
                createdPeds.Remove(id);
            }
            return ped;
        }

        public void addCreatedVehicle(long id, Vehicle vehicle)
        {
            if (vehicle != null)
            {
                createdVehicles[id] = vehicle;
            }
        }

        public void removeCreatedVehicle(long id)
        {
            createdVehicles.Remove(id);
        }

        public void addCreatedPed(long id, Ped ped)
        {
            if (ped  != null)
            {
                createdPeds[id] = ped;
            }
        }

        public void removeCreatedPed(long id)
        {
            createdPeds.Remove(id);
        }

    }


}
