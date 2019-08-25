using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.Dsl;
using AutoFixture.Kernel;
using Bogus;

namespace GivenFixture.Extensions
{
    /// <summary>
    /// Extensions for AutoFixture.
    /// </summary>
    public static class AutoFixtureExtensions
    {
        /// <summary>
        /// Registers that a writable property should be assigned to the result of calling
        /// the specified factory function as part of specimen post-processing.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="composer">The composer.</param>
        /// <param name="property">The property.</param>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        public static IPostprocessComposer<TModel> With<TModel, TProperty>(this IPostprocessComposer<TModel> composer,
                                                                           Expression<Func<TModel, TProperty>> property,
                                                                           Func<TProperty> factory)
        {
            var info = property.GetProperty();
            return composer.Without(property).Do(x => info.SetValue(x, factory()));
        }

        /// <summary>
        /// Configures AutoFixture.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static ITestFixture HavingAutoFixture(this ITestFixture fixture, Action<IFixture> action)
        {
            action(fixture.AutoFixture);
            return fixture;
        }

        /// <summary>
        /// Adds the specified behavior to the auto fixture instance.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns></returns>
        public static ITestFixture HavingAutoFixtureBehavior(this ITestFixture fixture, ISpecimenBuilderTransformation behavior) =>
            fixture.HavingAutoFixture(f => f.Behaviors.Add(behavior));

        /// <summary>
        /// Adds the specified composer transformation function as an AutoFixture customization.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="composer">The composer.</param>
        /// <returns></returns>
        public static ITestFixture HavingAutoFixtureCustomization<TModel>(this ITestFixture fixture,
            Func<ICustomizationComposer<TModel>, ISpecimenBuilder> composer) =>
            fixture.HavingAutoFixture(f => f.Customize(composer));

        /// <summary>
        /// Adds the specified composer transformation function as an AutoFixture customization.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="composer">The composer.</param>
        /// <returns></returns>
        public static ITestFixture HavingAutoFixtureCustomization<TModel>(this ITestFixture fixture,
            Func<Faker, ICustomizationComposer<TModel>, ISpecimenBuilder> composer) =>
            fixture.HavingAutoFixture(f => f.Customize<TModel>(c => composer(fixture.Faker, c)));

        /// <summary>
        /// Adds the specified composer transformation function as an AutoFixture customization.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="specimenBuilder">The specimen builder.</param>
        /// <returns></returns>
        public static ITestFixture HavingAutoFixtureCustomization(this ITestFixture fixture,
            ISpecimenBuilder specimenBuilder) =>
            fixture.HavingAutoFixture(f => f.Customizations.Add(specimenBuilder));

        /// <summary>
        /// Adds the omit on recursion behaviour to AutoFixture.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <returns></returns>
        public static ITestFixture HavingRecursiveModelSupport(this ITestFixture fixture) =>
            fixture.HavingAutoFixtureBehavior(new OmitOnRecursionBehavior());

        /// <summary>
        /// Creates an auto fixture constructed instance of the specified model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="fixture">The fixture.</param>
        /// <param name="model">The model.</param>
        /// <param name="composer">The composer.</param>
        /// <returns></returns>
        public static ITestFixture HavingComposedModel<TModel>(this ITestFixture fixture,
            out TModel model,
            Func<IPostprocessComposer<TModel>, IPostprocessComposer<TModel>> composer)
        {
            model = composer == null
                ? fixture.AutoFixture.Create<TModel>()
                : composer(fixture.AutoFixture.Build<TModel>()).Create();
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
        public static ITestFixture HavingComposedModel<TModel>(this ITestFixture fixture,
            out TModel model,
            Func<Faker, IPostprocessComposer<TModel>, IPostprocessComposer<TModel>> composer)
        {
            model = composer(fixture.Faker, fixture.AutoFixture.Build<TModel>()).Create();
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
            Action<Faker, TModel> composer = null)
        {
            model = fixture.AutoFixture.Create<TModel>();
            composer?.Invoke(fixture.Faker, model);
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
        public static ITestFixture HavingComposedModels<TModel>(this ITestFixture fixture,
            out ICollection<TModel> models,
            Func<IPostprocessComposer<TModel>, IPostprocessComposer<TModel>> composer)
        {
            models = composer == null
                ? fixture.AutoFixture.CreateMany<TModel>().ToList()
                : composer(fixture.AutoFixture.Build<TModel>()).CreateMany().ToList();
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
        public static ITestFixture HavingComposedModels<TModel>(this ITestFixture fixture,
            out ICollection<TModel> models,
            Func<Faker, IPostprocessComposer<TModel>, IPostprocessComposer<TModel>> composer)
        {
            models = composer(fixture.Faker, fixture.AutoFixture.Build<TModel>()).CreateMany().ToList();
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
            Action<Faker, TModel> composer = null)
        {
            models = fixture.AutoFixture.CreateMany<TModel>().ToList();

            if (composer != null)
            {
                foreach (var model in models)
                {
                    composer(fixture.Faker, model);
                }
            }
            
            return fixture;
        }
    }
}
