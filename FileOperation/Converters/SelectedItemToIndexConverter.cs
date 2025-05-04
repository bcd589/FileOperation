using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace FileOperation.Converters
{
    public class SelectedItemToIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || !(values[1] is IEnumerable items))
            {
                // Return empty or 0 if nothing is selected or items source is invalid
                return "0"; // Or perhaps string.Empty or Binding.DoNothing
            }

            object selectedItem = values[0];
            int index = 0;
            try // Add try-catch for safety during enumeration
            {
                foreach (var item in items)
                {
                    if (object.Equals(item, selectedItem))
                    {
                        // Return 1-based index as string
                        return (index + 1).ToString();
                    }
                    index++;
                }
            }
            catch (Exception) // Handle potential exceptions during enumeration
            {
                return "!"; // Indicate error
            }


            // Item not found (shouldn't happen if SelectedItem is from ItemsSource)
             return "0"; // Or indicate not found
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Not needed for one-way binding
            throw new NotImplementedException();
        }
    }
}