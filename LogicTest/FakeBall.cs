using System;
using System.Collections.Generic;
using System.Text;
using Data;

namespace LogicTest;

internal class FakeBall : IBall
{
    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
    public IVector Velocity { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Radius => 0.03;
    public double Weight => 1.0;
    public FakeBall(double x, double y, double velX, double velY)
    {
        X = x;
        Y = y;
        Velocity = new Vector { X = velX, Y = velY };
    }
}
