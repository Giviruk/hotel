namespace Hotel.Models;

public record ReservationRequestDto(
    long roomId,
    float price,
    float cost,
    DateTime StartDate,
    DateTime EndDate,
    string comment,
    string mainPerson,
    string contactInfo,
    string contactEmail);