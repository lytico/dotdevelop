//
// Copyright (c) Microsoft Corp. (https://www.microsoft.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using MonoDevelop.Components;
using MonoDevelop.Core;
using MonoDevelop.Core.FeatureConfiguration;
using MonoDevelop.DesignerSupport.Toolbox;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Commands;
using MonoDevelop.Ide.Gui.Documents;
using Xwt;
using Xwt.Backends;

namespace MonoDevelop.TextEditor
{
	[Export (typeof (EditorFormatDefinition))]
	[ClassificationType (ClassificationTypeNames = ClassificationTypeNames.Text)]
	[Name (ClassificationTypeNames.Text)]
	[Order (After = Priority.Default, Before = Priority.High)]
	[UserVisible (true)]
	class ClassificationFormatDefinitionFromPreferences : ClassificationFormatDefinition
	{
		public ClassificationFormatDefinitionFromPreferences ()
		{
			float fontSize = -1;
			var fontName = Ide.Editor.DefaultSourceEditorOptions.Instance.FontName;

			if (!string.IsNullOrEmpty (fontName)) {
				var sizeStartOffset = fontName.LastIndexOf (' ');
				if (sizeStartOffset >= 0) {
					float.TryParse (fontName.Substring (sizeStartOffset + 1), out fontSize);
					fontName = fontName.Substring (0, sizeStartOffset);
				}
			}

			if (string.IsNullOrEmpty (fontName))
				fontName = Xwt.Drawing.Font.SystemMonospaceFont?.Family;

			if (fontSize <= 1)
				fontSize = 12;

			FontTypeface = Xwt.Drawing.Font.FromName (fontName).WithSize (fontSize);
		}
	}

	class XwtTextViewContent :
		TextViewContent<IXwtTextView, XwtTextViewImports>,
		IToolboxDynamicProvider
	{
		IXwtTextViewHost textViewHost;
		Widget textViewHostControl;
		GtkNSViewHostControl embeddedControl;

		static readonly Lazy<bool> useLegacyGtkNSViewHost = new Lazy<bool> (
			() => FeatureSwitchService.IsFeatureEnabled ("LegacyGtkNSViewHost").GetValueOrDefault ());

		public event EventHandler ItemsChanged;

		abstract class GtkNSViewHostControl : Control
		{
			public global::Xwt.Widget XwtView { get; protected set; }
		}

		sealed class ManagedGtkNSViewHostControl : GtkNSViewHostControl
		{
			public ManagedGtkNSViewHostControl (IXwtTextViewHost textViewHost)
			{
				if (textViewHost == null)
					throw new ArgumentNullException (nameof (textViewHost));

				//TODO:
				// GtkView = new Gtk.GtkNSViewHost (
				// 	textViewHost.HostControl,
				// 	disposeViewOnGtkDestroy: true);

				XwtView = new Xwt.HBox ();
				XwtView.Show ();
			}

			protected override object CreateNativeWidget<T> ()
				=> XwtView;

			public override bool HasFocus
				=> XwtView.HasFocus;

			public override void GrabFocus ()
				=> XwtView.SetFocus ();
		}

		sealed class LegacyGtkNSViewHostControl : GtkNSViewHostControl
		{
			readonly IXwtTextViewHost textViewHost;
			readonly Xwt.Widget nsView;

			bool nativeViewNeedsFocus;

			public bool IsGrabbingFocus { get; private set; }

			public LegacyGtkNSViewHostControl (IXwtTextViewHost textViewHost)
			{
				this.textViewHost = textViewHost ?? throw new ArgumentNullException (nameof (textViewHost));
				this.nsView = textViewHost.HostControl;

				XwtView = this.GetNativeWidget<Xwt.Widget> ();
				XwtView.CanGetFocus = true;

				textViewHost.HostControlMovedToWindow += OnNativeViewMovedToWindow;
			}

			protected override void Dispose (bool disposing)
			{
				if (disposing)
					textViewHost.HostControlMovedToWindow -= OnNativeViewMovedToWindow;

				base.Dispose (disposing);
			}

			protected override object CreateNativeWidget<T> ()
				=> nsView;

			// NOTE: Doesn't seem to be used in any vital way?
			public override bool HasFocus => base.HasFocus;

			public override void GrabFocus ()
			{
				if (IsGrabbingFocus)
					return;

				IsGrabbingFocus = true;
				XwtView.SetFocus ();

				if (nsView.ParentWindow != null)
					FocusEditor ();
				else
					nativeViewNeedsFocus = true;
			}

			private void OnNativeViewMovedToWindow (object sender, EventArgs e)
			{
				if (!nativeViewNeedsFocus || nsView.ParentWindow == null)
					return;

				FocusEditor ();
			}

			private void FocusEditor ()
			{
				// We really want the nsView (our grid view) to be first responder,
				// so focus returns to find widget or whatever. But it's not working
				// for some reason I can't figure out.
				//if (!nsView.Window.MakeFirstResponder (nsView))
				textViewHost.TextView.Focus ();

				// This is necessary to get focus back when using the navigation/breadcrumb bar
				// nsView.Window.MakeKeyAndOrderFront (nsView.Window);

				IsGrabbingFocus = false;
			}
		}

		public XwtTextViewContent (XwtTextViewImports imports)
			: base (imports)
		{
		}

		protected override IXwtTextView CreateTextView (ITextViewModel viewModel, ITextViewRoleSet roles)
			=> Imports.TextEditorFactoryService.CreateTextView (viewModel, roles, Imports.EditorOptionsFactoryService.GlobalOptions);

		protected override ITextViewRoleSet GetAllPredefinedRoles ()
			=> Imports.TextEditorFactoryService.AllPredefinedRoles;

		protected override Control CreateControl ()
		{
			textViewHost = Imports.TextEditorFactoryService.CreateTextViewHost (TextView, setFocus: true);
			textViewHostControl = textViewHost.HostControl;
			textViewHostControl.Accessible.Title = GettextCatalog.GetString ("Source Editor Group");

			if (!useLegacyGtkNSViewHost.Value) {
				embeddedControl = new ManagedGtkNSViewHostControl (textViewHost);

				TextView.GotAggregateFocus += (sender, e)
					=> embeddedControl.XwtView.SetFocus ();
			} else {
				var legacyEmbeddedControl = new LegacyGtkNSViewHostControl (textViewHost);
				embeddedControl = legacyEmbeddedControl;

				TextView.GotAggregateFocus += (sender, e) => {
					if (!legacyEmbeddedControl.IsGrabbingFocus)
						embeddedControl.XwtView.SetFocus ();
				};
			}

			TextView.Properties.AddProperty (typeof (Xwt.Widget), embeddedControl.XwtView);
			TextBuffer.ContentTypeChanged += TextBuffer_ContentTypeChanged;
			return embeddedControl;
		}

		private void TextBuffer_ContentTypeChanged (object sender, Microsoft.VisualStudio.Text.ContentTypeChangedEventArgs e)
		{
			ItemsChanged?.Invoke (this, EventArgs.Empty);
		}

		protected override void OnGrabFocus (DocumentView view)
		{
			embeddedControl.GrabFocus ();
			base.OnGrabFocus (view);
		}

		protected override void OnDispose ()
		{
			if (TextBuffer != null)
				TextBuffer.ContentTypeChanged -= TextBuffer_ContentTypeChanged;
			base.OnDispose ();
			if (textViewHost != null) {
				textViewHost.Close ();
				textViewHost = null;
			}
		}

		protected override void InstallAdditionalEditorOperationsCommands ()
		{
			base.InstallAdditionalEditorOperationsCommands ();

			// TODO:
			// EditorOperationCommands.Add (SearchCommands.Find, new EditorOperationCommand (
			// 	_ => HandleTextFinderAction (
			// 		TextFinderAction.ShowFindInterface,
			// 		perform: true),
			// 	(_, info) => info.Enabled = HandleTextFinderAction (
			// 		TextFinderAction.ShowFindInterface,
			// 		perform: false)));
			//
			// EditorOperationCommands.Add (SearchCommands.Replace, new EditorOperationCommand (
			// 	_ => HandleTextFinderAction (
			// 		TextFinderAction.ShowReplaceInterface,
			// 		perform: true),
			// 	(_, info) => info.Enabled = HandleTextFinderAction (
			// 		TextFinderAction.ShowReplaceInterface,
			// 		perform: false)));
			//
			// bool HandleTextFinderAction (TextFinderAction action, bool perform)
			// {
			// 	var responder = textViewHostControl?.ParentWindow?.FirstResponder();
			//
			// 	if (responder != null && responder.RespondsToSelector (action.Action)) {
			// 		if (perform)
			// 			responder.PerformTextFinderAction (action);
			// 		return true;
			// 	}
			//
			// 	return false;
			// }
		}

		static string category = GettextCatalog.GetString ("Text Snippets");
		public IEnumerable<ItemToolboxNode> GetDynamicItems (IToolboxConsumer consumer)
		{
			var contentType = TextView?.TextBuffer?.ContentType;
			if (contentType == null)
				yield break;
			foreach (var template in Imports.ExpansionManager.EnumerateExpansions (contentType, false, null, true, false)) {
				yield return new SnippetToolboxNode (template) {
					Category = category,
					Icon = ImageService.GetIcon ("md-template", Gtk.IconSize.Menu)
				};
			}
		}

		// sealed class TextFinderAction : Foundation.NSObject, INSValidatedUserInterfaceItem
		// {
		// 	public static readonly TextFinderAction ShowFindInterface
		// 		= new TextFinderAction (NSTextFinderAction.ShowFindInterface);
		//
		// 	public static readonly TextFinderAction ShowReplaceInterface
		// 		= new TextFinderAction (NSTextFinderAction.ShowReplaceInterface);
		//
		// 	public Selector Action { get; } = new Selector ("performTextFinderAction:");
		// 	public nint Tag { get; }
		//
		// 	TextFinderAction (IntPtr handle) : base (handle)
		// 	{
		// 	}
		//
		// 	TextFinderAction (NSTextFinderAction action)
		// 		=> Tag = (int)action;
		// }
	}
}