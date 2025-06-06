using System.Globalization;
using Hotel.Models.Enums;

namespace Hotel.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is RoomStatus status)
        {
            return status switch
            {
                RoomStatus.Available => Colors.Green,
                RoomStatus.Booked => Colors.Red,
                RoomStatus.Cleaning => Colors.Orange,
                RoomStatus.Repair => Colors.Gray,
                RoomStatus.Reserved => Colors.Blue,
                _ => Colors.Black
            };
        }

        return Colors.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}