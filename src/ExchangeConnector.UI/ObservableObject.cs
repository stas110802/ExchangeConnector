using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExchangeConnector.UI;

public class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged([CallerMemberName] string? propName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    public virtual bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null) 
    {
        if (Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(name);

        return true;
    }
}