using EvernoteCloneLibrary.SharedNotes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibraryTests.SharedNotes
{
    [TestFixture, Order(2)]
    public class SharedNoteRepositoryTests
    {
        [Order(1)]
        [TestCase (null)]
        public void InsertSharedNote(int noteId = 2, int userId = -1)
        {
            //Arrange 
            SharedNoteRepository sharedNoteRepository = new SharedNoteRepository();
            SharedNoteModel sharedNoteModel = new SharedNoteModel()
            {
                NoteId = noteId,
                UserId = userId
            };


            //Act
            bool result = sharedNoteRepository.Insert(sharedNoteModel);

            //Assert
            Assert.IsTrue(result);
        }
    }
}
