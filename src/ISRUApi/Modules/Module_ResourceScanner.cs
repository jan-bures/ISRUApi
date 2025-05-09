﻿using I2.Loc;
using KSP.Sim;
using KSP.Sim.Definitions;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISRUApi.Modules;

[DisallowMultipleComponent]
public class Module_ResourceScanner : PartBehaviourModule
{
    public override Type PartComponentModuleType => typeof(PartComponentModule_ResourceScanner);

    [SerializeField]
    protected Data_ResourceScanner _dataResourceScanner;

    public override void AddDataModules()
    {
        base.AddDataModules();
        _dataResourceScanner ??= new Data_ResourceScanner();
        DataModules.TryAddUnique(_dataResourceScanner, out _dataResourceScanner);
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        
        if (PartBackingMode == PartBackingModes.Flight)
        {
            _dataResourceScanner.EnabledToggle.OnChangedValue += OnToggleChangedValue;
        }
       
        _dataResourceScanner.SetLabel(
            _dataResourceScanner.EnabledToggle,
            LocalizationManager.GetTermTranslation(_dataResourceScanner.ToggleName)
        );
        
        AddActionGroupAction(
            StartScanning,
            KSPActionGroup.None,
            LocalizationManager.GetTermTranslation(_dataResourceScanner.StartActionName)
        );
        AddActionGroupAction(
            StartScanning,
            KSPActionGroup.Custom01,
            LocalizationManager.GetTranslation(_dataResourceScanner.StartActionName)
        ); // TODO in Redux create a ScanResource action group
        _dataResourceScanner.CheckInclinationValues();
        UpdatePAMVisibility();
        _dataResourceScanner.statusTxt.SetValue(LocalizationManager.GetTranslation(
            ResourceScannerStatus.Idle.Description()
        ));
        
    }

    public override void OnShutdown()
    {
        base.OnShutdown();
        _dataResourceScanner.EnabledToggle.OnChangedValue -= OnToggleChangedValue;
    }

    public override void OnModuleFixedUpdate(float fixedDeltaTime)
    {
        // nothing
    }

    private void UpdatePAMVisibility()
    {
        _dataResourceScanner.SetVisible(_dataResourceScanner.EnabledToggle, PartBackingMode == PartBackingModes.Flight);
        _dataResourceScanner.SetVisible(_dataResourceScanner.statusTxt, PartBackingMode == PartBackingModes.Flight);
    }


    private void OnToggleChangedValue(bool newValue)
    {
        if (PartBackingMode != PartBackingModes.Flight)
        {
            return;
        }
        if (newValue) _dataResourceScanner._startScanTimestamp = Game.UniverseModel.Time.UniverseTime;
    } 

    private void StartScanning() => _dataResourceScanner.EnabledToggle.SetValue(true);


}