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
using System.Windows;

using Xwt;

using Gdk;
using Gtk;
using Microsoft.VisualStudio.Text.Classification;

using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Documents;
using MonoDevelop.Projects;
using Font = Xwt.Drawing.Font;

namespace MonoDevelop.TextEditor
{
	[ExportDocumentControllerFactory (FileExtension = "*", InsertBefore = "TextEditor")]
	class XwtTextViewDisplayBinding : TextViewDisplayBinding<XwtTextViewImports>
	{
		static XwtTextViewDisplayBinding ()
		{
			Microsoft.VisualStudio.UI.GettextCatalog.Initialize (GettextCatalog.GetString, GettextCatalog.GetString);
			// TODO:
			// Microsoft.VisualStudio.Text.Editor.Implementation.CocoaLocalEventMonitor.FilterGdkEvents += (enable) => {
			// 	if (enable)
			// 		Gdk.Window.AddFilterForAll (Filter);
			// 	else
			// 		Gdk.Window.RemoveFilterForAll (Filter);
			// };
		}

		static FilterReturn Filter (IntPtr xevent, Event evnt)
		{
			return FilterReturn.Remove;
		}

		protected override DocumentController CreateContent (XwtTextViewImports imports)
		{
			return new XwtTextViewContent (imports);
		}

		protected override ThemeToClassification CreateThemeToClassification (IEditorFormatMapService editorFormatMapService)
			=> new XwtThemeToClassification (editorFormatMapService);

		class XwtThemeToClassification : ThemeToClassification
		{
			public XwtThemeToClassification (IEditorFormatMapService editorFormatMapService) : base (editorFormatMapService) {}

			protected override void AddFontToDictionary (ResourceDictionary resourceDictionary, string appearanceCategory, string fontName, double fontSize)
			{
				if (appearanceCategory == "tooltip")
					return;

				if (fontSize <= 0)
					fontSize = Font.SystemFont.Size;

				var pangoFontDescription = $"{fontName} {fontSize}";

				Font xwtFont;

				try {
					xwtFont = GetXwtFontFromPangoFontDescription (pangoFontDescription);
				} catch (Exception e) {
					xwtFont = null;
					LoggingService.LogInternalError (
						$"Exception attempting to map Pango font description '{pangoFontDescription}' to an NSFont",
						e);
				}

				if (xwtFont == null) {
					LoggingService.LogWarning (
						$"Unable to map Pango font description '{pangoFontDescription}' " +
						$"to NSFont; falling back to system default at {fontSize} pt");
					//TODO: xwtFont = NSFontWorkarounds.UserFixedPitchFontOfSize ((nfloat)fontSize);
				}

				fontSize = xwtFont.Size;

				LoggingService.LogInfo ($"Mapped Pango font description '{pangoFontDescription}' to Font '{xwtFont}'");

				resourceDictionary [ClassificationFormatDefinition.TypefaceId] = xwtFont;
				resourceDictionary [ClassificationFormatDefinition.FontRenderingSizeId] = fontSize;
			}

			static Font GetXwtFontFromPangoFontDescription (string fontDescription)
				=> GetXwtFontFromPangoFontDescription (Pango.FontDescription.FromString (fontDescription));

			static Font GetXwtFontFromPangoFontDescription (Pango.FontDescription fontDescription)
			{
				if (fontDescription == null)
					return null;

				return Font.FromName (fontDescription.Family)
						.WithStyle (fontDescription.Style == Pango.Style.Italic || fontDescription.Style == Pango.Style.Oblique
							? Xwt.Drawing.FontStyle.Italic
							: 0)
						.WithWeight (NormalizeWeight (fontDescription.Weight))
						.WithSize (fontDescription.Size / (float)Pango.Scale.PangoScale);


				/// <summary>
				/// Normalizes a Pango font weight (100-1000 scale) to a weight
				/// suitable for Font.FontWithFamily (0-15 scale).
				/// </summary>
				Xwt.Drawing.FontWeight NormalizeWeight (Pango.Weight pangoWeight)
				{
					double Normalize (double value, double inMin, double inMax, double outMin, double outMax)
						=> (outMax - outMin) / (inMax - inMin) * (value - inMax) + outMax;

					var w =  (int)Math.Round (Normalize ((int)pangoWeight, 100, 1000, 0, 15));
					// TODO:
					return (Xwt.Drawing.FontWeight) (w % 50);
				}
			}
		}
	}
}