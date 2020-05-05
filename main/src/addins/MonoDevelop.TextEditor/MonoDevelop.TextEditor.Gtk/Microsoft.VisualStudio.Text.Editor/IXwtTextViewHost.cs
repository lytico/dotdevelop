//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License. See License.txt in the project root for license information.
//

using System;

using Xwt;

namespace Microsoft.VisualStudio.Text.Editor
{
    /// <summary>
    /// Contains an <see cref="IXwtTextView"/> and the margins that surround it,
    /// such as a scrollbar or line number gutter.
    /// </summary>
    public interface IXwtTextViewHost
    {
        /// <summary>
        /// Closes the text view host and its underlying text view.
        /// </summary>
        /// <exception cref="InvalidOperationException">The text view host is already closed.</exception>
        void Close();

        /// <summary>
        /// Determines whether this text view has been closed.
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// Occurs immediately after closing the text view.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Notify when HostControl gets a new NSWindow (which could be null)
        /// </summary>
        event EventHandler HostControlMovedToWindow;

        /// <summary>
        /// Gets the <see cref="IXwtTextViewMargin"/> with the given <paramref name="marginName"/> that is attached to an edge of this <see cref="IXwtTextView"/>.
        /// </summary>
        /// <param name="marginName">The name of the <see cref="ITextViewMargin"/>.</param>
        /// <returns>The <see cref="ITextViewMargin"/> with a name that matches <paramref name="marginName"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="marginName"/> is null.</exception>
        IXwtTextViewMargin GetTextViewMargin(string marginName);

        /// <summary>
        /// Gets the <see cref="IXwtTextView"/> that is contained within this <see cref="IXwtTextViewHost"/>.
        /// </summary>
        IXwtTextView TextView { get; }

        /// <summary>
        /// Gets the <see cref="Widget"/> control for this <see cref="IXwtTextViewHost"/>.
        /// </summary>
        /// <remarks> Use this property to display the <see cref="IXwtTextViewHost"/> Cocoa control.</remarks>
        Widget HostControl { get; }
    }
}