using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using PersonManagerApi.Controllers;
using PersonManagerApi.Data;
using PersonManagerApi.Models;

namespace PersonManagerApiTests
{
    public class PersonControllerTests
    {
        [Fact]
        public async Task GetPersons_Returns_PersonsListAsync()
        {
            // Arrange 
            var persons = new List<Person>();
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "PersonManagerTestDatabase")
                .Options;

            using (var context = new DataContext(options))
            {
                context.Database.EnsureDeleted();
                foreach (Person person in GetFakePersonsList())
                {
                    context.Persons.Add(person);
                }

                context.SaveChanges();
            }

            using (var context = new DataContext(options))
            {
                // Act
                PersonController personController = new PersonController(context);
                persons = (await personController.GetPersons()).Value as List<Person>;

                // Assert
                Assert.NotNull(persons);
                Assert.Equal(3, persons.Count());
            }

        }

        [Fact]
        public async Task GetPersons_Returns_PersonsListAsync_When_Used_With_Param()
        {
            // Arrange 
            var persons = new List<Person>();
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "PersonManagerTestDatabase")
                .Options;

            using (var context = new DataContext(options))
            {
                context.Database.EnsureDeleted();
                foreach (Person person in GetFakePersonsList())
                {
                    context.Persons.Add(person);
                }

                context.SaveChanges();
            }

            using (var context = new DataContext(options))
            {
                // Act
                PersonController personController = new PersonController(context);
                persons = (await personController.GetPersons("John")).Value as List<Person>;

                // Assert
                Assert.NotNull(persons);
                Assert.Equal(2, persons.Count());
            }
        }

        [Fact]
        public async Task GetPersons_Returns_Empty_When_Empty_Db()
        {
            // Arrange 
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "PersonManagerTestDatabase")
                .Options;

            using (var context = new DataContext(options))
            {
                context.Database.EnsureDeleted();

                // Act
                PersonController personController = new PersonController(context);
                var result = (await personController.GetPersons());

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result.Value);
            }
        }

        [Fact]
        public async Task GetPerson_Returns_PersonAsync()
        {
            // Arrange 
            var personReturned = new Person();
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "PersonManagerTestDatabase")
                .Options;

            using (var context = new DataContext(options))
            {
                context.Database.EnsureDeleted();
                foreach (Person person in GetFakePersonsList())
                {
                    context.Persons.Add(person);
                }

                context.SaveChanges();
            }

            using (var context = new DataContext(options))
            {
                // Act
                PersonController personController = new PersonController(context);
                personReturned = (await personController.GetPerson(new Guid("b0452eaf-537d-4221-84d9-2252f8bc5aef"))).Value as Person;

                // Assert
                Assert.NotNull(personReturned);
                Assert.Equal("Luke", personReturned.FirstName);
            }
        }

        [Fact]
        public async Task PutPerson_Updates_Person()
        {
            // Arrange 
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "PersonManagerTestDatabase")
                .Options;

            using (var context = new DataContext(options))
            {
                context.Database.EnsureDeleted();
                foreach (Person person in GetFakePersonsList())
                {
                    context.Persons.Add(person);
                }

                context.SaveChanges();
            }

            using (var context = new DataContext(options))
            {
                // Act
                PersonController personController = new PersonController(context);
                var result = (await personController.PutPerson(
                    new Guid("b0452eaf-537d-4221-84d9-2252f8bc5aef"),
                    new Person
                    {
                        Id = new Guid("b0452eaf-537d-4221-84d9-2252f8bc5aef"),
                        FirstName = "Luke",
                        LastName = "Lars"
                    }    
                ));
                var updatedPerson = context.Persons.Find(new Guid("b0452eaf-537d-4221-84d9-2252f8bc5aef"));

                // Assert
                Assert.NotNull(result);
                Assert.IsType<NoContentResult>(result);
                Assert.Equal("Lars", updatedPerson.LastName);
            }
        }

        [Fact]
        public async Task PostPerson_Creates_Person()
        {
            // Arrange 
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "PersonManagerTestDatabase")
                .Options;

            using (var context = new DataContext(options))
            {
                context.Database.EnsureDeleted();
                foreach (Person person in GetFakePersonsList())
                {
                    context.Persons.Add(person);
                }

                context.SaveChanges();
            }

            using (var context = new DataContext(options))
            {
                // Act
                PersonController personController = new PersonController(context);
                var result = (await personController.PostPerson(
                    new Person
                    {
                        Id = new Guid("bea1522b-fcc3-4f84-a021-9875d7bb2bbd"),
                        FirstName = "Leia",
                        LastName = "Organa"
                    }    
                ));
                var createdPerson = context.Persons.Find(new Guid("bea1522b-fcc3-4f84-a021-9875d7bb2bbd"));

                // Assert
                Assert.NotNull(result);
                Assert.Equal(4, context.Persons.Count());
                Assert.IsType<CreatedAtActionResult>(result.Result);
                Assert.Equal("Leia", createdPerson.FirstName);
            }
        }
        
        [Fact]
        public async Task DeletePerson_Deletes_Person()
        {
            // Arrange 
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "PersonManagerTestDatabase")
                .Options;

            using (var context = new DataContext(options))
            {
                context.Database.EnsureDeleted();
                foreach (Person person in GetFakePersonsList())
                {
                    context.Persons.Add(person);
                }

                context.SaveChanges();
            }

            using (var context = new DataContext(options))
            {
                // Act
                PersonController personController = new PersonController(context);
                var result = (await personController.DeletePerson(new Guid("b0452eaf-537d-4221-84d9-2252f8bc5aef")));
                var deletedPerson = context.Persons.Find(new Guid("b0452eaf-537d-4221-84d9-2252f8bc5aef"));

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, context.Persons.Count());
                Assert.IsType<NoContentResult>(result);
                Assert.Null(deletedPerson);
            }
        }


        private static List<Person> GetFakePersonsList()
        {
            return new List<Person>()
            {
                new Person
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe"
                },
                new Person
                {
                    Id = new Guid("b0452eaf-537d-4221-84d9-2252f8bc5aef"),
                    FirstName = "Luke",
                    LastName = "Skywalker"
                },
                new Person
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Jean"
                }
            };
        }
    }
}