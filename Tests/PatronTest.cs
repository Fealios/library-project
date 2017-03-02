using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using LibraryApp.Objects;

namespace LibraryApp
{
    public class PatronTest : IDisposable
    {
        public PatronTest()
        {
            DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
        }

        [Fact]
        public void TEST_Save_CheckPatronSaveToDB()
        {
            Patron testPatron = new Patron ("John Smith");
            testPatron.Save();

            List<Patron> tempList = new List<Patron>{testPatron};

            Assert.Equal(tempList, Patron.GetAll());
        }

        [Fact]
        public void TEST_GetCheckouts_GetListOfCheckouts()
        {
            Patron testPatron = new Patron("Alex");
            testPatron.Save();
            Copy testCopy = new Copy("Melvin Goes to the zoo");
            testCopy.Save();
            testPatron.CheckoutCopy(testCopy);
            List<Checkout> checkoutList = testPatron.GetCheckouts();
            // Console.WriteLine(checkoutList[0].GetDueDate());
            Assert.Equal("3/15/2017 12:00:00 AM", checkoutList[0].GetDueDate());
        }


        public void Dispose()
        {
            Book.DeleteAll();
            Patron.PC_DeleteAll();
            Author.DeleteAll();
            Copy.DeleteAll();
            Patron.DeleteAll();
        }
    }
}
