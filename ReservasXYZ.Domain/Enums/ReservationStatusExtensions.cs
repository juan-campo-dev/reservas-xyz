namespace ReservasXYZ.Domain.Enums;

public static class ReservationStatusExtensions
{
    public static string ToSpanish(this ReservationStatus status) => status switch
    {
        ReservationStatus.Pending => "Pendiente",
        ReservationStatus.Confirmed => "Confirmada",
        ReservationStatus.CheckedIn => "En curso",
        ReservationStatus.CheckedOut => "Completada",
        ReservationStatus.Cancelled => "Cancelada",
        ReservationStatus.NoShow => "No presentado",
        _ => status.ToString()
    };
}
