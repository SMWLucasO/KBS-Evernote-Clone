using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using EvernoteCloneLibrary.Utils;
using Caliburn.Micro;
using System.Linq;
using System.Windows.Shapes;

namespace EvernoteCloneGUI.ViewModels.Commands
{
    /// <summary>
    /// Contains all actions for the rich text editor buttons.
    /// </summary>
    public static class RichTextEditorCommands
    {

        #region Graphical (tables, separators, codeblocks)


        public static void InsertCodeBlocks(RichTextBox textEditor)
        {
            TextBox codeBlock = new TextBox()
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#dddddd")),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1, 1, 1, 1),
                MinWidth = 500,
                MinHeight = 200,
                Padding = new Thickness(5, 5, 5, 5),

            };

            Block nearest = GetNearestPosition(textEditor);

            Paragraph paragraph = new Paragraph();
            InlineUIContainer container = new InlineUIContainer(codeBlock);

            paragraph.Inlines.Add(container);
            if (nearest != null)
            {
                textEditor.Document.Blocks.InsertAfter(nearest, paragraph);
                textEditor.Document.Blocks.InsertAfter(paragraph, new Paragraph(new Run("")));
            }
            else
            {
                textEditor.Document.Blocks.Add(paragraph);
                textEditor.Document.Blocks.Add(new Paragraph(new Run("")));
            }

        }

        public static void InsertHorizontalLine(RichTextBox textEditor)
        {
            // create the horizontal line and find the nearest position to the cursor
            Separator separator = new Separator()
            {
                Background = Brushes.Black,
                Padding = new Thickness(0, 5, 0, 5),
                Height = 2,
                MinWidth = 500
            };

            Block nearest = GetNearestPosition(textEditor);

            // create a paragraph and insert the separator in it
            Paragraph paragraph = new Paragraph()
            {
                TextAlignment = TextAlignment.Center,
            };

            paragraph.Inlines.Add(separator);

            // When there is a block nearby, we insert the line afterwards in a new paragraph
            // else we just add the paragraph to the end of the document.
            // We add a newline paragraph for ease-of-use.
            if (nearest != null)
            {
                textEditor.Document.Blocks.InsertAfter(nearest, paragraph);
                textEditor.Document.Blocks.InsertAfter(paragraph, new Paragraph(new Run("")));
            }
            else
            {
                textEditor.Document.Blocks.Add(paragraph);
                textEditor.Document.Blocks.Add(new Paragraph(new Run("")));
            }

        }

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
                if (selection.GetPropertyValue(TextElement.BackgroundProperty) != Brushes.Yellow)
                {
                    selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Yellow);
                }
                else
                {
                    selection.ApplyPropertyValue(TextElement.BackgroundProperty, null);
                }
            });

            textEditor.Focus();
        }

        #endregion

        #region Text decoration

        public static void ToggleStrikethrough(RichTextBox textEditor)
        {
            ApplyChange(textEditor, (selection) =>
            {
                if (selection.GetPropertyValue(Inline.TextDecorationsProperty) != TextDecorations.Strikethrough)
                {
                    selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Strikethrough);
                }
                else
                {
                    selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                }
            });

            textEditor.Focus();
        }

        #endregion

        #region Fonts
        public static void ChangeFont(RichTextBox textEditor, string selectedFont)
        {
            FontFamily fontFamily = new FontFamily(selectedFont);

            ApplyChange(textEditor, (selection) =>
            {
                selection.ApplyPropertyValue(TextElement.FontFamilyProperty, fontFamily);
            });

            textEditor.Focus();
        }

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
        /// Use this method when no text is selected. It means that from now on the 'change' will take effect.
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
                .Where(x => x.ContentStart.CompareTo(textEditor.CaretPosition) == -1 && x.ContentEnd.CompareTo(textEditor.CaretPosition) == 1)
                .FirstOrDefault();
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
                DialogTitle = "Pick a color",
                DialogValueRequestText = "Insert a HEX color to change the color to (without the '#')"
            };
            valueRequestViewModel.Submission += (model) =>
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
                catch (Exception) { }
                // When an error occurs or when the hex value is an improper value, show this error message.
                MessageBox.Show("Please provide a valid hexadecimal color.", "Note Fever", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            valueRequestViewModel.Cancellation += (model) => model.TryClose(false);
            bool success = windowManager.ShowDialog(valueRequestViewModel) ?? false;

            return (success ? output : null);
        }

        /// <summary>
        /// Open a window and return a tuple containing: (rowcount, columncount)
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