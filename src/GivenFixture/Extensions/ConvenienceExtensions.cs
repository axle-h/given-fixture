using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Dsl;
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
        /// Creates an auto fixture constructed instance of the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <param name="composer">The composer.</param>
        /// <returns></returns>
        public static ITestFixture HavingModel<TModel>(this ITestFixture fixture,
                                                       out TModel model,
                                                       Func<IPostprocessComposer<TModel>, IPostprocessComposer<TModel>> composer = null)
        {
            model = fixture.GetComposer(composer).Create();
            return fixture;
        }

        /// <summary>
        /// Creates a collection of auto fixture constructed instances of the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="models">The models.</param>
        /// <param name="composer">The composer.</param>
        /// <returns></returns>
        public static ITestFixture HavingModels<TModel>(this ITestFixture fixture,
                                                        out ICollection<TModel> models,
                                                        Func<IPostprocessComposer<TModel>, IPostprocessComposer<TModel>> composer = null)
        {
            models = fixture.GetComposer(composer).CreateMany().ToList();
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

        private static IPostprocessComposer<TModel> GetComposer<TModel>(this ITestFixture fixture, 
                                                                        Func<IPostprocessComposer<TModel>, IPostprocessComposer<TModel>> composer)
        {
            var builder = fixture.AutoFixture.Build<TModel>();
            return composer == null ? builder : composer(builder);
        }
    }
}
