using CheckstoresMagnusRetail.sqlrepo;
using Xamarin.Forms;
using XFUniqueIdentifier.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidDevice))]
namespace XFUniqueIdentifier.Droid
{
	public class AndroidDevice : IDevice
	{
		public string GetIdentifier()
		{
			return Android.Provider.Settings.Secure.GetString(Forms.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
		}
	}
}