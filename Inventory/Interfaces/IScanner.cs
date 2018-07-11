using System;
namespace Inventory
{
    /// <summary>
    /// This is the interface for the Scanner code. This is only implemented natively on an
    /// Android device so we have to use Xamarin Forms Dependency Service to implement this.
    /// This interface defines how we use the Scanner, a matching class needs to be implemented
    /// on each platform as well. The link here incldues a fulld escription of the structure.
    /// https://developer.xamarin.com/guides/xamarin-forms/dependency-service/introduction/
    /// </summary>
    public interface IScanner
    {
        event EventHandler<StatusEventArgs> OnScanDataCollected;
        event EventHandler<string> OnStatusChanged;

        void Read();

        void Enable();

        void Disable();

        void SetConfig(IScannerConfig a_config);
    }
}
