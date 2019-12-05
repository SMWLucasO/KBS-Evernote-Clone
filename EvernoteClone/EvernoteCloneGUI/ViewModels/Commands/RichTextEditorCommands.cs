using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using EvernoteCloneLibrary.Utils;

namespace EvernoteCloneGUI.ViewModels.Commands
{
    /// <summary>
    /// Contains all actions for the rich text editor buttons.
    /// </summary>
    public static class RichTextEditorCommands
    {

        #region Text decoration

        public static void ToggleBold(RichTextBox textEditor)
        {

            if (!(textEditor.Selection.IsEmpty))
            {
                if (textEditor.Selection.GetPropertyValue(TextElement.FontWeightProperty).Equals(FontWeights.Bold))
                {
                    ApplySelectionChange(textEditor.Selection, TextElement.FontWeightProperty, FontWeights.Normal);
                }
                else
                {
                    ApplySelectionChange(textEditor.Selection, TextElement.FontWeightProperty, FontWeights.Bold);
                }

            }
            else
            {
                ApplyChange(textEditor, (obj) =>
                {
                    if (obj is Run run)
                    {
                        if (run.FontWeight == FontWeights.Normal)
                        {
                            run.FontWeight = FontWeights.Bold;
                        }
                        else
                        {
                            run.FontWeight = FontWeights.Normal;
                        }
                    }
                    else if (obj is Paragraph paragraph)
                    {
                        if (paragraph.FontWeight == FontWeights.Normal)
                        {
                            paragraph.FontWeight = FontWeights.Bold;
                        }
                        else
                        {
                            paragraph.FontWeight = FontWeights.Normal;
                        }
                    }
                }
                );
            }
        }

        public static void ToggleItalic(RichTextBox textEditor)
        {
            if (!(textEditor.Selection.IsEmpty))
            {
                if (textEditor.Selection.GetPropertyValue(TextElement.FontStyleProperty).Equals(FontStyles.Italic))
                {
                    ApplySelectionChange(textEditor.Selection, TextElement.FontStyleProperty, FontStyles.Normal);
                }
                else
                {
                    ApplySelectionChange(textEditor.Selection, TextElement.FontStyleProperty, FontStyles.Italic);
                }
            }
            else
            {
                ApplyChange(textEditor, (obj) =>
                {
                    if (obj is Run run)
                    {
                        if (run.FontStyle == FontStyles.Normal)
                        {
                            run.FontStyle = FontStyles.Italic;
                        }
                        else
                        {
                            run.FontStyle = FontStyles.Normal;
                        }
                    }
                    else if (obj is Paragraph paragraph)
                    {
                        if (paragraph.FontStyle == FontStyles.Normal)
                        {
                            paragraph.FontStyle = FontStyles.Italic;
                        }
                        else
                        {
                            paragraph.FontStyle = FontStyles.Normal;
                        }
                    }
                });
            }

        }

        public static void ToggleUnderlined(RichTextBox textEditor)
        {
            if (!(textEditor.Selection.IsEmpty))
            {
                if (textEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty) == TextDecorations.Underline)
                {
                    ApplySelectionChange(textEditor.Selection, Inline.TextDecorationsProperty, null);

                }
                else
                {
                    ApplySelectionChange(textEditor.Selection, Inline.TextDecorationsProperty, TextDecorations.Underline);
                }
            }
            else
            {
                ApplyChange(textEditor, (obj) =>
                {
                    if (obj is Run run)
                    {
                        if (run.TextDecorations == null)
                        {
                            run.TextDecorations = TextDecorations.Underline;
                        }
                        else
                        {

                            run.TextDecorations = null;
                        }
                    }
                    else if (obj is Paragraph paragraph)
                    {
                        if (paragraph.TextDecorations == null)
                        {
                            paragraph.TextDecorations = TextDecorations.Underline;
                        }
                        else
                        {
                            paragraph.TextDecorations = null;
                        }
                    }
                });
            }
        }

        public static void ToggleStrikethrough(RichTextBox textEditor)
        {
            if (!(textEditor.Selection.IsEmpty))
            {
                if (textEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty) == TextDecorations.Strikethrough)
                {
                    ApplySelectionChange(textEditor.Selection, Inline.TextDecorationsProperty, null);

                }
                else
                {
                    ApplySelectionChange(textEditor.Selection, Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
                }
            }
            else
            {
                ApplyChange(textEditor, (obj) =>
                {
                    if (obj is Run run)
                    {
                        if (run.TextDecorations == null)
                        {
                            run.TextDecorations = TextDecorations.Strikethrough;
                        }
                        else
                        {
                            run.TextDecorations = null;
                        }
                    }
                    else if (obj is Paragraph paragraph)
                    {
                        if (paragraph.TextDecorations == null)
                        {
                            paragraph.TextDecorations = TextDecorations.Strikethrough;
                        }
                        else
                        {
                            paragraph.TextDecorations = null;
                        }
                    }
                });
            }
        }

        #endregion

        #region Fonts
        public static void ChangeFont(RichTextBox textEditor, string selectedFont)
        {
            FontFamily fontFamily = new FontFamily(selectedFont);
            if (!(textEditor.Selection.IsEmpty))
            {
                ApplySelectionChange(textEditor.Selection, TextElement.FontFamilyProperty, fontFamily);
            }
            else
            {
                ApplyChange(textEditor, (obj) =>
                {
                    if (obj is Run run)
                    {
                        run.FontFamily = fontFamily;
                    }
                    else if (obj is Paragraph paragraph)
                    {
                        paragraph.FontFamily = fontFamily;
                    }
                }
                );
            }

            textEditor.Focus();
        }

        public static void ChangeFontSize(RichTextBox textEditor, int fontSize)
        {
            if (!(textEditor.Selection.IsEmpty))
            {
                ApplySelectionChange(textEditor.Selection, TextElement.FontSizeProperty, ((double)fontSize));
            }
            else
            {
                ApplyChange(textEditor, (obj) =>
                {
                    if (obj is Run run)
                    {
                        run.FontSize = fontSize;
                    }
                    else if (obj is Paragraph paragraph)
                    {
                        paragraph.FontSize = fontSize;
                    }
                });
            }

            textEditor.Focus();
        }
        #endregion

        #region Helper methods for applying text from now on(the point in the text where it was typed) or selectively
        /// <summary>
        /// Use this method when text is selected.
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="formattingProperty">A value property which can be applied to the selection</param>
        /// <param name="value">A value which fits together with the value property</param>
        private static void ApplySelectionChange(TextSelection selection, DependencyProperty formattingProperty, object value)
        {
            if (ValidationUtil.AreNotNull(selection, formattingProperty))
            {
                selection.ApplyPropertyValue(formattingProperty, value);
            }
        }

        /// <summary>
        /// Use this method when no text is selected. It means that from now on the 'change' will take effect.
        /// </summary>
        /// <param name="textEditor"></param>
        /// <param name="change"></param>
        private static void ApplyChange(RichTextBox textEditor, Action<object> change)
        {
            if (ValidationUtil.AreNotNull(textEditor, textEditor.Selection, change))
            {
                if (textEditor.Selection.Start.Paragraph == null)
                {
                    Paragraph paragraph = new Paragraph();
                    change(paragraph);
                    textEditor.Document.Blocks.Add(paragraph);

                }
                else
                {
                    TextPointer currentCaret = textEditor.CaretPosition;
                    Block curBlock = textEditor.Document.Blocks.Where
                        (x => x.ContentStart.CompareTo(currentCaret) == -1 && x.ContentEnd.CompareTo(currentCaret) == 1).FirstOrDefault();
                    if (curBlock != null)
                    {
                        Paragraph curParagraph = curBlock as Paragraph;

                        Inline currentRun = curParagraph.Inlines.Where(
                            (inline) => new TextRange(inline.ElementStart, inline.ElementEnd).Contains(textEditor.CaretPosition)).FirstOrDefault();

                        // Generate the new 'run' nodes, where the new piece of text is supposed to be in the middle.
                        Run firstHalf = new Run(new TextRange(currentRun.ContentStart, textEditor.CaretPosition).Text);
                        Run secondHalf = new Run(new TextRange(textEditor.CaretPosition, currentRun.ContentEnd).Text);
                        Run newContent = new Run(" ", textEditor.CaretPosition.GetInsertionPosition(LogicalDirection.Forward));

                        // Copy all the styling data (text deco, font, font size, etc...)
                        CopyStyleData(currentRun, firstHalf, secondHalf, newContent);

                        curParagraph.Inlines.InsertBefore(currentRun, firstHalf);
                        curParagraph.Inlines.Remove(currentRun);

                        TextRange range = new TextRange(currentRun.ElementStart, currentRun.ElementStart)
                        {
                            Text = ""
                        };

                        change(newContent);

                        // Reset the cursor into the new block. 
                        // If we don't do this, the font size will default again when you start typing.
                        textEditor.CaretPosition = newContent.ContentStart.GetPositionAtOffset(1);


                        textEditor.Focus();

                    }
                }

            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Copy all decorative data (fontweight, fontfamily, fontsize, textdecorations, etc.)
        /// </summary>
        /// <param name="currentRun"></param>
        /// <param name="runs"></param>
        private static void CopyStyleData(Inline currentRun, params Run[] runs)
        {
            foreach (Run run in runs)
            {
                run.FontWeight = currentRun.FontWeight;
                run.FontFamily = currentRun.FontFamily;
                run.FontSize = currentRun.FontSize;
                run.TextDecorations = currentRun.TextDecorations;
                run.FontStyle = currentRun.FontStyle;
            }
        }

        #endregion
    }
}
