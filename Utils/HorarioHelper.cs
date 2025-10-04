namespace PortalAcademico.Utils;

public static class HorarioHelper
{
    // Intervalos [aInicio, aFin) y [bInicio, bFin) se solapan si:
    // aInicio < bFin && bInicio < aFin
    public static bool SeSolapan(TimeSpan aInicio, TimeSpan aFin, TimeSpan bInicio, TimeSpan bFin)
        => aInicio < bFin && bInicio < aFin;
}
