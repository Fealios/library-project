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

        public void Dispose()
        {
            Book.DeleteAll();
            Book.BA_DeleteAll();
            Author.DeleteAll();
        }
    }
}
