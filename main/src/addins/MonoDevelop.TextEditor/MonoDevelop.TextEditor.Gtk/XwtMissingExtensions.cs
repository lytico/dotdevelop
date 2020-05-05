using Microsoft.VisualStudio.Text.Editor;
using Xwt;

namespace MonoDevelop.TextEditor.XwtImpl
{

	public static class XwtMissingExtensions
	{
		public static Point GetViewRelativeMousePosition (this IXwtTextView it, Point eventPostion)
		{
			return eventPostion;
		}

		public static void MoveCaretToPosition (this IXwtTextView textView, ButtonEventArgs theEvent)
		{ }

		public static Widget FirstResponder (this Xwt.WindowFrame it)
		{
			// TODO
			return default;
		}
	}

}