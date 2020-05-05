//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License. See License.txt in the project root for license information.
//

using Xwt;

namespace Microsoft.VisualStudio.Text.Editor
{
    /// <summary>
    /// Provides extensions for mouse bindings.
    /// </summary>
    public interface IXwtMouseProcessor
    {
        /// <summary>
        /// Handles a mouse left button down event before the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PreprocessMouseLeftButtonDown(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse left button down event after the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PostprocessMouseLeftButtonDown(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse right button down event before the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PreprocessMouseRightButtonDown(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse right button down event after the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PostprocessMouseRightButtonDown(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse left button up event before the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PreprocessMouseLeftButtonUp(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse left button up event after the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PostprocessMouseLeftButtonUp(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse right button up event before the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PreprocessMouseRightButtonUp(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse right button up event after the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PostprocessMouseRightButtonUp(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse up event before the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PreprocessMouseUp(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse up event after the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PostprocessMouseUp(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse down event before the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PreprocessMouseDown(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse down event after the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PostprocessMouseDown(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse move event before the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PreprocessMouseMove(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse move event after the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PostprocessMouseMove(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse wheel event before the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PreprocessMouseWheel(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse wheel event after the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PostprocessMouseWheel(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse enter event before the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PreprocessMouseEnter(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse enter event after the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PostprocessMouseEnter(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse leave event before the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PreprocessMouseLeave(ButtonEventArgs e);

        /// <summary>
        /// Handles a mouse leave event after the default handler.
        /// </summary>
        /// <param name="e">
        /// Event arguments that describe the event.
        /// </param>
        void PostprocessMouseLeave(ButtonEventArgs e);

        void PreprocessDraggingEntered(DragEventArgs e);
        void PostprocessDraggingEntered(DragEventArgs e);

        void PreprocessDraggingUpdated(DragEventArgs e);
        void PostprocessDraggingUpdated(DragEventArgs e);

        void PreprocessDraggingExited(DragEventArgs e);
        void PostprocessDraggingExited(DragEventArgs e);

        void PreprocessPrepareForDragOperation(DragEventArgs e);
        void PostprocessPrepareForDragOperation(DragEventArgs e);

        void PreprocessPerformDragOperation(DragEventArgs e);
        void PostprocessPerformDragOperation(DragEventArgs e);

        void PreprocessDraggingEnded(DragEventArgs e);
        void PostprocessDraggingEnded(DragEventArgs e);
    }
}