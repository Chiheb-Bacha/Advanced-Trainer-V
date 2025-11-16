using GTA;
using GTA.Native;
using GTA.UI;
using LemonUI;
using LemonUI.Elements;
using LemonUI.Menus;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Advanced_Trainer
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class AdvancedTrainerUI : Script
    {
        private static ObjectPool objectPool = new ObjectPool();
        private static int menuCount = 0;
        private static VehicleHash currentMenuVehicleHash = 0;
        
        private static NativeMenu lastVehicleClassMenu;

        private int mainMenuClosedTime;
        
        private NativeMenu mainMenu;
        private bool isMainMenuRecentlyClosed;

        private NativeMenu vehicleMenu;

        private NativeMenu gameMenu;

        private NativeMenu vehicleModelsMenu;
        private NativeMenu currentVehicleMenu;
        private NativeItem torqueValueItem;
        private NativeItem enginePowerMultiplierValueItem;

        private NativeMenu spawnedVehiclesMenu;

        private NativeMenu vehicleModelSubMenu;
        private NativeSubmenuItem currentVehicleSubMenuItem;

        private static NativeMenu currentlyOpenMenu;


        public AdvancedTrainerUI() {
            this.Tick += onTick;
            this.KeyDown += onKeyDown;

            mainMenu = initMenu("Advanced Trainer");

            mainMenu.Name = "Powered By SHVDNE";

            mainMenu.Closed += (sender, e) =>
            {
                mainMenuClosedTime = Game.GameTime;
                isMainMenuRecentlyClosed = true;
            };

            vehicleMenu = initMenu("Vehicles");

            gameMenu = initMenu("Game");

            vehicleModelsMenu = initMenu("Vehicle Models");

            currentVehicleMenu = initMenu("Current Vehicle");
            spawnedVehiclesMenu = initMenu("Spawned Vehicles");

            vehicleModelSubMenu = initMenu("");

            vehicleModelSubMenu.Closing += (sender, e) =>
            {
                if (lastVehicleClassMenu != null)
                    lastVehicleClassMenu.Visible = true;
            };

            addToObjectPool(mainMenu);
            addToObjectPool(vehicleMenu);
            addToObjectPool(gameMenu);

            addToObjectPool(vehicleModelsMenu);
            addToObjectPool(currentVehicleMenu);
            addToObjectPool(spawnedVehiclesMenu);

            var gameSubMenuItem = mainMenu.AddSubMenu(gameMenu, "");
            gameSubMenuItem.Title = "Game";

            populateVehicleMenus();
            populateGameMenus();

        }

        private static void addToObjectPool(NativeMenu menu)
        {
            menu.CloseOnInvalidClick = false;
            menu.ResetCursorWhenOpened = false;
            menu.MouseBehavior = MenuMouseBehavior.Scrolling;
            menuCount++;
            objectPool.Add(menu);
        }

        public void showVehiclesMenu()
        {
            currentlyOpenMenu.Visible = false;
            vehicleMenu.Visible = true;
        }

        private void populateGameMenus()
        {
            NativeCheckboxItem mpSelectionWheelsItem = new NativeCheckboxItem("MP Selection Wheels");
            gameMenu.Add(mpSelectionWheelsItem);
            mpSelectionWheelsItem.Activated += (sender, e) =>
            {
                if (mpSelectionWheelsItem.Checked)
                {
                    AdvancedTrainer.ActivateMPSelectionWheels();
                }
                else
                {
                    AdvancedTrainer.DeactivateMPSelectionWheels();
                }
            };

            

            NativeListItem<Language> languageItem = new NativeListItem<Language>("Language");
            gameMenu.Add(languageItem);
            foreach (Language language in Enum.GetValues(typeof(Language)))
            {
                languageItem.Add(language);
            }

            languageItem.Activated += (sender, e) =>
            {
                // TODO: Update all localized strings when the language changes.
                Game.Language = languageItem.SelectedItem;
                Notification.PostTicker("A game restart might be required to apply the new language", true);
            };

            gameMenu.Opening += (sender, e) =>
            {
                if (AdvancedTrainer.IsMPSelecitonWheelsActivated())
                {
                    mpSelectionWheelsItem.Checked = true;
                }
                else
                {
                    mpSelectionWheelsItem.Checked = false;
                }
                languageItem.SelectedItem = Game.Language;
            };
        }

        private void populateVehicleMenus()
        {
            var vehicleSubMenuItem = mainMenu.AddSubMenu(vehicleMenu, "");
            vehicleSubMenuItem.Title = "Vehicles";

            var vehicleModelsSubMenuItem = vehicleMenu.AddSubMenu(vehicleModelsMenu, "");
            vehicleModelsSubMenuItem.Title = "Models";

            NativeItem vehicleSpawnTrigger = new NativeItem("Spawn");
            vehicleSpawnTrigger.Activated += (sender, e) =>
            {
                Ped playerPed = Game.LocalPlayerPed;
                AdvancedTrainer.CreateVehicle(playerPed.Position + playerPed.ForwardVector * 5f, currentMenuVehicleHash);
            };
            vehicleModelSubMenu.Add(vehicleSpawnTrigger);
            addToObjectPool(vehicleModelSubMenu);

            foreach (var vehicleClass in AdvancedTrainer.GetVehicleClasses())
            { 
                string className = vehicleClass.ToString();
                NativeMenu classMenu = initMenu(className);

                var vehicleClassItem = vehicleModelsMenu.AddSubMenu(classMenu, "");
                vehicleClassItem.Title = className;
                foreach (var vehicleHash in AdvancedTrainer.GetVehicleHashesByClass(vehicleClass))
                {
                    int vehicleClassMenuIndex = menuCount; 

                    string vehicleName = Game.GetLocalizedString(Vehicle.GetModelDisplayName(new Model(vehicleHash)));

                    NativeItem vehicleItem = new NativeItem("");
                    classMenu.Add(vehicleItem);
                    vehicleItem.Title = vehicleName;
                    vehicleItem.Activated += (sender, e) =>
                    {
                        currentMenuVehicleHash = vehicleHash;
                        vehicleModelSubMenu.BannerText = new ScaledText(PointF.Empty, vehicleName, 1.02f, GTA.UI.Font.HouseScript);

                        float maximumBannerTextWidth = vehicleModelSubMenu.Width * 0.95f;
                        float textWidth = vehicleModelSubMenu.BannerText.Width;
                        if (textWidth > maximumBannerTextWidth)
                        {
                            vehicleModelSubMenu.BannerText.Scale = vehicleModelSubMenu.BannerText.Scale * (maximumBannerTextWidth / textWidth);
                        }

                        vehicleModelSubMenu.BannerText.Alignment = Alignment.Center;

                        currentlyOpenMenu.Visible = false;
                        vehicleModelSubMenu.Visible = true;

                        vehicleModelSubMenu.Recalculate();

                        lastVehicleClassMenu = classMenu;
                    };

                }

                addToObjectPool(classMenu);
            }

            currentVehicleSubMenuItem = vehicleMenu.AddSubMenu(currentVehicleMenu, "");
            currentVehicleSubMenuItem.Title = "Current vehicle";
            currentVehicleSubMenuItem.Selected += (sender, e) =>
            {
                if (AdvancedTrainerCommon.GetCurrentVehicle() == null)
                {
                    currentVehicleSubMenuItem.Enabled = false;
                } else
                {
                    currentVehicleSubMenuItem.Enabled = true;
                }
            };

            NativeCheckboxItem activateDriftModeItem = new NativeCheckboxItem("Drift mode");
            NativeCheckboxItem specialFlightModeItem = new NativeCheckboxItem("Flight mode");

            NativeListItem<DriftCarHash> activateDriftModeTypeItem = new NativeListItem<DriftCarHash>("Drift type");
            foreach (DriftCarHash driftType in Enum.GetValues(typeof(DriftCarHash)))
            {
                activateDriftModeTypeItem.Add(driftType);
            }
            currentVehicleMenu.Add(activateDriftModeTypeItem);
            activateDriftModeTypeItem.Activated += (sender, e) =>
            {
                Vehicle playerVehicle = AdvancedTrainerCommon.GetCurrentVehicle();
                if (playerVehicle == null) { showVehiclesMenu(); return; }
                if (activateDriftModeItem.Checked)
                {
                    playerVehicle.ActivateDriftMode(activateDriftModeTypeItem.SelectedItem);
                }
            };

            currentVehicleMenu.Add(activateDriftModeItem);

            activateDriftModeItem.Activated += (sender, e) => {
                Vehicle playerVehicle = AdvancedTrainerCommon.GetCurrentVehicle();
                if (playerVehicle == null) { showVehiclesMenu(); return; }
                if (IsDriftModeSupportedByVehicle(playerVehicle)) // Any other DriftCarHash value can be used instead of DriftChavosv6
                {
                    if (activateDriftModeItem.Checked)
                    {
                        AdvancedTrainer.ActivateDriftMode(playerVehicle, activateDriftModeTypeItem.SelectedItem);
                        specialFlightModeItem.Checked = false;
                    }
                    else
                    {
                        AdvancedTrainer.DeactivateDriftMode(playerVehicle);
                    }
                } 
                else
                {
                    // We shouldn't even be able to reach this.
                    activateDriftModeItem.Enabled = false;
                    activateDriftModeTypeItem.Enabled = false;
                    activateDriftModeItem.Checked = false;
                }
            };

            specialFlightModeItem.Activated += (sender, e) =>
            {
                Vehicle playerVehicle = AdvancedTrainerCommon.GetCurrentVehicle();
                if (playerVehicle == null) { showVehiclesMenu(); return; }
                if (IsSpecialFlightModeSupportedByVehicle(playerVehicle))
                {
                    if (specialFlightModeItem.Checked)
                    {
                        AdvancedTrainer.ActivateSpecialFlightMode(playerVehicle);
                        activateDriftModeItem.Checked = false;
                    }
                    else
                    {
                        var vehicleModel = playerVehicle.Model;
                        if (vehicleModel != null)
                        { 
                            if (vehicleModel.Hash != (int)VehicleHash.Oppressor2)
                            {
                                AdvancedTrainer.DeactivateSpecialFlightMode(playerVehicle);
                            }
                        }
                    }
                } 
                else
                {
                    // We shouldn't even be able to reach this.
                    specialFlightModeItem.Enabled = false;
                    specialFlightModeItem.Checked = false;
                }
            };

            currentVehicleMenu.Add(specialFlightModeItem);

            torqueValueItem = initFloatInputItem("Torque multiplier",
                (value) =>
                {
                    Vehicle vehicle = AdvancedTrainerCommon.GetCurrentVehicle();
                    if (vehicle != null)
                    {
                        vehicle.EngineTorqueMultiplier = value;
                    }
                }
            );

            currentVehicleMenu.Add(torqueValueItem);

            
            enginePowerMultiplierValueItem = initFloatInputItem("Engine power multiplier",
                (value) =>
                {
                    Vehicle vehicle = AdvancedTrainerCommon.GetCurrentVehicle();
                    if (vehicle != null)
                    {
                        vehicle.EnginePowerMultiplier = value;
                    }
                }
            );

            currentVehicleMenu.Add(enginePowerMultiplierValueItem);



            var maxTuneVehicle = new NativeItem("Max tune");
            maxTuneVehicle.Activated += (sender, e) =>
            {
                Vehicle playerVehicle = AdvancedTrainerCommon.GetCurrentVehicle();
                if (playerVehicle == null) return;
                MaxTuneVehicle(playerVehicle);
            };
            currentVehicleMenu.Add(maxTuneVehicle);

            var repairVehicle = new NativeItem("Repair");
            repairVehicle.Activated += (sender, e) =>
            {
                Vehicle playerVehicle = AdvancedTrainerCommon.GetCurrentVehicle();
                if (playerVehicle == null) return;
                playerVehicle.Repair();
            };
            currentVehicleMenu.Add(repairVehicle);

            currentVehicleMenu.Opening += (sender, e) =>
            {
                Vehicle playerVehicle = AdvancedTrainerCommon.GetCurrentVehicle();
                if (playerVehicle == null) { return; }

                if (IsSpecialFlightModeSupportedByVehicle(playerVehicle))
                {
                    specialFlightModeItem.Enabled = true;
                } 
                else
                {
                    specialFlightModeItem.Enabled = false;
                }

                if (IsDriftModeSupportedByVehicle(playerVehicle))
                {
                    activateDriftModeItem.Enabled = true;
                    activateDriftModeTypeItem.Enabled = true;
                } 
                else
                {
                    activateDriftModeItem.Enabled = false;
                    activateDriftModeTypeItem.Enabled = false;
                }

                if (playerVehicle.IsSpecialFlightModeActivated())
                {
                    specialFlightModeItem.Checked = true;
                    activateDriftModeItem.Checked = false;
                }
                else if (playerVehicle.IsDriftModeActivated())
                {
                    specialFlightModeItem.Checked = false;
                    activateDriftModeItem.Checked = true;
                }
                else
                {
                    specialFlightModeItem.Checked = false;
                    activateDriftModeItem.Checked = false;
                }

                torqueValueItem.AltTitle = playerVehicle.EngineTorqueMultiplier.ToString("0.0");
                enginePowerMultiplierValueItem.AltTitle = playerVehicle.EnginePowerMultiplier.ToString("0.0");
            };
        }

        private void MaxTuneVehicle(Vehicle vehicle)
        {
            vehicle.Mods.InstallModKit();
            var engine = vehicle.Mods[VehicleModType.Engine];
            engine.Index = engine.Count - 1;
            var brakes = vehicle.Mods[VehicleModType.Brakes];
            brakes.Index = brakes.Count - 1;
            var transmission = vehicle.Mods[VehicleModType.Transmission];
            transmission.Index = transmission.Count - 1;
            var suspension = vehicle.Mods[VehicleModType.Suspension];
            suspension.Index = suspension.Count - 1;
            var armor = vehicle.Mods[VehicleModType.Armor];
            armor.Index = armor.Count - 1;

            var spoiler = vehicle.Mods[VehicleModType.Spoilers];
            if (spoiler.Index < 0) spoiler.Index = spoiler.Count - 1; // Only set a spoiler if the vehicle has a stock spoiler set

            vehicle.Mods[VehicleToggleModType.Nitrous].IsInstalled = true;
            vehicle.Mods[VehicleToggleModType.Turbo].IsInstalled = true;
            vehicle.Mods[VehicleToggleModType.TireSmoke].IsInstalled = true;
            vehicle.Mods[VehicleToggleModType.XenonHeadlights].IsInstalled = true;
        }

        private bool IsSpecialFlightModeSupportedByVehicle(Vehicle vehicle)
        {
            return Vehicle.IsModelHandlingCompatibleWithVehicleModel(new Model(VehicleHash.Oppressor2), vehicle.Model);
        }

        private bool IsDriftModeSupportedByVehicle(Vehicle vehicle)
        {
            return Vehicle.IsModelHandlingCompatibleWithVehicleModel(new Model((int)DriftCarHash.DriftChavosv6), vehicle.Model);
        }

        public void processGameControls()
        {
            Game.EnableAllControlsThisFrame(); // This allows some controls that would otherwise be disabled (camera movement, controlling flying vehicles, ...)
            if (Game.Player.IsAiming)
            {
                Game.DisableControlThisFrame(GTA.Control.SelectNextWeapon);
                Game.DisableControlThisFrame(GTA.Control.SelectPrevWeapon);
            }

            // This is added so that we don't aim the second we close the mainMenu
            // isMainMenuRecentlyClosed short-circuits in case the main menu hasn't been closed recently
            // this way the native called when using Game.GameTime isn't called every frame
            if (isMainMenuRecentlyClosed && (Game.GameTime - mainMenuClosedTime) < 200) 
            {
                Game.DisableControlThisFrame(GTA.Control.Aim);
                Game.DisableControlThisFrame(GTA.Control.AccurateAim);
                Game.DisableControlThisFrame(GTA.Control.VehicleAim);
            } else
            {
                isMainMenuRecentlyClosed = false;
            }

            if (currentlyOpenMenu != null)
            {
                if (Game.LocalPlayerPed.IsInVehicle())
                {
                    Game.DisableControlThisFrame(GTA.Control.VehicleSelectNextWeapon);
                    Game.DisableControlThisFrame(GTA.Control.VehicleSelectPrevWeapon);
                    Game.DisableControlThisFrame(GTA.Control.RadioWheelLeftRight);
                    Game.DisableControlThisFrame(GTA.Control.RadioWheelUpDown);
                    Game.DisableControlThisFrame(GTA.Control.VehicleAim);
                    Game.DisableControlThisFrame(GTA.Control.VehicleNextRadio);
                    Game.DisableControlThisFrame(GTA.Control.VehiclePrevRadio);
                    Game.DisableControlThisFrame(GTA.Control.VehicleRadioWheel);
                }
                else
                {
                    Game.DisableControlThisFrame(GTA.Control.Attack);
                    Game.DisableControlThisFrame(GTA.Control.Attack2);
                    Game.DisableControlThisFrame(GTA.Control.SelectWeaponMelee);
                    Game.DisableControlThisFrame(GTA.Control.SelectWeapon);
                }
                Game.DisableControlThisFrame(GTA.Control.Aim);
                Game.DisableControlThisFrame(GTA.Control.Phone);
                Game.DisableControlThisFrame(GTA.Control.PhoneCancel);
                Game.DisableControlThisFrame(GTA.Control.PhoneSelect);
            }
        }

        private void setCurrentVehicleItemState(bool newState)
        {
            currentVehicleSubMenuItem.Enabled = newState;
        }

        // These should be called from the trainer main script
        public void enableCurrentVehicleItem()
        {
            setCurrentVehicleItemState(true);
        }

        public void disableCurrentVehicleItem()
        {
            setCurrentVehicleItemState(false);
        }

        private static NativeMenu initMenu(string name)
        {
            NativeMenu menu = new NativeMenu(name);

            menu.Opening += (sender, e) =>
            {
                currentlyOpenMenu = menu;
            };

            menu.Closing += (sender, e) =>
            {
                currentlyOpenMenu = null;
            };

            return menu;
        }

        private static NativeItem initFloatInputItem(string title, Action<float> onValueChanged) 
        {
            var item = new NativeItem(title);
            item.Activated += (sender, e) =>
            {
                var valueStr = Game.GetUserInput();
                if (valueStr != "")
                {
                    valueStr = valueStr?.Replace(',', '.');

                    if (float.TryParse(valueStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float value))
                    {
                        value = AdvancedTrainerCommon.Snap(value, 0.1f);

                        onValueChanged?.Invoke(value);
                    }
                    else
                    {
                        Notification.PostTicker("Invalid number!", true);
                    }
                }

            };

            return item; 
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F10)
            {
                if (currentlyOpenMenu != null)
                {
                    currentlyOpenMenu.Visible = false;
                } else
                {
                    mainMenu.Visible = true;
                }
            }
        }

        private void onTick(object sender, EventArgs e)
        {
            objectPool.Process();

            if (currentlyOpenMenu == currentVehicleMenu)
            {
                var currentVehicle = AdvancedTrainerCommon.GetCurrentVehicle();
                
                if (currentVehicle == null)
                {
                    showVehiclesMenu();
                } else
                {
                    torqueValueItem.AltTitle = currentVehicle.EngineTorqueMultiplier.ToString("0.0");
                    enginePowerMultiplierValueItem.AltTitle = currentVehicle.EnginePowerMultiplier.ToString("0.0");
                }
            } 
            else if (currentlyOpenMenu == vehicleMenu)
            {
                if (AdvancedTrainerCommon.GetCurrentVehicle() == null)
                {
                    disableCurrentVehicleItem();
                } else
                {
                    enableCurrentVehicleItem();
                }
            }

            processGameControls();
        }
    }
}
