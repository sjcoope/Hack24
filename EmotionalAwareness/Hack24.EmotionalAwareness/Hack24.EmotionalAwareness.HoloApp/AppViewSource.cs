using Windows.ApplicationModel.Core;

namespace Hack24.EmotionalAwareness.HoloApp
{
    // The entry point for the app.
    internal class AppViewSource : IFrameworkViewSource
    {
        public IFrameworkView CreateView()
        {
            return new AppView();
        }
    }
}
