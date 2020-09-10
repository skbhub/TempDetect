using System;
using System.Runtime.InteropServices;

public sealed class Planck
{
    public const float Emissivity = 1.0F;
    public const float Emissivity_97 = 0.97F;
    public const float ReflectedTemperature = 293.15F;
    public const float AtmosphericTemperature = 293.15F;
    public const float Distance = 1.0F;
    public const float RelativeHumidity = 0.5F;
    public const float ExternalOpticsTransmission = 1.0F;
    public const float ExternalOpticsTemperature = 293.15F;

    public static void CalculateFromFactoryLUT(
        float toEmissivity,
        float toReflectedTemperature,
        float toAtmosphericTemperature,
        float toDistance,
        float toRelativeHumidity,
        float toExternalOpticsTransmission,
        float toExternalOpticsTemperature,
        float[] values, int from, int to)
    {
        int count = values.Length;
        IntPtr hValues = IntPtr.Zero;
        try
        {
            hValues = Marshal.AllocHGlobal(count * sizeof(float));
            Marshal.Copy(values, 0, hValues, count);
            CalculateFromFactoryLUT(
                toEmissivity,
                toReflectedTemperature,
                toAtmosphericTemperature,
                toDistance,
                toRelativeHumidity,
                toExternalOpticsTransmission,
                toExternalOpticsTemperature,
                hValues, from, to);
            Marshal.Copy(hValues, values, 0, count);
        }
        finally
        {
            Marshal.FreeHGlobal(hValues);
        }
    }

    public static void CalculateToFactoryLUT(
        float fromEmissivity,
        float fromReflectedTemperature,
        float fromAtmosphericTemperature,
        float fromDistance,
        float fromRelativeHumidity,
        float fromExternalOpticsTransmission,
        float fromExternalOpticsTemperature,
        float[] values, int from, int to)
    {
        int count = values.Length;
        IntPtr hValues = IntPtr.Zero;
        try
        {
            hValues = Marshal.AllocHGlobal(count * sizeof(float));
            Marshal.Copy(values, 0, hValues, count);
            CalculateToFactoryLUT(
                fromEmissivity,
                fromReflectedTemperature,
                fromAtmosphericTemperature,
                fromDistance,
                fromRelativeHumidity,
                fromExternalOpticsTransmission,
                fromExternalOpticsTemperature,
                hValues, from, to);
            Marshal.Copy(hValues, values, 0, count);
        }
        finally
        {
            Marshal.FreeHGlobal(hValues);
        }
    }

    [DllImport("Planck.dll", EntryPoint = "CalculateFromFactoryLUT", CallingConvention = CallingConvention.StdCall)]
    private static extern void CalculateFromFactoryLUT(
        float toEmissivity,
        float toReflectedTemperature,
        float toAtmosphericTemperature,
        float toDistance,
        float toRelativeHumidity,
        float toExternalOpticsTransmission,
        float toExternalOpticsTemperature,
        IntPtr hValus, int from, int to);

    [DllImport("Planck.dll", EntryPoint = "CalculateToFactoryLUT", CallingConvention = CallingConvention.StdCall)]
    private static extern void CalculateToFactoryLUT(
        float fromEmissivity,
        float fromReflectedTemperature,
        float fromAtmosphericTemperature,
        float fromDistance,
        float fromRelativeHumidity,
        float fromExternalOpticsTransmission,
        float fromExternalOpticsTemperature,
        IntPtr hValus, int from, int to);
}