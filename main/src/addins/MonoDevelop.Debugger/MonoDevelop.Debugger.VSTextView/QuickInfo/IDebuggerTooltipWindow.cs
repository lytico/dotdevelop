using Mono.Debugging.Client;

namespace MonoDevelop.Debugger.VSTextView.QuickInfo
{
	interface IDebuggerTooltipWindow
	{
		void Expand ();
		DebuggerSession GetDebuggerSession ();

		void Close ();
	}
}
