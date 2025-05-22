namespace Hotel.Models;

public record ReservationResponseDto(
    long Id,
    long RoomId,
    float Price,
    float Cost,
    DateTime StartDate,
    DateTime EndDate,
    string Comment,
    string MainPerson,
    string ContactInfo,
    string ContactEmail);