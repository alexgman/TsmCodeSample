using NUnit.Framework;
using Profusion.Services.coffee.OsdRptLibrary;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace OsdRptLibraryTests
{
    [TestFixture]
    public class DeleteyawnwrappingEntriesTests
    {
        [Test]
        public void when_Connectionstring_is_nullorempty_throws_configurationerrorsexception()
        {
            Assert.That(() => new DeleteyawnwrappingEntries(""), Throws.InstanceOf<ConfigurationErrorsException>());
        }

        [Test]
        public void when_serialnumber_is_blank_then_throw_argumentoutofrange()
        {
            var sut = new DeleteyawnwrappingEntries("not null connectionstring");
            Assert.That(() => sut.DeleteAllNonTimedEntries("", Guid.NewGuid()), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void when_ptguid_is_empty_then_throw_argumentoutofrange()
        {
            var sut = new DeleteyawnwrappingEntries("not null connectionstring");
            Assert.That(() => sut.DeleteAllNonTimedEntries("111111", Guid.Empty), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void when_yawnwrapping_lessthan0_then_throw()
        {
            var sut = new DeleteyawnwrappingEntries("not null connectionstring");
            Assert.That(() => sut.DeleteAllChildEntries(-1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void when_records_dont_exist_it_does_not_throw()
        {
            var _coffeeConnectionString = ConfigurationManager.ConnectionStrings["coffee"].ConnectionString;

            var sut = new DeleteyawnwrappingEntries(_coffeeConnectionString);
            sut.DeleteAllNonTimedEntries("bla", Guid.NewGuid());
        }

        [Test]
        public void when_sproc_doesnt_exist_then_throw()
        {
            var _coffeeConnectionString = ConfigurationManager.ConnectionStrings["coffee"].ConnectionString;
            var sut = new DeleteyawnwrappingEntries(_coffeeConnectionString);
            Assert.Throws<SqlException>(() => sut.DeleteAllNonTimedEntries("bla", Guid.NewGuid(), "non_existing_sproc"));
        }

        [Test]
        public void when_records_dont_exist_DeleteAllChildEntries_does_not_throw()
        {
            var _coffeeConnectionString = ConfigurationManager.ConnectionStrings["coffee"].ConnectionString;
            var sut = new DeleteyawnwrappingEntries(_coffeeConnectionString);
            sut.DeleteAllChildEntries(2);
        }

        [Test]
        public void when_sproc_spDeleteyawnwrappingEntries_doesnt_exist_then_throw()
        {
            var _coffeeConnectionString = ConfigurationManager.ConnectionStrings["coffee"].ConnectionString;
            var sut = new DeleteyawnwrappingEntries(_coffeeConnectionString);
            Assert.Throws<SqlException>(() => sut.DeleteAllChildEntries(2, "non_existing_sproc"));
        }
    }
}