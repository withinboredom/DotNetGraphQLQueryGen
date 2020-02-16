using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using dotnet_gqlgen;

/// <summary>
/// Generated interfaces for making GraphQL API calls with a typed interface.
///
/// Generated on 16/10/19 3:20:40 pm from ../tests/DotNetGraphQLQueryGen.Tests/schema.graphql
/// </summary>

namespace Generated
{
    public interface RootQuery
    {
        /// <summary>
        ///     <para>Pagination [defaults: page , pagesize ]</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("actorPager")]
        PersonPagination ActorPager();

        /// <summary>
        ///     Pagination [defaults: <paramref name="page" /> ,
        ///     <paramref name="pagesize" /> ]
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("actorPager")]
        TReturn ActorPager<TReturn>(
            int? page,
            int? pagesize,
            string search,
            Expression<Func<PersonPagination, TReturn>> selection
        );

        /// <summary>
        ///     <para><see cref="List`1" /> of actors</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("actors")]
        List<Person> Actors();

        /// <summary>
        ///     <see cref="List`1" /> of actors
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("actors")]
        List<TReturn> Actors<TReturn>(Expression<Func<Person, TReturn>> selection);

        /// <summary>
        ///     <para><see cref="List`1" /> of directors</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("directors")]
        List<Person> Directors();

        /// <summary>
        ///     <see cref="List`1" /> of directors
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("directors")]
        List<TReturn> Directors<TReturn>(Expression<Func<Person, TReturn>> selection);

        /// <summary>
        ///     <para>Return a Movie by its Id</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("movie")]
        Movie Movie();

        /// <summary>
        ///     Return a Movie by its Id
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("movie")]
        TReturn Movie<TReturn>(int id, Expression<Func<Movie, TReturn>> selection);

        /// <summary>
        ///     <para>Collection of <see cref="Movies" /></para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("movies")]
        List<Movie> Movies();

        /// <summary>
        ///     Collection of <see cref="Movies" />
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("movies")]
        List<TReturn> Movies<TReturn>(Expression<Func<Movie, TReturn>> selection);

        /// <summary>
        ///     <para>Collection of Peoples</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("people")]
        List<Person> People();

        /// <summary>
        ///     Collection of Peoples
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("people")]
        List<TReturn> People<TReturn>(Expression<Func<Person, TReturn>> selection);

        /// <summary>
        ///     <para>Return a Person by its Id</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("person")]
        Person Person();

        /// <summary>
        ///     Return a Person by its Id
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("person")]
        TReturn Person<TReturn>(int id, Expression<Func<Person, TReturn>> selection);

        /// <summary>
        ///     <para><see cref="List`1" /> of writers</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("writers")]
        List<Person> Writers();

        /// <summary>
        ///     <see cref="List`1" /> of writers
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("writers")]
        List<TReturn> Writers<TReturn>(Expression<Func<Person, TReturn>> selection);
    }

    public interface SubscriptionType
    {
        [GqlFieldName("name")] string Name { get; }
    }

    /// <summary>
    ///     This is a movie entity
    /// </summary>
    public interface Movie
    {
        [GqlFieldName("id")] int Id { get; }

        [GqlFieldName("name")] string Name { get; }

        /// <summary>
        ///     <see cref="Enum" /> of <see cref="Genre" />
        /// </summary>
        [GqlFieldName("genre")]
        int Genre { get; }

        [GqlFieldName("released")] DateTime Released { get; }

        [GqlFieldName("directorId")] int DirectorId { get; }

        [GqlFieldName("rating")] double Rating { get; }

        /// <summary>
        ///     <para><see cref="Actors" /> in the movie</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("actors")]
        List<Person> Actors();

        /// <summary>
        ///     <see cref="Actors" /> in the movie
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("actors")]
        List<TReturn> Actors<TReturn>(Expression<Func<Person, TReturn>> selection);

        /// <summary>
        ///     <para><see cref="Writers" /> in the movie</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("writers")]
        List<Person> Writers();

        /// <summary>
        ///     <see cref="Writers" /> in the movie
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("writers")]
        List<TReturn> Writers<TReturn>(Expression<Func<Person, TReturn>> selection);

        /// <summary>
        ///     This shortcut will return a selection of all fields
        /// </summary>
        [GqlFieldName("director")]
        Person Director();

        /// <summary>
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("director")]
        TReturn Director<TReturn>(Expression<Func<Person, TReturn>> selection);
    }

    public interface Actor
    {
        [GqlFieldName("personId")] int PersonId { get; }

        [GqlFieldName("movieId")] int MovieId { get; }

        /// <summary>
        ///     This shortcut will return a selection of all fields
        /// </summary>
        [GqlFieldName("person")]
        Person Person();

        /// <summary>
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("person")]
        TReturn Person<TReturn>(Expression<Func<Person, TReturn>> selection);

        /// <summary>
        ///     This shortcut will return a selection of all fields
        /// </summary>
        [GqlFieldName("movie")]
        Movie Movie();

        /// <summary>
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("movie")]
        TReturn Movie<TReturn>(Expression<Func<Movie, TReturn>> selection);
    }

    public interface Person
    {
        [GqlFieldName("id")] int Id { get; }

        [GqlFieldName("firstName")] string FirstName { get; }

        [GqlFieldName("lastName")] string LastName { get; }

        [GqlFieldName("dob")] DateTime Dob { get; }

        [GqlFieldName("died")] DateTime Died { get; }

        [GqlFieldName("isDeleted")] bool IsDeleted { get; }

        /// <summary>
        ///     Show the persons age
        /// </summary>
        [GqlFieldName("age")]
        int Age { get; }

        /// <summary>
        ///     Persons name
        /// </summary>
        [GqlFieldName("name")]
        string Name { get; }

        /// <summary>
        ///     <para>Movies they acted in</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("actorIn")]
        List<Movie> ActorIn();

        /// <summary>
        ///     Movies they acted in
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("actorIn")]
        List<TReturn> ActorIn<TReturn>(Expression<Func<Movie, TReturn>> selection);

        /// <summary>
        ///     <para>Movies they wrote</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("writerOf")]
        List<Movie> WriterOf();

        /// <summary>
        ///     Movies they wrote
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("writerOf")]
        List<TReturn> WriterOf<TReturn>(Expression<Func<Movie, TReturn>> selection);

        /// <summary>
        ///     This shortcut will return a selection of all fields
        /// </summary>
        [GqlFieldName("directorOf")]
        List<Movie> DirectorOf();

        /// <summary>
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("directorOf")]
        List<TReturn> DirectorOf<TReturn>(Expression<Func<Movie, TReturn>> selection);
    }

    public interface Writer
    {
        [GqlFieldName("personId")] int PersonId { get; }

        [GqlFieldName("movieId")] int MovieId { get; }

        /// <summary>
        ///     This shortcut will return a selection of all fields
        /// </summary>
        [GqlFieldName("person")]
        Person Person();

        /// <summary>
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("person")]
        TReturn Person<TReturn>(Expression<Func<Person, TReturn>> selection);

        /// <summary>
        ///     This shortcut will return a selection of all fields
        /// </summary>
        [GqlFieldName("movie")]
        Movie Movie();

        /// <summary>
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("movie")]
        TReturn Movie<TReturn>(Expression<Func<Movie, TReturn>> selection);
    }

    public interface PersonPagination
    {
        /// <summary>
        ///     total records to match search
        /// </summary>
        [GqlFieldName("total")]
        int Total { get; }

        /// <summary>
        ///     total pages based on page size
        /// </summary>
        [GqlFieldName("pageCount")]
        int PageCount { get; }

        /// <summary>
        ///     <para>collection of people</para>
        ///     <para>This shortcut will return a selection of all fields</para>
        /// </summary>
        [GqlFieldName("people")]
        List<Person> People();

        /// <summary>
        ///     collection of people
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("people")]
        List<TReturn> People<TReturn>(Expression<Func<Person, TReturn>> selection);
    }

    public interface Mutation
    {
        /// <summary>
        ///     Add a new <see cref="Movie" /> object
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("addMovie")]
        TReturn AddMovie<TReturn>(
            string name,
            double? rating,
            List<Detail> details,
            int? genre,
            DateTime? released,
            Expression<Func<Movie, TReturn>> selection
        );

        /// <summary>
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("addActor")]
        TReturn AddActor<TReturn>(
            string firstName,
            string lastName,
            int? movieId,
            Expression<Func<Person, TReturn>> selection
        );

        /// <summary>
        /// </summary>
        /// <param name="selection">
        ///     Projection of fields to select from the object
        /// </param>
        [GqlFieldName("addActor2")]
        TReturn AddActor2<TReturn>(
            string firstName,
            string lastName,
            int? movieId,
            Expression<Func<Person, TReturn>> selection
        );
    }

    public class Detail
    {
        [GqlFieldName("description")] public string Description { get; set; }
    }
}
