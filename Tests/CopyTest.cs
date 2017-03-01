using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using LibraryApp.Objects;

namespace LibraryApp
{
    public class CopyTest : IDisposable
    {
        public CopyTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void TEST_Save_CheckCopySaveToDB()
        {
            Copy testCopy = new Copy ("Harry Plopper");
            testCopy.Save();

            List<Copy> tempList = new List<Copy>{testCopy};

            Assert.Equal(tempList, Copy.GetAll());
        }

        [Fact]
        public void TEST_Find_ReturnsCopy()
        {
            Copy tempCopy = new Copy("Math");
            tempCopy.Save();

            Assert.Equal(tempCopy, Copy.Find(tempCopy.GetId()));
        }

        [Fact]
        public void TEST_AddBookCopy_AddSingleBookCopy()
        {
            Copy tempCopy = new Copy("Twilight");
            Book tempBook = new Book("Twilight");
            tempCopy.Save();
            tempBook.Save();
            tempCopy.AddBookCopy(tempBook);
            Book foundBook = tempCopy.GetBook();
            Assert.Equal(tempBook, foundBook);
        }

        public void Dispose()
        {
            Book.DeleteAll();
            Book.BA_DeleteAll();
            Author.DeleteAll();
            Copy.DeleteAll();
        }
    }
}
