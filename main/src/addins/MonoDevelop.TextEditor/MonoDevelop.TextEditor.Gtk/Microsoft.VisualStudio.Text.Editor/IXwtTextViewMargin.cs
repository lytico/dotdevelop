//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License. See License.txt in the project root for license information.
//

using System;

using Xwt;

namespace Microsoft.VisualStudio.Text.Editor
{
    /// <summary>
    /// Represents margins that are attached to an edge of an <see cref="IXwtTextView"/>.
    /// </summary>
    public interface IXwtTextViewMargin : ITextViewMargin
    {
        /// <summary>
        /// Gets the <see cref="Widget"/> that renders the margin.
        /// </summary>
        /// <exception cref="ObjectDisposedException"> if the margin is disposed.</exception>
        Widget VisualElement { get; }
    }
}