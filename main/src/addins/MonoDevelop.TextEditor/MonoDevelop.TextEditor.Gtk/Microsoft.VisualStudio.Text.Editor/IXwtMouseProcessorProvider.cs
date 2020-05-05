//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License. See License.txt in the project root for license information.
//

namespace Microsoft.VisualStudio.Text.Editor
{
    /// <summary>
    /// Creates an <see cref="IXwtMouseProcessor"/> for a <see cref="IXwtTextView"/>.
    /// </summary>
    /// <remarks>This is a MEF component part, and should be exported with the following attribute:
    /// [Export(typeof(IMouseProcessorProvider))]
    /// Exporters must supply a NameAttribute, a ContentTypeAttribute, at least one TextViewRoleAttribute, and optionally an OrderAttribute.
    /// </remarks>
    public interface IXwtMouseProcessorProvider
    {
        /// <summary>
        /// Creates an <see cref="IXwtMouseProcessor"/> for a <see cref="IXwtTextView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IXwtTextView"/> for which to create the <see cref="IXwtMouseProcessor"/>.</param>
        /// <returns>The created <see cref="IXwtMouseProcessor"/>.
        /// The value may be null if this <see cref="IXwtMouseProcessorProvider"/> does not wish to participate in the current context.</returns>
        IXwtMouseProcessor GetAssociatedProcessor(IXwtTextView textView);
    }
}