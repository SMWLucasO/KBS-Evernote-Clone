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

        private static bool IsBold = false;

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
                        if (IsBold)
                        {
                            run.FontWeight = FontWeights.Normal;
                            IsBold = false;
                        }
                        else
                        {
                            run.FontWeight = FontWeights.Bold;
                            IsBold = true;
                        }

                    }
                    else if (obj is Paragraph paragraph)
                    {
                        if (IsBold)
                        {
                            paragraph.FontWeight = FontWeights.Normal;
                        }
                        else
                        {
                            paragraph.FontWeight = FontWeights.Bold;
                            IsBold = true;
                        }
                    }
                }
                );
            }
        }

        public static void ToggleItalic(RichTextBox textEditor)
        {

        }

        public static void ToggleUnderlined(RichTextBox textEditor)
        {

        }

        public static void ToggleStrikethrough(RichTextBox textEditor)
        {

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


        }

        #endregion

        /// <summary>
        /// Use this method when text is selected.
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="formattingProperty">A value property which can be applied to the selection</param>
        /// <param name="value">A value which fits together with the value property</param>
        private static void ApplySelectionChange(TextSelection selection, DependencyProperty formattingProperty, object value)
        {
            if (ValidationUtil.AreNotNull(selection, formattingProperty, value))
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

        private static void CopyStyleData(Inline currentRun, params Run[] runs)
        {
            foreach (Run run in runs)
            {
                run.FontWeight = currentRun.FontWeight;
                run.FontFamily = currentRun.FontFamily;
                run.FontSize = currentRun.FontSize;
                run.TextDecorations = currentRun.TextDecorations;
            }
        }
    }
}
