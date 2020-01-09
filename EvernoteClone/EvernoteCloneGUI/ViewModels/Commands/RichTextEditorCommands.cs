using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using EvernoteCloneLibrary.Utils;
using Caliburn.Micro;
using System.Linq;
using EvernoteCloneGUI.ViewModels.Popups;
// ReSharper disable PossibleNullReferenceException

namespace EvernoteCloneGUI.ViewModels.Commands
{
    /// <summary>
    /// Contains all actions for the rich text editor buttons.
    /// </summary>
    public static class RichTextEditorCommands
    {

        #region Graphical (tables, separators, codeblocks)
        
        /// <summary>
        /// Generate a table programmatically, including formatting, and insert it in the specified textEditor. 
        /// </summary>
        /// <param name="textEditor"></param>
        public static void InsertTable(RichTextBox textEditor)
        {

            // Get the row and column data, there is no use for the code if either the rows or cols are at 0
            (uint rows, uint columns) = OpenTableDataSpecifier();
            if (rows > 0 && columns > 0)
            {
                // Generate the table columns
                Table table = new Table
                {
                    CellSpacing = 5
                };


                for (int i = 0; i < columns; i++)
                {
                    table.Columns.Add(new TableColumn
                    {
                        Width = new GridLength(10, GridUnitType.Star)

                    }
                    );
                }

                // Generate the rows
                table.RowGroups.Add(new TableRowGroup());
                for (int i = 0; i < rows; i++)
                {
                    TableRow tableRow = new TableRow();
                    // make even table rows light gray and uneven table rows white.
                    if (i % 2 == 0)
                    {
                        tableRow.Background = Brushes.LightGray;
                    }
                    else
                    {
                        tableRow.Background = Brushes.White;
                    }

                    table.RowGroups[0].Rows.Add(tableRow);
                    
                    for (int j = 0; j < columns; j++)
                    {
                        table.RowGroups[0].Rows[i].Cells.Add(new TableCell(new Paragraph(new Run("")))
                        {
                            FontSize = 12,
                            Padding = new Thickness(1, 1, 1, 1) // Add a small padding so the cursor can be seen
                        }
                        );
                    }
                }

                Block positionToInsertAt = GetNearestPosition(textEditor);

                // If we can insert the table near our current position, we do this. Otherwise, insert it at the end of the document 
                if (positionToInsertAt != null)
                {
                    textEditor.Document.Blocks.InsertAfter(positionToInsertAt, table);

                    // Add a newline after the table if it is the last element.
                    if (textEditor.Document.Blocks.LastBlock.Equals(table))
                    {
                        textEditor.Document.Blocks.Add(new Paragraph(new Run("")));
                    }

                }
                else
                {
                    textEditor.Document.Blocks.Add(table);

                    // Add a newline after the table (we can already assume it is on the last line)
                    textEditor.Document.Blocks.Add(new Paragraph(new Run("")));
                }
            }


        }

        #endregion

        #region Color

        /// <summary>
        /// Handles the setting of textcolor of a selection or setting the color of the 'from now on' text
        /// </summary>
        /// <param name="textEditor"></param>
        public static void SetTextColor(RichTextBox textEditor)
        {
            // get the hex color value and generate a brush using it
            string hexadecimalColor = OpenColorPickRequest();
            if (hexadecimalColor != null)
            {
                Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexadecimalColor));

                ApplyChange(textEditor, (run) =>
                {
                    run.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
                });
            }

            textEditor.Focus();
        }


        /// <summary>
        /// Handles the toggling of the marking of a selection or the 'from now on' text
        /// </summary>
        /// <param name="textEditor"></param>
        public static void ToggleTextMarking(RichTextBox textEditor)
        {
            ApplyChange(textEditor, (selection) =>
            {
                if (!Equals(selection.GetPropertyValue(TextElement.BackgroundProperty), Brushes.Yellow))
                {
                    selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
                }
                else
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    selection.ApplyPropertyValue(TextElement.BackgroundProperty, null);
                }
            });

            textEditor.Focus();
        }

        #endregion

        #region Text decoration

        /// <summary>
        /// Handles the toggling of the strikethrough of a selection or the 'from now on' text
        /// </summary>
        /// <param name="textEditor"></param>
        public static void ToggleStrikethrough(RichTextBox textEditor)
        {
            ApplyChange(textEditor, (selection) =>
            {
                
                if (!Equals(selection.GetPropertyValue(Inline.TextDecorationsProperty), TextDecorations.Strikethrough))
                {
                    selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
                }
                else
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                }
            });

            textEditor.Focus();
        }

        #endregion

        #region Fonts
       
        /// <summary>
        /// Change the font for the current selection or the 'from now on' text.
        /// </summary>
        /// <param name="textEditor"></param>
        /// <param name="selectedFont"></param>
        public static void ChangeFont(RichTextBox textEditor, string selectedFont)
        {
            FontFamily fontFamily = new FontFamily(selectedFont);

            ApplyChange(textEditor, (selection) =>
            {
                selection.ApplyPropertyValue(TextElement.FontFamilyProperty, fontFamily);
            });

            textEditor.Focus();
        }

        /// <summary>
        /// Change the font size for the current selection or the 'from now on' text.
        /// </summary>
        /// <param name="textEditor"></param>
        /// <param name="fontSize"></param>
        public static void ChangeFontSize(RichTextBox textEditor, int fontSize)
        {
            ApplyChange(textEditor, (selection) =>
            {
                selection.ApplyPropertyValue(TextElement.FontSizeProperty, ((double)fontSize));
            });

            textEditor.Focus();
        }
        #endregion

        #region Helper methods for applying text from now on(the point in the text where it was typed) or selectively

        /// <summary>
        /// Prepares and applies the change given by the 'change' Action.
        /// </summary>
        /// <param name="textEditor"></param>
        /// <param name="change"></param>
        private static void ApplyChange(RichTextBox textEditor, Action<TextSelection> change)
        {
            if (ValidationUtil.AreNotNull(textEditor, textEditor.Selection, change))
            {
                // Prepare a paragraph to apply changes upon if there is none yet.
                if (textEditor.Selection.Start.Paragraph == null)
                {
                    Paragraph paragraph = new Paragraph();
                    Run run = new Run();
                    paragraph.Inlines.Add(run);
                    textEditor.Document.Blocks.Add(paragraph);
                }

                // apply the changes to the selection. 
                change(textEditor.Selection);
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Finds and returns the nearest block at our current cursor position
        /// </summary>
        /// <param name="textEditor"></param>
        /// <returns></returns>
        private static Block GetNearestPosition(RichTextBox textEditor)
        {
            return textEditor.Document.Blocks
                .FirstOrDefault(x => x.ContentStart.CompareTo(textEditor.CaretPosition) == -1 && x.ContentEnd.CompareTo(textEditor.CaretPosition) == 1);
        }

        /// <summary>
        /// Opens a popup where the user can insert a HEX-value which will be used as color.
        /// </summary>
        /// <returns></returns>
        private static string OpenColorPickRequest()
        {
            IWindowManager windowManager = new WindowManager();

            string output = "";

            ValueRequestViewModel valueRequestViewModel = new ValueRequestViewModel
            {
                DialogTitle = Properties.Settings.Default.RichTextEditorCommandsColorTitle,
                DialogValueRequestText = Properties.Settings.Default.RichTextEditorCommandsInsertHEX
            };
            valueRequestViewModel.Submission += model =>
            {
                try
                {
                    if (model.Value.Length == 3 || model.Value.Length == 6)
                    {
                        // If this doesn't error, it means that the value is a proper hexadecimal
                        Int32.Parse(model.Value, System.Globalization.NumberStyles.HexNumber);

                        output = $"#{model.Value}";
                        model.TryClose(true);
                        return;
                    }



                }
                catch (Exception)
                {
                    // ignored
                }

                // When an error occurs or when the hex value is an improper value, show this error message.
                MessageBox.Show(Properties.Settings.Default.RichTextEditorCommandsProvideHexadecimal, Properties.Settings.Default.MessageBoxTitleNotice, MessageBoxButton.OK, MessageBoxImage.Error);
            };

            valueRequestViewModel.Cancellation += model => model.TryClose(false);
            bool success = windowManager.ShowDialog(valueRequestViewModel) ?? false;

            return (success ? output : null);
        }

        /// <summary>
        /// Open a window and return a tuple containing: (row count, column count)
        /// </summary>
        /// <returns></returns>
        private static (uint, uint) OpenTableDataSpecifier()
        {

            IWindowManager windowManager = new WindowManager();
            TableRowColumnSpecifierViewModel rowColumnSpecifierViewModel
                = new TableRowColumnSpecifierViewModel();

            bool closed = windowManager.ShowDialog(rowColumnSpecifierViewModel) ?? false;

            // Validate whether the model submission was successful, if so, return the (row, col) tuple.
            if (closed && rowColumnSpecifierViewModel.Submitted && !rowColumnSpecifierViewModel.Cancelled)
            {
                return (rowColumnSpecifierViewModel.RowCount, rowColumnSpecifierViewModel.ColumnCount);
            }

            return (0, 0);
        }

        #endregion


    }
}