using System;
using System.Collections.Generic;
using Bogus;

namespace GivenFixture.Extensions
{
    /// <summary>
    /// Convenience extensions for <see cref="ITestFixture"/>.
    /// </summary>
    public static class ConvenienceExtensions
    {
        /// <summary>
        /// Performs an arrange action.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static ITestFixture Having(this ITestFixture fixture, Action action)
        {
            action();
            return fixture;
        }

        /// <summary>
        /// Performs an arrange action and returns the result as an output parameter.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static ITestFixture Having<TResult>(this ITestFixture fixture, Func<TResult> factory, out TResult result)
        {
            result = factory();
            return fixture;
        }

        /// <summary>
        /// Picks a random model from the specified collection.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="models">The models.</param>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static ITestFixture HavingRandom<TModel>(this ITestFixture fixture,
                                                        ICollection<TModel> models,
                                                        out TModel model)
        {
            model = fixture.Faker.Random.CollectionItem(models);
            return fixture;
        }

        /// <summary>
        /// Uses the faker on the fixture as a factory for some fake data.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="fake">The fake.</param>
        /// <returns></returns>
        public static ITestFixture HavingFake<TFake>(this ITestFixture fixture, Func<Faker, TFake> factory, out TFake fake)
        {
            fake = factory(fixture.Faker);
            return fixture;
        }
    }
}
