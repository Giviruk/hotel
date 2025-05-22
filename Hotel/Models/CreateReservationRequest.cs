namespace Hotel.Models;

public record ReservationRequestDto(
    long RoomId,
    float Price,
    float Cost,
    DateTime StartDate,
    DateTime EndDate,
    string Comment,
    string MainPerson,
    string ContactInfo,
    string ContactEmail);