using Caliburn.Micro;
using EvernoteCloneGUI.Views;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

// TODO summary @Lucas
namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// ViewModel which handles all interaction related to the NewNoteView
    /// </summary>
    public class NewNoteViewModel : Screen
    {
        private string _title = "";
        private readonly bool _loadNote;

        // We can use this later on to add the notification changes
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
                        DisplayName = $"NoteFever | {_title}";
                    else
                        DisplayName = $"NoteFever | Nameless note";
                }
                else
                    if (Parent is NoteFeverViewModel container && container.NotebookViewModel?.SelectedNoteElement != null)
                        container.NotebookViewModel.SelectedNoteElement.Title = _title;

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

        /// <summary>
        /// If this object is null, or has id '-1' it means we are generating a new note,
        /// otherwise we are updating one.
        /// </summary>
        public Note Note { get; set; } = new Note();

        /// <summary>
        /// Every note is part of a notebook, therefore we need the object when saving.
        /// </summary>
        public Notebook NoteOwner { get; set; }

        // TODO add all bindings
        public NewNoteViewModel(bool loadNote = false)
        {
            DisplayName = "NoteFever | Nameless note";
            _loadNote = loadNote;
        }

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
        public bool SaveNote()
        {
            // Set some standard values for now and save
            Note.Author = "Nameless author"; // If user is logged in, this should obv. be different!
            Note.Title = Title; // We don't have to check if it is empty or null, the property in note does that already
            Note.Save();
            
            if (Note.NoteOwner == null && NoteOwner != null && !(NoteOwner.IsNotNoteOwner))
                Note.NoteOwner = NoteOwner;


            // Add the note to the notes list
            if (NoteOwner != null && !NoteOwner.Notes.Contains(Note))
                NoteOwner.Notes.Add(Note);

            if (Parent != null && Parent is NoteFeverViewModel noteFeverViewModel)
            {
                // Dirty test code
                if (Constant.TEST_MODE && NoteOwner != null)
                {
                    if (noteFeverViewModel.SelectedNotebook == null)
                    {
                        noteFeverViewModel.Notebooks.Add(NoteOwner);
                        noteFeverViewModel.SelectedNotebook = NoteOwner;
                    }
                }

                noteFeverViewModel.NotebookViewModel?.NotebookNotesMenu?.LoadNotesIntoNotebookMenu();
            }

            // If the NoteOwner isn't null, we fetch the Id of the user it contains
            // though NoteOwner should never be null
            if (NoteOwner != null)
                return NoteOwner.Save(NoteOwner.UserID);

            return false;
        }

        /// <summary>
        /// Method which saves the note and notifies the user if it happened or not
        /// </summary>
        public void NotifyUserOfSave()
        {
            if (SaveNote())
            {
                MessageBox.Show("Note was saved successfully!", "NoteFever | Saved.", MessageBoxButton.OK, MessageBoxImage.Information);
                if (Parent != null)
                {
                    if (Parent is NoteFeverViewModel parent)
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
                MessageBox.Show("We were unable to save your note.", "NoteFever | Failed.", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Method which retrieves the new contents from the richtextbox.
        /// </summary>
        /// <param name="TextChangedEventArgs"></param>
        public void StoreRichTextBoxContent(TextChangedEventArgs TextChangedEventArgs)
        {
            if (TextChangedEventArgs.Source is RichTextBox richTextBox)
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
        /// Method which loads all the contents of the XaML string into the text editor.
        /// <see cref="https://stackoverflow.com/questions/1449121/how-to-insert-xaml-into-richtextbox">Impl. from here</see>
        /// </summary>
        /// <param name="XamlString"></param>
        /// <returns></returns>
        private FlowDocument SetRtf(string XamlString)
        {
            if (!(string.IsNullOrEmpty(XamlString.Trim())))
            {
                StringReader stringReader = new StringReader(XamlString);
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Parse,
                    MaxCharactersFromEntities = 1024
                };

                XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings);
                FlowDocument doc = new FlowDocument();

                if (XamlReader.Load(xmlReader) is Section sec)
                    while (sec.Blocks.Count > 0)
                        doc.Blocks.Add(sec.Blocks.FirstBlock);

                return doc;
            }

            return new FlowDocument();
        }

        /// <summary>
        /// When the view is ready, and we are loading the note, this is the only way to
        /// attach the content to the text editor.
        /// </summary>
        /// <param name="View"></param>
        protected override void OnViewReady(object View)
        {
            if (Note.Content == null)
                Note.Content = "";

            if (_loadNote)
            {
                NewNoteView newNoteView = (NewNoteView)View;
                newNoteView.TextEditor.Document = SetRtf(NewContent);
            }

            base.OnViewReady(View);
        }

        protected override void OnViewAttached(object View, object Context)
        {
            if (View is NewNoteView newNoteView)
                SetupTextEditor(newNoteView);
        }

        /// <summary>
        /// Method which loads the text into the text editor.
        /// Beforehand it set the content to an empty string if it is currently null, since this might
        /// cause unexpected behaviour otherwise.
        /// </summary>
        /// <param name="NewNoteView"></param>
        private void SetupTextEditor(NewNoteView NewNoteView)
        {
            if (Note.Content == null)
                Note.Content = "";

            if (_loadNote)
                NewNoteView.TextEditor.Document = SetRtf(NewContent);
        }
    }
}
