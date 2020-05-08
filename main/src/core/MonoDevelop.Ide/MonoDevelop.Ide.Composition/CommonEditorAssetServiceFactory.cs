using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;

namespace MonoDevelop.Ide.Composition
{
	[Export (typeof (ICommonEditorAssetServiceFactory))]
	internal class CommonEditorAssetServiceFactory : ICommonEditorAssetServiceFactory
	{
		public ICommonEditorAssetService GetOrCreate (ITextBuffer textBuffer)
		{
			return new CommonEditorAssetService (textBuffer);
		}
	}

	public class CommonEditorAssetService : ICommonEditorAssetService
	{
		private ITextBuffer textBuffer;

		public CommonEditorAssetService (ITextBuffer textBuffer)
		{
			this.textBuffer = textBuffer;
		}

		public T FindAsset<T> (Predicate<ICommonEditorAssetMetadata> isMatch = null) where T : class
		{
			return default(T);
		}
	}
}
