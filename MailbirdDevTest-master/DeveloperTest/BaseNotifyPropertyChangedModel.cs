using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DeveloperTest.Models
{
    public abstract class BaseNotifyPropertyChangedModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetProperty<T>(ref T propertyField, T value, [CallerMemberName] string propertyName = null)
        {
            propertyField = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
