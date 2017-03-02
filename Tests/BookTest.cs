using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using LibraryApp.Objects;

namespace LibraryApp
{
    public class BookTest : IDisposable
    {
        public BookTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void TEST_Save_CheckBookSaveToDb()
        {
            Book tempBook = new Book("math");
            List<Book> allBooks = new List<Book>{tempBook};
            tempBook.Save();

            Assert.Equal(allBooks, Book.GetAll());
        }

        [Fact]
       public void TEST_FindReturnsBook()
       {
           Book tempBook = new Book("Vyvy");
           tempBook.Save();

           Assert.Equal(tempBook, Book.Find(tempBook.GetId()));
       }

        [Fact]
        public void TEST_AddAuthor_AddAuthorToJoinTable()
        {
            Book tempBook = new Book("Harry Plopper");
            tempBook.Save();

            Author tempAuthor = new Author("Family Pig Melvin");
            tempAuthor.Save();

            List<Author> allAuthors = new List<Author>{tempAuthor};
            tempBook.AddAuthor(tempAuthor);
            Assert.Equal(allAuthors, tempBook.GetAuthors());
        }

        [Fact]
        public void TEST_DeleteSingle_DeleteSingleBookFromExistence()
        {
            Book tempBook = new Book("Playboy Magazine");
            tempBook.Save();
            Book tempBook2 = new Book("Hustler Magazine");
            tempBook2.Save();
            Author hefner = new Author("hefner");
            hefner.Save();
            tempBook.AddAuthor(hefner);
            tempBook.DeleteSingle();
            List<Book> testList = new List<Book>{tempBook2};
            Assert.Equal(testList, Book.GetAll());
        }

        [Fact]
        public void TEST_CheckAvailable_CheckIfSingleCopyExists()
        {
            Book tempBook = new Book("harry potter");
            tempBook.Save();
            Copy tempCopy = new Copy("harry potter", 0);
            Copy checkedOutCopy = new Copy("harry potter", 1);
            tempCopy.Save();
            checkedOutCopy.Save();
            tempCopy.AddBookCopy(tempBook);
            checkedOutCopy.AddBookCopy(tempBook);
            List<Copy> listOfAvailableCopies = new List<Copy>{tempCopy};
            Assert.Equal(listOfAvailableCopies, tempBook.CheckAvailabe());
        }

        public void Dispose()
        {
            Book.DeleteAll();
            Copy.DeleteAll();
            Copy.BC_DeleteAll();
            Book.BA_DeleteAll();
            Author.DeleteAll();
        }
    }
}
