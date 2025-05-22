namespace Hotel.Models;

public record ContactInfo(
    string Phone,
    string Email,
    string Fio,
    string Sex,
    string Citizenship,
    DateTime Birthday,
    string PassportNumber,
    DateTime PassportDate,
    DateTime PassDateEnd,
    string Registration);