using Caliburn.Micro;
using EvernoteCloneGUI.ViewModels.Commands;
using EvernoteCloneGUI.Views;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using EvernoteCloneLibrary.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using System.Linq;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// ViewModel which handles all interaction related to the NewNoteView
    /// </summary>
    public class NewNoteViewModel : Screen
    {

        #region Instance variables
        private readonly bool _loadNote;

        private string _font = "";
        private int _fontSize = 12;

        private RichTextBox _textEditor = null;
        #endregion

        #region Databound properties

        private string _title = "";

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);

                if (!_loadNote)
                {
                    // Dynamically set the title of the window.
                    if (!(string.IsNullOrEmpty(_title)))
                    {
                        DisplayName = $"Note Fever | {_title}";
                    }
                    else
                    {
                        DisplayName = $"Note Fever | Nameless note";
                    }
                }
                else
                {
                    if (Parent is NoteFeverViewModel container && container.NotebookViewModel?.SelectedNoteElement != null)
                    {
                        container.NotebookViewModel.SelectedNoteElement.Title = _title;
                    }
                }
            }
        }

        public string NewContent
        {
            get => Note.NewContent;
            set
            {
                Note.NewContent = value;
                NotifyOfPropertyChange(() => NewContent);
            }
        }

        public List<string> Fonts { get; set; } = new List<string>();
        public string SelectedFont
        {
            get
            {
                return _font;
            }
            set
            {
                // make sure that the value is not null & the given value is an existing font.
                if (value != null && FontFamily.Families.Where((family) => family.Name == value).ToList().Count > 0)
                {
                    _font = value;
                    RichTextEditorCommands.ChangeFont(_textEditor, _font);
                }

            }
        }

        public List<int> FontSizes { get; set; } = new List<int>();

        public int SelectedFontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                if (value > 0 && value < 250)
                {
                    _fontSize = value;
                    RichTextEditorCommands.ChangeFontSize(_textEditor, _fontSize);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// If this object is null, or has id '-1' it means we are generating a new note,
        /// otherwise we are updating one.
        /// </summary>
        public Note Note { get; set; } = new Note();

        /// <summary>
        /// Every note is part of a notebook, therefore we need the object when saving.
        /// </summary>
        public Notebook NoteOwner { get; set; }

        #endregion

        // TODO add all bindings
        public NewNoteViewModel(bool loadNote = false)
        {
            DisplayName = "Note Fever | Nameless note";
            _loadNote = loadNote;
        }

        #region Saving and loading

        /// <summary>
        /// Method for loading the contents of the note object into the databound properties.
        /// </summary>
        public void LoadNote()
        {
            if (Note != null)
            {
                Title = Note.Title;
                NewContent = Note.Content;
            }
        }

        /// <summary>
        /// Method for storing the note into the database.
        /// </summary>
        /// <returns></returns>
        public bool SaveNote(bool showDeletedNotes = false)
        {
            // Set some standard values for now and save
            Note.Author = "Nameless author"; // If user is logged in, this should obv. be different!
            Note.Title = Title; // We don't have to check if it is empty or null, the property in note does that already
            Note.Save();

            if (Note.NoteOwner == null && NoteOwner != null && !(NoteOwner.IsNotNoteOwner))
            {
                Note.NoteOwner = NoteOwner;
            }

            // Add the note to the notes list
            if (NoteOwner != null && !NoteOwner.Notes.Contains(Note))
            {
                NoteOwner.Notes.Add(Note);
            }

            if (Parent != null && Parent is NoteFeverViewModel noteFeverViewModel)
            {
                // Dirty test code
                // TODO remove this code and make sure everytinh still works.
                if (Constant.TEST_MODE && NoteOwner != null)
                {
                    if (noteFeverViewModel.SelectedNotebook == null)
                    {
                        noteFeverViewModel.Notebooks.Add(NoteOwner);
                        noteFeverViewModel.SelectedNotebook = NoteOwner;
                    }
                }

                noteFeverViewModel.NotebookViewModel?.NotebookNotesMenu?.LoadNotesIntoNotebookMenu(showDeletedNotes);
            }

            // If the NoteOwner isn't null, we fetch the Id of the user it contains
            // though NoteOwner should never be null
            if (NoteOwner != null)
            {
                return NoteOwner.Save(NoteOwner.UserId);
            }
                
            return false;
        }

        /// <summary>
        /// Method which saves the note and notifies the user if it happened or not
        /// </summary>
        public void NotifyUserOfSave()
        {
           
            bool shouldShowDeleted = false;
            NoteFeverViewModel parent = null;
            
            // Validate whether deleted notes should be shown or not
            if (Parent != null && Parent is NoteFeverViewModel)
            {
                parent = (NoteFeverViewModel)Parent;
                if (ValidationUtil.AreNotNull(parent.NotebookViewModel, parent.NotebookViewModel.NotebookNotesMenu))
                {
                    shouldShowDeleted = parent.NotebookViewModel.NotebookNotesMenu.ShowDeletedNotes;
                }
            }

            if (SaveNote(shouldShowDeleted))
            {
                MessageBox.Show("Note was saved successfully!", "Note Fever | Saved.", MessageBoxButton.OK, MessageBoxImage.Information);
                if (Parent != null)
                {
                    // If _loadNote is true, we are not creating a new note. Thus, we will only load the note and close the window
                    // when it is false.
                    if (Parent is NoteFeverViewModel)
                    {
                        if (GetView() != null && !(_loadNote))
                        {
                            parent.SelectedNote = Note;
                            parent.LoadNoteViewIfNoteExists();
                            (GetView() as Window)?.Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("We were unable to save your note.", "Note Fever | Failed.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Method which retrieves the new contents from the richtextbox.
        /// </summary>
        /// <param name="textChangedEventArgs"></param>
        public void StoreRichTextBoxContent(TextChangedEventArgs textChangedEventArgs)
        {
            if (textChangedEventArgs.Source is RichTextBox richTextBox)
            {
                // Get the text from the richtextbox and create/read from a stream to get the data
                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                MemoryStream stream = new MemoryStream();
                range.Save(stream, DataFormats.Xaml);

                // Generate the XaML content
                NewContent = Encoding.Default.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// When the view is ready, and we are loading the note, this is the only way to
        /// attach the content to the text editor.
        /// </summary>
        /// <param name="view"></param>
        protected override void OnViewReady(object view)
        {
            if (Note.Content == null)
            {
                Note.Content = "";
            }
                
            // when we are loading a note and not creating a new one, we load in the contents of said note
            if (_loadNote)
            {
                NewNoteView newNoteView = (NewNoteView)view;
                newNoteView.TextEditor.Document = SetRtf(NewContent);
            }

            base.OnViewReady(view);
        }

        /// <summary>
        /// When the view is attached, prepare the text editor for usage
        /// </summary>
        /// <param name="view"></param>
        /// <param name="context"></param>
        protected override void OnViewAttached(object view, object context)
        {
            if (view is NewNoteView newNoteView)
            {
                _textEditor = newNoteView.TextEditor;
                _textEditor.MinHeight = SystemParameters.FullPrimaryScreenHeight;
                SetupTextEditor(newNoteView);
            }

        }

        #region Toolbar events

        public void OnInsertTable()
        {
            RichTextEditorCommands.InsertTable(_textEditor);
        }

        public void OnInsertHorizontalLine()
        {
            RichTextEditorCommands.InsertHorizontalLine(_textEditor);
        }

        public void OnToggleStrikethrough()
        {
            RichTextEditorCommands.ToggleStrikethrough(_textEditor);
        }

        public void OnSetTextColor()
        {
            RichTextEditorCommands.SetTextColor(_textEditor);
        }

        public void OnToggleTextMarking()
        {
            RichTextEditorCommands.ToggleTextMarking(_textEditor);
        }

        #endregion


        #endregion

        #region Helper methods

        /// <summary>
        /// Method which loads all the contents of the XaML string into the text editor.
        /// <see cref="https://stackoverflow.com/questions/1449121/how-to-insert-xaml-into-richtextbox">Impl. from here</see>
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        private FlowDocument SetRtf(string xamlString)
        {
            // validate whether the xaml string is empty or not, since if it is, it would throw an error if we were to continue.
            if (!(string.IsNullOrEmpty(xamlString.Trim())))
            {
                StringReader stringReader = new StringReader(xamlString);
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse,
                    MaxCharactersFromEntities = 1024
                };

                XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings);
                FlowDocument doc = new FlowDocument();

                // insert all the sections into the document blocks.
                if (XamlReader.Load(xmlReader) is Section sec)
                {
                    while (sec.Blocks.Count > 0)
                    {
                        doc.Blocks.Add(sec.Blocks.FirstBlock);
                    }
                }
                
                return doc;
            }

            return new FlowDocument();
        }

        #endregion

        #region Setup methods

        /// <summary>
        /// Method which loads the text into the text editor.
        /// Beforehand it set the content to an empty string if it is currently null, since this might
        /// cause unexpected behaviour otherwise.
        /// </summary>
        /// <param name="newNoteView"></param>
        private void SetupTextEditor(NewNoteView newNoteView)
        {
            if (Note.Content == null)
            {
                Note.Content = "";
            }
                
            // Load all the fonts into the databound font list.
            foreach (FontFamily font in FontFamily.Families)
            {
                Fonts.Add(font.Name);
            }

            for (int i = 0; i < 200; ++i)
            {
                FontSizes.Add(i);
            }

            // defaults
            _fontSize = 11;
            SelectedFont = "Arial";

            NotifyOfPropertyChange(() => Fonts);

            if (_loadNote)
            {
                newNoteView.TextEditor.Document = SetRtf(NewContent);
            }
        }

        #endregion
    }
}
