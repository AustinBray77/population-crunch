using CityExtras;
using Godot;
using System;
using System.Collections.Generic;

public partial class City : Area2D
{
    [Export]
    public int Population { get; set; } = 0;

    [Export]
    public double GrowthFactor { get; set; } = 1.2;

    private BetterTimer _tripTimer, _growthTimer;

    public List<Road> Roads;

    public Main MainReference;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("City Intialised..");
        _ConfigureVariables();
        _StartTimers();
        _DrawCity();
    }

    private void _ConfigureVariables()
    {
        Roads = new List<Road>();
        GrowthFactor = Main.Rand.NextDouble() * (0.3) + 1.05;
        Population = (int)Main.Rand.Next(10, 200);
        _tripTimer = new BetterTimer(600 / Population, 1000 / Population, _OnTripTimerTimeout);
        _growthTimer = new BetterTimer(10, _OnGrowthTimerTimeout);
    }

    private void _StartTimers()
    {
        _tripTimer.Start();
        _growthTimer.Start();
    }

    private void _DrawCity()
    {
        AddChild(Graphics.DrawCircle(Vector2.Zero, 100, Graphics.Orange, 20));
    }

    public void UpdatePopulation()
    {
        Population = (int)(Population * GrowthFactor);
        if (Population < 10000)
        {
            GrowthFactor += 0.01;
        }

        if (Population > 50000)
        {
            GrowthFactor -= 0.02;
        }

        GD.Print($"New Population -> {Population}");
    }

    private void _OnTripTimerTimeout()
    {
        MainReference.GenerateTrip(this);
    }

    private void _OnGrowthTimerTimeout()
    {
        _UpdateTripTimerRanges();
        UpdatePopulation();
    }

    private void _UpdateTripTimerRanges()
    {
        _tripTimer.MinTriggerTime /= GrowthFactor;
        _tripTimer.MaxTriggerTime /= GrowthFactor;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _tripTimer.IncrementTime(delta);
        _growthTimer.IncrementTime(delta);
    }
}
