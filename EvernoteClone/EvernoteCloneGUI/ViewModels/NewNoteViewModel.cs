using Caliburn.Micro;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace EvernoteCloneGUI.ViewModels
{
    public class NewNoteViewModel : Screen
    {


        // We can use this later on to add the notification changes
        public string Title { get; set; }
        public string NewContent
        {
            get
            {
                return Note.NewContent;
            }
            set
            {
                Note.NewContent = value;
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

        public NewNoteViewModel()
        {
            DisplayName = "NoteFever | Nameless note";
        }

        public bool SaveNote()
        {
            // For testing purposes.
            // Sometime in the future, we actually need to know the notebook beforehand.
            if (Constant.TEST_MODE && NoteOwner == null)
            {

                NoteOwner = new Notebook()
                {
                    Id = 71,
                    UserID = 3,
                    LocationID = 1,
                    Title = "Nameless title",
                    CreationDate = DateTime.Now.Date,
                    LastUpdated = DateTime.Now,
                };
            }


            // Set some standard values for now and save
            Note.Author = "Nameless author"; // If user is logged in, this should obv. be different!
            Note.Title = Title; // We don't have to check if it is empty or null, the property in note does that already
            Note.Save();

            // Add the note to the notes list
            NoteOwner.Notes.Add(Note);

            // If the NoteOwner isn't null, we fetch the Id of the user it contains
            // though NoteOwner should never occur
            if (NoteOwner != null)
            {
                return NoteOwner.Save(NoteOwner.UserID);
            }

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
                TryClose();
            }
            else
            {
                MessageBox.Show("We were unable to save your note.", "NoteFever | Failed.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Method which retrieves the new contents from the richtextbox.
        /// </summary>
        /// <param name="textChangedEventArgs"></param>
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

    }
}
